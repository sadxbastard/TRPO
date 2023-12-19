using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Calc;
using CommunityToolkit.Mvvm.Input;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            
        }
    }
    class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {

        private string _inputString = string.Empty;
        private bool _isValid;
        private Calculator _calculator = new Calculator();

        public MainViewModel()
        {
            WritingACharCommand = new RelayCommand<string>(ch =>
            {
                InputString += ch;
            });

            ClearCommand = new RelayCommand(() =>
            {
                InputString = string.Empty;
            }, () => string.IsNullOrWhiteSpace(_inputString) == false);

            DeleteCommand = new RelayCommand(() =>
            {
                InputString = InputString.Remove(InputString.Length - 1, 1);
            }, () => string.IsNullOrWhiteSpace(_inputString) == false);

            Calculate = new RelayCommand<string>(ch =>
            {
                _isValid = true;
                _isValid = IsValid;
                if (_isValid) { InputString = Calculator.StartCalculating(_inputString); }
                else OnPropertyChanged(nameof(InputString));
                
            }, ch => string.IsNullOrWhiteSpace(_inputString) == false);
        }

        public string InputString
        {
            get => _inputString;
            set
            {
                _inputString = value;

                _errors[nameof(InputString)] = null;
                OnPropertyChanged();
                ClearCommand.NotifyCanExecuteChanged();
                DeleteCommand.NotifyCanExecuteChanged();
                Calculate.NotifyCanExecuteChanged();
            }
        }
        public Calculator Calculator
        {
            get => _calculator;
            set
            {
                _calculator = value;
                OnPropertyChanged();
                
                Calculate.NotifyCanExecuteChanged();
            }
        }

        public bool IsValid
        {
            get
            {
                bool valueF = false;
                bool commaF = false;
                bool opF = false;
                Stack<char> stack = new Stack<char>();
                foreach (char c in InputString)
                {
                    if (c == '(')
                    {
                        stack.Push(c);
                    }
                    else if (c == ')')
                    {
                        if (stack.Count > 0)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            _errors[nameof(InputString)] = "Не хватает открывающей скобки";
                            return false;
                        }
                    }
                    if (c == ',')
                    {
                        if (commaF && !opF)
                        {
                            _errors[nameof(InputString)] = "Невалидное выражение";
                            return false;
                        }
                        if (!valueF)
                        {
                            _errors[nameof(InputString)] = "Перед запятой не хватает числа";
                            return false;
                        }
                        commaF = true;
                        valueF = false;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        valueF = true;
                        opF = false;
                    }
                    else if (c is '+' or '-' or '/' or '*')
                    {
                        if (commaF && !valueF)
                        {
                            _errors[nameof(InputString)] = "После запятой не хватает числа";
                            return false;
                        }
                        opF = true;
                        commaF = false;
                        valueF = false;
                    }
                }

                if (stack.Count > 0)
                {
                    _errors[nameof(InputString)] = "Не хватает закрывающей скобки";
                    return false;
                }
                return _isValid;
            }
            set
            {
                _isValid = value;
                OnPropertyChanged();
                Calculate.NotifyCanExecuteChanged();
            }
        }

        public RelayCommand<string> WritingACharCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand<string> Calculate { get; }
        public RelayCommand DeleteCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public string Error
        {
            get
            {
                return string.Join(Environment.NewLine, _errors.Values);
            }

        }

        public string this[string columnName]
        {
            get
            {
                return _errors.TryGetValue(columnName, out var value) ? value : string.Empty;
            }
        }
    }
}
