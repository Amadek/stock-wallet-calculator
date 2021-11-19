using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace StockWalletCalculator
{
    class Program
    {
        static void Main()
        {
            Config config = Program.LoadConfig();
            CompanyVariantsGenerator companyVariantsGenerator = new CompanyVariantsGenerator(config);
            CompanyGenomeParser companyGenomeParser = new CompanyGenomeParser(new List<IEnumerable<Company>>
            {
                companyVariantsGenerator.Generate(new Company { Name = "AFG", Percent = 0.30M, Price = 116.43M }),
                companyVariantsGenerator.Generate(new Company { Name = "DEV", Percent = 0.07M, Price = 28.40M }),
                companyVariantsGenerator.Generate(new Company { Name = "EVR", Percent = 0.34M, Price = 124.17M }),
                companyVariantsGenerator.Generate(new Company { Name = "INTEL", Percent = 0.12M, Price = 46.89M }),
                companyVariantsGenerator.Generate(new Company { Name = "KGHM", Percent = 0.09M, Price = 33.00M }),
                companyVariantsGenerator.Generate(new Company { Name = "MFO", Percent = 0.04M, Price = 10.77M }),
                companyVariantsGenerator.Generate(new Company { Name = "OPTEAM", Percent = 0.04M, Price = 3.36M }),
            });
            CompaniesFitnessCalculator companiesFitnessCalculator = new CompaniesFitnessCalculator(companyGenomeParser, config);
            EvolutionRunner generationsRunner = new EvolutionRunner(
                new GenomeGenerator(companyGenomeParser.GenomeLength), 
                new GenomeCrossover(),
                new GenomeMutator(),
                companiesFitnessCalculator
            );

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            EvolutionResult evolutionResult = generationsRunner.Evolve(config.CompanyGenerationsLimit, config.WalletFitnessLimit);
            List<Company> bestCompaniesCombination = companyGenomeParser.ParseGenome(evolutionResult.FinalGenome).ToList();

            stopwatch.Stop();

            foreach (Company company in bestCompaniesCombination)
            {
                int companySharesToBuy = companiesFitnessCalculator.GetCompanySharesToBuy(company);
                Console.WriteLine($"x{companySharesToBuy}\t{company}");
            }

            Console.WriteLine($"\nWallet fitness: {evolutionResult.Fitness}");
            Console.WriteLine($"Alghorithm generations count: {evolutionResult.GenerationIndex}");
            Console.WriteLine($"Alghorithm performance time: {stopwatch.Elapsed}");
        }

        private static Config LoadConfig()
        {
            string appSettingsPath = "AppConfig.json";
            if (!File.Exists(appSettingsPath))
            {
                throw new FileNotFoundException("Cannot found AppSettings.json in app root directory");
            }

            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(appSettingsPath));
            return config;
        }
    }
}
