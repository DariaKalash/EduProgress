using EduProgress.Models;

namespace EduProgress.Pages;

public partial class DashboardPage : BasePage
{
    public DashboardPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => pnlOuter.Width = Width;
        LoadData();
    }

    private void LoadData()
    {
        pnlOuter.Controls.Clear();
        var u      = AppSession.CurrentUser!;
        var courses = AppSession.Db.GetCoursesForUser(u.Id);
        var notifs  = AppSession.Db.GetNotificationsForUser(u.Id);

        pnlOuter.Controls.Add(BuildGreeting(u));
        pnlOuter.Controls.Add(BuildStatsRow(courses));
        pnlOuter.Controls.Add(BuildMainContent(courses, notifs));
        pnlOuter.Width = Width;
    }

    private static Panel BuildGreeting(User u)
    {
        var p  = new Panel { Height = 70, Width = 900, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 16) };
        var g  = MakeLbl($"Добро пожаловать, {u.Name.Split(' ')[0]}!", AppTheme.FontH1);
        var rl = MakeLbl($"{u.RoleLabel} · {DateTime.Now:dddd, d MMMM yyyy}", AppTheme.FontBody, AppTheme.TextMuted);
        g.Location  = new Point(0, 4);
        rl.Location = new Point(0, 36);
        p.Controls.AddRange(new Control[] { g, rl });
        return p;
    }

    private static FlowLayoutPanel BuildStatsRow(List<Course> courses)
    {
        int enrolled  = courses.Count(c => c.Status == CourseStatus.Enrolled);
        int completed = courses.Count(c => c.Status == CourseStatus.Completed);
        int hours     = courses.Where(c => c.Status != CourseStatus.Available).Sum(c => c.TotalHours);
        int avg       = enrolled > 0 ? courses.Where(c => c.Status == CourseStatus.Enrolled)
                                               .Sum(c => c.Progress) / enrolled : 0;

        var row = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents  = false,
            AutoSize      = true,
            Margin        = new Padding(0, 0, 0, 20),
        };
        row.Controls.Add(StatCard("Всего курсов",       courses.Count.ToString(), AppTheme.Accent,   "≡"));
        row.Controls.Add(StatCard("Записан",            enrolled.ToString(),      AppTheme.Purple,   "◈"));
        row.Controls.Add(StatCard("Завершено",          completed.ToString(),     AppTheme.Success,  "✓"));
        row.Controls.Add(StatCard("Средний прогресс",   $"{avg}%",               AppTheme.Warning,  "%"));
        return row;
    }

    private static Panel StatCard(string label, string value, Color accent, string icon)
    {
        var card = MakeCard(180, 110, 0);
        card.Margin = new Padding(0, 0, 16, 0);
        card.Controls.Add(new Panel { Height = 4, Dock = DockStyle.Top, BackColor = accent });
        card.Controls.Add(new Label { Text = icon,  Font = AppTheme.F(18f),         ForeColor = accent,             AutoSize = true, Location = new Point(16, 18) });
        card.Controls.Add(new Label { Text = value, Font = AppTheme.FontStat,        ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(14, 36) });
        card.Controls.Add(new Label { Text = label, Font = AppTheme.FontSmall,       ForeColor = AppTheme.TextMuted,   AutoSize = true, Location = new Point(16, 82) });
        return card;
    }

    private static Panel BuildMainContent(List<Course> courses, List<Notification> notifs)
    {
        var row = new Panel { Height = 420, Width = 900, BackColor = Color.Transparent };

        var leftCard = MakeCard(560, 420, 0);
        leftCard.Location = new Point(0, 0);
        var hdr = new Panel { Height = 48, Dock = DockStyle.Top, BackColor = AppTheme.White };
        var ht  = MakeLbl("Активные курсы", AppTheme.FontH2);
        ht.Location = new Point(20, 12);
        hdr.Controls.Add(ht);
        hdr.Paint += (s, e) => { using var pen = new Pen(AppTheme.BorderLight); e.Graphics.DrawLine(pen, 0, hdr.Height - 1, hdr.Width, hdr.Height - 1); };

        var courseFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents  = false,
            Dock          = DockStyle.Fill,
            BackColor     = AppTheme.White,
            Padding       = new Padding(16, 8, 16, 8),
        };
        foreach (var c in courses.Where(c => c.Status == CourseStatus.Enrolled))
            courseFlow.Controls.Add(CourseRow(c));
        if (!courseFlow.Controls.Count.Equals(0) == false)
            courseFlow.Controls.Add(MakeLbl("Нет активных курсов", AppTheme.FontBody, AppTheme.TextMuted));

        leftCard.Controls.Add(courseFlow);
        leftCard.Controls.Add(hdr);

        var rightCard = MakeCard(240, 420, 0);
        rightCard.Location = new Point(576, 0);
        var rh = new Panel { Height = 48, Dock = DockStyle.Top, BackColor = AppTheme.White };
        var rt = MakeLbl("Уведомления", AppTheme.FontH2);
        rt.Location = new Point(16, 12);
        rh.Controls.Add(rt);
        rh.Paint += (s, e) => { using var pen = new Pen(AppTheme.BorderLight); e.Graphics.DrawLine(pen, 0, rh.Height - 1, rh.Width, rh.Height - 1); };

        var nFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents  = false,
            Dock          = DockStyle.Fill,
            BackColor     = AppTheme.White,
        };
        foreach (var n in notifs.Take(5)) nFlow.Controls.Add(MiniNotif(n));

        rightCard.Controls.Add(nFlow);
        rightCard.Controls.Add(rh);

        row.Controls.Add(leftCard);
        row.Controls.Add(rightCard);
        return row;
    }

    private static Panel CourseRow(Course c)
    {
        var row = new Panel { Width = 524, Height = 100, BackColor = AppTheme.White, Padding = new Padding(0, 12, 0, 0) };
        row.Paint += (s, e) => { using var pen = new Pen(AppTheme.BorderLight); e.Graphics.DrawLine(pen, 0, row.Height - 1, row.Width, row.Height - 1); };

        var title = MakeLbl(c.Title, AppTheme.FontBodyBold); title.Location = new Point(0, 4); title.MaximumSize = new Size(380, 0);
        var cat   = MakeLbl(c.Category, AppTheme.FontSmall, AppTheme.TextMuted); cat.Location = new Point(0, 26);
        var pct   = MakeLbl($"{c.Progress}%", AppTheme.FontSmallBold, AppTheme.Success); pct.Location = new Point(460, 26);
        var bar   = MakeProgressBar(c.Progress, 524, 6); bar.Location = new Point(0, 48);

        var btn = MakePrimBtn("Продолжить", 110, 30);
        btn.Location = new Point(0, 60); btn.Font = AppTheme.FontSmall; btn.Tag = c;
        btn.Click   += (_, _) => AppSession.NavigateTo?.Invoke("course", c);

        row.Controls.AddRange(new Control[] { title, cat, pct, bar, btn });
        return row;
    }

    private static Panel MiniNotif(Notification n)
    {
        var row = new Panel { Width = 240, Height = 70, BackColor = n.IsRead ? AppTheme.White : Color.FromArgb(235, 246, 255) };
        row.Paint += (s, e) => { using var pen = new Pen(AppTheme.BorderLight); e.Graphics.DrawLine(pen, 0, row.Height - 1, row.Width, row.Height - 1); };

        Color dc = n.Category == NotificationCategory.Reminder ? AppTheme.CatReminder
                 : n.Category == NotificationCategory.System   ? AppTheme.CatSystem : AppTheme.CatCourse;
        var dot = new Panel { Size = new Size(8, 8), Location = new Point(12, 14), BackColor = dc };
        dot.Paint += (_, e) => { e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; using var b = new SolidBrush(dc); e.Graphics.FillEllipse(b, 0, 0, 7, 7); };

        var tl = new Label { Text = n.Title, Font = AppTheme.FontSmallBold, ForeColor = AppTheme.TextPrimary, AutoSize = false, Width = 208, Height = 18, Location = new Point(26, 8) };
        var ml = new Label { Text = n.Message.Length > 60 ? n.Message[..60] + "…" : n.Message, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = false, Width = 208, Height = 30, Location = new Point(26, 28) };
        row.Controls.AddRange(new Control[] { dot, tl, ml });
        return row;
    }

    private static Label MakeLbl(string text, Font font, Color? color = null) => new Label
    { Text = text, Font = font, ForeColor = color ?? AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent };
}
