using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockWalletCalculator
{
    class GenomeGenerator
    {
        private const int _generationPopulation = 16;

        private readonly Random _random = new Random();
        private readonly IFitnessCalculator _fitnessCalculator;
        private readonly GenomeEntryVariant[][] _genomeEntries;
        private readonly int _genomeVariantBinaryLength;

        /// <summary>
        /// Przyjmuje macierz możliwych wariantów obiektów, które tworzą genom.
        /// Np. [ [INTEL 3%, INTEL 4%], [AFG 9%, AFG 10%] ].
        /// Przy tworzeniu genomu wybierany jest tylko jeden wariant obiektu z podrzędnej tablicy.
        /// </summary>
        /// <param name="objectVariants">Matryca wariantów obiektów</param>
        /// <param name="fitnessCalculator">Kalkulator jakości kombinacji obiektów</param>
        public GenomeGenerator(object[][] objectVariantsMatrix, IFitnessCalculator fitnessCalculator)
        {
            _fitnessCalculator = fitnessCalculator;

            // Mapowanie przekazanych obiektów na możliwe warianty obiektów w genomie.
            _genomeEntries = objectVariantsMatrix
                .Select(objectVariants => objectVariants.Select(objectVariant => new GenomeEntryVariant { StoredObject = objectVariant }).ToArray())
                .ToArray();

            // Zapisanie długości części genomu odpowiadającego tylko za jeden wariant obiektu.
            // Bierzemy szerokość (ilość wariantów obiektu) matrycy i wyliczamy na jej podstawie logarytm z dwójki co określi długość części genomu w postaci binarnej.
            // Przez to działanie, szerokość matrycy (ilość wariantów obiektu) musi być potęgą dwójki - może być 1, 2, 4, 8, ... wariantów tego samego obiektu.
            _genomeVariantBinaryLength = (int)Math.Log2(_genomeEntries[0].Length);

            // Wyliczanie binarnego przedstowienia każdego z wariantów każdego z obiektów.
            foreach (GenomeEntryVariant[] entryVariants in _genomeEntries)
            {
                int entryVariantIndex = 0;
                foreach (GenomeEntryVariant entryVariant in entryVariants)
                {
                    string binaryRepresentation = string.Format("{0:D" + _genomeVariantBinaryLength + "}", int.Parse(Convert.ToString(entryVariantIndex++, 2)));
                    entryVariant.BinaryRepresentation = binaryRepresentation;
                }
            }
        }

        public Genome GenerateGenome()
        {
            Genome genome = new Genome { Value = string.Empty };

            foreach (GenomeEntryVariant[] entryVariants in _genomeEntries)
            {
                // Do genomu dodajemy losowy wariant obiektu A. Dla kolejnego przebiegu robimy tak dla kolejnego obiektu B, itd...
                genome.Value += entryVariants[_random.Next(0, entryVariants.Length)].BinaryRepresentation;
            }

            object[] objectsCombination = this.ParseGenome(genome.Value).ToArray();

            genome.Fitness = _fitnessCalculator.Calculate(objectsCombination);
            return genome;
        }

        public IEnumerable<Genome> GenerateGeneration()
        {
            foreach (int _ in Enumerable.Range(0, _generationPopulation))
            {
                yield return this.GenerateGenome();
            }
        }

        public IEnumerable<object> ParseGenome(string genome)
        {
            for (int genomePartIndex = 0, genomeEntryIndex = 0; 
                genomePartIndex < genome.Length;
                genomePartIndex += _genomeVariantBinaryLength, genomeEntryIndex++)
            {
                // Wydzielenie części genomu odpowiedzialnej za pojedynczy wariant elementu.
                string genomePart = genome.Substring(genomePartIndex, _genomeVariantBinaryLength);
                // Zwrócenie wariantu elementu, którego postać binarna jest pobraną częścią genomu.
                yield return _genomeEntries[genomeEntryIndex].First(entry => entry.BinaryRepresentation == genomePart).StoredObject;
            }
        }

        private class GenomeEntryVariant
        {
            public object StoredObject { get; set; }

            public string BinaryRepresentation { get; set; }
        }
    }

    public class Genome
    {
        public string Value { get; set; }

        public decimal Fitness { get; set; }
    }
}
