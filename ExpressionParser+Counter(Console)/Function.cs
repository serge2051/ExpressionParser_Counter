using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser_Counter_Console_
{
    class Function
    {
        public int HowArguments { get; set; }

        public string Mnemonic { get; set; }

        public Function(string input_func, int HowArgumentsinFunc)
        {
            this.HowArguments = HowArgumentsinFunc;
            this.Mnemonic = input_func + "(";
        }
    }
}
