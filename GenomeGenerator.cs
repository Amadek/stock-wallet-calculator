using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    class GenomeGenerator : IGenomeGenerator
    {
        private readonly Random _random = new Random();
        private const int _generationPopulation = 16;
        private readonly int _genomeLength;

        public GenomeGenerator(int genomeLength)
        {
            _genomeLength = genomeLength;
        }

        public IEnumerable<string> GenerateGeneration()
        {
            for (int generationIndex = 0; generationIndex < _generationPopulation; generationIndex++)
            {
                yield return this.GenerateGenome();
            }
        }

        private string GenerateGenome()
        {
            string genome = string.Empty;

            for (int genomeBitIndex = 0; genomeBitIndex < _genomeLength; genomeBitIndex++)
            {
                genome += _random.Next(2) == 1 ? '1' : '0';
            }

            return genome;
        }
    }
}
