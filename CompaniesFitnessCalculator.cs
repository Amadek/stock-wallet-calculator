
namespace StockWalletCalculator
{
    class CompaniesFitnessCalculator : IFitnessCalculator
    {
        private readonly CompanyGenomeParser _companyGenomeParser;
        private readonly Config _config;

        public CompaniesFitnessCalculator(CompanyGenomeParser companyGenomeParser, Config config)
        {
            _companyGenomeParser = companyGenomeParser;
            _config = config;
        }

        public decimal Calculate(string genome)
        {
            decimal walletFitness = 0.00M;
            foreach (Company company in _companyGenomeParser.ParseGenome(genome))
            {
                int companySharesToBuy = (int)(_config.Wallet * company.Percent / company.Price);
                walletFitness += companySharesToBuy * company.Price;

                if (walletFitness > _config.Wallet)
                {
                    return 0.00M;
                }
            }

            return walletFitness;
        }

        public int GetCompanySharesToBuy(Company company)
        {
            int companySharesToBuy = (int)(_config.Wallet * company.Percent / company.Price);
            return companySharesToBuy;
        }
    }
}
