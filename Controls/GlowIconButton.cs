using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class GlowIconButton : Button
    {
        private bool _hover;
        private bool _pressed;

        public GlowIconButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 1;

            // чтобы выглядело как у тебя в теме
            BackColor = Color.FromArgb(12, 12, 18);
            ForeColor = Color.Gainsboro;

            ImageAlign = ContentAlignment.MiddleLeft;
            TextAlign = ContentAlignment.MiddleLeft;
            TextImageRelation = TextImageRelation.ImageBeforeText;
            Padding = new Padding(12, 0, 10, 0);
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool Selected { get; set; }

        [Browsable(true)]
        public Color GlowColor { get; set; } = Color.FromArgb(170, 120, 110, 255);

        [Browsable(true)]
        [DefaultValue(10)]
        public int GlowRadius { get; set; } = 10;

        [Browsable(true)]
        [DefaultValue(8)]
        public int GlowStrength { get; set; } = 8;

        [Browsable(true)]
        [DefaultValue(true)]
        public bool GlowOnHover { get; set; } = true;

        [Browsable(true)]
        [DefaultValue(true)]
        public bool GlowOnSelected { get; set; } = true;

        [Browsable(true)]
        [DefaultValue(18)]
        public int IconSize { get; set; } = 18;

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { _pressed = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _pressed = false; Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // фон/рамка как у твоих кнопок
            var bg = BackColor;
            var border = FlatAppearance.BorderColor;

            if (_pressed)
                bg = Shift(bg, -10);
            else if (_hover)
                bg = Shift(bg, +8);

            using (var b = new SolidBrush(bg))
                g.FillRectangle(b, ClientRectangle);

            // иконка (если задана)
            Rectangle iconRect = Rectangle.Empty;

            if (Image != null)
            {
                int s = Math.Max(8, IconSize);
                int x = Padding.Left;
                int y = (Height - s) / 2;
                iconRect = new Rectangle(x, y, s, s);

                bool glow =
                    (GlowOnHover && _hover) ||
                    (GlowOnSelected && Selected);

                if (glow)
                    DrawImageGlow(g, Image, iconRect, GlowColor, GlowRadius, GlowStrength);

                g.DrawImage(Image, iconRect);
            }

            // текст (чтобы не ломать твою разметку)
            var textX = (Image != null) ? (iconRect.Right + 10) : Padding.Left;
            var textRect = new Rectangle(textX, 0, Width - textX - 8, Height);

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis
            );

            using (var p = new Pen(border))
                g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
        }

        private static void DrawImageGlow(Graphics g, Image img, Rectangle rect, Color glow, int radius, int strength)
        {
            // Имитация blur: рисуем иконку несколько раз чуть "раздутой" и с уменьшающейся альфой
            // Важно: работает отлично на PNG с альфой.
            int steps = Math.Max(3, strength);
            for (int i = steps; i >= 1; i--)
            {
                float t = i / (float)steps; // 1..0
                int spread = (int)(radius * t);

                var rr = new Rectangle(rect.X - spread, rect.Y - spread, rect.Width + spread * 2, rect.Height + spread * 2);

                int a = (int)(glow.A * (t * t) * 0.55f); // квадратичное затухание
                if (a < 1) continue;

                using var ia = new ImageAttributesTint(Color.FromArgb(a, glow));
                ia.Draw(g, img, rr);
            }
        }

        private static Color Shift(Color c, int delta)
        {
            int r = Math.Clamp(c.R + delta, 0, 255);
            int g = Math.Clamp(c.G + delta, 0, 255);
            int b = Math.Clamp(c.B + delta, 0, 255);
            return Color.FromArgb(c.A, r, g, b);
        }

        /// <summary>
        /// Тинтование картинки (через ColorMatrix), чтобы glow был цветной.
        /// </summary>
        private sealed class ImageAttributesTint : IDisposable
        {
            private readonly System.Drawing.Imaging.ImageAttributes _attr = new();
            public ImageAttributesTint(Color c)
            {
                float r = c.R / 255f;
                float g = c.G / 255f;
                float b = c.B / 255f;
                float a = c.A / 255f;

                var matrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
                {
                    new float[] { r, 0, 0, 0, 0 },
                    new float[] { 0, g, 0, 0, 0 },
                    new float[] { 0, 0, b, 0, 0 },
                    new float[] { 0, 0, 0, a, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                _attr.SetColorMatrix(matrix);
            }

            public void Draw(Graphics g, Image img, Rectangle dest)
            {
                g.DrawImage(
                    img,
                    dest,
                    0, 0, img.Width, img.Height,
                    GraphicsUnit.Pixel,
                    _attr
                );
            }

            public void Dispose() => _attr.Dispose();
        }
    }
}
