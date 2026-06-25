CREATE DATABASE EduProgress;
GO

USE EduProgress;
GO

CREATE TABLE roles (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    full_name NVARCHAR(150) NOT NULL,
    email NVARCHAR(120) NOT NULL UNIQUE,
    login NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    role_id INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    last_login DATETIME,

    CONSTRAINT FK_users_roles FOREIGN KEY (role_id)
        REFERENCES roles(id)
);

CREATE TABLE courses (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(200) NOT NULL,
    description NVARCHAR(MAX),
    total_hours INT,
    created_by INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    status NVARCHAR(30),

    -- ШАБЛОН СЕРТИФИКАТА (PDF файл)
    -- В текущей версии хранится только шаблон.
    -- В перспективе возможно добавление генерации именных сертификатов.
    certificate_template NVARCHAR(255),

    CONSTRAINT FK_courses_users FOREIGN KEY (created_by)
        REFERENCES users(id)
);

CREATE TABLE modules (
    id INT PRIMARY KEY IDENTITY(1,1),
    course_id INT NOT NULL,
    title NVARCHAR(200) NOT NULL,
    order_number INT NOT NULL,

    CONSTRAINT FK_modules_courses FOREIGN KEY (course_id)
        REFERENCES courses(id)
        ON DELETE CASCADE
);

CREATE TABLE lessons (
    id INT PRIMARY KEY IDENTITY(1,1),
    module_id INT NOT NULL,
    title NVARCHAR(200) NOT NULL,
    content NVARCHAR(MAX),
    duration_minutes INT,
    zoom_link NVARCHAR(255),
    order_number INT NOT NULL,

    CONSTRAINT FK_lessons_modules FOREIGN KEY (module_id)
        REFERENCES modules(id)
        ON DELETE CASCADE
);

CREATE TABLE enrollments (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    course_id INT NOT NULL,
    enrollment_date DATETIME DEFAULT GETDATE(),
    status NVARCHAR(30),
    last_accessed_lesson_id INT,

    CONSTRAINT FK_enrollments_users FOREIGN KEY (user_id)
        REFERENCES users(id),

    CONSTRAINT FK_enrollments_courses FOREIGN KEY (course_id)
        REFERENCES courses(id),

    CONSTRAINT FK_enrollments_lessons FOREIGN KEY (last_accessed_lesson_id)
        REFERENCES lessons(id)
);

CREATE TABLE lesson_progress (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    lesson_id INT NOT NULL,
    completed BIT DEFAULT 0,
    completion_date DATETIME,
    time_spent_minutes INT,

    CONSTRAINT FK_progress_users FOREIGN KEY (user_id)
        REFERENCES users(id),

    CONSTRAINT FK_progress_lessons FOREIGN KEY (lesson_id)
        REFERENCES lessons(id),

    CONSTRAINT UQ_user_lesson UNIQUE (user_id, lesson_id)
);

CREATE TABLE notifications (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    course_id INT NULL,
    lesson_id INT NULL,
    message NVARCHAR(MAX) NOT NULL,
    is_read BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_notifications_users FOREIGN KEY (user_id)
        REFERENCES users(id),

    CONSTRAINT FK_notifications_courses FOREIGN KEY (course_id)
        REFERENCES courses(id),

    CONSTRAINT FK_notifications_lessons FOREIGN KEY (lesson_id)
        REFERENCES lessons(id)
);

CREATE TABLE course_statistics (
    id INT PRIMARY KEY IDENTITY(1,1),
    course_id INT NOT NULL,
    total_enrolled INT,
    total_completed INT,
    average_progress_percent DECIMAL(5,2),
    updated_at DATETIME,

    CONSTRAINT FK_statistics_courses FOREIGN KEY (course_id)
        REFERENCES courses(id)
);
