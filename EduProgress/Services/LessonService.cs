using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduProgress.Services
{
    public static class LessonService
    {
        public static bool CompleteLesson(int userId, int lessonId)
        {
            if (userId > 0 && lessonId > 0)
                return true;

            return false;
        }
    }
}
