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

        public static Matrix Invert(Matrix matrixAA)
        {
            Matrix matrixAa = new Matrix(matrixAA.MatrixRows, matrixAA.MatrixCols, matrixAA.MatrixName);
            Matrix matrixTmp = new Matrix(matrixAA.MatrixRows, matrixAA.MatrixCols, matrixAA.MatrixName);
            for (int i = 0; i < matrixAA.MatrixRows; i++)
                for (int j = 0; j < matrixAA.MatrixCols; j++)
                    matrixAa[i, j] = matrixAA[i, j];
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
                for (int n = 0; n < matrixAA.MatrixRows; n++)
                {
                    double p = 1.0;
                    if (n > 0)
                    {
                        p = matrixAa[n - 1, n - 1];
                        for (int i = 0; i < matrixAA.MatrixRows; i++)
                            for (int j = 0; j < matrixAA.MatrixCols; j++)
                                matrixAa[i, j] = matrixTmp[i, j];
                    }

                    for (int i = 0; i < matrixAA.MatrixRows; i++)
                    {
                        if (n == i)
                        {
                            for (int j = 0; j < matrixAA.MatrixCols; j++)
                            {
                                matrixTmp[i, j] = matrixAa[i, j];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < matrixAA.MatrixCols; j++)
                            {
                                matrixTmp[i, j] = (matrixAa[n, n] * matrixAa[i, j] - matrixAa[i, n] * matrixAa[n, j]) / p;
                            }
                        }
                        
                            
                    }
                        
                }
                
                detA = matrixTmp[matrixTmp.MatrixRows-1, matrixTmp.MatrixCols-1];
                Console.WriteLine(detA.ToString());

                return matrixAa;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
