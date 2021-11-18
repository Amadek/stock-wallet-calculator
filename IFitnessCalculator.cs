
using System.Collections.Generic;

namespace StockWalletCalculator
{
    /// <summary>
    /// Iface jakości kombinacji obiektów z genomu, im wyższy wynik, tym lepsza kombinacja.
    /// </summary>
    public interface IFitnessCalculator
    {
        public decimal Calculate(IEnumerable<object> objects);
    }
}
