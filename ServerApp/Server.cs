using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerApp
{
    public class Server
    {
        private TcpListener _listener;
        private bool _isRunning = false;

        public static event Action<string> OnServerLog;

        // ======================================================
        // START SERVER
        // ======================================================
        public async void Start(int port = 5000)
        {
            if (_isRunning) return;

            _isRunning = true;
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Log($"🚀 Server started at port {port}");
            Log("⏳ Waiting for clients...\n");

            while (_isRunning)
            {
                TcpClient c = await _listener.AcceptTcpClientAsync();
                AttachEvents(new ClientHandler(c));
            }
        }

        // ======================================================
        // GẮN SỰ KIỆN TỪ CLIENT → SERVER LOG
        // ======================================================
        private void AttachEvents(ClientHandler handler)
        {
            // Set event khi login thành công
            handler.OnLoginSuccess = (user, role) =>
            {
                Log($"🟢 [LOGIN] {user} logged in as {role}.");
            };

            // Set event khi nhận tin nhắn
            handler.OnMessage = (from, to, msg) =>
            {
                Log($"💬 [MESSAGE] {from} → {to}: {msg}");
            };

            // Set event khi disconnect
            handler.OnDisconnected = (user) =>
            {
                Log($"🔴 [LEFT] {user} disconnected.");
            };
        }

        // ======================================================
        // STOP SERVER
        // ======================================================
        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _listener.Stop();

            Log("🛑 Server stopped.\n");
        }

        private void Log(string msg)
        {
            Console.WriteLine(msg);
            OnServerLog?.Invoke(msg);
        }
    }
}
