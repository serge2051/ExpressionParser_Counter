using System;
using System.Collections.Generic;

namespace ExpressionParser_Counter_Console_
{
    public class Parser
    {
        private readonly List<Operator> _operationsList = new List<Operator>();
        readonly List<Function> _functionList = new List<Function>();
        private readonly int _maxPriority;
        private readonly Dictionary<string, string> _constants = new Dictionary<string, string>();
        private readonly Operator _noneOperation = new Operator(); //для того, если нету операции, то она заменяется "пустой"

        public Parser()
        {
            _maxPriority = 4;

            _operationsList.Add(new Operator("+", 2));
            _operationsList.Add(new Operator("-", 2));
            _operationsList.Add(new Operator("*", 1));
            _operationsList.Add(new Operator("/", 1));
            _operationsList.Add(new Operator(">", 4));
            _operationsList.Add(new Operator("%", 1));
            _operationsList.Add(new Operator("<", 4));
            _operationsList.Add(new Operator(">=", 4));
            _operationsList.Add(new Operator("<=", 4));
            _operationsList.Add(new Operator("==", 4));
            _operationsList.Add(new Operator("!=", 4));
            _operationsList.Add(new Operator("-", 2,false)); //унарная операция

            _functionList.Add(new Function("sin", 1));
            _functionList.Add(new Function("cos", 1));
            _functionList.Add(new Function("tg", 1));
            _functionList.Add(new Function("ctg", 1));
            _functionList.Add(new Function("lg", 1));
            _functionList.Add(new Function("ln", 1));
            _functionList.Add(new Function("sqrt", 1));
        }

        public Parser(List<Operator> inputOperators)
        {
            _operationsList.AddRange(inputOperators);
            foreach (Operator tmp in inputOperators)
            {
                if (tmp.Priority > _maxPriority)
                    _maxPriority = tmp.Priority;
            }
        }

        public Parser(Dictionary<string, string> inputConstants)
        {
            foreach (var tmp in inputConstants.Keys)
            {
                _constants.Add(tmp, inputConstants[tmp]);
            }
            _operationsList.Add(new Operator("+", 2));
            _operationsList.Add(new Operator("-", 2));
            _operationsList.Add(new Operator("*", 1));
            _operationsList.Add(new Operator("/", 1));
            _operationsList.Add(new Operator(">", 4));
            _operationsList.Add(new Operator("%", 1));
            _operationsList.Add(new Operator("<", 4));
            _operationsList.Add(new Operator(">=", 4));
            _operationsList.Add(new Operator("<=", 4));
            _operationsList.Add(new Operator("==", 4));
            _operationsList.Add(new Operator("!=", 4));
            _operationsList.Add(new Operator("-", 2, false)); //унарная операция

            foreach (Operator tmp in _operationsList)
            {
                if (tmp.Priority > _maxPriority)
                    _maxPriority = tmp.Priority;
            }

            _functionList.Add(new Function("sin", 1));
            _functionList.Add(new Function("cos", 1));
            _functionList.Add(new Function("tg", 1));
            _functionList.Add(new Function("ctg", 1));
            _functionList.Add(new Function("lg", 1));
            _functionList.Add(new Function("ln", 1));
            _functionList.Add(new Function("sqrt", 1));
        }

        public Parser(Dictionary<string, string> inputConstants, List<Operator> inputOperators)
        {
            foreach (var tmp in inputConstants.Keys)
            {
                _constants.Add(tmp, inputConstants[tmp]);
            }

            _operationsList.AddRange(inputOperators);
            foreach (Operator tmp in inputOperators)
            {
                if (tmp.Priority > _maxPriority)
                    _maxPriority = tmp.Priority;
            }

            _functionList.Add(new Function("sin", 1));
            _functionList.Add(new Function("cos", 1));
            _functionList.Add(new Function("tg", 1));
            _functionList.Add(new Function("ctg", 1));
            _functionList.Add(new Function("lg", 1));
            _functionList.Add(new Function("ln", 1));
            _functionList.Add(new Function("sqrt", 1));
        }

        public void AddOperation(Operator inputOperator)
        {
            _operationsList.Add(inputOperator);
        }

        private IEnumerable<Operation> ParseExpression(string inputExpression, int howIsCreate)
        {
            List<Operation> output = new List<Operation>();
            _operationsList.Sort((first, next) => first.Mnemonic.Length.CompareTo(next.Mnemonic.Length));
            _operationsList.Reverse();
            inputExpression = inputExpression.Replace(" ", "");
            inputExpression = " " + inputExpression + " ";
            foreach (var tmpOp in _operationsList)
            {
                int pos = inputExpression.IndexOf(tmpOp.Mnemonic);
                int prevpos = -1;
                while (pos > 0)
                {
                    if ((inputExpression[pos - 1] != ' ') && (inputExpression[pos + tmpOp.Mnemonic.Length] != ' ') && (Math.Abs(pos - prevpos) > 1))
                    {
                        inputExpression = inputExpression.Remove(pos, tmpOp.Mnemonic.Length);
                        inputExpression = inputExpression.Insert(pos, " " + tmpOp.Mnemonic + " ");
                    }
                    prevpos = pos;
                    pos = inputExpression.IndexOf(tmpOp.Mnemonic, pos + 1);
                }
                //if (!inputExpression.Contains(" " + tmpOp.Mnemonic))
                //    inputExpression = inputExpression.Replace(tmpOp.Mnemonic, " " + tmpOp.Mnemonic + " ");
            }
            if (!ContainsAnyOperators(inputExpression))
            {
                //inputExpression = inputExpression.Replace(inputExpression, GetName(output.Count, howIsCreate));
                inputExpression = inputExpression.Replace(" ", "");
                Operation tmp = new Operation();
                tmp.Result = inputExpression;
                tmp.LeftOperand = inputExpression;
                tmp.RightOperand = "0";
                tmp.Op = _noneOperation;
                //if (!ContainsOnlyDigits(inputExpression))
                //{
                //    if (!_constants.ContainsKey(tmp.Result))
                //        throw new ParseExcept("Result Name not defineded (" + tmp.Result + ")");
                //}
                output.Add(tmp);
                return output;
            }
            while (ContainsAnyOperators(inputExpression))
            {
                //string firstOperatorMnemonic = ""; //мнемоническое обозначение первого оператора
                int firstOpIndex = inputExpression.Length; //номер первого попавшегося оператора
                bool flag = false; //есть ли хотя бы один с таким приоритетом
                Operator WhichOpInExpression= new Operator();

                for (int i = 1; i <= _maxPriority; ++i)
                {
                    List<Operator> currPriorityOperators = new List<Operator>();
                    currPriorityOperators.AddRange(_operationsList.FindAll(k => k.Priority == i));
                    
                    currPriorityOperators.Sort((first, next) => first.Mnemonic.Length.CompareTo(next.Mnemonic.Length));
                    currPriorityOperators.Reverse();
                    foreach (var tmpOp in currPriorityOperators)
                    {
                        int tmpCounter;
                        if (tmpOp.IsBinary)
                            tmpCounter= inputExpression.IndexOf(" " + tmpOp.Mnemonic + " ");
                        else
                        {
                            int index = inputExpression.IndexOf(tmpOp.Mnemonic);
                            if (inputExpression[index + 1] == ' ')
                                continue;
                            tmpCounter = index;
                        }

                        if ((tmpCounter < firstOpIndex) && (tmpCounter > 0))
                        {
                            WhichOpInExpression = tmpOp;
                            firstOpIndex = tmpCounter;
                            flag = true;
                            if (!tmpOp.IsBinary)
                                break;
                        }
                    }
                    if (flag)
                    {
                        string action="";
                        if (WhichOpInExpression.IsBinary)
                        {
                            int pos2 = inputExpression.IndexOf(' ',
                                firstOpIndex + WhichOpInExpression.Mnemonic.Length + 2);
                            int pos1 = inputExpression.LastIndexOf(' ', firstOpIndex - 2);
                            action = inputExpression.Substring(pos1 + 1, pos2 - pos1 - 1);
                            firstOpIndex = action.IndexOf(WhichOpInExpression.Mnemonic + " ");
                                //чтобы отличить от просто отрицательного числа
                            var tmpOp = new Operation
                            {
                                Op = WhichOpInExpression,
                                LeftOperand = action.Substring(0, firstOpIndex - 1),
                                RightOperand = action.Substring(firstOpIndex + WhichOpInExpression.Mnemonic.Length + 1),
                                Result = "$" + (output.Count + 1 + howIsCreate) + "$"
                            };
                            output.Add(tmpOp);
                        }
                        else
                        {
                            int pos2 = inputExpression.IndexOf(' ');
                            int pos1 = inputExpression.IndexOf(' ',pos2+1);
                            action = inputExpression.Substring(pos2+1,pos1-pos2-1);
                            //firstOpIndex = action.IndexOf(WhichOpInExpression.Mnemonic);
                            var tmpOp = new Operation
                            {
                                Op = WhichOpInExpression,
                                LeftOperand = "0",
                                RightOperand = action.Replace(WhichOpInExpression.Mnemonic,""),
                                Result = "$" + (output.Count + 1 + howIsCreate) + "$"
                            };
                            output.Add(tmpOp);
                        }

                        inputExpression = inputExpression.Replace(action, GetName(output.Count, howIsCreate));
                        break;
                    }
                }
            }
            return output;
        }

        private string GetName(int count, int lastIndex)
        {
            int tmp = count + lastIndex;
            return "$" + tmp + "$";
        }

        private List<Operation> Parse(string inputExpression, int lastIndex)
        {
            List<Operation> output = new List<Operation>();
            string answer = inputExpression.Substring(0, inputExpression.IndexOf('='));
            inputExpression = inputExpression.Remove(0, inputExpression.IndexOf('=') + 1);
            answer = answer.Replace(" ", "");
            while (inputExpression.Contains(")"))
            {
                int pos2 = inputExpression.IndexOf(')');
                int pos1 = inputExpression.LastIndexOf('(', pos2 - 2);
                string oneBracket = inputExpression.Substring(pos1 + 1, pos2 - pos1 - 1);
                //Output.Add(OneBracket);
                if (ContainsAnyOperators(oneBracket))
                {
                    output.AddRange(ParseExpression(oneBracket, output.Count + lastIndex));

                    inputExpression = inputExpression.Replace("(" + oneBracket + ")", GetName(output.Count, lastIndex));
                }
                else
                {
                    inputExpression = inputExpression.Replace("(" + oneBracket + ")", oneBracket);
                }
            }
            output.AddRange(ParseExpression(inputExpression, output.Count + lastIndex));
            //Console.Write(Output[0].Result);
            output[output.Count - 1].Result = answer;
            return output;
        }

        private void Calculate_Operation(Operation output)
        {
            var op = output;
            if (ContainsOnlyDigits(op.LeftOperand) && ContainsOnlyDigits(op.RightOperand))
            {
                op.Count(CountExpression(op));
            }
        }

        private static bool ProcessOperation(int i, List<Operation> output)
        {
            if (output[i] == null)
                return false;
            Operation op = output[i];
            bool flag;
            flag = false;
            for (int j = i; j < output.Count; ++j)
            {
                var operation = output[j];
                if (operation.LeftOperand.Replace("-","") == op.Result)
                {
                    flag = true;
                    if (op.IsCounted)
                        operation.LeftOperand = operation.LeftOperand.Replace(op.Result,op.ResultNumeric.ToString());
                    //else
                    //    operation.LeftOperand = op.Result;
                }

                if (operation.RightOperand.Replace("-", "") == op.Result)
                {
                    flag = true;
                    if (op.IsCounted)
                        operation.RightOperand = operation.RightOperand.Replace(op.Result, op.ResultNumeric.ToString());
                    //else
                    //    operation.RightOperand = op.Result;
                }
            }
            if (flag && op.IsCounted)
            {
                op.Result = op.ResultNumeric.ToString();
            }
            else
                flag = false;
            return flag;
        }

        private object CountExpression(Operation input)
        {
            if (!input.IsBinary && (input.Op.Mnemonic == "-"))
            {
                if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                    return Double.Parse(input.LeftOperand.Replace(".", ","));
                return Int64.Parse(input.LeftOperand);
            }

            switch (input.Op.Mnemonic)
            {
                case "+":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(",") || input.RightOperand.Contains(".") || input.RightOperand.Contains(","))
                            return Double.Parse(input.LeftOperand.Replace(".", ",")) + Double.Parse(input.RightOperand.Replace(".", ","));
                        return Int64.Parse(input.LeftOperand) + Int64.Parse(input.RightOperand);
                    }
                case "-":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(",") || input.RightOperand.Contains(".") || input.RightOperand.Contains(","))
                            return Double.Parse(input.LeftOperand.Replace(".", ",")) - Double.Parse(input.RightOperand.Replace(".", ","));
                        return Int64.Parse(input.LeftOperand) - Int64.Parse(input.RightOperand);
                    }
                case "*":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(",") || input.RightOperand.Contains(".") || input.RightOperand.Contains(","))
                            return Double.Parse(input.LeftOperand.Replace(".", ",")) * Double.Parse(input.RightOperand.Replace(".", ","));
                        return Int64.Parse(input.LeftOperand) * Int64.Parse(input.RightOperand);
                    }

                case "/":
                    {
                            return Double.Parse(input.LeftOperand.Replace(".", ",")) / Double.Parse(input.RightOperand.Replace(".", ","));
                    }
                case "%":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(",") || input.RightOperand.Contains(".") || input.RightOperand.Contains(","))
                            return Double.Parse(input.LeftOperand.Replace(".", ",")) % Double.Parse(input.RightOperand.Replace(".", ","));
                        return Int64.Parse(input.LeftOperand) % Int64.Parse(input.RightOperand);
                    }
                case "":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                            return Double.Parse(input.LeftOperand.Replace(".", ","));
                        return Int64.Parse(input.LeftOperand);
                    }
                case "<":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                            return (Double.Parse(input.LeftOperand.Replace(".", ","))<Double.Parse(input.RightOperand.Replace(".", ",")));
                        return Int64.Parse(input.LeftOperand) < Int64.Parse(input.RightOperand);
                    }
                case ">":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                            return (Double.Parse(input.LeftOperand.Replace(".", ",")) > Double.Parse(input.RightOperand.Replace(".", ",")))?1:0;
                        return (Int64.Parse(input.LeftOperand) > Int64.Parse(input.RightOperand))?1:0;
                    }
                case "<=":
                    {
                        if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                            return (Double.Parse(input.LeftOperand.Replace(".", ",")) <= Double.Parse(input.RightOperand.Replace(".", ","))) ? 1 : 0;
                        return Int64.Parse(input.LeftOperand) <= Int64.Parse(input.RightOperand) ? 1 : 0;
                    }
                case ">=":
                {
                    if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                        return (Double.Parse(input.LeftOperand.Replace(".", ",")) >=
                                Double.Parse(input.RightOperand.Replace(".", ","))) ? 1 : 0;
                    return Int64.Parse(input.LeftOperand) >= Int64.Parse(input.RightOperand) ? 1 : 0;
                }
                case "!=":
                {
                    if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                        return (Double.Parse(input.LeftOperand.Replace(".", ",")) !=
                                Double.Parse(input.RightOperand.Replace(".", ","))) ? 1 : 0;
                    return Int64.Parse(input.LeftOperand) != Int64.Parse(input.RightOperand) ? 1 : 0;
                }
                case "==":
                {
                    if (input.LeftOperand.Contains(".") || input.LeftOperand.Contains(","))
                        return (Double.Parse(input.LeftOperand.Replace(".", ",")) ==
                                Double.Parse(input.RightOperand.Replace(".", ","))) ? 1 : 0;
                    return Int64.Parse(input.LeftOperand) == Int64.Parse(input.RightOperand) ? 1 : 0;
                }
                default:
                    {
                        throw new OperatorExcept("This operation is not counted(" + input.Op + ")");
                    }
            }
        }

        private bool ContainsOnlyDigits(string input)
        {
            if (input.LastIndexOf('.') != input.IndexOf('.'))
                return false;
            if (input.LastIndexOf(',') != input.IndexOf(','))
                return false;
            if (input.LastIndexOf('-') != input.IndexOf('-'))
                return false;

            for (int i = 0; i < input.Length; ++i)
            {
                if ((input[i] < '0') || (input[i] > '9'))
                    if (input[i] != '-')
                        if (!((input[i] == '.') && (input[i] != ',') || ((input[i] != '.') && (input[i] == ','))))
                            return false;
            }
            return true;
        }

        public bool ContainsAnyOperators(string inputExpression)
        {
            foreach (var tmpOp in _operationsList)
            {
                if (inputExpression.Contains(tmpOp.Mnemonic))
                    return true;
            }
            return false;
        }

        //содержит ли входящая строка какую-либо функцию
        private bool ContainsAnyFunctions(string inputExpression)
        {
            foreach (Function tmpfunc in _functionList)
            {
                if (inputExpression.Contains(tmpfunc.Mnemonic))
                    return true;
            }
            return false;
        }

        public List<Operation> Parse(List<string> inputExpressions)
        {
            List<Operation> output = new List<Operation>();

            for (int i = 0; i < inputExpressions.Count; ++i)
            {
                output.AddRange(Parse(inputExpressions[i], output.Count));
            }

            int q = 0;
            while (q < output.Count)
            {
                if (_constants.ContainsKey(output[q].RightOperand))
                    output[q].RightOperand = _constants[output[q].RightOperand];
                if (_constants.ContainsKey(output[q].LeftOperand))
                    output[q].LeftOperand = _constants[output[q].LeftOperand];
                if (_constants.ContainsKey(output[q].Result))
                    throw new ParseExcept("Result Name equal Constant Name (" + output[q].Result + ")");
                Calculate_Operation(output[q]);

                if (ProcessOperation(q, output))
                    output.RemoveAt(q);
                else
                    q++;
            }

            return output;
        }

        public object Calculate(string formula)
        {
            List<Operation> output = Parse(new List<string> { formula });
            if ((!output[output.Count - 1].IsCounted) || (output.Count != 1))
                throw new CalcExcept("Not all operations can be calculated.");
            return output[0].ResultNumeric;
        }

        public string ParseFunctions(string inputExpression)
        {
            //List<Operation> Output = new List<Operation>();
            inputExpression = inputExpression.Replace(" ", "");
            inputExpression = " " + inputExpression + " ";

            while (ContainsAnyFunctions(inputExpression))
            {
                int indexOfFirst1 = -1;
                int indexOfFirst2 = -1;
                string firstMnemonic = "";
                for (int i = 0; i < _functionList.Count; ++i)
                {
                    indexOfFirst1 = inputExpression.IndexOf(_functionList[i].Mnemonic);
                    if (indexOfFirst1 != -1)
                    {
                        firstMnemonic = _functionList[i].Mnemonic; break;
                    }
                }
                indexOfFirst2 = inputExpression.IndexOf(")", indexOfFirst2 + 1);
                string funcAll = inputExpression.Substring(indexOfFirst1, indexOfFirst2 - indexOfFirst1 + 1);
                string tmp;
                List<double> args = new List<double>();
                tmp = funcAll.Replace(firstMnemonic, "");
                tmp = tmp.Replace(")", "");
                while (tmp.Contains(","))
                {
                    string oneArg = tmp.Substring(0, tmp.IndexOf(","));
                    args.Add(double.Parse(tmp));
                    tmp = tmp.Replace(oneArg + ",", "");
                }
                args.Add(double.Parse(tmp));
                ParsedFunction parsFunc = new ParsedFunction(args, firstMnemonic);
                inputExpression = inputExpression.Replace(funcAll, CountFunction(parsFunc).ToString());
            }
            return inputExpression;
        }

        private double CountFunction(ParsedFunction input)
        {
            switch (input.Mnemonic)
            {
                case "sin(":
                    {
                        return (Math.Sin(input.Argumens[0]));
                    }
                case "cos(":
                    {
                        return (Math.Cos(input.Argumens[0]));
                    }
                case "tg(":
                    {
                        return (Math.Tan(input.Argumens[0]));
                    }
                case "ctg(":
                    {
                        return (1 / Math.Tan(input.Argumens[0]));
                    }
                case "ln(":
                    {
                        return (Math.Log(input.Argumens[0]));
                    }
                case "lg(":
                    {
                        return (Math.Log10(input.Argumens[0]));
                    }
                case "sqrt(":
                    {
                        return (Math.Sqrt(input.Argumens[0]));
                    }
                default:
                    {
                        throw new OperatorExcept("This funstion is not counted(" + input.Mnemonic + ")");
                    }
            }
        }
    }
}
