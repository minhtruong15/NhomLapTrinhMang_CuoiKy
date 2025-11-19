using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ClientApp
{
    public class ChatClient
    {
        private TcpClient _tcp;
        private NetworkStream _stream;

        public event Action<Packet> OnPacketReceived;
        public event Action OnDisconnected;   // 👉 báo cho UI biết đã mất kết nối

        public bool IsConnected => _tcp != null && _tcp.Connected && _stream != null;

        // ============================
        // KẾT NỐI / KẾT NỐI LẠI
        // ============================
        public async Task<bool> ConnectAsync(string host = "127.0.0.1", int port = 5000)
        {
            try
            {
                // nếu đang còn kết nối cũ thì đóng lại cho sạch
                Close();

                _tcp = new TcpClient();
                await _tcp.ConnectAsync(host, port);
                _stream = _tcp.GetStream();

                _ = Task.Run(ReceiveLoop);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT ERROR] {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Đảm bảo đã kết nối – nếu chưa thì tự connect.
        /// Gọi hàm này TRƯỚC khi gửi gói AuthLogin ở LoginForm.
        /// </summary>
        public async Task<bool> EnsureConnectedAsync(string host = "127.0.0.1", int port = 5000)
        {
            if (IsConnected) return true;
            return await ConnectAsync(host, port);
        }

        // ============================
        // ĐỌC ĐỦ SỐ BYTE (rất quan trọng)
        // ============================
        private async Task ReadExactAsync(byte[] buffer, int length)
        {
            int offset = 0;
            while (offset < length)
            {
                int read = await _stream.ReadAsync(buffer, offset, length - offset);
                if (read == 0)
                    throw new Exception("Server disconnected");

                offset += read;
            }
        }

        // ============================
        // RECEIVE LOOP CHUẨN TCP
        // ============================
        private async Task ReceiveLoop()
        {
            try
            {
                while (_tcp != null && _tcp.Connected)
                {
                    // 1. đọc 4 byte đầu để biết packet length
                    byte[] lenB = new byte[4];
                    await ReadExactAsync(lenB, 4);

                    int len = BitConverter.ToInt32(lenB, 0);
                    if (len <= 0 || len > 20 * 1024 * 1024)
                        throw new Exception("Invalid packet length");

                    // 2. đọc đúng len byte nội dung
                    byte[] data = new byte[len];
                    await ReadExactAsync(data, len);

                    string json = Encoding.UTF8.GetString(data);
                    Packet packet = Packet.FromJson(json);

                    OnPacketReceived?.Invoke(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Mất kết nối server: {ex.Message}");
            }
            finally
            {
                // đảm bảo resource được giải phóng & báo cho UI
                Close();
                OnDisconnected?.Invoke();
            }
        }

        // ============================
        // GỬI GÓI TIN CHUẨN TCP
        // ============================
        public async Task SendAsync(Packet packet)
        {
            try
            {
                if (!IsConnected) return;

                string json = packet.ToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);
                byte[] len = BitConverter.GetBytes(data.Length);

                await _stream.WriteAsync(len, 0, 4);
                await _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT SEND ERROR] {ex.Message}");
            }
        }

        public void Close()
        {
            try
            {
                _stream?.Close();
                _tcp?.Close();
            }
            catch { }
            finally
            {
                _stream = null;
                _tcp = null;
            }
        }
    }
}
