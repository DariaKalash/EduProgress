using EduProgress.Models;

namespace EduProgress.Pages;

public partial class NotificationsPage : BasePage
{
    private string _catFilter = "all";
    private List<Notification> _allNotifs = new();
    private readonly List<Button> _catBtns = new();

    private readonly (string Key, string Label, Color Color)[] _cats =
    {
        ("all",      "Все",          AppTheme.Primary),
        ("course",   "Курсы",        AppTheme.CatCourse),
        ("reminder", "Напоминания",  AppTheme.CatReminder),
        ("system",   "Системные",    AppTheme.CatSystem),
    };

    public NotificationsPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => pnlOuter.Width = Width;
        AddTitle();
        BuildCatTabs();
        LoadData();
    }

    private void AddTitle()
    {
        var lbl = new Label
        {
            Text = "Список уведомлений системы",
            Font = AppTheme.FontH1,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 4),
        };
        pnlHeader.Controls.Add(lbl);
    }

    private void BuildCatTabs()
    {
        for (int i = 0; i < _cats.Length; i++)
        {
            int idx = i;
            var (key, label, color) = _cats[i];
            var btn = new Button
            {
                Text = label, Tag = key,
                Size = new Size(130, 34),
                FlatStyle = FlatStyle.Flat, Font = AppTheme.FontSmall, Cursor = Cursors.Hand,
                BackColor = i == 0 ? AppTheme.Primary : AppTheme.White,
                ForeColor = i == 0 ? Color.White       : AppTheme.TextPrimary,
            };
            btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.BorderColor = i == 0 ? AppTheme.Primary : AppTheme.Border;
            btn.Click += (_, _) =>
            {
                _catFilter = _cats[idx].Key;
                for (int j = 0; j < _catBtns.Count; j++)
                {
                    bool sel = _catBtns[j].Tag?.ToString() == _catFilter;
                    _catBtns[j].BackColor = sel ? _cats[j].Color : AppTheme.White;
                    _catBtns[j].ForeColor = sel ? Color.White     : AppTheme.TextPrimary;
                    _catBtns[j].FlatAppearance.BorderColor = sel ? _cats[j].Color : AppTheme.Border;
                }
                RefreshList();
            };
            _catBtns.Add(btn);
            pnlCatTabs.Controls.Add(btn);
        }
    }

    private void LoadData()
    {
        _allNotifs = AppSession.Db.GetNotificationsForUser(AppSession.CurrentUser!.Id);
        RefreshList();
        pnlOuter.Width = Width;
    }

    private void RefreshList()
    {
        pnlList.Controls.Clear();
        var filtered = _catFilter == "all" ? _allNotifs : _allNotifs.Where(n =>
            n.Category == NotificationCategory.Course   && _catFilter == "course"   ||
            n.Category == NotificationCategory.Reminder && _catFilter == "reminder" ||
            n.Category == NotificationCategory.System   && _catFilter == "system");

        foreach (var n in filtered) pnlList.Controls.Add(NotifRow(n));
    }

    private static Panel NotifRow(Notification n)
    {
        var row = new Panel
        {
            Width = 800, Height = 88,
            BackColor = n.IsRead ? AppTheme.White : Color.FromArgb(232, 244, 255),
            Margin = new Padding(0, 0, 0, 8),
        };
        row.Paint += (_, e) => { using var pen = new Pen(AppTheme.Border, 1); e.Graphics.DrawRectangle(pen, 0, 0, row.Width - 1, row.Height - 1); };

        Color dc = n.Category == NotificationCategory.Reminder ? AppTheme.CatReminder
                 : n.Category == NotificationCategory.System   ? AppTheme.CatSystem : AppTheme.CatCourse;

        row.Controls.Add(new Panel { Size = new Size(4, 88), Location = new Point(0, 0), BackColor = dc });
        var badge = MakeBadge(n.CategoryLabel, dc);
        badge.Location = new Point(20, 14);
        row.Controls.Add(badge);

        var unread = new Panel { Size = new Size(10, 10), Location = new Point(770, 14), BackColor = Color.Transparent, Visible = !n.IsRead };
        unread.Paint += (_, e) => { e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; using var b = new SolidBrush(AppTheme.Accent); e.Graphics.FillEllipse(b, 0, 0, 9, 9); };
        row.Controls.Add(unread);

        row.Controls.Add(new Label { Text = n.Title, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextPrimary, AutoSize = false, Width = 600, Height = 22, Location = new Point(20, 36) });
        row.Controls.Add(new Label { Text = n.Message, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = false, Width = 600, Height = 20, Location = new Point(20, 58) });
        row.Controls.Add(new Label { Text = n.TimeAgo, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextLight, AutoSize = true, Location = new Point(650, 58) });

        var readBtn = new Button { Text = n.IsRead ? "Прочитано" : "Отметить", Size = new Size(100, 26), Location = new Point(680, 14), FlatStyle = FlatStyle.Flat, Font = AppTheme.FontSmall, BackColor = n.IsRead ? AppTheme.Background : AppTheme.Accent, ForeColor = n.IsRead ? AppTheme.TextMuted : Color.White, Cursor = Cursors.Hand };
        readBtn.FlatAppearance.BorderSize = 0;
        readBtn.Click += (_, _) =>
        {
            if (!n.IsRead)
            {
                AppSession.Db.MarkNotificationRead(n.Id);
                n.IsRead = true;
                row.BackColor = AppTheme.White;
                readBtn.Text = "Прочитано"; readBtn.BackColor = AppTheme.Background; readBtn.ForeColor = AppTheme.TextMuted;
                unread.Visible = false;
            }
        };
        row.Controls.Add(readBtn);
        return row;
    }

    private void btnMarkAll_Click(object? sender, EventArgs e)
    {
        AppSession.Db.MarkAllNotificationsRead(AppSession.CurrentUser!.Id);
        foreach (var n in _allNotifs) n.IsRead = true;
        RefreshList();
    }
}
