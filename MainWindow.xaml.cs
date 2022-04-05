﻿using System;
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
        enum Operations
        {
            Addition,
            Subtraction,
            Multiplication
        }

        public static ObservableCollection<Matrix> MatrixList = new ObservableCollection<Matrix>();

        public MainWindow()
        {
            InitializeComponent();
            PrepareBinding();
        }

        private void PrepareBinding()
        {
            lvMatrix.ItemsSource = MatrixList;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvMatrix.ItemsSource);
            cbMatrixA.ItemsSource = MatrixList;
            CollectionView viewA = (CollectionView)CollectionViewSource.GetDefaultView(cbMatrixA.ItemsSource);
            cbMatrixB.ItemsSource = MatrixList;
            CollectionView viewB = (CollectionView)CollectionViewSource.GetDefaultView(cbMatrixB.ItemsSource);
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
                if (name == "Matrix") name = "Matrix 0";
                Matrix newMatrix = new Matrix(rows, cols, name);
                MatrixList.Add(newMatrix);
                tbName.Text = $"Matrix {MatrixList.Count}";
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                for (int k = 0; k < MatrixList.Count; k++)
                {
                    try
                    {
                        if (item.Content.ToString().Contains(MatrixList[k].MatrixName))
                        {
                            Matrix matrix = MatrixList[k];
                            DataTable dataTable = matrix.ToDataTable();
                            gridMatrix.DataContext = dataTable.DefaultView;
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
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
            {
                btnSaveChanges.IsEnabled = true;
                btnDeleteMatrix.IsEnabled = true;
                return true;
            }
            else
            {
                btnSaveChanges.IsEnabled = false;
                btnDeleteMatrix.IsEnabled = false;
                return ((item as Matrix).MatrixName.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

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

        private void cbMatrixA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var matrix = cbMatrixA.SelectedItem as Matrix;
            if (matrix != null)
            {
                try
                {
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixA.DataContext = dataTable.DefaultView;
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }
            }
        }

        private void cbMatrixB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var matrix = cbMatrixB.SelectedItem as Matrix;
            if (matrix != null)
            {
                try
                {
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixB.DataContext = dataTable.DefaultView;
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }
            }
        }

        private void CalculateMatricesAndSetResult(Operations operation)
        {
            var matrixA = cbMatrixA.SelectedItem as Matrix;
            var matrixB = cbMatrixB.SelectedItem as Matrix;
            string name = tbMatrixC.Text;
            Matrix matrixC = new Matrix(matrixA.MatrixRows, matrixA.MatrixCols, name);
            try
            {
                switch (operation)
                {
                    case Operations.Addition:
                        matrixC = matrixA + matrixB;
                        break;
                    case Operations.Subtraction:
                        matrixC = matrixA - matrixB;
                        break;
                    //case Operations.Multiplication:
                    //    matrixAB = matrixA * matrixB;
                    //    break;
                }
                matrixC.MatrixName = name;
                DataTable dataTable = matrixC.ToDataTable();
                gridMatrixC.DataContext = dataTable.DefaultView;
                MatrixList.Add(matrixC);
                if(matrixC.MatrixName == tbName.Text) tbName.Text = $"Matrix {MatrixList.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlusMinus_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rbPlus.IsChecked)
            {
                CalculateMatricesAndSetResult(Operations.Addition);
            }
            else if ((bool)rbMinus.IsChecked)
            {
                CalculateMatricesAndSetResult(Operations.Subtraction);
            }
        }

    }
}
