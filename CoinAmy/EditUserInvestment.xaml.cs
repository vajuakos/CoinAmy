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
    /// Az adatbázisból, listába beolvasott adatok megjelenítésre kerülnek a TextBox-okban, amelyek módosítására van lehetőség. Módosítás esetén a szülő ablak egy esemény segítségével frissül.
    /// </summary>
    public partial class EditUserInvestment : Window
    {
        private DbConnect dbConnect;

        private int userInvestmentsSelectedIndex;

        private List<AddUserInvestmentModel> selectedInvestment;

        public EditUserInvestment(UserInvestments userInvestments, int selectedIndex)
        {
            InitializeComponent();

            userInvestmentsSelectedIndex = selectedIndex; //ListView-ból választott befektetés indexe

            tb_errorMessage.Text = string.Empty;

            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");

            selectedInvestment = new List<AddUserInvestmentModel>();
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

        //Betöltésre kerülnek a szerkesztendő befektetési adatok az adatbázisból.
        private void window_editUserInvestment_Loaded(object sender, RoutedEventArgs e)
        {
            selectedInvestment = dbConnect.LoadDataToEditInvestment(userInvestmentsSelectedIndex);

            textBox_currencyName.Text = selectedInvestment[0].name;
            textBox_ticker.Text = selectedInvestment[0].ticker;
            textBox_moneyInvested.Text = selectedInvestment[0].moneyInvested.ToString();
            textBox_priceOnBought.Text = selectedInvestment[0].priceOnBought.ToString();
            datePicker_dateOnBought.SelectedDate = selectedInvestment[0].dateOnBought;

            textBox_currencyName.Focus();
        }

        private void button_editInvestment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsDataValid())
                {
                    dbConnect.EditInvestmentData(new AddUserInvestmentModel(textBox_ticker.Text, textBox_currencyName.Text, Convert.ToDouble(textBox_moneyInvested.Text), Convert.ToDouble(textBox_priceOnBought.Text), datePicker_dateOnBought.SelectedDate.Value), userInvestmentsSelectedIndex);
                    Insert();

                    tb_errorMessage.Text = "Sikeres módosítás!";
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Hibás érték került megadására!", "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
    }
}
