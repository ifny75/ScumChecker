using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public sealed class ToolTileCard : UserControl
    {
        private readonly PictureBox _pic;
        private readonly Label _lblTitle;
        private readonly Label _lblDesc;
        private readonly Label _lblStatus;

        private bool _selected;

        public event EventHandler? Clicked;
        public event EventHandler? DoubleClicked;

        public ToolTileCard()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(14, 14, 20);
            ForeColor = Color.Gainsboro;
            Cursor = Cursors.Hand;

            Padding = new Padding(12);
            Margin = new Padding(0, 0, 0, 12);
            Height = 74;

            _pic = new PictureBox
            {
                Size = new Size(44, 44),
                Location = new Point(12, 14),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            _lblTitle = new Label
            {
                AutoSize = false,
                Location = new Point(68, 12),
                Size = new Size(520, 22),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Text = "Tool"
            };

            _lblDesc = new Label
            {
                AutoSize = false,
                Location = new Point(68, 34),
                Size = new Size(680, 34),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.Gainsboro,
                Text = "Description"
            };

            _lblStatus = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0),
                Size = new Size(110, 22),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.FromArgb(90, 150, 255),
                Text = "Unknown"
            };

            Controls.Add(_pic);
            Controls.Add(_lblTitle);
            Controls.Add(_lblDesc);
            Controls.Add(_lblStatus);

            // клики по любому месту карточки
            foreach (Control c in Controls)
            {
                c.Click += (_, __) => OnClicked();
                c.DoubleClick += (_, __) => OnDoubleClicked();
            }
            Click += (_, __) => OnClicked();
            DoubleClick += (_, __) => OnDoubleClicked();

            Resize += (_, __) =>
            {
                // статус в правом верхнем углу
                _lblStatus.Location = new Point(Width - _lblStatus.Width - 12, 12);
                _lblTitle.Width = Math.Max(200, _lblStatus.Left - _lblTitle.Left - 10);
                _lblDesc.Width = Math.Max(200, Width - _lblDesc.Left - 12);
            };
        }

        public Image? Icon
        {
            get => _pic.Image;
            set => _pic.Image = value;
        }

        public string Title
        {
            get => _lblTitle.Text;
            set => _lblTitle.Text = value ?? "";
        }

        public string Description
        {
            get => _lblDesc.Text;
            set => _lblDesc.Text = value ?? "";
        }

        public string StatusText
        {
            get => _lblStatus.Text;
            set => _lblStatus.Text = value ?? "";
        }

        public Color StatusColor
        {
            get => _lblStatus.BackColor;
            set => _lblStatus.BackColor = value;
        }

        public bool Selected
        {
            get => _selected;
            set { _selected = value; Invalidate(); }
        }

        public object? Payload { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            Color border = Selected
                ? Color.FromArgb(140, 110, 255)
                : Color.FromArgb(60, 60, 80);

            Color fill = Selected
                ? Color.FromArgb(18, 14, 36)
                : BackColor;

            using var br = new SolidBrush(fill);
            using var pen = new Pen(border, 1);

            int r = 14;
            using var path = RoundedRect(rect, r);
            e.Graphics.FillPath(br, path);
            e.Graphics.DrawPath(pen, path);
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
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

        private void OnClicked() => Clicked?.Invoke(this, EventArgs.Empty);
        private void OnDoubleClicked() => DoubleClicked?.Invoke(this, EventArgs.Empty);
    }
}
