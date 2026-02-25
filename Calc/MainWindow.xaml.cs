using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;

namespace Calc
{
    public partial class MainWindow : Window
    {
        private double _c = 0.0;
        private double _s = 0.0;    
        private int _d = 0;
        private string _op = "";
        private bool _nn = true;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Digit_Click(object sender, RoutedEventArgs e)
        {

            if (sender is Button btn)
            {

                if (_nn)
                {
                    _c = 0;
                    _d = 0;
                    _nn = false;
                }
                switch (btn.Tag)
                {
                    case "pm": _c = -_c; break;
                    case ".": _d = 1; break;
                    default:
                        {
                            var digit = Double.Parse(btn.Tag.ToString());
                            if (_d == 0)
                            {
                                _c = (Math.Sign(_c) == 0 ? 1 : Math.Sign(_c)) * (Math.Abs(_c) * 10 + digit);
                            }
                            else
                            {
                                _d *= 10;
                                _c = (Math.Sign(_c) == 0 ? 1 : Math.Sign(_c)) * (Math.Abs(_c) + digit / _d);
                            }
                            break;
                        }
                }
                Display.Text = _c.ToString();
            }
        }
        private void Command_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string command = btn.Tag.ToString();

                if (!string.IsNullOrEmpty(_op) && !_nn)
                {
                    Calculate();
                }
                _s = _c;
                _op = command;
                _nn = true;
            }
        }

        private void Calculate()
        {
            switch (_op)
            {
                case "+": _s = _s + _c; break;
                case "-": _s = _s - _c; break;
                case "*": _s = _s * _c; break;
                case "/":
                    if (_c != 0)
                        _s = _s / _c;
                    else
                    {
                        MessageBox.Show("Деление на ноль невозможно!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                case "inv":
                    if (_c != 0)
                        _s = 1 / _c;
                    else
                    {
                        MessageBox.Show("Деление на ноль невозможно!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                case "sqrt":
                    if (_c >= 0)
                        _s = Math.Sqrt(_c);
                    else
                    {
                        MessageBox.Show("Корень из отрицательного числа невозможен!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                default: _s = _c; break;
            }
            _c = _s;
            Display.Text = _c.ToString();
            _nn = true;
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_op))
            {
                Calculate();
                _op = "";
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _c = 0;
            _s = 0;
            _d = 0;
            Display.Text = "0";
        }
    }
}