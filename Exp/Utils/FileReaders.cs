using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exp.InstrumentTypes;
using Exp.Investments;
using LumenWorks.Framework.IO.Csv;

namespace Exp.Utils
{
    public class FileReaders
    {
        public static List<Security> ReadSecurities(string csvFileName)
        {
            var results = new List<Security>();
            using (var reader = new CsvReader(new StreamReader(csvFileName), false, ','))
            {
                while (reader.ReadNextRecord())
                {
                    var sec = new Security();
                    sec.Id = int.Parse(reader[0]);
                    sec.Symbol = reader[1];
                    sec.MarketPrice = double.Parse(reader[2]);
                    sec.Volatility = double.Parse(reader[3]);
                    sec.YieldRate = double.Parse(reader[4]);
                    results.Add(sec);
                }
            }
            return results;
        }

        public static List<SecuritiesCovariance> ReadCorrelations(string csvFileName, List<Security> securities)
        {
            var results = new List<SecuritiesCovariance>();
            using (var reader = new CsvReader(new StreamReader(csvFileName), false, ','))
            {
                while (reader.ReadNextRecord())
                {
                    var cov = new SecuritiesCovariance();
                    var symbolOne = reader[0];
                    var symbolTwo = reader[1];
                    cov.Correlation = double.Parse(reader[2]);
                    cov.SecurityOne = securities.FirstOrDefault(s => s.Symbol == symbolOne);
                    cov.SecurityTwo = securities.FirstOrDefault(s => s.Symbol == symbolTwo);
                    results.Add(cov);
                }
            }
            return results;
        }
    }
}