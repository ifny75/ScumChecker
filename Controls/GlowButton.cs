using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class GlowButton : Button
    {
        [Browsable(true)] public int CornerRadius { get; set; } = 16;

        [Browsable(true)] public int BorderSize { get; set; } = 1;
        [Browsable(true)] public Color BorderColor { get; set; } = Color.FromArgb(120, 110, 255);

        [Browsable(true)] public Color BaseColor { get; set; } = Color.FromArgb(12, 12, 18);
        [Browsable(true)] public Color HoverColor { get; set; } = Color.FromArgb(20, 16, 36);
        [Browsable(true)] public Color PressedColor { get; set; } = Color.FromArgb(16, 12, 28);

        [Browsable(true)] public int GlowSize { get; set; } = 10;
        [Browsable(true)] public Color GlowColor { get; set; } = Color.FromArgb(160, 120, 110, 255);

        [Browsable(true)] public bool UseGlow { get; set; } = true;

        private bool _hover;
        private bool _down;

        public GlowButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Cursor = Cursors.Hand;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { if (mevent.Button == MouseButtons.Left) _down = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _down = false; Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            var rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using var path = RoundRect(rect, CornerRadius);
            Region = new Region(path);

            // glow
            if (UseGlow && _hover && GlowSize > 0)
            {
                for (int i = GlowSize; i >= 1; i--)
                {
                    float t = i / (float)GlowSize;
                    int a = (int)(GlowColor.A * (1f - t) * 0.9f);
                    using var p = new Pen(Color.FromArgb(a, GlowColor), 2f);
                    using var gp = RoundRect(Rectangle.Inflate(rect, i, i), CornerRadius + i);
                    e.Graphics.DrawPath(p, gp);
                }
            }

            // fill
            Color fill = _down ? PressedColor : (_hover ? HoverColor : BaseColor);
            using (var b = new SolidBrush(fill))
                e.Graphics.FillPath(b, path);

            // border
            if (BorderSize > 0)
            {
                using var pen = new Pen(BorderColor, BorderSize);
                e.Graphics.DrawPath(pen, path);
            }

            // text
            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                rect,
                ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
            );
        }

        private static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Max(1, radius * 2);

            if (radius <= 0)
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
