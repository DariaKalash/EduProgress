using EduProgress.Controls;

namespace EduProgress.Pages;

/// <summary>
/// Common helpers shared by all page UserControls.
/// </summary>
public abstract class BasePage : UserControl
{
    protected BasePage()
    {
        Dock      = DockStyle.Fill;
        BackColor = AppTheme.Background;
    }

    // ── Layout helpers ────────────────────────────────────────────────────────

    protected static Panel MakeCard(int? width = null, int? height = null, int padding = 20)
    {
        var c = new Panel
        {
            BackColor = AppTheme.White,
            Padding   = new Padding(padding),
        };
        if (width  != null) c.Width  = width.Value;
        if (height != null) c.Height = height.Value;
        c.Paint += (s, e) =>
        {
            using var pen = new Pen(AppTheme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
        };
        return c;
    }

    protected static Button MakePrimBtn(string text, int w = 150, int h = 38)
    {
        var b = new Button
        {
            Text      = text,
            Size      = new Size(w, h),
            BackColor = AppTheme.Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = AppTheme.FontBodyBold,
            Cursor    = Cursors.Hand,
        };
        b.FlatAppearance.BorderSize = 0;
        return b;
    }

    protected static Button MakeSecBtn(string text, int w = 150, int h = 38)
    {
        var b = new Button
        {
            Text      = text,
            Size      = new Size(w, h),
            BackColor = AppTheme.White,
            ForeColor = AppTheme.Accent,
            FlatStyle = FlatStyle.Flat,
            Font      = AppTheme.FontBodyBold,
            Cursor    = Cursors.Hand,
        };
        b.FlatAppearance.BorderColor = AppTheme.Accent;
        b.FlatAppearance.BorderSize  = 1;
        return b;
    }

    protected static Label MakeLbl(string text, Font? font = null, Color? color = null,
        bool autoSize = true)
        => new Label
        {
            Text      = text,
            Font      = font  ?? AppTheme.FontBody,
            ForeColor = color ?? AppTheme.TextPrimary,
            AutoSize  = autoSize,
            BackColor = Color.Transparent,
        };

    protected static Panel MakeBadge(string text, Color back)
    {
        int w = TextRenderer.MeasureText(text, AppTheme.FontSmallBold).Width + 16;
        var p = new Panel { Size = new Size(w, 22), BackColor = back };
        p.Controls.Add(new Label
        {
            Text      = text,
            Font      = AppTheme.FontSmallBold,
            ForeColor = Color.White,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        });
        return p;
    }

    protected static CustomProgressBar MakeProgressBar(int value, int w = 200, int h = 8,
        Color? color = null)
        => new CustomProgressBar
        {
            Width     = w,
            Height    = h,
            Value     = value,
            FillColor = color ?? AppTheme.Success,
            Rounded   = true,
        };

    protected static Panel MakeDivider(bool horizontal = true)
    {
        var d = new Panel { BackColor = AppTheme.Border };
        if (horizontal) { d.Height = 1; d.Dock = DockStyle.Top; }
        else            { d.Width  = 1; d.Dock = DockStyle.Left; }
        return d;
    }

    protected static Panel MakeScrollContent(int topPad = 24, int sidePad = 24)
    {
        return new Panel
        {
            AutoSize     = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding      = new Padding(sidePad, topPad, sidePad, topPad),
        };
    }
}
