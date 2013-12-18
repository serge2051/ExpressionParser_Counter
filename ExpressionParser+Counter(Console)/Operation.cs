using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser_Counter_Console_
{
    public class Operation
    {
        public string LeftOperand; //левый операнд
        public string RightOperand; //правый операнд
        public Operator Op; //операция
        public string Result; //результат
        public object ResultNumeric { get; private set; }

        private bool _isCounted = false; //посчитано ли значение выражения
        public bool IsCounted { get { return _isCounted; } private set { _isCounted = value; } }
        public bool IsBinary = true; //изначально у нас операция считается бинарной.

        public void Count(object Res)
        {
            ResultNumeric = Res;
            IsCounted = true;
        }
    }
}
