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
                    MatrixBody[i, j] = i+j;
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
                if(matrixA.MatrixRows == matrixB.MatrixRows && matrixA.MatrixCols == matrixB.MatrixCols)
                {
                    for(int i = 0; i < matrixA.MatrixRows; i++)
                        for(int j = 0; j < matrixA.MatrixCols; j++)
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
    }
}
