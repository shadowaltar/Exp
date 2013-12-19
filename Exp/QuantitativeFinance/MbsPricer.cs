using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exp.InstrumentTypes;

namespace Exp.QuantitativeFinance
{
    public class MbsPricer
    {
        public static double ComputeCpr(int month, int seasoningMonths = 0, double prepaymentMultiplier = 1)
        {
            if (month <= 0)
                throw new ArgumentException();
            if (seasoningMonths < 0)
                throw new ArgumentException();
            if (prepaymentMultiplier < 0)
                throw new ArgumentException();

            if (month > 30 - seasoningMonths)
                return 0.06 * prepaymentMultiplier;
            return 0.06 * prepaymentMultiplier * (month + seasoningMonths) / 30;
        }

        public static double ComputeSmm(int month, int seasoningMonths = 0, double prepaymentMultiplier = 1)
        {
            if (month <= 0)
                throw new ArgumentException();
            if (seasoningMonths < 0)
                throw new ArgumentException();
            if (prepaymentMultiplier < 0)
                throw new ArgumentException();

            return 1 - Math.Pow(1 - ComputeCpr(month, seasoningMonths, prepaymentMultiplier), 1d / 12d);
        }

        public static double ComputeSmmFromCpr(double cpr)
        {
            return 1 - Math.Pow(1 - cpr, 1d / 12d);
        }

        public static void ComputeMortgage(Mortgage mortgage)
        {
            if (mortgage.MortgagePrepayment != null)
            {
                ComputePrepayment(mortgage.MortgagePrepayment);
            }

            var rate = mortgage.MonthlyMortgageRate;
            var n = mortgage.Periods;
            var s = mortgage.Seasoning;

            for (int i = 0; i < mortgage.ActuralPeriods; i++)
            {
                var currentStatus = new Mortgage.MortgageStatus();
                mortgage.MortgageStatuses.Add(currentStatus);
                var balance = i == 0
                    ? mortgage.InitialBalance : mortgage.MortgageStatuses[i - 1].EndingBalance;
                currentStatus.BeginningBalance = balance;
                currentStatus.MortgagePayment = balance * rate / (1 - Math.Pow(1 + rate, -n + s + i));
                currentStatus.InterestPayment = balance * rate;
                currentStatus.PrincipalRepaid = currentStatus.MortgagePayment - currentStatus.InterestPayment;

                if (mortgage.MortgagePrepayment != null)
                {
                    currentStatus.Prepayment = (balance - currentStatus.PrincipalRepaid)
                        * mortgage.MortgagePrepayment.Smms[i];
                }
                currentStatus.EndingBalance = balance - currentStatus.PrincipalRepaid - currentStatus.Prepayment;
            }
        }

        public static void ComputePrepayment(MortgagePrepayment prepayment)
        {
            var periods = prepayment.Periods - prepayment.Seasoning;
            for (int i = 1; i <= periods; i++)
            {
                prepayment.Cprs.Add(ComputeCpr(i, prepayment.Seasoning, prepayment.PrepaymentMultiplier));
                prepayment.Smms.Add(ComputeSmmFromCpr(prepayment.Cprs[i - 1]));
            }
        }

        public static void ComputeInterestOnlyMbs(MortgageBackedSecurity mbs)
        {
            ComputeMortgage((Mortgage)mbs.Underlying);
            //
        }
    }
}
