﻿using System;
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
            CompanyGenomeParser companyGenomeParser = new CompanyGenomeParser(companyVariantsGenerator.Generate());
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
