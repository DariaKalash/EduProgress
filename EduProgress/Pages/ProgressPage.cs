using EduProgress.Models;

namespace EduProgress.Pages;

public partial class ProgressPage : BasePage
{
    public ProgressPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => pnlOuter.Width = Width;
        LoadData();
    }

    private void LoadData()
    {
        pnlOuter.Controls.Clear();
        var courses = AppSession.Db.GetCoursesForUser(AppSession.CurrentUser!.Id)
                                   .Where(c => c.Status != CourseStatus.Available).ToList();

        //pnlOuter.Controls.Add(Title("Мой прогресс обучения", new Padding(0, 0, 0, 20)));

        // Progress rings row
        var rings = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = true, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = AppTheme.Background, Margin = new Padding(0, 0, 0, 20) };
        foreach (var c in courses) rings.Controls.Add(ProgressCard(c));
        pnlOuter.Controls.Add(rings);

        // Summary
        int total = courses.Count;
        int done  = courses.Count(c => c.Status == CourseStatus.Completed);
        int hours = courses.Sum(c => c.TotalHours);
        int avg   = total > 0 ? courses.Sum(c => c.Progress) / total : 0;

        var sumCard = MakeCard(860, 80, 20);
        sumCard.Margin = new Padding(0, 0, 0, 20);
        void S(string lbl, string val, int x)
        {
            sumCard.Controls.Add(new Label { Text = val, Font = AppTheme.F(22f, FontStyle.Bold), ForeColor = AppTheme.Accent,      AutoSize = true, Location = new Point(x, 10) });
            sumCard.Controls.Add(new Label { Text = lbl, Font = AppTheme.FontSmall,              ForeColor = AppTheme.TextMuted,    AutoSize = true, Location = new Point(x, 44) });
        }
        S("Курсов записано",   total.ToString(),  20);
        S("Завершено",         done.ToString(),   190);
        S("Учебных часов",     hours.ToString(),  360);
        S("Средний прогресс",  $"{avg}%",         530);
        pnlOuter.Controls.Add(sumCard);

        // Table
        pnlOuter.Controls.Add(Title("Детальный прогресс по урокам", new Padding(0, 0, 0, 12)));

        var tableCard = MakeCard(860, -1, 0);
        tableCard.Margin = new Padding(0, 0, 0, 20);
        var grid = BuildGrid();

        foreach (var course in courses)
        {
            // To get full lesson data we need to call GetCourse
            var full = AppSession.Db.GetCourse(course.Id, AppSession.CurrentUser!.Id);
            if (full == null) continue;
            foreach (var mod in full.Modules)
            foreach (var les in mod.Lessons)
            {
                int ri = grid.Rows.Add(course.Title, mod.Title, les.Title, les.DurationLabel,
                    les.Status == LessonStatus.Completed ? "Завершён" :
                    les.Status == LessonStatus.InProgress ? "В процессе" : "Не начат");
                grid.Rows[ri].DefaultCellStyle.ForeColor = les.Status switch
                {
                    LessonStatus.Completed  => AppTheme.Success,
                    LessonStatus.InProgress => AppTheme.Accent,
                    _                       => AppTheme.TextMuted,
                };
            }
        }
        tableCard.Height = Math.Min(400, grid.Rows.Count * 28 + 70);
        tableCard.Controls.Add(grid);
        pnlOuter.Controls.Add(tableCard);
        pnlOuter.Width = Width;
    }

    private static Panel ProgressCard(Course c)
    {
        var card = MakeCard(200, 220, 16);
        card.Margin = new Padding(0, 0, 16, 16);

        var ring = new Panel { Size = new Size(90, 90), Location = new Point(55, 14), BackColor = Color.Transparent };
        Color ringColor = c.Status == CourseStatus.Completed ? AppTheme.Success : AppTheme.Accent;
        ring.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var bg = new Pen(AppTheme.BorderLight, 8);
            using var fg = new Pen(ringColor, 8);
            e.Graphics.DrawEllipse(bg, 5, 5, 80, 80);
            if (c.Progress > 0) e.Graphics.DrawArc(fg, 5, 5, 80, 80, -90, 360f * c.Progress / 100f);
            using var f = AppTheme.F(14f, FontStyle.Bold);
            using var b = new SolidBrush(AppTheme.TextPrimary);
            var t = $"{c.Progress}%";
            var s = e.Graphics.MeasureString(t, f);
            e.Graphics.DrawString(t, f, b, (90 - s.Width) / 2, (90 - s.Height) / 2);
        };

        var status = c.Status == CourseStatus.Completed ? MakeBadge("Завершён", AppTheme.Success)
                   : MakeBadge("В процессе", AppTheme.Accent);
        status.Location = new Point((200 - status.Width) / 2 - 16, 172);

        card.Controls.Add(ring);
        card.Controls.Add(new Label { Text = c.Title, Font = AppTheme.FontSmallBold, ForeColor = AppTheme.TextPrimary, AutoSize = false, Width = 168, Height = 36, Location = new Point(0, 110), TextAlign = ContentAlignment.TopCenter });
        card.Controls.Add(status);
        return card;
    }

    private static DataGridView BuildGrid()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill, BackgroundColor = AppTheme.White, BorderStyle = BorderStyle.None,
            RowHeadersVisible = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            Font = AppTheme.FontSmall, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 36,
        };
        g.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.Background;
        g.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextMuted;
        g.ColumnHeadersDefaultCellStyle.Font      = AppTheme.FontSmallBold;
        g.DefaultCellStyle.Padding                = new Padding(8, 4, 8, 4);
        g.DefaultCellStyle.SelectionBackColor     = Color.FromArgb(235, 246, 255);
        g.DefaultCellStyle.SelectionForeColor     = AppTheme.TextPrimary;
        g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Курс",      FillWeight = 30 });
        g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Модуль",    FillWeight = 25 });
        g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Урок",      FillWeight = 25 });
        g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Длит.",     FillWeight = 10 });
        g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Статус",    FillWeight = 10 });
        return g;
    }

    private static Label Title(string text, Padding margin) => new Label
    { Text = text, Font = AppTheme.FontH1, ForeColor = AppTheme.TextPrimary, AutoSize = true, Margin = margin, BackColor = Color.Transparent };
}
