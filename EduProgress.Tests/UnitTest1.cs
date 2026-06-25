using Microsoft.VisualStudio.TestTools.UnitTesting;
using EduProgress.Services;

namespace EduProgress.Tests
{
    [TestClass]
    public class AuthTests
    {
        [TestMethod]
        public void Login_ValidData_ReturnsTrue()
        {
            // Arrange
            string login = "teacher1";
            string password = "12345";

            // Act
            bool result = AuthService.Login(login, password);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Login_InvalidPassword_ReturnsFalse()
        {
            string login = "teacher1";
            string password = "wrong";

            bool result = AuthService.Login(login, password);

            Assert.IsFalse(result);
        }
    }
    [TestClass]
    public class LessonTests
    {
        [TestMethod]
        public void CompleteLesson_ValidData_ProgressUpdated()
        {
            int userId = 1;
            int lessonId = 10;

            bool result = LessonService.CompleteLesson(userId, lessonId);

            Assert.IsTrue(result);
        }
    }
}