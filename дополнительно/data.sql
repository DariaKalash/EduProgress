INSERT INTO roles (name) VALUES
(N'Администратор'),
(N'Методист'),
(N'Преподаватель');

INSERT INTO users (full_name, email, login, password, role_id)
VALUES
(N'Администратор системы', 'admin@edu.ru', 'admin', '1234', 1),
(N'Петров Сергей Викторович', 'petrov@edu.ru', 'petrov', '1234', 2),
(N'Иванова Анна Ивановна', 'ivanova@edu.ru', 'ivanova', '1234', 3);

INSERT INTO courses (title, description, total_hours, created_by, status, certificate_template)
VALUES
(
    N'Цифровые технологии в образовании',
    N'Изучение современных цифровых инструментов для образовательного процесса',
    36,
    2,
    N'Активный',
    N'certificates/digital_tech_template.pdf'
),
(
    N'Педагогика высшей школы',
    N'Основы педагогического мастерства и методики преподавания',
    24,
    2,
    N'Активный',
    N'certificates/pedagogy_template.pdf'
);

INSERT INTO modules (course_id, title, order_number)
VALUES
(1, N'Введение в цифровые инструменты', 1),
(1, N'Интерактивные технологии', 2),
(2, N'Основы педагогики', 1);

INSERT INTO lessons (module_id, title, content, duration_minutes, zoom_link, order_number)
VALUES
(1, N'Обзор платформ LMS', N'Описание LMS систем', 45, NULL, 1),
(1, N'Google Workspace', N'Работа с Google Docs, Sheets', 60, NULL, 2),
(2, N'Вебинар по Zoom', N'Практическое занятие', 90, N'https://zoom.us/j/123456789', 1),
(3, N'Введение в педагогику', N'Основные принципы обучения', 50, NULL, 1);

INSERT INTO enrollments (user_id, course_id, status)
VALUES
(3, 1, N'В процессе'),
(3, 2, N'Записан');

INSERT INTO lesson_progress (user_id, lesson_id, completed, completion_date, time_spent_minutes)
VALUES
(3, 1, 1, GETDATE(), 40),
(3, 2, 0, NULL, 10);

INSERT INTO notifications (user_id, course_id, lesson_id, message)
VALUES
(3, 1, NULL, N'Новый урок добавлен в курс'),
(3, 1, 3, N'Напоминание о вебинаре');

INSERT INTO course_statistics (course_id, total_enrolled, total_completed, average_progress_percent, updated_at)
VALUES
(1, 10, 5, 60.5, GETDATE()),
(2, 8, 3, 45.0, GETDATE());