using System;

namespace Exp.Utils
{
    public static class GenericExtensions
    {
        public static int ToInt(this object value)
        {
            return Convert.ToInt32(value);
        }
    }
}