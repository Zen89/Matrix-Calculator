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

namespace Matrix_Calculator
{
    public static class FileSupport
    {
        public static void SaveMatrixesToFile(ObservableCollection<Matrix> matrixList)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;
            //var temp = new C
            //var float_options = new CsvHelper.TypeConversion.TypeConverterOptions
            //{
            //    Formats = new[] { "###.0" }
            //};

            if (dialog.ShowDialog() == true)
            {
                string path = (new Uri(dialog.FileName)).ToString();
                path.Replace('\\', '/');
                path = path.Substring(8);

                using (var writer = new StreamWriter($"{path}"))
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    //csv.Configuration.TypeConverterOptionsCache.AddOptions<float>(float_options);
                    csv.WriteHeader<MatrixTemp>();
                    csv.NextRecord();

                    foreach (var matrix in matrixList)
                    {
                        var record = new MatrixTemp { Row = matrix.MatrixRows, Col = matrix.MatrixCols, Name = matrix.MatrixName };
                        List<double> tab = new List<double>();

                        for (int i = 0; i < matrix.MatrixRows; i++)
                        {
                            for (int j = 0; j < matrix.MatrixCols; j++)
                            {
                                tab.Add(matrix.MatrixBody[i, j]);
                            }
                        }

                        try
                        {
                            csv.WriteRecord(record);
                        }
                        catch (Exception ex)
                        { 
                            Console.WriteLine(ex.ToString()); 
                        }
                        csv.NextRecord();
                        csv.WriteRecords(tab);
                    }
                }
            }
        }

        public static void LoadMatrixesFromFile(ObservableCollection<Matrix> matrixList)
        {
            matrixList.Clear();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv";

            //var float_options = new CsvHelper.TypeConversion.TypeConverterOptions
            //{
            //    Formats = new[] { "###.0" }
            //};

            if (dialog.ShowDialog() == true)
            {
                string path = (new Uri(dialog.FileName)).ToString();
                path.Replace('\\', '/');
                path = path.Substring(8);

                using (var reader = new StreamReader($"{path}"))
                using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
                {
                    while (csv.Read())
                    {
                        List<double> tab = new List<double>();
                        MatrixTemp record = new MatrixTemp();

                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                record = csv.GetRecord<MatrixTemp>();
                            }
                            catch { }
                        }

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
                            }
                            counter--;
                        }

                        Matrix matrix = new Matrix(record.Row, record.Col, record.Name);
                        for (int i = 0; i < record.Row; i++)
                        {
                            for (int j = 0; j < record.Col; j++)
                            {
                                matrix.MatrixBody[i, j] = tab[counter++];
                            }
                        }
                        matrixList.Add(matrix);
                    }
                }
            }
        }
    }

    public class MatrixTemp : IDataErrorInfo
    {
        public string Name { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        //public int Body { get; set; }

        //TODO replace the validation rules from IDataErrorInfo with INotifyDataErrorInfo
        public string Error
        {
            get
            {
                return null;
                //throw new NotImplementedException();
            }
        }

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
