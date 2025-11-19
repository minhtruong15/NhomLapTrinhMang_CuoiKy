using System;
using System.Windows.Forms;
using Shared;

namespace ClientApp
{
    public partial class RegisterForm : Form
    {
        private readonly ChatClient _client = new ChatClient();

        public RegisterForm()
        {
            InitializeComponent();
            _client.OnPacketReceived += HandlePacket;
            
            // Khởi tạo notification helper
            NotificationHelper.Initialize();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu.");
                return;
            }

            if (!await _client.ConnectAsync())
            {
                lblStatus.Text = "Không thể kết nối server!";
                NotificationHelper.ShowErrorNotification("Lỗi kết nối", "Không thể kết nối đến server. Vui lòng kiểm tra server đã chạy chưa.");
                return;
            }

            var packet = new Shared.Packet(Shared.Command.AuthRegister, new { username = txtUser.Text, password = txtPass.Text });
            await _client.SendAsync(packet);
        }

        private void HandlePacket(Shared.Packet p)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                switch (p.Cmd)
                {
                    case Shared.Command.AuthOk:
                        // Hiển thị thông báo thành công
                        NotificationHelper.ShowSuccessNotification("Đăng ký thành công", "Bạn có thể đăng nhập ngay bây giờ!");
                        MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        new LoginForm().Show();
                        this.Hide();
                        break;

                    case Shared.Command.AuthFail:
                        dynamic data = p.Data;
                        string reason = data?.reason ?? "Tên người dùng đã tồn tại!";
                        lblStatus.Text = reason;
                        
                        // Hiển thị thông báo lỗi
                        NotificationHelper.ShowErrorNotification("Đăng ký thất bại", reason);
                        break;
                }
            }));
        }

        private void linkBack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new LoginForm().Show();
            this.Hide();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }
    }
}
