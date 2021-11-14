using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            WeightedRandomBag<Company> companies = new WeightedRandomBag<Company>();

            companies.Add(new Company { Name = "AFG" }, 10);
            companies.Add(new Company { Name = "INTEL" }, 300);
            companies.Add(new Company { Name = "EVERCORE" }, 50);

            foreach (int _ in Enumerable.Range(1, 100))
            {
                Console.WriteLine("Random weight company: " + companies.GetWeghtedRandomElement().Name);
            }
            // ^ Super, mamy losowanie z wiekszą szansą na element z wyższą wagą, teraz trzeba pomyśleć nad przedstawieniem kombinacji akcji w postaci genomu.
            // Akcja A stanowiąca 20% portfela może być jako 0 (20%) i 1 (21%) <= Na razie mały zbiór możliwości żeby były proste do ogarnięcia.
            // Akcja B stanowiącja 80% portfela może być jako 0 (80%) i 1 (81%) <= Można tak zrobić i odrzucić coś co jest większe od 100%, dla większej ilości możliwości będzie to mieć sens.
            decimal wallet = 1000.00M;
            Company afg = new Company { Name = "AFG", Percent = 0.20M, Price = 24.30M };
            Company intel = new Company { Name = "INTEL", Percent = 0.80M, Price = 43.70M };
            decimal moneyForAFG = (int)Math.Floor(wallet * afg.Percent / afg.Price) * afg.Price;
            decimal moneyForINTEL = (int)Math.Floor(wallet * intel.Percent / intel.Price) * intel.Price;
            decimal usedMoney = moneyForAFG + moneyForINTEL;
            Console.WriteLine("Used money: " + usedMoney);
            // ^ Tutaj nadchodzi moment kombinowania. Trzeba tak pomodyfikować % akcji żeby wykorzystać jak najwięcej pieniędzy. Do tego wykorzystamy algorytm genetyczny.
            // Genom dla portfela z INTEL i AFG mógłby wyglądać tak: 00, 01, 10, 11.
            // 01 oznaczałby 20% INTEL i 81% AFG <- przekroczymy tutaj wartość portfela więc waga genomu od razu = 0.
            int[] genome1 = { 0, 0 }; // 18% INTEL
            genome1 = new int[] { 0, 1 }; // 19% INTEL
            genome1 = new int[] { 1, 0 }; // 20% INTEL
            genome1 = new int[] { 1, 1 }; // 21% INTEL

            CompaniesGenomeFitnessCalculator genomeFitnessCalculator = new CompaniesGenomeFitnessCalculator();
            genomeFitnessCalculator.AddCompany(afg);
            genomeFitnessCalculator.AddCompany(intel);
            Console.WriteLine("Genome fitness 0000: " + genomeFitnessCalculator.CalculateFitness(new List<int> { 0, 0, 0, 0 }));
            Console.WriteLine("Genome fitness 0001: " + genomeFitnessCalculator.CalculateFitness(new List<int> { 0, 0, 0, 1 }));
            Console.WriteLine("Genome fitness 0101: " + genomeFitnessCalculator.CalculateFitness(new List<int> { 0, 1, 0, 1 }));
            Console.WriteLine("Genome fitness 1000: " + genomeFitnessCalculator.CalculateFitness(new List<int> { 1, 0, 0, 0 }));
            Console.WriteLine("Genome fitness 0010: " + genomeFitnessCalculator.CalculateFitness(new List<int> { 0, 0, 1, 0 }));
            Console.WriteLine("Genome fitness 1110: " + genomeFitnessCalculator.CalculateFitness(new List<int> { 1, 1, 1, 0 }));
            Console.WriteLine("Genome fitness 1111: " + genomeFitnessCalculator.CalculateFitness(new List<int> { 1, 1, 1, 1 }));
            // ^ EKSTRA, mamy liczenie ceny portfela na podstawie kombinacji genomu - moznaby to lepiej napisac bo ciezko to ogarnac
            var genomeGenerator = new GenomeGenerator(new object[][]
            {
                new object[]
                {
                    new Company { Name = "AFG", Percent = 0.18M, Price = 24.30M },
                    new Company { Name = "AFG", Percent = 0.19M, Price = 24.30M },
                    new Company { Name = "AFG", Percent = 0.20M, Price = 24.30M },
                    new Company { Name = "AFG", Percent = 0.21M, Price = 24.30M }
                },
                new object[]
                {
                    new Company { Name = "INTEL", Percent = 0.78M, Price = 43.70M },
                    new Company { Name = "INTEL", Percent = 0.79M, Price = 43.70M },
                    new Company { Name = "INTEL", Percent = 0.80M, Price = 43.70M },
                    new Company { Name = "INTEL", Percent = 0.81M, Price = 43.70M } }
            }, new CompaniesFitnessCalculator(1000.00M));

            Console.WriteLine("GENOME GENERATION");

            foreach (Genome genome in genomeGenerator.GenerateGeneration())
            {
                object[] objects = genomeGenerator.ParseGenome(genome.Value).ToArray();
                string objectsDetails = string.Join('\t', objects.Select(o => o.ToString()));
                Console.WriteLine($"{genome.Value}\t{genome.Fitness}\t{objectsDetails}");
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

        public int GenomePartLength => 2;

        public decimal GetModifiedPercent(IEnumerable<int> genomePartArray)
        {
            int genomePart = int.Parse(string.Join(string.Empty, genomePartArray));
            Console.WriteLine($"Genome part for {this.Name}: {genomePart}");
            switch (genomePart)
            {
                case 0: return this.Percent - 0.02M;
                case 1: return this.Percent - 0.01M;
                case 10: return this.Percent;
                case 11: return this.Percent + 0.01M;
                default: throw new NotSupportedException(genomePart.ToString());
            }
        }

        public override string ToString()
        {
            return $"{this.Name}\t{this.Price}\t{this.Percent * 100}%";
        }
    }

    /// <summary>
    /// Genom listy spółek do kupienia, np. 01|11 oznacza zaniżony procent społki A i maksymalny procent spółki B.
    /// </summary>
    public class CompaniesGenomeFitnessCalculator
    {
        private readonly List<Company> _companiesInGenome = new List<Company>();

        public int GenomePartLength => 2;
        public decimal Wallet => 1000.00M;

        public void AddCompany(Company company)
        {
            _companiesInGenome.Add(company);
        }

        public decimal CalculateFitness(List<int> genome)
        {
            if (genome.Count % this.GenomePartLength != 0 || genome.Count / this.GenomePartLength != _companiesInGenome.Count)
            {
                throw new NotSupportedException("Invalid genome length");
            }
            

            decimal walletFitness = 0.00M;
            int companiesInGenomeIndex = 0;
            for (int index = 0; index < genome.Count; index += this.GenomePartLength)
            {
                List<int> companyGenomePart = genome.GetRange(index, this.GenomePartLength);
                Company company = _companiesInGenome[companiesInGenomeIndex++];

                int companySharesToBuy = (int)(this.Wallet * company.GetModifiedPercent(companyGenomePart) / company.Price);
                Console.WriteLine($"Shares to buy for {company.Name}: {companySharesToBuy}");
                walletFitness += companySharesToBuy * company.Price;

                if (walletFitness > this.Wallet)
                {
                    return 0.00M;
                }
            }

            return walletFitness;
        }
    }
}
