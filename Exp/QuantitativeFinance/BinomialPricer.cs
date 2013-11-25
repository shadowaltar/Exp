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

        public static double Compute(Option option, List<double> interestRates, List<double> volatilities = null)
        {
            var n = interestRates.Count;
            var maturity = option.TimeToMaturity;
            var t = maturity / n;
            var style = option.Style;
            var strike = option.Strike;
            if (volatilities == null)
            {
                volatilities = option.Underlying.Volatility.Repeat(n);
            }

            var lattice = new BinomialLattice(n);

            lattice.InterestRates = interestRates;
            lattice.UnderlyingVolatilities = volatilities;
            lattice.DerivedValueFormulas = new Func<double, double>(option.Payoff).Repeat(n);

            var node = new BinomialLatticeNode();
            node.UnderlyingValue = option.Underlying.MarketPrice;
            lattice.Root = node;

            // from node to leaves; excludes root node
            for (int stage = 1; stage <= n; stage++)
            {
                var r = lattice.InterestRates[stage];
                var sig = lattice.UnderlyingVolatilities[stage];
                var u = Math.Exp(sig * Math.Sqrt(t));
                var d = 1 / u;
                var p = (Math.Exp(r * t) - d) / (u - d);

                var lowerNodeFactor = d / u;

                var value = lattice[stage - 1][0].UnderlyingValue * u / lowerNodeFactor; // last topmost node. since there is a *d/u, here just *u/d; so intended to have *u only.

                for (int i = 0; i < lattice[stage].Length; i++)
                {
                    lattice[stage][i] = new BinomialLatticeNode();

                    node = lattice[stage][i];
                    node.Stage = stage;
                    node.Index = i;

                    node.InterestRate = r;
                    node.Volatility = sig;

                    node.UpRatio = u;
                    node.DownRatio = d;
                    node.UpRiskNeutralProbability = p;
                    node.DownRiskNeutralProbability = 1 - p;
                    value *= lowerNodeFactor;
                    node.UnderlyingValue = value; // each index = i+1 node equals its upper node (of index i) *d/u
                }
            }

            for (int i = 0; i < lattice.Dimension; i++)
            {
                node = lattice.Get(lattice.Dimension, i);
                node.DerivedValue = lattice.DerivedValueFormulas[lattice.Dimension](node.UnderlyingValue);
            }

            // from last 2nd level leaves to nodes; includes root node
            for (int stage = n - 1; stage >= 0; stage--)
            {
                var disc = Math.Exp(-lattice.InterestRates[stage] * t);
                var pUp = node.UpRiskNeutralProbability;
                var pDown = node.DownRiskNeutralProbability;
                for (int i = 0; i < lattice[stage].Length; i++)
                {
                    node = lattice[stage][i];
                    var childUp = lattice[stage + 1][i]; // up child
                    var childDown = lattice[stage + 1][i + 1]; // down child
                    node.DerivedValue = disc * (pUp * childUp.DerivedValue + pDown * childDown.DerivedValue);
                    if (style == OptionStyleType.American)
                        node.DerivedValue = Math.Max(strike - node.UnderlyingValue, node.DerivedValue);
                }
            }

            throw new NotImplementedException();
        }

        private class BinomialLattice
        {
            public int Dimension { get; private set; }

            public BinomialLattice(int stages)
            {
                Dimension = stages;
                Nodes = new BinomialLatticeNode[stages + 1][];
                for (int i = 0; i < stages + 1; i++)
                {
                    Nodes[i] = new BinomialLatticeNode[i + 1];
                }
            }

            public BinomialLatticeNode Root
            {
                get { return Nodes[0][0]; }
                set { Nodes[0][0] = value; }
            }

            public BinomialLatticeNode[][] Nodes { get; private set; }

            public List<double> InterestRates { get; set; }
            public List<double> UnderlyingVolatilities { get; set; }

            public List<Func<double, double>> DerivedValueFormulas { get; set; }

            public BinomialLatticeNode[] this[int key]
            {
                get { return Nodes[key]; }
            }

            public BinomialLatticeNode Get(int stage, int index)
            {
                try
                {
                    return Nodes[stage][index];
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Invalid stage or index of the lattice.", e);
                }
            }
        }

        private class BinomialLatticeNode
        {
            public int Stage { get; set; }
            public int Index { get; set; }

            public double InterestRate { get; set; }
            public double Volatility { get; set; }

            public double UpRiskNeutralProbability { get; set; }
            public double DownRiskNeutralProbability { get; set; }

            public double UpRatio { get; set; }
            public double DownRatio { get; set; }

            public double UnderlyingValue { get; set; }
            public double DerivedValue { get; set; }
        }
    }
}