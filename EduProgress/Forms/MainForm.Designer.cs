using EduProgress.Controls;

namespace EduProgress.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.sidebar     = new SidebarControl();
        this.pnlRight    = new Panel();
        this.header      = new HeaderControl();
        this.pnlContent  = new Panel();

        this.SuspendLayout();

        // ── sidebar ───────────────────────────────────────────────────────
        this.sidebar.NavClicked += new EventHandler<string>(this.sidebar_NavClicked);

        // ── pnlRight ──────────────────────────────────────────────────────
        this.pnlRight.Dock      = DockStyle.Fill;
        this.pnlRight.BackColor = AppTheme.Background;

        // ── header ────────────────────────────────────────────────────────
        // (Dock=Top set in HeaderControl itself)

        // ── pnlContent ────────────────────────────────────────────────────
        this.pnlContent.Dock       = DockStyle.Fill;
        this.pnlContent.BackColor  = AppTheme.Background;
        this.pnlContent.AutoScroll = true;

        this.pnlRight.Controls.Add(this.pnlContent);
        this.pnlRight.Controls.Add(this.header);

        this.Controls.Add(this.pnlRight);
        this.Controls.Add(this.sidebar);

        // ── Form ──────────────────────────────────────────────────────────
        this.Text            = "EduProgress";
        this.Size            = new Size(1280, 800);
        this.MinimumSize     = new Size(1100, 700);
        this.StartPosition   = FormStartPosition.CenterScreen;
        this.BackColor       = AppTheme.Background;
        this.Font            = AppTheme.FontBody;
        this.FormBorderStyle = FormBorderStyle.Sizable;

        this.ResumeLayout(false);
    }

    // ── Field declarations ─────────────────────────────────────────────────
    private SidebarControl sidebar;
    private Panel          pnlRight;
    private HeaderControl  header;
    private Panel          pnlContent;
}
