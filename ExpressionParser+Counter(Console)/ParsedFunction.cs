using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser_Counter_Console_
{
    class ParsedFunction
    {
        public List<double> Argumens = new List<double>();
        public double Result;
        public string Mnemonic;

        public ParsedFunction(List<double> listInput, string inMnemo)
        {
            Argumens.AddRange(listInput);
            Mnemonic = inMnemo;
        }
    }
}
