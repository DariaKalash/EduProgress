using EduProgress.Models;

namespace EduProgress.Pages;

public partial class CatalogPage : BasePage
{
    private string _filter     = "all";
    private string _searchText = "";
    private List<Course> _courses = new();
    private readonly List<Button> _filterBtns = new();

    private readonly (string Key, string Label)[] _filterDefs =
    {
        ("all",       "Все"),
        ("enrolled",  "Активные"),
        ("completed", "Завершённые"),
        ("available", "Доступные"),
    };

    public CatalogPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => pnlOuter.Width = Width;
        //AddTitleLabel();
        BuildFilters();
        LoadData();
    }

    //private void AddTitleLabel()
    //{
    //    var lbl = new Label
    //    {
    //        Text      = "Каталог курсов",
    //        Font      = AppTheme.FontH1,
    //        ForeColor = AppTheme.TextPrimary,
    //        AutoSize  = true,
    //        Margin    = new Padding(0, 0, 0, 16),
    //    };
    //    pnlOuter.Controls.Add(lbl);
    //    pnlOuter.Controls.SetChildIndex(lbl, 0);
    //}

    private void BuildFilters()
    {
        for (int i = 0; i < _filterDefs.Length; i++)
        {
            int idx = i;
            var (key, label) = _filterDefs[i];
            var btn = new Button
            {
                Text      = label,
                Tag       = key,
                Size      = new Size(120, 34),
                FlatStyle = FlatStyle.Flat,
                Font      = AppTheme.FontSmall,
                Cursor    = Cursors.Hand,
                BackColor = i == 0 ? AppTheme.Accent : AppTheme.White,
                ForeColor = i == 0 ? Color.White      : AppTheme.TextPrimary,
            };
            btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.BorderColor = i == 0 ? AppTheme.Accent : AppTheme.Border;
            btn.Click += (_, _) =>
            {
                _filter = _filterDefs[idx].Key;
                for (int j = 0; j < _filterBtns.Count; j++)
                {
                    bool sel = _filterBtns[j].Tag?.ToString() == _filter;
                    _filterBtns[j].BackColor = sel ? AppTheme.Accent : AppTheme.White;
                    _filterBtns[j].ForeColor = sel ? Color.White      : AppTheme.TextPrimary;
                    _filterBtns[j].FlatAppearance.BorderColor = sel ? AppTheme.Accent : AppTheme.Border;
                }
                RefreshGrid();
            };
            _filterBtns.Add(btn);
            pnlFilters.Controls.Add(btn);
        }
    }

    private void LoadData()
    {
        _courses = AppSession.Db.GetCoursesForUser(AppSession.CurrentUser!.Id);
        RefreshGrid();
        pnlOuter.Width = Width;
    }

    private void RefreshGrid()
    {
        gridPanel.SuspendLayout();
        gridPanel.Controls.Clear();
        var filtered = _filter switch
        {
            "enrolled"  => _courses.Where(c => c.Status == CourseStatus.Enrolled),
            "completed" => _courses.Where(c => c.Status == CourseStatus.Completed),
            "available" => _courses.Where(c => c.Status == CourseStatus.Available),
            _           => _courses.AsEnumerable(),
        };
        if (!string.IsNullOrWhiteSpace(_searchText))
            filtered = filtered.Where(c =>
                c.Title.Contains(_searchText,       StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(_searchText, StringComparison.OrdinalIgnoreCase));

        foreach (var c in filtered) gridPanel.Controls.Add(CourseCard(c));

        if (gridPanel.Controls.Count == 0)
            gridPanel.Controls.Add(new Label
            {
                Text = "Курсы не найдены", Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextMuted, AutoSize = true,
            });
        gridPanel.ResumeLayout();
    }

    // ── Course card ────────────────────────────────────────────────────────

    private static Panel CourseCard(Course c)
    {
        var card = MakeCard(420, 280, 20);
        card.Margin = new Padding(0, 0, 20, 20);
        card.Cursor = Cursors.Hand;

        Color stripe = c.Category switch
        {
            "Педагогика" => AppTheme.Accent,
            "ИТ"         => AppTheme.Purple,
            "Наука"      => AppTheme.Warning,
            _            => AppTheme.Success,
        };
        card.Controls.Add(new Panel { Height = 5, Dock = DockStyle.Top, BackColor = stripe });

        var badges = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = false,
            Width = 420,
            Height = 25,
            Location = new Point(20, 22),
            BackColor = Color.Transparent,
            WrapContents = false,
        };
        badges.Controls.Add(MakeBadge(c.Category, stripe));
        if (c.IsNew) badges.Controls.Add(MakeBadge("Новый", AppTheme.Success));
        card.Controls.Add(badges);

        card.Controls.Add(new Label
        {
            Text     = c.Title,
            Font     = AppTheme.FontBodyBold,
            ForeColor = AppTheme.TextPrimary,
            AutoSize  = false,
            Width     = 380, Height = 50,
            Location  = new Point(20, 56),
        });
        card.Controls.Add(new Label
        {
            Text      = c.Description.Length > 90 ? c.Description[..90] + "…" : c.Description,
            Font      = AppTheme.FontSmall,
            //ForeColor = AppTheme.TextMuted,
            ForeColor = AppTheme.TextPrimary,
            AutoSize  = false,
            Width = 380, Height = 50,
            Location  = new Point(20, 108),
        });

        var meta = new Panel { Location = new Point(20, 168), Width = 230, Height = 20, BackColor = Color.Transparent };
        void AddMeta(string t, int x) => meta.Controls.Add(new Label { Text = t, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(x, 0) });
        AddMeta($"◷ {c.TotalHours}ч",     0);
        AddMeta($"◎ {c.EnrolledCount}",   80);

        if (c.Status != CourseStatus.Available)
        {
            var pb = MakeProgressBar(c.Progress, 230, 6);
            pb.Location = new Point(20, 196);
            card.Controls.Add(pb);
            card.Controls.Add(new Label { Text = $"{c.Progress}% завершено", Font = AppTheme.FontSmall,
                ForeColor = c.Status == CourseStatus.Completed ? AppTheme.Success : AppTheme.Accent,
                AutoSize = true, Location = new Point(20, 208) });
        }

        //var btn = c.Status == CourseStatus.Available ? MakePrimBtn("Записаться", 110, 32) : MakeSecBtn(c.Status == CourseStatus.Completed ? "Открыть" : "Продолжить", 110, 32);
        bool isAdminOrMethodist =
            AppSession.CurrentUser!.Role == UserRole.Administrator ||
            AppSession.CurrentUser!.Role == UserRole.Methodist;

        Button btn;

        if (isAdminOrMethodist)
        {
            btn = MakePrimBtn("Редактировать", 130, 32);

            btn.Click += (_, _) =>
            {
                AppSession.NavigateTo?.Invoke("courseeditor", c);
            };
        }
        else
        {
            btn = c.Status == CourseStatus.Available
                ? MakePrimBtn("Записаться", 110, 32)
                : MakeSecBtn(c.Status == CourseStatus.Completed ? "Открыть" : "Продолжить", 110, 32);

            btn.Click += (_, _) =>
            {
                if (c.Status == CourseStatus.Available)
                {
                    AppSession.Db.EnrollUser(AppSession.CurrentUser!.Id, c.Id);
                    c.Status = CourseStatus.Enrolled;
                }

                AppSession.NavigateTo?.Invoke("course",
                    AppSession.Db.GetCourse(c.Id, AppSession.CurrentUser!.Id) ?? c);
            };
        }

        btn.Location = new Point(20, 248);
        btn.Font     = AppTheme.FontSmall;
        btn.Click   += (_, _) =>
        {
            if (c.Status == CourseStatus.Available)
            {
                AppSession.Db.EnrollUser(AppSession.CurrentUser!.Id, c.Id);
                c.Status = CourseStatus.Enrolled;
            }
            AppSession.NavigateTo?.Invoke("course",
                AppSession.Db.GetCourse(c.Id, AppSession.CurrentUser!.Id) ?? c);
        };

        card.Controls.Add(meta);
        card.Controls.Add(btn);
        card.Click += (_, _) => btn.PerformClick();
        return card;
    }

    // ── Search events ──────────────────────────────────────────────────────

    private void txtSearch_Enter(object? s, EventArgs e)
    { if (txtSearch.Text == "Поиск по названию…") txtSearch.Text = ""; txtSearch.ForeColor = AppTheme.TextPrimary; }

    private void txtSearch_Leave(object? s, EventArgs e)
    { if (txtSearch.Text == "") { txtSearch.Text = "Поиск по названию…"; txtSearch.ForeColor = AppTheme.TextMuted; } }

    private void txtSearch_TextChanged(object? s, EventArgs e)
    {
        _searchText = txtSearch.Text == "Поиск по названию…" ? "" : txtSearch.Text;
        RefreshGrid();
    }
}
