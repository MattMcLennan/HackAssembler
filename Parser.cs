using System;
using System.Collections.Generic;
using System.IO;

namespace HackAssembler
{
    internal class Parser
    {
        public Parser(SymbolTable symbolTable)
        {
            Symbols = symbolTable;
        }

        public List<string> Parse(string inputFile)
        {
            FirstPass(inputFile);
            return SecondPass(inputFile);
        }

        private void FirstPass(string inputFile)
        {
            int address = 0;

            foreach (var line in File.ReadLines(@inputFile))
            {
                if (IsEmptyLine(line) || IsCommentLine(line))
                {
                    continue;
                }

                if (IsSymbolDeclaration(line))
                {
                    Symbols.AddEntry(GetSymbol(line), address);
                    continue;
                }

                address++;
            }
        }

        private List<string> SecondPass(string inputFile)
        {
            var result = new List<string>();

            foreach (var line in File.ReadLines(@inputFile))
            {
                if (IsEmptyLine(line) || IsCommentLine(line) || IsSymbolDeclaration(line))
                {
                    continue;
                }

                result.Add(ParseCommentOut(line).Trim());
            }

            return result;
        }

        private static bool IsEmptyLine(string line)
        {
            return string.IsNullOrWhiteSpace(line);
        }

        private static bool IsCommentLine(string line)
        {
            return line.StartsWith("//");
        }

        private static bool IsSymbolDeclaration(string line)
        {
            return line.StartsWith('(') && line.EndsWith(')');
        }

        private static bool IsLabel(string input)
        {
            return (Char.IsUpper(input[0]));
        }

        private static bool HasMoreCommands(string line, int currentPosition)
        {
            return currentPosition < line.Length;
        }

        private static bool IsAllDigits(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsDigit(input[i]))
                    return false;
            }

            return true;
        }

        internal static bool IsVariable(string command)
        {
            if (command.StartsWith("@"))
            {
                if (!IsAllDigits(command.Substring(1)))
                {
                    return true;
                }
            }

            return false;
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

        public static CommandType CommandTypeIs(string command)
        {
            if (command.StartsWith("@"))
            {
                if (IsLabel(command.Substring(1)))
                {
                    return CommandType.L;
                }

                return CommandType.A;
            }

            return CommandType.C;
        }

        private static string GetSymbol(string symbol)
        {
            return symbol.Split('(', ')')[1];
        }

        public static string GetDestCmd(string command)
        {
            int end = command.IndexOf("=");

            if (end > 0)
            {
                return command.Substring(0, end);
            }

            return string.Empty;
        }

        public static string GetCompCmd(string command)
        {
            int start = command.IndexOf("=") + 1;
            int end = command.IndexOf(";");

            if (end > 0)
            {
                return command.Substring(start, end);
            }

            return command.Substring(start);
        }

        public static string GetJumpCmd(string command)
        {
            int start = command.IndexOf(";") + 1;

            if (start > 1)
            {
                return command.Substring(start);
            }

            return string.Empty;
        }

        private SymbolTable Symbols { get; set; }
    }
}