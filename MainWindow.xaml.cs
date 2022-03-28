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
            MatrixList.Add(newMatrix);
            MatrixList.Add(testowy);
            
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
                Matrix newMatrix = new Matrix(rows, cols, $"Matrix {count}");
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
            Matrix rec = new Matrix(5, 4);
            List<double> tab = new List<double>();
            //int counter = 0;
            for (int i = 0; i < rec.MatrixRows; i++)
            {
                for (int j = 0; j < rec.MatrixCols; j++)
                {
                    tab.Add(rec.MatrixBody[i, j]);
                }
            }
            var records = new List<TesteR>
            {
                new TesteR{ Name = rec.MatrixName, Row = rec.MatrixRows, Col = rec.MatrixCols },
                new TesteR{ Name = rec.MatrixName, Row = rec.MatrixRows, Col = rec.MatrixCols },
            };

            using (var writer = new StreamWriter("file.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<TesteR>();
                csv.NextRecord();
                foreach (var record in records)
                {
                    csv.WriteRecord(record);
                    foreach (var recordd in tab)
                    {
                        csv.WriteRecord(recordd);
                    }
                    csv.NextRecord();
                }
            }
        }
    }

    public class TesteR
    {
        public string Name { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int Body { get; set; }
    }
}
