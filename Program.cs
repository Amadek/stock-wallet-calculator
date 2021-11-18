using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    class Program
    {
        static void Main()
        {
            int companyVariantsNumber = (int)Math.Pow(2, 8);
            decimal percentDiffer = 0.02M;
            decimal wallet = 1000.00M;

            CompanyGenomeParser companyGenomeParser = new CompanyGenomeParser(new List<IEnumerable<Company>>
            {
                Program.GenerateCompanyVariant(new Company { Name = "AFG", Percent = 0.30M, Price = 116.43M }, percentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "DEV", Percent = 0.07M, Price = 28.40M }, percentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "EVR", Percent = 0.34M, Price = 124.17M }, percentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "INTEL", Percent = 0.12M, Price = 46.89M }, percentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "KGHM", Percent = 0.09M, Price = 33.00M }, percentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "MFO", Percent = 0.04M, Price = 10.77M }, percentDiffer, companyVariantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "OPTEAM", Percent = 0.04M, Price = 3.36M }, percentDiffer, companyVariantsNumber),
            });
            CompaniesFitnessCalculator companiesFitnessCalculator = new CompaniesFitnessCalculator(companyGenomeParser, wallet);
            GenomeGenerator genomeGenerator = new GenomeGenerator(companyGenomeParser.GenomeLength);
            GenerationsRunner generationsRunner = new GenerationsRunner(
                genomeGenerator, 
                new GenomeCrossover(),
                new GenomeMutator(),
                companiesFitnessCalculator
            );

            string bestGenome = generationsRunner.RunGenerations(generationLimit: 100, fitnessLimit: 950.00M);
            List<Company> bestCompaniesCombination = companyGenomeParser.ParseGenome(bestGenome).ToList();
            decimal bestFitness = companiesFitnessCalculator.Calculate(bestGenome);

            Console.WriteLine(string.Join('\n', bestCompaniesCombination) + '\n' + bestFitness);
        }

        static IEnumerable<Company> GenerateCompanyVariant(Company company, decimal maxCompanyPercentChange, int companyVariantsCount)
        {
            // Na start przyjmujemy namniejszy możliwy procent. Jeżeli user podał 2% procent możliwej różnicy dla akcji 10%, to przyjmie ona wartość 8% na start.
            decimal companyPercent = company.Percent - maxCompanyPercentChange;
            // Wyliczamy wzrost procentu, który będzie zwiększany z każdym przebiegiem pętli aż osiągnie maksymalną wartość określoną w argumencie. 12% dla przykładku powyżej.
            decimal companyPercentChange = maxCompanyPercentChange / (companyVariantsCount - 1);

            for (int companyNumber = 0; companyNumber < companyVariantsCount; companyNumber++)
            {
                yield return new Company { Name = company.Name, Percent = companyPercent, Price = company.Price };
                companyPercent += companyPercentChange;
            }
        }
    }

    /// <summary>
    /// Spółka w portfelu inwestycyjnym, zawiera nazwę, procent w portfelu i aktualną cenę za akcję.
    /// </summary>
    public class Company
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Percent { get; set; }

        public override string ToString()
        {
            return $"{this.Name}\t{this.Price}\t{this.Percent * 100}%";
        }
    }
}
