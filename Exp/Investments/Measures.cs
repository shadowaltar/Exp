using System;
using System.Collections.Generic;
using System.Linq;
using Exp.Maths;

namespace Exp.Investments
{
    public class Measures
    {
        public double GetReturn(double start, double end)
        {
            return (end - start) / start;
        }

        public double GetSharpeRatio(IList<double> values, IList<double> benchmarkRates)
        {
            if (values.Count != benchmarkRates.Count)
                throw new ArgumentException();
            var diffs = values.Select((t, i) => t - benchmarkRates[i]).ToList();
            return diffs.Average() / diffs.StandardDeviation();
        }

        public double GetSharpeRatio(IList<double> values, double benchmarkRate)
        {
            return (values.Average() - benchmarkRate) / values.StandardDeviation();
        }
    }
}