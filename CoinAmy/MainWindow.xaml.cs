using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace CoinAmy
{
    /// <summary>
    /// Megtekinthetőek a választott kriptovalutához tartozó árfolyamgrafikonok. A ListView-ban láthatóak az API-tól érkező kriptovaluta adatok.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private API_call api_call; //API hívó osztály
        private DbConnect dbConnect;

        private List<CryptocurrencyDataModel> coinDatas; //Kriptovaluták adatai
        private List<Coin> HistoricalDatas; //Árfolyam adatok

        private SeriesCollection priceChart;
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public SeriesCollection PriceChart
        {
            get { return priceChart; }
            set
            {
                priceChart = value;
                OnPropertyChanged(nameof(priceChart)); //A property változása esetén értesíti a UI-t.
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            api_call = new API_call();
            dbConnect = new DbConnect("127.0.0.1", "coinamy_db", "root", "");

            coinDatas = new List<CryptocurrencyDataModel>();
            HistoricalDatas = new List<Coin>();

            PriceChart = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Árfolyam",
                    Values = new ChartValues<double>()
                }
            };

            DataContext = this;
        }

        private void listView_coins_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataToListView(); //Felépíti a ListView elrendezését és betölti az API által biztosított adatokat.

            listView_coins.SelectedIndex = 0;
        }

        private async void LoadDataToListView()
        {
            coinDatas = await api_call.GetData();

            GridView gridView = new GridView();
            listView_coins.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "#",
                DisplayMemberBinding = new Binding("market_cap_rank")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Ikon",
                DisplayMemberBinding = new Binding("image")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Megnevezés",
                DisplayMemberBinding = new Binding("name")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Ticker",
                DisplayMemberBinding = new Binding("symbol")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Változás (24 órás)",
                DisplayMemberBinding = new Binding(nameof(CryptocurrencyDataModel.price_change_percentage_24h))
                {
                    StringFormat = "{0:0.00} %"
                }
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Árfolyam",
                DisplayMemberBinding = new Binding(nameof(CryptocurrencyDataModel.current_price))
                {
                    StringFormat = "{0:0.##} HUF"
                }
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Kapitalizáció",
                DisplayMemberBinding = new Binding(nameof(CryptocurrencyDataModel.market_cap))
                {
                    StringFormat = "{0} HUF"
                }
            });

            //ListView adatainak feltöltése
            for (int i = 0; i < coinDatas.Count; i++)
            {
                listView_coins.Items.Add(new CryptocurrencyDataModel
                {
                    market_cap_rank = coinDatas[i].market_cap_rank, //#
                    image = coinDatas[i].image, //Ikon
                    name = coinDatas[i].name, //Megnevezés
                    symbol = coinDatas[i].symbol.ToUpper(), //Ticker
                    price_change_percentage_24h = coinDatas[i].price_change_percentage_24h, //Változás (24 órás)
                    current_price = coinDatas[i].current_price, //Árfolyam
                    market_cap = coinDatas[i].market_cap //Kapitalizáció
                });
            }
        }

        private async void listView_coins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            priceChart[0].Values.Clear();

            int listView_coins_SelectedIndex = listView_coins.SelectedIndex;

            //A historikus adatokat biztosító API végpont URL-be beillesztésre kerül a ListView kiválasztott indexén szerplő kriptovaluta.
            //Mivel a ListView is a coinDatas lista adataiból kerül feltöltésre, ezért a választott indexek megegyeznek.
            HistoricalDatas = await api_call.GetHistoricalData(coinDatas[listView_coins_SelectedIndex].id);

            foreach (var coin in HistoricalDatas)
            {
                PriceChart[0].Values.Add(coin.price);
            }
        }

        private void window_MW_Loaded(object sender, RoutedEventArgs e)
        {
            //Betölti a bejelentkezett ember felhasználónevét.
            tb_userName.Text = dbConnect.GetUsername();
        }

        private void image_user_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Megnyitja a jelszó változtatásra szolgáló ablakot.
            new UserSettings().ShowDialog();
        }

        private void button_portfolio_Click(object sender, RoutedEventArgs e)
        {
            //Portfolio ablak
            new UserInvestments(this).Show();
            this.Hide();
        }

        private void button_currencyConverter_Click(object sender, RoutedEventArgs e)
        {
            //Árfolyamátváltó ablak
            new CurrencyConverter(this).Show();
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
