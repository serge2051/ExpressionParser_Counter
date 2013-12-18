using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser_Counter_Console_
{
    public class ExpressionException : Exception
    {
        public ExpressionException()
            : base()
        {
        }

        public ExpressionException(string message)
            : base(message)
        {
        }
    }

    public class ParseExcept : ExpressionException
    {
        public ParseExcept(string message)
            : base(message)
        {
        }
    }

    public class CalcExcept : ExpressionException
    {
        public CalcExcept(string message)
            : base(message)
        {
        }
    }

    public class OperatorExcept : ExpressionException
    {
        public OperatorExcept(string message)
            : base(message)
        {
        }
    }
}
