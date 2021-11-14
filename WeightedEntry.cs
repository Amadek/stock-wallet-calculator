using System;
using System.Collections.Generic;
using System.Text;

namespace StockWalletCalculator
{
    public class WeightedEntry<TElement>
    {
        public TElement Element { get; set; }

        public int AccumulatedWeight { get; set; }
    }
}
