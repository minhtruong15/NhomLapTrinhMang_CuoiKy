using System;
using System.Windows.Forms;

namespace ServerApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServerMainForm()); // Mở giao diện server chính
        }
    }
}
