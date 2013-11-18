using System;
using System.Collections.Generic;
using System.Linq;
using Exp.InstrumentTypes;
using Exp.Maths;
using Microsoft.SolverFoundation.Services;
using NUnit.Framework;
using SolverFoundation.Plugin.Gurobi;

namespace Exp.Investments
{
    public class Portfolios
    {
        public static LabeledMatrix<Security> ComputeCovariances(List<SecuritiesCovariance> covariances)
        {
            List<Security> securities = new List<Security>();
            foreach (var cov in covariances)
            {
                if (!securities.Contains(cov.SecurityOne))
                    securities.Add(cov.SecurityOne);
                if (!securities.Contains(cov.SecurityTwo))
                    securities.Add(cov.SecurityTwo);
                cov.Compute();
            }

            var matrix = LabeledMatrix<Security>.New(securities, securities);
            foreach (var rowEntity in matrix.RowEntities)
            {
                foreach (var columnEntity in matrix.ColumnEntities)
                {
                    var cov = covariances.First(c => (c.SecurityOne == rowEntity && c.SecurityTwo == columnEntity)
                        || (c.SecurityTwo == rowEntity && c.SecurityOne == columnEntity))
                        .Covariance;
                    matrix.Set(rowEntity, columnEntity, cov);
                }
            }
            return matrix;
        }

        public static Dictionary<Security, double> GetEqualWeights(List<Security> securities)
        {
            return securities.ToDictionary(s => s, v => 1d / securities.Count);
        }

        public static double ComputePortfolioVariance(LabeledMatrix<Security> matrix, Dictionary<Security, double> weights)
        {
            var sum = 0d;
            foreach (Security rowEntity in matrix.RowEntities)
            {
                foreach (Security columnEntity in matrix.ColumnEntities)
                {
                    sum += (matrix.Get(rowEntity, columnEntity) * weights[rowEntity] * weights[columnEntity]);
                }
            }
            return sum;
        }

        public static double ComputePortfolioMeanVariance(LabeledMatrix<Security> matrix, double expectedReturn, out Dictionary<Security, double> weights, bool allowShortSelling = false)
        {
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            // since the row securities are the same as the column securities. use row.
            var securities = matrix.RowEntities;
            var weightDecisions = new Dictionary<Security, Decision>();
            var rangeOfWeights = allowShortSelling ? Domain.RealRange(-10, 10) : Domain.RealRange(0, 10);

            Term t1 = 0d; // constraint 1, sum of weights = 1
            Term t2 = 0d; // constraint 2, expected portfolio return (sum of weight*price) = expectedReturn
            foreach (var security in securities)
            {
                var securityWeightDecision = new Decision(rangeOfWeights, security.Symbol); // -10 <= w <= 10
                weightDecisions[security] = securityWeightDecision;
                model.AddDecisions(securityWeightDecision);

                t1 += securityWeightDecision;
                t2 += (securityWeightDecision * security.MarketPrice);
            }

            model.AddConstraint("SumOfWeights", t1 == 1d);
            model.AddConstraint("ExpectedPortfolioReturn", t2 == expectedReturn);

            // goal 1, the ptf var to be minimized
            Term varianceTerm = 0d;
            foreach (Security rowEntity in matrix.RowEntities)
            {
                foreach (Security columnEntity in matrix.ColumnEntities)
                {
                    varianceTerm += (matrix.Get(rowEntity, columnEntity) * weightDecisions[rowEntity] * weightDecisions[columnEntity]);
                }
            }

            Goal goal = model.AddGoal("MeanVariance", GoalKind.Minimize, varianceTerm);

            var gurobiDirective = new GurobiDirective();
            //gurobiDirective.OutputFlag = true;

            context.Solve(gurobiDirective);
            //Report report = solution.GetReport();
            //Console.WriteLine("{0}", report);
            //Console.WriteLine(goal.ToDouble());
            //foreach (var weightDecision in weightDecisions)
            //{
            //    Console.WriteLine(weightDecision.Key.Symbol + ":" + weightDecision.Value.GetDouble());
            //}
            context.ClearModel();

            weights = weightDecisions.ToDictionary(p => p.Key, p => p.Value.GetDouble());
            return goal.ToDouble();
        }
    }
}