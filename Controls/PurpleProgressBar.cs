using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class PurpleProgressBar : Control
    {
        private int _maximum = 100;
        private int _value = 0;

        public int Maximum
        {
            get => _maximum;
            set { _maximum = Math.Max(1, value); Invalidate(); }
        }

        public int Value
        {
            get => _value;
            set { _value = Math.Max(0, Math.Min(value, Maximum)); Invalidate(); }
        }

        public int CornerRadius { get; set; } = 10;

        public Color TrackColor { get; set; } = Color.FromArgb(14, 14, 24);
        public Color GlowColor { get; set; } = Color.FromArgb(70, 120, 255);

        public Color BarColor1 { get; set; } = Color.FromArgb(120, 90, 255); // фиолетовый
        public Color BarColor2 { get; set; } = Color.FromArgb(0, 170, 255);   // синий

        public bool ShowPercent { get; set; } = false;
        public Color TextColor { get; set; } = Color.Gainsboro;

        public PurpleProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            Height = 14;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Track
            using (var trackPath = RoundedRect(rect, CornerRadius))
            using (var trackBrush = new SolidBrush(TrackColor))
            {
                e.Graphics.FillPath(trackBrush, trackPath);
            }

            // Progress width
            float p = (float)Value / Maximum;
            int w = (int)Math.Round(rect.Width * p);
            if (w <= 0) return;

            var fillRect = new Rectangle(rect.X, rect.Y, w, rect.Height);

            // Glow (soft)
            using (var glowBrush = new SolidBrush(Color.FromArgb(50, GlowColor)))
            using (var glowPath = RoundedRect(rect, CornerRadius))
            {
                e.Graphics.FillPath(glowBrush, glowPath);
            }

            // Bar
            using (var barPath = RoundedRect(fillRect, CornerRadius))
            using (var barBrush = new LinearGradientBrush(fillRect, BarColor1, BarColor2, 0f))
            {
                e.Graphics.FillPath(barBrush, barPath);
            }

            if (ShowPercent)
            {
                string text = $"{(int)(p * 100)}%";
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var tb = new SolidBrush(TextColor);
                e.Graphics.DrawString(text, Font, tb, rect, sf);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            if (radius <= 0 || r.Width <= 0 || r.Height <= 0)
            {
                path.AddRectangle(r);
                path.CloseFigure();
                return path;
            }

            int rr = Math.Min(radius, Math.Min(r.Width, r.Height) / 2);
            d = rr * 2;

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
