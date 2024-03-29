﻿using System;
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
using Microsoft.Extensions.DependencyInjection;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mainViewModel = App.Provider.GetRequiredService<MainViewModel>();
            DataContext = mainViewModel;
        }
    }
    class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _inputString;
        private bool _isValid;
        private IMemory _memory;
        private Calculator _calculator;

        public MainViewModel(IMemory memory)
        {
            _calculator = new Calculator();
            _memory = memory;
            _inputString = "0";

            WritingACharCommand = new RelayCommand<string>(ch =>
            {
                if ((InputString == "0" || InputString == "0*" || InputString == "0+" || InputString == "0/") && ch != "+" && ch != "/" && ch != "*" && ch != ",") InputString = string.Empty;
                else if (InputString is not "0" && ch is "+" or "-" or "/" or "*" && InputString.Length > 0)
                {
                    if (InputString.Length > 1 && InputString[InputString.Length - 1] is '+' or '-' or '/' or '*')
                        InputString = InputString.Substring(0, InputString.Length - 1);
                }
                InputString += ch;
            });

            ClearCommand = new RelayCommand(() =>
            {
                InputString = "0";
            }, () => string.IsNullOrWhiteSpace(_inputString) == false);

            DeleteCommand = new RelayCommand(() =>
            {
                InputString = InputString.Remove(InputString.Length - 1, 1);
                if (InputString is "") { InputString = "0"; }
            }, () => string.IsNullOrWhiteSpace(_inputString) == false);

            Calculate = new RelayCommand<string>(ch =>
            {
                _isValid = true;
                _isValid = IsValid;
                if (_isValid) { InputString = Calculator.StartCalculating(_inputString); }
                else OnPropertyChanged(nameof(InputString));
                
            }, ch => string.IsNullOrWhiteSpace(_inputString) == false);

            ReadMemory = new RelayCommand(() =>
            {
                var ch = InputString[InputString.Length - 1];
                var result = _memory.GetExpression();
                if (ch != '+' && ch != '-' && ch != '/' && ch != '*')
                    InputString = result;
                else InputString += result;
                if (InputString is "") { InputString = "0"; }
            });

            ClearMemory = new RelayCommand(() =>
            {
                _memory.Delete();
            });

            SumMemory = new RelayCommand(() =>
            {
                _memory.Put(_inputString);
            });

            SubMemory = new RelayCommand(() =>
            {
                StringBuilder tempStr = new StringBuilder(_inputString);
                _memory.Put(tempStr.Append("*-1").ToString());
            });
        }

        public string InputString
        {
            get => _inputString;
            set
            {
                _inputString = value;
                Validate();
                OnPropertyChanged();
                WritingACharCommand.NotifyCanExecuteChanged();
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
                return _errors.Values.All(x=> string.IsNullOrEmpty(x));
            }
        }

        public bool Validate()
        {
            bool valueF = false;
            bool commaF = false;
            bool opF = false;
            Stack<char> stack = new Stack<char>();
            _errors.Clear();
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

        public RelayCommand<string> WritingACharCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand<string> Calculate { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ReadMemory {  get; }
        public RelayCommand ClearMemory { get; }
        public RelayCommand SumMemory { get; }
        public RelayCommand SubMemory { get; }


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
