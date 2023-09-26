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
            var source = "1,123";
            var result = _calc.StartCalculating(source);
            result.Should().Be("1.123");
        }
        [Fact]
        public void Calculation_ShouldReturnANumberOfTypeDouble_IfSourceHaveOperatorsAtTheBeggining()
        {
            var arrStr = new string[] { "+123", "*123", "/123" };
            var arrResult = new string[3];
            for (int i = 0; i < arrStr.Length; i++)
            {
                arrResult[i] = _calc.StartCalculating(arrStr[i]);
                arrResult[0].Should().Be("123");
            }
        }
        [Fact]
        public void Calculation_ShouldReturnCorrectANumberOfTypeDouble_IfSourceIsAnExpressionWithPriorityOperators()
        {
            var source = "1+2*(4/2)";
            var result = _calc.StartCalculating(source);
            result.Should().Be("5");
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
            _string = SourceStr;
            indexsrc = 0;
            GetSymbol();
            return MethodE().ToString();

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
                else if (symbol == 'c' || symbol == 's')
                {
                    string namefunc = "";
                    while (symbol != '(')
                    {
                        namefunc += (char)symbol;
                        GetSymbol();

                    }
                }
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