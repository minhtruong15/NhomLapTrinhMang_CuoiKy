using System;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class ServerMainForm : Form
    {
        private Server server;

        public ServerMainForm()
        {
            InitializeComponent();
            server = new Server();

            // Khi server gửi log, hiển thị ra textbox
            Server.OnServerLog += AddLog;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start(5000);
            AddLog("Server khởi động thành công.");
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            AddLog("Server đã dừng.");
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void AddLog(string msg)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AddLog(msg)));
                return;
            }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
        }
    }
}
