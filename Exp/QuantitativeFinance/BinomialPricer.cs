using System;
using Exp.InstrumentTypes;

namespace Exp.QuantitativeFinance
{
    public class BinomialPricer
    {
        public static double ComputeEuropeanOptionPrice(Option option,
            double interestRate, int periods)
        {
            var t = option.TimeToMaturity;
            var r = interestRate;
            var u = Math.Exp(option.Underlying.Volatility * Math.Sqrt(t / periods)); // u=e^(sig*sqrt(t))
            var d = 1 / u;

            var disc = Math.Pow(Math.Exp(-r * t / periods), periods); //1 / Math.Pow(1 + r, periods);
            var s0 = option.Underlying.MarketPrice;
            var p = ((r + 1) - d) / (u - d);

            var result = 0.0;
            for (int i = 0; i <= periods; i++)
            {
                var st = s0 * Math.Pow(d, i) * Math.Pow(u, periods - i);
                var callValue = option.Payoff(st);

                var prob = Math.Pow(p, periods - i) * Math.Pow(1 - p, i);
                var coeff = Algorithms.BinomialCoefficient(periods, i);
                result += (coeff * prob * callValue);
            }
            result *= disc;
            return result;
        }
    }
}