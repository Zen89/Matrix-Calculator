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

            if (tbRows.Text != null && tbCols != null)
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
    }
}
