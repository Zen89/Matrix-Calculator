using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Calculator
{
    class Matrix
    {
        public string MatrixName { get; set; }
        public int MatrixRows { get; set; }
        public int MatrixCols { get; set; }
        public double[,] MatrixBody;

        public Matrix(string name, int rows, int cols)
        {
            MatrixName = name;
            MatrixRows = rows;
            MatrixCols = cols;
            MatrixBody = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    MatrixBody[i, j] = i*j;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{MatrixName}");
            //for (int i = 0; i < MatrixRows; i++)
            //{
            //    sb.Append("\r");
            //    for (int j = 0; j < MatrixCols; j++)
            //    {
            //        sb.Append($"{MatrixBody[i, j]} ".PadLeft(4));
            //    }
            //}
            return sb.ToString();
        }
    }
}
