using EduProgress.Models;

namespace EduProgress.Pages;

public partial class LessonPage : BasePage
{
    private readonly Lesson _lesson;

    public LessonPage(Lesson lesson)
    {
        _lesson = lesson;
        InitializeComponent();
        BuildContent();
    }

    private void BuildContent()
    {
        innerFlow.Controls.Add(BuildHeaderCard());
        innerFlow.Controls.Add(BuildContentCard());

        if (_lesson.Resources.Count > 0)
            innerFlow.Controls.Add(BuildResourcesCard());

        if (!string.IsNullOrEmpty(_lesson.ZoomLink))
            innerFlow.Controls.Add(BuildZoomCard());

        innerFlow.Controls.Add(BuildNavButtons());
        innerFlow.Width = pnlScroll.Width;
    }

    // ── Header card ───────────────────────────────────────────────────────

    private Panel BuildHeaderCard()
    {
        var card = MakeCard(800, 90, 24);
        card.Margin = new Padding(0, 0, 0, 16);

        var badge = _lesson.Status switch
        {
            LessonStatus.Completed  => MakeBadge("Завершён",   AppTheme.Success),
            LessonStatus.InProgress => MakeBadge("В процессе", AppTheme.Accent),
            _                       => MakeBadge("Не начат",   AppTheme.TextMuted),
        };
        badge.Location = new Point(24, 16);

        card.Controls.Add(badge);
        card.Controls.Add(new Label
        {
            Text = _lesson.Title, Font = AppTheme.FontH1, ForeColor = AppTheme.TextPrimary,
            AutoSize = false, Width = 600, Height = 34, Location = new Point(24, 44),
        });
        card.Controls.Add(new Label
        {
            Text = $"◷  {_lesson.DurationLabel}", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted,
            AutoSize = true, Location = new Point(700, 60),
        });
        return card;
    }

    // ── Content card ──────────────────────────────────────────────────────

    private Panel BuildContentCard()
    {
        string text = string.IsNullOrWhiteSpace(_lesson.Content)
            ? "Материалы к этому уроку ещё не добавлены."
            : _lesson.Content;

        var textLbl = new Label
        {
            Text = text, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary,
            AutoSize = false, Width = 752, Location = new Point(24, 56),
        };
        using var g = CreateGraphics();
        textLbl.Height = (int)g.MeasureString(text, AppTheme.FontBody, 752).Height + 8;

        var card = MakeCard(800, textLbl.Height + 80, 24);
        card.Margin = new Padding(0, 0, 0, 16);
        card.Controls.Add(new Label { Text = "Материалы урока", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(24, 16) });
        card.Controls.Add(new Panel { Height = 1, Width = 752, Location = new Point(24, 44), BackColor = AppTheme.Border });
        card.Controls.Add(textLbl);
        return card;
    }

    // ── Resources card ────────────────────────────────────────────────────

    private Panel BuildResourcesCard()
    {
        var card = MakeCard(800, 60 + _lesson.Resources.Count * 48, 24);
        card.Margin = new Padding(0, 0, 0, 16);
        card.Controls.Add(new Label { Text = "Прикреплённые материалы", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(24, 16) });

        int ry = 48;
        foreach (var res in _lesson.Resources)
        {
            var row = new Panel { Width = 752, Height = 42, BackColor = AppTheme.Background, Location = new Point(24, ry) };
            Color ic = res.Type == "pdf" ? AppTheme.Danger : res.Type == "video" ? AppTheme.Purple : AppTheme.Accent;
            var badge = MakeBadge(res.Type.ToUpper(), ic);
            badge.Location = new Point(0, 10);
            row.Controls.Add(badge);
            row.Controls.Add(new Label { Text = res.Name, Font = AppTheme.FontBody, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(60, 12) });
            var openBtn = MakeSecBtn("Открыть", 80, 28);
            openBtn.Location = new Point(680, 7);
            openBtn.Font     = AppTheme.FontSmall;
            row.Controls.Add(openBtn);
            card.Controls.Add(row);
            ry += 48;
        }
        return card;
    }

    // ── Zoom card ─────────────────────────────────────────────────────────

    private Panel BuildZoomCard()
    {
        var card = MakeCard(800, 80, 24);
        card.Margin = new Padding(0, 0, 0, 16);
        card.Controls.Add(new Label { Text = "Онлайн-занятие (Zoom)", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(24, 14) });
        var zBtn = MakePrimBtn("Подключиться к Zoom", 200, 36);
        zBtn.BackColor = Color.FromArgb(45, 140, 255);
        zBtn.Location  = new Point(24, 40);
        zBtn.Click    += (_, _) => MessageBox.Show($"Ссылка:\n{_lesson.ZoomLink}", "Zoom Meeting", MessageBoxButtons.OK, MessageBoxIcon.Information);
        card.Controls.Add(zBtn);
        card.Controls.Add(new Label { Text = _lesson.ZoomLink ?? "", Font = AppTheme.FontSmall, ForeColor = AppTheme.Accent, AutoSize = true, Location = new Point(234, 50) });
        return card;
    }

    // ── Navigation ────────────────────────────────────────────────────────

    private Panel BuildNavButtons()
    {
        var nav = new Panel { Width = 800, Height = 60, BackColor = Color.Transparent, Margin = new Padding(0, 8, 0, 0) };

        var prevBtn = MakeSecBtn("← Предыдущий", 150, 38);
        prevBtn.Location = new Point(0, 11);

        var nextBtn = MakePrimBtn("Следующий →", 150, 38);
        nextBtn.Location = new Point(650, 11);

        var doneBtn = new Button
        {
            Text = "✓  Завершить урок", Location = new Point(224, 11), Size = new Size(200, 38),
            BackColor = AppTheme.Success, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
            Font = AppTheme.FontBodyBold, Cursor = Cursors.Hand,
        };
        doneBtn.FlatAppearance.BorderSize = 0;
        doneBtn.Click += btnDone_Click;

        nav.Controls.AddRange(new Control[] { prevBtn, doneBtn, nextBtn });
        return nav;
    }

    // ── Events ────────────────────────────────────────────────────────────

    private void btnDone_Click(object? sender, EventArgs e)
    {
        AppSession.Db.CompleteLesson(AppSession.CurrentUser!.Id, _lesson.Id);
        _lesson.Status = LessonStatus.Completed;
        MessageBox.Show("Урок успешно завершён!", "EduProgress", MessageBoxButtons.OK, MessageBoxIcon.Information);
        AppSession.NavigateTo?.Invoke("catalog", null);
    }

    private void lnkBack_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        => AppSession.NavigateTo?.Invoke("catalog", null);

    private void pnlBreadcrumb_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(AppTheme.Border, 1);
        e.Graphics.DrawLine(pen, 0, pnlBreadcrumb.Height - 1, pnlBreadcrumb.Width, pnlBreadcrumb.Height - 1);
    }

    private void pnlScroll_SizeChanged(object? sender, EventArgs e)
        => innerFlow.Width = pnlScroll.Width;
}
