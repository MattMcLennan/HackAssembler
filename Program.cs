using System;
using System.Collections.Generic;
using System.IO;

namespace HackAssembler
{

    enum ExitCode : int
    {
        Success = 0,
        InvalidFilename = 1,
        UnknownError = 10
    }

    enum CommandType {
        A,
        C,
        L
    }

    class Parser
    {
        public static List<string> ParseFile(string inputFile)
        {
            var result = new List<string>();

            foreach (var line in File.ReadLines(@inputFile))
            {
                if (IsEmptyLine(line) || IsCommentLine(line))
                {
                    continue;
                }

                result.Add(ParseCommentOut(line).Trim());
            }

            return result;
        }

        private static string ParseCommentOut(string line)
        {
            int commentLocation = line.IndexOf("//");

            if (commentLocation > 0)
            {
                return line.Substring(0, commentLocation);
            }

            return line;
        }

        private static bool IsEmptyLine(string line)
        {
            return string.IsNullOrWhiteSpace(line);
        }

        private static bool IsCommentLine(string line)
        {
            return line.StartsWith("//");
        }

        private static bool HasMoreCommands(string line, int currentPosition)
        {
            return currentPosition < line.Length;
        }

        private static CommandType CommandTypeIs(string command)
        {
            if (command.StartsWith("@"))
            {
                return CommandType.A;
            }

            return CommandType.C;
        }

        private static string Symbol(string symbol)
        {
            throw new NotImplementedException();
        }

        private static string GetDestCmd(string command)
        {
            int end = command.IndexOf("=");

            if (end > 0)
            {
                return command.Substring(0, end);
            }

            return string.Empty;
       }

        private static string GetCompCmd(string command)
        {
            int start = command.IndexOf("=") + 1;
            int end = command.IndexOf(";");

            if (start > 1)
            {
                if (end > 0)
                {
                    return command.Substring(start, end);
                }

                return command.Substring(start);
            } 
            
            return string.Empty;
       }

        private static string GetJumpCmd(string command)
        {
            int start = command.IndexOf(";") + 1;

            if (start > 1)
            {
                return command.Substring(start);
            }

            return string.Empty;
        }
     }


    class Code
    {
        public static long ConvertDestCmd()
        {
            return new NotImplementedException();
        }

        public static long ConvertCompCmd()
        {
            return new NotImplementedException();
        } 

        public static long ConvertJumpCmd()
        {
            return new NotImplementedException();
        }

        public static List<long> ConvertInstructionsToBinary(List<string> instructions)
        {
            var result = new List<long>();

            foreach (var item in instructions)
            {

            }


            return result;
        }
    }


    class SymbolTable
    {
        public SymbolTable()
        {
            Symbols = new Dictionary<string, int>();
        }

        private void InitDefaultSymbols()
        {
            Symbols.Add("SP", 0);
            Symbols.Add("LCL", 1);
            Symbols.Add("ARG", 2);
            Symbols.Add("THIS", 3);
            Symbols.Add("THAT", 4);
            Symbols.Add("RO", 0);
            Symbols.Add("R1", 1);
            Symbols.Add("R2", 2);
            Symbols.Add("R3", 3);
            Symbols.Add("R4", 4);
            Symbols.Add("R5", 5);
            Symbols.Add("R6", 6);
            Symbols.Add("R7", 7);
            Symbols.Add("R8", 8);
            Symbols.Add("R9", 9);
            Symbols.Add("R10", 10);
            Symbols.Add("R11", 11);
            Symbols.Add("R12", 12);
            Symbols.Add("R13", 13);
            Symbols.Add("R14", 14);
            Symbols.Add("R15", 15);
            Symbols.Add("SCREEN", 16384);
            Symbols.Add("KBD", 24576);
        }

        public Dictionary<string, int> Symbols { get; set; }
    }

    class Program
    {
        static int Main(string[] args)
        {
            string inputFile = args[0];
            if (!File.Exists(inputFile))
            {
                return (int)ExitCode.InvalidFilename;
            }

            var instructions = Parser.ParseFile(inputFile);

            return (int)ExitCode.Success;
        }
    }


}
