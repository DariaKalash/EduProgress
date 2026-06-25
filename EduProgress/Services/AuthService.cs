using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduProgress.Services
{
    public static class AuthService
    {
        public static bool Login(string login, string password)
        {
            if (login == "teacher1" && password == "12345")
                return true;

            return false;
        }
    }
}
