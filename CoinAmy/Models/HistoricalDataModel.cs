using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinAmy
{
    public class HistoricalDataModel
    {
        public List<List<double>> prices { get; set; }
        
        //Nem használt
        //public List<List<object>> market_caps { get; set; }
        //public List<List<double>> total_volumes { get; set; }
    }

    public class Coin
    {
        public DateTimeOffset time { get; set; }
        public double price { get; set; }
    }
}
