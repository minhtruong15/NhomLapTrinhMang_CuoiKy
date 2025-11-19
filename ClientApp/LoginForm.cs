using Shared;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class LoginForm : Form
    {
        private readonly ChatClient _client = new ChatClient();

        public LoginForm()
        {
            InitializeComponent();

            _client.OnPacketReceived += HandlePacket;
            _client.OnDisconnected += () =>
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    lblStatus.Text = "Mất kết nối server!";
                    btnLogin.Enabled = true;
                }));
            };

            NotificationHelper.Initialize();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Vui lòng nhập tài khoản và mật khẩu.");
                return;
            }

            btnLogin.Enabled = false;
            lblStatus.Text = "Đang kết nối server...";

            // 👉 FIX QUAN TRỌNG — đảm bảo kết nối lại
            if (!await _client.EnsureConnectedAsync())
            {
                lblStatus.Text = "Không thể kết nối server!";
                btnLogin.Enabled = true;

                NotificationHelper.ShowErrorNotification("Lỗi kết nối",
                    "Không thể kết nối đến server. Hãy kiểm tra server có chạy không.");

                return;
            }

            var packet = new Packet(Command.AuthLogin, new
            {
                username = txtUser.Text,
                password = txtPass.Text
            });

            await _client.SendAsync(packet);
        }

        private void HandlePacket(Packet p)
        {
            this.Invoke((MethodInvoker)(async () =>
            {
                switch (p.Cmd)
                {
                    case Command.AuthOk:
                        dynamic d = p.Data;
                        string username = d.username;
                        string role = d.role;

                        lblStatus.Text = "Đăng nhập thành công!";
                        NotificationHelper.ShowSuccessNotification("Đăng nhập thành công", $"Chào mừng {username}!");

                        await Task.Delay(300);

                        if (role == "admin")
                            new AdminForm(_client, username).Show();
                        else
                            new RoomForm(_client, username).Show();

                        this.Hide();
                        break;

                    case Command.AuthFail:
                        lblStatus.Text = "Sai tài khoản hoặc mật khẩu!";
                        btnLogin.Enabled = true;

                        NotificationHelper.ShowErrorNotification("Đăng nhập thất bại",
                            "Sai tài khoản hoặc mật khẩu. Vui lòng thử lại.");

                        break;
                }
            }));
        }

        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new RegisterForm().Show();
            this.Hide();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }
    }
}
