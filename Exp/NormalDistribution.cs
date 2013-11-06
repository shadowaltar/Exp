using System;
using Exp.Utils;

namespace Exp
{
    public class NormalDistribution
    {
        private static readonly NormalDistribution StandardNormalDistribution;

        static NormalDistribution()
        {
            StandardNormalDistribution = new NormalDistribution(0.0, 1.0);
            SpecialFactor = 1.0 / Math.Sqrt(6.2831853071795862);
        }

        public NormalDistribution(double mean, double stdDev)
        {
            Mean = mean;
            StdDev = stdDev;
        }

        public static double SpecialFactor { get; private set; }
        public double Mean { get; private set; }
        public double StdDev { get; private set; }

        public static NormalDistribution Standard
        {
            get { return StandardNormalDistribution; }
        }

        public static double RandomNext(Random generator)
        {
            double one;
            double w;
            do
            {
                one = 2.0 * generator.NextDouble() - 1.0;
                double two = 2.0 * generator.NextDouble() - 1.0;
                w = one * one + two * two;
            } while (w >= 1.0);
            w = Math.Sqrt(-2.0 * Math.Log(w) / w);
            return w * one;
        }

        public static double RandomNext()
        {
            double one = BetterRandom.NextDouble();
            double two = BetterRandom.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(one)) * Math.Cos(6.2831853071795862 * two);
        }

        public double Cdf(double x)
        {
            x = Normalize(x, Mean, StdDev);
            return NormalizedCdf(x);
        }

        public double Pdf(double x)
        {
            x = Normalize(x, Mean, StdDev);
            return NormalizedPdf(x) / StdDev;
        }

        public static double Normalize(double x, double mean, double stdDev)
        {
            return (x - mean) / stdDev;
        }

        public static double NormalizedCdf(double x)
        {
            double factor = SpecialFactor;
            double t = 1.0 / (1.0 + 0.2316419 * x);
            double result;
            if (x >= 0.0)
            {
                result = 1.0 -
                         ((((1.330274429 * t + -1.821255978) * t + 1.781477937) * t + -0.356563782) * t + 0.31938153) * t *
                         Math.Exp(-x * x / 2.0) * factor;
            }
            else
            {
                result = 1.0 - NormalizedCdf(-x);
            }
            return result;
        }

        public static double NormalizedPdf(double x)
        {
            return SpecialFactor * Math.Exp(-0.5 * x * x);
        }
    }
}