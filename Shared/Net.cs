using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Net
    {
        public static async Task SendAsync(NetworkStream stream, string json)
        {
            var data = Encoding.UTF8.GetBytes(json);
            var len = BitConverter.GetBytes(data.Length);
            await stream.WriteAsync(len, 0, 4);
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
        }

        public static async Task<string> ReceiveAsync(NetworkStream stream)
        {
            var lenBuf = new byte[4];
            await ReadExact(stream, lenBuf, 4);
            int len = BitConverter.ToInt32(lenBuf, 0);

            var buf = new byte[len];
            await ReadExact(stream, buf, len);
            return Encoding.UTF8.GetString(buf);
        }

        private static async Task ReadExact(NetworkStream s, byte[] buf, int need)
        {
            int have = 0;
            while (have < need)
            {
                int n = await s.ReadAsync(buf, have, need - have);
                if (n <= 0) throw new IOException("Disconnected");
                have += n;
            }
        }
    }
}
