using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class GradientPanel : Panel
    {
        public Color Color1 { get; set; } = Color.FromArgb(18, 18, 30);
        public Color Color2 { get; set; } = Color.FromArgb(26, 18, 55);
        public float Angle { get; set; } = 135f;

        public int CornerRadius { get; set; } = 16;

        public bool DrawBorder { get; set; } = true;
        public Color BorderColor { get; set; } = Color.FromArgb(80, 70, 140);
        public float BorderWidth { get; set; } = 1f;

        public GradientPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            // ✅ ВАЖНО: реальное скругление — через Region
            if (Width <= 1 || Height <= 1) return;

            var rect = new Rectangle(0, 0, Width, Height);
            using var path = RoundedRect(rect, CornerRadius);

            // освобождаем старый регион
            Region?.Dispose();
            Region = new Region(path);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using var path = RoundedRect(rect, CornerRadius);
            using var brush = new LinearGradientBrush(rect, Color1, Color2, Angle);

            e.Graphics.FillPath(brush, path);

            if (DrawBorder && BorderWidth > 0f)
            {
                using var pen = new Pen(BorderColor, BorderWidth);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int rr = Math.Min(radius, Math.Min(r.Width, r.Height) / 2);
            int d = rr * 2;

            if (rr <= 0)
            {
                path.AddRectangle(r);
                path.CloseFigure();
                return path;
            }

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
