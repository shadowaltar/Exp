using System;
using Exp.InstrumentTypes;

namespace Exp.QuantitativeFinance
{
    public class BinomialPricer
    {
        public static double ComputeOnePeriodTreeOptionPrice(Option option,
            double interestRate, int periods, double childUpNode = 0, double childDownNode = 0)
        {
            var t = option.TimeToMaturity;
            var dt = t / periods;
            var r = interestRate;
            var u = Math.Exp(option.Underlying.Volatility * Math.Sqrt(option.TimeToMaturity)); // u=e^(sig*sqrt(t))
            var d = 1 / u;
            var dr = Math.Exp((interestRate - option.Underlying.YieldRate) * dt); // e^[(r-q)*dt]
            if (dr < d || dr > u)
                // not a no-arbitrage interest rate
                return double.NaN;
            var p = (dr - d) / (u - d);
            var s = option.Underlying.MarketPrice;

            var binomialValue = Math.Exp(-r * dt) * (p * childUpNode + (1 - p) * childDownNode);

            if (option.Style == OptionStyleType.American)
            {
                binomialValue = Math.Max(binomialValue, option.Strike);
            }

            var upPrice = s * u;
            var downPrice = s * d;
            var upPay = option.Payoff(upPrice);
            var downPay = option.Payoff(downPrice);

            throw new NotImplementedException();
        }
    }
}