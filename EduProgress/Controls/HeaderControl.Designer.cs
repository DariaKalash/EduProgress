namespace EduProgress.Controls;

partial class HeaderControl
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.lblTitle    = new Label();
        this.btnBell     = new Button();
        this.lblBadge    = new Label();
        this.lblDate     = new Label();

        this.SuspendLayout();

        // ── lblTitle ──────────────────────────────────────────────────────
        this.lblTitle.Text      = "Главная";
        this.lblTitle.Font      = AppTheme.FontH1;
        this.lblTitle.ForeColor = AppTheme.TextPrimary;
        this.lblTitle.AutoSize  = true;
        this.lblTitle.Location  = new Point(24, 0);

        // ── btnBell ───────────────────────────────────────────────────────
        this.btnBell.Text      = "◎";
        this.btnBell.Font      = AppTheme.F(16f);
        this.btnBell.FlatStyle = FlatStyle.Flat;
        this.btnBell.BackColor = AppTheme.White;
        this.btnBell.ForeColor = AppTheme.TextPrimary;
        this.btnBell.Size      = new Size(42, 42);
        this.btnBell.Cursor    = Cursors.Hand;
        this.btnBell.Anchor    = AnchorStyles.Top | AnchorStyles.Right;
        this.btnBell.FlatAppearance.BorderSize = 0;
        this.btnBell.Click += new EventHandler(this.btnBell_Click);

        // ── lblBadge ──────────────────────────────────────────────────────
        this.lblBadge.Font      = AppTheme.FontSmallBold;
        this.lblBadge.ForeColor = Color.White;
        this.lblBadge.BackColor = AppTheme.Danger;
        this.lblBadge.TextAlign = ContentAlignment.MiddleCenter;
        this.lblBadge.Size      = new Size(18, 18);
        this.lblBadge.Anchor    = AnchorStyles.Top | AnchorStyles.Right;

        // ── lblDate ───────────────────────────────────────────────────────
        this.lblDate.Font      = AppTheme.FontSmall;
        this.lblDate.ForeColor = AppTheme.TextMuted;
        this.lblDate.AutoSize  = true;
        this.lblDate.Anchor    = AnchorStyles.Top | AnchorStyles.Right;

        // ── UserControl ───────────────────────────────────────────────────
        this.Controls.AddRange(new Control[] { this.lblTitle, this.btnBell, this.lblBadge, this.lblDate });

        this.Height    = 64;
        this.Dock      = DockStyle.Top;
        this.BackColor = AppTheme.White;

        this.SizeChanged   += new EventHandler(this.HeaderControl_SizeChanged);
        this.HandleCreated += new EventHandler(this.HeaderControl_HandleCreated);
        this.Paint         += new PaintEventHandler(this.HeaderControl_Paint);

        this.ResumeLayout(false);
        this.PerformLayout();
    }

    // ── Field declarations ─────────────────────────────────────────────────
    private Label  lblTitle;
    private Button btnBell;
    private Label  lblBadge;
    private Label  lblDate;
}
