using System;

namespace Exp.InstrumentTypes
{
    public class Option : Derivatives
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

        public Func<double, double> Payoff { get; set; }

        /// <summary>
        /// Option does not have yield/payoffs.
        /// </summary>
        public new double YieldRate
        {
            get { return double.NaN; }
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