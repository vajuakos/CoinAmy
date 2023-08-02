using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
    /// A felhasználó rögzíthet, eltávolíthat illetve módosíthatja a befektetéseit, amelyek az adatbázisban módosításra kerülnek.
    /// Event segítségével frissítésre kerül a szülő ablak a módosítás után.
    /// </summary>
    public partial class UserInvestments : Window
    {
        protected MainWindow mainWindow;

        private DbConnect dbConnect;
        private List<AddUserInvestmentModel> userInvestments;

        public UserInvestments(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");
            userInvestments = new List<AddUserInvestmentModel>();

            textBlock_sumOfInvestments.Text = string.Empty;
            textBlock_avarageOfInvestments.Text = string.Empty;
        }

        private void listView_investments_Loaded(object sender, RoutedEventArgs e)
        {
            //A vezérlő betöltése után betöltöltődnek az adatbázisből a listába a befektetések adatai.
            userInvestments = dbConnect.GetInvestmentDatas();

            BuildListViewStructure();

            //A lista tartalma betöltésre kerül a ListView-ba.
            listView_investments.ItemsSource = userInvestments;
        }

        //Felépíti a ListView oszlopait és megvalósítja az adatkötést (DataBinding) az AddUserInvestmentModel-el.
        private void BuildListViewStructure()
        {
            GridView gridView = new GridView();
            listView_investments.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Ticker",
                DisplayMemberBinding = new Binding("ticker"),
                Width = 80
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Megnevezés",
                DisplayMemberBinding = new Binding("name"),
                Width = 150
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Befektetett összeg",
                DisplayMemberBinding = new Binding(nameof(AddUserInvestmentModel.moneyInvested))
                {
                    StringFormat = "{0:0.##} HUF"
                },
                Width = 200
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Vásárlás árfolyama",
                DisplayMemberBinding = new Binding(nameof(AddUserInvestmentModel.priceOnBought))
                {
                    StringFormat = "{0:0.##} HUF"
                },
                Width = 200
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Vásárolt mennyiség",
                DisplayMemberBinding = new Binding("amountBought"),
                Width = 200
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Vásárlás dátuma",
                DisplayMemberBinding = new Binding("dateOnBought"),
                Width = 200
            });
        }

        //Amikor az AddUserInvestment osztály eseményt ad ki, ez a metódus figyeli azt és fissíti a ListView tartalmát.
        private void UpdateEventHandlerAdd(object sender, AddUserInvestment.UpdateEventArgs args)
        {
            listView_investments.ItemsSource = dbConnect.GetInvestmentDatas();

            //Frissítésre kerülnek a különböző statisztikák is.

            textBlock_sumOfInvestments.Text = $"{dbConnect.SumOfInvestments()} Ft";

            textBlock_avarageOfInvestments.Text = $"{Convert.ToInt32(dbConnect.AverageOfInvestments())} Ft";

            textBlock_lastInvestment.Text = dbConnect.LastInvestment();

            textBlock_rewardPoints.Text = $"{Convert.ToInt32(dbConnect.GetRewardPoints())}";
        }

        //Amikor az EditUserInvestment osztály eseményt ad ki, ez a metódus figyeli azt és fissíti a ListView tartalmát.
        private void UpdateEventHandlerEdit(object sender, EditUserInvestment.UpdateEventArgs args)
        {
            listView_investments.ItemsSource = dbConnect.GetInvestmentDatas();

            //Frissítésre kerülnek a különböző statisztikák is.

            textBlock_sumOfInvestments.Text = $"{dbConnect.SumOfInvestments()} Ft";

            textBlock_avarageOfInvestments.Text = $"{Convert.ToInt32(dbConnect.AverageOfInvestments())} Ft";

            textBlock_lastInvestment.Text = dbConnect.LastInvestment();
        }

        //Felhasználói kattintás után megnyílik az új befektetés rögzítésére szolgáló ablak. Emellett az UpdateEventHandler metódus által keltett eseménnyel
        //bővítésre kerül a ListView tartalma.
        private void button_addUserInvestment_Click(object sender, RoutedEventArgs e)
        {
            AddUserInvestment addUserInvestmentWindow = new AddUserInvestment(this);
            addUserInvestmentWindow.UpdateEventHandler += UpdateEventHandlerAdd;
            
            addUserInvestmentWindow.ShowDialog();
        }

        //A felhasználó által választott indexen törlésre kerül a befektetés.
        private void button_removeUserInvestment_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listView_investments.SelectedIndex;

            if (listView_investments.SelectedItem == null)
            {
                MessageBox.Show("Válasszon egy befektetést a listából!", "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBoxResult mbResult = MessageBox.Show("Biztosan törli a befektetést?", "CoinAmy", MessageBoxButton.YesNo, MessageBoxImage.Question);

                //Amennyiben a felhasználó választása igenlő a MessageBox-ban.
                if (mbResult == MessageBoxResult.Yes)
                {
                    //Törlésre kerül az adatbázisból a választott befektetés.
                    dbConnect.DeleteInvestmentData(selectedIndex);

                    //A ListView tartalma frissítésre kerül.
                    listView_investments.ItemsSource = dbConnect.GetInvestmentDatas();

                    textBlock_sumOfInvestments.Text = $"{dbConnect.SumOfInvestments()} Ft";

                    textBlock_avarageOfInvestments.Text = $"{Convert.ToInt32(dbConnect.AverageOfInvestments())} Ft";

                    textBlock_lastInvestment.Text = dbConnect.LastInvestment();
                }
            }
        }

        //Felhasználói kattintás után megnyílik a befektetés szerkesztésére szolgáló ablak. Emellett az UpdateEventHandler metódus által keltett eseménnyel
        //megváltozatásra kerül a ListView tartalma.
        private void button_editUserInvestment_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listView_investments.SelectedIndex;

            if (listView_investments.SelectedItem == null)
            {
                MessageBox.Show("Válasszon egy befektetést a listából!", "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                EditUserInvestment editUserInvestmentWindow = new EditUserInvestment(this, selectedIndex);
                editUserInvestmentWindow.UpdateEventHandler += UpdateEventHandlerEdit;

                editUserInvestmentWindow.ShowDialog();
            }
        }

        private void hyperlink_generatePDF_Click(object sender, RoutedEventArgs e)
        {
            //Amennyiben a felhasználónak nincs hozzáadott befektetése, az exportálás nem lehetséges.
            if (!dbConnect.HasUserHaveInvestments())
            {
                MessageBox.Show("Adjon hozzá befektetést az exportáláshoz!", "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                //Amennyiben a függvény igaz értékkel tér vissza, egy SaveFileDialog nyílik meg.

                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Exportálás PDF formátumban",
                    FileName = $"CoinAmy_portfolio_{DateTime.Now:yyyy-MM-dd}",
                    Filter = "PDF document (*.pdf)|*.pdf",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) //A dokumentum alapértelmezett mentési könyvtára.
                };

                if ((bool)saveFileDialog.ShowDialog())
                {
                    PDF_generate pdf_generate = new PDF_generate(saveFileDialog.FileName);

                    pdf_generate.GenerateDocument();

                    MessageBox.Show($"Sikeresen mentve a {System.IO.Path.GetDirectoryName(saveFileDialog.FileName)} helyre!"); //A Path.GetDirectyName megadja a fájl mentési helyének elérésiútvonalát.
                }
            }
        }

        private void window_UI_Loaded(object sender, RoutedEventArgs e)
        {
            //Betölti a bejelentkezett ember felhasználónevét.
            tb_userName.Text = dbConnect.GetUsername();

            textBlock_sumOfInvestments.Text = $"{dbConnect.SumOfInvestments()} Ft";

            textBlock_avarageOfInvestments.Text = $"{Convert.ToInt32(dbConnect.AverageOfInvestments())} Ft";

            textBlock_lastInvestment.Text = dbConnect.LastInvestment();

            textBlock_rewardPoints.Text = $"{Convert.ToInt32(dbConnect.GetRewardPoints())}";
        }

        private void image_user_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Megnyitja a jelszó változtatásra szolgáló ablakot.
            new UserSettings().ShowDialog();
        }

        private void button_exchangeRates_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Hide();
        }

        private void button_currencyConverter_Click(object sender, RoutedEventArgs e)
        {
            new CurrencyConverter(mainWindow).Show();
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
