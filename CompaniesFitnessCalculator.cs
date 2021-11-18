using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockWalletCalculator
{
    public class CompaniesFitnessCalculator : IFitnessCalculator
    {
        private readonly decimal _wallet;

        public CompaniesFitnessCalculator(decimal wallet)
        {
            _wallet = wallet;
        }

        public decimal Calculate(IEnumerable<object> objects)
        {
            Company[] companies = objects.Cast<Company>().ToArray();

            decimal walletFitness = 0.00M;
            foreach (Company company in companies)
            {
                int companySharesToBuy = (int)(_wallet * company.Percent / company.Price);
                walletFitness += companySharesToBuy * company.Price;

                if (walletFitness > _wallet)
                {
                    return 0.00M;
                }
            }

            return walletFitness;
        }
    }
}
