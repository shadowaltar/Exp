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
            Test();
        }

        private void Test()
        {
            var secs = FileReaders.ReadSecurities(@"C:\A\Experiments\Exp\securities.csv");
            var covs = FileReaders.ReadCorrelations(@"C:\A\Experiments\Exp\correlations.csv", secs);
            var matrix = Portfolios.ComputeCovariances(covs);

            using (new ReportTime())
            {
                Dictionary<Security, double> weights;
                var variance = Portfolios.ComputePortfolioMeanVariance(matrix, 9d, out weights, true);
                Console.WriteLine(weights.Aggregate("Weights:" + Environment.NewLine,
                    (str, pair) => str + (Environment.NewLine + pair.Key.Symbol + "," + pair.Value)));
                Console.WriteLine("Variance: " + variance);
            }
        }
    }
}
