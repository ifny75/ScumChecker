using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class AccountCard : UserControl
{
    // Данные карточки
    public Image? Avatar { get; set; }
    public string Title { get; set; } = "STEAM Account";
    public bool IsCurrent { get; set; } = false;

    public bool VacBanned { get; set; } = false;
    public string Registered { get; set; } = "-";
    public string RealName { get; set; } = "-";
    public string Country { get; set; } = "-";
    public string ProfileType { get; set; } = "-";
    public string GameOwnedSince { get; set; } = "-";

    // Внешний вид
    public int CornerRadius { get; set; } = 14;
    public Color CardBack1 { get; set; } = Color.FromArgb(16, 22, 50);
    public Color CardBack2 { get; set; } = Color.FromArgb(10, 14, 32);
    public Color Accent { get; set; } = Color.FromArgb(120, 110, 255);

    public AccountCard()
    {
        DoubleBuffered = true;
        Size = new Size(360, 250);
        BackColor = Color.Transparent;
        Margin = new Padding(12);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        // Тень
        using (var shadowPath = RoundedRect(new Rectangle(3, 4, rect.Width, rect.Height), CornerRadius))
        using (var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            g.FillPath(shadowBrush, shadowPath);

        // Фон (градиент)
        using (var path = RoundedRect(rect, CornerRadius))
        using (var lg = new LinearGradientBrush(rect, CardBack1, CardBack2, 90f))
            g.FillPath(lg, path);

        // Обводка
        using (var path = RoundedRect(rect, CornerRadius))
        using (var pen = new Pen(Color.FromArgb(70, 120, 110, 255), 1f))
            g.DrawPath(pen, path);

        // Верхний заголовок
        DrawText(g, Title, new Point(16, 14), FontStyle.Bold, 10f, Color.White);

        // Галочка current
        if (IsCurrent)
        {
            using var b = new SolidBrush(Accent);
            g.FillEllipse(b, Width - 30, 16, 12, 12);
            using var w = new SolidBrush(Color.White);
            g.FillEllipse(w, Width - 28, 18, 8, 8);
        }

        // Аватар
        var avRect = new Rectangle(16, 44, 56, 56);
        if (Avatar != null)
        {
            using var ap = RoundedRect(avRect, 12);
            g.SetClip(ap);
            g.DrawImage(Avatar, avRect);
            g.ResetClip();
            using var pen = new Pen(Color.FromArgb(80, 255, 255, 255));
            g.DrawPath(pen, RoundedRect(avRect, 12));
        }
        else
        {
            using var br = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
            g.FillEllipse(br, avRect);
        }

        // Полоска-разделитель как у них
        using (var pen = new Pen(Color.FromArgb(110, Accent), 2f))
            g.DrawLine(pen, 16, 118, Width - 16, 118);

        // Строки с иконками (можно заменить на свои картинки)
        int y = 128;
        DrawRow(g, "VAC бан: " + (VacBanned ? "Да" : "Нет"), y, VacBanned ? Color.FromArgb(230, 60, 60) : Color.FromArgb(110, 220, 140)); y += 22;
        DrawRow(g, "Регистрация: " + Registered, y, Color.Gainsboro); y += 22;
        DrawRow(g, "Имя: " + RealName, y, Color.Gainsboro); y += 22;
        DrawRow(g, "Страна: " + Country, y, Color.Gainsboro); y += 22;
        DrawRow(g, "Профиль: " + ProfileType, y, Color.Gainsboro); y += 22;
        DrawRow(g, "Игра скачана: " + GameOwnedSince, y, Color.Gainsboro);
    }

    private void DrawRow(Graphics g, string text, int y, Color color)
    {
        using var dot = new SolidBrush(Color.FromArgb(140, 255, 255, 255));
        g.FillEllipse(dot, 18, y + 4, 8, 8);
        DrawText(g, text, new Point(32, y), FontStyle.Regular, 9f, color);
    }

    private void DrawText(Graphics g, string txt, Point p, FontStyle style, float size, Color c)
    {
        using var f = new Font("Segoe UI", size, style);
        using var br = new SolidBrush(c);
        g.DrawString(txt, f, br, p);
    }

    private GraphicsPath RoundedRect(Rectangle r, int radius)
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
