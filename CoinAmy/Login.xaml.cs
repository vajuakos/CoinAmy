using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// A felhasználó megadhatja a bejelentkezéshez szükséges adatait, majd azok megvizsgálásra és ellenőrzésre kerülnek az adatbázisban.
    /// </summary>
    public partial class Login : Window
    {
        private DbConnect dbConnect;
        private UserDataValidator userDataValidator;
        public static string UserID { get; set; }

        public Login()
        {
            InitializeComponent();
            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");
            userDataValidator = new UserDataValidator();

            tb_errorMessage.Text = string.Empty;
        }

        private void window_Login_Loaded(object sender, RoutedEventArgs e)
        {
            textBox_username_email.Focus();
        }

        private void button_login_Click(object sender, RoutedEventArgs e)
        {
            string usernameOrEmail = textBox_username_email.Text;
            string password = passwordBox_password.Password;

            //Ha a beviteli mezők értékben minden érték megfelelő és az adatbázisban szerepel egy darab, a felhasználó által bevitt
            //értéknek megfelelő, a belépés sikeres. Nulla érték esetében sikertelen.

            try
            {
                if (IsUsernameOrEmailValid(usernameOrEmail) && IsPasswordValid(password) && LoginUser(usernameOrEmail, password) && dbConnect.LoginUser(new UserDataModel(usernameOrEmail, usernameOrEmail, Hash.HashPassword(password))))
                {
                    UserID = dbConnect.GetUserId(textBox_username_email.Text); //Visszadja a felhasználónévhez/e-mail címhez tartózó user_ID-t

                    new MainWindow().Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsUsernameOrEmailValid(string usernameOrEmail)
        {
            //Amennyiben a felhasználónév mező kitöltetlen vagy white space-t tartlmaz, hamis értékkel tér vissza
            if (userDataValidator.IsUsernameOrEmailEmpty(usernameOrEmail))
            {
                tb_errorMessage.Text = "Adja meg a felhasználónevét / e-mail címét!";
                textBox_username_email.Focus();
                return false;
            }
            //Amennyiben a felhasználónév vagy e-mail cím nem szerepel a users relációban, hamis értékkel tér vissza
            else if (userDataValidator.IsUsernameOrEmailExist(usernameOrEmail))
            {
                tb_errorMessage.Text = "A megadott felhasználónév / e-mail nem regisztrált!";
                textBox_username_email.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsPasswordValid(string password)
        {
            //Amennyiben a jelszó mező kitöltetlen vagy white space-t tartlmaz, hamis értékkel tér vissza
            if (userDataValidator.IsPasswordEmpty(password))
            {
                tb_errorMessage.Text = "Adja meg jelszavát!";
                passwordBox_password.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool LoginUser(string usernameOrEmail, string password)
        {
            //Amennyiben a belépési adatok nem találhatóak a users relációban, hamis értékkel tér vissza
            if (userDataValidator.LoginUser(usernameOrEmail, password))
            {
                tb_errorMessage.Text = "Hibás belépési felhasználónév/e-mail cím vagy jelszó!";
                textBox_username_email.Focus();
                return false;
            }
            //Minen más esetben igaz értéket ad
            else
            {
                return true;
            }
        }

        private void button_registration_Click(object sender, RoutedEventArgs e)
        {
            new Registration().Show();
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
