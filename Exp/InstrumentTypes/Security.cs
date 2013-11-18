namespace Exp.InstrumentTypes
{
    public class Security
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public double MarketPrice { get; set; }
        public double FairPrice { get; set; }

        public double Volatility { get; set; }
        public double YieldRate { get; set; }
    }
}