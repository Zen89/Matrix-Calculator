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
    /// Interaction logic for the main window of the Matrix Calculator application.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The available operations for the calculator.
        /// </summary>
        enum Operations
        {
            Addition,
            Subtraction,
            Multiplication
        }

        /// <summary>
        /// The list of matrices currently loaded into the calculator.
        /// </summary>
        public static ObservableCollection<Matrix> MatrixList = new ObservableCollection<Matrix>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            PrepareBinding();
        }

        /// <summary>
        /// Prepares the data binding for the matrix list and its views.
        /// </summary>
        private void PrepareBinding()
        {
            lvMatrix.ItemsSource = MatrixList;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvMatrix.ItemsSource);
            cbMatrixA.ItemsSource = MatrixList;
            CollectionView viewA = (CollectionView)CollectionViewSource.GetDefaultView(cbMatrixA.ItemsSource);
            cbMatrixB.ItemsSource = MatrixList;
            CollectionView viewB = (CollectionView)CollectionViewSource.GetDefaultView(cbMatrixB.ItemsSource);
            cbMatrixAA.ItemsSource = MatrixList;
            CollectionView viewAA = (CollectionView)CollectionViewSource.GetDefaultView(cbMatrixAA.ItemsSource);
            view.Filter = UserFilter;
        }

        /// <summary>
        /// Adds a new matrix to the list based on the input values.
        /// </summary>
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

        /// <summary>
        /// Displays the selected matrix in the grid view.
        /// </summary>
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

        /// <summary>
        /// Handles the button click event to save changes made to a matrix.
        /// </summary>
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

        /// <summary>
        /// Handles the button click event to delete a matrix from the matrix list.
        /// </summary>
        private void btnDeleteMatrix_Click(object sender, RoutedEventArgs e)
        {
            var index = lvMatrix.SelectedIndex;

            if (index != -1)
            {
                MessageBoxResult odpowiedz = MessageBox.Show("Czy skasować macierz: " + MatrixList[index].ToString() + "?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (odpowiedz == MessageBoxResult.Yes) MatrixList.RemoveAt(index);
            }
        }

        /// <summary>
        /// Handles the key down event to delete a matrix from the matrix list when the delete key is pressed.
        /// </summary>
        private void lvMatrix_KeyDown(object sender, KeyEventArgs e)
        {
            var index = lvMatrix.SelectedIndex;
            if (e.Key == Key.Delete && index != -1)
            {
                MessageBoxResult odpowiedz = MessageBox.Show("Czy skasować macierz: " + MatrixList[index].ToString() + "?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (odpowiedz == MessageBoxResult.Yes) MatrixList.RemoveAt(index);
            }
        }

        /// <summary>
        /// Filters the matrix list based on user input.
        /// </summary>
        /// <param name="item">The matrix item to be filtered.</param>
        /// <returns>True if the matrix name contains the user input string, false otherwise.</returns>
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

        /// <summary>
        /// Handles the text changed event to filter the matrix list based on user input.
        /// </summary>
        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvMatrix.ItemsSource).Refresh();
        }

        /// <summary>
        /// Handles the click event of the Save Matrixes button. Saves the MatrixList to a file using the FileSupport class.
        /// </summary>
        private void btnSaveMatrixes_Click(object sender, RoutedEventArgs e)
        {
            FileSupport.SaveMatrixesToFile(MatrixList);
        }

        /// <summary>
        /// Handles the click event of the Load Matrixes button. Loads MatrixList from a file using the FileSupport class and sets the text of the tbName TextBox to "Matrix [MatrixList.Count]".
        /// </summary>
        private void btnLoadMatrixes_Click(object sender, RoutedEventArgs e)
        {
            FileSupport.LoadMatrixesFromFile(MatrixList);
            tbName.Text = $"Matrix {MatrixList.Count}";
        }

        /// <summary>
        /// Handles the PreviewTextInput event of the tbRows TextBox. Prevents non-numeric characters from being entered in the tbRows TextBox.
        /// </summary>
        private void tbRows_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Handles the PreviewTextInput event of the tbCols TextBox. Prevents non-numeric characters from being entered in the tbCols TextBox.
        /// </summary>
        private void tbCols_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cbMatrixA ComboBox. Converts the selected Matrix object to a DataTable object and sets the DataContext of the gridMatrixA DataGrid to a DataView object based on the DataTable object.
        /// </summary>
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

        /// <summary>
        /// Handles the SelectionChanged event of the cbMatrixB ComboBox. Converts the selected Matrix object to a DataTable object and sets the DataContext of the gridMatrixB DataGrid to a DataView object based on the DataTable object.
        /// </summary>
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

        /// <summary>
        /// Calculates the result of the operation between the two selected matrices and sets the result to gridMatrixC
        /// </summary>
        /// <param name="operation">The operation to be performed: Addition, Subtraction or Multiplication</param>
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
                    case Operations.Multiplication:
                        matrixC = matrixA * matrixB;
                        break;
                }
                matrixC.MatrixName = name;
                DataTable dataTable = matrixC.ToDataTable();
                gridMatrixC.DataContext = dataTable.DefaultView;
                MatrixList.Add(matrixC);
                if (matrixC.MatrixName == tbName.Text) tbName.Text = $"Matrix {MatrixList.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the click event for the Calculate button and performs the operation selected by the user
        /// </summary>
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rbPlus.IsChecked)
            {
                CalculateMatricesAndSetResult(Operations.Addition);
            }
            else if ((bool)rbMinus.IsChecked)
            {
                CalculateMatricesAndSetResult(Operations.Subtraction);
            }
            else if ((bool)rbMulti.IsChecked)
            {
                CalculateMatricesAndSetResult(Operations.Multiplication);
            }
        }

        /// <summary>
        /// Handles the selection changed event for the matrix selection ComboBox cbMatrixAA and displays the selected matrix in gridMatrixAA
        /// </summary>
        private void cbMatrixAA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            if (matrix != null)
            {
                try
                {
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixAA.DataContext = dataTable.DefaultView;
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }
            }
        }

        /// <summary>
        /// Handles the click event for the Transpose button and displays the transposed matrix in gridMatrixD
        /// </summary>
        private void btnTranspose_Click(object sender, RoutedEventArgs e)
        {
            var matrixAA = cbMatrixAA.SelectedItem as Matrix;
            string name = tbMatrixD.Text;
            Matrix matrixAT = new Matrix(matrixAA.MatrixCols, matrixAA.MatrixRows, name);
            try
            {
                matrixAT = Matrix.Transpose(matrixAA);
                matrixAT.MatrixName = name;
                DataTable dataTable = matrixAT.ToDataTable();
                gridMatrixD.DataContext = dataTable.DefaultView;
                MatrixList.Add(matrixAT);
                if (matrixAT.MatrixName == tbName.Text) tbName.Text = $"Matrix {MatrixList.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Inverts the selected matrix, sets its name to the value entered in the name textbox, and displays it in the datagrid.
        /// </summary>
        private void btnInvert_Click(object sender, RoutedEventArgs e)
        {
            var matrixAA = cbMatrixAA.SelectedItem as Matrix;
            string name = tbMatrixD.Text;
            Matrix matrixAT = new Matrix(matrixAA.MatrixCols, matrixAA.MatrixRows, name);
            try
            {
                matrixAT = Matrix.Invert(matrixAA);
                matrixAT.MatrixName = name;
                DataTable dataTable = matrixAT.ToDataTable();
                gridMatrixD.DataContext = dataTable.DefaultView;
                MatrixList.Add(matrixAT);
                if (matrixAT.MatrixName == tbName.Text) tbName.Text = $"Matrix {MatrixList.Count}";
                //Console.WriteLine(matrixAT.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Calculates the algebraic complements of the selected matrix, sets its name to the value entered in the name textbox, and displays it in the datagrid.
        /// </summary>
        private void btnAlgebraicComplements_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            string name = tbMatrixD.Text;
            Matrix matrixAD = new Matrix(matrix.MatrixCols, matrix.MatrixRows, name);
            try
            {
                matrixAD = Matrix.AlgebraicComplements(matrix);
                matrixAD.MatrixName = name;
                DataTable dataTable = matrixAD.ToDataTable();
                gridMatrixD.DataContext = dataTable.DefaultView;
                MatrixList.Add(matrixAD);
                if (matrixAD.MatrixName == tbName.Text) tbName.Text = $"Matrix {MatrixList.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Calculates the transpose of the algebraic complements of the selected matrix, sets its name to the value entered in the name textbox, and displays it in the datagrid.
        /// </summary>
        private void btnAttached_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            string name = tbMatrixD.Text;
            Matrix matrixADT = new Matrix(matrix.MatrixCols, matrix.MatrixRows, name);
            try
            {
                matrixADT = Matrix.Transpose(Matrix.AlgebraicComplements(matrix));
                matrixADT.MatrixName = name;
                DataTable dataTable = matrixADT.ToDataTable();
                gridMatrixD.DataContext = dataTable.DefaultView;
                MatrixList.Add(matrixADT);
                if (matrixADT.MatrixName == tbName.Text) tbName.Text = $"Matrix {MatrixList.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the button click event for row switching operation
        /// </summary>
        private void btnRowSwitch_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            int firstRow = 0;
            int secondRow = 0;

            try
            {
                if (int.TryParse(tbFirstRow.Text, out firstRow) && int.TryParse(tbSecondRow.Text, out secondRow))
                {
                    firstRow--;
                    secondRow--;
                    matrix = Matrix.RowSwitch(matrix, firstRow, secondRow);
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixAA.DataContext = dataTable.DefaultView;
                }
                else
                {
                    throw new Exception("Źle wprowadzony/e numer/y wierszy do zamiany!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the button click event for column switching operation
        /// </summary>
        private void btnColSwitch_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            int firstCol = 0;
            int secondCol = 0;

            try
            {
                if (int.TryParse(tbFirstCol.Text, out firstCol) && int.TryParse(tbSecondCol.Text, out secondCol))
                {
                    firstCol--;
                    secondCol--;
                    matrix = Matrix.ColSwitch(matrix, firstCol, secondCol);
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixAA.DataContext = dataTable.DefaultView;
                }
                else
                {
                    throw new Exception("Źle wprowadzony/e numer/y kolumn do zamiany!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the button click event for matrix multiplication by number operation
        /// </summary>
        private void btnMultiByNum_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            int number = 0;

            try
            {
                if(tbMultiNum.Text != null)
                {
                    int.TryParse(tbMultiNum.Text, out number);
                    matrix = Matrix.MultiplicationByNumber(matrix, number);
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixAA.DataContext = dataTable.DefaultView;
                }
                else
                {
                    throw new Exception("Brak liczby do przemnożenia macierzy!");
                }
            }
                
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Calculates the determinant of the selected matrix and displays it in the corresponding textbox.
        /// </summary>
        private void btnCalcDeterminant_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            double determinant = Matrix.DeterminantBareiss(matrix);
            lblDetText.Visibility = Visibility.Visible;
            tbDeterminant.Text = determinant.ToString();
        }

        /// <summary>
        /// Handles the PreviewTextInput event for the first row textbox of a matrix, allowing only numeric input.
        /// </summary>
        private void tbFirstRowAdd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Handles the PreviewTextInput event for the second row textbox of a matrix, allowing only numeric input.
        /// </summary>
        private void tbSecondRowAdd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Handles the PreviewTextInput event for the textbox used for adding a row to a matrix, allowing only numeric and minus sign input.
        /// </summary>
        private void tbNumberRowAdd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, @"[^-0-9]+$");
        }

        /// <summary>
        /// Event handler for the "Add Row" button. Adds two rows in a matrix with a given multiplication factor and updates the UI.
        /// </summary>
        private void btnRowAdd_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            int firstRow = 0;
            int secondRow = 0;
            double multiplicator = 0;
            string swichDotwithComma = tbNumberRowAdd.Text.Replace('.', ',');
            try
            {
                if (int.TryParse(tbFirstRowAdd.Text, out firstRow) && int.TryParse(tbSecondRowAdd.Text, out secondRow) && double.TryParse(swichDotwithComma, out multiplicator))
                {
                    firstRow--;
                    secondRow--;
                    matrix = Matrix.AdditionRow(matrix, firstRow, secondRow, multiplicator);
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixAA.DataContext = dataTable.DefaultView;
                }
                else
                {
                    throw new Exception("Źle wprowadzony/e numer/y wierszy lub mnożnik!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for the preview text input of the first column textbox. Allows only digits to be entered.
        /// </summary>
        private void tbFirstColAdd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Event handler for the preview text input of the second column textbox. Allows only digits to be entered.
        /// </summary>
        private void tbSecondColAdd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Handles the PreviewTextInput event of the tbNumberColAdd control.
        /// </summary>
        private void tbNumberColAdd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, @"[^-0-9]+$");
        }

        /// <summary>
        /// Handles the Click event of the btnColAdd control.
        /// </summary>
        private void btnColAdd_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            int firstCol = 0;
            int secondCol = 0;
            double multiplicator = 0;
            string swichDotwithComma = tbNumberRowAdd.Text.Replace('.', ',');
            try
            {
                if (int.TryParse(tbFirstColAdd.Text, out firstCol) && int.TryParse(tbSecondColAdd.Text, out secondCol) && double.TryParse(swichDotwithComma, out multiplicator))
                {
                    firstCol--;
                    secondCol--;
                    matrix = Matrix.AdditionCol(matrix, firstCol, secondCol, multiplicator);
                    DataTable dataTable = matrix.ToDataTable();
                    gridMatrixAA.DataContext = dataTable.DefaultView;
                }
                else
                {
                    throw new Exception("Źle wprowadzony/e numer/y kolumn lub mnożnik!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCalcMatrixRow control.
        /// </summary>
        private void btnCalcMatrixRow_Click(object sender, RoutedEventArgs e)
        {
            var matrix = cbMatrixAA.SelectedItem as Matrix;
            Matrix matrixTmp = new Matrix(matrix.MatrixRows, matrix.MatrixCols);
            int matrixRow = Matrix.MatrixRow(matrix, out matrixTmp);
            lblMRowText.Visibility = Visibility.Visible;
            tbMatrixRow.Text = matrixRow.ToString();
            DataTable dataTable = matrixTmp.ToDataTable();
            gridMatrixD.DataContext = dataTable.DefaultView;

        }
    }
}
