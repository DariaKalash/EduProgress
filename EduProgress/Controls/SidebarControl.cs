using EduProgress.Models;

namespace EduProgress.Controls;

public partial class SidebarControl : UserControl
{
    public event EventHandler<string>? NavClicked;

    private readonly List<Button> _navButtons = new();
    private string _activePage = "dashboard";

    private readonly (string Page, string Icon, string Label, UserRole[]? Roles)[] _items =
    {
        ("dashboard",     "◈",  "Главная",          null),
        ("catalog",       "≡",  "Каталог курсов",    null),
        ("progress",      "◉",  "Мой прогресс",      new[] { UserRole.Teacher }),
        ("notifications", "◎",  "Уведомления",       null),
        ("faq",           "?",  "FAQ",               new[] { UserRole.Teacher }),
        ("reports",       "≣",  "Отчётность",        new[] { UserRole.Administrator, UserRole.Methodist }),
        ("users",         "⊕",  "Пользователи",      new[] { UserRole.Administrator }),
        ("courseeditor",  "✎",  "Редактор курсов",   new[] { UserRole.Methodist, UserRole.Administrator }),
    };

    public SidebarControl()
    {
        InitializeComponent();
        BuildUserCard();
        BuildNavItems();
    }

    // ── Build ──────────────────────────────────────────────────────────────

    private void BuildUserCard()
    {
        var u = AppSession.CurrentUser;
        if (u == null) return;

        // Avatar (drawn)
        var avatar = new Panel { Size = new Size(40, 40), Location = new Point(12, 16) };
        avatar.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var b = new SolidBrush(AppTheme.Accent);
            e.Graphics.FillEllipse(b, 0, 0, 39, 39);
            using var tf = AppTheme.F(12f, FontStyle.Bold);
            using var tb = new SolidBrush(Color.White);
            var sz = e.Graphics.MeasureString(u.Initials, tf);
            e.Graphics.DrawString(u.Initials, tf, tb, (40 - sz.Width) / 2, (40 - sz.Height) / 2);
        };

        var shortName = u.Name.Split(' ') is var p && p.Length >= 2
            ? $"{p[0]} {p[1][0]}." : u.Name;

        pnlUser.Controls.Add(avatar);
        pnlUser.Controls.Add(new Label
        {
            Text      = shortName,
            ForeColor = Color.White,
            Font      = AppTheme.FontSmallBold,
            AutoSize  = false,
            Size      = new Size(170, 18),
            Location  = new Point(58, 18),
            TextAlign = ContentAlignment.MiddleLeft,
        });
        pnlUser.Controls.Add(new Label
        {
            Text      = u.RoleLabel,
            ForeColor = AppTheme.SidebarText,
            Font      = AppTheme.FontSmall,
            AutoSize  = false,
            Size      = new Size(170, 16),
            Location  = new Point(58, 38),
            TextAlign = ContentAlignment.MiddleLeft,
        });
    }

    //private void BuildNavItems()
    //{
    //    pnlNavFlow.Controls.Clear();
    //    _navButtons.Clear();

    //    var role = AppSession.CurrentUser?.Role ?? UserRole.Teacher;
    //    int unread = AppSession.Db.GetNotificationsForUser(
    //        AppSession.CurrentUser?.Id ?? 0).Count(n => !n.IsRead);

    //    foreach (var item in _items)
    //    {
    //        if (item.Roles != null && !item.Roles.Contains(role)) continue;

    //        string label = item.Page == "notifications" && unread > 0
    //            ? $"{item.Label}  ({unread})" : item.Label;

    //        var btn = CreateNavBtn(item.Page, item.Icon, label);
    //        pnlNavFlow.Controls.Add(btn);
    //        _navButtons.Add(btn);
    //    }
    //    SetActive(_activePage);
    //}

    private void BuildNavItems()
    {
        pnlNavFlow.Controls.Clear();
        _navButtons.Clear();

        var role = AppSession.CurrentUser?.Role ?? UserRole.Teacher;

        int unread = AppSession.Db.GetNotificationsForUser(
            AppSession.CurrentUser?.Id ?? 0).Count(n => !n.IsRead);

        // если НЕ преподаватель — стартовая страница "Отчётность"
        if (role != UserRole.Teacher)
            _activePage = "reports";
        else
            _activePage = "dashboard";

        foreach (var item in _items)
        {
            // убрать dashboard для админа и методиста
            if (item.Page == "dashboard" && role != UserRole.Teacher)
                continue;

            // проверка ролей
            if (item.Roles != null && !item.Roles.Contains(role))
                continue;

            string label = item.Page == "notifications" && unread > 0
                ? $"{item.Label}  ({unread})"
                : item.Label;

            var btn = CreateNavBtn(item.Page, item.Icon, label);
            pnlNavFlow.Controls.Add(btn);
            _navButtons.Add(btn);
        }

        SetActive(_activePage);

        // автоматически открываем первую нужную страницу
        NavClicked?.Invoke(this, _activePage);
    }

    private Button CreateNavBtn(string page, string icon, string label)
    {
        var btn = new Button
        {
            Tag       = page,
            Text      = $"  {icon}   {label}",
            Width     = 240,
            Height    = 48,
            BackColor = AppTheme.Primary,
            ForeColor = AppTheme.SidebarText,
            FlatStyle = FlatStyle.Flat,
            Font      = AppTheme.FontNav,
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor    = Cursors.Hand,
        };
        btn.FlatAppearance.BorderSize         = 0;
        btn.FlatAppearance.MouseOverBackColor = AppTheme.SidebarActive;
        btn.FlatAppearance.MouseDownBackColor = AppTheme.SidebarActive;
        btn.Click += (_, _) =>
        {
            _activePage = page;
            SetActive(page);
            NavClicked?.Invoke(this, page);
        };
        return btn;
    }

    // ── Public API ────────────────────────────────────────────────────────

    public void SetActive(string page)
    {
        _activePage = page;
        foreach (var btn in _navButtons)
        {
            bool active   = btn.Tag?.ToString() == page;
            btn.BackColor = active ? AppTheme.SidebarActive : AppTheme.Primary;
            btn.ForeColor = active ? Color.White            : AppTheme.SidebarText;
        }
    }

    public void Rebuild()
    {
        pnlUser.Controls.Clear();
        BuildUserCard();
        BuildNavItems();
    }

    // ── Events ────────────────────────────────────────────────────────────

    private void btnLogout_Click(object? sender, EventArgs e)
    {
        AppSession.CurrentUser = null;
        var lf = new Forms.LoginForm();
        lf.Show();
        FindForm()?.Close();
    }

    private void pnlUser_Paint(object? sender, PaintEventArgs e)
    {
        // nothing extra needed – background set in Designer
    }
}
