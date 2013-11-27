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


        public static double Compute(Option option, double interestRate, int periods)
        {
            return Compute(option, interestRate.Repeat(periods));
        }

        public static double Compute(Option option, List<double> interestRates,
            List<double> volatilities = null, List<double> underlyingYieldRates = null)
        {
            var n = interestRates.Count;
            var maturity = option.TimeToMaturity;
            var dt = maturity / n;
            var style = option.Style;
            var strike = option.Strike;

            if (volatilities == null)
            {
                volatilities = option.Underlying.Volatility.Repeat(n);
            }
            if (underlyingYieldRates == null)
            {
                underlyingYieldRates = option.Underlying.YieldRate.Repeat(n);
            }

            var lattice = new BinomialLattice(n);

            lattice.InterestRates = interestRates;
            lattice.UnderlyingVolatilities = volatilities;
            lattice.UnderlyingYieldRates = underlyingYieldRates;

            var node = new BinomialLatticeNode
            {
                Stage = 0,
                Index = 0,
                UnderlyingValue = option.Underlying.MarketPrice
            };

            lattice.Root = node;

            // from node to leaves; excludes root node
            for (int stage = 0; stage <= n; stage++)
            {
                var tempIndex = stage == n ? stage - 1 : stage;
                var r = lattice.InterestRates[tempIndex];
                var q = lattice.UnderlyingYieldRates[tempIndex];
                var sig = lattice.UnderlyingVolatilities[tempIndex];
                var u = Math.Exp(sig * Math.Sqrt(dt));
                var d = 1 / u;
                var p = (Math.Exp((r - q) * dt) - d) / (u - d);

                var lowerNodeFactor = d / u;

                // compute current stage
                for (int i = 0; i <= stage; i++)
                {
                    lattice[stage][i].Stage = stage;
                    lattice[stage][i].Index = i;
                    lattice[stage][i].UpRatio = u;
                    lattice[stage][i].DownRatio = d;
                    lattice[stage][i].UpRiskNeutralProbability = p;
                    lattice[stage][i].DownRiskNeutralProbability = 1 - p;
                }

                if (stage == n)
                    continue;

                // compute next stage
                lattice[stage + 1][0] = new BinomialLatticeNode
                {
                    UnderlyingValue = lattice[stage][0].UnderlyingValue * lattice[stage][0].UpRatio
                };
                for (int i = 1; i <= stage + 1; i++)
                {
                    lattice[stage + 1][i] = new BinomialLatticeNode
                    {
                        UnderlyingValue = lattice[stage + 1][i - 1].UnderlyingValue * lowerNodeFactor
                    };
                }
            }

            // process the leaves
            for (int i = 0; i <= lattice.Dimension; i++)
            {
                node = lattice.Get(lattice.Dimension, i);
                var exerciseValue = option.Type == OptionType.Call
                    ? node.UnderlyingValue - strike
                    : strike - node.UnderlyingValue;
                node.DerivedValue = Math.Max(exerciseValue, 0);
            }

            // from last 2nd level leaves to nodes; includes root node
            for (int stage = n - 1; stage >= 0; stage--)
            {
                var disc = Math.Exp(-lattice.InterestRates[stage] * dt);
                var pUp = node.UpRiskNeutralProbability;
                var pDown = node.DownRiskNeutralProbability;
                for (int i = 0; i < lattice[stage].Length; i++)
                {
                    node = lattice[stage][i];
                    var childUp = lattice[stage + 1][i]; // up child
                    var childDown = lattice[stage + 1][i + 1]; // down child
                    node.DerivedValue = disc * (pUp * childUp.DerivedValue + pDown * childDown.DerivedValue);
                    if (style == OptionStyleType.American)
                    {
                        var exerciseValue = option.Type == OptionType.Call
                            ? node.UnderlyingValue - strike
                            : strike - node.UnderlyingValue;
                        node.DerivedValue = Math.Max(exerciseValue, node.DerivedValue);
                    }
                }
            }

            return lattice.Root.DerivedValue;
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
            public List<double> UnderlyingYieldRates { get; set; }

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
            /// <summary>
            /// The index of period, starting from 0 (root) to n (period count).
            /// </summary>
            public int Stage { get; set; }
            /// <summary>
            /// The index of a node of a period. For the nodes of a period, the indices
            /// are ascending from top to bottom. Hence the index will never be larger
            /// than its <see cref="Stage"/>.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// The factor p (risk neutral probability) by the inputs of the same stage:
            /// probability that the parent node goes up to this node.
            /// </summary>
            public double UpRiskNeutralProbability { get; set; }
            /// <summary>
            /// The factor 1-p (p is risk neutral probability) by the inputs of the same stage:
            /// probability that the parent node goes down to this node.
            /// </summary>
            public double DownRiskNeutralProbability { get; set; }

            /// <summary>
            /// The factor u by the inputs of the same stage.
            /// </summary>
            public double UpRatio { get; set; }
            /// <summary>
            /// The factor d by the inputs of the same stage.
            /// </summary>
            public double DownRatio { get; set; }

            /// <summary>
            /// The underlying value.
            /// </summary>
            public double UnderlyingValue { get; set; }
            /// <summary>
            /// The derived entity's value based on the underlying value calculated backwards.
            /// For example an option based on a stock, or a bond basd on interest rate or cash.
            /// </summary>
            public double DerivedValue { get; set; }

            public override string ToString()
            {
                return string.Format("({0}, {1})", UnderlyingValue.ToString("N4"), DerivedValue.ToString("N4"));
            }
        }
    }
}