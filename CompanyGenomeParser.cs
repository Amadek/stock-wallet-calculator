using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockWalletCalculator
{
    class CompanyGenomeParser
    {
        private readonly List<Dictionary<string, Company>> _genomeEntries = new List<Dictionary<string, Company>>();
        private readonly int _genomeVariantBinaryLength;

        /// <summary>
        /// Zwraca długość genomu. Jest to ilość firm * długość postaci binarnej firmy.
        /// </summary>
        public int GenomeLength => _genomeEntries.Count * _genomeVariantBinaryLength;

        /// <summary>
        /// Przyjmuje macierz możliwych wariantów obiektów, które tworzą genom.
        /// Np.
        /// [
        ///     [INTEL 3%, INTEL 4%],
        ///     [AFG 9%, AFG 10%],
        ///     itd...
        /// ]
        /// Przy tworzeniu genomu wybierany jest tylko jeden wariant obiektu z podrzędnej tablicy.
        /// </summary>
        /// <param name="companyVariantsMatrix">Matryca wariantów firm</param>
        public CompanyGenomeParser(IEnumerable<IEnumerable<Company>> companyVariantsMatrix)
        {
            //TODO przydałaby się walidacja argumentu

            // Zapisanie długości części genomu odpowiadającego tylko za jeden wariant obiektu.
            // Bierzemy szerokość (ilość wariantów obiektu) matrycy i wyliczamy na jej podstawie logarytm z dwójki co określi długość części genomu w postaci binarnej.
            // Przez to działanie, szerokość matrycy (ilość wariantów obiektu) musi być potęgą dwójki - może być 1, 2, 4, 8, ... wariantów tego samego obiektu.
            _genomeVariantBinaryLength = (int)Math.Log2(companyVariantsMatrix.First().Count());

            // Wyliczanie binarnego przedstowienia każdego z wariantów każdego z obiektów.
            foreach (IEnumerable<Company> companyVariants in companyVariantsMatrix)
            {
                int companyVariantIndex = 0;
                Dictionary<string, Company> companyVariantsDictionary = new Dictionary<string, Company>();
                foreach (Company companyVariant in companyVariants)
                {
                    string companyBinaryRepresentation = string.Format("{0:D" + _genomeVariantBinaryLength + "}", int.Parse(Convert.ToString(companyVariantIndex++, 2)));
                    companyVariantsDictionary[companyBinaryRepresentation] = companyVariant;
                }
                _genomeEntries.Add(companyVariantsDictionary);
            }
        }

        public IEnumerable<Company> ParseGenome(string genome)
        {
            for (int genomePartIndex = 0, genomeEntryIndex = 0;
                genomePartIndex < genome.Length;
                genomePartIndex += _genomeVariantBinaryLength, genomeEntryIndex++)
            {
                // Wydzielenie części genomu odpowiedzialnej za pojedynczy wariant firmy.
                string genomePart = genome.Substring(genomePartIndex, _genomeVariantBinaryLength);
                // Zwrócenie wariantu firmy, której postać binarna jest pobraną częścią genomu.
                yield return _genomeEntries[genomeEntryIndex][genomePart];
            }
        }
    }
}
