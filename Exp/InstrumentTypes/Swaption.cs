namespace Exp.InstrumentTypes
{
    public class Swaption : Option
    {
        public Swaption(Swap underlying)
        {
            Underlying = underlying;
        }
    }
}