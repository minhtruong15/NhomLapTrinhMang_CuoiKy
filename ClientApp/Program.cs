using System;
using System.Windows.Forms;

namespace ClientApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Cleanup notification khi ứng dụng đóng
            Application.ApplicationExit += (s, e) => NotificationHelper.Dispose();
            
            Application.Run(new LoginForm()); // Mở form đăng nhập đầu tiên
        }
    }
}
