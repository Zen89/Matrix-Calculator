using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    table.Rows[i][j] = matrix[i, j];

            return table;
        }
    }
}
