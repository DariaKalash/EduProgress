using EduProgress.Models;

namespace EduProgress.Pages;

/// <summary>Administrator: user management screen.</summary>
public partial class UsersPage : BasePage
{
    private DataGridView _grid     = null!;
    private string       _filter   = "all";
    private List<User>   _allUsers = new();

    public UsersPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => pnlOuter.Width = Width;
        LoadData();
    }

    private void LoadData()
    {
        _allUsers = AppSession.Db.GetAllUsers();
        Build();
        pnlOuter.Width = Width;
    }

    private void Build()
    {
        pnlOuter.Controls.Clear();

        // ── Title + Add button ────────────────────────────────────────────────
        var header   = new Panel { Height = 48, Width = 900, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 20) };
        var titleLbl = MakeLbl("Управление пользователями", AppTheme.FontH1);
        titleLbl.Location = new Point(0, 6);

        var addBtn = MakePrimBtn("+ Добавить", 140, 36);
        addBtn.Location  = new Point(760, 6);
        addBtn.Click    += (_, _) => ShowAddUserDialog();

        header.Controls.AddRange(new Control[] { titleLbl, addBtn });
        pnlOuter.Controls.Add(header);

        // ── Stat cards ────────────────────────────────────────────────────────
        var statsRow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize      = true,
            BackColor     = AppTheme.Background,
            Margin        = new Padding(0, 0, 0, 20),
        };
        statsRow.Controls.Add(MakeStatCard("Всего",         _allUsers.Count.ToString(),                                          AppTheme.Accent));
        statsRow.Controls.Add(MakeStatCard("Преподавателей", _allUsers.Count(u => u.Role == UserRole.Teacher).ToString(),        AppTheme.Success));
        statsRow.Controls.Add(MakeStatCard("Методистов",    _allUsers.Count(u => u.Role == UserRole.Methodist).ToString(),       AppTheme.Warning));
        statsRow.Controls.Add(MakeStatCard("Активных",      _allUsers.Count(u => u.IsActive).ToString(),                        AppTheme.Purple));
        pnlOuter.Controls.Add(statsRow);

        // ── Filter + Search ───────────────────────────────────────────────────
        var toolbar = new Panel { Height = 44, Width = 900, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 12) };

        var searchBox = new TextBox
        {
            Size        = new Size(260, 32),
            Location    = new Point(0, 6),
            Font        = AppTheme.FontInput,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor   = AppTheme.White,
            Text        = "Поиск по имени или email…",
        };
        searchBox.Enter       += (_, _) => { if (searchBox.Text.Contains("Поиск")) searchBox.Text = ""; };
        searchBox.Leave       += (_, _) => { if (searchBox.Text == "") searchBox.Text = "Поиск по имени или email…"; };
        searchBox.TextChanged += (_, _) => RefreshGrid(searchBox.Text.Contains("Поиск") ? "" : searchBox.Text);

        var roleLabels = new[] { "Все", "Преподаватели", "Методисты", "Администраторы" };
        var roleKeys   = new[] { "all", "teacher",       "methodist", "admin"          };
        var roleBtns   = new List<Button>();
        for (int i = 0; i < roleLabels.Length; i++)
        {
            int idx = i;
            var rb = new Button
            {
                Text      = roleLabels[i],
                Tag       = roleKeys[i],
                Size      = new Size(130, 32),
                Location  = new Point(270 + i * 138, 6),
                FlatStyle = FlatStyle.Flat,
                Font      = AppTheme.FontSmall,
                Cursor    = Cursors.Hand,
                BackColor = i == 0 ? AppTheme.Accent : AppTheme.White,
                ForeColor = i == 0 ? Color.White      : AppTheme.TextPrimary,
            };
            rb.FlatAppearance.BorderSize  = 1;
            rb.FlatAppearance.BorderColor = i == 0 ? AppTheme.Accent : AppTheme.Border;
            rb.Click += (_, _) =>
            {
                _filter = roleKeys[idx];
                foreach (var b in roleBtns)
                {
                    bool sel = b.Tag?.ToString() == _filter;
                    b.BackColor = sel ? AppTheme.Accent : AppTheme.White;
                    b.ForeColor = sel ? Color.White      : AppTheme.TextPrimary;
                    b.FlatAppearance.BorderColor = sel ? AppTheme.Accent : AppTheme.Border;
                }
                RefreshGrid("");
            };
            roleBtns.Add(rb);
            toolbar.Controls.Add(rb);
        }
        toolbar.Controls.Add(searchBox);
        pnlOuter.Controls.Add(toolbar);

        // ── Grid ──────────────────────────────────────────────────────────────
        var tableCard = MakeCard(900, 340, 0);
        tableCard.Margin = new Padding(0);

        _grid = new DataGridView
        {
            Dock                  = DockStyle.Fill,
            BackgroundColor       = AppTheme.White,
            BorderStyle           = BorderStyle.None,
            RowHeadersVisible     = false,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            ReadOnly              = true,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            Font                  = AppTheme.FontSmall,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight   = 38,
            RowTemplate           = { Height = 40 },
        };
        _grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.Background;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextMuted;
        _grid.ColumnHeadersDefaultCellStyle.Font      = AppTheme.FontSmallBold;
        _grid.ColumnHeadersDefaultCellStyle.Padding   = new Padding(8, 0, 0, 0);
        _grid.DefaultCellStyle.Padding                = new Padding(8, 4, 8, 4);
        _grid.DefaultCellStyle.SelectionBackColor     = Color.FromArgb(235, 246, 255);
        _grid.DefaultCellStyle.SelectionForeColor     = AppTheme.TextPrimary;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name",   HeaderText = "Имя",            FillWeight = 28 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email",  HeaderText = "Email",          FillWeight = 25 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role",   HeaderText = "Роль",           FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Last",   HeaderText = "Последний вход", FillWeight = 18 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Статус",         FillWeight = 10 });

        var editCol = new DataGridViewButtonColumn
        {
            Name                        = "Edit",
            HeaderText                  = "",
            Text                        = "Изменить",
            UseColumnTextForButtonValue = true,
            FillWeight                  = 10,
        };
        _grid.Columns.Add(editCol);
        _grid.CellClick += OnGridCellClick;

        tableCard.Controls.Add(_grid);
        pnlOuter.Controls.Add(tableCard);

        RefreshGrid("");
    }

    private void RefreshGrid(string search)
    {
        _grid.Rows.Clear();
        var all = _allUsers.AsEnumerable();

        if (_filter != "all")
        {
            UserRole role = _filter switch
            {
                "teacher"   => UserRole.Teacher,
                "methodist" => UserRole.Methodist,
                _           => UserRole.Administrator,
            };
            all = all.Where(u => u.Role == role);
        }
        if (!string.IsNullOrWhiteSpace(search))
            all = all.Where(u =>
                u.Name.Contains(search,  StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));

        foreach (var u in all)
        {
            int ri = _grid.Rows.Add(u.Name, u.Email, u.RoleLabel,
                u.LastActive.ToString("dd.MM.yyyy HH:mm"),
                u.IsActive ? "Активен" : "Неактивен");
            _grid.Rows[ri].Tag = u;
            _grid.Rows[ri].DefaultCellStyle.ForeColor =
                u.IsActive ? AppTheme.TextPrimary : AppTheme.TextLight;
        }
    }

    private void OnGridCellClick(object? s, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
        if (_grid.Columns[e.ColumnIndex].Name != "Edit") return;

        var user = _grid.Rows[e.RowIndex].Tag as User;
        if (user == null) return;
        ShowEditDialog(user);
    }

    private void ShowEditDialog(User user)
    {
        var dlg = new Form
        {
            Text            = $"Редактировать: {user.Name}",
            Size            = new Size(420, 320),
            StartPosition   = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox     = false,
            Font            = AppTheme.FontBody,
        };

        int y = 20;
        void AddRow(string label, Control ctrl)
        {
            dlg.Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = AppTheme.FontSmallBold });
            ctrl.Location = new Point(20, y + 22); dlg.Controls.Add(ctrl); y += 60;
        }

        var nameBox    = new TextBox  { Size = new Size(360, 28), Text = user.Name,  Font = AppTheme.FontInput };
        var emailBox   = new TextBox  { Size = new Size(360, 28), Text = user.Email, Font = AppTheme.FontInput };
        var roleCombo  = new ComboBox { Size = new Size(360, 28), Font = AppTheme.FontInput, DropDownStyle = ComboBoxStyle.DropDownList };
        roleCombo.Items.AddRange(new object[] { "Преподаватель", "Методист", "Администратор" });
        roleCombo.SelectedIndex = (int)user.Role;

        var activeCheck = new CheckBox { Text = "Активен", Location = new Point(20, y), AutoSize = true, Checked = user.IsActive, Font = AppTheme.FontBody };
        y += 36;

        AddRow("Имя",   nameBox);
        AddRow("Email", emailBox);
        AddRow("Роль",  roleCombo);
        dlg.Controls.Add(activeCheck);

        var saveBtn = MakePrimBtn("Сохранить", 120, 36);
        saveBtn.Location = new Point(20, y + 10);
        saveBtn.Click += (_, _) =>
        {
            user.Name     = nameBox.Text.Trim();
            user.Email    = emailBox.Text.Trim();
            user.Role     = (UserRole)roleCombo.SelectedIndex;
            user.IsActive = activeCheck.Checked;
            AppSession.Db.UpdateUser(user);
            RefreshGrid("");
            dlg.Close();
        };
        dlg.Controls.Add(saveBtn);

        dlg.ShowDialog(this);
    }

    private void ShowAddUserDialog()
    {
        MessageBox.Show(
            "Добавление пользователей выполняется через SQL INSERT в таблицу users.\nОбратитесь к администратору БД.",
            "Добавить пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static Panel MakeStatCard(string label, string value, Color accent)
    {
        var card = MakeCard(200, 90, 0);
        card.Margin = new Padding(0, 0, 16, 0);
        var bar  = new Panel { Height = 4, Dock = DockStyle.Top, BackColor = accent };
        var vLbl = new Label { Text = value, Font = AppTheme.F(22f, FontStyle.Bold), ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 14) };
        var lLbl = new Label { Text = label, Font = AppTheme.FontSmall,             ForeColor = AppTheme.TextMuted,    AutoSize = true, Location = new Point(16, 56) };
        card.Controls.AddRange(new Control[] { bar, vLbl, lLbl });
        return card;
    }

    private static Label MakeLbl(string t, Font f, Color? c = null) =>
        new Label { Text = t, Font = f, ForeColor = c ?? AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent };
}
