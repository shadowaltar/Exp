using System;
using Exp.Maths;

namespace Exp.InstrumentTypes
{
    public class Rates
    {
        public static double ComputeDiscountFactor(double rate, double periods)
        {
            if (rate.ApproxEquals(-1))
                throw new ArgumentException();
            return 1 / Math.Pow(1 + rate, periods);
        }
    }
}