using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser_Counter_Console_
{
    public class Operator
    {
        public int Priority { get; set; }

        public string Mnemonic { get; set; }

        public string OpName;

        public bool IsBinary; //изначально у нас операция считается бинарной.

        public Operator(string operation, int priority,bool IsBinaryInput = true)
        {
            Priority = priority;
            Mnemonic = operation;
            OpName = "";
            IsBinary = IsBinaryInput;
        }

        public Operator(string operation, string name, int priority)
        {
            Priority = priority;
            Mnemonic = operation;
            OpName = name;
        }

        public Operator()
        {
            Priority = 0;
            Mnemonic = "";
        }
    }
}
