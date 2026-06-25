namespace EduProgress.Controls;

public partial class HeaderControl : UserControl
{
    public HeaderControl()
    {
        InitializeComponent();
        RefreshBadge();
        lblDate.Text = DateTime.Now.ToString("dd MMMM yyyy",
            new System.Globalization.CultureInfo("ru-RU"));
        lblTitle.Top = (64 - lblTitle.PreferredHeight) / 2;
    }

    // ── Public API ────────────────────────────────────────────────────────

    public void SetTitle(string title)
    {
        lblTitle.Text = title;
        lblTitle.Top  = (64 - lblTitle.PreferredHeight) / 2;
    }

    public void RefreshBadge()
    {
        int unread = AppSession.CurrentUser == null ? 0
            : AppSession.Db.GetNotificationsForUser(AppSession.CurrentUser.Id)
                         .Count(n => !n.IsRead);
        lblBadge.Text    = unread.ToString();
        lblBadge.Visible = unread > 0;
    }

    // ── Events ────────────────────────────────────────────────────────────

    private void btnBell_Click(object? sender, EventArgs e)
        => AppSession.NavigateTo?.Invoke("notifications", null);

    private void HeaderControl_SizeChanged(object? sender, EventArgs e)
        => PositionRight();

    private void HeaderControl_HandleCreated(object? sender, EventArgs e)
        => PositionRight();

    private void HeaderControl_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(AppTheme.Border, 1);
        e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
    }

    private void PositionRight()
    {
        btnBell.Location  = new Point(Width - 56, (64 - btnBell.Height) / 2);
        lblBadge.Location = new Point(btnBell.Right - 14, btnBell.Top + 2);
        lblDate.Location  = new Point(Width - 56 - lblDate.PreferredWidth - 16,
                                      (64 - lblDate.PreferredHeight) / 2);
    }
}
