using EduProgress.Models;

namespace EduProgress.Services;

public class MockDataService
{
    public List<User>         Users         { get; private set; }
    public List<Course>       Courses       { get; private set; }
    public List<Notification> Notifications { get; private set; }

    public MockDataService()
    {
        Users         = BuildUsers();
        Courses       = BuildCourses();
        Notifications = BuildNotifications();
    }

    // ── Auth ──────────────────────────────────────────────────────────────────
    public User? Authenticate(string email, string password)
        => Users.FirstOrDefault(u =>
               u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
               u.Password == password);

    // ── Users ─────────────────────────────────────────────────────────────────
    private static List<User> BuildUsers() => new()
    {
        new User { Id=1, Name="Администратов Иван Петрович", Email="admin@edu.ru",    Password="admin",     Role=UserRole.Administrator, LastActive=DateTime.Now,                    IsActive=true  },
        new User { Id=2, Name="Методистова Анна Сергеевна",  Email="methodist@edu.ru", Password="methodist", Role=UserRole.Methodist,     LastActive=DateTime.Now.AddHours(-2),       IsActive=true  },
        new User { Id=3, Name="Преподавателев Пётр Иванович",Email="teacher@edu.ru",  Password="teacher",   Role=UserRole.Teacher,       LastActive=DateTime.Now.AddDays(-1),        IsActive=true  },
        new User { Id=4, Name="Смирнова Елена Викторовна",   Email="smirnova@edu.ru", Password="123456",    Role=UserRole.Teacher,       LastActive=DateTime.Now.AddDays(-3),        IsActive=true  },
        new User { Id=5, Name="Козлов Дмитрий Александрович",Email="kozlov@edu.ru",   Password="123456",    Role=UserRole.Teacher,       LastActive=DateTime.Now.AddDays(-7),        IsActive=false },
        new User { Id=6, Name="Новикова Мария Олеговна",     Email="novikova@edu.ru", Password="123456",    Role=UserRole.Teacher,       LastActive=DateTime.Now.AddMinutes(-30),    IsActive=true  },
    };

    // ── Courses ───────────────────────────────────────────────────────────────
    private static List<Course> BuildCourses()
    {
        var c1 = new Course
        {
            Id=1, Title="Современные методы обучения", Category="Педагогика",
            Description="Освойте актуальные педагогические технологии: проектный метод, смешанное обучение, геймификация и цифровые инструменты для повышения вовлечённости студентов.",
            TotalHours=36, EnrolledCount=24, IsNew=true, Status=CourseStatus.Enrolled, Progress=65,
            Modules = new List<Module>
            {
                new Module { Id=1, CourseId=1, Title="Введение в современную педагогику", Order=1,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=1, ModuleId=1, Order=1, Title="Тенденции развития образования", DurationMinutes=45, Status=LessonStatus.Completed,
                            Content="В данном уроке рассматриваются ключевые тенденции развития современного образования: цифровизация, персонализация обучения, компетентностный подход.\n\nОбразование в XXI веке претерпевает значительные изменения под влиянием цифровых технологий и новых требований рынка труда. Преподаватели должны адаптироваться к новым реалиям и осваивать современные инструменты.",
                            Resources = new List<LessonResource>
                            {
                                new LessonResource { Name="Тенденции образования 2024.pdf", Type="pdf", Url="resources/trends.pdf" },
                                new LessonResource { Name="Введение — видеолекция",          Type="video", Url="https://example.com/video1" },
                            }
                        },
                        new Lesson { Id=2, ModuleId=1, Order=2, Title="Компетентностный подход", DurationMinutes=60, Status=LessonStatus.Completed,
                            Content="Компетентностный подход в образовании ориентирован на формирование у обучающихся способности применять знания и умения в практической деятельности.",
                            ZoomLink="https://zoom.us/j/123456789",
                            Resources = new List<LessonResource>
                            {
                                new LessonResource { Name="Компетентностный подход.pdf", Type="pdf", Url="resources/competence.pdf" },
                            }
                        },
                        new Lesson { Id=3, ModuleId=1, Order=3, Title="Цифровые инструменты преподавателя", DurationMinutes=90, Status=LessonStatus.InProgress,
                            Content="Обзор цифровых инструментов, которые преподаватели могут использовать в своей работе: LMS-платформы, интерактивные доски, системы голосования, инструменты для создания контента.",
                            ZoomLink="https://zoom.us/j/987654321",
                            Resources = new List<LessonResource>
                            {
                                new LessonResource { Name="Цифровые инструменты.xlsx", Type="pdf", Url="resources/tools.xlsx" },
                                new LessonResource { Name="Практические задания",      Type="link", Url="https://example.com/tasks" },
                            }
                        },
                    }
                },
                new Module { Id=2, CourseId=1, Title="Активные методы обучения", Order=2,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=4, ModuleId=2, Order=1, Title="Проектный метод", DurationMinutes=75, Status=LessonStatus.NotStarted,
                            Content="Проектный метод — педагогическая технология, ориентированная на применение и приобретение новых знаний путём самостоятельной деятельности обучающихся." },
                        new Lesson { Id=5, ModuleId=2, Order=2, Title="Кейс-метод (Case Study)", DurationMinutes=60, Status=LessonStatus.NotStarted,
                            Content="Кейс-метод — метод анализа конкретных ситуаций, позволяющий обучающимся применять теоретические знания для решения практических задач.",
                            ZoomLink="https://zoom.us/j/111222333" },
                        new Lesson { Id=6, ModuleId=2, Order=3, Title="Геймификация в обучении", DurationMinutes=45, Status=LessonStatus.NotStarted,
                            Content="Геймификация — применение игровых механик и элементов в неигровых контекстах для повышения вовлечённости и мотивации обучающихся." },
                    }
                },
                new Module { Id=3, CourseId=1, Title="Оценивание и обратная связь", Order=3,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=7, ModuleId=3, Order=1, Title="Формирующее оценивание", DurationMinutes=60, Status=LessonStatus.NotStarted,
                            Content="Формирующее оценивание направлено на мониторинг прогресса обучающегося в процессе обучения для предоставления обратной связи." },
                        new Lesson { Id=8, ModuleId=3, Order=2, Title="Обратная связь как инструмент развития", DurationMinutes=45, Status=LessonStatus.NotStarted,
                            Content="Качественная обратная связь является одним из ключевых факторов, влияющих на успешность обучения." },
                    }
                },
            }
        };

        var c2 = new Course
        {
            Id=2, Title="Информационные технологии в образовании", Category="ИТ",
            Description="Практический курс по использованию IT-инструментов в учебном процессе: создание электронных курсов, работа с LMS, интерактивный контент и дистанционное обучение.",
            TotalHours=24, EnrolledCount=18, IsNew=false, Status=CourseStatus.Enrolled, Progress=30,
            Modules = new List<Module>
            {
                new Module { Id=4, CourseId=2, Title="Основы работы с LMS", Order=1,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=9,  ModuleId=4, Order=1, Title="Обзор LMS-платформ", DurationMinutes=60, Status=LessonStatus.Completed,
                            Content="Рассматриваются основные LMS-платформы: Moodle, Canvas, Blackboard, Google Classroom. Сравнительный анализ функциональности и области применения." },
                        new Lesson { Id=10, ModuleId=4, Order=2, Title="Настройка курса в Moodle", DurationMinutes=90, Status=LessonStatus.InProgress,
                            Content="Практическое занятие по созданию и настройке учебного курса в системе Moodle.", ZoomLink="https://zoom.us/j/555666777" },
                        new Lesson { Id=11, ModuleId=4, Order=3, Title="Управление контингентом", DurationMinutes=45, Status=LessonStatus.NotStarted,
                            Content="Работа со студентами в LMS: запись, группы, роли и права доступа." },
                    }
                },
                new Module { Id=5, CourseId=2, Title="Создание интерактивного контента", Order=2,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=12, ModuleId=5, Order=1, Title="Инструменты для создания видеолекций", DurationMinutes=60, Status=LessonStatus.NotStarted,
                            Content="Обзор инструментов: OBS Studio, Camtasia, Loom. Основы записи и монтажа видеолекций." },
                        new Lesson { Id=13, ModuleId=5, Order=2, Title="H5P: интерактивный контент", DurationMinutes=75, Status=LessonStatus.NotStarted,
                            Content="H5P — инструмент для создания интерактивного образовательного контента: тесты, игры, интерактивные видео." },
                    }
                },
            }
        };

        var c3 = new Course
        {
            Id=3, Title="Научно-исследовательская деятельность", Category="Наука",
            Description="Методология научных исследований, публикационная активность, работа с наукометрическими базами данных и подготовка научных статей к публикации.",
            TotalHours=18, EnrolledCount=12, IsNew=true, Status=CourseStatus.Available, Progress=0,
            Modules = new List<Module>
            {
                new Module { Id=6, CourseId=3, Title="Методология исследований", Order=1,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=14, ModuleId=6, Order=1, Title="Научная методология", DurationMinutes=60, Status=LessonStatus.NotStarted,
                            Content="Введение в методологию научных исследований. Виды исследований, этапы проведения, методы сбора и анализа данных." },
                        new Lesson { Id=15, ModuleId=6, Order=2, Title="Наукометрия и рейтинги", DurationMinutes=45, Status=LessonStatus.NotStarted,
                            Content="Наукометрические показатели: h-индекс, импакт-фактор. Работа с базами Web of Science и Scopus." },
                    }
                },
                new Module { Id=7, CourseId=3, Title="Публикационная деятельность", Order=2,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=16, ModuleId=7, Order=1, Title="Структура научной статьи", DurationMinutes=60, Status=LessonStatus.NotStarted,
                            Content="Разбор структуры научной статьи: введение, методология, результаты, обсуждение, заключение." },
                        new Lesson { Id=17, ModuleId=7, Order=2, Title="Публикация в рецензируемых журналах", DurationMinutes=90, Status=LessonStatus.NotStarted,
                            Content="Процесс подачи и рецензирования статьи в международных и российских рецензируемых журналах." },
                    }
                },
            }
        };

        var c4 = new Course
        {
            Id=4, Title="Психология педагогического общения", Category="Психология",
            Description="Психологические основы взаимодействия преподавателя и студента: коммуникация, конфликты, мотивация и создание благоприятного учебного климата.",
            TotalHours=12, EnrolledCount=31, IsNew=false, Status=CourseStatus.Completed, Progress=100,
            Modules = new List<Module>
            {
                new Module { Id=8, CourseId=4, Title="Основы педагогической коммуникации", Order=1,
                    Lessons = new List<Lesson>
                    {
                        new Lesson { Id=18, ModuleId=8, Order=1, Title="Коммуникативная компетентность", DurationMinutes=60, Status=LessonStatus.Completed,
                            Content="Коммуникативная компетентность преподавателя: вербальные и невербальные компоненты общения." },
                        new Lesson { Id=19, ModuleId=8, Order=2, Title="Педагогические стили общения", DurationMinutes=45, Status=LessonStatus.Completed,
                            Content="Демократический, авторитарный и либеральный стили педагогического общения. Эффективность различных стилей." },
                    }
                },
            }
        };

        return new List<Course> { c1, c2, c3, c4 };
    }

    // ── Notifications ─────────────────────────────────────────────────────────
    private static List<Notification> BuildNotifications() => new()
    {
        new Notification { Id=1, Title="Новый урок доступен",      Message="В курсе «Современные методы обучения» добавлен урок «Геймификация в обучении».",       Category=NotificationCategory.Course,   CreatedAt=DateTime.Now.AddMinutes(-15), IsRead=false },
        new Notification { Id=2, Title="Напоминание о вебинаре",   Message="Завтра в 14:00 состоится вебинар «Компетентностный подход» по курсу ИТ в образовании.", Category=NotificationCategory.Reminder, CreatedAt=DateTime.Now.AddHours(-2),    IsRead=false },
        new Notification { Id=3, Title="Курс завершён",            Message="Вы успешно завершили курс «Психология педагогического общения»! Поздравляем!",          Category=NotificationCategory.Course,   CreatedAt=DateTime.Now.AddHours(-5),    IsRead=true  },
        new Notification { Id=4, Title="Обновление системы",       Message="Плановое обновление платформы EduProgress состоится 25 марта с 2:00 до 4:00.",           Category=NotificationCategory.System,   CreatedAt=DateTime.Now.AddDays(-1),     IsRead=true  },
        new Notification { Id=5, Title="Дедлайн приближается",     Message="До завершения курса «ИТ в образовании» осталось 7 дней. Не забудьте пройти все уроки.",  Category=NotificationCategory.Reminder, CreatedAt=DateTime.Now.AddDays(-2),     IsRead=true  },
        new Notification { Id=6, Title="Новый курс в каталоге",    Message="Добавлен курс «Научно-исследовательская деятельность». Запишитесь прямо сейчас!",        Category=NotificationCategory.Course,   CreatedAt=DateTime.Now.AddDays(-3),     IsRead=true  },
        new Notification { Id=7, Title="Сертификат готов",         Message="Ваш сертификат о повышении квалификации по курсу «Психология» доступен для скачивания.", Category=NotificationCategory.System,   CreatedAt=DateTime.Now.AddDays(-5),     IsRead=true  },
    };
}
