using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoinAmy
{
    /// <summary>
    /// A felhasználó fiókot regisztrálhat az alkalmazás használatához. A regisztrációs adatoknak meg kell felelnie az elvártaknak.
    /// </summary>
    public partial class Registration : Window
    {
        DbConnect dbConnect;
        UserDataValidator userDataValidator;

        public Registration()
        {
            InitializeComponent();

            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");
            userDataValidator = new UserDataValidator();
            
            tb_errorMessage.Text = string.Empty;
        }

        private void window_Registration_Loaded(object sender, RoutedEventArgs e)
        {
            textBox_username.Focus();
        }

        private void button_registration_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox_username.Text;
            string email = textBox_email.Text;
            string password = passwordBox_password.Password;
            string rePassword = passwordBox_passwordCheck.Password;

            //Amennyiben a bevitt adatok megfelelőek, az ellenőrző függvények 'true' értékkel térnek vissza, a továbblépés engedélyezett

            try
            {
                if (IsUsernameValid(username) && IsEmailValid(email) && IsPasswordValid(password, rePassword))
                {
                    dbConnect.RegisterUser(new UserDataModel(username, email, Hash.HashPassword(password)));

                    this.Hide();
                    new Login().Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool IsUsernameValid(string username)
        {
            //A felhasználónév mező nem lehet üres
            if (userDataValidator.IsUsernameEmpty(username))
            {
                tb_errorMessage.Text = "Adjon meg felhasználónevet!";
                textBox_username.Focus();
                return false;
            }
            else if (userDataValidator.IsUsernameAtLeast(username))
            {
                tb_errorMessage.Text = "Legalább 3 karakter legyen a felhasználónév!";
                textBox_username.Focus();
                return false;
            }
            else if (userDataValidator.IsUsernameAtMost(username))
            {
                tb_errorMessage.Text = "Legfeljebb 50 karakter lehet a felhasználónév!";
                textBox_username.Focus();
                return false;
            }
            //A megadott felhasználónév létezik-e a relációban
            else if (userDataValidator.IsUsernameExist(username))
            {
                tb_errorMessage.Text = "A megadott felhasználónév már regisztrált!";
                textBox_username.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsEmailValid(string email)
        {
            //A E-mail mező nem lehet üres
            if (userDataValidator.IsEmailEmpty(email))
            {
                tb_errorMessage.Text = "Adjon meg e-mail címet!";
                textBox_email.Focus();
                return false;
            }
            //E-mail összehasonlítása reguláris kifejezés mintával
            else if (userDataValidator.IsEmailValid(email))
            {
                tb_errorMessage.Text = "Valódi e-mail címet adjon meg!";
                textBox_email.Focus();
                return false;
            }
            else if (userDataValidator.IsEmailAtMost(email))
            {
                tb_errorMessage.Text = "Legfeljebb 100 karakter lehet az e-mail cím!";
                textBox_username.Focus();
                return false;
            }
            //A megadott E-mail létezik-e a relációban
            else if (userDataValidator.IsEmailExist(email))
            {
                tb_errorMessage.Text = "A megadott e-mail cím már regisztrált!";
                textBox_email.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsPasswordValid(string password, string rePassword)
        {
            //Jelszó mező nem lehet üres
            if (userDataValidator.IsPasswordEmpty(password))
            {
                tb_errorMessage.Text = "Adjon meg jelszót!";
                passwordBox_password.Focus();
                return false;
            }
            //Jelszó összehasonlítása reguláris kifejezés mintával
            else if (userDataValidator.IsPasswordValid(password))
            {
                tb_errorMessage.Text = "A jelszónak legalább 8 karaktert, kis és nagy betűt, számot, valamint speciális karaktert kell tartalmaznia!";
                passwordBox_password.Focus();
                return false;
            }
            //Jelszó ellenőrző mező nem lehet üres
            else if (userDataValidator.IsRePasswordEmpty(rePassword))
            {
                tb_errorMessage.Text = "Erősítse meg jelszavát!";
                passwordBox_passwordCheck.Focus();
                return false;
            }
            //Jelszavak összehasonlítása
            else if (userDataValidator.IsPasswordsEquals(password, rePassword))
            {
                tb_errorMessage.Text = "A két jelszó nem egyezik meg!";
                return false;
            }
            else
            {
                return true;
            }
        }

        private void button_login_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
