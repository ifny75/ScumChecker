using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScumChecker.Controls
{
    public class SteamAccountCard : UserControl
    {
        private readonly PictureBox _avatar = new PictureBox();
        private readonly Label _lblName = new Label();
        private readonly FlowLayoutPanel _rows = new FlowLayoutPanel();

        public SteamAccountCard()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;

            Size = new Size(380, 250);

            _avatar.Size = new Size(56, 56);
            _avatar.Location = new Point(16, 16);
            _avatar.SizeMode = PictureBoxSizeMode.Zoom;
            _avatar.BackColor = Color.FromArgb(30, 30, 45);

            _lblName.AutoSize = false;
            _lblName.Location = new Point(84, 18);
            _lblName.Size = new Size(280, 22);
            _lblName.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            _lblName.ForeColor = Color.White;
            _lblName.Text = "Steam user";

            _rows.Location = new Point(16, 86);
            _rows.Size = new Size(348, 150);
            _rows.FlowDirection = FlowDirection.TopDown;
            _rows.WrapContents = false;
            _rows.AutoScroll = false;
            _rows.BackColor = Color.Transparent;

            Controls.Add(_avatar);
            Controls.Add(_lblName);
            Controls.Add(_rows);

            Padding = new Padding(0);
        }

        public void SetHeader(string name, Image? avatar)
        {
            _lblName.Text = name;
            _avatar.Image = avatar;
        }

        public void ClearRows() => _rows.Controls.Clear();

        public void AddRow(string text, Image? icon = null)
        {
            var row = new Panel
            {
                Size = new Size(_rows.Width - 4, 24),
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 8)
            };

            PictureBox? pic = null;
            if (icon != null)
            {
                pic = new PictureBox
                {
                    Size = new Size(18, 18),
                    Location = new Point(0, 3),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = icon,
                    BackColor = Color.Transparent
                };
                row.Controls.Add(pic);
            }

            var lbl = new Label
            {
                AutoSize = false,
                Location = new Point(icon != null ? 24 : 0, 2),
                Size = new Size(row.Width - (icon != null ? 24 : 0), 20),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.Gainsboro,
                Text = text
            };

            row.Controls.Add(lbl);
            _rows.Controls.Add(row);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var r = 14;

            using var path = RoundedRect(rect, r);

            // фон (типа сине-фиолетовый)
            using var brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(16, 32, 80),
                Color.FromArgb(18, 14, 36),
                90f
            );
            e.Graphics.FillPath(brush, path);

            // лёгкая обводка
            using var pen = new Pen(Color.FromArgb(70, 120, 110, 255), 1f);
            e.Graphics.DrawPath(pen, path);

            // разделительная линия под шапкой
            using var linePen = new Pen(Color.FromArgb(80, 120, 110, 255), 1f);
            e.Graphics.DrawLine(linePen, 16, 74, Width - 16, 74);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
