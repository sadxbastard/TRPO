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

        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var textBox = sender as TextBox;
        //    textBox.CaretIndex = textBox.Text.Length;
        //    textBox.ScrollToHorizontalOffset(double.MaxValue);
        //}
    }
    class MainViewModel : INotifyPropertyChanged
    {

        private string _inputString = string.Empty;
        //private string _outputString = string.Empty;
        //private bool _operationFlag = false;
        private Calculator _calculator = new Calculator();

        public MainViewModel()
        {
            WritingACharCommand = new RelayCommand<string>(ch =>
            {
                InputString += ch;
                //if (OperationFlag)
                //{
                //    OutputString = Calculator.StartCalculating(_inputString);
                //}
            });

            ClearCommand = new RelayCommand(() =>
            {
                InputString = string.Empty;
                //OutputString = string.Empty;
                //OperationFlag = false;
            }, () => string.IsNullOrWhiteSpace(_inputString) == false);

            Calculate = new RelayCommand<string>(ch =>
            {
                InputString = Calculator.StartCalculating(_inputString);
                //OutputString = string.Empty;
                //OperationFlag = false;
            }, ch => string.IsNullOrWhiteSpace(_inputString) == false);

            //OperationCommand = new RelayCommand<string>(ch =>
            //{
            //    InputString += ch;
            //    //OperationFlag = true;
            //});
        }

        public string InputString
        {
            get => _inputString;
            set
            {
                _inputString = value;
                OnPropertyChanged();

                ClearCommand.NotifyCanExecuteChanged();
                Calculate.NotifyCanExecuteChanged();
                //OutputString = Calculator.StartCalculating(_inputString);
            }
        }
        //public string OutputString
        //{
        //    get => _outputString;
        //    set
        //    {
        //        _outputString = value;
        //        OnPropertyChanged();

        //        Calculate.NotifyCanExecuteChanged();
        //    }
        //}

        //public bool OperationFlag
        //{
        //    get => _operationFlag;
        //    set
        //    {
        //        _operationFlag = value;
        //    }
        //}
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

        public RelayCommand<string> WritingACharCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand<string> Calculate { get; }
        public RelayCommand<string> OperationCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        //    field = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

    }
}
