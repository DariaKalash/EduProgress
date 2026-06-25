namespace EduProgress.Pages;

partial class CatalogPage
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        this.pnlOuter   = new FlowLayoutPanel();
        this.pnlToolbar = new Panel();
        this.txtSearch  = new TextBox();
        this.pnlFilters = new FlowLayoutPanel();
        this.gridPanel  = new FlowLayoutPanel();

        this.SuspendLayout();

        // ── pnlOuter ──────────────────────────────────────────────────────
        this.pnlOuter.FlowDirection = FlowDirection.TopDown;
        this.pnlOuter.WrapContents  = false;
        this.pnlOuter.AutoSize      = true;
        this.pnlOuter.Dock = DockStyle.Fill;
        this.pnlOuter.AutoScroll = true;

        this.pnlOuter.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.pnlOuter.Padding       = new Padding(28, 24, 28, 32);
        this.pnlOuter.BackColor     = AppTheme.Background;

        // ── pnlToolbar ────────────────────────────────────────────────────
        this.pnlToolbar.Height    = 48;
        this.pnlToolbar.Width     = 860;
        this.pnlToolbar.BackColor = Color.Transparent;
        this.pnlToolbar.Margin    = new Padding(0, 0, 0, 16);

        // ── txtSearch ─────────────────────────────────────────────────────
        this.txtSearch.Size        = new Size(450, 50);
        this.txtSearch.Location    = new Point(0, 6);
        this.txtSearch.Font        = AppTheme.FontInput;
        this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
        this.txtSearch.BackColor   = AppTheme.White;
        this.txtSearch.ForeColor   = AppTheme.TextMuted;
        this.txtSearch.Text        = "Поиск по названию…";
        this.txtSearch.Enter      += new EventHandler(this.txtSearch_Enter);
        this.txtSearch.Leave      += new EventHandler(this.txtSearch_Leave);
        this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
        this.pnlToolbar.Controls.Add(this.txtSearch);

        // ── pnlFilters ────────────────────────────────────────────────────
        this.pnlFilters.FlowDirection = FlowDirection.LeftToRight;
        this.pnlFilters.AutoSize      = true;
        this.pnlFilters.BackColor     = AppTheme.Background;
        this.pnlFilters.Margin        = new Padding(0, 0, 0, 16);

        // ── gridPanel ─────────────────────────────────────────────────────
        this.gridPanel.FlowDirection = FlowDirection.LeftToRight;
        this.gridPanel.WrapContents = true;
        this.gridPanel.AutoSize = true;
        this.gridPanel.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.gridPanel.BackColor     = AppTheme.Background;

        // Assemble
        this.pnlOuter.Controls.Add(this.pnlToolbar);
        this.pnlOuter.Controls.Add(this.pnlFilters);
        this.pnlOuter.Controls.Add(this.gridPanel);
        this.Controls.Add(this.pnlOuter);

        this.Dock       = DockStyle.Fill;
        this.AutoScroll = true;
        this.BackColor  = AppTheme.Background;

        this.ResumeLayout(false);
    }

    private FlowLayoutPanel pnlOuter;
    private Panel           pnlToolbar;
    private TextBox         txtSearch;
    private FlowLayoutPanel pnlFilters;
    private FlowLayoutPanel gridPanel;
}
