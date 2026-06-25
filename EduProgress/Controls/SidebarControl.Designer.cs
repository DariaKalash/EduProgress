namespace EduProgress.Controls;

partial class SidebarControl
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.pnlLogo    = new Panel();
        this.lblLogo    = new Label();
        this.pnlUser    = new Panel();
        this.pnlNavFlow = new FlowLayoutPanel();
        this.btnLogout  = new Button();

        this.SuspendLayout();

        // ── pnlLogo ───────────────────────────────────────────────────────
        this.pnlLogo.Height    = 64;
        this.pnlLogo.Dock      = DockStyle.Top;
        this.pnlLogo.BackColor = AppTheme.SidebarDark;

        this.lblLogo.Text      = "EduProgress";
        this.lblLogo.ForeColor = Color.White;
        this.lblLogo.Font      = AppTheme.FontLogo;
        this.lblLogo.Dock      = DockStyle.Fill;
        this.lblLogo.TextAlign = ContentAlignment.MiddleCenter;
        this.pnlLogo.Controls.Add(this.lblLogo);

        // ── pnlUser ───────────────────────────────────────────────────────
        this.pnlUser.Height    = 72;
        this.pnlUser.Dock      = DockStyle.Top;
        this.pnlUser.BackColor = AppTheme.SidebarDark;
        this.pnlUser.Padding   = new Padding(12, 10, 12, 10);
        this.pnlUser.Paint    += new PaintEventHandler(this.pnlUser_Paint);

        // ── pnlNavFlow ────────────────────────────────────────────────────
        this.pnlNavFlow.FlowDirection = FlowDirection.TopDown;
        this.pnlNavFlow.WrapContents  = false;
        this.pnlNavFlow.AutoSize      = false;
        this.pnlNavFlow.Dock          = DockStyle.Fill;
        this.pnlNavFlow.BackColor     = AppTheme.Primary;
        this.pnlNavFlow.Padding       = new Padding(0, 8, 0, 0);

        // ── btnLogout ─────────────────────────────────────────────────────
        this.btnLogout.Text      = "  ✕  Выйти из системы";
        this.btnLogout.Dock      = DockStyle.Bottom;
        this.btnLogout.Height    = 48;
        this.btnLogout.BackColor = Color.FromArgb(192, 57, 43);
        this.btnLogout.ForeColor = Color.White;
        this.btnLogout.FlatStyle = FlatStyle.Flat;
        this.btnLogout.Font      = AppTheme.FontNav;
        this.btnLogout.TextAlign = ContentAlignment.MiddleLeft;
        this.btnLogout.Cursor    = Cursors.Hand;
        this.btnLogout.FlatAppearance.BorderSize = 0;
        this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

        // ── UserControl ───────────────────────────────────────────────────
        this.Controls.Add(this.pnlNavFlow);
        this.Controls.Add(this.pnlUser);
        this.Controls.Add(this.pnlLogo);
        this.Controls.Add(this.btnLogout);

        this.Width     = 240;
        this.Dock      = DockStyle.Left;
        this.BackColor = AppTheme.Primary;

        this.ResumeLayout(false);
    }

    // ── Field declarations ─────────────────────────────────────────────────
    private Panel           pnlLogo;
    private Label           lblLogo;
    private Panel           pnlUser;
    private FlowLayoutPanel pnlNavFlow;
    private Button          btnLogout;
}
