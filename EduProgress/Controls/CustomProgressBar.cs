namespace EduProgress.Controls;

public class CustomProgressBar : Panel
{
    private int   _value     = 0;
    private Color _fillColor = AppTheme.Success;
    private bool  _rounded   = false;

    public int Value
    {
        get => _value;
        set { _value = Math.Clamp(value, 0, 100); Invalidate(); }
    }

    public Color FillColor
    {
        get => _fillColor;
        set { _fillColor = value; Invalidate(); }
    }

    public bool Rounded
    {
        get => _rounded;
        set { _rounded = value; Invalidate(); }
    }

    public CustomProgressBar()
    {
        Height    = 8;
        BackColor = AppTheme.BorderLight;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (_value <= 0) return;

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        int fillWidth = Math.Max(0, (int)(Width * _value / 100.0));
        using var brush = new SolidBrush(_fillColor);

        if (_rounded && Height >= 4)
        {
            int r = Height / 2;
            // draw background
            using var bgBrush = new SolidBrush(BackColor);
            using var bgPath  = RoundedRect(new Rectangle(0, 0, Width, Height), r);
            e.Graphics.FillPath(bgBrush, bgPath);
            // draw fill
            if (fillWidth > r * 2)
            {
                using var fillPath = RoundedRect(new Rectangle(0, 0, fillWidth, Height), r);
                e.Graphics.FillPath(brush, fillPath);
            }
        }
        else
        {
            e.Graphics.FillRectangle(brush, 0, 0, fillWidth, Height);
        }
    }

    private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return path;
    }
}
