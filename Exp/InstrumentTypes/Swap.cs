using System.Collections.Generic;
using System.Linq;
using Exp.Utils;

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

        public static double ComputeFixedRate(List<double> floatRateDiscountFactors)
        {
            floatRateDiscountFactors.ThrowIfNull();

            var lastDf = floatRateDiscountFactors.Last();

            return (1 - lastDf) / floatRateDiscountFactors.Sum(); // Sum(fix value) = fix * Sum(df) = 1 - lastdf = Sum(float value)
        }
    }
}