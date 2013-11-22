using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace Exp.InstrumentTypes
{
    public interface BaseBond
    {
        DateTime Today { get; set; }
        DateTime SettlementDate { get; set; }
        int SettlementDayLag { get; set; }
        Schedule Schedule { get; set; }

        DateTime StartDate { get; }
        DateTime EndDate { get; }
        DateTime LastCouponDate { get; }
        DateTime NextCouponDate { get; }
        List<double> Coupons { get; set; }

        double CurrentCouponRate { get; }

        double FaceValue { get; }
        double AccruedInterest { get; }
    }

    public class Bond : Security, BaseBond
    {
        /// <summary>
        /// Bond face value; also called par value.
        /// </summary>
        public double FaceValue { get; private set; }
        /// <summary>
        /// Numbers of years to maturity.
        /// </summary>
        public int Years { get; private set; }
        /// <summary>
        /// Coupon payment counts per year.
        /// </summary>
        public int PaymentsPerYear { get; private set; }
        /// <summary>
        /// Total coupon payment counts from start to maturity, by Years*PaymentsPerYear.
        /// </summary>
        public int TotalPaymentCounts { get; private set; }
        /// <summary>
        /// Percentage rate of coupon payments to the face value.
        /// </summary>
        public double CouponRate { get; private set; }
        /// <summary>
        /// Periodic coupon payment.
        /// </summary>
        public double CouponPayment { get; private set; }
        /// <summary>
        /// YTM of the bond, which is the IRR of all its payments.
        /// </summary>
        public double YieldToMaturity { get { return YieldRate; } }

        /// <summary>
        /// The weighted average time to maturity of all the cash flows from a bond.
        /// </summary>
        public double MacauleyDuration { get; set; }

        /// <summary>
        /// The (opposite) change in the value of a the bond price in response to a change in the interest rate.
        /// </summary>
        public double ModifiedDuration { get; set; }

        /// <summary>
        /// The dollar duration of one basis point: bond mktvalue * modi dura / 10000.
        /// </summary>
        public double DollarDuration { get; set; }

        /// <summary>
        /// The duration which calculated the effect of embedded options.
        /// </summary>
        public double EffectiveDuration { get; set; }

        public Bond(double faceValue, int years, int paymentsPerYear, double couponRate)
        {
            FaceValue = faceValue;
            Years = years;
            PaymentsPerYear = paymentsPerYear;
            CouponRate = couponRate;
            CouponPayment = FaceValue * couponRate;
            TotalPaymentCounts = Years * PaymentsPerYear;

            YieldRate = double.NaN;
            MacauleyDuration = double.NaN;
            ModifiedDuration = double.NaN;
            DollarDuration = double.NaN;
        }
    }

    public class Schedule
    {
        /// <summary>
        /// Start dates of each period.
        /// </summary>
        public List<DateTime> FromDates { get; set; }
        /// <summary>
        /// End dates of each period.
        /// </summary>
        public List<DateTime> ToDates { get; set; }
        /// <summary>
        /// Payment dates of each period.
        /// </summary>
        public List<DateTime> PayDates { get; set; }

        public DateTime FixingDate { get; set; }

        /// <summary>
        /// Start date of schedule.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// End date of schedule.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Tenor
        /// </summary>
        public Tenor Tenor { get; set; }

        public Rule GeneratorRule { get; set; }

        public BusinessDayAdjustment RollAdjust { get; set; }
        public BusinessDayAdjustment PayAdjust { get; set; }
    }

    public class FloatingSchedule : Schedule
    {
        /// <summary>
        /// Payment dates of each period.
        /// </summary>
        public List<DateTime> FixingDates { get; set; }

        /// <summary>
        /// Lag of days to treat as business day, or rather T-X.
        /// </summary>
        public int BusinessDaysLag { get; set; }

        /// <summary>
        /// Leg from EndDate or StartDate.
        /// </summary>
        public bool IsArrears { get; set; }
    }

    public class BusinessDayAdjustment
    {
    }

    public struct Tenor
    {
        public int Length { get; set; }
        public TimeUnit TimeUnit { get; set; }

        public bool Equals(Tenor other)
        {
            return Length == other.Length && TimeUnit == other.TimeUnit;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Tenor && Equals((Tenor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Length * 397) ^ (int)TimeUnit;
            }
        }

        public override string ToString()
        {
            return Length + TimeUnit.ToString();
        }
    }

    public enum TimeUnit
    {
        M,
        Y,
    }
}