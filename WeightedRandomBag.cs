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
        private int _accumulatedWeight = 0;

        public void Add(TElement element, int weight)
        {
            _accumulatedWeight += weight;
            _entries.Add(new WeightedEntry<TElement> { Element = element, AccumulatedWeight = _accumulatedWeight });
        }

        public TElement GetWeghtedRandomElement()
        {
            double randomWeight = _random.NextDouble() * _accumulatedWeight;
            return _entries.FirstOrDefault(entry => entry.AccumulatedWeight >= randomWeight)?.Element; // Null powinien się pojawić tylko dla pustej kolekcji.
        }
    }
}
