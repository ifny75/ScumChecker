using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class ToolActionCard : Panel
    {
        public event EventHandler? Clicked;

        private readonly PictureBox _ico;
        private readonly Label _title;
        private readonly Label _desc;

        private bool _hover;
        private bool _pressed;

        public ToolActionCard()
        {
            DoubleBuffered = true;
            Cursor = Cursors.Hand;

            BackColor = Color.FromArgb(12, 12, 18);
            Padding = new Padding(12);
            Size = new Size(210, 48);

            _ico = new PictureBox
            {
                Size = new Size(22, 22),
                Location = new Point(12, 13),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            _title = new Label
            {
                AutoSize = false,
                Location = new Point(44, 8),
                Size = new Size(150, 18),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Text = "Action"
            };

            _desc = new Label
            {
                AutoSize = false,
                Location = new Point(44, 26),
                Size = new Size(160, 16),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.Gainsboro,
                Text = "Description"
            };

            Controls.Add(_ico);
            Controls.Add(_title);
            Controls.Add(_desc);

            // клики по дочерним -> кликают карточку
            foreach (Control c in Controls)
            {
                c.MouseEnter += (_, __) => { _hover = true; Invalidate(); };
                c.MouseLeave += (_, __) => { _hover = false; _pressed = false; Invalidate(); };
                c.MouseDown += (_, __) => { _pressed = true; Invalidate(); };
                c.MouseUp += (_, __) => { _pressed = false; Invalidate(); Clicked?.Invoke(this, EventArgs.Empty); };
                c.Click += (_, __) => Clicked?.Invoke(this, EventArgs.Empty);
            }

            MouseEnter += (_, __) => { _hover = true; Invalidate(); };
            MouseLeave += (_, __) => { _hover = false; _pressed = false; Invalidate(); };
            MouseDown += (_, __) => { _pressed = true; Invalidate(); };
            MouseUp += (_, __) => { _pressed = false; Invalidate(); Clicked?.Invoke(this, EventArgs.Empty); };
            Click += (_, __) => Clicked?.Invoke(this, EventArgs.Empty);
        }

        public Image? Icon
        {
            get => _ico.Image;
            set => _ico.Image = value;
        }

        public string Title
        {
            get => _title.Text;
            set => _title.Text = value;
        }

        public string Description
        {
            get => _desc.Text;
            set => _desc.Text = value;
        }

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set { _selected = value; Invalidate(); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            rect.Inflate(-1, -1);

            Color border = Color.FromArgb(60, 60, 80);

            if (!Enabled) border = Color.FromArgb(35, 35, 50);
            else if (_selected) border = Color.FromArgb(120, 110, 255);
            else if (_pressed) border = Color.FromArgb(160, 120, 255);
            else if (_hover) border = Color.FromArgb(90, 90, 130);

            using var pen = new Pen(border, 1f);
            using var path = Rounded(rect, 10);

            e.Graphics.DrawPath(pen, path);

            // лёгкий “glow” при selected/hover (очень мягко)
            if (Enabled && (_selected || _hover))
            {
                using var glow = new Pen(Color.FromArgb(_selected ? 70 : 35, 120, 110, 255), 4f);
                e.Graphics.DrawPath(glow, path);
            }
        }

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
