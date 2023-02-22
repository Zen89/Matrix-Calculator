using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Matrix_Calculator
{
    /// <summary>
    /// Provides support for saving and loading matrices to and from CSV files.
    /// </summary>
    public static class FileSupport
    {
        /// <summary>
        /// Saves a list of matrices to a CSV file.
        /// </summary>
        /// <param name="matrixList">The list of matrices to save.</param>
        public static void SaveMatrixesToFile(ObservableCollection<Matrix> matrixList)
        {
            // Create a save file dialog and set the file type filter
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            // If the user selects a file and clicks "Save"
            if (dialog.ShowDialog() == true)
            {
                // Convert the file path to a URI, replace backslashes with forward slashes, and remove the "file:///" prefix
                string path = (new Uri(dialog.FileName)).ToString();
                path.Replace('\\', '/');
                path = path.Substring(8);

                // Open the file for writing and create a CsvWriter to write the matrix data to the file
                using (var writer = new StreamWriter($"{path}"))
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    // Write the header row to the file
                    csv.WriteHeader<MatrixTemp>();
                    csv.NextRecord();

                    // Write each matrix to the file
                    foreach (var matrix in matrixList)
                    {
                        // Create a temporary matrix object to hold the matrix metadata (name, rows, and columns)
                        var record = new MatrixTemp { Row = matrix.MatrixRows, Col = matrix.MatrixCols, Name = matrix.MatrixName };
                        List<double> tab = new List<double>();

                        // Flatten the matrix data into a list of doubles
                        for (int i = 0; i < matrix.MatrixRows; i++)
                        {
                            for (int j = 0; j < matrix.MatrixCols; j++)
                            {
                                tab.Add(matrix.MatrixBody[i, j]);
                            }
                        }

                        try
                        {
                            // Write the matrix metadata to the file
                            csv.WriteRecord(record);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        // Write the flattened matrix data to the file
                        csv.NextRecord();
                        csv.WriteRecords(tab);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a list of matrices from a CSV file.
        /// </summary>
        /// <param name="matrixList">The list to which the matrices will be added.</param>
        public static void LoadMatrixesFromFile(ObservableCollection<Matrix> matrixList)
        {
            // Clear the list of matrices
            matrixList.Clear();

            // Create an open file dialog and set the file type filter
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv";

            // If the user selects a file and clicks "Open"
            if (dialog.ShowDialog() == true)
            {
                // Convert the file path to a URI, replace backslashes with forward slashes, and remove the "file:///" prefix
                string path = (new Uri(dialog.FileName)).ToString();
                path.Replace('\\', '/');
                path = path.Substring(8);

                // Open the file for reading and create a CsvReader to read the matrix data from the file
                using (var reader = new StreamReader($"{path}"))
                using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
                {
                    while (csv.Read())
                    {
                        List<double> tab = new List<double>();
                        MatrixTemp record = new MatrixTemp();

                        // Get the matrix dimensions and name from the first 3 rows of the CSV file
                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                record = csv.GetRecord<MatrixTemp>();
                            }
                            catch { }
                        }

                        // Load the matrix elements from the CSV file
                        int counter = Convert.ToInt32(record.Row) * Convert.ToInt32(record.Col);
                        while (counter > 0 && csv.Read())
                        {
                            try
                            {
                                tab.Add(csv.GetRecord<double>());
                            }
                            catch
                            {
                                tab.Add(0);
                                MessageBox.Show("In matrix was bad element, who was switch to 0");
                            }
                            counter--;
                        }

                        // Create a new Matrix object and populate it with the loaded elements
                        Matrix matrix = new Matrix(record.Row, record.Col, record.Name);
                        for (int i = 0; i < record.Row; i++)
                        {
                            for (int j = 0; j < record.Col; j++)
                            {
                                matrix.MatrixBody[i, j] = tab[counter++];
                            }
                        }
                        // Add the Matrix object to the matrixList
                        matrixList.Add(matrix);
                    }
                }
            }
        }
    }

    public class MatrixTemp
    {
        /// <summary>
        /// Gets or sets the name of the matrix.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of rows in the matrix.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the matrix.
        /// </summary>
        public int Col { get; set; }
        //public int Body { get; set; }

        
    }

    public class ElementsValidate : IDataErrorInfo
    {
        /// <summary>
        /// Gets or sets the name of the matrix.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of rows in the matrix.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the matrix.
        /// </summary>
        public int Col { get; set; }
        //public int Body { get; set; }

        /// <summary>
        /// Gets or sets the index of the first row.
        /// </summary>
        public int FirstRow { get; set; }

        /// <summary>
        /// Gets or sets the index of the second row.
        /// </summary>
        public int SecondRow { get; set; }

        /// <summary>
        /// Gets or sets the multiplier for the row operation.
        /// </summary>
        public double MultiplicatorRow { get; set; }

        /// <summary>
        /// Gets or sets the index of the first column.
        /// </summary>
        public int FirstCol { get; set; }

        /// <summary>
        /// Gets or sets the index of the second column.
        /// </summary>
        public int SecondCol { get; set; }

        /// <summary>
        /// Gets or sets the multiplier for the column operation.
        /// </summary>
        public double MultiplicatorCol { get; set; }

        //TODO replace the validation rules from IDataErrorInfo with INotifyDataErrorInfo

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                return null;
                //throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="nameMatrixProperty">The name of the property to retrieve the error message for.</param>
        /// <returns>The error message for the property with the given name.</returns>
        public string this[string nameMatrixProperty]
        {
            get
            {
                string message = String.Empty;
                switch (nameMatrixProperty)
                {
                    case "Name":
                        if (string.IsNullOrEmpty(Name))
                            message = "Nazwa musi być wpisana.";
                        else if (Name.ToString() == "Matrix")
                            message = "Nazwa zablokowana, wybierz inną nazwę.";
                        break;
                    case "Row":
                        if (Row < 1 || Row.ToString() == null)
                            message = "Liczba musi być większa od 0.";
                        break;
                    case "Col":
                        if (Col < 1 || Col.ToString() == null)
                            message = "Liczba musi być większa od 0.";
                        break;
                };
                return message;
            }
        }
    }
}
