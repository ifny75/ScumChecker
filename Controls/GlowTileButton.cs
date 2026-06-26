using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class GlowTileButton : Button
    {
        private bool _hover;
        private bool _pressed;

        public int CornerRadius { get; set; } = 16;
        public Color AccentColor { get; set; } = Color.FromArgb(120, 110, 255);
        public Color TileColor { get; set; } = Color.FromArgb(14, 14, 22);

        public string IconGlyph { get; set; } = "";
        public int IconSize { get; set; } = 18;

        public GlowTileButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;

            Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            ForeColor = Color.White;
            BackColor = Color.Transparent;

            Size = new Size(260, 52);
            Cursor = Cursors.Hand;
            TextAlign = ContentAlignment.MiddleLeft;
            Padding = new Padding(46, 0, 14, 0);
            Margin = new Padding(0, 0, 12, 12);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { _pressed = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _pressed = false; Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Color.Black);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Max(6, CornerRadius);

            var bg = TileColor;
            if (_hover) bg = Color.FromArgb(bg.A, Math.Min(255, bg.R + 6), Math.Min(255, bg.G + 6), Math.Min(255, bg.B + 8));
            if (_pressed) bg = Color.FromArgb(bg.A, Math.Max(0, bg.R - 10), Math.Max(0, bg.G - 10), Math.Max(0, bg.B - 10));

            if (_hover)
                DrawGlow(e.Graphics, rect, r, Color.FromArgb(90, AccentColor), 10);

            using (var path = RoundRect(rect, r))
            using (var b = new SolidBrush(bg))
                e.Graphics.FillPath(b, path);

            // subtle border
            using (var path = RoundRect(rect, r))
            using (var pen = new Pen(Color.FromArgb(35, 255, 255, 255), 1f))
                e.Graphics.DrawPath(pen, path);

            // icon
            if (!string.IsNullOrWhiteSpace(IconGlyph))
            {
                using var f = new Font("Segoe MDL2 Assets", IconSize, FontStyle.Regular, GraphicsUnit.Pixel);
                using var br = new SolidBrush(Color.FromArgb(220, 220, 220));
                int iconX = 16;
                int iconY = (Height - IconSize) / 2;
                e.Graphics.DrawString(IconGlyph, f, br, new PointF(iconX, iconY));
            }

            // text
            var textRect = new Rectangle(Padding.Left, 0, Width - Padding.Left - 12, Height);
            using var tbr = new SolidBrush(ForeColor);
            using var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(Text, Font, tbr, textRect, sf);
        }

        private static void DrawGlow(Graphics g, Rectangle rect, int radius, Color color, int spread)
        {
            for (int i = spread; i >= 1; i--)
            {
                int a = (int)(color.A * (i / (float)spread) * 0.35f);
                var c = Color.FromArgb(Math.Max(0, Math.Min(255, a)), color);
                var rr = Rectangle.Inflate(rect, i, i);

                using var p = RoundRect(rr, radius + i);
                using var pen = new Pen(c, 2f);
                g.DrawPath(pen, p);
            }
        }

        private static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var gp = new GraphicsPath();
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }
    }
}
