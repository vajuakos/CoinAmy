using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace CoinAmy
{
    public class API_call
    {
        private HttpClient httpClient;

        private List<CryptocurrencyDataModel> coinDatas;

        public API_call()
        {
            httpClient = new HttpClient();

            coinDatas = new List<CryptocurrencyDataModel>();
        }

        public async Task<List<CryptocurrencyDataModel>> GetData()
        {
            Uri coinGeckoAPI = new Uri($"https://api.coingecko.com/api/v3/coins/markets?vs_currency=huf&order=market_cap_desc&per_page=50&page=1&sparkline=false&price_change_percentage=24h");

            try
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CoinAmy");
                
                var httpResponse = await httpClient.GetAsync(coinGeckoAPI);

                if (httpResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                    coinDatas = JsonConvert.DeserializeObject<List<CryptocurrencyDataModel>>(jsonResponse);
                }
                else
                {
                    MessageBox.Show($"{httpResponse.StatusCode}, {httpResponse.ReasonPhrase}");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Hiba a kapcsolódás során!");
            }

            return coinDatas;
        }

        public async Task<List<Coin>> GetHistoricalData(string coinName)
        {
            Uri coinGeckoHistoricalDataAPI = new Uri($"https://api.coingecko.com/api/v3/coins/{coinName}/market_chart?vs_currency=huf&days=365");

            try
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CoinAmy");

                var httpResponse = await httpClient.GetAsync(coinGeckoHistoricalDataAPI);

                if (httpResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<HistoricalDataModel>(jsonResponse);
                    return data.prices.Select(listOfDatas => new Coin
                    {
                        time = DateTimeOffset.FromUnixTimeMilliseconds((long)listOfDatas[0]).LocalDateTime,
                        price = listOfDatas[1]
                    }).ToList();
                }
                else
                {
                    MessageBox.Show($"{httpResponse.StatusCode}, {httpResponse.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Hiba a kapcsolódás során!");
                return null;
            }
        }
    }
}
