using System;
using System.Collections.Generic;
using System.Linq;
using Exp.Maths;

namespace Exp.Finance
{
    public class Measures
    {
        public double GetReturn(double start, double end)
        {
            return (end - start) / start;
        }

        public double GetSharpeRatio(IList<double> values, IList<double> benchmarks)
        {
            if (values.Count != benchmarks.Count)
                throw new ArgumentException();
            var diffs = values.Select((t, i) => t - benchmarks[i]).ToList();
            return diffs.Average() / Calculation.StandardDeviation(diffs);
        }
    }
}