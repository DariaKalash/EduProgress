namespace EduProgress.Pages;

partial class CoursePage
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        this.pnlBanner  = new Panel();
        this.split      = new SplitContainer();
        this.pnlTreeHdr = new Panel();
        this.treeScroll = new Panel();
        this.treeFlow   = new FlowLayoutPanel();
        this.pnlDetail  = new Panel();

        this.SuspendLayout();

        // ── pnlBanner ─────────────────────────────────────────────────────
        this.pnlBanner.Dock      = DockStyle.Top;
        this.pnlBanner.Height    = 140;
        this.pnlBanner.BackColor = AppTheme.Primary;
        this.pnlBanner.Paint    += new PaintEventHandler(this.pnlBanner_Paint);

        // ── split ─────────────────────────────────────────────────────────
        this.split.Dock          = DockStyle.Fill;
        this.split.SplitterWidth = 1;
        this.split.Panel1MinSize = 20;
        //this.split.Panel2MinSize = 400;
        this.split.Panel2MinSize = 110;
        this.split.BackColor     = AppTheme.Border;
        this.split.Panel1.BackColor = AppTheme.White;
        this.split.Panel2.BackColor = AppTheme.Background;
        //this.split.SplitterDistance = 260;

        // ── pnlTreeHdr ────────────────────────────────────────────────────
        this.pnlTreeHdr.Height    = 48;
        this.pnlTreeHdr.Dock      = DockStyle.Top;
        this.pnlTreeHdr.BackColor = AppTheme.White;
        this.pnlTreeHdr.Paint    += new PaintEventHandler(this.pnlTreeHdr_Paint);

        // ── treeScroll ────────────────────────────────────────────────────
        this.treeScroll.Dock       = DockStyle.Fill;
        this.treeScroll.AutoScroll = true;
        this.treeScroll.BackColor  = AppTheme.White;

        // ── treeFlow ──────────────────────────────────────────────────────
        this.treeFlow.FlowDirection = FlowDirection.TopDown;
        this.treeFlow.WrapContents  = false;
        this.treeFlow.AutoSize      = true;
        this.treeFlow.AutoSizeMode  = AutoSizeMode.GrowAndShrink;
        this.treeFlow.BackColor     = AppTheme.White;

        // ── pnlDetail ─────────────────────────────────────────────────────
        this.pnlDetail.Dock       = DockStyle.Fill;
        this.pnlDetail.AutoScroll = true;
        this.pnlDetail.BackColor  = AppTheme.Background;

        // Assemble
        this.treeScroll.Controls.Add(this.treeFlow);
        this.split.Panel1.Controls.Add(this.treeScroll);
        this.split.Panel1.Controls.Add(this.pnlTreeHdr);
        this.split.Panel2.Controls.Add(this.pnlDetail);

        this.Controls.Add(this.split);
        this.Controls.Add(this.pnlBanner);

        this.Dock      = DockStyle.Fill;
        this.BackColor = AppTheme.Background;

        this.ResumeLayout(false);
    }

    private Panel           pnlBanner;
    private SplitContainer  split;
    private Panel           pnlTreeHdr;
    private Panel           treeScroll;
    private FlowLayoutPanel treeFlow;
    private Panel           pnlDetail;
}
