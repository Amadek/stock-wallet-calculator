using System;
using System.Collections.Generic;
using System.Text;

namespace StockWalletCalculator
{
    interface IGenomeGenerator
    {
        public IEnumerable<string> GenerateGeneration();
    }
}
