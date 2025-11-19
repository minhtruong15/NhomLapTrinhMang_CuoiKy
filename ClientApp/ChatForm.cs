using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Shared;

namespace ClientApp
{
    public partial class ChatForm : Form
    {
        private const string MessageRowTag = "MESSAGE_ROW";
        private const string BubbleControlTag = "MESSAGE_BUBBLE";
        private const int AvatarSize = 44;

        private readonly ChatClient _client;
        private readonly string _username;
        private readonly string _friend;

        private readonly HashSet<string> _pendingSelfMessages = new HashSet<string>();

        public ChatForm(ChatClient client, string username, string friend)
        {
            InitializeComponent();
            DoubleBuffered = true;

            flowMessages.AutoScrollMargin = new Size(0, 60);
            flowMessages.SizeChanged += FlowMessages_SizeChanged;

            ApplyRoundedCorners(messageBox, 18);
            messageBox.Paint += MessageBox_Paint;

            _client = client;
            _username = username;
            _friend = friend;

            lblRoom.Text = $"Đang trò chuyện với {_friend}";

            // đảm bảo không đăng ký 2 lần
            _client.OnPacketReceived -= HandlePacket;
            _client.OnPacketReceived += HandlePacket;

            _ = _client.SendAsync(new Packet(Command.RequestPrivateHistory, new { with = _friend }));

            txtMessage.KeyDown += TxtMessage_KeyDown;
            NotificationHelper.Initialize();
        }

        // =========================================================
        // TỰ ĐIỀU CHỈNH BUBBLE KHI RESIZE FORM
        // =========================================================
        private void FlowMessages_SizeChanged(object sender, EventArgs e)
        {
            int rowWidth = CalculateRowWidth();
            int bubbleWidth = CalculateBubbleWidth(rowWidth);

            foreach (Control control in flowMessages.Controls)
            {
                if (control.Tag as string == MessageRowTag)
                {
                    control.Width = rowWidth;
                    control.MinimumSize = new Size(rowWidth, 0);
                    control.MaximumSize = new Size(rowWidth, int.MaxValue);
                }

                UpdateBubbleWidth(control, bubbleWidth);
            }

            flowMessages.PerformLayout();
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSend.PerformClick();
            }
        }

        // =========================================================
        // GỬI TIN NHẮN (TEXT)
        // =========================================================
        private async void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            var payload = MessageContent.CreateText(msg);
            await SendPayloadAsync(payload);

            txtMessage.Clear();
        }

        private async System.Threading.Tasks.Task SendPayloadAsync(MessageContent content)
        {
            if (content == null) return;

            string serialized = MessageContent.Serialize(content);

            await _client.SendAsync(new Packet(Command.PrivateMessage, new
            {
                to = _friend,
                payload = serialized
            }));

            lock (_pendingSelfMessages)
            {
                if (!string.IsNullOrWhiteSpace(content.ClientMessageId))
                    _pendingSelfMessages.Add(content.ClientMessageId);
            }

            NotificationHelper.ShowSendNotification(_friend, content.Text);

            // hiển thị ngay để người dùng thấy phản hồi tức thì
            AddMessageBubble(_username, content, DateTime.Now, true);
        }

        // =========================================================
        // NHẬN PACKET TỪ SERVER
        // =========================================================
        private void HandlePacket(Packet p)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Packet>(HandlePacket), p);
                return;
            }

            switch (p.Cmd)
            {
                case Command.PrivateMessage:
                    {
                        dynamic d = p.Data;
                        string from = d.from;
                        string payload = d.payload;
                        string time = d.time;
                        string toUser = d.to;

                        bool isSelf = EqualsUser(from, _username);
                        bool isFriend = EqualsUser(from, _friend);

                        bool shouldRender =
                            (isSelf && EqualsUser(toUser, _friend)) ||
                            (isFriend && EqualsUser(toUser, _username));

                        if (shouldRender)
                        {
                            var content = MessageContent.Deserialize(payload);
                            DateTime dt = ParseTime(time);

                            if (isSelf && SuppressIfPending(content))
                                break;

                            AddMessageBubble(from, content, dt, isSelf);

                            if (!isSelf)
                                NotificationHelper.ShowReceiveNotification(_friend, content.Text);
                        }
                    }
                    break;

                case Command.PrivateHistory:
                    {
                        dynamic d = p.Data;

                        foreach (var m in d.messages)
                        {
                            string from = m.sender;
                            string payload = m.payload;
                            string time = m.time;

                            var content = MessageContent.Deserialize(payload);
                            DateTime dt = ParseTime(time);

                            AddMessageBubble(from, content, dt, EqualsUser(from, _username));
                        }
                    }
                    break;
            }
        }

        private DateTime ParseTime(string t)
        {
            if (DateTime.TryParse(t, out var dt)) return dt;
            return DateTime.Now;
        }

        private bool SuppressIfPending(MessageContent content)
        {
            if (content == null || string.IsNullOrWhiteSpace(content.ClientMessageId))
                return false;

            lock (_pendingSelfMessages)
            {
                if (_pendingSelfMessages.Remove(content.ClientMessageId))
                    return true;
            }
            return false;
        }

        // =========================================================
        // VẼ BUBBLE
        // =========================================================
        private void AddMessageBubble(string sender, MessageContent content, DateTime at, bool isSelf)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddMessageBubble(sender, content, at, isSelf)));
                return;
            }

            RenderMessageBubble(sender, content, at, isSelf);
        }

        private void RenderMessageBubble(string sender, MessageContent content, DateTime at, bool isSelf)
        {
            int rowWidth = CalculateRowWidth();
            int bubbleWidth = CalculateBubbleWidth(rowWidth);

            var row = new TableLayoutPanel
            {
                Tag = MessageRowTag,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                Width = rowWidth,
                MinimumSize = new Size(rowWidth, 0),
                MaximumSize = new Size(rowWidth, int.MaxValue),
                Margin = new Padding(0, 0, 0, 22),
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            PictureBox avatar = BuildAvatar(sender, isSelf);
            Control bubble = BuildBubblePanel(sender, content, at, isSelf, bubbleWidth);
            Control spacer = BuildSpacer();

            if (isSelf)
            {
                // self: spacer chiếm 100%, bubble + avatar dính sát phải
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                row.Controls.Add(spacer, 0, 0);
                row.Controls.Add(bubble, 1, 0);
                row.Controls.Add(avatar, 2, 0);
            }
            else
            {
                // friend: avatar + bubble sát trái
                row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                row.Controls.Add(avatar, 0, 0);
                row.Controls.Add(bubble, 1, 0);
                row.Controls.Add(spacer, 2, 0);
            }

            flowMessages.SuspendLayout();
            flowMessages.Controls.Add(row);
            flowMessages.ResumeLayout();
            flowMessages.ScrollControlIntoView(row);
        }

        private Control BuildSpacer()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                AutoSize = false
            };
        }

        private PictureBox BuildAvatar(string user, bool isSelf)
        {
            return new PictureBox
            {
                Size = new Size(AvatarSize, AvatarSize),
                Image = AvatarHelper.GetAvatar(user, AvatarSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = isSelf
                    ? new Padding(6, 4, 12, 0)   // bên mình sát mép phải
                    : new Padding(12, 4, 6, 0)   // bên bạn sát mép trái
            };
        }

        private Control BuildBubblePanel(string sender, MessageContent content, DateTime at, bool isSelf, int maxWidth)
        {
            var bubble = new TableLayoutPanel
            {
                Tag = BubbleControlTag,
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MaximumSize = new Size(maxWidth, 0),
                BackColor = isSelf ? Color.FromArgb(5, 150, 105) : Color.White,
                Padding = new Padding(18, 14, 18, 12),
                Margin = isSelf
                    ? new Padding(0, 0, 4, 0)    // sát avatar bên phải
                    : new Padding(8, 0, 0, 0),   // sát avatar bên trái
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
            bubble.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int row = 0;

            if (!isSelf)
            {
                bubble.Controls.Add(new Label
                {
                    Text = sender,
                    AutoSize = true,
                    Font = new Font("Segoe UI Semibold", 9.5F),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    Margin = new Padding(0, 0, 0, 6),
                    BackColor = Color.Transparent
                }, 0, row++);
            }

            bubble.Controls.Add(new Label
            {
                Text = content.Text,
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 40, 0),
                Font = new Font("Segoe UI", 11F),
                ForeColor = isSelf ? Color.White : Color.FromArgb(30, 41, 59),
                Margin = new Padding(0, 0, 0, 8),
                BackColor = Color.Transparent
            }, 0, row++);

            bubble.Controls.Add(new Label
            {
                Text = at.ToString("HH:mm"),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(203, 213, 225),
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            }, 0, row);

            ApplyRoundedCorners(bubble, 18);
            return bubble;
        }

        // =========================================================
        // BO GÓC
        // =========================================================
        private void ApplyRoundedCorners(Control control, int radius)
        {
            void Update(object s, EventArgs e)
            {
                if (control.Width <= 0 || control.Height <= 0) return;

                using (var path = CreateRoundedRectanglePath(
                           new Rectangle(0, 0, control.Width, control.Height), radius))
                {
                    var region = new Region(path);
                    var old = control.Region;
                    control.Region = region;
                    old?.Dispose();
                }
            }

            control.Resize += Update;
            Update(control, EventArgs.Empty);
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void UpdateBubbleWidth(Control control, int width)
        {
            if (control == null) return;

            if (control.Tag as string == BubbleControlTag)
            {
                control.MaximumSize = new Size(width, 0);
            }

            foreach (Control c in control.Controls)
                UpdateBubbleWidth(c, width);
        }

        private int CalculateRowWidth()
        {
            int padding = flowMessages.Padding.Left + flowMessages.Padding.Right;
            return Math.Max(360, flowMessages.ClientSize.Width - padding);
        }

        private int CalculateBubbleWidth(int rowWidth)
        {
            int reserved = AvatarSize + 80; // avatar + margin
            return Math.Max(280, rowWidth - reserved);
        }

        private static bool EqualsUser(string a, string b)
        {
            return string.Equals(a ?? string.Empty, b ?? string.Empty,
                StringComparison.OrdinalIgnoreCase);
        }

        private void MessageBox_Paint(object sender, PaintEventArgs e)
        {
            var r = messageBox.ClientRectangle;
            r.Width--;
            r.Height--;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedRectanglePath(r, 18))
            using (var brush = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.FromArgb(226, 232, 240)))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _client.OnPacketReceived -= HandlePacket;
            base.OnFormClosed(e);
        }
    }
}
