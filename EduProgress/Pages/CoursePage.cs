using EduProgress.Models;

namespace EduProgress.Pages;

public partial class CoursePage : BasePage
{
    private readonly Course _course;

    public CoursePage(Course course)
    {
        _course = course;
        InitializeComponent();
        BuildBanner();
        BuildTree();
        BuildDetail();

        //this.Load += (_, _) =>
        //{
        //    if (this.Width > split.Panel1MinSize + split.Panel2MinSize)
        //        split.SplitterDistance = 260;
        //};
        this.Load += (_, _) =>
        {
            var totalMin = split.Panel1MinSize + split.Panel2MinSize;

            if (this.Width > totalMin)
                split.SplitterDistance = 260;
            else
                split.SplitterDistance = split.Panel1MinSize;
        };
    }

    // ── Banner ────────────────────────────────────────────────────────────

    private void BuildBanner()
    {
        pnlBanner.Controls.Add(new Label
        {
            Text = _course.Title, Font = AppTheme.FontH1, ForeColor = Color.White,
            AutoSize = false, Width = 700, Height = 34, Location = new Point(28, 22),
        });
        var cat = MakeBadge(_course.Category, Color.FromArgb(80, 255, 255, 255));
        cat.Location = new Point(28, 60);

        pnlBanner.Controls.Add(cat);
        pnlBanner.Controls.Add(new Label
        {
            Text = $"◷ {_course.TotalHours} ч   ≡ {_course.ModulesCount} модулей   ✦ {_course.TotalLessons} уроков",
            Font = AppTheme.FontSmall, ForeColor = Color.FromArgb(200, 255, 255, 255),
            AutoSize = true, Location = new Point(28, 88),
        });

        var ring = new Panel { Size = new Size(80, 80), Location = new Point(880, 28), BackColor = Color.Transparent };
        ring.Paint += (_, e) => DrawRing(e.Graphics, _course.Progress);
        pnlBanner.Controls.Add(ring);
    }

    // ── Tree ──────────────────────────────────────────────────────────────

    private void BuildTree()
    {
        var hdrLbl = MakeLbl("Содержание курса", AppTheme.FontH2);
        hdrLbl.Location = new Point(16, 12);
        pnlTreeHdr.Controls.Add(hdrLbl);

        foreach (var mod in _course.Modules.OrderBy(m => m.Order))
        {
            treeFlow.Controls.Add(ModuleRow(mod));
            foreach (var lesson in mod.Lessons.OrderBy(l => l.Order))
                treeFlow.Controls.Add(LessonRow(lesson));
        }
    }

    // ── Detail ────────────────────────────────────────────────────────────

    private void BuildDetail()
    {
        var innerFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents  = false,
            AutoSize      = true,
            AutoSizeMode  = AutoSizeMode.GrowAndShrink,
            BackColor     = AppTheme.Background,
            Padding       = new Padding(24),
        };

        // Progress card
        var progCard = MakeCard(500, 110, 20);
        progCard.Margin = new Padding(0, 0, 0, 16);
        var pbar  = MakeProgressBar(_course.Progress, 460, 10); pbar.Location = new Point(20, 50);
        var ptxt  = MakeLbl($"{_course.Progress}% завершено  ({_course.DoneLessons} из {_course.TotalLessons} уроков)", AppTheme.FontSmall, AppTheme.TextMuted);
        ptxt.Location = new Point(20, 68);
        var pTitle = MakeLbl("Прогресс курса", AppTheme.FontH2); pTitle.Location = new Point(20, 16);
        var contBtn = MakePrimBtn("Продолжить обучение", 200, 36); contBtn.Location = new Point(20, 86);
        contBtn.Click += (_, _) =>
        {
            var next = _course.Modules.SelectMany(m => m.Lessons)
                              .FirstOrDefault(l => l.Status == LessonStatus.InProgress)
                    ?? _course.Modules.SelectMany(m => m.Lessons)
                              .FirstOrDefault(l => l.Status == LessonStatus.NotStarted);
            if (next != null) AppSession.NavigateTo?.Invoke("lesson", next);
        };
        progCard.Controls.AddRange(new Control[] { pTitle, pbar, ptxt, contBtn });

        // Description card
        var descCard = MakeCard(500, 120, 20);
        descCard.Margin = new Padding(0, 0, 0, 16);
        var dTitle = MakeLbl("О курсе", AppTheme.FontH2); dTitle.Location = new Point(20, 16);
        var dText  = new Label { Text = _course.Description, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary, AutoSize = false, Width = 460, Height = 80, Location = new Point(20, 44) };
        descCard.Controls.AddRange(new Control[] { dTitle, dText });

        innerFlow.Controls.Add(progCard);
        innerFlow.Controls.Add(descCard);
        pnlDetail.Controls.Add(innerFlow);
    }

    // ── Tree rows ──────────────────────────────────────────────────────────

    private static Panel ModuleRow(Module mod)
    {
        var row = new Panel { Width = 260, Height = 42, BackColor = AppTheme.Background, Margin = new Padding(0) };
        row.Controls.Add(new Label { Text = $"≡  {mod.Title}", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary, AutoSize = false, Width = 240, Height = 42, TextAlign = ContentAlignment.MiddleLeft });
        row.Paint += DrawBottomBorder;
        return row;
    }

    private static Panel LessonRow(Lesson lesson)
    {
        var row = new Panel
        {
            Width     = 260, Height = 40,
            BackColor = lesson.Status == LessonStatus.InProgress ? Color.FromArgb(235, 246, 255) : AppTheme.White,
            Margin    = new Padding(0), Cursor = Cursors.Hand,
        };
        row.Paint += DrawBottomBorder;

        string icon  = lesson.Status == LessonStatus.Completed  ? "✓"
                     : lesson.Status == LessonStatus.InProgress ? "▶" : "○";
        Color icCol  = lesson.Status == LessonStatus.Completed  ? AppTheme.Success
                     : lesson.Status == LessonStatus.InProgress ? AppTheme.Accent : AppTheme.TextLight;

        row.Controls.Add(new Label { Text = icon, Font = AppTheme.F(10f), ForeColor = icCol, Size = new Size(28, 40), Location = new Point(24, 0), TextAlign = ContentAlignment.MiddleCenter });
        row.Controls.Add(new Label { Text = lesson.Title, Font = AppTheme.FontSmall, ForeColor = lesson.Status == LessonStatus.Completed ? AppTheme.TextMuted : AppTheme.TextPrimary, AutoSize = false, Width = 200, Height = 40, Location = new Point(52, 0), TextAlign = ContentAlignment.MiddleLeft });

        row.Click += (_, _) => AppSession.NavigateTo?.Invoke("lesson", lesson);
        foreach (Control c in row.Controls) c.Click += (_, _) => AppSession.NavigateTo?.Invoke("lesson", lesson);
        return row;
    }

    // ── Paint ─────────────────────────────────────────────────────────────

    private static void DrawRing(Graphics g, int value)
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var bgPen = new Pen(Color.FromArgb(60, 255, 255, 255), 7);
        using var fgPen = new Pen(AppTheme.Success, 7);
        g.DrawEllipse(bgPen, 4, 4, 72, 72);
        if (value > 0) g.DrawArc(fgPen, 4, 4, 72, 72, -90, 360f * value / 100f);
        using var f  = AppTheme.F(12f, FontStyle.Bold);
        using var tb = new SolidBrush(Color.White);
        var t = $"{value}%";
        var s = g.MeasureString(t, f);
        g.DrawString(t, f, tb, (80 - s.Width) / 2, (80 - s.Height) / 2);
    }

    private void pnlBanner_Paint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var b1 = new SolidBrush(Color.FromArgb(15, 255, 255, 255));
        using var b2 = new SolidBrush(Color.FromArgb(8,  255, 255, 255));
        g.FillEllipse(b1, pnlBanner.Width - 200, -80,  260, 260);
        g.FillEllipse(b2, pnlBanner.Width - 380, pnlBanner.Height - 100, 200, 200);
    }

    private void pnlTreeHdr_Paint(object? s, PaintEventArgs e)
    {
        using var pen = new Pen(AppTheme.BorderLight, 1);
        e.Graphics.DrawLine(pen, 0, pnlTreeHdr.Height - 1, pnlTreeHdr.Width, pnlTreeHdr.Height - 1);
    }

    private static void DrawBottomBorder(object? s, PaintEventArgs e)
    {
        if (s is not Control c) return;
        using var pen = new Pen(AppTheme.BorderLight, 1);
        e.Graphics.DrawLine(pen, 0, c.Height - 1, c.Width, c.Height - 1);
    }

    private static Label MakeLbl(string t, Font f, Color? c = null)
        => new Label { Text = t, Font = f, ForeColor = c ?? AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent };
}
