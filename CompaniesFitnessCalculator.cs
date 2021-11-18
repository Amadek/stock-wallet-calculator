
namespace StockWalletCalculator
{
    class CompaniesFitnessCalculator : IFitnessCalculator
    {
        private readonly CompanyGenomeParser _companyGenomeParser;
        private readonly decimal _wallet;

        public CompaniesFitnessCalculator(CompanyGenomeParser companyGenomeParser, decimal wallet)
        {
            _companyGenomeParser = companyGenomeParser;
            _wallet = wallet;
        }

        public decimal Calculate(string genome)
        {
            decimal walletFitness = 0.00M;
            foreach (Company company in _companyGenomeParser.ParseGenome(genome))
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
