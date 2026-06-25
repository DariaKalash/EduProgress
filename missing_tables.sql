-- ═══════════════════════════════════════════════════════════════════════════
-- EduProgress — дополнительные таблицы и столбцы
-- Запустить в контексте базы данных EduProgress
-- ═══════════════════════════════════════════════════════════════════════════
USE EduProgress;
GO

-- ─── 1. Новые столбцы в courses ───────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('courses') AND name = 'category')
    ALTER TABLE courses ADD category NVARCHAR(100) NOT NULL DEFAULT N'Общий';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('courses') AND name = 'is_new')
    ALTER TABLE courses ADD is_new BIT NOT NULL DEFAULT 0;
GO

-- ─── 2. Новые столбцы в notifications ────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('notifications') AND name = 'title')
    ALTER TABLE notifications ADD title NVARCHAR(200);
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('notifications') AND name = 'category')
    ALTER TABLE notifications ADD category NVARCHAR(30) NOT NULL DEFAULT N'Курс';
GO

-- ─── 3. Флаг активности пользователя ─────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('users') AND name = 'is_active')
    ALTER TABLE users ADD is_active BIT NOT NULL DEFAULT 1;
GO

-- ─── 4. Таблица ресурсов урока ────────────────────────────────────────────
IF OBJECT_ID('lesson_resources', 'U') IS NULL
BEGIN
    CREATE TABLE lesson_resources (
        id        INT PRIMARY KEY IDENTITY(1,1),
        lesson_id INT NOT NULL,
        name      NVARCHAR(200) NOT NULL,
        type      NVARCHAR(30)  NOT NULL DEFAULT N'pdf',  -- pdf | video | link
        url       NVARCHAR(500),

        CONSTRAINT FK_resources_lessons FOREIGN KEY (lesson_id)
            REFERENCES lessons(id) ON DELETE CASCADE
    );
END
GO

-- ─── 5. Таблица FAQ ───────────────────────────────────────────────────────
IF OBJECT_ID('faq', 'U') IS NULL
BEGIN
    CREATE TABLE faq (
        id           INT PRIMARY KEY IDENTITY(1,1),
        category     NVARCHAR(30)  NOT NULL,   -- course | system | technical
        question     NVARCHAR(500) NOT NULL,
        answer       NVARCHAR(MAX) NOT NULL,
        order_number INT           NOT NULL DEFAULT 1
    );
END
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- Обновление данных существующих записей
-- ═══════════════════════════════════════════════════════════════════════════

-- Категории курсов (обновить по id своих курсов)
UPDATE courses SET category = N'ИТ'          WHERE id = 1;
UPDATE courses SET category = N'Педагогика'  WHERE id = 2;
UPDATE courses SET is_new   = 1              WHERE id = 1;

-- Заголовки и категории уведомлений
UPDATE notifications SET title = N'Новый урок доступен',    category = N'Курс'         WHERE id = 1;
UPDATE notifications SET title = N'Напоминание о вебинаре', category = N'Напоминание'  WHERE id = 2;

-- ═══════════════════════════════════════════════════════════════════════════
-- Тестовые данные для новых таблиц
-- ═══════════════════════════════════════════════════════════════════════════

-- Ресурсы уроков
IF NOT EXISTS (SELECT 1 FROM lesson_resources)
BEGIN
    INSERT INTO lesson_resources (lesson_id, name, type, url) VALUES
    (1, N'Обзор LMS-платформ.pdf',        N'pdf',   N'resources/lms_overview.pdf'),
    (1, N'Видеолекция — введение',         N'video', N'https://example.com/video1'),
    (2, N'Google Workspace — руководство', N'pdf',   N'resources/google_ws.pdf'),
    (3, N'Практические задания по Zoom',   N'link',  N'https://example.com/zoom_tasks');
END
GO

-- FAQ
IF NOT EXISTS (SELECT 1 FROM faq)
BEGIN
    INSERT INTO faq (category, question, answer, order_number) VALUES
    (N'course', N'Как записаться на курс?',
     N'Перейдите в раздел «Каталог курсов», найдите интересующий вас курс и нажмите кнопку «Записаться». После этого курс появится в вашем личном кабинете в разделе «Активные курсы».', 1),
    (N'course', N'Можно ли проходить несколько курсов одновременно?',
     N'Да, вы можете записаться на несколько курсов и проходить их в удобном для вас темпе. Рекомендуем не записываться более чем на 3 курса одновременно.', 2),
    (N'course', N'Что происходит после завершения курса?',
     N'По завершении всех уроков курс переходит в статус «Завершён». Сертификат о прохождении становится доступен в вашем профиле.', 3),
    (N'course', N'Как посмотреть свой прогресс?',
     N'Перейдите в раздел «Мой прогресс» — там отображается детальная информация о прохождении каждого курса.', 4),
    (N'system', N'Как изменить данные профиля?',
     N'Изменение профиля выполняется администратором. Обратитесь к вашему администратору с запросом на изменение данных.', 1),
    (N'system', N'Что означают роли пользователей?',
     N'Система поддерживает три роли: Преподаватель — проходит курсы; Методист — создаёт курсы; Администратор — управляет системой.', 2),
    (N'system', N'Как работают уведомления?',
     N'Уведомления приходят автоматически при добавлении новых уроков, приближении дедлайнов и других событиях.', 3),
    (N'technical', N'Что делать, если Zoom-ссылка не открывается?',
     N'Убедитесь, что на вашем компьютере установлено приложение Zoom. Если ссылка не открывается, скопируйте её вручную в браузер.', 1),
    (N'technical', N'Что делать при ошибке входа в систему?',
     N'Проверьте правильность логина и пароля. Убедитесь, что Caps Lock выключен. Если проблема повторяется, обратитесь к администратору.', 2),
    (N'technical', N'Как обновить систему?',
     N'Обновления EduProgress устанавливаются администратором системы централизованно.', 3);
END
GO

PRINT N'Скрипт выполнен успешно.';
