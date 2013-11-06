using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exp;
using Exp.Maths;
using Exp.Utils;
using NUnit;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestMethod1()
        {
            var numbers = new List<double> { 3, 3, 3, 3, 1, 1, 1, 1, 4 };
            var m = numbers.Mode();
            Assert.Equals(m, 1d);
        }
    }
}
