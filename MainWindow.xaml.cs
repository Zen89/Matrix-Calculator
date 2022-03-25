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

namespace Matrix_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ObservableCollection<Matrix> MatrixList = new ObservableCollection<Matrix>();
        internal Collection<Matrix> MatrixCollection = new Collection<Matrix>;
        Matrix testowy = new Matrix("testowy", 4, 4);
        public MainWindow()
        {
            InitializeComponent();
            PrepareBinding();

            lvMatrix.ItemsSource = MatrixList;
            double[,] tablica = new double[testowy.MatrixRows,testowy.MatrixCols];
            Array.Copy(testowy.MatrixBody, tablica, testowy.MatrixBody.Length);


        }

        private void PrepareBinding()
        {
            //MatrixList = new ObservableCollection<Matrix>();
            Matrix newMatrix = new Matrix($"Matrix 0", 4, 4);
            MatrixList.Add(newMatrix);
            MatrixList.Add(testowy);
            double[,] tablica = new double[testowy.MatrixRows, testowy.MatrixCols];
            Array.Copy(testowy.MatrixBody, tablica, testowy.MatrixBody.Length);
            
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
            //MessageBox.Show(newMatrix.ToString());
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                var temp = item.Content.ToString();
                MessageBox.Show(MatrixList.);
            }
        }
    }
}
