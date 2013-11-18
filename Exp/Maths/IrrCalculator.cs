using System;
using System.Collections.Generic;

namespace Exp.Maths
{
    public class IrrCalculator
    {
        public static int MaxIterations { get { return 50000; } }
        public static double Tolerance { get { return 0.00000001; } }

        private readonly List<double> cashFlows;
        private int numberOfIterations;
        private double result;

        public IrrCalculator(List<double> cashFlows)
        {
            this.cashFlows = cashFlows;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid cash flows.
        /// </summary>
        private bool IsValidCashFlows
        {
            //Cash flows for the first period must be positive
            //There should be at least two cash flow periods         
            get
            {
                const int minNoCashFlowPeriods = 2;

                if (cashFlows.Count < minNoCashFlowPeriods || (cashFlows[0] > 0))
                {
                    throw new ArgumentOutOfRangeException(
                        "Cash flow for the first period  must be negative and there should");
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the initial guess.
        /// </summary>
        /// <value>The initial guess.</value>
        private double InitialGuess
        {
            get
            {
                double initialGuess = -1 * (1 + (cashFlows[1] / cashFlows[0]));
                return initialGuess;
            }
        }

        public double Compute()
        {
            if (IsValidCashFlows)
            {
                DoNewtonRapshonCalculation(InitialGuess);

                if (result > 1)
                    throw new ArgumentException("Failed to calculate the IRR for the cash flow series. Please provide a valid cash flow sequence");
            }
            return result;
        }

        /// <summary>
        /// Does the newton rapshon calculation.
        /// </summary>
        /// <param name="estimatedReturn">The estimated return.</param>
        /// <returns></returns>
        private void DoNewtonRapshonCalculation(double estimatedReturn)
        {
            numberOfIterations++;
            result = estimatedReturn - SumOfIrrPolynomial(estimatedReturn) / IrrDerivativeSum(estimatedReturn);
            while (!HasConverged(result) && MaxIterations != numberOfIterations)
            {
                DoNewtonRapshonCalculation(result);
            }
        }

        /// <summary>
        /// Sums the of IRR polynomial.
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns></returns>
        private double SumOfIrrPolynomial(double estimatedReturnRate)
        {
            double sumOfPolynomial = 0;
            if (IsValidIterationBounds(estimatedReturnRate))
                for (int j = 0; j < cashFlows.Count; j++)
                {
                    sumOfPolynomial += cashFlows[j] / (Math.Pow((1 + estimatedReturnRate), j));
                }
            return sumOfPolynomial;
        }

        /// <summary>
        /// Determines whether the specified estimated return rate has converged.
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified estimated return rate has converged; otherwise, <c>false</c>.
        /// </returns>
        private bool HasConverged(double estimatedReturnRate)
        {
            //Check that the calculated value makes the IRR polynomial zero.
            bool isWithinTolerance = Math.Abs(SumOfIrrPolynomial(estimatedReturnRate)) <= Tolerance;
            return isWithinTolerance;
        }

        /// <summary>
        /// IRRs the derivative sum.
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns></returns>
        private double IrrDerivativeSum(double estimatedReturnRate)
        {
            double sumOfDerivative = 0;
            if (IsValidIterationBounds(estimatedReturnRate))
                for (int i = 1; i < cashFlows.Count; i++)
                {
                    sumOfDerivative += cashFlows[i] * (i) / Math.Pow((1 + estimatedReturnRate), i);
                }
            return sumOfDerivative * -1;
        }

        /// <summary>
        /// Determines whether [is valid iteration bounds] [the specified estimated return rate].
        /// </summary>
        /// <param name="estimatedReturnRate">The estimated return rate.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid iteration bounds] [the specified estimated return rate]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsValidIterationBounds(double estimatedReturnRate)
        {
            return estimatedReturnRate != -1 && (estimatedReturnRate < int.MaxValue) &&
                   (estimatedReturnRate > int.MinValue);
        }
    }
}