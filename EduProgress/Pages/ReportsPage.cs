using EduProgress.Models;

namespace EduProgress.Pages;

public partial class ReportsPage : BasePage
{
    // (Title, Enrolled, Completed, AvgProgress)
    private List<(string Title, int Enrolled, int Completed, double AvgProgress)> _reportData = new();
    private List<User> _users = new();

    public ReportsPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => pnlOuter.Width = Width;
        LoadData();
    }

    private void LoadData()
    {
        _reportData = AppSession.Db.GetReportData();
        _users      = AppSession.Db.GetAllUsers();
        Build();
        pnlOuter.Width = Width;
    }

    private void Build()
    {
        pnlOuter.Controls.Clear();

        //pnlOuter.Controls.Add(MakeLbl("Отчётность и аналитика", AppTheme.FontH1,
        //    margin: new Padding(0, 0, 0, 20)));

        // ── Summary stat cards ─────────────────────────────────────────────────
        int totalCourses    = _reportData.Count;
        int activeTeachers  = _users.Count(u => u.IsActive && u.Role == UserRole.Teacher);
        int avgCompletion   = totalCourses > 0 ? (int)_reportData.Average(r => r.AvgProgress) : 0;
        int totalEnrollments = _reportData.Sum(r => r.Enrolled);

        var statsRow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize      = true,
            BackColor     = AppTheme.Background,
            Margin        = new Padding(0, 0, 0, 20),
        };
        statsRow.Controls.Add(MakeStatCard("Курсов в системе",    totalCourses.ToString(),     AppTheme.Accent));
        statsRow.Controls.Add(MakeStatCard("Активных слушателей", activeTeachers.ToString(),   AppTheme.Success));
        statsRow.Controls.Add(MakeStatCard("Средн. завершение",   $"{avgCompletion}%",         AppTheme.Warning));
        statsRow.Controls.Add(MakeStatCard("Всего записей",       totalEnrollments.ToString(), AppTheme.Purple));
        pnlOuter.Controls.Add(statsRow);

        // ── Filter row ─────────────────────────────────────────────────────────
        var filterRow = new Panel { Height = 44, Width = 860, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 16) };
        var filterLbl = MakeLbl("Фильтр по курсу:", AppTheme.FontSmallBold);
        filterLbl.Location = new Point(0, 12);
        var courseCombo = new ComboBox
        {
            Location      = new Point(140, 8),
            Size          = new Size(280, 30),
            Font          = AppTheme.FontBody,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
        courseCombo.Items.Add("Все курсы");
        foreach (var r in _reportData) courseCombo.Items.Add(r.Title);
        courseCombo.SelectedIndex = 0;

        filterRow.Controls.AddRange(new Control[] { filterLbl, courseCombo });
        pnlOuter.Controls.Add(filterRow);

        // ── Bar chart card ─────────────────────────────────────────────────────
        var chartCard = MakeCard(860, 260, 20);
        chartCard.Margin = new Padding(0, 0, 0, 20);

        var chartTitle = MakeLbl("Прогресс по курсам (%)", AppTheme.FontH2);
        chartTitle.Location = new Point(20, 16);

        var chartPanel = new Panel { Location = new Point(20, 46), Size = new Size(820, 200), BackColor = Color.Transparent };
        var snapData   = _reportData.ToList();   // capture for closure
        chartPanel.Paint += (_, e) => DrawBarChart(e.Graphics, chartPanel.Width, chartPanel.Height, snapData);

        chartCard.Controls.AddRange(new Control[] { chartTitle, chartPanel });
        pnlOuter.Controls.Add(chartCard);

        // ── Course data table ──────────────────────────────────────────────────
        pnlOuter.Controls.Add(MakeLbl("Детализация по курсам", AppTheme.FontH2,
            margin: new Padding(0, 0, 0, 12)));

        var tableCard = MakeCard(860, 220, 0);
        tableCard.Margin = new Padding(0, 0, 0, 20);
        tableCard.Controls.Add(BuildCourseTable(_reportData));
        pnlOuter.Controls.Add(tableCard);

        // ── User activity table ────────────────────────────────────────────────
        pnlOuter.Controls.Add(MakeLbl("Активность пользователей", AppTheme.FontH2,
            margin: new Padding(0, 0, 0, 12)));

        var userCard = MakeCard(860, 260, 0);
        userCard.Margin = new Padding(0);
        userCard.Controls.Add(BuildUserTable(_users));
        pnlOuter.Controls.Add(userCard);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Panel MakeStatCard(string label, string value, Color accent)
    {
        var card = MakeCard(200, 100, 0);
        card.Margin = new Padding(0, 0, 16, 0);
        var topBar = new Panel { Height = 4, Dock = DockStyle.Top, BackColor = accent };
        var valLbl = new Label { Text = value, Font = AppTheme.F(26f, FontStyle.Bold), ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 18) };
        var lblLbl = new Label { Text = label, Font = AppTheme.FontSmall,             ForeColor = AppTheme.TextMuted,    AutoSize = true, Location = new Point(16, 64) };
        card.Controls.AddRange(new Control[] { topBar, valLbl, lblLbl });
        return card;
    }

    private static void DrawBarChart(Graphics g, int w, int h,
        List<(string Title, int Enrolled, int Completed, double AvgProgress)> data)
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        if (data.Count == 0) return;

        int count  = data.Count;
        int barW   = Math.Min(80, (w - 60) / count - 20);
        int gap    = (w - 40 - count * barW) / (count + 1);
        int chartH = h - 50;

        using var gridPen = new Pen(AppTheme.BorderLight, 1);
        for (int pct = 0; pct <= 100; pct += 25)
        {
            int y = chartH - (int)(chartH * pct / 100.0);
            g.DrawLine(gridPen, 30, y, w - 10, y);
            using var lbFont  = AppTheme.F(8f);
            using var lbBrush = new SolidBrush(AppTheme.TextLight);
            g.DrawString($"{pct}%", lbFont, lbBrush, 2, y - 6);
        }

        Color[] barColors = { AppTheme.Accent, AppTheme.Success, AppTheme.Warning, AppTheme.Purple };
        for (int i = 0; i < count; i++)
        {
            var r  = data[i];
            int x  = 40 + gap + i * (barW + gap);
            int bH = (int)(chartH * r.AvgProgress / 100.0);
            int y  = chartH - bH;

            using var brush = new SolidBrush(barColors[i % barColors.Length]);
            g.FillRectangle(brush, x, y, barW, bH);

            using var vFont  = AppTheme.F(9f, FontStyle.Bold);
            using var vBrush = new SolidBrush(AppTheme.TextPrimary);
            g.DrawString($"{r.AvgProgress:F0}%", vFont, vBrush, x + barW / 2 - 10, y - 16);

            using var nFont  = AppTheme.F(8f);
            using var nBrush = new SolidBrush(AppTheme.TextMuted);
            string shortName = r.Title.Length > 14 ? r.Title[..14] + "…" : r.Title;
            g.DrawString(shortName, nFont, nBrush, x - 4, chartH + 8);
        }
    }

    private static DataGridView BuildCourseTable(
        List<(string Title, int Enrolled, int Completed, double AvgProgress)> data)
    {
        var grid = MakeGrid();
        grid.Height = 200;
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Курс",            FillWeight = 40 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Слушателей",      FillWeight = 15 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Завершили",       FillWeight = 15 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Средн. прогресс", FillWeight = 15 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "% завершения",    FillWeight = 15 });

        foreach (var r in data)
        {
            double finishRate = r.Enrolled > 0 ? (double)r.Completed / r.Enrolled * 100 : 0;
            int ri = grid.Rows.Add(r.Title, r.Enrolled, r.Completed,
                $"{r.AvgProgress:F1}%", $"{finishRate:F0}%");
            grid.Rows[ri].DefaultCellStyle.ForeColor = r.AvgProgress >= 100 ? AppTheme.Success
                : r.AvgProgress > 0 ? AppTheme.Accent : AppTheme.TextMuted;
        }
        return grid;
    }

    private static DataGridView BuildUserTable(List<User> users)
    {
        var grid = MakeGrid();
        grid.Height = 240;
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Имя",             FillWeight = 30 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email",           FillWeight = 25 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Роль",            FillWeight = 15 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Последний визит", FillWeight = 20 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Статус",          FillWeight = 10 });

        foreach (var u in users)
        {
            int ri = grid.Rows.Add(u.Name, u.Email, u.RoleLabel,
                u.LastActive.ToString("dd.MM.yyyy HH:mm"),
                u.IsActive ? "Активен" : "Неактивен");
            grid.Rows[ri].DefaultCellStyle.ForeColor = u.IsActive ? AppTheme.TextPrimary : AppTheme.TextMuted;
        }
        return grid;
    }

    private static DataGridView MakeGrid()
    {
        var g = new DataGridView
        {
            Dock                  = DockStyle.Fill,
            BackgroundColor       = AppTheme.White,
            BorderStyle           = BorderStyle.None,
            RowHeadersVisible     = false,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            ReadOnly              = true,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            Font                  = AppTheme.FontSmall,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight   = 36,
        };
        g.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.Background;
        g.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextMuted;
        g.ColumnHeadersDefaultCellStyle.Font      = AppTheme.FontSmallBold;
        g.ColumnHeadersDefaultCellStyle.Padding   = new Padding(8, 0, 0, 0);
        g.DefaultCellStyle.Padding                = new Padding(8, 4, 8, 4);
        g.DefaultCellStyle.SelectionBackColor     = Color.FromArgb(235, 246, 255);
        g.DefaultCellStyle.SelectionForeColor     = AppTheme.TextPrimary;
        return g;
    }

    private static Label MakeLbl(string t, Font f, Color? c = null, Padding margin = default)
        => new Label { Text = t, Font = f, ForeColor = c ?? AppTheme.TextPrimary, AutoSize = true, Margin = margin, BackColor = Color.Transparent };
}
