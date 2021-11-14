using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            GenomeGenerator genomeGenerator = new GenomeGenerator(new object[][]
            {
                new object[]
                {
                    new Company { Name = "AFG", Percent = 0.16M, Price = 24.37M },
                    new Company { Name = "AFG", Percent = 0.17M, Price = 24.37M },
                    new Company { Name = "AFG", Percent = 0.18M, Price = 24.37M },
                    new Company { Name = "AFG", Percent = 0.19M, Price = 24.37M },
                    new Company { Name = "AFG", Percent = 0.20M, Price = 24.37M },
                    new Company { Name = "AFG", Percent = 0.21M, Price = 24.37M },
                    new Company { Name = "AFG", Percent = 0.22M, Price = 24.37M },
                    new Company { Name = "AFG", Percent = 0.23M, Price = 24.37M },
                },
                new object[]
                {
                    new Company { Name = "INTEL", Percent = 0.76M, Price = 43.73M },
                    new Company { Name = "INTEL", Percent = 0.77M, Price = 43.73M },
                    new Company { Name = "INTEL", Percent = 0.78M, Price = 43.73M },
                    new Company { Name = "INTEL", Percent = 0.79M, Price = 43.73M },
                    new Company { Name = "INTEL", Percent = 0.80M, Price = 43.73M },
                    new Company { Name = "INTEL", Percent = 0.81M, Price = 43.73M },
                    new Company { Name = "INTEL", Percent = 0.82M, Price = 43.73M },
                    new Company { Name = "INTEL", Percent = 0.83M, Price = 43.73M },
                }
            }, new CompaniesFitnessCalculator(wallet: 1000.00M));

            Console.WriteLine("FIRST GENERATION");

            WeightedRandomBag<Genome> genomes = new WeightedRandomBag<Genome>();
            foreach (Genome genome in genomeGenerator.GenerateGeneration())
            {
                object[] objects = genomeGenerator.ParseGenome(genome.Value).ToArray();
                string objectsDetails = string.Join('\t', objects);
                Console.WriteLine($"{genome.Value}\t{genome.Fitness}\t{objectsDetails}");

                genomes.Add(genome, genome.Fitness);
            }

            Console.WriteLine("RANDOM PAIR WITH WEIGHT");
            
            foreach (Genome genome in genomes.GetWeghtedRandomPair())
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

        public override string ToString()
        {
            return $"{this.Name}\t{this.Price}\t{this.Percent * 100}%";
        }
    }
}
