using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CoinAmy
{
    class RegistrationViewModel : INotifyPropertyChanged
    {
        private string username;
        private string email;
        private string password;

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        //INotifyPropertyChanged
        //Minden alkalommal értesít, amikor az adott Property-n változást észlel
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //A következő osztályok a ValidationRule ősosztályból öröklődve összehasonlítják, hogy az értékek megfelelnek-e a meghatározott feltételeknek
    class UsernameValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string username = value as string;

            if (string.IsNullOrWhiteSpace(username))
            {
                return new ValidationResult(false, "A felhasználónév nem lehet üres!");
            }

            if (username.Length < 3)
            {
                return new ValidationResult(false, "A felhasználónévnek legalább 3 karakter hosszúnak kell lennie!");
            }

            if (username.Length > 50)
            {
                return new ValidationResult(false, "A felhasználónév legfeljebb 50 karakter hosszú lehet!");
            }

            return new ValidationResult(true, null);
        }
    }

    class EmailValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string email = value as string;

            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult(false, "A e-mail cím nem lehet üres!");
            }

            if (!new Regex("^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$").IsMatch(email))
            {
                return new ValidationResult(false, "Nem valódi e-mail cím!");
            }

            if (email.Length > 100)
            {
                return new ValidationResult(false, "Az e-mail cím legfeljebb 100 karakter hosszú lehet!");
            }

            return new ValidationResult(true, null);
        }
    }

    public class UserDataValidator
    {
        DbConnect dbConnect;

        public UserDataValidator()
        {
            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");
        }

        #region Register

        //*Username*
        public bool IsUsernameEmpty(string username)
        {
            //A felhasználónév mező nem lehet üres
            if (string.IsNullOrWhiteSpace(username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUsernameAtLeast(string username)
        {
            if (username.Length < 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUsernameAtMost(string username)
        {
            if (username.Length > 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUsernameExist(string username)
        {
            //A megadott felhasználónév létezik-e a relációban
            if (dbConnect.IsUsernameExist(username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //*E-mail*
        public bool IsEmailEmpty(string email)
        {
            //A E-mail mező nem lehet üres
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsEmailValid(string email)
        {
            //E-mail összehasonlítása reguláris kifejezés mintával
            if (!new Regex("^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$").IsMatch(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsEmailAtMost(string email)
        {
            if (email.Length > 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsEmailExist(string email)
        {
            //A megadott E-mail létezik-e a relációban
            if (dbConnect.IsEmailExist(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //*Password*
        public bool IsPasswordEmpty(string password)
        {
            //Jelszó mező nem lehet üres
            if (string.IsNullOrWhiteSpace(password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPasswordValid(string password)
        {
            //Jelszó összehasonlítása reguláris kifejezés mintával
            if (!new Regex("^(?=.*[A-ZÁÉÍÓÖŐÚÜŰ])(?=.*[a-záéíóöőúüű])(?=.*\\d)(?=.*[!@#$%^&*()_+=])[A-Za-zÁÉÍÓÖŐÚÜŰáéíóöőúüű\\d!@#$%^&*()_+=]{8,}$").IsMatch(password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsRePasswordEmpty(string rePassword)
        {
            //Jelszó ellenőrző mező nem lehet üres
            if (string.IsNullOrEmpty(rePassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPasswordsEquals(string password, string rePassword)
        {
            //Jelszavak összehasonlítása
            if (password != rePassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Login

        public bool IsUsernameOrEmailEmpty(string usernameOrEmail)
        {
            //Amennyiben a felhasználónév mező kitöltetlen vagy white space-t tartlmaz, hamis értékkel tér vissza
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsUsernameOrEmailExist(string usernameOrEmail)
        {
            //Amennyiben a felhasználónév vagy e-mail cím nem szerepel a users relációban, hamis értékkel tér vissza
            if (!dbConnect.IsUsernameExist(usernameOrEmail) && !dbConnect.IsEmailExist(usernameOrEmail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LoginUser(string usernameOrEmail, string password)
        {
            //Amennyiben a belépési adatok nem találhatóak a users relációban, hamis értékkel tér vissza
            if (!dbConnect.LoginUser(new UserDataModel(usernameOrEmail, usernameOrEmail, Hash.HashPassword(password))))
            {
                return true;
            }
            //Minen más esetben igaz értéket ad
            else
            {
                return false;
            }
        }

        #endregion
    }
}
