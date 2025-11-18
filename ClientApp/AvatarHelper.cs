using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClientApp
{
    public static class AvatarHelper
    {
        private static readonly Color[] Palette = new[]
        {
            Color.FromArgb(59, 130, 246),
            Color.FromArgb(236, 72, 153),
            Color.FromArgb(16, 185, 129),
            Color.FromArgb(249, 115, 22),
            Color.FromArgb(139, 92, 246),
            Color.FromArgb(245, 158, 11),
            Color.FromArgb(14, 165, 233)
        };

        private static readonly ConcurrentDictionary<string, Image> Cache = new ConcurrentDictionary<string, Image>();

        public static Image GetAvatar(string name, int size = 44)
        {
            string key = $"{name}|{size}";
            return Cache.GetOrAdd(key, _ => GenerateAvatar(name, size));
        }

        private static Image GenerateAvatar(string name, int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                int colorIndex = Math.Abs((name ?? string.Empty).GetHashCode()) % Palette.Length;
                using (var brush = new SolidBrush(Palette[colorIndex]))
                {
                    g.FillEllipse(brush, 0, 0, size, size);
                }

                string initials = "?";
                if (!string.IsNullOrWhiteSpace(name))
                {
                    name = name.Trim();
                    if (name.Length <= 2)
                        initials = name.ToUpper();
                    else
                    {
                        var parts = name.Split(new[] { ' ', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                            initials = $"{parts[0][0]}{parts[1][0]}".ToUpper();
                        else
                            initials = name.Substring(0, 2).ToUpper();
                    }
                }

                using (var font = new Font("Segoe UI", size * 0.4f, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.DrawString(initials, font, textBrush, new RectangleF(0, 0, size, size), format);
                }
            }

            return bmp;
        }
    }
}



