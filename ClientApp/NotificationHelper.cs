using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClientApp
{
    public static class NotificationHelper
    {
        private static NotifyIcon _notifyIcon;
        private static bool _isInitialized = false;

        // Khởi tạo notification icon
        public static void Initialize()
        {
            try
            {
                if (!_isInitialized)
                {
                    _notifyIcon = new NotifyIcon
                    {
                        Icon = SystemIcons.Application, // Icon ứng dụng
                        Visible = true,
                        BalloonTipIcon = ToolTipIcon.Info,
                        Text = "NetChat - Ứng dụng Chat"
                    };
                    
                    _isInitialized = true;
                    Console.WriteLine("[Notification] ✅ Đã khởi tạo notification icon");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notification] ❌ Error initializing: {ex.Message}");
            }
        }

        // Hiển thị thông báo khi gửi tin nhắn
        public static void ShowSendNotification(string toUser, string message)
        {
            try
            {
                Initialize();
                if (_notifyIcon != null && _notifyIcon.Visible)
                {
                    // Rút ngắn message nếu quá dài
                    string shortMessage = message.Length > 60 ? message.Substring(0, 60) + "..." : message;
                    
                    _notifyIcon.BalloonTipTitle = "✅ Đã gửi tin nhắn";
                    _notifyIcon.BalloonTipText = $"Đến: {toUser}\n\n{shortMessage}";
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(2500); // Hiển thị 2.5 giây
                    
                    Console.WriteLine($"[Notification] 📤 Đã hiển thị thông báo gửi tin nhắn đến {toUser}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notification] ❌ Error showing send notification: {ex.Message}");
            }
        }

        // Hiển thị thông báo khi nhận tin nhắn
        public static void ShowReceiveNotification(string fromUser, string message)
        {
            try
            {
                Initialize();
                if (_notifyIcon != null && _notifyIcon.Visible)
                {
                    // Rút ngắn message nếu quá dài
                    string shortMessage = message.Length > 60 ? message.Substring(0, 60) + "..." : message;
                    
                    _notifyIcon.BalloonTipTitle = $"💬 Tin nhắn mới từ {fromUser}";
                    _notifyIcon.BalloonTipText = shortMessage;
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(4000); // Hiển thị 4 giây cho tin nhắn đến
                    
                    Console.WriteLine($"[Notification] 📥 Đã hiển thị thông báo nhận tin nhắn từ {fromUser}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notification] ❌ Error showing receive notification: {ex.Message}");
            }
        }

        // Hiển thị thông báo thông thường
        public static void ShowNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            try
            {
                Initialize();
                if (_notifyIcon != null && _notifyIcon.Visible)
                {
                    string shortMessage = message.Length > 100 ? message.Substring(0, 100) + "..." : message;
                    
                    _notifyIcon.BalloonTipTitle = title;
                    _notifyIcon.BalloonTipText = shortMessage;
                    _notifyIcon.BalloonTipIcon = icon;
                    _notifyIcon.ShowBalloonTip(3000);
                    
                    Console.WriteLine($"[Notification] 📢 Đã hiển thị thông báo: {title}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notification] ❌ Error showing notification: {ex.Message}");
            }
        }

        // Hiển thị thông báo lỗi
        public static void ShowErrorNotification(string title, string message)
        {
            try
            {
                Initialize();
                if (_notifyIcon != null && _notifyIcon.Visible)
                {
                    string shortMessage = message.Length > 100 ? message.Substring(0, 100) + "..." : message;
                    
                    _notifyIcon.BalloonTipTitle = $"❌ {title}";
                    _notifyIcon.BalloonTipText = shortMessage;
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                    _notifyIcon.ShowBalloonTip(5000); // Hiển thị lâu hơn cho lỗi
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notification] ❌ Error showing error notification: {ex.Message}");
            }
        }

        // Hiển thị thông báo thành công
        public static void ShowSuccessNotification(string title, string message)
        {
            try
            {
                Initialize();
                if (_notifyIcon != null && _notifyIcon.Visible)
                {
                    string shortMessage = message.Length > 100 ? message.Substring(0, 100) + "..." : message;
                    
                    _notifyIcon.BalloonTipTitle = $"✅ {title}";
                    _notifyIcon.BalloonTipText = shortMessage;
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(3000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notification] ❌ Error showing success notification: {ex.Message}");
            }
        }

        // Cleanup
        public static void Dispose()
        {
            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                    _isInitialized = false;
                    Console.WriteLine("[Notification] 🗑️ Đã cleanup notification icon");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Notification] ❌ Error disposing: {ex.Message}");
            }
        }
    }
}

