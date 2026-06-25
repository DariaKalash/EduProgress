namespace EduProgress.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.pnlLeft         = new Panel();
        this.pnlRight        = new Panel();
        this.pnlCard         = new Panel();
        this.lblTitle        = new Label();
        this.lblSub          = new Label();
        this.pnlRoles        = new FlowLayoutPanel();
        this.lblEmailCaption = new Label();
        this.txtEmail        = new TextBox();
        this.lblPassCaption  = new Label();
        this.txtPassword     = new TextBox();
        this.lblError        = new Label();
        this.btnLogin        = new Button();
        this.lblHint         = new Label();

        this.SuspendLayout();

        // ── pnlLeft ───────────────────────────────────────────────────────
        this.pnlLeft.Dock      = DockStyle.Left;
        this.pnlLeft.Width     = 480;
        this.pnlLeft.BackColor = AppTheme.Primary;
        this.pnlLeft.Paint    += new PaintEventHandler(this.pnlLeft_Paint);

        // ── pnlRight ──────────────────────────────────────────────────────
        this.pnlRight.Dock      = DockStyle.Fill;
        this.pnlRight.BackColor = AppTheme.Background;
        this.pnlRight.Resize   += new EventHandler(this.pnlRight_Resize);
        this.pnlRight.Controls.Add(this.pnlCard);

        // ── pnlCard ───────────────────────────────────────────────────────
        this.pnlCard.Width     = 400;
        this.pnlCard.Height    = 560;
        this.pnlCard.BackColor = Color.White;
        this.pnlCard.Paint    += new PaintEventHandler(this.pnlCard_Paint);

        // ── lblTitle ──────────────────────────────────────────────────────
        this.lblTitle.Text      = "Добро пожаловать";
        this.lblTitle.Font      = AppTheme.F(18f, FontStyle.Bold);
        this.lblTitle.ForeColor = AppTheme.TextPrimary;
        this.lblTitle.AutoSize  = false;
        this.lblTitle.Size      = new Size(340, 32);
        this.lblTitle.Location  = new Point(30, 36);

        // ── lblSub ────────────────────────────────────────────────────────
        this.lblSub.Text      = "Войдите в систему EduProgress";
        this.lblSub.Font      = AppTheme.FontBody;
        this.lblSub.ForeColor = AppTheme.TextMuted;
        this.lblSub.AutoSize  = false;
        this.lblSub.Size      = new Size(340, 22);
        this.lblSub.Location  = new Point(30, 72);

        // ── pnlRoles ──────────────────────────────────────────────────────
        this.pnlRoles.Location      = new Point(30, 108);
        this.pnlRoles.Size          = new Size(340, 44);
        this.pnlRoles.FlowDirection = FlowDirection.LeftToRight;
        this.pnlRoles.WrapContents  = false;
        this.pnlRoles.BackColor     = AppTheme.Background;
        this.pnlRoles.Padding       = new Padding(4);
        this.pnlRoles.Paint        += new PaintEventHandler(this.pnlRoles_Paint);

        // ── lblEmailCaption ───────────────────────────────────────────────
        this.lblEmailCaption.Text      = "Логин";
        this.lblEmailCaption.Font      = AppTheme.FontSmallBold;
        this.lblEmailCaption.ForeColor = AppTheme.TextPrimary;
        this.lblEmailCaption.AutoSize  = false;
        this.lblEmailCaption.Size      = new Size(340, 20);
        this.lblEmailCaption.Location  = new Point(30, 164);

        // ── txtEmail ──────────────────────────────────────────────────────
        this.txtEmail.Location    = new Point(30, 186);
        this.txtEmail.Size        = new Size(340, 36);
        this.txtEmail.Font        = AppTheme.FontInput;
        this.txtEmail.BorderStyle = BorderStyle.FixedSingle;
        this.txtEmail.BackColor   = AppTheme.Background;
        this.txtEmail.ForeColor   = AppTheme.TextPrimary;

        // ── lblPassCaption ────────────────────────────────────────────────
        this.lblPassCaption.Text      = "Пароль";
        this.lblPassCaption.Font      = AppTheme.FontSmallBold;
        this.lblPassCaption.ForeColor = AppTheme.TextPrimary;
        this.lblPassCaption.AutoSize  = false;
        this.lblPassCaption.Size      = new Size(340, 20);
        this.lblPassCaption.Location  = new Point(30, 234);

        // ── txtPassword ───────────────────────────────────────────────────
        this.txtPassword.Location     = new Point(30, 256);
        this.txtPassword.Size         = new Size(340, 36);
        this.txtPassword.Font         = AppTheme.FontInput;
        this.txtPassword.BorderStyle  = BorderStyle.FixedSingle;
        this.txtPassword.BackColor    = AppTheme.Background;
        this.txtPassword.ForeColor    = AppTheme.TextPrimary;
        this.txtPassword.PasswordChar = '●';

        // ── lblError ──────────────────────────────────────────────────────
        this.lblError.Text      = "";
        this.lblError.Font      = AppTheme.FontSmall;
        this.lblError.ForeColor = AppTheme.Danger;
        this.lblError.AutoSize  = false;
        this.lblError.Size      = new Size(340, 20);
        this.lblError.Location  = new Point(30, 302);

        // ── btnLogin ──────────────────────────────────────────────────────
        this.btnLogin.Text      = "Войти";
        this.btnLogin.Location  = new Point(30, 324);
        this.btnLogin.Size      = new Size(340, 44);
        this.btnLogin.BackColor = AppTheme.Accent;
        this.btnLogin.ForeColor = Color.White;
        this.btnLogin.FlatStyle = FlatStyle.Flat;
        this.btnLogin.Font      = AppTheme.F(12f, FontStyle.Bold);
        this.btnLogin.Cursor    = Cursors.Hand;
        this.btnLogin.FlatAppearance.BorderSize = 0;
        this.btnLogin.Click    += new EventHandler(this.btnLogin_Click);

        // ── lblHint ───────────────────────────────────────────────────────
        //this.lblHint.Text      = "Выберите роль — данные заполнятся автоматически";
        this.lblHint.Font      = AppTheme.FontSmall;
        this.lblHint.ForeColor = AppTheme.TextMuted;
        this.lblHint.AutoSize  = false;
        this.lblHint.Size      = new Size(340, 20);
        this.lblHint.Location  = new Point(30, 380);
        this.lblHint.TextAlign = ContentAlignment.MiddleCenter;

        // ── Assemble card ──────────────────────────────────────────────────
        this.pnlCard.Controls.AddRange(new Control[] {
            this.lblTitle, this.lblSub, this.pnlRoles,
            this.lblEmailCaption, this.txtEmail,
            this.lblPassCaption,  this.txtPassword,
            this.lblError, this.btnLogin, this.lblHint
        });

        // ── Form ──────────────────────────────────────────────────────────
        this.Controls.Add(this.pnlLeft);
        this.Controls.Add(this.pnlRight);
        this.pnlRight.BringToFront();

        this.Text            = "EduProgress — Вход";
        this.Size            = new Size(1100, 700);
        this.MinimumSize     = new Size(800, 600);
        this.StartPosition   = FormStartPosition.CenterScreen;
        this.BackColor       = AppTheme.Primary;
        this.Font            = AppTheme.FontBody;
        this.FormBorderStyle = FormBorderStyle.Sizable;

        this.ResumeLayout(false);
    }

    // ── Field declarations ─────────────────────────────────────────────────
    private Panel          pnlLeft;
    private Panel          pnlRight;
    private Panel          pnlCard;
    private Label          lblTitle;
    private Label          lblSub;
    private FlowLayoutPanel pnlRoles;
    private Label          lblEmailCaption;
    private TextBox        txtEmail;
    private Label          lblPassCaption;
    private TextBox        txtPassword;
    private Label          lblError;
    private Button         btnLogin;
    private Label          lblHint;
}
