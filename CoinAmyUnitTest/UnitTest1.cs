using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using CoinAmy;

namespace CoinAmyUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void UsernameIsEmpty_UserIsRegisters_ReturnsFalse()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string username = "TesztFerenc";

            //Act
            bool result = userDataValidator.IsUsernameEmpty(username);

            //Assert
            Assert.IsFalse(result, "A felhasználónév mező üres!");
        }

        [TestMethod]
        public void UsernameIsEmpty_UserIsRegisters_ReturnsTrue()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string username = "";

            //Act
            bool result = userDataValidator.IsUsernameEmpty(username);

            //Assert
            Assert.IsTrue(result, "A felhasználónév mező üres!");
        }

        [TestMethod]
        public void EmailIsValid_UserIsRegisters_ReturnsFalse()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string email = "proba@gmail.com";

            //Act
            bool result = userDataValidator.IsEmailValid(email);

            //Assert
            Assert.IsFalse(result, "Az e-mail cím nem valid!");
        }

        [TestMethod]
        public void EmailIsValid_UserIsRegisters_ReturnsTrue()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string email = "almaspite";

            //Act
            bool result = userDataValidator.IsEmailValid(email);

            //Assert
            Assert.IsTrue(result, "Az e-mail cím nem valid!");
        }

        [TestMethod]
        public void PasswordIsValid_UserIsRegisters_ReturnsFalse()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string password = "Rozsadomb%47_*";

            //Act
            bool result = userDataValidator.IsPasswordValid(password);

            //Assert
            Assert.IsFalse(result, "A jelszó nem valid!");
        }

        [TestMethod]
        public void PasswordIsValid_UserIsRegisters_ReturnsTrue()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string password = "kiskutya1234";

            //Act
            bool result = userDataValidator.IsPasswordValid(password);

            //Assert
            Assert.IsTrue(result, "A jelszó nem valid!");
        }

        [TestMethod]
        public void PasswordsAreEqual_UserIsRegisters_ReturnsFalse()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string password = "Citrom72Sun%";
            string rePassword = "Citrom72Sun%";

            //Act
            bool result = userDataValidator.IsPasswordsEquals(password, rePassword);

            //Assert
            Assert.IsFalse(result, "A jelszavak nem egyeznek!");
        }

        [TestMethod]
        public void PasswordsAreEqual_UserIsRegisters_ReturnsTrue()
        {
            //Arrange
            UserDataValidator userDataValidator = new UserDataValidator();

            string password = "Citrom72Sun%";
            string rePassword = "Citrom72Sun";

            //Act
            bool result = userDataValidator.IsPasswordsEquals(password, rePassword);

            //Assert
            Assert.IsTrue(result, "A jelszavak nem egyeznek!");
        }
    }
}
