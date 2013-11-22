using System;
using System.Collections.Generic;
using System.Linq;
using Exp.InstrumentTypes;
using Exp.Investments;
using Exp.Maths;
using Exp.QuantitativeFinance;
using Exp.Utils;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestMethod1()
        {
            var numbers = new List<double> { 3, 3, 3, 3, 1, 1, 1, 1, 4 };
            var m = numbers.Mode();
            Assert.AreEqual(m, 3d);
        }

        [Test]
        public void TestPayerSwaption()
        {
            var swap = new PlainVanillaInterestRateSwap();
            swap.Tenor = 4;
            swap.YearlyFloatPaymentCount = 2;
            swap.FloatRate = 0.07;
            swap.FixedRate = 0.075;

            var swaption = new Swaption(swap);
            swaption.Underlying = swap;
            swaption.Strike = swap.FixedRate;
            swaption.TimeToMaturity = 2;
            swaption.Volatility = 0.2;

            var r = 0.06;

            BlackScholesPricer.ComputeEuropeanPayerSwaptionPrice(swaption, r);
        }

        [Test]
        public void TestVasicekModel()
        {
            var r = 0.05;
            var speed = 0.15;
            var sig = 0.01;
            var model = new VasicekModel(speed, r, sig);
            model.TimeToMaturity = 5;

            var rate = model.SpotRate;
            var price = model.PresentValue;
            var yieldSig = model.YieldVolatility;

            model.TimeToMaturity = 1;
            rate = model.SpotRate;
            price = model.PresentValue;
            yieldSig = model.YieldVolatility;
        }

        [Test]
        public void TestVarianceMatrix()
        {
            var secs = FileReaders.ReadSecurities(@"securities.csv");
            var covs = FileReaders.ReadCorrelations(@"correlations.csv", secs);

            using (new ReportTime())
            {
                Dictionary<Security, double> weights;
                var expectedReturn = 9d;
                var matrix = Portfolios.ComputeCovariances(covs);
                var variance = Portfolios.ComputePortfolioMinimumVariance(matrix, expectedReturn, out weights);


                Console.WriteLine(weights.Aggregate("Weights:" + Environment.NewLine,
                    (str, pair) => str + (Environment.NewLine + pair.Key.Symbol + "," + pair.Value)));
                Console.WriteLine("ExpectedReturn: " + expectedReturn);
                Console.WriteLine("Variance: " + variance);
                Console.WriteLine("Sharpe: " + expectedReturn / Math.Sqrt(variance));
            }
        }

        [Test]
        public void TestAnnuity()
        {
            // single payment upfront, get FV.
            var n = 6;
            double notional = 10e7;
            double r = 0.092;
            var t1 = TimeValues.ComputeFutureValueOfOneTimeInvestment(notional, n, r);

            // same but paying semiannually
            var m = 2;
            var t2 = TimeValues.ComputeFutureValueOfOneTimeInvestment(notional, n, m, r);

            // FV of annuity
            var pmt = 2 * 10e6;
            r = 0.08;
            n = 15;
            var t3 = TimeValues.ComputeFutureValueOfCashFlows(pmt, n, r);

            // PV of annuity
            var fv = 5 * 10e6;
            r = 0.1;
            n = 7;
            var t4 = TimeValues.ComputePresentValue(fv, n, r);

            // PV of CFs
            var pmts = new List<double> { 100, 100, 100, 100, 1100 };
            r = .0625;
            var t5 = TimeValues.ComputePresentValueOfCashFlows(pmts, r);

            pmt = 100;
            r = .09;
            n = 8;
            var t6 = TimeValues.ComputePresentValueOfCashFlows(100, n, r);

            var couponRate = 0.05;
            var faceValue = 1000;
            var years = 20;
            r = .11;
            m = 2;
            var bond = new Bond(faceValue, years, m, couponRate);
            BondPricer.ComputePrice(bond, r);
            var t7 = bond.FairPrice;

        }

        [Test]
        public void TestComputeMinVarianceHedgingFuturesCount()
        {
            var orangeFutures = new Security { Id = 1, MarketPrice = 118.65, Symbol = "ORANGE_FUTURES", Volatility = 0.2 };
            var grapefruitCommodity = new Security { Id = 1, MarketPrice = double.NaN, Symbol = "GRAPEFRUIT_COMMODITY", Volatility = 0.25 };
            var secCov = new SecuritiesCovariance(orangeFutures, grapefruitCommodity, 0.7);
            secCov.Compute();
            var contractCount = Hedges.ComputeMinVarianceHedgingFuturesCount(Math.Pow(orangeFutures.Volatility, 2), secCov, 150000, 15000);
            Console.WriteLine(contractCount);
        }
    }
}
