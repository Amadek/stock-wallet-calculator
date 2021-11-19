using System.Collections.Generic;

namespace StockWalletCalculator
{
    class CompanyVariantsGenerator
    {
        private readonly Config _config;

        public CompanyVariantsGenerator(Config config)
        {
            _config = config;
        }

        /// <summary>
        /// Zwraca matrycę możliwych wariantów firm. <br />
        /// Wiersz oznacza każdą kolejną firmę, kolumna oznacza wariant danej firmy.
        /// </summary>
        /// <returns>Matryca możliwych wariantów firm</returns>
        public IEnumerable<IEnumerable<Company>> Generate()
        {
            foreach (Company company in _config.Companies)
            {
                yield return this.Generate(company);
            }
        }

        private IEnumerable<Company> Generate(Company company)
        {
            decimal maxCompanyPercentChange = _config.CompanyPercentDiffer;
            int companyVariantsCount = _config.CompanyVariantsCount;

            // Na start przyjmujemy namniejszy możliwy procent. Jeżeli user podał 2% procent możliwej różnicy dla akcji 10%, to przyjmie ona wartość 9.8% na start.
            decimal minCompanyPercent = company.Percent * (1 - maxCompanyPercentChange);
            decimal maxCompanyPercent = company.Percent * (1 + maxCompanyPercentChange);
            decimal companyPercent = minCompanyPercent;
            // Wyliczamy wzrost procentu, który będzie zwiększany z każdym przebiegiem pętli aż osiągnie maksymalną wartość określoną w argumencie. 10.2% dla przykładku powyżej.
            decimal companyPercentChange = (maxCompanyPercent - minCompanyPercent) / companyVariantsCount;

            for (int companyNumber = 0; companyNumber < companyVariantsCount; companyNumber++)
            {
                yield return new Company { Name = company.Name, Percent = companyPercent, Price = company.Price };
                companyPercent += companyPercentChange;
            }
        }
    }
}
