namespace Exp.InstrumentTypes
{
    public class Swap : Security
    {
        public double Tenor { get; set; }
    }

    public class PlainVanillaInterestRateSwap : Swap
    {
        public double FloatRate { get; set; }
        public double FixedRate { get; set; }
        public double YearlyFixedPaymentCount { get; set; }
        public double YearlyFloatPaymentCount { get; set; }
    }
}