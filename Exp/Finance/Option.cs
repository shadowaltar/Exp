using System;

namespace Exp.Finance
{
    public class Security
    {
        public double Price { get; set; }
        public double Volatility { get; set; }

        public virtual double YieldRate { get; set; }
    }

    public abstract class Option : Security
    {
        public OptionType Type { get; set; }
        public OptionStyleType Style { get; set; }
        public double Strike { get; set; }
        public double TimeToMaturity { get; set; }
        public Security Underlying { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
        public double Vega { get; set; }
        public double Theta { get; set; }
        public double Rho { get; set; }
        /// <summary>
        /// Get or set the delta decay Charm, equals to -dDelta/dt, dTheta/dS and -dValue/dSdt.
        /// </summary>
        public double Charm { get; set; }
        public abstract double Payoff(double underlyingPrice);

        /// <summary>
        /// Option does not have yield/payoffs.
        /// </summary>
        public override double YieldRate { get { return double.NaN; } set { } }
    }

    public abstract class Swaption : Option
    {
        public Swaption(Swap underlying)
        {
            Underlying = underlying;
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