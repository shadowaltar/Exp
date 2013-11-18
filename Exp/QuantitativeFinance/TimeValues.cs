using System;
using System.Collections.Generic;
using System.Linq;
using Exp.Maths;

namespace Exp.QuantitativeFinance
{
    public class TimeValues
    {
        public static double ComputeFutureValueOfCashFlows(double payment, int periods, double interestRate)
        {
            return payment / interestRate * (Math.Pow(1 + interestRate, periods) - 1);
        }

        public static double ComputeFutureValueOfOneTimeInvestment(double investmentNow, int periods, double interestRate)
        {
            return investmentNow * Math.Pow(1 + interestRate, periods);
        }

        public static double ComputeFutureValueOfOneTimeInvestment(double investmentNow, int years, int periodsPerYear, double annualInterestRate)
        {
            return investmentNow * Math.Pow(1 + annualInterestRate / periodsPerYear, periodsPerYear * years);
        }

        public static double ComputePresentValue(double futureValue, int periods, double interestRate)
        {
            return futureValue / Math.Pow(1 + interestRate, periods);
        }

        public static double ComputePresentValueOfCashFlows(double payment, int periods, double interestRate)
        {
            return payment / interestRate * (1 - 1 / Math.Pow(1 + interestRate, periods));
        }

        public static double ComputePresentValueOfCashFlows(List<double> payments, List<double> interestRates)
        {
            if (payments.Count != interestRates.Count)
                throw new ArgumentException();
            return payments.Select((t, i) => t / Math.Pow((1 + interestRates[i]), i)).Sum();
        }

        public static double ComputePresentValueOfCashFlows(List<double> payments, double interestRate)
        {
            return payments.Select((pmt, i) => pmt / Math.Pow((1 + interestRate), i)).Sum();
        }

        public static double ComputeInternalRateOfReturn(List<double> cashFlows)
        {
            return new IrrCalculator(cashFlows).Compute();
        }

        /// <summary>
        /// Compute Implied Forward Rate F(x,y). X must be smaller than Y.
        /// It is computed from: (1+F)^(y-x) * (1+Rx)^x = (1+Ry)^y
        /// </summary>
        /// <param name="spotRateX"></param>
        /// <param name="periodX"></param>
        /// <param name="spotRateY"></param>
        /// <param name="periodY"></param>
        /// <returns></returns>
        public static double ComputeImpliedForwardRate(double spotRateX, int periodX, double spotRateY, int periodY)
        {
            return Math.Pow(Math.Pow(1 + spotRateY, periodY) / Math.Pow(1 + spotRateX, periodX),
                1d / (periodY - periodX)) - 1; // F=[(1+Ry)^y]/[(1+Rx)^x] ^[1/(y-x)] - 1
        }
    }
}