using System;
using System.Collections.Generic;
using Exp.InstrumentTypes;
using NUnit.Framework;

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

        public static double Compute(Option option, List<double> interestRates)
        {
            var n = interestRates.Count;
            var maturity = option.TimeToMaturity;
            var style = option.Style;
            var payoffFunction = new Func<double, double>(option.Payoff);
            var root = new BinomialLatticeNode();

            root.UnderlyingValue = option.Underlying.MarketPrice;
            root.UpRatioFormula = sec =>
            {
                var o = (Option)sec;
                return Math.Exp(o.Underlying.Volatility * Math.Sqrt(maturity / n));
            };
            root.DownRatioFormula = sec => 1 / root.ChildUpPossibility;

            root.UpProbabilityFormula = sec =>
            {
                var 
            }

            throw new NotImplementedException();
        }

        private class BinomialLattice
        {

        }

        private class BinomialLatticeNode
        {
            public int Stage { get; set; }
            public int Index { get; set; }

            public List<double> InterestRates { get; set; } 

            public BinomialLattice ParentUp { get; set; }
            public BinomialLattice ParentDown { get; set; }
            public double PreviousStageInterestRate { get; set; }

            public BinomialLattice ChildUp { get; set; }
            public BinomialLattice ChildDown { get; set; }
            public double NextStageInterestRate { get; set; }
            public double ChildUpPossibility { get; set; }
            public double ChildDownPossibility { get; set; }

            public double UnderlyingValue { get; set; }
            public double DerivedValue { get; set; }

            public Func<Security, double> UpProbabilityFormula { get; set; }
            public Func<Security, double> DownProbabilityFormula { get; set; }

            public Func<Security, double> UpRatioFormula { get; set; }
            public Func<Security, double> DownRatioFormula { get; set; }
        }
    }
}