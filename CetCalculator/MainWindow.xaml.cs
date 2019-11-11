using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CetCalculator
{
    public enum Operation { None, Sum, Subtract, Divide, Multiply}
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Func<double, double, double> _operation;
        private double _lastNumber;
        private bool _firstEntry = true;
        private bool _fromEquals = false;

        public MainWindow()
        {
            InitializeComponent();

            this.SetDefaultOperation();
        }

        private void SetDefaultOperation()
        {
            _operation = (num1, num2) => num1 + num2;
        }

        private double ParseCurrentNumber()
        {
            string text = this.CurrentNumber.Text;

            if (text == "" || text == "-")
                return 0;

            try
            {
                if (text[0] == '-')
                    return -int.Parse(text.Substring(1));

                return double.Parse(text);
            }
            catch (FormatException)
            {
                return int.Parse(text.Remove(text.Length - 1));
            }
            catch (OverflowException)
            {
                MessageBox.Show("Int sınırları dışında bir sayı girilemez.");

                return 0;
            }
        }

        private void SetLastNumber(double lastNumber)
        {
            this.LastNumber.Text = lastNumber.ToString();
            _lastNumber = lastNumber;
        }

        private void ClearCurrent()
        {
            this.CurrentNumber.Text = "";
        }

        private string GetCurrentText()
        {
            return this.CurrentNumber.Text;
        }

        private void SetCurrentText(double value)
        {
            this.SetCurrentText(value.ToString());
        }

        private void SetCurrentText(string value)
        {
            if (value == "" || value == "-")
                this.CurrentNumber.Text = "0";
            else
                this.CurrentNumber.Text = value;
        }

        private void AppendToCurrentText(string value)
        {
            string text = this.GetCurrentText();

            if (text == "0")
                this.SetCurrentText(value);
            else
                this.SetCurrentText(text + value);
        }

        private void FromEqualsDigitClick(string clicked)
        {
            string text = this.GetCurrentText();

            this.SetCurrentText(clicked);

            this.SetLastNumber(0);
            this.SetDefaultOperation();

            _fromEquals = false;
        }

        private void FromEqualsOperatorClick(Func<double, double, double> operation)
        {
            if (_firstEntry)
                _firstEntry = false;

            double current = this.ParseCurrentNumber();

            this.SetLastNumber(current);
            this.ClearCurrent();
            this._operation = operation;
        }

        private void OperationForCurrent(Func<double, double> operation)
        {
            double current = this.ParseCurrentNumber();

            double result = operation(current);

            this.SetCurrentText(result);
        }

        private bool IsEmptyOrZero()
        {
            return this.CurrentNumber.Text == "" || this.CurrentNumber.Text == "0";
        }

        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            
            string clicked = button.Content.ToString();

            if (_fromEquals)
                FromEqualsDigitClick(clicked);
            else
                this.AppendToCurrentText(clicked);
        }

        private void OperationClicked(Func<double, double, double> operation)
        {
            if (this.IsEmptyOrZero())
                return;
            if (_fromEquals)
            {
                this.FromEqualsOperatorClick(operation);

                _fromEquals = false;

                return;
            }

            double current = this.ParseCurrentNumber();

            if (_firstEntry)
            {
                this.SetLastNumber(current);

                _firstEntry = false;
            }
            else
            {
                double result = _operation(_lastNumber, current);

                this.SetLastNumber(result);
            }

            this.ClearCurrent();

            _operation = operation;
        }

        private void OpSum_Click(object sender, RoutedEventArgs e)
        {
            OperationClicked((num1, num2) => num1 + num2);
        }

        private void OpSubtract_Click(object sender, RoutedEventArgs e)
        {
            OperationClicked((num1, num2) => num1 - num2);
        }

        private void OpMultiply_Click(object sender, RoutedEventArgs e)
        {
            OperationClicked((num1, num2) => num1 * num2);
        }

        private void OpDivide_Click(object sender, RoutedEventArgs e)
        {
            OperationClicked((num1, num2) => num1 / num2);
        }

        private void OpEqual_Click(object sender, RoutedEventArgs e)
        {
            if (_fromEquals)
                return;

            double current = this.ParseCurrentNumber();

            double result = _operation(_lastNumber, current);

            this.SetCurrentText(result);
            this.SetLastNumber(result);
            this.SetDefaultOperation();

            _fromEquals = true;
        }

        private void BackSpace_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.IsEmptyOrZero())
                return;

            string text = this.GetCurrentText();

            this.SetCurrentText(text.Remove(text.Length - 1));
        }

        private void CButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.SetCurrentText(0);
            this.SetLastNumber(0);
            this.SetDefaultOperation();
        }

        private void CEButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.SetCurrentText(0);
        }

        private void ChangeSignButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.IsEmptyOrZero())
                return;

            double current = this.ParseCurrentNumber();

            this.SetCurrentText(-current);
        }

        private void CommaButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.IsEmptyOrZero())
            {
                this.SetCurrentText("0,");

                return;
            }
            if (_fromEquals)
            {
                this.SetLastNumber(0);
                this.SetCurrentText("0,");
                this.SetDefaultOperation();
                _fromEquals = false;

                return;
            }

            string text = this.GetCurrentText();

            if (text.Contains(","))
                return;

            this.SetCurrentText(text + ",");
        }

        private void Modulus_OnClick(object sender, RoutedEventArgs e)
        {
            OperationClicked((num1, num2) => num1 % num2);
        }


        private void SquareRoot_OnClick(object sender, RoutedEventArgs e)
        {
            OperationForCurrent(Math.Sqrt);
        }


        private void Square_OnClick(object sender, RoutedEventArgs e)
        {
            OperationForCurrent(num => num * num);
        }

        private void OneOverX_OnClick(object sender, RoutedEventArgs e)
        {
            OperationForCurrent(num => 1 / num);
        }
    }
}
