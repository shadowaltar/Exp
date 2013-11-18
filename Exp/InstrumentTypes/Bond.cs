using System;

namespace Exp.InstrumentTypes
{
    public class Bond : Security
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

        public double MacauleyDuration { get; set; }

        public double ModifiedDuration { get; set; }

        /// <summary>
        /// The dollar duration of one basis point: bond mktvalue * modi dura / 10000.
        /// </summary>
        public double DollarDuration { get; set; }

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

    public class ZeroCouponBond : Bond
    {
        public ZeroCouponBond(double faceValue, int years)
            : base(faceValue, years, 1, 0)
        {
        }
    }

    public class DateSchedule
    {
        public DateTime FixingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsBusinessDaysAdjusted { get; set; }
        public bool IsFixingDaysCountedFromTheEnd { get; set; }
        public bool IsShortPeriodAt { get; set; }
    }
}