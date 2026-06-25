using EduProgress.Models;
using Microsoft.Data.SqlClient;

namespace EduProgress.Services;

public class DatabaseService
{
    private SqlConnection Conn() => new SqlConnection(DbConfig.ConnectionString);

    // ════════════════════════════════════════════════════════════════════════
    // AUTH
    // ════════════════════════════════════════════════════════════════════════

    public User? Authenticate(string login, string password)
    {
        const string sql = @"
            SELECT u.id, u.full_name, u.email, u.login, u.role_id, r.name AS role_name,
                   ISNULL(u.last_login, u.created_at) AS last_login,
                   ISNULL(u.is_active, 1) AS is_active
            FROM   users u
            JOIN   roles r ON u.role_id = r.id
            WHERE  u.login = @login AND u.password = @pwd";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@pwd",   password);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            var user = MapUser(r);
            r.Close();
            UpdateLastLogin(conn, user.Id);
            return user;
        }
        catch (Exception ex) { ShowDbError(ex); return null; }
    }

    private static void UpdateLastLogin(SqlConnection conn, int userId)
    {
        using var cmd = new SqlCommand(
            "UPDATE users SET last_login = GETDATE() WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", userId);
        cmd.ExecuteNonQuery();
    }

    // ════════════════════════════════════════════════════════════════════════
    // COURSES
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>Все курсы с данными записи и прогресса для конкретного пользователя.</summary>
    public List<Course> GetCoursesForUser(int userId)
    {
        const string sql = @"
            SELECT
                c.id, c.title, c.description, c.total_hours,
                ISNULL(c.category, N'Общий')    AS category,
                ISNULL(c.is_new, 0)             AS is_new,
                ISNULL(e.status, N'')           AS enroll_status,
                ISNULL(cs.total_enrolled, 0)    AS total_enrolled,
                (SELECT COUNT(*)
                 FROM   lessons l2 JOIN modules m2 ON l2.module_id = m2.id
                 WHERE  m2.course_id = c.id)    AS total_lessons,
                (SELECT COUNT(*)
                 FROM   lesson_progress lp2
                 JOIN   lessons l2 ON lp2.lesson_id = l2.id
                 JOIN   modules m2 ON l2.module_id  = m2.id
                 WHERE  m2.course_id = c.id AND lp2.user_id = @uid AND lp2.completed = 1)
                                                AS done_lessons
            FROM  courses c
            LEFT JOIN enrollments e   ON c.id = e.course_id AND e.user_id = @uid
            LEFT JOIN course_statistics cs ON c.id = cs.course_id
            ORDER BY c.id";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            var list = new List<Course>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapCourse(r));
            return list;
        }
        catch (Exception ex) { ShowDbError(ex); return new List<Course>(); }
    }

    /// <summary>Полные данные одного курса с модулями, уроками и прогрессом.</summary>
    public Course? GetCourse(int courseId, int userId)
    {
        const string sqlCourse = @"
            SELECT
                c.id, c.title, c.description, c.total_hours,
                ISNULL(c.category, N'Общий') AS category,
                ISNULL(c.is_new, 0)          AS is_new,
                ISNULL(e.status, N'')        AS enroll_status,
                ISNULL(cs.total_enrolled, 0) AS total_enrolled,
                (SELECT COUNT(*) FROM lessons l2 JOIN modules m2 ON l2.module_id=m2.id WHERE m2.course_id=c.id) AS total_lessons,
                (SELECT COUNT(*) FROM lesson_progress lp2 JOIN lessons l2 ON lp2.lesson_id=l2.id
                 JOIN modules m2 ON l2.module_id=m2.id
                 WHERE m2.course_id=c.id AND lp2.user_id=@uid AND lp2.completed=1) AS done_lessons
            FROM  courses c
            LEFT JOIN enrollments e ON c.id=e.course_id AND e.user_id=@uid
            LEFT JOIN course_statistics cs ON c.id=cs.course_id
            WHERE c.id = @cid";

        const string sqlModLes = @"
            SELECT
                m.id   AS mid,   m.title AS mtitle, m.order_number AS morder,
                l.id   AS lid,   l.title AS ltitle, l.content,
                l.duration_minutes, l.zoom_link, l.order_number AS lorder,
                CASE
                    WHEN lp.completed = 1           THEN 2
                    WHEN lp.id IS NOT NULL          THEN 1
                    ELSE 0
                END AS lstatus
            FROM modules m
            JOIN lessons l ON m.id = l.module_id
            LEFT JOIN lesson_progress lp ON l.id = lp.lesson_id AND lp.user_id = @uid
            WHERE m.course_id = @cid
            ORDER BY m.order_number, l.order_number";

        const string sqlRes = @"
            SELECT lr.id, lr.lesson_id, lr.name, lr.type, lr.url
            FROM   lesson_resources lr
            JOIN   lessons l  ON lr.lesson_id = l.id
            JOIN   modules m  ON l.module_id  = m.id
            WHERE  m.course_id = @cid";
        try
        {
            using var conn = Conn(); conn.Open();

            // Course
            using var cmd1 = new SqlCommand(sqlCourse, conn);
            cmd1.Parameters.AddWithValue("@cid", courseId);
            cmd1.Parameters.AddWithValue("@uid", userId);
            using var r1 = cmd1.ExecuteReader();
            if (!r1.Read()) return null;
            var course = MapCourse(r1);
            r1.Close();

            // Modules & Lessons
            var moduleDict = new Dictionary<int, Module>();
            var lessonDict = new Dictionary<int, Lesson>();
            using var cmd2 = new SqlCommand(sqlModLes, conn);
            cmd2.Parameters.AddWithValue("@cid", courseId);
            cmd2.Parameters.AddWithValue("@uid", userId);
            using var r2 = cmd2.ExecuteReader();
            while (r2.Read())
            {
                int mid = r2.GetInt32(r2.GetOrdinal("mid"));
                if (!moduleDict.TryGetValue(mid, out var mod))
                {
                    mod = new Module
                    {
                        Id       = mid,
                        CourseId = courseId,
                        Title    = r2.GetString(r2.GetOrdinal("mtitle")),
                        Order    = r2.GetInt32(r2.GetOrdinal("morder")),
                    };
                    moduleDict[mid] = mod;
                }
                var lesson = new Lesson
                {
                    Id              = r2.GetInt32(r2.GetOrdinal("lid")),
                    ModuleId        = mid,
                    Title           = r2.GetString(r2.GetOrdinal("ltitle")),
                    Content         = r2.IsDBNull(r2.GetOrdinal("content"))  ? "" : r2.GetString(r2.GetOrdinal("content")),
                    DurationMinutes = r2.IsDBNull(r2.GetOrdinal("duration_minutes")) ? 45 : r2.GetInt32(r2.GetOrdinal("duration_minutes")),
                    ZoomLink        = r2.IsDBNull(r2.GetOrdinal("zoom_link")) ? null : r2.GetString(r2.GetOrdinal("zoom_link")),
                    Order           = r2.GetInt32(r2.GetOrdinal("lorder")),
                    Status          = (LessonStatus)r2.GetInt32(r2.GetOrdinal("lstatus")),
                };
                mod.Lessons.Add(lesson);
                lessonDict[lesson.Id] = lesson;
            }
            r2.Close();
            course.Modules.AddRange(moduleDict.Values.OrderBy(m => m.Order));

            // Resources
            using var cmd3 = new SqlCommand(sqlRes, conn);
            cmd3.Parameters.AddWithValue("@cid", courseId);
            using var r3 = cmd3.ExecuteReader();
            while (r3.Read())
            {
                int lid = r3.GetInt32(r3.GetOrdinal("lesson_id"));
                if (lessonDict.TryGetValue(lid, out var les))
                    les.Resources.Add(new LessonResource
                    {
                        Name = r3.GetString(r3.GetOrdinal("name")),
                        Type = r3.GetString(r3.GetOrdinal("type")),
                        Url  = r3.IsDBNull(r3.GetOrdinal("url")) ? "" : r3.GetString(r3.GetOrdinal("url")),
                    });
            }
            return course;
        }
        catch (Exception ex) { ShowDbError(ex); return null; }
    }

    public void EnrollUser(int userId, int courseId)
    {
        const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM enrollments WHERE user_id=@uid AND course_id=@cid)
                INSERT INTO enrollments (user_id, course_id, status) VALUES (@uid, @cid, N'В процессе')";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@cid", courseId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    // ════════════════════════════════════════════════════════════════════════
    // LESSON PROGRESS
    // ════════════════════════════════════════════════════════════════════════

    public void CompleteLesson(int userId, int lessonId)
    {
        const string sql = @"
            MERGE lesson_progress AS target
            USING (SELECT @uid AS user_id, @lid AS lesson_id) AS source
            ON target.user_id = source.user_id AND target.lesson_id = source.lesson_id
            WHEN MATCHED THEN
                UPDATE SET completed = 1, completion_date = GETDATE()
            WHEN NOT MATCHED THEN
                INSERT (user_id, lesson_id, completed, completion_date, time_spent_minutes)
                VALUES (@uid, @lid, 1, GETDATE(), 0);

            -- Обновить статус записи
            UPDATE e SET e.last_accessed_lesson_id = @lid
            FROM enrollments e
            JOIN lessons  l  ON l.id = @lid
            JOIN modules  m  ON m.id = l.module_id
            WHERE e.user_id = @uid AND e.course_id = m.course_id;

            -- Автоматически завершить курс если все уроки пройдены
            UPDATE e SET e.status = N'Завершён'
            FROM enrollments e
            JOIN modules m ON m.course_id = e.course_id
            JOIN lessons l ON l.module_id = m.id
            WHERE e.user_id = @uid
              AND e.status <> N'Завершён'
              AND NOT EXISTS (
                    SELECT 1 FROM lessons l2
                    JOIN modules m2 ON l2.module_id = m2.id
                    WHERE m2.course_id = e.course_id
                      AND NOT EXISTS (
                            SELECT 1 FROM lesson_progress lp
                            WHERE lp.lesson_id = l2.id AND lp.user_id = @uid AND lp.completed = 1));";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@lid", lessonId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    // ════════════════════════════════════════════════════════════════════════
    // NOTIFICATIONS
    // ════════════════════════════════════════════════════════════════════════

    public List<Notification> GetNotificationsForUser(int userId)
    {
        const string sql = @"
            SELECT id, ISNULL(title, N'Уведомление') AS title, message,
                   ISNULL(category, N'Курс') AS category,
                   created_at, ISNULL(is_read, 0) AS is_read
            FROM   notifications
            WHERE  user_id = @uid
            ORDER  BY created_at DESC";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            var list = new List<Notification>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Notification
                {
                    Id        = r.GetInt32(0),
                    Title     = r.GetString(1),
                    Message   = r.GetString(2),
                    Category  = r.GetString(3) switch
                    {
                        "Напоминание" => NotificationCategory.Reminder,
                        "Система"     => NotificationCategory.System,
                        _             => NotificationCategory.Course,
                    },
                    CreatedAt = r.GetDateTime(4),
                    IsRead    = r.GetBoolean(5),
                });
            return list;
        }
        catch (Exception ex) { ShowDbError(ex); return new List<Notification>(); }
    }

    public void MarkNotificationRead(int notifId)
    {
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(
                "UPDATE notifications SET is_read = 1 WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", notifId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    public void MarkAllNotificationsRead(int userId)
    {
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(
                "UPDATE notifications SET is_read = 1 WHERE user_id = @uid", conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    // ════════════════════════════════════════════════════════════════════════
    // FAQ
    // ════════════════════════════════════════════════════════════════════════

    public List<FaqItem> GetFaq()
    {
        const string sql = @"
            SELECT id, category, question, answer, order_number
            FROM   faq
            ORDER  BY category, order_number";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            var list = new List<FaqItem>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new FaqItem
                {
                    Id          = r.GetInt32(0),
                    Category    = r.GetString(1),
                    Question    = r.GetString(2),
                    Answer      = r.GetString(3),
                    OrderNumber = r.GetInt32(4),
                });
            return list;
        }
        catch (Exception ex) { ShowDbError(ex); return new List<FaqItem>(); }
    }

    // ════════════════════════════════════════════════════════════════════════
    // USERS  (Admin)
    // ════════════════════════════════════════════════════════════════════════

    public List<User> GetAllUsers()
    {
        const string sql = @"
            SELECT u.id, u.full_name, u.email, u.login, u.role_id, r.name AS role_name,
                   ISNULL(u.last_login, u.created_at) AS last_login,
                   ISNULL(u.is_active, 1) AS is_active
            FROM   users u JOIN roles r ON u.role_id = r.id
            ORDER  BY u.id";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            var list = new List<User>();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapUser(r));
            return list;
        }
        catch (Exception ex) { ShowDbError(ex); return new List<User>(); }
    }

    public void UpdateUser(User u)
    {
        const string sql = @"
            UPDATE users
            SET full_name = @name, email = @email,
                role_id   = (SELECT id FROM roles WHERE name = @role),
                is_active = @active
            WHERE id = @id";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id",     u.Id);
            cmd.Parameters.AddWithValue("@name",   u.Name);
            cmd.Parameters.AddWithValue("@email",  u.Email);
            cmd.Parameters.AddWithValue("@role",   u.RoleLabel);
            cmd.Parameters.AddWithValue("@active", u.IsActive);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    // ════════════════════════════════════════════════════════════════════════
    // REPORTS
    // ════════════════════════════════════════════════════════════════════════

    public List<(string Title, int Enrolled, int Completed, double AvgProgress)> GetReportData()
    {
        const string sql = @"
            SELECT c.title,
                   ISNULL(cs.total_enrolled,  0)    AS enrolled,
                   ISNULL(cs.total_completed, 0)    AS completed,
                   ISNULL(cs.average_progress_percent, 0) AS avg_progress
            FROM   courses c
            LEFT JOIN course_statistics cs ON c.id = cs.course_id
            ORDER  BY c.id";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            var list = new List<(string, int, int, double)>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add((r.GetString(0), r.GetInt32(1), r.GetInt32(2),
                          (double)r.GetDecimal(3)));
            return list;
        }
        catch (Exception ex) { ShowDbError(ex); return new List<(string, int, int, double)>(); }
    }

    // ════════════════════════════════════════════════════════════════════════
    // COURSE EDITOR  (Methodist)
    // ════════════════════════════════════════════════════════════════════════

    public List<Course> GetAllCoursesWithStructure()
    {
        const string sqlCourses = @"
            SELECT c.id, c.title, c.description, c.total_hours,
                   ISNULL(c.category,N'Общий') AS category,
                   ISNULL(c.is_new,0) AS is_new,
                   N'' AS enroll_status, 0 AS total_enrolled,
                   0 AS total_lessons, 0 AS done_lessons
            FROM courses c ORDER BY c.id";

        const string sqlMod = @"
            SELECT m.id, m.course_id, m.title, m.order_number
            FROM   modules m ORDER BY m.course_id, m.order_number";

        const string sqlLes = @"
            SELECT l.id, l.module_id, l.title, l.content,
                   ISNULL(l.duration_minutes,45) AS duration_minutes,
                   l.zoom_link, l.order_number
            FROM   lessons l ORDER BY l.module_id, l.order_number";
        try
        {
            using var conn = Conn(); conn.Open();
            var courses = new Dictionary<int, Course>();
            using (var cmd = new SqlCommand(sqlCourses, conn))
            using (var r   = cmd.ExecuteReader())
                while (r.Read()) { var c = MapCourse(r); courses[c.Id] = c; }

            var modules = new Dictionary<int, Module>();
            using (var cmd = new SqlCommand(sqlMod, conn))
            using (var r   = cmd.ExecuteReader())
                while (r.Read())
                {
                    var m = new Module
                    {
                        Id       = r.GetInt32(0),
                        CourseId = r.GetInt32(1),
                        Title    = r.GetString(2),
                        Order    = r.GetInt32(3),
                    };
                    modules[m.Id] = m;
                    if (courses.TryGetValue(m.CourseId, out var c)) c.Modules.Add(m);
                }

            using (var cmd = new SqlCommand(sqlLes, conn))
            using (var r   = cmd.ExecuteReader())
                while (r.Read())
                {
                    var l = new Lesson
                    {
                        Id              = r.GetInt32(0),
                        ModuleId        = r.GetInt32(1),
                        Title           = r.GetString(2),
                        Content         = r.IsDBNull(3) ? "" : r.GetString(3),
                        DurationMinutes = r.GetInt32(4),
                        ZoomLink        = r.IsDBNull(5) ? null : r.GetString(5),
                        Order           = r.GetInt32(6),
                    };
                    if (modules.TryGetValue(l.ModuleId, out var m)) m.Lessons.Add(l);
                }

            return courses.Values.ToList();
        }
        catch (Exception ex) { ShowDbError(ex); return new List<Course>(); }
    }

    public int CreateCourse(string title, string description, int hours, string category, int createdBy)
    {
        const string sql = @"
            INSERT INTO courses (title, description, total_hours, created_by, status, category, is_new)
            OUTPUT INSERTED.id
            VALUES (@title, @desc, @hours, @by, N'активен', @cat, 1)";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@desc",  description);
            cmd.Parameters.AddWithValue("@hours", hours);
            cmd.Parameters.AddWithValue("@by",    createdBy);
            cmd.Parameters.AddWithValue("@cat",   category);
            return (int)cmd.ExecuteScalar()!;
        }
        catch (Exception ex) { ShowDbError(ex); return -1; }
    }

    public void UpdateCourse(int id, string title, string description, int hours, string category)
    {
        const string sql = @"UPDATE courses SET title=@t, description=@d, total_hours=@h, category=@c WHERE id=@id";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@t",  title);
            cmd.Parameters.AddWithValue("@d",  description);
            cmd.Parameters.AddWithValue("@h",  hours);
            cmd.Parameters.AddWithValue("@c",  category);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    public void DeleteCourse(int id)
    {
        Execute("DELETE FROM courses WHERE id = @p", ("@p", id));
    }

    public int CreateModule(int courseId, string title, int order)
    {
        const string sql = @"
            INSERT INTO modules (course_id, title, order_number)
            OUTPUT INSERTED.id VALUES (@cid, @t, @o)";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cid", courseId);
            cmd.Parameters.AddWithValue("@t",   title);
            cmd.Parameters.AddWithValue("@o",   order);
            return (int)cmd.ExecuteScalar()!;
        }
        catch (Exception ex) { ShowDbError(ex); return -1; }
    }

    public void UpdateModule(int id, string title, int order)
    {
        const string sql = "UPDATE modules SET title=@t, order_number=@o WHERE id=@id";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@t",  title);
            cmd.Parameters.AddWithValue("@o",  order);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    public void DeleteModule(int id) => Execute("DELETE FROM modules WHERE id = @p", ("@p", id));

    public int CreateLesson(int moduleId, string title, int durationMin, string? zoomLink, int order)
    {
        const string sql = @"
            INSERT INTO lessons (module_id, title, content, duration_minutes, zoom_link, order_number)
            OUTPUT INSERTED.id VALUES (@mid, @t, N'', @dur, @zoom, @o)";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@mid",  moduleId);
            cmd.Parameters.AddWithValue("@t",    title);
            cmd.Parameters.AddWithValue("@dur",  durationMin);
            cmd.Parameters.AddWithValue("@zoom", (object?)zoomLink ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@o",    order);
            return (int)cmd.ExecuteScalar()!;
        }
        catch (Exception ex) { ShowDbError(ex); return -1; }
    }

    public void UpdateLesson(Lesson l)
    {
        const string sql = @"UPDATE lessons SET title=@t, content=@c, duration_minutes=@d,
                              zoom_link=@z, order_number=@o WHERE id=@id";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", l.Id);
            cmd.Parameters.AddWithValue("@t",  l.Title);
            cmd.Parameters.AddWithValue("@c",  l.Content);
            cmd.Parameters.AddWithValue("@d",  l.DurationMinutes);
            cmd.Parameters.AddWithValue("@z",  (object?)l.ZoomLink ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@o",  l.Order);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    public void DeleteLesson(int id) => Execute("DELETE FROM lessons WHERE id = @p", ("@p", id));

    public void AddLessonResource(int lessonId, string name, string type, string url)
    {
        const string sql = @"INSERT INTO lesson_resources (lesson_id, name, type, url)
                              VALUES (@lid, @name, @type, @url)";
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@lid",  lessonId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@url",  url);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    // ════════════════════════════════════════════════════════════════════════
    // PRIVATE HELPERS
    // ════════════════════════════════════════════════════════════════════════

    private static User MapUser(SqlDataReader r) => new User
    {
        Id         = r.GetInt32(0),
        Name       = r.GetString(1),
        Email      = r.GetString(2),
        Login      = r.GetString(3),
        Role       = r.GetString(5) switch
        {
            var s when s.Contains("Администратор") => UserRole.Administrator,
            var s when s.Contains("Методист")      => UserRole.Methodist,
            _                                       => UserRole.Teacher,
        },
        LastActive = r.IsDBNull(6) ? DateTime.Now : r.GetDateTime(6),
        IsActive   = !r.IsDBNull(7) && r.GetBoolean(7),
    };

    private static Course MapCourse(SqlDataReader r)
    {
        int total = r.IsDBNull(8) ? 0 : r.GetInt32(8);
        int done  = r.IsDBNull(9) ? 0 : r.GetInt32(9);
        int prog  = total > 0 ? (int)(done * 100.0 / total) : 0;

        string enrollStatus = r.GetString(6);
        CourseStatus status = enrollStatus switch
        {
            "Завершён" or "Завершен" => CourseStatus.Completed,
            var s when s.Length > 0  => CourseStatus.Enrolled,
            _                         => CourseStatus.Available,
        };

        return new Course
        {
            Id            = r.GetInt32(0),
            Title         = r.GetString(1),
            Description   = r.IsDBNull(2) ? "" : r.GetString(2),
            TotalHours    = r.IsDBNull(3) ? 0  : r.GetInt32(3),
            Category      = r.GetString(4),
            IsNew         = !r.IsDBNull(5) && r.GetBoolean(5),
            Status        = status,
            Progress      = prog,
            EnrolledCount = r.GetInt32(7),
        };
    }

    private void Execute(string sql, params (string name, object value)[] pars)
    {
        try
        {
            using var conn = Conn(); conn.Open();
            using var cmd  = new SqlCommand(sql, conn);
            foreach (var (n, v) in pars) cmd.Parameters.AddWithValue(n, v);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { ShowDbError(ex); }
    }

    private static void ShowDbError(Exception ex)
    {
        MessageBox.Show(
            $"Ошибка базы данных:\n{ex.Message}\n\n" +
            "Проверьте строку подключения в Services/DbConfig.cs.",
            "EduProgress — Ошибка БД",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
