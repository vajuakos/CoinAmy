using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinAmy
{
    internal class AddUserInvestmentModel
    {
        public string ticker { get; set; }
        public string name { get; set; }
        public double moneyInvested { get; set; }
        public double priceOnBought { get; set; }
        public double amountBought { get; set; }
        public DateTime dateOnBought { get; set; }
        public double rewardPoints { get; set; }

        public AddUserInvestmentModel(string ticker, string name, double moneyInvested, double priceOnBought, DateTime dateOnBought)
        {
            this.ticker = ticker;
            this.name = name;
            this.moneyInvested = moneyInvested;
            this.priceOnBought = priceOnBought;
            this.dateOnBought = dateOnBought;
        }

        public AddUserInvestmentModel(string ticker, string name, double moneyInvested, double priceOnBought, double amountBought, DateTime dateOnBought)
        {
            this.ticker = ticker;
            this.name = name;
            this.moneyInvested = moneyInvested;
            this.priceOnBought = priceOnBought;
            this.amountBought = amountBought;
            this.dateOnBought = dateOnBought;
        }
    }
}
