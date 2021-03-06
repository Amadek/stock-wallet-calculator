using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    class EvolutionRunner
    {
        private readonly GenomeGenerator _genomeGenerator;
        private readonly GenomeCrossover _genomeCrossover;
        private readonly GenomeMutator _genomeMutator;
        private readonly IFitnessCalculator _fitnessCalculator;

        public EvolutionRunner(
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

        public EvolutionResult Evolve(int generationLimit, decimal fitnessLimit)
        {
            List<GenomeWithFitness> oldGeneration = _genomeGenerator.GenerateGeneration()
                .Select(g => new GenomeWithFitness { Genome = g, Fitness = _fitnessCalculator.Calculate(g) })
                .OrderByDescending(g => g.Fitness)
                .ToList();

            GenomeWithFitness bestGenome = null;
            int generationIndex;

            for (generationIndex = 0; generationIndex < generationLimit; generationIndex++)
            {
                List<GenomeWithFitness> newGeneration = oldGeneration.Take(2).ToList();
                oldGeneration = oldGeneration.Skip(2).ToList();

                for (int genomeIndex = 0; genomeIndex < oldGeneration.Count / 2; genomeIndex++)
                {
                    string[] crossedGenomes = _genomeCrossover.CrossoverGenomes(oldGeneration[genomeIndex].Genome, oldGeneration[genomeIndex + 1].Genome).ToArray();
                    foreach (string crossedGenome in crossedGenomes)
                    {
                        string mutatedGenome = _genomeMutator.MutateGenome(crossedGenome);
                        GenomeWithFitness newGenome = new GenomeWithFitness { Genome = mutatedGenome, Fitness = _fitnessCalculator.Calculate(mutatedGenome) };
                        newGeneration.Add(newGenome);
                    }
                }

                bestGenome = newGeneration.OrderByDescending(g => g.Fitness).First();
                if (bestGenome.Fitness >= fitnessLimit)
                {
                    return new EvolutionResult
                    {
                        FinalGenome = bestGenome.Genome,
                        Fitness = bestGenome.Fitness,
                        GenerationIndex = generationIndex
                    };
                }

                oldGeneration = newGeneration.OrderByDescending(g => g.Fitness).ToList();
            }

            return new EvolutionResult
            {
                FinalGenome = bestGenome.Genome,
                Fitness = bestGenome.Fitness,
                GenerationIndex = generationIndex
            };
        }

        private class GenomeWithFitness
        {
            public string Genome { get; set; }

            public decimal Fitness { get; set; }
        }
    }
}
