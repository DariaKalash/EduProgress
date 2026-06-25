using EduProgress.Models;

namespace EduProgress.Pages;

public partial class FaqPage : BasePage
{
    private string _catFilter = "all";
    private List<FaqItem> _allFaq = new();
    private readonly List<Button> _catBtns = new();

    private readonly (string Key, string Label)[] _cats =
    {
        ("all",       "Все вопросы"),
        ("course",    "По курсам"),
        ("system",    "По системе"),
        ("technical", "Технические"),
    };

    public FaqPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => pnlOuter.Width = Width;
        AddTitles();
        BuildCatTabs();
        LoadData();
    }

    private void AddTitles()
    {
        //var h1 = new Label { Text = "Часто задаваемые вопросы", Font = AppTheme.FontH1, ForeColor = AppTheme.TextPrimary, AutoSize = true, Margin = new Padding(0, 0, 0, 6) };
        var h2 = new Label { Text = "Ответы на наиболее распространённые вопросы о работе системы", Font = AppTheme.FontBody, ForeColor = AppTheme.TextMuted, AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
        //pnlOuter.Controls.Add(h1);
        //pnlOuter.Controls.SetChildIndex(h1, 0);
        pnlOuter.Controls.Add(h2);
        pnlOuter.Controls.SetChildIndex(h2, 1);
    }

    private void BuildCatTabs()
    {
        for (int i = 0; i < _cats.Length; i++)
        {
            int idx = i; var (key, label) = _cats[i];
            var btn = new Button { Text = label, Tag = key, Size = new Size(150, 34), FlatStyle = FlatStyle.Flat, Font = AppTheme.FontSmall, Cursor = Cursors.Hand, BackColor = i == 0 ? AppTheme.Accent : AppTheme.White, ForeColor = i == 0 ? Color.White : AppTheme.TextPrimary };
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = i == 0 ? AppTheme.Accent : AppTheme.Border;
            btn.Click += (_, _) =>
            {
                _catFilter = _cats[idx].Key;
                for (int j = 0; j < _catBtns.Count; j++)
                {
                    bool sel = _catBtns[j].Tag?.ToString() == _catFilter;
                    _catBtns[j].BackColor = sel ? AppTheme.Accent : AppTheme.White;
                    _catBtns[j].ForeColor = sel ? Color.White      : AppTheme.TextPrimary;
                    _catBtns[j].FlatAppearance.BorderColor = sel ? AppTheme.Accent : AppTheme.Border;
                }
                RefreshItems();
            };
            _catBtns.Add(btn);
            pnlCatTabs.Controls.Add(btn);
        }
    }

    private void LoadData()
    {
        _allFaq = AppSession.Db.GetFaq();
        if (_allFaq.Count == 0)
            _allFaq = FallbackFaq();
        RefreshItems();
        pnlOuter.Width = Width;
    }

    private void RefreshItems()
    {
        pnlItems.Controls.Clear();
        var items = _catFilter == "all" ? _allFaq : _allFaq.Where(f => f.Category == _catFilter).ToList();
        foreach (var item in items) pnlItems.Controls.Add(AccordionItem(item.Question, item.Answer));
    }

    private static Panel AccordionItem(string question, string answer)
    {
        bool expanded = false;

        var container = new Panel
        {
            Width = 820,
            BackColor = AppTheme.White,
            Margin = new Padding(0, 0, 0, 8),
            AutoSize = false
        };

        var header = new Panel
        {
            Height = 52,
            Dock = DockStyle.Top,
            BackColor = AppTheme.White,
            Cursor = Cursors.Hand
        };

        var qLbl = new Label
        {
            Text = question,
            Font = AppTheme.FontBodyBold,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = false,
            Width = 740,
            Height = 52,
            Location = new Point(20, 0),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var arrow = new Label
        {
            Text = "▼",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextMuted,
            AutoSize = true,
            Location = new Point(780, 18)
        };

        header.Controls.AddRange(new Control[] { qLbl, arrow });

        // === ПАНЕЛЬ С ОТВЕТОМ ===
        var ansPanel = new Panel
        {
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(20, 10, 20, 10),
            Visible = false,
            AutoSize = true
        };

        var aLbl = new Label
        {
            Text = answer,
            Font = AppTheme.FontBody,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            MaximumSize = new Size(760, 0) // 🔥 ключевая строка
        };

        ansPanel.Controls.Add(aLbl);

        container.Controls.Add(ansPanel);
        container.Controls.Add(header);

        container.Height = header.Height;

        void Toggle()
        {
            expanded = !expanded;

            ansPanel.Visible = expanded;
            arrow.Text = expanded ? "▲" : "▼";

            header.BackColor = expanded
                ? Color.FromArgb(240, 248, 255)
                : AppTheme.White;

            container.Height = expanded
                ? header.Height + ansPanel.PreferredSize.Height
                : header.Height;
        }

        header.Click += (_, _) => Toggle();
        qLbl.Click += (_, _) => Toggle();
        arrow.Click += (_, _) => Toggle();

        return container;
    }

    // Fallback if FAQ table is empty
    private static List<FaqItem> FallbackFaq() => new()
    {
        new FaqItem { Category = "course",    Question = "Как записаться на курс?", Answer = "Перейдите в раздел «Каталог курсов» и нажмите «Записаться».", OrderNumber = 1 },
        new FaqItem { Category = "system",    Question = "Как изменить данные профиля?", Answer = "Обратитесь к администратору системы.", OrderNumber = 1 },
        new FaqItem { Category = "technical", Question = "Что делать при ошибке входа?", Answer = "Проверьте правильность логина и пароля. При повторных проблемах обратитесь к администратору.", OrderNumber = 1 },
    };
}
