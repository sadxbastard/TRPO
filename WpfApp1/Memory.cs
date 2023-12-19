using Calc;
using System.IO;
using System.Data.SQLite;
using Dapper;
using System.Linq;

namespace WpfApp1
{
    public interface IMemory
    {
        void Put(string expression);
        string GetExpression();
        void Delete();
    }
    class RAMMemory : IMemory
    {
        private string _memory;
        private Calculator _calculator;
        public RAMMemory()
        {
            _memory = string.Empty;
            _calculator = new Calculator();
        }

        public void Put(string expression)
        {
            _memory = _calculator.StartCalculating(_memory + expression);
        }
        public string GetExpression()
        {
            return _memory;
        }
        public void Delete()
        {
            _memory = string.Empty;
        }
    }
    class FileMemory : IMemory
    {
        private Calculator _calculator;
        private string _filePath;
        public FileMemory()
        {
            _calculator = new Calculator();
            _filePath = "C:\\Помойка\\Учеба\\ТРПО\\TRPO\\WpfApp1\\FileMemory.txt";
        }
        public void Put(string expression)
        {
            File.WriteAllText(_filePath, _calculator.StartCalculating(expression));
        }
        public string GetExpression()
        {
            return File.ReadAllText(_filePath);
        }
        public void Delete()
        {
            File.WriteAllText(_filePath, string.Empty);
        }
    }
    class DbMemory : IMemory
    {
        private SQLiteConnection _connection;
        private Calculator _calculator;

        public DbMemory()
        {
            _connection = new SQLiteConnection("Data Source=c:\\Помойка\\Учеба\\ТРПО\\TRPO\\db;");
            _calculator = new Calculator();
        }
        public void Put(string expression)
        {
            _connection.Open();
            var id = 1;
            var number = _calculator.StartCalculating(expression);
            var result = _connection.Query<string>("UPDATE expression SET result = :number WHERE id = :id", new { number = number, id = id });
            _connection.Close();
        }

        public string GetExpression()
        {
            _connection.Open();
            var id = 1;
            var result = _connection.QueryFirst<string>("SELECT result FROM expression WHERE id = :id", new { id = id});
            _connection.Close();
            return result;
        }

        public void Delete()
        {
           
        }
    }
}
