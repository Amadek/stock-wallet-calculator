using System;
using System.Collections.Generic;

namespace StockWalletCalculator
{
    public class GenomeCrossover
    {
        private readonly Random _random = new Random();

        public IEnumerable<string> CrossoverGenomes(string genomeA, string genomeB)
        {
            if (genomeA.Length != genomeB.Length) throw new NotSupportedException("Genome A and B does not have same length.");

            int randomGenomeIndex = _random.Next(0, genomeA.Length);

            yield return genomeA.Substring(0, randomGenomeIndex) + genomeB.Substring(randomGenomeIndex, genomeB.Length - randomGenomeIndex);
            yield return genomeB.Substring(0, randomGenomeIndex) + genomeA.Substring(randomGenomeIndex, genomeA.Length - randomGenomeIndex);

        }
    }
}
