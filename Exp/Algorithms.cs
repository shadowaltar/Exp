using System;
using System.Linq;
using System.Collections.Generic;
using Exp.Utils;

namespace Exp
{
    public static class Algorithms
    {
        public static int BinarySearch(int input, List<int> values, bool ignoreDuplicates = false)
        {
            values.Sort();
            if (ignoreDuplicates)
            {
                List<int> tempValues = values;
                values = new List<int>();
                foreach (int tempValue in tempValues)
                {
                    if (values.Count == 0 || values[values.Count - 1] != tempValue)
                    {
                        values.Add(tempValue);
                    }
                }
            }
            int lo = 0;
            int hi = values.Count() - 1;
            int result;
            while (lo <= hi)
            {
                int mi = lo + (hi - lo) / 2;
                if (input < values[mi])
                {
                    hi = mi - 1;
                }
                else
                {
                    if (input <= values[mi])
                    {
                        result = mi;
                        return result;
                    }
                    lo = mi + 1;
                }
            }
            result = -1;
            return result;
        }

        public static int BruteForceSearch(int input, List<int> values)
        {
            int result;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == input)
                {
                    result = i;
                    return result;
                }
            }
            result = -1;
            return result;
        }

        public static void BubbleSort(List<double> values)
        {
            int count = values.Count;
            for (int i = 1; i <= count - 1; i++)
            {
                for (int j = 1; j <= count - i; j++)
                {
                    if (values[j] > values[j + 1])
                    {
                        double t = values[j];
                        values[j] = values[j + 1];
                        values[j + 1] = t;
                    }
                }
            }
        }

        public static List<T> Repeat<T>(this T value, int count)
        {
            return Enumerable.Repeat(value, count).ToList();
        }

        public static List<int> Sequence(int length)
        {
            var result = new List<int>(length);
            for (int i = 0; i < length; i++)
            {
                result.Add(i);
            }
            return result;
        }

        public static IEnumerable<int> Sequence(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                yield return i;
            }
        }

        public static List<int> Random(int length)
        {
            Random r = BetterRandom.NewRandom();
            var result = new List<int>(length);
            double multiplier = Math.Pow(10.0, Math.Floor(Math.Log10(length) + 1.0));
            for (int i = 0; i < length; i++)
            {
                result.Add(Convert.ToInt32(Math.Floor(r.NextDouble() * multiplier)));
            }
            return result;
        }

        public static List<double> RandomDoubles(int length)
        {
            Random r = BetterRandom.NewRandom();
            var result = new List<double>(length);
            double multiplier = Math.Pow(10.0, Math.Floor(Math.Log10(length) + 1.0));
            for (int i = 0; i < length; i++)
            {
                result.Add(Math.Floor(r.NextDouble() * multiplier));
            }
            return result;
        }

        public static void Reverse(List<int> values)
        {
            if (values != null && values.Count >= 2)
            {
                int len = values.Count;
                for (int i = 0; i < len; i++)
                {
                    int temp = values[i];
                    values[i] = values[len - i - 1];
                    values[len - i - 1] = temp;
                }
            }
        }

        public static int Digits(double value, int precision = 4, bool countDecimalPoint = false)
        {
            int decimalDigitCount = 0;
            if (value % 1.0 != 0.0)
            {
                value = Math.Round(value, precision);
                decimalDigitCount = value.ToString().Split(new[] { '.' })[1].Length;
                if (decimalDigitCount != 0 && countDecimalPoint)
                {
                    decimalDigitCount++;
                }
            }
            return (int)Math.Floor(Math.Log10(value) + 1.0) + decimalDigitCount;
        }

        public static double RandomInUniform(double a, double b)
        {
            Random r = BetterRandom.NewRandom();
            return r.NextDouble() * (b - a) + a;
        }

        public static double RandomInUniform(double a)
        {
            Random r = BetterRandom.NewRandom();
            return r.NextDouble() * a;
        }

        public static int RandomInUniform(int a)
        {
            Random r = BetterRandom.NewRandom();
            return (int)(r.NextDouble() * a);
        }

        public static double RandomInDiscrete(List<double> probabilities)
        {
            double r = BetterRandom.NewRandom().NextDouble();
            double sum = 0.0;
            double result;
            for (int i = 0; i < probabilities.Count; i++)
            {
                sum += probabilities[i];
                if (sum >= r)
                {
                    result = i;
                    return result;
                }
            }
            result = -1.0;
            return result;
        }

        public static double BinomialSlow(int n, int k, double p)
        {
            double result;
            if (n == 0 && k == 0)
            {
                result = 1.0;
            }
            else
            {
                if (n < 0 || k < 0)
                {
                    result = 0.0;
                }
                else
                {
                    result = (1.0 - p) * BinomialSlow(n - 1, k, p) + p * BinomialSlow(n - 1, k - 1, p);
                }
            }
            return result;
        }

        public static double Binomial(int n, int k, double p)
        {
            return BinomialCoefficient(n, k) * Math.Pow(p, k) * Math.Pow(1.0 - p, n - k);
        }

        public static double BinomialCoefficient(int n, int k)
        {
            double result;
            if (k < 0 || k > n)
            {
                result = 0.0;
            }
            else
            {
                if (k > n - k)
                {
                    k = n - k;
                }
                double c = 1.0;
                for (int i = 0; i < k; i++)
                {
                    c *= n - i;
                    c /= i + 1;
                }
                result = c;
            }
            return result;
        }

        public static void Rescale(List<double> values, double factor)
        {
            for (int i = 0; i < values.Count; i++)
            {
                int index;
                values[index = i] = values[index] * factor;
            }
        }

        public static double ComputeRootByBisection(Func<double, double> function, double leftEnd, double rightEnd,
            double tolerance = 1E-11)
        {
            double mid = (leftEnd + rightEnd) / 2.0;
            double yLeft = function(leftEnd);
            double yMid = function(mid);
            while (mid - leftEnd > tolerance)
            {
                if ((yLeft > 0.0 && yMid > 0.0) || (yLeft < 0.0 && yMid < 0.0))
                {
                    leftEnd = mid;
                    yLeft = yMid;
                }
                else
                {
                    rightEnd = mid;
                }
                mid = (leftEnd + rightEnd) / 2.0;
                yMid = function(mid);
            }
            return mid;
        }

        public static void Shuffle(List<double> values)
        {
            int len = values.Count;
            for (int i = 0; i < len; i++)
            {
                int r = i + RandomInUniform(len - i);
                double temp = values[i];
                values[i] = values[r];
                values[r] = temp;
            }
        }

        public static double ValuateExpression(string expression)
        {
            var operands = new Stack<char>();
            var values = new Stack<double>();
            foreach (char c in expression)
            {
                switch (c)
                {
                    case '%':
                    case '*':
                    case '+':
                    case '-':
                    case '/':
                        operands.Push(c);
                        break;
                    case ')':
                        {
                            char operand = operands.Pop();
                            double value = values.Pop();
                            char c2 = operand;
                            if (c2 != '%')
                            {
                                switch (c2)
                                {
                                    case '*':
                                        value *= values.Pop();
                                        break;
                                    case '+':
                                        value += values.Pop();
                                        break;
                                    case '-':
                                        value -= values.Pop();
                                        break;
                                    case '/':
                                        value /= values.Pop();
                                        break;
                                }
                            }
                            else
                            {
                                value = values.Pop() % value;
                            }
                            values.Push(value);
                            break;
                        }
                }
            }
            return values.Pop();
        }
    }
}