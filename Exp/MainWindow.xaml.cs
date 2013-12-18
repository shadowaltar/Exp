using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Exp.InstrumentTypes;
using Exp.Investments;
using Exp.Utils;
using Exp.QuantitativeFinance;
using Exp.InstrumentTypes;

namespace Exp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TestMbsCpr();
            //TestComputeAmericanPutByBinomialPricer();
            //Test();

            var mortgage = new Mortgage(360, 4200000, 0.0215, 0, 1);
            var mbs = new MortgageBackedSecurity(mortgage);
            MbsPricer.ComputeMortgage(mortgage);
        }

        private void Test()
        {
            var secs = FileReaders.ReadSecurities(@"C:\A\Experiments\Exp\securities.csv");
            var covs = FileReaders.ReadCorrelations(@"C:\A\Experiments\Exp\correlations.csv", secs);
            var matrix = Portfolios.ComputeCovariances(covs);

            using (new ReportTime())
            {
                Dictionary<Security, double> weights;
                var variance = Portfolios.ComputePortfolioMinimumVariance(matrix, 9d, out weights, true);
                Console.WriteLine(weights.Aggregate("Weights:" + Environment.NewLine,
                    (str, pair) => str + (Environment.NewLine + pair.Key.Symbol + "," + pair.Value)));
                Console.WriteLine("Variance: " + variance);
            }
        }

        private static void TestMbsCpr()
        {
            var cpr = MbsPricer.ComputeCpr(4, 3);
            Console.WriteLine(cpr == 0.014);
            var smm = MbsPricer.ComputeSmm(4, 3);
            Console.WriteLine(smm > 0.0011742 && smm < 0.0011743);
        }

        public static void TestComputeAmericanPutByBinomialPricer()
        {
            var n = 15;
            var rf = 0.02;
            var option = new Option
            {
                Type = OptionType.Put,
                Underlying = new Security { Volatility = 0.3, MarketPrice = 100, YieldRate = 0.01 },
                Strike = 110,
                TimeToMaturity = 0.25,
            };
            using (ReportTime.Start())
            {
                BinomialPricer.ComputeOption(option, rf, n);
                Console.WriteLine(option.FairPrice >= 12.359);
                Console.WriteLine(option.FairPrice <= 12.360);
            }
        }
    }
}
