using EduProgress.Models;

namespace EduProgress.Forms;

public partial class LoginForm : Form
{
    private UserRole     _selectedRole = UserRole.Teacher;
    private List<Button> _roleBtns     = new();

    public LoginForm()
    {
        InitializeComponent();
        BuildRoleButtons();
        SelectRole(UserRole.Teacher);
    }

    // ── Role buttons ──────────────────────────────────────────────────────

    private void BuildRoleButtons()
    {
        var roles = new[]
        {
            (UserRole.Teacher,       "Педагог"),
            (UserRole.Methodist,     "Методист"),
            (UserRole.Administrator, "Админ"),
        };

        foreach (var (role, label) in roles)
        {
            var r = role;
            var btn = new Button
            {
                Text      = label,
                Size      = new Size(104, 34),
                FlatStyle = FlatStyle.Flat,
                Font      = AppTheme.FontSmall,
                Cursor    = Cursors.Hand,
                Tag       = r,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (_, _) => SelectRole(r);
            _roleBtns.Add(btn);
            pnlRoles.Controls.Add(btn);
        }
    }

    private void SelectRole(UserRole role)
    {
        _selectedRole = role;
        foreach (var b in _roleBtns)
        {
            bool sel    = (UserRole)b.Tag! == role;
            b.BackColor = sel ? AppTheme.Accent  : Color.Transparent;
            b.ForeColor = sel ? Color.White       : AppTheme.TextPrimary;
        }
        //txtEmail.Text = role switch
        //{
        //    UserRole.Administrator => "admin",
        //    UserRole.Methodist     => "petrov",
        //    _                      => "ivanova",
        //};
        //txtPassword.Text = "1234";
    }

    // ── Events ────────────────────────────────────────────────────────────

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";
        var login    = txtEmail.Text.Trim();
        var password = txtPassword.Text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            lblError.Text = "Введите логин и пароль";
            return;
        }

        var user = AppSession.Db.Authenticate(login, password);
        if (user == null)
        {
            lblError.Text = "Неверный логин или пароль";
            return;
        }

        if (user.Role != _selectedRole)
        {
            lblError.Text = "Неверная роль пользователя";
            return;
        }

        AppSession.CurrentUser = user;
        var main = new MainForm();
        main.Show();
        Hide();
    }

    private void pnlRight_Resize(object? sender, EventArgs e)
    {
        pnlCard.Location = new Point(
            (pnlRight.Width  - pnlCard.Width)  / 2,
            (pnlRight.Height - pnlCard.Height) / 2);
    }

    private void pnlLeft_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel p) return;
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using var cb1 = new SolidBrush(Color.FromArgb(20, 255, 255, 255));
        using var cb2 = new SolidBrush(Color.FromArgb(12, 255, 255, 255));
        g.FillEllipse(cb1, -60, p.Height - 260, 320, 320);
        g.FillEllipse(cb2, p.Width - 120, -60,  240, 240);

        using var logoFont = new Font("Segoe UI", 32f, FontStyle.Bold);
        using var subFont  = new Font("Segoe UI", 12f);
        using var descFont = new Font("Segoe UI", 11f);
        using var wb  = new SolidBrush(Color.White);
        using var wmt = new SolidBrush(Color.FromArgb(180, 255, 255, 255));

        g.DrawString("EduProgress", logoFont, wb, 40, p.Height / 2 - 120);
        g.DrawString(
            "Информационная система\nсопровождения обучения\nи повышения квалификации\nпреподавателей",
            subFont, wmt, 40, p.Height / 2 - 56);

        //string[] features = { "✓  Три роли пользователей", "✓  Управление курсами", "✓  Отслеживание прогресса", "✓  Аналитика и отчётность" };
        //int fy = p.Height / 2 + 80;
        //foreach (var f in features)
        //{
        //    g.DrawString(f, descFont, wb, 40, fy);
        //    fy += 28;
        //}
    }

    private void pnlCard_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel c) return;
        using var pen = new Pen(AppTheme.Border, 1);
        e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
    }

    private void pnlRoles_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel c) return;
        using var pen = new Pen(AppTheme.Border, 1);
        e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
    }
}
