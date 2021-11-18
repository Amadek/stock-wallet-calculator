namespace StockWalletCalculator
{
    /// <summary>
    /// Spółka w portfelu inwestycyjnym, zawiera nazwę, procent w portfelu i aktualną cenę za akcję.
    /// </summary>
    public class Company
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Percent { get; set; }

        public override string ToString()
        {
            return $"{this.Name}\t{this.Price}\t{this.Percent * 100}%";
        }
    }
}
