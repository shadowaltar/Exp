using System;

namespace Exp.InstrumentTypes
{
    public abstract class Option : Derivatives
    {
        public OptionType Type { get; set; }
        public OptionStyleType Style { get; set; }
        public double Strike { get; set; }
        public double TimeToMaturity { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
        public double Vega { get; set; }
        public double Theta { get; set; }
        public double Rho { get; set; }
        /// <summary>
        /// Get or set the delta decay Charm, equals to -dDelta/dt, dTheta/dS and -dValue/dSdt.
        /// </summary>
        public double Charm { get; set; }

        /// <summary>
        /// Empty default payoff funtion for an option.
        /// </summary>
        /// <param name="underlyingPrice"></param>
        /// <returns></returns>
        public virtual double Payoff(double underlyingPrice)
        {
            return 0;
        }

        /// <summary>
        /// Option does not have yield/payoffs.
        /// </summary>
        public new double YieldRate
        {
            get { return double.NaN; }
        }
    }

    public class EuropeanCall : Option
    {
        public new OptionType Type { get { return OptionType.Call; } }
        public new OptionStyleType Style { get { return OptionStyleType.European; } }

        public override double Payoff(double underlyingPrice)
        {
            return Math.Max(0, underlyingPrice - Strike);
        }
    }

    public class EuropeanPut : Option
    {
        public new OptionType Type { get { return OptionType.Put; } }
        public new OptionStyleType Style { get { return OptionStyleType.European; } }

        public override double Payoff(double underlyingPrice)
        {
            return Math.Max(0, Strike - underlyingPrice);
        }
    }

    public enum OptionType
    {
        Call,
        Put,
    }

    public enum OptionStyleType
    {
        American,
        European,
        Asian
    }
}