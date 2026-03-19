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
using System.Collections.Generic;
using System.Xml.Serialization;


namespace Calc
{
    public partial class MainWindow : Window
    {
        private double _c = 0.0;
        private int _d = 0;
        private string _op = "";
        private bool _nn = true;
        private string _ex = "";

        private List<double> _num = new List<double>();
        private List<string> _oper = new List<string>();

        private string _fun = "";

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

                    if (!string.IsNullOrEmpty(_op) && !_ex.EndsWith(" "))
                    {
                        _ex += " ";
                    }
                }

                switch (btn.Tag)
                {
                    case "pm":
                        _c = -_c;
                        int lastSpaceIndex = _ex.LastIndexOf(' ');
                        if (lastSpaceIndex >= 0)
                        {
                            _ex = _ex.Substring(0, lastSpaceIndex + 1) + _c.ToString();
                        }
                        else
                        {
                            _ex = _c.ToString();
                        }
                        Display.Text = _ex;
                        break;

                    case ".": _d = 1; _ex += ","; break;
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
                            _ex += digit.ToString();
                            break;
                        }
                }
                Display.Text = _ex;
            }
        }

        private void Command_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = (string)btn.Tag;

                if (tag == "inv" || tag == "sqrt")
                {
                    if (!string.IsNullOrEmpty(_fun) && !_nn)
                    {
                        ApplyFunctionToCurrentNumber();
                    }

                    if (_num.Count == 0 && !_nn)
                    {
                        _num.Add(_c);
                    }

                    _fun = tag;

                    string functionSymbol = GetFunctionSymbol(tag);
                    _ex += " " + functionSymbol;

                    Display.Text = _ex;
                    _nn = true;
                    return;
                }

                if (!string.IsNullOrEmpty(_fun) && !_nn)
                {
                    ApplyFunctionToCurrentNumber();
                    _fun = "";
                }

                if (_num.Count == 0 && !_nn)
                {
                    _num.Add(_c);
                }
                else if (!_nn)
                {
                    _num.Add(_c);
                    _oper.Add(_op);
                }

                _op = tag;
                _nn = true;

                string opSymbol = GetOperatorSymbol(tag);
                _ex += " " + opSymbol + " ";

                Display.Text = _ex;
            }
        }

        private void ApplyFunctionToCurrentNumber()
        {
            switch (_fun)
            {
                case "inv":
                    if (_c != 0)
                    {
                        _c = 1 / _c;
                    }
                    else
                    {
                        MessageBox.Show("Деление на ноль", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;

                case "sqrt":
                    if (_c >= 0)
                    {
                        _c = Math.Sqrt(_c);
                    }
                    else
                    {
                        MessageBox.Show("Корень из отрицательного числа", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
            }

            if (_num.Count == 0)
            {
                _num.Add(_c);
            }
        }

        private void ProcessHighPriorityOperations()
        {
            for (int i = _oper.Count - 1; i >= 0; i--)
            {
                if (_oper[i] == "*" || _oper[i] == "/")
                {
                    double res = 0;

                    if (_oper[i] == "*")
                    {
                        res = _num[i] * _num[i + 1];
                    }
                    else if (_oper[i] == "/")
                    {
                        if (_num[i + 1] != 0)
                        {
                            res = _num[i] / _num[i + 1];
                        }
                        else
                        {
                            MessageBox.Show("Деление на ноль", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    _num[i] = res;
                    _num.RemoveAt(i + 1);
                    _oper.RemoveAt(i);
                }
            }
        }

        private double CalculateResult()
        {
            if (_num.Count == 0) return 0;

            double result = _num[0];

            for (int i = 0; i < _oper.Count; i++)
            {
                if (_oper[i] == "+")
                {
                    result += _num[i + 1];
                }
                else if (_oper[i] == "-")
                {
                    result -= _num[i + 1];
                }
            }

            return result;
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_fun) && !_nn)
            {
                ApplyFunctionToCurrentNumber();
                _fun = "";
            }

            if (!_nn)
            {
                _num.Add(_c);
                if (!string.IsNullOrEmpty(_op))
                {
                    _oper.Add(_op);
                }
            }

            if (_num.Count > 0)
            {
                ProcessHighPriorityOperations();

                _c = CalculateResult();
                _ex = _c.ToString();
                Display.Text = _ex;
            }
            
            _nn = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _c = 0;
            _d = 0;
            _op = "";
            _nn = true;
            _ex = "";
            _num.Clear();
            _oper.Clear();
            _fun = "";
            Display.Text = "0";
        }

        private string GetOperatorSymbol(string tag)
        {
            switch (tag)
            {
                case "+": return "+";
                case "-": return "-";
                case "*": return "×";
                case "/": return "÷";
                default: return "";
            }
        }

        private string GetFunctionSymbol(string tag)
        {
            switch (tag)
            {
                case "inv": return "1/";
                case "sqrt": return "√";
                default: return "";
            }
        }
    }
}