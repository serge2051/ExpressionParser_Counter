using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace ExpressionParser_Counter_Console_
{
    public abstract class CommandLineArgs
    {
        private static StringDictionary _argParameters;

        public static string InputExpressionsFile = @"";

        public static string InputConstantsFile = @"";

        public static string OutputFileName = @"";

        public static void ReadParams(string[] args)
        {
            bool showHelp = (args.Length == 0);
            foreach (string arg in args)
            {
                if ((arg == "/?") || (arg == "-?") || (arg == "--?") || (arg == "--help"))
                {
                    showHelp = true;
                    break;
                }
            }
            if (showHelp)
            {
                Console.WriteLine("ExpressionParser usage: ");
                Console.WriteLine("   Drive:\\Folder\\ExpressionParser.exe -parameter=\"value\" -parameter=\"value\" ...");
                Console.WriteLine("");
                Console.WriteLine("Required parameters: ");
                Console.WriteLine("   -InputExpressions:  file with the expressions(*.txt)");
                Console.WriteLine("   -InputConstants: file with the constants(*.txt)");
                Console.WriteLine("   -Output: name of output file with solution(*.txt)");
                Console.WriteLine("");
                Console.WriteLine("Used operations: ");
                Console.WriteLine("   + - * / % > >= < <= == !=");
                Console.WriteLine("");
                Console.WriteLine("Used funstions: ");
                Console.WriteLine("   sin() cos() tg() ctg() ln() lg() sqrt()");
                Console.WriteLine("");
                Console.WriteLine("Expression's format: ");
                Console.WriteLine("   x=1+2");
                Console.WriteLine("");
                Console.WriteLine("Constant's format: ");
                Console.WriteLine("   pi=3.14");
                Console.ReadKey();
            }

            Arguments(args);

            var param = ReadParameter("InputExpressions", false);
            if (!String.IsNullOrEmpty(param))
            {
                InputExpressionsFile = param;
            }

            param = ReadParameter("InputConstants", false);
            if (!String.IsNullOrEmpty(param))
            {
                InputConstantsFile = param;
            }

            param = ReadParameter("Output", false);
            if (!String.IsNullOrEmpty(param))
            {
                OutputFileName = param;
            }
        }

        private static string ReadParameter(string param, bool isCritical)
        {
            try
            {
                if (_argParameters[param] != null)
                {
                    return _argParameters[param];
                }
            }
            catch (Exception)
            {
                string ex =
                       String.Format(
                           "Reading parameters: \n Some error occured while reading settings from command line arguments. \n");
                Exception(ex);
            }

            // Если и там не находим, а параметр важный - ругаемся и выдаем эксепшн
            if (isCritical)
            {
                string ex =
                      String.Format(
                          "Reading parameters: \n Missing critical parameter: {0} \n", param);
                Exception(ex);
            }

            return null;
        }

        public static void Exception(string p)
        {
            Console.WriteLine(p);
            throw new Exception();
        }

        // Собирает все аргументы коммандной строки и парсит их.
        // <param name="args">Сюда подается список аргументов коммандрой строки
        private static void Arguments(IEnumerable<string> args)
        {
            _argParameters = new StringDictionary();
            var spliter =
                new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var remover =
                new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;
            string[] parts;

            foreach (string txt in args)
            {
                parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                    case 1:
                        if (parameter != null)
                        {
                            if (!_argParameters.ContainsKey(parameter))
                            {
                                parts[0] =
                                    remover.Replace(parts[0], "$1");
                                _argParameters.Add(parameter, parts[0]);
                            }

                            parameter = null;
                        }

                        break;

                    case 2:
                        if (parameter != null)
                        {
                            if (!_argParameters.ContainsKey(parameter))
                            {
                                _argParameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];
                        break;

                    case 3:

                        if (parameter != null)
                        {
                            if (!_argParameters.ContainsKey(parameter))
                            {
                                _argParameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];

                        if (!_argParameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            _argParameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }

            if (parameter != null)
            {
                if (!_argParameters.ContainsKey(parameter))
                {
                    _argParameters.Add(parameter, "true");
                }
            }
        }

        private string this[string param]
        {
            get
            {
                return _argParameters[param];
            }
        }
    }
}
