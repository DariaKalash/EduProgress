using EduProgress.Controls;
using EduProgress.Models;
using EduProgress.Pages;

namespace EduProgress.Forms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        AppSession.NavigateTo = NavigateTo;
        NavigateTo(
            AppSession.CurrentUser?.Role == UserRole.Teacher
                ? "dashboard"
                : "reports",
            null
        );
        this.FormClosed += (_, _) => Application.Exit();
    }

    public void NavigateTo(string page, object? param)
    {
        pnlContent.Controls.Clear();

        UserControl pageCtrl = page switch
        {
            "dashboard"     => new DashboardPage(),
            "catalog"       => new CatalogPage(),
            "course"        => new CoursePage((Course)param!),
            "lesson"        => new LessonPage((Lesson)param!),
            "progress"      => new ProgressPage(),
            "notifications" => new NotificationsPage(),
            "faq"           => new FaqPage(),
            "reports"       => new ReportsPage(),
            "users"         => new UsersPage(),
            "courseeditor" => param is Course course
                            ? new CourseEditorPage(course)
                            : new CourseEditorPage(),
            _               => new DashboardPage(),
        };

        pageCtrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(pageCtrl);
        sidebar.SetActive(page);
        header.SetTitle(PageTitle(page));
    }

    private void sidebar_NavClicked(object? sender, string page) => NavigateTo(page, null);

    private static string PageTitle(string p) => p switch
    {
        "dashboard"     => "Главная",
        "catalog"       => "Каталог курсов",
        "course"        => "Страница курса",
        "lesson"        => "Урок",
        "progress"      => "Мой прогресс",
        "notifications" => "Уведомления",
        "faq"           => "Часто задаваемые вопросы",
        "reports"       => "Отчётность",
        "users"         => "Управление пользователями",
        "courseeditor"  => "Редактор курсов",
        _               => "EduProgress",
    };
}
