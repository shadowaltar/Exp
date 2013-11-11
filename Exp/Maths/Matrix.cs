using System;
using System.Collections.Generic;
using System.Text;

namespace Exp.Maths
{
    public class Matrix
    {
        private readonly double[,] matrix;

        protected Matrix(double[,] values)
        {
            matrix = values;
        }

        protected Matrix(int m, int n)
        {
            matrix = new double[m, n];
        }

        public int RowCount
        {
            get { return matrix.GetLength(0); }
        }

        public int ColumnCount
        {
            get { return matrix.GetLength(1); }
        }

        public double this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }

        public static Matrix New(int m, int n)
        {
            return new Matrix(m, n);
        }

        public static Matrix New(double[,] values)
        {
            return new Matrix(values);
        }

        public static Matrix NewRowVector(int n)
        {
            return new Matrix(1, n);
        }

        public static Matrix NewRowVector(List<double> values)
        {
            Matrix i = NewRowVector(values.Count);
            for (int j = 0; j < i.ColumnCount; j++)
            {
                i[0, j] = values[j];
            }
            return i;
        }

        public static Matrix NewColumnVector(int m)
        {
            return new Matrix(m, 1);
        }

        public static Matrix NewColumnVector(List<double> values)
        {
            Matrix i = NewColumnVector(values.Count);
            for (int j = 0; j < i.RowCount; j++)
            {
                i[j, 0] = values[j];
            }
            return i;
        }

        public static Matrix NewIdentity(int n)
        {
            var result = NewSquare(n);
            for (int i = 0; i < n; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }

        public static Matrix NewSquare(int n)
        {
            return new Matrix(n, n);
        }

        public static Matrix NewDiagnoal(List<double> diagnalValues)
        {
            var dim = diagnalValues.Count;
            var result = NewSquare(dim);
            for (int i = 0; i < dim; i++)
            {
                result[i, i] = diagnalValues[i];
            }
            return result;
        }

        public Matrix Copy()
        {
            return New(matrix);
        }

        public bool IsSquare()
        {
            return RowCount == ColumnCount;
        }

        public List<double> RowVector(int i)
        {
            var v = new List<double>(ColumnCount);
            for (int j = 0; j < ColumnCount; j++)
            {
                v.Add(this[i, j]);
            }
            return v;
        }

        public List<double> ColumnVector(int j)
        {
            var v = new List<double>(RowCount);
            for (int i = 0; i < RowCount; i++)
            {
                v.Add(this[i, j]);
            }
            return v;
        }

        public double LargestValue()
        {
            double max = 0.0;
            double[,] array = matrix;
            int upperBound = array.GetUpperBound(0);
            int upperBound2 = array.GetUpperBound(1);
            for (int i = array.GetLowerBound(0); i <= upperBound; i++)
            {
                for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
                {
                    double d = array[i, j];
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
            return max;
        }

        public Matrix Transpose()
        {
            Matrix i = New(ColumnCount, RowCount);
            for (int j = 0; j < RowCount; j++)
            {
                for (int k = 0; k < ColumnCount; k++)
                {
                    i[k, j] = matrix[j, k];
                }
            }
            return i;
        }

        public Matrix Sub(int row, int column)
        {
            if (RowCount < 2 || ColumnCount < 2)
            {
                throw new ArithmeticException();
            }
            var i = New(RowCount - 1, ColumnCount - 1);
            var rowSkipped = false;
            var columnSkipped = false;
            for (var j = 0; j < RowCount; j++)
            {
                if (j == row)
                {
                    rowSkipped = true;
                }
                else
                {
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        if (k == column)
                        {
                            columnSkipped = true;
                        }
                        else
                        {
                            if (!rowSkipped && !columnSkipped)
                            {
                                i[j, k] = this[j, k];
                            }
                            else
                            {
                                if (!rowSkipped)
                                {
                                    i[j, k - 1] = this[j, k];
                                }
                                else
                                {
                                    if (!columnSkipped)
                                    {
                                        i[j - 1, k] = this[j, k];
                                    }
                                    else
                                    {
                                        i[j - 1, k - 1] = this[j, k];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return i;
        }

        public Matrix ToLowerTriangular()
        {
            if (!IsSquare())
                throw new ArithmeticException();

            var result = Copy();
            if (RowCount > 1 && ColumnCount > 1)
            {
                for (int i = 0; i < RowCount - 1; i++)
                {
                    for (int j = 1; j < ColumnCount; j++)
                    {
                        result[i, j] = 0;
                    }
                }
            }
            return result;
        }

        public Matrix ToUpperTriangular()
        {
            if (!IsSquare())
                throw new ArithmeticException();

            var result = Copy();
            if (RowCount > 1 && ColumnCount > 1)
            {
                for (int i = 1; i < RowCount; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        result[i, j] = 0;
                    }
                }
            }
            return result;
        }

        public static double DotProduct(List<double> x, List<double> y)
        {
            if (x == null || y == null || x.Count != y.Count)
            {
                throw new ArgumentException();
            }
            double result = 0.0;
            for (int i = 0; i < x.Count; i++)
            {
                result += x[i] * y[i];
            }
            return result;
        }

        public static Matrix operator *(Matrix x, Matrix y)
        {
            if (x == null || y == null || x.ColumnCount != y.RowCount)
            {
                throw new ArgumentException();
            }
            Matrix i = New(x.RowCount, y.ColumnCount);
            for (int j = 0; j < x.RowCount; j++)
            {
                for (int k = 0; k < y.ColumnCount; k++)
                {
                    i[j, k] = DotProduct(x.RowVector(j), y.ColumnVector(k));
                }
            }
            return i;
        }

        public static Matrix operator *(Matrix x, List<double> y)
        {
            return x * NewColumnVector(y);
        }

        public static Matrix operator *(double x, Matrix y)
        {
            Matrix i = New(y.RowCount, y.ColumnCount);
            for (int j = 0; j < y.RowCount; j++)
            {
                for (int k = 0; k < y.ColumnCount; k++)
                {
                    i[j, k] = x * y[j, k];
                }
            }
            return i;
        }

        public static Matrix operator +(Matrix x, Matrix y)
        {
            if (x == null || y == null || x.RowCount != y.RowCount || x.ColumnCount != y.ColumnCount)
            {
                throw new ArgumentException();
            }
            Matrix i = New(x.RowCount, x.ColumnCount);
            for (int j = 0; j < x.RowCount; j++)
            {
                for (int k = 0; k < x.ColumnCount; k++)
                {
                    i[j, k] = x[j, k] + y[j, k];
                }
            }
            return i;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    sb.Append(this[i, j]).Append('|');
                }
                sb.AppendLine("|");
            }
            return sb.ToString();
        }
    }

    public class LabeledMatrix<T> : Matrix
    {
        public List<T> RowEntities { get; private set; }
        public List<T> ColumnEntities { get; private set; }

        protected LabeledMatrix(double[,] values)
            : base(values)
        {
        }

        protected LabeledMatrix(int m, int n)
            : base(m, n)
        {
        }

        public static LabeledMatrix<T> New(List<T> rowEntities, List<T> columnEntities)
        {
            var matrix = new LabeledMatrix<T>(rowEntities.Count, columnEntities.Count)
            {
                RowEntities = rowEntities,
                ColumnEntities = columnEntities
            };
            return matrix;
        }

        public double Get(T rowEntity, T columnEntity)
        {
            if (!RowEntities.Contains(rowEntity) || !ColumnEntities.Contains(columnEntity))
            {
                throw new ArgumentException();
            }
            var i = RowEntities.IndexOf(rowEntity);
            var j = ColumnEntities.IndexOf(columnEntity);
            return this[i, j];
        }

        public void Set(T rowEntity, T columnEntity, double value)
        {
            if (!RowEntities.Contains(rowEntity) || !ColumnEntities.Contains(columnEntity))
            {
                throw new ArgumentException();
            }
            var i = RowEntities.IndexOf(rowEntity);
            var j = ColumnEntities.IndexOf(columnEntity);
            this[i, j] = value;
        }
    }
}