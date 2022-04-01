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
using System.Text.RegularExpressions;

namespace Matrix_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        internal ObservableCollection<Matrix> MatrixList = new ObservableCollection<Matrix>();

        public MainWindow()
        {
            InitializeComponent();
            PrepareBinding();
        }

        private void PrepareBinding()
        {
            lvMatrix.ItemsSource = MatrixList;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvMatrix.ItemsSource);
            view.Filter = UserFilter;
        }

        private void btnAddMatrix_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            if (MatrixList != null)
            {
                count = MatrixList.Count();
            }

            if (tbRows.Text.Length > 0 && int.Parse(tbRows.Text) > 0 && tbCols.Text.Length > 0 && int.Parse(tbCols.Text) > 0 && tbName.Text.Length > 0)
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

            if (index != -1)
            {
                var name = $"{MatrixList[index].MatrixName}";
                Matrix matrix = new Matrix((MatrixList[index].MatrixRows), (MatrixList[index].MatrixCols), name);
                MatrixList[index] = ((DataView)gridMatrix.DataContext).ToTable().ToMatrix(matrix);
                lvMatrix.SelectedIndex = index;
            }
        }

        private void btnDeleteMatrix_Click(object sender, RoutedEventArgs e)
        {
            var index = lvMatrix.SelectedIndex;

            if (index != -1)
            {
                MessageBoxResult odpowiedz = MessageBox.Show("Czy skasować macierz: " + MatrixList[index].ToString() + "?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (odpowiedz == MessageBoxResult.Yes) MatrixList.RemoveAt(index);
            }
        }

        private void lvMatrix_KeyDown(object sender, KeyEventArgs e)
        {
            var index = lvMatrix.SelectedIndex;
            if (e.Key == Key.Delete && index != -1)
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
            FileSupport.SaveMatrixesToFile(MatrixList);
        }

        private void btnLoadMatrixes_Click(object sender, RoutedEventArgs e)
        {
            FileSupport.LoadMatrixesFromFile(MatrixList);
        }

        private void tbRows_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void tbCols_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}
