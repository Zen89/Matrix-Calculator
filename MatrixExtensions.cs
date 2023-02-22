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
    /// <summary>
    /// Provides extension methods for the Matrix class to convert it to a DataTable object.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Converts the current Matrix object to a DataTable object.
        /// </summary>
        /// <param name="matrix">The current Matrix object to convert.</param>
        /// <returns>A DataTable object representing the current Matrix object.</returns>
        public static DataTable ToDataTable(this Matrix matrix)
        {
            // Create a new DataTable object.
            DataTable table = new DataTable();

            // Add the necessary number of rows and columns to the DataTable.
            for (int i = 0; i < matrix.MatrixRows; i++)
                table.Rows.Add();
            for (int i = 0; i < matrix.MatrixCols; i++)
                table.Columns.Add();

            // Copy the values of the current Matrix object to the corresponding cells of the DataTable.
            for (int i = 0; i < matrix.MatrixRows; i++)
                for (int j = 0; j < matrix.MatrixCols; j++)
                    table.Rows[i][j] = matrix.MatrixBody[i, j].ToString("N2");

            return table;
        }

        /// <summary>
        /// Converts the specified DataTable object to a Matrix object.
        /// </summary>
        /// <param name="table">The DataTable object to convert.</param>
        /// <param name="matrix">The Matrix object to set the values of the DataTable to.</param>
        /// <returns>A new Matrix object with the values set to those of the specified DataTable.</returns>
        public static Matrix ToMatrix(this DataTable table, Matrix matrix)
        {
            // Create a new Matrix object with the same number of rows, columns and name as the specified Matrix object.
            Matrix result = new Matrix(table.Rows.Count, table.Columns.Count, matrix.MatrixName);

            // Copy the values of the DataTable to the Matrix object.
            for (int i = 0; i < table.Rows.Count; i++)
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    try
                    {
                        result.MatrixBody[i, j] = Convert.ToDouble(table.Rows[i][j]);
                    }
                    catch
                    {
                        // If an error occurs while converting the value, set it to 0 and display a warning message.
                        MessageBox.Show($"Bad element {i},{j} was switch to 0");
                        result.MatrixBody[i, j] = 0;
                    }
                }
            return result;
        }
    }
}
