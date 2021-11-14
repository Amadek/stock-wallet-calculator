using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWalletCalculator
{
    /// <summary>
    /// Zbiór elementów z określoną wagą, która określa wiekszą szansę na wylosowanie.
    /// </summary>
    /// <typeparam name="TElement">Typ przechowywanego elementu</typeparam>
    public class WeightedRandomBag<TElement> where TElement : class
    {
        private readonly Random _random = new Random();
        private readonly List<WeightedEntry<TElement>> _entries = new List<WeightedEntry<TElement>>();
        private decimal _accumulatedWeight = 0;

        public void Add(TElement element, decimal weight)
        {
            _accumulatedWeight += weight;
            _entries.Add(new WeightedEntry<TElement> { Element = element, AccumulatedWeight = _accumulatedWeight });
        }

        public TElement GetWeghtedRandomElement()
        {
            decimal randomWeight = (decimal)_random.NextDouble() * _accumulatedWeight;
            return _entries.FirstOrDefault(entry => entry.AccumulatedWeight >= randomWeight)?.Element; // Null powinien się pojawić tylko dla pustej kolekcji.
        }

        public IEnumerable<TElement> GetWeghtedRandomPair()
        {
            List<WeightedEntry<TElement>> entriesCopy = _entries.ToList();
            decimal randomWeight = (decimal)_random.NextDouble() * _accumulatedWeight;
            WeightedEntry<TElement> firstEntry = entriesCopy.First(entry => entry.AccumulatedWeight >= randomWeight); // Null powinien się pojawić tylko dla pustej kolekcji.
            entriesCopy.Remove(firstEntry); // Usuwamy żeby nie wybrać tego samego jeszcze raz.
            WeightedEntry<TElement> secondEntry = entriesCopy.First(entry => entry.AccumulatedWeight >= randomWeight);
            return new TElement[] { firstEntry.Element, secondEntry.Element };
        }
    }
}
