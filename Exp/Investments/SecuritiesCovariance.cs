using System;
using Exp.InstrumentTypes;

namespace Exp.Investments
{
    public class SecuritiesCovariance
    {
        public SecuritiesCovariance()
        {
        }

        public SecuritiesCovariance(Security securityOne, Security securityTwo, double correlation)
        {
            SecurityOne = securityOne;
            SecurityTwo = securityTwo;
            Correlation = correlation;
        }

        public Security SecurityOne { get; set; }
        public Security SecurityTwo { get; set; }
        public double Correlation { get; set; }

        public double Covariance { get; private set; }

        public void Compute()
        {
            if (SecurityOne == null || SecurityTwo == null)
                throw new ArgumentNullException();
            Covariance = SecurityOne.Volatility * SecurityTwo.Volatility * Correlation;
        }
    }
}