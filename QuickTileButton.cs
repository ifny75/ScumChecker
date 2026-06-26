using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class QuickTileButton : Button
    {
        public Color BorderColor { get; set; } = Color.FromArgb(120, 110, 255);
        public Color HoverBackColor { get; set; } = Color.FromArgb(18, 14, 36);
        public Color NormalBackColor { get; set; } = Color.FromArgb(10, 10, 16);
        public int BorderSize { get; set; } = 1;

        private bool _hover;

        public QuickTileButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = NormalBackColor;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Cursor = Cursors.Hand;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true
            );
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            _hover = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            _hover = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // background
            using (var bg = new SolidBrush(_hover ? HoverBackColor : NormalBackColor))
                e.Graphics.FillRectangle(bg, rect);

            // border (Inset — чтобы не резало края)
            using (var pen = new Pen(BorderColor, BorderSize) { Alignment = PenAlignment.Inset })
                e.Graphics.DrawRectangle(pen, rect);

            DrawImageAndText(e.Graphics);
        }

        private void DrawImageAndText(Graphics g)
        {
            int left = Padding.Left;
            int iconGap = 8;

            if (Image != null)
            {
                int iy = (Height - Image.Height) / 2;
                g.DrawImage(Image, left, iy, Image.Width, Image.Height);
                left += Image.Width + iconGap;
            }

            var textRect = new Rectangle(
                left,
                0,
                Width - left - Padding.Right,
                Height
            );

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                ForeColor,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis
            );
        }
    }
}
