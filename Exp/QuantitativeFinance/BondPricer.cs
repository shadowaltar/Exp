using System;
using System.Collections.Generic;
using Exp.InstrumentTypes;
using Exp.Maths;
using Exp.Utils;

namespace Exp.QuantitativeFinance
{
    public class BondPricer
    {
        public static void ComputePrice(Bond bond, double constantAnnualInterestRate)
        {
            var par = bond.FaceValue;
            var c = bond.CouponPayment;
            var n = bond.TotalPaymentCounts;
            var r = constantAnnualInterestRate / bond.PaymentsPerYear;
            var mkt = bond.MarketPrice;

            // compute fair price
            var pvFaceValue = TimeValues.ComputePresentValue(par, n, r);
            var pvCoupon = TimeValues.ComputePresentValueOfCashFlows(c, n, r);
            bond.FairPrice = pvFaceValue + pvCoupon;

            // assign fair price to market price if not exist
            if (mkt == 0 || double.IsNaN(mkt))
                bond.MarketPrice = bond.FairPrice;

            // compute ytm
            var cfs = new List<double> { -bond.FairPrice }; // cf0 = mktvalue or fair price at inception
            for (int i = 0; i < n - 1; i++)
            {
                cfs.Add(c);
            }
            cfs.Add(c + par); // cfn = last coupon + repaid par value
            bond.YieldRate = new IrrCalculator(cfs).Compute();

            // compute maca dura
            ComputeDuration(bond, r);
        }

        /// <summary>
        /// Compute Macauley, Modified and Dollar Durations.
        /// All of them uses bond.MarketPrice.
        /// For Effective Duration, please refer to
        /// </summary>
        /// <param name="bond"></param>
        /// <param name="constantAnnualInterestRate"></param>
        public static void ComputeDuration(Bond bond, double constantAnnualInterestRate)
        {
            var p = bond.FaceValue;
            var n = bond.TotalPaymentCounts;
            var c = bond.CouponPayment;
            var r = constantAnnualInterestRate / bond.PaymentsPerYear;
            var mkt = bond.MarketPrice;

            // compute maca dura
            var maca = 0.0;
            for (int i = 1; i <= n; i++)
            {
                maca += TimeValues.ComputePresentValue(i * c, i, r);
            }
            maca += TimeValues.ComputePresentValue(n * p, n, r);
            bond.MacauleyDuration = maca / mkt;

            // compute modified dura
            bond.ModifiedDuration = bond.MacauleyDuration / (1 + r);

            // compute dv01
            bond.DollarDuration = mkt * bond.ModifiedDuration / 10000;
        }

        public static void ComputeEffectiveDuration(Bond bond,
            double priceYieldFalls, double priceYieldRises, double yieldChange)
        {
            var mkt = bond.MarketPrice;
            // we expect when yield rises, bond price falls; vice versa
            if (priceYieldFalls < mkt || priceYieldRises > mkt)
                throw new ArgumentException();
            if (yieldChange == 0)
                throw new ArgumentException();

            bond.EffectiveDuration = (priceYieldFalls - priceYieldRises) / 2 / mkt / yieldChange;
        }

        public static void ComputeConvexity(Bond bond, double constantAnnualInterestRate)
        {
            throw new NotImplementedException();
        }

        public static double ComputeAccruedInterest(Bond bond, double constantAnnualInterestRate,
            DateTime startDate, DateTime endDate,
            DayCountConvention dcc, DayRollingConvention drc)
        {
            var yearFraction = GetDayCountConventionNumerator(startDate, endDate, dcc) / GetDayCountConventionDenominator(startDate, endDate, dcc);
            return yearFraction * bond.FaceValue * constantAnnualInterestRate;
        }

        private static int GetDayCountConventionNumerator(DateTime dateOne, DateTime dateTwo,
            DayCountConvention dcc)
        {
            switch (dcc)
            {
                case DayCountConvention.ThirtyToThreeSixty:
                    return 360 * (dateTwo.Year - dateOne.Year) + 30 * (dateTwo.Month - dateOne.Month) + dateTwo.Day -
                           dateOne.Day;
                case DayCountConvention.ActToThreeSixty:
                case DayCountConvention.ActToThreeSixtyFive:
                case DayCountConvention.ActToAct:
                case DayCountConvention.ActToThreeSixtyFivePointTwoFive:
                    return (dateTwo.Date - dateOne.Date).TotalDays.ToInt();
                default:
                    throw new ArgumentException();
            }
        }

        private static double GetDayCountConventionDenominator(DateTime dateOne, DateTime dateTwo,
            DayCountConvention dcc)
        {
            switch (dcc)
            {
                case DayCountConvention.ActToThreeSixty:
                case DayCountConvention.ThirtyToThreeSixty:
                    return 360;
                case DayCountConvention.ActToThreeSixtyFive:
                    return 365;
                case DayCountConvention.ActToThreeSixtyFivePointTwoFive:
                    return 365.25;
                case DayCountConvention.ActToAct:
                    return (dateTwo.Date - dateOne.Date).TotalDays;
                default:
                    throw new ArgumentException();
            }
        }

        public static ZeroCouponBond Convert(Bond bond)
        {
            throw new NotImplementedException();
        }
    }

    public enum DayCountConvention
    {
        ActToThreeSixty,
        ActToThreeSixtyFive,
        ActToThreeSixtyFivePointTwoFive,
        ThirtyToThreeSixty,
        ActToAct,
    }

    public enum DayRollingConvention
    {
        FollowingBusinessDay,
        ModifiedFollowingBusinessDay,
        PreviousBusinessDay,
        ModifiedPreviousBusinessDay,
    }
}