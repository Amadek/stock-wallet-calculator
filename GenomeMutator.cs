using System;
using System.Text;

namespace StockWalletCalculator
{
    public class GenomeMutator
    {
        private readonly Random _random = new Random();

        public string MutateGenome(string genome)
        {
            // Negacja losowego bita.
            int randomGenomeIndex = _random.Next(0, genome.Length);
            StringBuilder genomeBuilder = new StringBuilder(genome);
            genomeBuilder[randomGenomeIndex] = genomeBuilder[randomGenomeIndex] == '1' ? '0' : '1';

            return genomeBuilder.ToString();
        }
    }
}
