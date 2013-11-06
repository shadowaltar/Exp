using System;

namespace Exp.Maths
{
    public class Processes
    {
        public static double NextStandardWiener(double rate, double sigma, double time,
            double previous = 1.0)
        {
            return previous * Math.Exp((rate - sigma * sigma * 0.5) * time
                + sigma * Math.Sqrt(time) * NormalDistribution.RandomNext());
        }
    }
}