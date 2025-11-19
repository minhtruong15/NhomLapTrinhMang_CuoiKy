using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Shared;

namespace ClientApp
{
    public partial class RoomForm : Form
    {
        private ChatClient _client;
        private string _username;
        private Dictionary<string, ChatForm> _openChats = new Dictionary<string, ChatForm>();

        public RoomForm(ChatClient client, string username)
        {
            InitializeComponent();
            _client = client;
            _username = username;

            lblWelcome.Text = "Xin chào, " + _username;

            // Bảo đảm không đăng ký trùng sự kiện
            _client.OnPacketReceived -= HandlePacket;
            _client.OnPacketReceived += HandlePacket;

            // Gửi yêu cầu danh sách bạn bè
            _client.SendAsync(new Packet(Command.RequestFriendList, new { }));

            // Sự kiện mở cửa sổ chat khi double click
            lstUsers.DoubleClick += new EventHandler(lstUsers_DoubleClick);

            // Placeholder cho ô tìm kiếm
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;
            txtSearch.Text = "Nhập tên người dùng...";
            txtSearch.ForeColor = System.Drawing.Color.Gray;
            
            // Khởi tạo notification helper
            NotificationHelper.Initialize();
        }

        // 🎨 Placeholder - focus
        private void TxtSearch_GotFocus(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Nhập tên người dùng...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = System.Drawing.Color.Black;
            }
        }

        // 🎨 Placeholder - mất focus
        private void TxtSearch_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập tên người dùng...";
                txtSearch.ForeColor = System.Drawing.Color.Gray;
            }
        }

        // 🧠 Xử lý gói tin đến từ server
        private void HandlePacket(Packet p)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Packet>(HandlePacket), p);
                return;
            }

            try
            {
                switch (p.Cmd)
                {
                    case Command.UpdateFriendList:
                        {
                            dynamic d = p.Data;
                            lstUsers.Items.Clear();

                            foreach (var userObj in d.users)
                            {
                                string name = userObj.ToString();
                                if (name != _username && name.ToLower() != "admin")
                                    lstUsers.Items.Add(name);
                            }

                            lblCount.Text = "Tổng số người dùng: " + lstUsers.Items.Count;
                            break;
                        }

                    case Command.PrivateMessage:
                        {
                            dynamic d = p.Data;
                            string from = d.from;
                            string to = d.to;
                            string payload = d.payload ?? d.text;
                            string time = d.time;

                            // 🧩 Chỉ xử lý tin nhắn gửi cho mình
                            if (to != _username) return;

                            // ✅ Nếu ChatForm đang mở → KHÔNG append ở đây (ChatForm tự xử lý)
                            if (_openChats.ContainsKey(from) && !_openChats[from].IsDisposed)
                            {
                                // Vẫn phát âm thanh và thông báo
                                SoundHelper.PlayReceiveSound();
                                var previewContent = MessageContent.Deserialize(payload);
                                string preview = previewContent.IsImage ? "[Ảnh]" : previewContent.Text;
                            NotificationHelper.ShowReceiveNotification(from, preview);
                                return;
                            }

                            // 🟡 Nếu chưa mở → gắn tag [Tin mới]
                            for (int i = 0; i < lstUsers.Items.Count; i++)
                            {
                                string item = lstUsers.Items[i].ToString();
                                if (item == from || item.StartsWith(from + " "))
                                {
                                    lstUsers.Items[i] = from + " [Tin mới]";
                                }
                            }
                            
                            // Phát âm thanh khi nhận tin nhắn
                            SoundHelper.PlayReceiveSound();
                            
                            // Hiển thị thông báo khi nhận tin nhắn
                            var content = MessageContent.Deserialize(payload);
                            string shortText = content.IsImage ? "[Ảnh]" : content.Text;
                            NotificationHelper.ShowReceiveNotification(from, shortText);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý gói tin: " + ex.Message);
            }
        }

        // 💬 Mở phòng chat khi double click
        private void lstUsers_DoubleClick(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem == null) return;

            string friend = lstUsers.SelectedItem.ToString().Replace(" [Tin mới]", "");
            if (friend.ToLower() == "admin") return;

            // Nếu chat đã mở thì đưa lên trước
            if (_openChats.ContainsKey(friend) && !_openChats[friend].IsDisposed)
            {
                _openChats[friend].BringToFront();
                return;
            }

            // 🧩 Tạo cửa sổ chat mới
            ChatForm chatForm = new ChatForm(_client, _username, friend);
            _openChats[friend] = chatForm;

            this.Hide();

            chatForm.FormClosed += new FormClosedEventHandler(delegate
            {
                this.Show();
                this.BringToFront();
            });

            chatForm.Show();

            // Gỡ tag [Tin mới] khi mở lại
            for (int i = 0; i < lstUsers.Items.Count; i++)
            {
                if (lstUsers.Items[i].ToString().StartsWith(friend))
                    lstUsers.Items[i] = friend;
            }
        }

        // 🔍 Nút tìm kiếm
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (keyword == "" || keyword == "nhập tên người dùng...")
            {
                for (int i = 0; i < lstUsers.Items.Count; i++)
                    lstUsers.SetSelected(i, false);
                return;
            }

            for (int i = 0; i < lstUsers.Items.Count; i++)
            {
                string user = lstUsers.Items[i].ToString().ToLower();
                lstUsers.SetSelected(i, user.Contains(keyword));
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void RoomForm_Load(object sender, EventArgs e)
        {

        }

        private void lstUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0) return;

            string itemText = lstUsers.Items[e.Index].ToString();
            bool hasNew = itemText.EndsWith(" [Tin mới]");
            string displayName = hasNew ? itemText.Replace(" [Tin mới]", "") : itemText;

            var bounds = e.Bounds;
            Color baseColor = ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                ? Color.FromArgb(224, 231, 255)
                : Color.White;

            using (var bgBrush = new SolidBrush(baseColor))
                e.Graphics.FillRectangle(bgBrush, bounds);

            var avatar = AvatarHelper.GetAvatar(displayName, 48);
            var avatarRect = new Rectangle(bounds.Left + 16, bounds.Top + 8, 48, 48);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(avatar, avatarRect);

            using (var nameBrush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            using (var nameFont = new Font("Segoe UI Semibold", 11F))
            {
                var nameRect = new Rectangle(bounds.Left + 76, bounds.Top + 14, bounds.Width - 180, 24);
                e.Graphics.DrawString(displayName, nameFont, nameBrush, nameRect);
            }

            if (hasNew)
            {
                var badgeRect = new Rectangle(bounds.Right - 120, bounds.Top + 20, 100, 26);
                using (var badgeBrush = new SolidBrush(Color.FromArgb(190, 242, 100)))
                using (var textBrush = new SolidBrush(Color.FromArgb(63, 98, 18)))
                using (var badgeFont = new Font("Segoe UI", 9F, FontStyle.Bold))
                {
                    var radius = 12;
                    using (var path = new GraphicsPath())
                    {
                        path.AddArc(badgeRect.Left, badgeRect.Top, radius, radius, 180, 90);
                        path.AddArc(badgeRect.Right - radius, badgeRect.Top, radius, radius, 270, 90);
                        path.AddArc(badgeRect.Right - radius, badgeRect.Bottom - radius, radius, radius, 0, 90);
                        path.AddArc(badgeRect.Left, badgeRect.Bottom - radius, radius, radius, 90, 90);
                        path.CloseFigure();
                        e.Graphics.FillPath(badgeBrush, path);
                    }
                    var textRect = new RectangleF(badgeRect.Left, badgeRect.Top + 4, badgeRect.Width, badgeRect.Height);
                    var format = new StringFormat { Alignment = StringAlignment.Center };
                    e.Graphics.DrawString("TIN MỚI", badgeFont, textBrush, textRect, format);
                }
            }

            e.DrawFocusRectangle();
        }
    }
}
