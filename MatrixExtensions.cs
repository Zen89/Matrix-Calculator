using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Matrix_Calculator
{
    public static class MatrixExtensions
    {
        public static DataTable ToDataTable(this Matrix matrix)
        {
            DataTable table = new DataTable();

            for (int i = 0; i < matrix.MatrixRows; i++)
                table.Rows.Add();
            for (int i = 0; i < matrix.MatrixCols; i++)
                table.Columns.Add();

            for (int i = 0; i < matrix.MatrixRows; i++)
                for (int j = 0; j < matrix.MatrixCols; j++)
                    table.Rows[i][j] = matrix.MatrixBody[i, j].ToString("N2");

            return table;
        }

        public static Matrix ToMatrix(this DataTable table, Matrix matrix)
        {
            Matrix result = new Matrix(table.Rows.Count, table.Columns.Count, matrix.MatrixName);
            
            for (int i = 0; i < table.Rows.Count; i++)
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    try
                    {
                        result.MatrixBody[i, j] = Convert.ToDouble(table.Rows[i][j]);
                    }
                    catch
                    {
                        MessageBox.Show($"Bad element {i},{j} was switch to 0");
                        result.MatrixBody[i, j] = 0;
                    }
                }
            return result;
        }
    }
}
