using System;
using System.Collections.Generic;

namespace ExpressionParser_Counter_Console_
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandLineArgs.ReadParams(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            List<string> input = new List<string>();
            Dictionary<string, string> Const = new Dictionary<string, string>();
            if (CommandLineArgs.InputExpressionsFile != "")
            {
                Console.WriteLine("Expressions:");
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(CommandLineArgs.InputExpressionsFile);
                while ((line = file.ReadLine()) != null)
                {
                    Console.WriteLine("  "+line);
                    if (!line.Contains("="))
                    {
                        throw new ParseExcept("This line can't be parsed(" + line + ")");
                    }
                    input.Add(line);
                }
                file.Close();
            }
            else
            {
                Console.WriteLine("How string: ");
                int how = int.Parse(Console.ReadLine());
                for (int i = 0; i < how; i++)
                {
                    Console.WriteLine("Enter the " + (i + 1) + " str");
                    string line = Console.ReadLine();
                    if (!line.Contains("="))
                    {
                        throw new ParseExcept("This line can't be parsed(" + line + ")");
                    }
                    input.Add(line);
                } 
            }

            if (CommandLineArgs.InputConstantsFile != "")
            {
                Console.WriteLine("Constants:");
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(CommandLineArgs.InputConstantsFile);
                while ((line = file.ReadLine()) != null)
                {
                    Console.WriteLine("  " + line);
                    Const.Add((line.Remove(line.IndexOf("="))).Replace(" ", ""), (line.Remove(0, line.IndexOf("=") + 1)).Replace(" ", ""));
                }
                file.Close();
            }
            else
            {
                Console.WriteLine("How constants: ");
                int how = int.Parse(Console.ReadLine());
                for (int i = 0; i < how; i++)
                {
                    Console.WriteLine("Enter the " + (i + 1) + " const");
                    string tmp = Console.ReadLine();
                    Const.Add((tmp.Remove(tmp.IndexOf("="))).Replace(" ", ""),(tmp.Remove(0, tmp.IndexOf("=")+1)).Replace(" ", ""));
                }
            }
            
            //Const.Add("pi", "3.14");
            
            Parser parserr = new Parser(Const);
            List<Operation> output = new List<Operation>();

            for (int i = 0; i < input.Count; ++i)
            {
                input[i]=parserr.ParseFunctions(input[i]);
            }
            output = parserr.Parse(input);

            if (CommandLineArgs.OutputFileName != "")
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(CommandLineArgs.OutputFileName))
                {
                    for (int i = 0; i < output.Count; ++i)
                    {
                        file.Write(output[i].LeftOperand + output[i].Op.Mnemonic + output[i].RightOperand + " = " +
                                      output[i].Result);
                        if (output[i].IsCounted)
                            file.Write(" = " + output[i].ResultNumeric);
                        file.WriteLine();
                    }
                }
            }
            else
            {

                if ((output.Count == 1) && (output[0].IsCounted))
                {
                    Console.WriteLine("All operations are counted successefull! ");
                    Console.WriteLine(output[0].Result + " = " + output[0].LeftOperand + output[0].Op.Mnemonic +
                                      output[0].RightOperand + " = " + output[0].ResultNumeric);
                }
                else
                    for (int i = 0; i < output.Count; ++i)
                    {
                        Console.Write(output[i].LeftOperand + output[i].Op.Mnemonic + output[i].RightOperand + " = " +
                                      output[i].Result);
                        if (output[i].IsCounted)
                            Console.Write(" = " + output[i].ResultNumeric);
                        Console.WriteLine();
                    }
            }
            Console.ReadLine();
        }
    }
}
