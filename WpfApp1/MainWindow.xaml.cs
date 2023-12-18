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
    class MainViewModel : INotifyPropertyChanged
    {

        private string _inputString = string.Empty;
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

            Calculate = new RelayCommand<string>(ch =>
            {
                InputString = Calculator.StartCalculating(_inputString);
            }, ch => string.IsNullOrWhiteSpace(_inputString) == false);
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

        public RelayCommand<string> WritingACharCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand<string> Calculate { get; }
        public RelayCommand<string> OperationCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
