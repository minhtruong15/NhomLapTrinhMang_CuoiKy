using System;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;

namespace ServerApp
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private string _username;
        private string _role;

        public static readonly List<ClientHandler> Connected = new List<ClientHandler>();

        public Action<object, object> OnLoginSuccess { get; internal set; }
        public Action<object, object, object> OnMessage { get; internal set; }
        public Action<object> OnDisconnected { get; internal set; }

        public ClientHandler(TcpClient c)
        {
            _client = c;
            _stream = c.GetStream();

            lock (Connected)
            {
                Connected.Add(this);
            }

            Task.Run((Func<Task>)Handle);
        }

        // ====== HÀM ĐỌC ĐỦ SỐ BYTE YÊU CẦU (QUAN TRỌNG) ======
        private async Task ReadExactAsync(NetworkStream stream, byte[] buffer, int length)
        {
            int offset = 0;
            while (offset < length)
            {
                int read = await stream.ReadAsync(buffer, offset, length - offset);
                if (read == 0)
                {
                    // client đóng kết nối giữa chừng
                    throw new Exception("Client disconnected");
                }
                offset += read;
            }
        }

        // ======================================================
        // VÒNG LẶP NHẬN PACKET
        // ======================================================
        private async Task Handle()
        {
            try
            {
                while (true)
                {
                    // 1. đọc đúng 4 byte độ dài
                    byte[] lenB = new byte[4];
                    await ReadExactAsync(_stream, lenB, 4);

                    int len = BitConverter.ToInt32(lenB, 0);
                    if (len <= 0 || len > 20 * 1024 * 1024) // giới hạn 20MB cho chắc
                        throw new Exception("Invalid packet length");

                    // 2. đọc đúng len byte dữ liệu
                    byte[] data = new byte[len];
                    await ReadExactAsync(_stream, data, len);

                    string json = Encoding.UTF8.GetString(data);
                    Packet p = Packet.FromJson(json);

                    await Process(p);
                }
            }
            catch
            {
                // có thể log ra nếu muốn
            }
            finally
            {
                Disconnect();
            }
        }

        private async Task Process(Packet p)
        {
            switch (p.Cmd)
            {
                case Command.AuthLogin:
                    {
                        dynamic d = p.Data;
                        var u = Database.Authenticate((string)d.username, (string)d.password);

                        if (u != null)
                        {
                            _username = u.Item1;
                            _role = u.Item2;

                            await Send(new Packet(Command.AuthOk, new
                            {
                                username = _username,
                                role = _role
                            }));

                            OnLoginSuccess?.Invoke(_username, _role);

                            await BroadcastUserListsAsync();
                        }
                        else
                        {
                            await Send(new Packet(Command.AuthFail, new
                            {
                                reason = "Sai tài khoản hoặc mật khẩu"
                            }));
                        }
                        break;
                    }

                case Command.AuthRegister:
                    {
                        dynamic d = p.Data;
                        bool ok = Database.Register((string)d.username, (string)d.password);

                        if (ok)
                            await Send(new Packet(Command.AuthOk, new { username = (string)d.username, role = "user" }));
                        else
                            await Send(new Packet(Command.AuthFail, new { reason = "Tên đã tồn tại" }));

                        break;
                    }

                case Command.RequestFriendList:
                    {
                        await SendUserList();
                        break;
                    }

                case Command.PrivateMessage:
                    {
                        dynamic d = p.Data;

                        string to = d.to;
                        string payload = d.payload;
                        string time = DateTime.Now.ToString("HH:mm:ss");

                        Database.SavePrivateMessage(_username, to, payload);

                        var messagePacket = new Packet(Command.PrivateMessage, new
                        {
                            from = _username,
                            to = to,
                            payload = payload,
                            time = time
                        });

                        var target = FindConnected(to);
                        if (target != null)
                        {
                            await target.Send(messagePacket);
                        }

                        await Send(messagePacket);
                        OnMessage?.Invoke(_username, to, payload);

                        break;
                    }

                case Command.RequestPrivateHistory:
                    {
                        dynamic d = p.Data;
                        string withUser = d.with;

                        var history = Database.GetPrivateHistory(_username, withUser);

                        await Send(new Packet(Command.PrivateHistory, new
                        {
                            messages = history
                        }));

                        break;
                    }
            }
        }

        private async Task SendUserList()
        {
            List<string> users = Database.GetAllUsers();

            users.Remove(_username);
            users.RemoveAll(u => u.Equals("admin", StringComparison.OrdinalIgnoreCase));

            await Send(new Packet(Command.UpdateFriendList, new
            {
                users = users
            }));
        }

        public async Task Send(Packet p)
        {
            string json = p.ToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] len = BitConverter.GetBytes(data.Length);

            await _stream.WriteAsync(len, 0, 4);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        private void Disconnect()
        {
            try
            {
                if (_username != null)
                    OnDisconnected?.Invoke(_username);

                _stream.Close();
                _client.Close();
            }
            catch { }

            bool removed = false;
            lock (Connected)
            {
                removed = Connected.Remove(this);
            }

            if (removed)
            {
                _ = BroadcastUserListsAsync();
            }
        }

        private static ClientHandler FindConnected(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            lock (Connected)
            {
                return Connected.FirstOrDefault(x =>
                    string.Equals(x._username, username, StringComparison.OrdinalIgnoreCase));
            }
        }

        private static async Task BroadcastUserListsAsync()
        {
            List<ClientHandler> snapshot;

            lock (Connected)
            {
                snapshot = Connected.ToList();
            }

            if (snapshot.Count == 0)
                return;

            var allUsers = Database.GetAllUsers();

            foreach (var handler in snapshot)
            {
                if (handler._username == null) continue;

                var users = allUsers
                    .Where(u => !u.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    .Where(u => !string.Equals(u, handler._username, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                await handler.Send(new Packet(Command.UpdateFriendList, new
                {
                    users = users
                }));
            }
        }
    }
}
