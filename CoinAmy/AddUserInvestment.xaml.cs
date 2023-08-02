using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// Az adatbázis 'investments' táblájába történő adatrögzítésre van lehetőség. Rögzítés esetén a szülő ablak egy esemény segítségével frissül.
    /// </summary>
    public partial class AddUserInvestment : Window
    {
        DbConnect dbConnect;

        public AddUserInvestment(UserInvestments userInvestments)
        {
            InitializeComponent();
            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");
            
            tb_errorMessage.Text = string.Empty;
        }

        public delegate void UpdateDelegate(object sender, UpdateEventArgs args);
        public event UpdateDelegate UpdateEventHandler;

        public class UpdateEventArgs : EventArgs
        {
            //Data
        }

        protected void Insert()
        {
            UpdateEventArgs args = new UpdateEventArgs();
            UpdateEventHandler.Invoke(this, args);
        }

        private void button_addInvestment_Click(object sender, RoutedEventArgs e)
        {
            if (IsDataValid()) //Amennyiben a megadott adatok megfelelnek, a függvény 'true' logikai értékkel tér vissza és az adatok inzertálhatóak az adatbázisba.
            {
                try
                {
                    dbConnect.InsertInvestmentData(new AddUserInvestmentModel(textBox_ticker.Text, textBox_currencyName.Text, Convert.ToDouble(textBox_moneyInvested.Text), Convert.ToDouble(textBox_priceOnBought.Text), datePicker_dateOnBought.SelectedDate.Value));
                    dbConnect.UpdateRewardPoints(calculateRewardPoints(Convert.ToDouble(textBox_moneyInvested.Text)));
                    
                    Insert();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Hibás érték került megadására!", "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                this.Hide();
            }
        }

        private bool IsDataValid()
        {
            if (string.IsNullOrWhiteSpace(textBox_currencyName.Text)) //Kriptovaluta neve beviteli mező üres
            {
                tb_errorMessage.Text = "Adja meg a kriptovaluta nevét!";
                textBox_currencyName.Focus();
                return false;
            }
            else if (textBox_currencyName.Text.Length > 50) //Ticker beviteli mező üres
            {
                tb_errorMessage.Text = "A kriptovaluta neve nem lehet több 50 karakternél!";
                textBox_ticker.Focus();
                return false;
            }
            else if (string.IsNullOrWhiteSpace(textBox_ticker.Text)) //Ticker beviteli mező üres
            {
                tb_errorMessage.Text = "Adja meg a ticker-t!";
                textBox_ticker.Focus();
                return false;
            }
            else if (textBox_ticker.Text.Length < 2 || textBox_ticker.Text.Length > 6) //Nem valódi ticker
            {
                tb_errorMessage.Text = "Ilyen ticker nem létezik!";
                textBox_ticker.Focus();
                return false;
            }
            else if (double.TryParse(textBox_currencyName.Text, out double number) || double.TryParse(textBox_ticker.Text, out double number1)) //A mezők csak szöveget tartalmazhatnak
            {
                tb_errorMessage.Text = "A név és ticker mező csak szöveget tartalmazhat!";
                textBox_currencyName.Focus();
                return false;
            }
            else if (string.IsNullOrWhiteSpace(textBox_moneyInvested.Text)) //Befektetett összeg beviteli mező üres
            {
                tb_errorMessage.Text = "Adja meg a befektetés összegét!";
                textBox_moneyInvested.Focus();
                return false;
            }
            else if (string.IsNullOrWhiteSpace(textBox_priceOnBought.Text)) //Vásárlás árfolyama beviteli mező üres
            {
                tb_errorMessage.Text = "Adja meg a vásárlási árfolyamot!";
                textBox_priceOnBought.Focus();
                return false;
            }
            else if (datePicker_dateOnBought.SelectedDate == null) //Nincs kiválasztott dátum
            {
                tb_errorMessage.Text = "Adja meg a befektetés dátumát!";
                datePicker_dateOnBought.Focus();
                return false;
            }
            else if (datePicker_dateOnBought.SelectedDate > DateTime.Now) //Az aktuális dátumnál későbbi került megadásra
            {
                tb_errorMessage.Text = $"Nem adhat meg {DateTime.Now:yyyy-MM-dd}-nál későbbi dátumot!";
                datePicker_dateOnBought.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        //A befektetett pénzösszeg után annak x%-ával jutalmazza a felhasználót a befektetett összegtől függően.
        private double calculateRewardPoints(double moneyInvested)
        {
            if (moneyInvested <= 300000)
            {
                return moneyInvested * 0.005; //0,5%
            }
            else if (moneyInvested > 300000 && moneyInvested > 500000)
            {
                return moneyInvested * 0.008; //0,8%
            }
            else
            {
                return moneyInvested * 0.01; //1%
            }
        }

        private void window_addUserInvestment_Loaded(object sender, RoutedEventArgs e)
        {
            textBox_currencyName.Focus();
            datePicker_dateOnBought.SelectedDate = DateTime.Now;
        }
    }
}
