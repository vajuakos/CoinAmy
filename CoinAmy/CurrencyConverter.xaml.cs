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
    /// Az API-től érkező adatok segítségével és a felhasználó által megadott információk alapján kiszámításra kerül két kriptovaluta
    /// egyikből a másikba történő váltása esetén a kapható mennyiség.
    /// </summary>
    public partial class CurrencyConverter : Window
    {
        private MainWindow mainWindow;
        
        private API_call api_call;
        private DbConnect dbConnect;

        private List<CryptocurrencyDataModel> coinDatas;

        private int cbIndexFrom;
        private int cbIndexTo;

        public CurrencyConverter(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            api_call = new API_call();
            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");

            coinDatas = new List<CryptocurrencyDataModel>();

            textBox_from.Text = 0.ToString();
            textBox_to.IsEnabled = false;
        }

        private void cb_from_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataToComboBoxFrom();
        }

        private void cb_to_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataToComboBoxTo();
        }

        private void button_convert_Click(object sender, RoutedEventArgs e)
        {
            cbIndexFrom = cb_from.SelectedIndex;
            cbIndexTo = cb_to.SelectedIndex;

            double input = 0;
            double output = 0;

            try
            {
                if (CheckValue())
                {
                    input = Convert.ToDouble(textBox_from.Text) * coinDatas[cbIndexFrom].current_price;
                    output = coinDatas[cbIndexTo].current_price;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Hibás érték került megadására!", "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            textBox_to.Text = (input / output).ToString();
        }

        private async void LoadDataToComboBoxFrom()
        {
            coinDatas = await api_call.GetData();

            foreach (var coin in coinDatas)
            {
                //A ComboBox minden eleme egy StackPanel, amely Label-ből és Image-ből áll
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.VerticalAlignment = VerticalAlignment.Center;

                //Az API-től érkező Kriptovaluta nevek
                Label text = new Label();
                text.Content = coin.name;
                text.VerticalAlignment = VerticalAlignment.Center;

                //Az API-től érkező Kriptovaluta logok
                Image logoImage = new Image();
                logoImage.Source = new BitmapImage(new Uri(coin.image));
                logoImage.Width = 55;
                logoImage.Width = 55;
                logoImage.VerticalAlignment = VerticalAlignment.Center;

                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.Content = logoImage;

                //Hozzáadásra kerül mindkét al-elem
                stackPanel.Children.Add(text);
                stackPanel.Children.Add(cbItem);

                cb_from.Items.Add(stackPanel);
            }

            if (cb_from.Items.Count > 0)
            {
                cb_from.SelectedIndex = 0; //A Combobox kezdőértékét a nulladik indexre állítja
            }
        }

        private async void LoadDataToComboBoxTo()
        {
            coinDatas = await api_call.GetData();

            foreach (var coin in coinDatas)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.VerticalAlignment = VerticalAlignment.Center;

                Label text = new Label();
                text.Content = coin.name;
                text.VerticalAlignment = VerticalAlignment.Center;

                Image logoImage = new Image();
                logoImage.Source = new BitmapImage(new Uri(coin.image));
                logoImage.Width = 55;
                logoImage.Width = 55;
                logoImage.VerticalAlignment = VerticalAlignment.Center;

                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.Content = logoImage;

                stackPanel.Children.Add(text);
                stackPanel.Children.Add(cbItem);

                cb_to.Items.Add(stackPanel);
            }

            if (cb_to.Items.Count > 0)
            {
                cb_to.SelectedIndex = 1; //A Combobox kezdőértékét az első indexre állítja
            }
        }

        private bool CheckValue()
        {
            if (string.IsNullOrEmpty(textBox_from.Text))
            {
                MessageBox.Show("Adja meg az átváltandó összeget!", "CoinAmy", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }

        //A gombra kattintva kicserélődik a két mező tartalma.
        private void image_switchCurrencies_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            cbIndexFrom = cb_from.SelectedIndex;
            cbIndexTo = cb_to.SelectedIndex;

            cb_from.SelectedIndex = cbIndexTo;
            cb_to.SelectedIndex = cbIndexFrom;

            string temporary = textBox_from.Text;

            textBox_from.Text = textBox_to.Text;

            textBox_to.Text = temporary;
        }

        private void window_currencyConverter_Loaded(object sender, RoutedEventArgs e)
        {
            //Betölti a bejelentkezett ember felhasználónevét.
            tb_userName.Text = dbConnect.GetUsername();
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

        private void button_portfolio_Click(object sender, RoutedEventArgs e)
        {
            new UserInvestments(mainWindow).Show();
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
