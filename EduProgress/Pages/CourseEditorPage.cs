using EduProgress.Models;

namespace EduProgress.Pages;

/// <summary>Methodist: course creation and editing screen.</summary>
public partial class CourseEditorPage : BasePage
{
    private List<Course> _courses       = new();
    private Course?      _selectedCourse = null;
    private Module?      _selectedModule = null;
    private Lesson?      _selectedLesson = null;
    private TreeView     _tree           = null!;
    private Panel        _editorPanel    = null!;

    public CourseEditorPage()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        _courses = AppSession.Db.GetAllCoursesWithStructure();
        Build();
    }

    private void Build()
    {
        Controls.Clear();

        // ── Top toolbar ───────────────────────────────────────────────────────
        var toolbar = new Panel
        {
            Height    = 60,
            Dock      = DockStyle.Top,
            BackColor = AppTheme.White,
            Padding   = new Padding(20, 0, 20, 0),
        };
        toolbar.Paint += DrawBottomBorder;

        //var titleLbl = new Label
        //{
        //    Text      = "Редактор курсов",
        //    Font      = AppTheme.FontH1,
        //    ForeColor = AppTheme.TextPrimary,
        //    AutoSize  = true,
        //    Location  = new Point(20, 14),
        //};
        var newBtn = MakePrimBtn("+ Создать курс", 160, 36);
        newBtn.Location  = new Point(20, 14);
        newBtn.Click    += (_, _) => NewCourse();
        toolbar.SizeChanged += (_, _) => newBtn.Location = new Point(toolbar.Width - 180, 14);

        toolbar.Controls.AddRange(new Control[] {newBtn });

        // ── Split: left = tree, right = form ──────────────────────────────────
        var split = new SplitContainer
        {
            Dock          = DockStyle.Fill,
            SplitterWidth = 1,
            Panel1MinSize = 0,
            Panel2MinSize = 0,
            BackColor     = AppTheme.Border,
        };
        split.Panel1.BackColor = AppTheme.White;
        split.Panel2.BackColor = AppTheme.Background;

        // ── Left: tree ────────────────────────────────────────────────────────
        var treeHeader = new Panel { Height = 44, Dock = DockStyle.Top, BackColor = AppTheme.Background };
        treeHeader.Paint += DrawBottomBorder;
        treeHeader.Controls.Add(new Label { Text = "Структура курсов", Font = AppTheme.FontH3, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(12, 10) });

        _tree = new TreeView
        {
            Dock          = DockStyle.Fill,
            BackColor     = AppTheme.White,
            ForeColor     = AppTheme.TextPrimary,
            Font          = AppTheme.FontBody,
            BorderStyle   = BorderStyle.None,
            ShowPlusMinus = true,
            ShowLines     = true,
            FullRowSelect = true,
            HotTracking   = true,
            ItemHeight    = 28,
            Indent        = 16,
        };
        RebuildTree();
        _tree.AfterSelect += OnTreeSelect;

        var treeActions = new FlowLayoutPanel
        {
            Dock          = DockStyle.Bottom,
            Height        = 44,
            BackColor     = AppTheme.Background,
            Padding       = new Padding(6),
            FlowDirection = FlowDirection.LeftToRight,
        };
        treeActions.Paint += DrawTopBorder;

        var addModBtn = MakeSecBtn("+ Модуль", 100, 30);
        var addLsnBtn = MakeSecBtn("+ Урок",   88,  30);
        var delBtn    = new Button { Text = "✕", Size = new Size(36, 30), FlatStyle = FlatStyle.Flat, BackColor = AppTheme.Danger, ForeColor = Color.White, Cursor = Cursors.Hand };
        delBtn.FlatAppearance.BorderSize = 0;

        addModBtn.Click += (_, _) => AddModule();
        addLsnBtn.Click += (_, _) => AddLesson();
        delBtn.Click    += (_, _) => DeleteSelected();

        treeActions.Controls.AddRange(new Control[] { addModBtn, addLsnBtn, delBtn });

        split.Panel1.Controls.Add(_tree);
        split.Panel1.Controls.Add(treeActions);
        split.Panel1.Controls.Add(treeHeader);

        // ── Right: editor ─────────────────────────────────────────────────────
        _editorPanel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Background, AutoScroll = true };
        ShowWelcome();
        split.Panel2.Controls.Add(_editorPanel);

        split.SplitterDistance = 240;

        Controls.Add(split);
        Controls.Add(toolbar);
    }

    // ── Tree management ───────────────────────────────────────────────────────

    private void RebuildTree()
    {
        _tree.Nodes.Clear();
        foreach (var c in _courses)
        {
            var cNode = new TreeNode($"≡  {c.Title}") { Tag = c, ForeColor = AppTheme.Primary };
            foreach (var m in c.Modules.OrderBy(x => x.Order))
            {
                var mNode = new TreeNode($"▸  {m.Title}") { Tag = m, ForeColor = AppTheme.Accent };
                foreach (var l in m.Lessons.OrderBy(x => x.Order))
                    mNode.Nodes.Add(new TreeNode($"  ○  {l.Title}") { Tag = l, ForeColor = AppTheme.TextMuted });
                cNode.Nodes.Add(mNode);
            }
            _tree.Nodes.Add(cNode);
        }
        _tree.ExpandAll();
    }

    private void OnTreeSelect(object? s, TreeViewEventArgs e)
    {
        if      (e.Node?.Tag is Course c) { _selectedCourse = c; _selectedModule = null; _selectedLesson = null; ShowCourseForm(c); }
        else if (e.Node?.Tag is Module m) { _selectedModule = m; _selectedLesson = null; ShowModuleForm(m); }
        else if (e.Node?.Tag is Lesson l) { _selectedLesson = l; ShowLessonForm(l); }
    }

    // ── Editor forms ──────────────────────────────────────────────────────────

    private void ShowWelcome()
    {
        _editorPanel.Controls.Clear();
        _editorPanel.Controls.Add(new Label
        {
            Text      = "Выберите элемент в дереве слева\nдля редактирования или создайте новый курс.",
            Font      = AppTheme.FontBody,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Fill,
        });
    }

    private void ShowCourseForm(Course c)
    {
        _editorPanel.Controls.Clear();
        var form = BuildFormPanel("Редактировать курс");

        var titleBox = AddField(form, "Название курса",  c.Title,           0);
        var catBox   = AddField(form, "Категория",       c.Category,        60);
        var descBox  = AddTextArea(form, "Описание",     c.Description,     120, 100);
        var hoursBox = AddField(form, "Учебных часов",   c.TotalHours.ToString(), 240);

        var saveBtn = MakePrimBtn("Сохранить изменения", 200, 38);
        saveBtn.Location = new Point(20, 308);
        saveBtn.Click += (_, _) =>
        {
            c.Title       = titleBox.Text.Trim();
            c.Category    = catBox.Text.Trim();
            c.Description = descBox.Text.Trim();
            if (int.TryParse(hoursBox.Text, out int h)) c.TotalHours = h;
            AppSession.Db.UpdateCourse(c.Id, c.Title, c.Description, c.TotalHours, c.Category);
            RebuildTree();
            MessageBox.Show("Курс сохранён.", "EduProgress", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        form.Controls.Add(saveBtn);
        _editorPanel.Controls.Add(form);
    }

    private void ShowModuleForm(Module m)
    {
        _editorPanel.Controls.Clear();
        var form = BuildFormPanel("Редактировать модуль");

        var titleBox = AddField(form, "Название модуля",  m.Title,         0);
        var orderBox = AddField(form, "Порядковый номер", m.Order.ToString(), 60);

        var saveBtn = MakePrimBtn("Сохранить", 160, 38);
        saveBtn.Location = new Point(20, 148);
        saveBtn.Click += (_, _) =>
        {
            m.Title = titleBox.Text.Trim();
            if (int.TryParse(orderBox.Text, out int o)) m.Order = o;
            AppSession.Db.UpdateModule(m.Id, m.Title, m.Order);
            RebuildTree();
            MessageBox.Show("Модуль сохранён.", "EduProgress", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        form.Controls.Add(saveBtn);
        _editorPanel.Controls.Add(form);
    }

    private void ShowLessonForm(Lesson l)
    {
        _editorPanel.Controls.Clear();
        var form = BuildFormPanel("Редактировать урок");

        var titleBox   = AddField(form, "Название урока",     l.Title,                   0);
        var durBox     = AddField(form, "Длительность (мин)", l.DurationMinutes.ToString(), 60);
        var contentBox = AddTextArea(form, "Содержание урока", l.Content,                120, 100);
        var zoomBox    = AddField(form, "Ссылка на Zoom",     l.ZoomLink ?? "",          240);

        var fileLbl  = new Label { Text = "Прикреплённые файлы", Font = AppTheme.FontSmallBold, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(20, 302) };
        var dropZone = new Panel { Location = new Point(20, 322), Size = new Size(560, 60), BackColor = Color.FromArgb(248, 250, 252) };
        dropZone.Paint += (_, e) =>
        {
            using var pen = new Pen(AppTheme.Border, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            e.Graphics.DrawRectangle(pen, 0, 0, dropZone.Width - 1, dropZone.Height - 1);
        };
        var dropLbl = new Label
        {
            Text      = "⊕  Нажмите для выбора файлов или перетащите их сюда",
            Font      = AppTheme.FontSmall,
            ForeColor = AppTheme.TextMuted,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor    = Cursors.Hand,
        };
        dropZone.Controls.Add(dropLbl);
        dropLbl.Click += (_, _) =>
        {
            using var ofd = new OpenFileDialog { Multiselect = true, Title = "Выберите файлы" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var f in ofd.FileNames)
                {
                    string name = Path.GetFileName(f);
                    string ext  = Path.GetExtension(f).TrimStart('.').ToLower();
                    AppSession.Db.AddLessonResource(l.Id, name, ext, f);
                }
                MessageBox.Show($"Добавлено файлов: {ofd.FileNames.Length}", "EduProgress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        var saveBtn = MakePrimBtn("Сохранить урок", 160, 38);
        saveBtn.Location = new Point(20, 400);
        saveBtn.Click += (_, _) =>
        {
            l.Title       = titleBox.Text.Trim();
            if (int.TryParse(durBox.Text, out int d)) l.DurationMinutes = d;
            l.Content     = contentBox.Text.Trim();
            l.ZoomLink    = string.IsNullOrWhiteSpace(zoomBox.Text) ? null : zoomBox.Text.Trim();
            AppSession.Db.UpdateLesson(l);
            RebuildTree();
            MessageBox.Show("Урок сохранён.", "EduProgress", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        form.Controls.AddRange(new Control[] { fileLbl, dropZone, saveBtn });
        _editorPanel.Controls.Add(form);
    }

    // ── Form builder helpers ──────────────────────────────────────────────────

    private static Panel BuildFormPanel(string title)
    {
        var panel = new Panel { Width = 620, AutoSize = true, BackColor = AppTheme.White, Padding = new Padding(20, 16, 20, 20), Margin = new Padding(20) };
        panel.Paint += (_, e) => { using var pen = new Pen(AppTheme.Border, 1); e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1); };
        panel.Controls.Add(new Label { Text = title, Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(20, 16) });
        panel.Controls.Add(new Panel { Location = new Point(20, 42), Size = new Size(580, 1), BackColor = AppTheme.Border });
        return panel;
    }

    private static TextBox AddField(Panel p, string label, string value, int yOffset)
    {
        int y = 56 + yOffset;
        p.Controls.Add(new Label { Text = label, Font = AppTheme.FontSmallBold, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(20, y) });
        var box = new TextBox { Location = new Point(20, y + 22), Size = new Size(560, 28), Font = AppTheme.FontInput, BorderStyle = BorderStyle.FixedSingle, BackColor = AppTheme.Background, Text = value };
        p.Controls.Add(box);
        return box;
    }

    private static TextBox AddTextArea(Panel p, string label, string value, int yOffset, int height)
    {
        int y = 56 + yOffset;
        p.Controls.Add(new Label { Text = label, Font = AppTheme.FontSmallBold, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(20, y) });
        var box = new TextBox { Location = new Point(20, y + 22), Size = new Size(560, height), Font = AppTheme.FontInput, BorderStyle = BorderStyle.FixedSingle, BackColor = AppTheme.Background, Text = value, Multiline = true, ScrollBars = ScrollBars.Vertical };
        p.Controls.Add(box);
        return box;
    }

    // ── CRUD ──────────────────────────────────────────────────────────────────

    private void NewCourse()
    {
        int newId = AppSession.Db.CreateCourse(
            "Новый курс", "Описание курса", 12, "Общий",
            AppSession.CurrentUser!.Id);
        _courses = AppSession.Db.GetAllCoursesWithStructure();
        RebuildTree();
        var newNode = _tree.Nodes.Cast<TreeNode>().LastOrDefault();
        if (newNode != null) _tree.SelectedNode = newNode;
    }

    private void AddModule()
    {
        Course? course = FindCourseFromSelection();
        if (course == null) { MessageBox.Show("Выберите курс для добавления модуля.", "EduProgress", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        int order = course.Modules.Count + 1;
        AppSession.Db.CreateModule(course.Id, "Новый модуль", order);
        _courses = AppSession.Db.GetAllCoursesWithStructure();
        RebuildTree();
    }

    private void AddLesson()
    {
        Module? mod = FindModuleFromSelection();
        if (mod == null) { MessageBox.Show("Выберите модуль для добавления урока.", "EduProgress", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        int order = mod.Lessons.Count + 1;
        AppSession.Db.CreateLesson(mod.Id, "Новый урок", 45, null, order);
        _courses = AppSession.Db.GetAllCoursesWithStructure();
        RebuildTree();
    }

    private void DeleteSelected()
    {
        var node = _tree.SelectedNode;
        if (node == null) return;
        if (MessageBox.Show("Удалить выбранный элемент?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        if      (node.Tag is Course c) AppSession.Db.DeleteCourse(c.Id);
        else if (node.Tag is Module m) AppSession.Db.DeleteModule(m.Id);
        else if (node.Tag is Lesson l) AppSession.Db.DeleteLesson(l.Id);

        _courses = AppSession.Db.GetAllCoursesWithStructure();
        RebuildTree();
        ShowWelcome();
    }

    // ── Selection helpers ─────────────────────────────────────────────────────

    private Course? FindCourseFromSelection()
    {
        var tag = _tree.SelectedNode?.Tag;
        if (tag is Course c) return c;
        if (tag is Module m) return _courses.FirstOrDefault(x => x.Modules.Any(mo => mo.Id == m.Id));
        if (tag is Lesson l)
        {
            var parentMod = _courses.SelectMany(x => x.Modules).FirstOrDefault(mo => mo.Lessons.Any(le => le.Id == l.Id));
            return _courses.FirstOrDefault(x => x.Modules.Any(mo => mo.Id == parentMod?.Id));
        }
        return _courses.FirstOrDefault();
    }

    private Module? FindModuleFromSelection()
    {
        var tag = _tree.SelectedNode?.Tag;
        if (tag is Module m) return m;
        if (tag is Lesson l) return _courses.SelectMany(x => x.Modules).FirstOrDefault(mo => mo.Lessons.Any(le => le.Id == l.Id));
        return null;
    }

    public CourseEditorPage(Course course)
    {
        InitializeComponent();
        LoadData();

        _selectedCourse = course;
        ShowCourseForm(course);
    }

    // ── Draw helpers ──────────────────────────────────────────────────────────

    private static void DrawBottomBorder(object? s, PaintEventArgs e)
    {
        if (s is not Control c) return;
        using var pen = new Pen(AppTheme.Border, 1);
        e.Graphics.DrawLine(pen, 0, c.Height - 1, c.Width, c.Height - 1);
    }

    private static void DrawTopBorder(object? s, PaintEventArgs e)
    {
        if (s is not Control c) return;
        using var pen = new Pen(AppTheme.Border, 1);
        e.Graphics.DrawLine(pen, 0, 0, c.Width, 0);
    }
}
