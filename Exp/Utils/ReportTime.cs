﻿using System;
using System.Diagnostics;

namespace Exp.Utils
{
    public class ReportTime : IDisposable
    {
        private readonly Stopwatch sw;
        private readonly string formattedString = "Used {0}";

        public static ReportTime Start()
        {
            return new ReportTime();
        }

        public ReportTime()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        public ReportTime(string formattedString)
        {
            this.formattedString = formattedString;
            sw = new Stopwatch();
            sw.Start();
        }

        public void Dispose()
        {
            sw.Stop();
            Console.WriteLine(formattedString, sw.Elapsed.TotalMilliseconds);
        }
    }
}