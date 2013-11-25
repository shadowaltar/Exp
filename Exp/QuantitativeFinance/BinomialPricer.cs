using System;
using System.Collections.Generic;
using Exp.InstrumentTypes;
using NUnit.Framework;

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