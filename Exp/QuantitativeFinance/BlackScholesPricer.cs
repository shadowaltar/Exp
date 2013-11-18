using System;
using Exp.InstrumentTypes;
using Exp.Maths;

namespace Exp.QuantitativeFinance
{
    public class BlackScholesPricer
    {
        public static void ComputePrice(Option option, double interestRate)
        {
            if (option.Style == OptionStyleType.European)
            {
                if (option.Type == OptionType.Call)
                    ComputeEuropeanCallPrice(option, interestRate);
                else
                    ComputeEuropeanPutPrice(option, interestRate);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Compute European call's price.
        /// </summary>
        /// <param name="option">The call option.</param>
        /// <param name="interestRate">The constant interest rate as the discount rate.</param>
        /// <param name="computeGreeks"></param>
        public static void ComputeEuropeanCallPrice(Option option,
            double interestRate, bool computeGreeks = true)
        {
            var sig = option.Volatility;
            var t = option.TimeToMaturity;
            var s = option.Underlying.MarketPrice;
            var q = option.Underlying.YieldRate;
            var k = option.Strike;
            var r = interestRate;

            var sqrtT = Math.Sqrt(t);
            var sigSqrtT = sig * sqrtT; // vola * (t^0.5)
            var rateDisc = Math.Exp(-r * t); // e^(-r*t)
            var yieldDisc = Math.Exp(-q * t); // e^(-q*t)
            var d1 = (Math.Log(s / k) + (r - q + sig * sig / 2) * t) / sigSqrtT;
            var d2 = d1 - sigSqrtT;
            var nd1 = N(d1);
            var nd2 = N(d2);
            option.MarketPrice = s * yieldDisc * nd1 - k * rateDisc * nd2;

            if (computeGreeks)
            {
                var pd1 = P(d1);
                option.Delta = yieldDisc * nd1;
                option.Gamma = yieldDisc * pd1 / s / sigSqrtT;
                option.Vega = s * yieldDisc * pd1 * sqrtT;
                option.Theta = -yieldDisc * s * pd1 * sig / 2 / sqrtT
                    - r * k * rateDisc * nd2 + q * s * yieldDisc * nd1;
                option.Rho = k * t * rateDisc * nd2;
                option.Charm = yieldDisc * (q * nd1 - pd1 * (2 * (r - q) * t - d2 * sigSqrtT) / (2 * t * sigSqrtT));
            }
        }

        public static void ComputeEuropeanPutPrice(Option option,
            double interestRate, bool computeGreeks = true)
        {
            var sig = option.Volatility;
            var t = option.TimeToMaturity;
            var s = option.Underlying.MarketPrice;
            var q = option.Underlying.YieldRate;
            var k = option.Strike;
            var r = interestRate;

            var sqrtT = Math.Sqrt(t);
            var sigSqrtT = sig * sqrtT; // vola * (t^0.5)
            var rateDisc = Math.Exp(-r * t); // e^(-r*t)
            var yieldDisc = Math.Exp(-q * t); // e^(-q*t)
            var d1 = (Math.Log(s / k) + (r + sig * sig / 2) * t) / sigSqrtT;
            var d2 = d1 - sigSqrtT;
            var nnd1 = N(-d1);
            var nnd2 = N(-d2);
            option.MarketPrice = k * rateDisc * nnd2 - s * yieldDisc * nnd1;

            if (computeGreeks)
            {
                var pd1 = P(d1);
                option.Delta = -yieldDisc * N(d1);
                option.Gamma = yieldDisc * pd1 / s / sigSqrtT;
                option.Vega = s * yieldDisc * pd1 * sqrtT;
                option.Theta = -yieldDisc * s * pd1 * sig / 2 / sqrtT
                    + r * k * rateDisc * nnd2 - q * s * yieldDisc * nnd1;
                option.Rho = -k * t * rateDisc * nnd2;
                option.Charm = -yieldDisc * (q * nnd1 + pd1 * (2 * (r - q) * t - d2 * sigSqrtT) / (2 * t * sigSqrtT));
            }
        }

        public static void ComputeEuropeanPayerSwaptionPrice(Swaption swaption, double interestRate)
        {
            var swap = swaption.Underlying as PlainVanillaInterestRateSwap;
            if (swap == null)
                throw new ArgumentException("Need a plain vanilla IRS as the swaption's underlying.");
            if (!swap.FixedRate.ApproxEquals(swaption.Strike))
                throw new ArgumentException("Fixed rate of a plain vanilla IRS underlying should equal to the strike rate.");

            var tswap = swap.Tenor;
            var flt = swap.FloatRate;
            var fix = swap.FixedRate; // = swaption.Strike
            var r = interestRate;
            var t = swaption.TimeToMaturity;
            var sig = swaption.Volatility; // the Black Implied Volatility, swaption maturity, forward tenor, and strike dependent
            var m = swap.YearlyFloatPaymentCount;

            var alpha = (1 - 1 / Math.Pow(1 + flt / m, tswap * m)) / flt;
            var d1 = Math.Log(flt / fix) + (sig * sig / 2 * t);
            var d2 = d1 - sig * Math.Sqrt(t);

            swaption.MarketPrice = alpha * Math.Exp(-r * t) * (flt * N(d1) - fix * N(d2));
        }

        public static void ComputeEuropeanReceiverSwaptionPrice(Swaption swaption, double interestRate)
        {
            var swap = swaption.Underlying as PlainVanillaInterestRateSwap;
            if (swap == null)
                throw new ArgumentException("Need a plain vanilla IRS as the swaption's underlying.");
            if (swap.FixedRate.ApproxEquals(swaption.Strike))
                throw new ArgumentException("Fixed rate of a plain vanilla IRS underlying should equal to the strike rate.");

            var tswap = swap.Tenor;
            var flt = swap.FloatRate;
            var fix = swap.FixedRate; // = swaption.Strike
            var r = interestRate;
            var t = swaption.TimeToMaturity;
            var sig = swaption.Volatility; // the Black Implied Volatility, swaption maturity, forward tenor, and strike dependent
            var m = swap.YearlyFloatPaymentCount;

            var alpha = (1 - 1 / Math.Pow(1 + flt / m, tswap * m)) / flt;
            var d1 = Math.Log(flt / fix) + (sig * sig / 2 * t);
            var d2 = d1 - sig * Math.Sqrt(t);

            swaption.MarketPrice = alpha * Math.Exp(-r * t) * (fix * N(-d2) - flt * N(-d1));
        }

        private static double N(double d)
        {
            return NormalDistribution.Standard.Cdf(d);
        }

        private static double P(double d)
        {
            return NormalDistribution.Standard.Pdf(d);
        }
    }
}