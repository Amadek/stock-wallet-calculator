using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockWalletCalculator
{
    class GenerationsRunner
    {
        private readonly GenomeGenerator _genomeGenerator;
        private readonly GenomeCrossover _genomeCrossover;
        private readonly GenomeMutator _genomeMutator;
        private readonly IFitnessCalculator _fitnessCalculator;

        public GenerationsRunner(
            GenomeGenerator genomeGenerator,
            GenomeCrossover genomeCrossover,
            GenomeMutator genomeMutator,
            IFitnessCalculator fitnessCalculator)
        {
            _genomeGenerator = genomeGenerator;
            _genomeCrossover = genomeCrossover;
            _genomeMutator = genomeMutator;
            _fitnessCalculator = fitnessCalculator;
        }

        public string RunGenerations(int generationLimit, decimal fitnessLimit)
        {
            List<Genome> oldGeneration = _genomeGenerator.GenerateGeneration()
                .Select(g => new Genome { Value = g, Fitness = _fitnessCalculator.Calculate(_genomeGenerator.ParseGenome(g).ToArray()) })
                .OrderByDescending(g => g.Fitness)
                .ToList();

            Genome bestGenome = null;

            for (int generationIndex = 0; generationIndex < generationLimit; generationIndex++)
            {
                List<Genome> newGeneration = oldGeneration.Take(2).ToList();
                oldGeneration = oldGeneration.Skip(2).ToList();

                for (int genomeIndex = 0; genomeIndex < oldGeneration.Count / 2; genomeIndex++)
                {
                    string[] crossedGenomes = _genomeCrossover.CrossoverGenomes(oldGeneration[genomeIndex].Value, oldGeneration[genomeIndex + 1].Value).ToArray();
                    foreach (string crossedGenome in crossedGenomes)
                    {
                        string mutatedGenome = _genomeMutator.MutateGenome(crossedGenome);
                        Genome newGenome = new Genome { Value = mutatedGenome, Fitness = _fitnessCalculator.Calculate(_genomeGenerator.ParseGenome(mutatedGenome)) };
                        newGeneration.Add(newGenome);
                    }
                }

                bestGenome = newGeneration.OrderByDescending(g => g.Fitness).First();
                if (bestGenome.Fitness >= fitnessLimit)
                {
                    return bestGenome.Value;
                }

                oldGeneration = newGeneration.OrderByDescending(g => g.Fitness).ToList();
            }

            return bestGenome.Value;
        }
    }
}
