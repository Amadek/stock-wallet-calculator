using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            CompaniesFitnessCalculator companiesFitnessCalculator = new CompaniesFitnessCalculator(wallet: 1000.00M);
            int variantsNumber = (int)Math.Pow(2, 4);
            decimal percentDiffer = 0.001M;

            GenomeGenerator genomeGenerator = new GenomeGenerator(new List<IEnumerable<object>>
            {
                Program.GenerateCompanyVariant(new Company { Name = "AFG", Percent = 0.30M, Price = 116.43M }, percentDiffer, variantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "DEV", Percent = 0.07M, Price = 28.40M }, percentDiffer, variantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "EVR", Percent = 0.34M, Price = 124.17M }, percentDiffer, variantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "INTEL", Percent = 0.12M, Price = 46.89M }, percentDiffer, variantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "KGHM", Percent = 0.09M, Price = 33.00M }, percentDiffer, variantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "MFO", Percent = 0.04M, Price = 10.77M }, percentDiffer, variantsNumber),
                Program.GenerateCompanyVariant(new Company { Name = "OPTEAM", Percent = 0.04M, Price = 3.36M }, percentDiffer, variantsNumber),
            }, companiesFitnessCalculator);

            GenerationsRunner generationsRunner = new GenerationsRunner(
                genomeGenerator, 
                new GenomeCrossover(),
                new GenomeMutator(),
                companiesFitnessCalculator
            );

            string bestGenome = generationsRunner.RunGenerations(50, 950.00M);
            List<object> bestCombination = genomeGenerator.ParseGenome(bestGenome).ToList();
            decimal bestFitness = companiesFitnessCalculator.Calculate(bestCombination);

            Console.WriteLine(string.Join('\t', bestCombination) + '\t' + bestFitness);
        }

        static IEnumerable<Company> GenerateCompanyVariant(Company company, decimal percent, int count)
        {
            // Chcemy mieć wyniki takie żeby firma o podanym procencie była mniej więcej w środku wyników. Dodatkowa obsługa żebyśmy nie wyjechali na ujemne wartości.
            decimal biggestPossiblePercentDiffer = Enumerable.Range(1, count)
                .Select(n => percent * n / 2)
                .Where(x => x < company.Percent)
                .Max();

            decimal companyPersent = company.Percent - biggestPossiblePercentDiffer;
            for (int companyNumber = 0; companyNumber < count; companyNumber++)
            {
                yield return new Company { Name = company.Name, Percent = companyPersent, Price = company.Price };
                companyPersent += percent;
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
