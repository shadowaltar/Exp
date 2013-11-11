using System;

namespace Exp.QuantitativeFinance
{
    public class InterestRateModel
    {
        public static bool SetNotify<T>(InterestRateModel model, ref T target, T value)
        {
            if (!Equals(target, value))
            {
                target = value;
                model.Compute();
                return true;
            }
            return false;
        }

        protected virtual void Compute() { }
    }

    public class VasicekModel : InterestRateModel
    {
        public VasicekModel(double speedOfReversion, double longTermRate, double interestRateVolatility)
        {
            SpeedOfReversion = speedOfReversion;
            LongTermRate = longTermRate;
            InterestRateVolatility = interestRateVolatility;
        }

        public double PresentValue { get; private set; }
        public double SpotRate { get; private set; }
        public double YieldVolatility { get; private set; }

        public double SpeedOfReversion { get; set; }
        public double LongTermRate { get; set; }
        public double InterestRateVolatility { get; set; }

        private double timeToMaturity;
        public double TimeToMaturity
        {
            get { return timeToMaturity; }
            set
            {
                SetNotify(this, ref timeToMaturity, value);
            }
        }

        protected override void Compute()
        {
            var vol = InterestRateVolatility;
            var a = SpeedOfReversion;
            var b = LongTermRate;
            var t = timeToMaturity;

            var disc = Math.Exp(-a * t);
            var minusDisc = 1 - disc;
            var longTermVar = b - vol * vol / a / a / 2;

            var logAt = longTermVar / a * minusDisc
                - t * longTermVar
                - vol * vol / 4 / a / a / a * minusDisc * minusDisc;

            var bt = 1 / a * minusDisc;

            YieldVolatility = vol / a / t * minusDisc;
            SpotRate = -(logAt - bt * b) / t;
            PresentValue = Math.Exp(logAt) * Math.Exp(-b * bt);
        }
    }
}