namespace EduProgress.Pages;

partial class FaqPage
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        this.pnlOuter   = new FlowLayoutPanel();
        this.pnlCatTabs = new FlowLayoutPanel();
        this.pnlItems   = new FlowLayoutPanel();

        this.SuspendLayout();

        this.pnlOuter.FlowDirection = FlowDirection.TopDown;
        this.pnlOuter.WrapContents  = false;
        this.pnlOuter.AutoSize      = true;
        this.pnlOuter.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.pnlOuter.Padding       = new Padding(28, 24, 28, 40);
        this.pnlOuter.BackColor     = AppTheme.Background;

        this.pnlCatTabs.FlowDirection = FlowDirection.LeftToRight;
        this.pnlCatTabs.AutoSize      = true;
        this.pnlCatTabs.BackColor     = AppTheme.Background;
        this.pnlCatTabs.Margin        = new Padding(0, 0, 0, 16);

        this.pnlItems.FlowDirection = FlowDirection.TopDown;
        this.pnlItems.WrapContents  = false;
        this.pnlItems.AutoSize      = true;
        this.pnlItems.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.pnlItems.BackColor     = AppTheme.Background;

        this.pnlOuter.Controls.Add(this.pnlCatTabs);
        this.pnlOuter.Controls.Add(this.pnlItems);
        this.Controls.Add(this.pnlOuter);

        this.Dock       = DockStyle.Fill;
        this.AutoScroll = true;
        this.BackColor  = AppTheme.Background;

        this.ResumeLayout(false);
    }

    private FlowLayoutPanel pnlOuter;
    private FlowLayoutPanel pnlCatTabs;
    private FlowLayoutPanel pnlItems;
}
