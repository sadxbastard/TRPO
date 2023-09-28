using FluentAssertions;
using Xunit;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Calc
{
    public class CalculatorTests
    {
        private Calculator _calc;
        public CalculatorTests()
        {
            _calc = new Calculator();
        }

        [Fact]
        public void Calculation_ShouldReturnCorrectANumberOfTypeDouble_IfSourceIsEmpty()
        {
            var source = string.Empty;
            var result = _calc.StartCalculating(source);
            result.Should().Be("0");
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
        [InlineData("*123", "0")]
        [InlineData("/123", "0")]
        public void Calculation_ShouldReturnANumberOfTypeDouble_IfSourceHaveOperatorsAtTheBeggining(string source, string output)
        {
                var result = _calc.StartCalculating(source);
                result.Should().Be(output);
        }

        [Theory]
        [InlineData(" 1 + 2 * ( 4 / 2 ) ", "5")]
        [InlineData("2+2 * 2/ 2", "4")]
        [InlineData("0/2","0")]
        [InlineData("(4,5+4,5)/2","4,5")]
        public void Calculation_ShouldReturnCorrectANumberOfTypeDouble_IfSourceIsAnExpressionWithPriorityOperators(string source, string output)
        {
            var result = _calc.StartCalculating(source);
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("()", "Expression error")]
        [InlineData("123()", "Expression error")]
        [InlineData("()123+123", "Expression error")]
        [InlineData("123(123)", "Expression error")]
        [InlineData("123 + 321 abc", "Expression error")]
        public void Calculation_ShouldReturnAnError_IfSourceIsUncorrect(string source, string output)
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

        public string StartCalculating(string SourceStr)
        {
            _string = SourceStr.Replace(" ", "");
            indexsrc = 0;
            GetSymbol();
            try
            {
                return MethodE().ToString();
            }
            catch(Exception ex)
            {
                return ex.Message;
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
                if (symbol == ')') throw new Exception("Expression error");
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
                if (symbol >= 'a' && symbol <= 'z')
                    throw new Exception("Expression error");
                if (symbol == '(') throw new Exception("Expression error");
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
