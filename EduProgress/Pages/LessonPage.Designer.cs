namespace EduProgress.Pages;

partial class LessonPage
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        this.pnlBreadcrumb = new Panel();
        this.lnkBack       = new LinkLabel();
        this.pnlScroll     = new Panel();
        this.innerFlow     = new FlowLayoutPanel();

        this.SuspendLayout();

        // ── pnlBreadcrumb ─────────────────────────────────────────────────
        this.pnlBreadcrumb.Height    = 40;
        this.pnlBreadcrumb.Dock      = DockStyle.Top;
        this.pnlBreadcrumb.BackColor = AppTheme.White;
        this.pnlBreadcrumb.Padding   = new Padding(28, 0, 0, 0);
        this.pnlBreadcrumb.Paint    += new PaintEventHandler(this.pnlBreadcrumb_Paint);

        this.lnkBack.Text           = "← Назад к каталогу";
        this.lnkBack.Font           = AppTheme.FontSmall;
        this.lnkBack.LinkColor      = AppTheme.Accent;
        this.lnkBack.ActiveLinkColor= AppTheme.Primary;
        this.lnkBack.AutoSize       = true;
        this.lnkBack.Location       = new Point(28, 10);
        this.lnkBack.LinkClicked   += new LinkLabelLinkClickedEventHandler(this.lnkBack_LinkClicked);
        this.pnlBreadcrumb.Controls.Add(this.lnkBack);

        // ── pnlScroll ─────────────────────────────────────────────────────
        this.pnlScroll.Dock       = DockStyle.Fill;
        this.pnlScroll.AutoScroll = true;
        this.pnlScroll.BackColor  = AppTheme.Background;

        // ── innerFlow ─────────────────────────────────────────────────────
        this.innerFlow.FlowDirection = FlowDirection.TopDown;
        this.innerFlow.WrapContents  = false;
        this.innerFlow.AutoSize      = true;
        this.innerFlow.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.innerFlow.Padding       = new Padding(28, 20, 28, 40);
        this.innerFlow.BackColor     = AppTheme.Background;

        this.pnlScroll.Controls.Add(this.innerFlow);
        this.pnlScroll.SizeChanged += new EventHandler(this.pnlScroll_SizeChanged);

        this.Controls.Add(this.pnlScroll);
        this.Controls.Add(this.pnlBreadcrumb);

        this.Dock      = DockStyle.Fill;
        this.BackColor = AppTheme.Background;

        this.ResumeLayout(false);
    }

    private Panel     pnlBreadcrumb;
    private LinkLabel lnkBack;
    private Panel     pnlScroll;
    private FlowLayoutPanel innerFlow;
}
