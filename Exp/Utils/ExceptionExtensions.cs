using System;
using System.Collections.Generic;

namespace Exp.Utils
{
    public static class ExceptionExtensions
    {
        public static void ThrowIfNull(this object target)
        {
            if (target == null)
                throw new NullReferenceException();
        }

        public static void ThrowIfEmpty<T>(this IList<T> target)
        {
            if (target == null || target.Count == 0)
                throw new ArgumentException();
        }

        public static void ThrowIfZero<T>(this T target)
        {
            if (Equals(target, 0))
                throw new ArgumentException();
        }
    }
}