using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exp.InstrumentTypes
{
    public class Mortgage : Security
    {
        public Mortgage(int periods, double initialBalance, double annualMortgageRate,
            int seasoning = 0, double prepaymentMultiplier = 0)
        {
            Periods = periods;
            Seasoning = seasoning;
            InitialBalance = initialBalance;
            MortgageStatuses = new List<MortgageStatus>();
            MortgageRate = annualMortgageRate;
            if (prepaymentMultiplier != 0)
            {
                MortgagePrepayment = new MortgagePrepayment(periods, seasoning, prepaymentMultiplier);
            }
        }

        public double InitialBalance { get; protected set; }
        public int Periods { get; protected set; }
        public int Seasoning { get; set; }
        public int ActuralPeriods { get { return Periods - Seasoning; } }

        public double MortgageRate { get; set; }
        public double MonthlyMortgageRate { get { return MortgageRate / 12; } }

        public virtual List<MortgageStatus> MortgageStatuses { get; protected set; }

        public MortgagePrepayment MortgagePrepayment { get; protected set; }

        public class MortgageStatus
        {
            public int Period { get; set; }
            public double BeginningBalance { get; set; }
            public double EndingBalance { get; set; }

            public double MortgagePayment { get; set; }
            public double InterestPayment { get; set; }
            public double PrincipalRepaid { get; set; }

            public double Prepayment { get; set; }

            public double TotalPrincipalReduced { get { return PrincipalRepaid + Prepayment; } }
        }
    }

    public class MortgagePrepayment
    {
        public MortgagePrepayment(int periods, int seasoning = 0, double prepaymentMultiplier = 1)
        {
            Periods = periods;
            Seasoning = seasoning;
            ActuralPeriods = periods - seasoning;
            PrepaymentMultiplier = prepaymentMultiplier;
            Cprs = new List<double>();
            Smms = new List<double>();
        }

        public int Periods { get; private set; }
        public int Seasoning { get; private set; }
        public int ActuralPeriods { get; private set; }
        public double PrepaymentMultiplier { get; set; }
        public List<double> Cprs { get; private set; }
        public List<double> Smms { get; private set; }
    }

    public class MortgageBackedSecurity : Derivatives
    {
        public MortgageBackedSecurity(Mortgage mortgage)
        {
            Underlying = mortgage;
        }

        public Mortgage Mortgage { get { return (Mortgage)Underlying; } }
        public double PassthroughRate { get; set; }
        public List<double> ReceivedInterests { get; private set; }

        public double InterestOnlyMbs { get; set; }
        public double PrincipalOnlyMbs { get; set; }
    }

    public class InterestOnlyMbs : Derivatives
    {
        public InterestOnlyMbs(MortgageBackedSecurity mbs)
        {
            Underlying = mbs;
        }
    }

    public class PrincipalOnlyMbs : Derivatives
    {
        public PrincipalOnlyMbs(MortgageBackedSecurity mbs)
        {
            Underlying = mbs;
        }
    }
}
