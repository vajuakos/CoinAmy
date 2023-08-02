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
    /// A felhasználó módosíthatja a fiókjához tartozó jelszót, amely az adatbázisban is frissítésre kerül, ha az új jelszó megfelel a szempontoknak.
    /// </summary>
    public partial class UserSettings : Window
    {
        DbConnect dbConnect;

        public UserSettings()
        {
            InitializeComponent();
            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");

            tb_errorMessage.Text = string.Empty;
        }

        private void button_changePassword_Click(object sender, RoutedEventArgs e)
        {
            if (IsPasswordValid())
            {
                dbConnect.ChangePassword(Hash.HashPassword(passwordBox_oldPassword.Password), Hash.HashPassword(passwordBox_newPassword.Password));

                tb_errorMessage.Text = "Sikeres jelszó változtatás!";
            }
        }

        private bool IsPasswordValid()
        {
            //A régi jelszó mező nem maradhat üres.
            if (string.IsNullOrEmpty(passwordBox_oldPassword.Password))
            {
                tb_errorMessage.Text = "Adja meg jelenlegi jelszavát!";
                passwordBox_oldPassword.Focus();
                return false;
            }
            //Az új jelszó mező nem maradhat üres.
            else if (string.IsNullOrEmpty(passwordBox_newPassword.Password))
            {
                tb_errorMessage.Text = "Adja meg új jelszavát!";
                passwordBox_newPassword.Focus();
                return false;
            }
            //Jelszó összehasonlítása reguláris kifejezés mintával
            else if (!new Regex("^(?=.*[A-ZÁÉÍÓÖŐÚÜŰ])(?=.*[a-záéíóöőúüű])(?=.*\\d)(?=.*[!@#$%^&*()_+=])[A-Za-zÁÉÍÓÖŐÚÜŰáéíóöőúüű\\d!@#$%^&*()_+=]{8,}$").IsMatch(passwordBox_newPassword.Password))
            {
                tb_errorMessage.Text = "A jelszónak legalább 8 karaktert, kis és nagy betűt, számot, valamint speciális karaktert kell tartalmaznia!";
                passwordBox_newPassword.Focus();
                return false;
            }
            //Az új jelszó megerősítésére szolgáló mező nem maradhat üres.
            else if (string.IsNullOrEmpty(passwordBox_newPasswordAgain.Password))
            {
                tb_errorMessage.Text = "Erőtsítse meg új jelszavát!";
                passwordBox_newPasswordAgain.Focus();
                return false;
            }
            //A bejelentkezéskor használt jelszónak meg kell egyeznie a jelszóváltoztatáskor megadott régi jelszóval.
            else if (dbConnect.IsPasswordMatches() != Hash.HashPassword(passwordBox_oldPassword.Password))
            {
                tb_errorMessage.Text = "A jelenlegi jelszó hibásan került megadásra!";
                passwordBox_oldPassword.Focus();
                return false;
            }
            //Új jelszóként nem állítható be a régi jelszó.
            else if (passwordBox_oldPassword.Password == passwordBox_newPassword.Password)
            {
                tb_errorMessage.Text = "A régi és új jelszó nem egyezhet meg!";
                passwordBox_newPassword.Focus();
                return false;
            }
            //A új jelszónak és a megerősítésre szolgáló mezőben szereplő jelszónak meg kell egyeznie.
            else if (passwordBox_newPassword.Password != passwordBox_newPasswordAgain.Password)
            {
                tb_errorMessage.Text = "A két jelszó nem egyezik!";
                passwordBox_oldPassword.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void window_userSettings_Loaded(object sender, RoutedEventArgs e)
        {
            passwordBox_oldPassword.Focus();
        }
    }
}
