using System;

namespace Exp.Investments
{
    public class Hedges
    {
        public static double ComputeMinVarianceHedgingFuturesCount(double futuresVariance, double covariance)
        {
            return covariance / futuresVariance;
        }
        public static double ComputeMinVarianceHedgingFuturesCount(double futuresVariance, SecuritiesCovariance covariance, double toHedgeSize, double futuresContractSize)
        {
            // -cov/var * contract count
            return Math.Round(-covariance.Covariance / futuresVariance * (toHedgeSize / futuresContractSize));
        }
    }
}