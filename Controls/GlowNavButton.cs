using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class GlowNavButton : Button
    {
        private bool _hover;
        private bool _pressed;

        public GlowNavButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;

            Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            ForeColor = Color.Gainsboro;
            BackColor = Color.FromArgb(18, 18, 26);

            Height = 44;
            TextAlign = ContentAlignment.MiddleLeft;
            ImageAlign = ContentAlignment.MiddleLeft;
            Padding = new Padding(44, 0, 12, 0);

            Cursor = Cursors.Hand;
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool Selected { get; set; }

        [Browsable(true)]
        [DefaultValue(14)]
        public int CornerRadius { get; set; } = 14;

        [Browsable(true)]
        [DefaultValue(typeof(Color), "120, 110, 255")]
        public Color AccentColor { get; set; } = Color.FromArgb(120, 110, 255);

        [Browsable(true)]
        [DefaultValue(typeof(Color), "18, 18, 26")]
        public Color BaseColor { get; set; } = Color.FromArgb(18, 18, 26);

        [Browsable(true)]
        [DefaultValue(typeof(Color), "22, 22, 34")]
        public Color HoverColor { get; set; } = Color.FromArgb(22, 22, 34);

        [Browsable(true)]
        [DefaultValue(typeof(Color), "26, 22, 46")]
        public Color SelectedColor { get; set; } = Color.FromArgb(26, 22, 46);

        [Browsable(true)]
        public Image? IconImage { get; set; } // optional

        [Browsable(true)]
        [DefaultValue("")]
        public string IconGlyph { get; set; } = ""; // Segoe MDL2 Assets glyph

        [Browsable(true)]
        [DefaultValue(16)]
        public int IconSize { get; set; } = 16;

        [Browsable(true)]
        [DefaultValue(3)]
        public int LeftAccentWidth { get; set; } = 3;

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { _pressed = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _pressed = false; Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Color.Black);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Max(4, CornerRadius);

            Color bg = Selected ? SelectedColor : (_hover ? HoverColor : BaseColor);
            if (_pressed) bg = Blend(bg, Color.Black, 0.10f);

            // Glow (как на скрине): рисуем 3-4 слоя вокруг при Selected / Hover
            if (Selected || _hover)
            {
                var glowAlpha = Selected ? 110 : 60;
                DrawGlow(e.Graphics, rect, r, Color.FromArgb(glowAlpha, AccentColor), 10);
            }

            // Main rounded bg
            using (var path = RoundRect(rect, r))
            using (var b = new SolidBrush(bg))
                e.Graphics.FillPath(b, path);

            // Left accent
            if (Selected)
            {
                var accRect = new Rectangle(6, 8, LeftAccentWidth, Height - 16);
                using var accPath = RoundRect(accRect, Math.Min(6, r));
                using var ab = new SolidBrush(Color.FromArgb(220, AccentColor));
                e.Graphics.FillPath(ab, accPath);
            }

            // Icon
            int iconX = 16;
            int iconY = (Height - IconSize) / 2;

            if (IconImage != null)
            {
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(IconImage, new Rectangle(iconX, iconY, IconSize, IconSize));
            }
            else if (!string.IsNullOrWhiteSpace(IconGlyph))
            {
                using var f = new Font("Segoe MDL2 Assets", IconSize, FontStyle.Regular, GraphicsUnit.Pixel);
                using var br = new SolidBrush(Selected ? Color.White : Color.Gainsboro);
                e.Graphics.DrawString(IconGlyph, f, br, new PointF(iconX, iconY - 1));
            }

            // Text
            var textRect = new Rectangle(Padding.Left, 0, Width - Padding.Left - 12, Height);
            using var tbr = new SolidBrush(Selected ? Color.White : ForeColor);
            using var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(Text, Font, tbr, textRect, sf);
        }

        private static void DrawGlow(Graphics g, Rectangle rect, int radius, Color color, int spread)
        {
            // мягкий glow: несколько расширяющихся рамок с убывающей прозрачностью
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

        private static Color Blend(Color c1, Color c2, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            int r = (int)(c1.R + (c2.R - c1.R) * t);
            int g = (int)(c1.G + (c2.G - c1.G) * t);
            int b = (int)(c1.B + (c2.B - c1.B) * t);
            return Color.FromArgb(c1.A, r, g, b);
        }
    }
}
