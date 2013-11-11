namespace Exp.InstrumentTypes
{
    public class Security
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public double Price { get; set; }
        public double Volatility { get; set; }
        public double YieldRate { get; set; }
    }
}