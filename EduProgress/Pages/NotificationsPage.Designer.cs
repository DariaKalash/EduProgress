namespace EduProgress.Pages;

partial class NotificationsPage
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        this.pnlOuter   = new FlowLayoutPanel();
        this.pnlHeader  = new Panel();
        this.pnlCatTabs = new FlowLayoutPanel();
        this.pnlList    = new FlowLayoutPanel();
        this.btnMarkAll = new Button();

        this.SuspendLayout();

        // ── pnlOuter ──────────────────────────────────────────────────────
        this.pnlOuter.FlowDirection = FlowDirection.TopDown;
        this.pnlOuter.WrapContents  = false;
        this.pnlOuter.AutoSize      = true;
        this.pnlOuter.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.pnlOuter.Padding       = new Padding(28, 24, 28, 40);
        this.pnlOuter.BackColor     = AppTheme.Background;

        // ── pnlHeader ─────────────────────────────────────────────────────
        this.pnlHeader.Height    = 44;
        this.pnlHeader.Width     = 800;
        this.pnlHeader.BackColor = Color.Transparent;
        this.pnlHeader.Margin    = new Padding(0, 0, 0, 16);

        // ── btnMarkAll ────────────────────────────────────────────────────
        this.btnMarkAll.Text      = "Прочитать все";
        this.btnMarkAll.Size      = new Size(150, 34);
        this.btnMarkAll.Location  = new Point(650, 5);
        this.btnMarkAll.BackColor = AppTheme.Background;
        this.btnMarkAll.ForeColor = AppTheme.Accent;
        this.btnMarkAll.FlatStyle = FlatStyle.Flat;
        this.btnMarkAll.Font      = AppTheme.FontSmall;
        this.btnMarkAll.Cursor    = Cursors.Hand;
        this.btnMarkAll.FlatAppearance.BorderColor = AppTheme.Accent;
        this.btnMarkAll.FlatAppearance.BorderSize  = 1;
        this.btnMarkAll.Click += new EventHandler(this.btnMarkAll_Click);
        this.pnlHeader.Controls.Add(this.btnMarkAll);

        // ── pnlCatTabs ────────────────────────────────────────────────────
        this.pnlCatTabs.FlowDirection = FlowDirection.LeftToRight;
        this.pnlCatTabs.AutoSize      = true;
        this.pnlCatTabs.BackColor     = AppTheme.Background;
        this.pnlCatTabs.Margin        = new Padding(0, 0, 0, 16);

        // ── pnlList ───────────────────────────────────────────────────────
        this.pnlList.FlowDirection = FlowDirection.TopDown;
        this.pnlList.WrapContents  = false;
        this.pnlList.AutoSize      = true;
        this.pnlList.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.pnlList.BackColor     = AppTheme.Background;

        // Assemble
        this.pnlOuter.Controls.Add(this.pnlHeader);
        this.pnlOuter.Controls.Add(this.pnlCatTabs);
        this.pnlOuter.Controls.Add(this.pnlList);
        this.Controls.Add(this.pnlOuter);

        this.Dock       = DockStyle.Fill;
        this.AutoScroll = true;
        this.BackColor  = AppTheme.Background;

        this.ResumeLayout(false);
    }

    private FlowLayoutPanel pnlOuter;
    private Panel           pnlHeader;
    private FlowLayoutPanel pnlCatTabs;
    private FlowLayoutPanel pnlList;
    private Button          btnMarkAll;
}
