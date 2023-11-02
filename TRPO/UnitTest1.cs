using FluentAssertions;
using Xunit;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace Calc
{
    public class CalculatorTests
    {
        private Calculator _calc;
        public CalculatorTests()
        {
            _calc = new Calculator();
        }

        [Theory]
        [InlineData("", "0")]
        [InlineData("()", "0")]
        public void Calculation_ShouldReturnCorrectANumberOfTypeDouble_IfSourceIsEmpty(string source, string output)
        {
            _calc.StartCalculating(source).Should().Be(output);
        }

        [Fact]
        public void Calculation_ShouldReturnANumberOfTypeDouble_IfSourceHaveCommas()
        {
            var source = "1.123";
            var result = _calc.StartCalculating(source);
            result.Should().Be("1,123");
        }

        [Theory]
        [InlineData("+123", "123")]
        [InlineData("-123", "-123")]
        [InlineData("(123)","123")]
        [InlineData("((123))", "123")]
        [InlineData("*123", "0")]
        [InlineData("/123", "0")]
        public void Calculation_ShouldReturnANumberOfTypeDouble_IfSourceHaveOperatorsAtTheBeggining(string source, string output)
        {
                var result = _calc.StartCalculating(source);
                result.Should().Be(output);
        }

        [Theory]
        [InlineData("123()", "123")]
        [InlineData("123)))", "123")]
        [InlineData("(((123", "123")]
        [InlineData("()123+123", "246")]
        [InlineData("123(123)", "15129")]
        [InlineData("123(123 + 123)", "30258")]
        [InlineData("123(123 + 123)))))", "30258")]
        [InlineData("123(((((123 + 123)", "30258")]
        [InlineData("123 + 321 abc", "444")]

        public void Calculation_ShouldReturnAnError_IfSourceIsIncorrectlySet(string source, string output)
        {
            var result = _calc.StartCalculating(source);
            result.Should().Be(output);
        }

        [Theory]
        [InlineData(" 1 + 2 * ( 4 / 2 ) ", "5")]
        [InlineData("2+2 * 2/ 2", "4")]
        [InlineData("0/2","0")]
        [InlineData("(4,5+4,5)/2","4,5")]
        [InlineData("1,1+2,2", "3,3")]
        [InlineData("999999999999999999*999999999999999999", "1E+36")]

        public void Calculation_ShouldReturnCorrectANumberOfTypeDouble_IfSourceIsAnExpressionWithPriorityOperators(string source, string output)
        {
            var result = _calc.StartCalculating(source);
            result.Should().Be(output);
        }
    }
    class Calculator
    {
        private string _string = "";
        private int symbol, indexsrc = 0;
        private void GetSymbol()
        {
            if (indexsrc <= _string.Length - 1)
            {
                symbol = _string[indexsrc];
                indexsrc++;
            }
            else
                symbol = '\0';
        }

        public void Handler(string SourceStr)
        {
            _string = SourceStr.Replace(" ", "");
            Regex _regex = new Regex(@"[A-Za-z~!@#$%^&_\|]}[{'\"";:?><]");
            _string = _regex.Replace(_string, string.Empty);

            Stack<char> stack = new Stack<char>();
            SourceStr = "";
            int index;
            foreach (char c in _string)
            {
                if (c == '(')
                {
                    stack.Push(c);
                    SourceStr += c;
                }
                else if (c == ')' && stack.Count > 0)
                {
                    stack.Pop();
                    SourceStr += c;
                }
                else 
                    SourceStr += c;
            }
            while (stack.Count > 0)
            {
                stack.Pop();
                index = SourceStr.IndexOf('(');
                SourceStr = SourceStr.Remove(index, 1);
            }
            _string = SourceStr;

            indexsrc = 0;
            index = 0;

            GetSymbol();
            while (symbol != '\0')
            {
                if (symbol == '(')
                {
                    GetSymbol();
                    index++;
                    if (symbol == ')')
                    {
                        _string = _string.Remove(index, 1);
                        _string = _string.Remove(index - 1, 1);
                    }
                    else if (symbol >= '0' && symbol <= '9')
                    {
                        if (index - 2 >= 0)
                            if (_string[index - 2] >= '0' && _string[index - 2] <= '9')
                            {
                                StringBuilder stringBuilder = new StringBuilder(_string);
                                stringBuilder.Insert(index - 1, "*");
                                _string = stringBuilder.ToString();
                            }
                    }
                }
                GetSymbol();
                index++;
            }
        }

        public string StartCalculating(string SourceStr)
        {
            Handler(SourceStr);
            indexsrc = 0;
            GetSymbol();
            double value = MethodE();
            if (Math.Floor(value) == value)
                return value.ToString();
            else
            {
                string formattedValue = Math.Round(value, 5).ToString("0.#####");
                formattedValue = formattedValue.TrimEnd('0');
                if (formattedValue.EndsWith("."))
                {
                    formattedValue = formattedValue.Substring(0, formattedValue.Length - 1);
                }
                return formattedValue.ToString();
            }
        }
        private double MethodE()
        {
            double x = MethodT();
            while (symbol == '+' || symbol == '-')
            {
                char p = (char)symbol;
                GetSymbol();
                if (p == '+')
                    x += MethodT();
                else
                    x -= MethodT();
            }
            return x;
        }
        private double MethodT()
        {
            double x = MethodM();
            while (symbol == '*' || symbol == '/')
            {
                char p = (char)symbol;
                GetSymbol();
                if (p == '*')
                    x *= MethodM();
                else
                    x /= MethodM();
            }
            return x;
        }
        private double MethodM()
        {
            double x = 0;
            if (symbol == '(')
            {
                GetSymbol();
                if (symbol == ')') return 0;
                x = MethodE();
                GetSymbol();
            }
            else
            {
                if (symbol == '-')
                {
                    GetSymbol();
                    x = -MethodM();
                }
                else if (symbol >= '0' && symbol <= '9')
                    x = MethodC();
            }
            return x;
        }
        private double MethodC()
        {
            string x = "";

            while (symbol >= '0' && symbol <= '9')
            {
                x += (char)symbol;
                GetSymbol();
                if (symbol == ',' || symbol == '.')
                {
                    x += ',';
                    GetSymbol();
                }

            }
            return double.Parse(x);
        }

    }
}
