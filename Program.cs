using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    class Program
    {
        static void Main()
        {
            int companyVariantsNumber = (int)Math.Pow(2, 4);
            decimal wallet = 1000.00M;
            decimal walletFitnessLimit = 990.00M;
            decimal companyPercentDiffer = 0.20M;
            int generationsLimit = 100;

            CompanyGenomeParser companyGenomeParser = new CompanyGenomeParser(new List<IEnumerable<Company>>
            {
                Program.GenerateCompanyVariant(new Company { Name = "AFG", Percent = 0.30M, Price = 116.43M }, companyPercentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "DEV", Percent = 0.07M, Price = 28.40M }, companyPercentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "EVR", Percent = 0.34M, Price = 124.17M }, companyPercentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "INTEL", Percent = 0.12M, Price = 46.89M }, companyPercentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "KGHM", Percent = 0.09M, Price = 33.00M }, companyPercentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "MFO", Percent = 0.04M, Price = 10.77M }, companyPercentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "OPTEAM", Percent = 0.04M, Price = 3.36M }, companyPercentDiffer, companyVariantsNumber),
            });
            CompaniesFitnessCalculator companiesFitnessCalculator = new CompaniesFitnessCalculator(companyGenomeParser, wallet);
            GenerationsRunner generationsRunner = new GenerationsRunner(
                new GenomeGenerator(companyGenomeParser.GenomeLength), 
                new GenomeCrossover(),
                new GenomeMutator(),
                companiesFitnessCalculator
            );

            string bestGenome = generationsRunner.RunGenerations(generationsLimit, walletFitnessLimit);
            List<Company> bestCompaniesCombination = companyGenomeParser.ParseGenome(bestGenome).ToList();
            decimal bestFitness = companiesFitnessCalculator.Calculate(bestGenome);

            foreach (Company company in bestCompaniesCombination)
            {
                int companySharesToBuy = (int)(wallet * company.Percent / company.Price);
                Console.WriteLine($"x{companySharesToBuy}\t{company}");
            }

            Console.WriteLine($"\nWallet fitness {bestFitness}");
;
        }

        static IEnumerable<Company> GenerateCompanyVariant(Company company, decimal maxCompanyPercentChange, int companyVariantsCount)
        {
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
