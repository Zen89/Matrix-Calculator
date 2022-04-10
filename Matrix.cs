using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Calculator
{
    public class Matrix
    {
        public string MatrixName { get; set; }
        public double[,] MatrixBody { get; set; }
        public int MatrixRows { get; set; }
        public int MatrixCols { get; set; }

        public Matrix(int rows, int cols, string name = "Matrix 0")
        {
            MatrixName = name;
            MatrixBody = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    MatrixBody[i, j] = 0;
                }
            }
            MatrixRows = rows;
            MatrixCols = cols;
        }

        public double this[int i, int j]
        {
            get { return MatrixBody[i, j]; }
            set { MatrixBody[i, j] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{MatrixName}");
            for (int i = 0; i < MatrixBody.GetLength(0); i++)
            {
                sb.Append("\r");
                for (int j = 0; j < MatrixBody.GetLength(1); j++)
                {
                    sb.Append($"{MatrixBody[i, j]} ".PadLeft(4));
                }
            }
            return sb.ToString();
        }

        public static Matrix operator +(Matrix matrixA, Matrix matrixB)
        {
            Matrix matrixAplusB = new Matrix(matrixA.MatrixRows, matrixA.MatrixCols, "Matrix AB");

            try
            {
                if (matrixA.MatrixRows == matrixB.MatrixRows && matrixA.MatrixCols == matrixB.MatrixCols)
                {
                    for (int i = 0; i < matrixA.MatrixRows; i++)
                        for (int j = 0; j < matrixA.MatrixCols; j++)
                            matrixAplusB[i, j] = matrixA[i, j] + matrixB[i, j];

                    return matrixAplusB;
                }
                else
                {
                    throw new Exception("Macierze nie mają tych samych rozmiarów!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Matrix operator -(Matrix matrixA, Matrix matrixB)
        {
            Matrix matrixAminusB = new Matrix(matrixA.MatrixRows, matrixA.MatrixCols, "Matrix AB");

            try
            {
                if (matrixA.MatrixRows == matrixB.MatrixRows && matrixA.MatrixCols == matrixB.MatrixCols)
                {
                    for (int i = 0; i < matrixA.MatrixRows; i++)
                        for (int j = 0; j < matrixA.MatrixCols; j++)
                            matrixAminusB[i, j] = matrixA[i, j] - matrixB[i, j];

                    return matrixAminusB;
                }
                else
                {
                    throw new Exception("Macierze nie mają tych samych rozmiarów!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Matrix operator *(Matrix matrixA, Matrix matrixB)
        {
            Matrix matrixAmultiB = new Matrix(matrixA.MatrixRows, matrixB.MatrixCols, "MatrixAB");

            try
            {
                if (matrixA.MatrixCols == matrixB.MatrixRows)
                {
                    for (int i = 0; i < matrixA.MatrixRows; i++)
                        for (int j = 0; j < matrixB.MatrixCols; j++)
                            for (int k = 0; k < matrixA.MatrixCols; k++)
                                matrixAmultiB[i, j] += matrixA[i, k] * matrixB[k, j];

                    return matrixAmultiB;
                }
                else
                {
                    throw new Exception("Liczba kolumn macierzy A nie jest równa liczbie wierszy macierzy B!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Matrix Transpose(Matrix matrixAA)
        {
            Matrix matrixAT = new Matrix(matrixAA.MatrixCols, matrixAA.MatrixRows, $"{matrixAA.MatrixName}T");

            try
            {
                for (int i = 0; i < matrixAA.MatrixRows; i++)
                    for (int j = 0; j < matrixAA.MatrixCols; j++)
                        matrixAT[j, i] = matrixAA[i, j];

                return matrixAT;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static double DeterminantBareiss(Matrix matrixAA)
        {
            Matrix matrixDB = new Matrix(matrixAA.MatrixRows, matrixAA.MatrixCols, matrixAA.MatrixName);
            Matrix matrixTmp = new Matrix(matrixAA.MatrixRows, matrixAA.MatrixCols, matrixAA.MatrixName);
            for (int i = 0; i < matrixAA.MatrixRows; i++)
                for (int j = 0; j < matrixAA.MatrixCols; j++)
                {
                    matrixDB[i, j] = matrixAA[i, j];
                }

            double detA = 0;

            try
            {
                //wyznacznik dla macierzy 1x1, 2x2 i 3x3 metodą Sarrusa
                //double add = 0;
                //for (int j = 0; j < matrixAA.MatrixCols; j++)
                //{
                //    double multi = 1;
                //    for (int i = 0; i < matrixAA.MatrixRows; i++)
                //    {
                //        int k = j + i;
                //        if (k >= matrixAA.MatrixCols) k = k - matrixAa.MatrixCols;
                //        multi *= matrixAA[i, k];
                //    }
                //    add += multi;
                //}

                //double sub = 0;
                //for (int j = 0; j < matrixAA.MatrixCols; j++)
                //{
                //    double multi = 1;
                //    for (int i = 0; i < matrixAA.MatrixRows; i++)
                //    {
                //        int k = (matrixAA.MatrixRows - 1) - (j + i);
                //        if (k < 0) k = matrixAA.MatrixCols + k;
                //        multi *= matrixAA[i, k];
                //    }
                //    sub += multi;
                //}
                //detA = add - sub;
                //Console.WriteLine(detA.ToString());

                //wyznacznik dla macierzy n x n algorytmem Bareissa
                if (matrixAA.MatrixRows == matrixAA.MatrixCols)
                {
                    for (int n = 0; n < matrixAA.MatrixRows; n++)
                    {
                        double p = 1.0;

                        if (n == 0 && matrixDB[n, n] == 0)
                        {
                            int i = n;
                            while (matrixDB[i, n] == 0)
                            {
                                i++;
                                if (i >= matrixDB.MatrixRows)
                                {
                                    return 0;
                                }
                            }
                            double[] temp = new double[matrixDB.MatrixCols];
                            for (int j = 0; j < matrixDB.MatrixCols; j++)
                            {
                                temp[j] = Math.Pow((-1), (i - n)) * matrixDB[i, j];
                                matrixDB[i, j] = matrixDB[n, j];
                                matrixDB[n, j] = temp[j];
                            }
                        }
                        else if (n > 0)
                        {
                            p = matrixDB[n - 1, n - 1];

                            if (matrixTmp[n, n] == 0)
                            {
                                int i = n;
                                while (matrixTmp[i, n] == 0)
                                {
                                    i++;
                                    if (i >= matrixDB.MatrixRows)
                                    {
                                        return 0;
                                    }
                                }
                                double[] temp = new double[matrixDB.MatrixCols];
                                for (int j = 0; j < matrixDB.MatrixCols; j++)
                                {
                                    temp[j] = Math.Pow((-1), (i - n)) * matrixTmp[i, j];
                                    matrixTmp[i, j] = matrixTmp[n, j];
                                    matrixTmp[n, j] = temp[j];
                                }
                            }

                            for (int i = 0; i < matrixAA.MatrixRows; i++)
                                for (int j = 0; j < matrixAA.MatrixCols; j++)
                                {
                                    matrixDB[i, j] = matrixTmp[i, j];
                                }
                        }

                        for (int i = 0; i < matrixAA.MatrixRows; i++)
                        {
                            if (n == i)
                            {
                                for (int j = 0; j < matrixAA.MatrixCols; j++)
                                {
                                    matrixTmp[i, j] = matrixDB[i, j];
                                }
                            }
                            else
                            {
                                for (int j = 0; j < matrixAA.MatrixCols; j++)
                                {
                                    matrixTmp[i, j] = (matrixDB[n, n] * matrixDB[i, j] - matrixDB[i, n] * matrixDB[n, j]) / p;
                                }
                            }
                        }
                    }
                    detA = matrixTmp[matrixTmp.MatrixRows - 1, matrixTmp.MatrixCols - 1];
                    return detA;
                }
                else
                {
                    throw new Exception("Liczba kolumn macierzy A nie jest równa liczbie wierszy!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Matrix RowSwitch(Matrix matrix, int firstRow, int secondRow)
        {
            double[] temp = new double[matrix.MatrixCols];
            if (firstRow >= 0 && secondRow >= 0 && firstRow < matrix.MatrixRows && secondRow < matrix.MatrixRows)
            {

                for (int j = 0; j < matrix.MatrixCols; j++)
                {
                    temp[j] = Math.Pow((-1), Math.Abs(firstRow - secondRow)) * matrix[firstRow, j];
                    matrix[firstRow, j] = matrix[secondRow, j];
                    matrix[secondRow, j] = temp[j];
                }
            }
            else
            {
                throw new Exception("Błędny/e numer/y wierszy do zamiany!");
            }
            return matrix;
        }

        public static Matrix ColSwitch(Matrix matrix, int firstCol, int secondCol)
        {
            double[] temp = new double[matrix.MatrixRows];
            if (firstCol >= 0 && secondCol >= 0 && firstCol < matrix.MatrixCols && secondCol < matrix.MatrixCols)
            {
                for (int j = 0; j < matrix.MatrixRows; j++)
                {
                    temp[j] = Math.Pow((-1), Math.Abs(firstCol - secondCol)) * matrix[j, firstCol];
                    matrix[j, firstCol] = matrix[j, secondCol];
                    matrix[j, secondCol] = temp[j];
                }
            }
            else
            {
                throw new Exception("Błędny/e numer/y kolumn do zamiany!");
            }
            return matrix;
        }

        public static Matrix Invert(Matrix matrixAA)
        {

            Matrix matrixAlgebraicComplements = new Matrix(matrixAA.MatrixRows, matrixAA.MatrixCols);
            Matrix matrixTemp = new Matrix(matrixAA.MatrixRows - 1, matrixAA.MatrixCols - 1);
            Matrix matrixACT = new Matrix(matrixAA.MatrixRows, matrixAA.MatrixCols);
            try
            {
                if (matrixAA.MatrixRows == matrixAA.MatrixCols)
                {
                    double detA = DeterminantBareiss(matrixAA);
                    if (detA != 0)
                    {
                        Console.WriteLine(detA.ToString());
                        for (int i = 0; i < matrixAA.MatrixRows; i++)
                            for (int j = 0; j < matrixAA.MatrixCols; j++)
                            {
                                int k = 0;
                                for (int m = 0; m < matrixAA.MatrixRows; m++)
                                {
                                    if (m != i)
                                    {
                                        int l = 0;
                                        for (int n = 0; n < matrixAA.MatrixCols; n++)
                                        {
                                            if (n != j)
                                            {
                                                matrixTemp[k, l++] = matrixAA[m, n];
                                            }
                                        }
                                        k++;
                                    }
                                }
                                matrixAlgebraicComplements[i, j] = Math.Pow((-1), (i + 1 + j + 1)) * DeterminantBareiss(matrixTemp);
                            }
                        matrixACT = Transpose(matrixAlgebraicComplements);
                        for (int i = 0; i < matrixAA.MatrixRows; i++)
                            for (int j = 0; j < matrixAA.MatrixCols; j++)
                                matrixACT[i, j] = matrixACT[i, j] / detA;
                        return matrixACT;
                    }
                    else
                    {
                        throw new Exception("Wyznacznik macierzy równy 0, nie można odwrócić macierzy!");
                    }
                }
                else
                {
                    throw new Exception("Liczba kolumn macierzy A nie jest równa liczbie wierszy!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Matrix MultiplicationByNumber(Matrix matrix, double number)
        {
            for (int i = 0; i < matrix.MatrixRows; i++)
                for (int j = 0; j < matrix.MatrixCols; j++)
                    matrix.MatrixBody[i, j] *= number;
            return matrix;
        }

        public static Matrix AdditionRow(Matrix matrix, int firstRow, int secondRow, double multiplicatorSecondRow)
        {
            for (int j = 0; j < matrix.MatrixCols; j++)
                matrix.MatrixBody[firstRow, j] += multiplicatorSecondRow * matrix.MatrixBody[secondRow, j];
            return matrix;
        }

        public static Matrix AdditionCol(Matrix matrix, int firstCol, int secondCol, double multiplicatorSecondCol)
        {
            for (int i = 0; i < matrix.MatrixRows; i++)
                matrix.MatrixBody[i, firstCol] += multiplicatorSecondCol * matrix.MatrixBody[i, secondCol];
            return matrix;
        }

        public static int MatrixRow(Matrix matrix)
        {
            int matrixRow = 0;
            int row = matrix.MatrixRows;
            int col = matrix.MatrixCols;

            if(matrix.MatrixRows == matrix.MatrixCols)
            {
                if(DeterminantBareiss(matrix) != 0) return row;
                else
                {
                    for(int k = 1; k < row; k++)
                        for(int i = 0; i < row; i++)
                            for(int j = 0; j < col; j++)
                            {
                                Matrix matrixTemp = new Matrix(row-k, col-k, "MatrixTemp");

                                for(int m = 0,  p = 0; m < row-k; m++, p++)
                                    for(int n = 0, r = 0; n < col-k; n++, r++)
                                    {
                                        if (p == i) p++;
                                        if (r == j) r++;
                                        matrixTemp.MatrixBody[m, n] = matrix.MatrixBody[p, r];
                                    }
                                if (DeterminantBareiss(matrixTemp) != 0) return matrixTemp.MatrixRows;
                            }

                }
            }
            else
            {
                throw new Exception("Liczenie rzędu macierzy niekwadratowych jeszcze nie jest obsługiwane!");
            }
        }
    }
}
