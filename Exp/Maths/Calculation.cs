using System;
using System.Collections.Generic;
using System.Linq;

namespace Exp.Maths
{
    public static class Calculation
    {
        public static double DoubleEpsilon = 0.00000001;

        public static double DefiniteIntegralTrapezoidal(Func<double, double> func, double a, double b, int n = 100000)
        {
            double h = (b - a) / n;
            double result = 0.0;
            for (int i = 0; i <= n; i++)
            {
                double x = a + i * h;
                double y = func(x);
                if (i == 0 || i == n)
                {
                    result += y;
                }
                else
                {
                    result += 2.0 * y;
                }
            }
            return result * (h / 2.0);
        }

        public static double DefiniteIntegralSimpson(Func<double, double> func, double a, double b)
        {
            return (b - a) / 6.0 * (func(a) + 4.0 * func((a + b) / 2.0) + func(b));
        }

        public static int GreatestCommonDivisor(int a, int b)
        {
            int result;
            if (b == 0)
            {
                result = a;
            }
            else
            {
                int r = a % b;
                result = GreatestCommonDivisor(b, r);
            }
            return result;
        }

        public static bool IsPrime(this int value)
        {
            bool result;
            if (value < 2)
            {
                result = false;
            }
            else
            {
                int i = 2;
                while (i * i < value)
                {
                    if (value % i == 0)
                    {
                        return false;
                    }
                    i++;
                }
                result = true;
            }
            return result;
        }

        public static bool AreRelativelyPrime(int a, int b)
        {
            return GreatestCommonDivisor(a, b) == 1;
        }

        public static double Hypotenuse(double a, double b)
        {
            return Math.Sqrt(a * a + b * b);
        }

        public static double HarmonicNumber(this int n)
        {
            double result = 0.0;
            for (int i = 1; i <= n; i++)
            {
                result += 1.0 / i;
            }
            return result;
        }

        public static decimal Sqrt(this decimal value, decimal epsilon = 0.0M)
        {
            if (value < 0) throw new OverflowException("Cannot calculate square root from a negative number");

            decimal current = (decimal)Math.Sqrt((double)value), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + value / previous) / 2;
            }
            while (Math.Abs(previous - current) > epsilon);
            return current;
        }

        /// <summary>
        /// Get the median of a list of values. It will sort the list.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double Median(this List<double> values)
        {
            int len = values.Count;
            double result;
            if (len == 1)
            {
                result = values[0];
            }
            else
            {
                values.Sort();
                result = ((len % 2 == 0) ? ((values[len / 2] + values[len / 2 - 1]) / 2.0) : values[(len - 1) / 2]);
            }
            return result;
        }

        public static double Mode(this List<double> values)
        {
            if (values ==null || values.Count==0)
                throw new ArithmeticException();

            var groups = values.GroupBy(v => v);
            var maxCount = groups.Max(g => g.Count());
            return groups.First(g => g.Count() == maxCount).Key;
        }

        public static double Variance(this List<double> values)
        {
            double result;
            if (values == null)
            {
                result = double.NaN;
            }
            else
            {
                if (values.Count < 2)
                {
                    result = 0.0;
                }
                else
                {
                    double average = values.Sum() / values.Count;
                    double diffSquaredSum = 0.0;
                    foreach (double value in values)
                    {
                        double diff = value - average;
                        diffSquaredSum += diff * diff;
                    }
                    result = diffSquaredSum / values.Count;
                }
            }
            return result;
        }

        public static double Variance(this List<int> values)
        {
            double result;
            if (values == null)
            {
                result = double.NaN;
            }
            else
            {
                if (values.Count < 2)
                {
                    result = 0.0;
                }
                else
                {
                    double average = values.Sum() / (double)values.Count;
                    double diffSquaredSum = 0.0;
                    foreach (var value in values)
                    {
                        var diff = value - average;
                        diffSquaredSum += diff * diff;
                    }
                    result = diffSquaredSum / values.Count;
                }
            }
            return result;
        }

        public static decimal Variance(this List<decimal> values)
        {
            decimal result;
            if (values.Count < 2)
            {
                result = 0M;
            }
            else
            {
                decimal average = values.Sum() / values.Count;
                decimal diffSquaredSum = 0M;
                foreach (var value in values)
                {
                    var diff = value - average;
                    diffSquaredSum += diff * diff;
                }
                result = diffSquaredSum / values.Count;
            }
            return result;
        }

        public static double StandardDeviation(this List<double> values)
        {
            double r = Variance(values);
            return double.IsNaN(r) ? double.NaN : Math.Sqrt(r);
        }

        public static double StandardDeviation(this List<int> values)
        {
            double r = Variance(values);
            return Math.Sqrt(r);
        }

        public static decimal StandardDeviation(this List<decimal> values)
        {
            decimal r = Variance(values);
            return r.Sqrt();
        }

        /// <summary>
        /// Get the skewness of a list of values. It will sort the list.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double Skewness(this List<double> values)
        {
            values.Sort();
            var mean = values.Average();
            var median = values.Median();
            var stddev = values.StandardDeviation();
            return 3 * (mean - median) / stddev;
        }
    }
}