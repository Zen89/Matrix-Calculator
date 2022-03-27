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
        

        Matrix testowy = new Matrix("testowy", 2, 3);
        public MainWindow()
        {
            InitializeComponent();
            PrepareBinding();

            lvMatrix.ItemsSource = MatrixList;
        }

        private void PrepareBinding()
        {
            Matrix newMatrix = new Matrix($"Matrix 0", 3, 2);
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
                        
            int rows = int.Parse(tbRows.Text);
            int cols = int.Parse(tbCols.Text);
            Matrix newMatrix = new Matrix($"Matrix {count}", rows, cols);
            MatrixList.Add(newMatrix);
            MessageBox.Show(newMatrix.ToString());
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
                        int rowsNum = MatrixList[k].MatrixRows;
                        int columnsNum = MatrixList[k].MatrixCols;
                        Matrix matrix = new Matrix("test", rowsNum, columnsNum);
                        DataTable dataTable = matrix.ToDataTable();
                        gridMatrix.DataContext = dataTable.DefaultView;
                    }
                }
                //MessageBox.Show(temp.ToString());
            }
        }
    }
}
