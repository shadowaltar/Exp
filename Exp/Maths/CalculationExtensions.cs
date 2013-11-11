using System;

namespace Exp.Maths
{
    public static class CalculationExtensions
    {
        public static bool ApproxEquals(this double a, double b)
        {
            return Math.Abs(a - b) <= Calculation.DoubleEpsilon;
        }
    }
}