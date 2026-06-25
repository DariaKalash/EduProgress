namespace EduProgress.Pages;

partial class ProgressPage
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        this.pnlOuter = new FlowLayoutPanel();
        this.SuspendLayout();

        this.pnlOuter.FlowDirection = FlowDirection.TopDown;
        this.pnlOuter.WrapContents  = false;
        this.pnlOuter.AutoSize      = true;
        this.pnlOuter.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.pnlOuter.Padding       = new Padding(28, 24, 28, 40);
        this.pnlOuter.BackColor     = AppTheme.Background;

        this.Controls.Add(this.pnlOuter);
        this.Dock       = DockStyle.Fill;
        this.AutoScroll = true;
        this.BackColor  = AppTheme.Background;

        this.ResumeLayout(false);
    }

    private FlowLayoutPanel pnlOuter;
}
