using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Data;
using System.IO;
using CsvHelper;
using System.Globalization;
using Microsoft.Win32;

namespace Matrix_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        internal ObservableCollection<Matrix> MatrixList = new ObservableCollection<Matrix>();


        Matrix testowy = new Matrix(2, 3, "testowy");
        List<string> Namelist = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            PrepareBinding();

            lvMatrix.ItemsSource = MatrixList;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvMatrix.ItemsSource);
            view.Filter = UserFilter;
        }

        private void PrepareBinding()
        {
            Matrix newMatrix = new Matrix(3, 2, $"Matrix 0");
            //MatrixList.Add(newMatrix);
            //MatrixList.Add(testowy);

        }

        private void btnAddMatrix_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            if (MatrixList != null)
            {
                count = MatrixList.Count();
            }

            if (tbRows.Text != null && tbCols.Text != null)
            {
                int rows = int.Parse(tbRows.Text);
                int cols = int.Parse(tbCols.Text);
                string name = tbName.Text;
                Matrix newMatrix = new Matrix(rows, cols, name);
                MatrixList.Add(newMatrix);
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                for (int k = 0; k < MatrixList.Count; k++)
                {
                    if (item.Content.ToString().Contains(MatrixList[k].MatrixName))
                    {
                        Matrix matrix = MatrixList[k];
                        DataTable dataTable = matrix.ToDataTable();
                        gridMatrix.DataContext = dataTable.DefaultView;
                    }
                }
            }
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var index = lvMatrix.SelectedIndex;
            var name = $"{MatrixList[index].MatrixName}";
            Matrix matrix = new Matrix((MatrixList[index].MatrixRows), (MatrixList[index].MatrixCols), name);
            MatrixList[index] = ((DataView)gridMatrix.DataContext).ToTable().ToMatrix(matrix);
            lvMatrix.SelectedIndex = index;
        }

        private void btnDeleteMatrix_Click(object sender, RoutedEventArgs e)
        {
            var index = lvMatrix.SelectedIndex;

            if (lvMatrix != null)
            {
                MessageBoxResult odpowiedz = MessageBox.Show("Czy skasować macierz: " + MatrixList[index].ToString() + "?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (odpowiedz == MessageBoxResult.Yes) MatrixList.RemoveAt(index);
            }
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as Matrix).MatrixName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvMatrix.ItemsSource).Refresh();
        }

        private void btnSaveMatrixes_Click(object sender, RoutedEventArgs e)
        {
            SaveMatrixesToFile();
        }

        private void btnLoadMatrixes_Click(object sender, RoutedEventArgs e)
        {
            LoadMatrixesFromFile();
        }

        private void SaveMatrixesToFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                string path = (new Uri(dialog.FileName)).ToString();
                path.Replace('\\', '/');
                path = path.Substring(8);

                using (var writer = new StreamWriter($"{path}"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<TesteR>();
                    csv.NextRecord();

                    foreach (var matrix in MatrixList)
                    {
                        var record = new TesteR { Row = matrix.MatrixRows, Col = matrix.MatrixCols, Name = matrix.MatrixName };
                        List<double> tab = new List<double>();

                        for (int i = 0; i < matrix.MatrixRows; i++)
                        {
                            for (int j = 0; j < matrix.MatrixCols; j++)
                            {
                                tab.Add(matrix.MatrixBody[i, j]);
                            }
                        }

                        csv.WriteRecord(record);
                        csv.NextRecord();
                        csv.WriteRecords(tab);
                    }
                }
            }
        }

        private void LoadMatrixesFromFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                string path = (new Uri(dialog.FileName)).ToString();
                path.Replace('\\', '/');
                path = path.Substring(8);

                using (var reader = new StreamReader($"{path}"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    while (csv.Read())
                    {
                        List<double> tab = new List<double>();
                        TesteR record = new TesteR();

                        for (int i = 0; i < 3; i++)
                        {
                            record = csv.GetRecord<TesteR>();
                        }

                        int counter = Convert.ToInt32(record.Row) * Convert.ToInt32(record.Col);
                        while (counter > 0 && csv.Read())
                        {
                            tab.Add(csv.GetRecord<double>());
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
                        MatrixList.Add(matrix);
                    }
                }
            }
        }
    }

    public class TesteR
    {
        public string Name { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        //public int Body { get; set; }
    }
}
