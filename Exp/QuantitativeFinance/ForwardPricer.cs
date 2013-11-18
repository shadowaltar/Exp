using System;
using Exp.InstrumentTypes;

namespace Exp.QuantitativeFinance
{
    public class ForwardPricer
    {
        public static double ComputeForwardPrice(Forward forward, double interestRate)
        {
            var s = forward.Underlying.MarketPrice;
            var q = forward.Underlying.YieldRate;
            var r = interestRate - q;
            var t = forward.Tenor;
            return s * Math.Exp(r * t);
        }
    }
}