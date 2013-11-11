using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exp;
using Exp.InstrumentTypes;
using Exp.Investments;
using Exp.Maths;
using Exp.QuantitativeFinance;
using Exp.Utils;
using Moq;
using NUnit;
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
            Assert.Equals(m, 1d);
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
            var secs = FileReaders.ReadSecurities(@"C:\A\Experiments\Exp\securities.csv");
            var covs = FileReaders.ReadCorrelations(@"C:\A\Experiments\Exp\correlations.csv", secs);

            using (new ReportTime())
            {
                Dictionary<Security, double> weights;
                var expectedReturn = 9d;
                var matrix = Portfolios.ComputeCovariances(covs);
                var variance = Portfolios.ComputePortfolioMeanVariance(matrix, expectedReturn, out weights);


                Console.WriteLine(weights.Aggregate("Weights:" + Environment.NewLine,
                    (str, pair) => str + (Environment.NewLine + pair.Key.Symbol + "," + pair.Value)));
                Console.WriteLine("ExpectedReturn: " + expectedReturn);
                Console.WriteLine("Variance: " + variance);
                Console.WriteLine("Sharpe: " + expectedReturn / Math.Sqrt(variance));
            }
        }
    }
}
