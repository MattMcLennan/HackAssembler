using System;
using System.Collections;
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

    enum CommandType
    {
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

        public static CommandType CommandTypeIs(string command)
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

        public static string GetJumpCmd(string command)
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
        private static long ConvertDestCmd(string cmd)
        {
            var destCmd = Parser.GetDestCmd(cmd);

            long value;
            if (!DestCmdMapping.TryGetValue(destCmd, out value))
            {
                throw new Exception("Key not found in dest cmd mapping dictionary");
            }

            return value;
        }

        private static Dictionary<string, BitArray> _destCmdMapping;

        private static Dictionary<string, BitArray> DestCmdMapping
        {
            get
            {
                if (_destCmdMapping == null)
                {
                    _destCmdMapping = new Dictionary<string, BitArray>
                    {
                        { "", new BitArray(new int[] { 0, 0, 0})  }
                        , { "M", new BitArray(new int[] { 0, 0, 1 }) }
                        , { "D", new BitArray(new int[] { 0, 1, 0 }) }
                        , { "MD", new BitArray(new int[] { 0, 1, 1 }) }
                        , { "A", new BitArray(new int[] { 1, 0, 0 }) }
                        , { "AM", new BitArray(new int[] { 1, 0, 1 }) }
                        , { "AD", new BitArray(new int[] { 1, 1, 0 }) }
                        , { "AMD", new BitArray(new int[] { 1, 1, 1 }) }
                    };
                }

                return _destCmdMapping;
            }
        }

        private static Dictionary<string, BitArray> _jumpCmdMapping;

        private static Dictionary<string, BitArray> JumpCmdMapping
        {
            get
            {
                if (_jumpCmdMapping == null)
                {
                    _jumpCmdMapping = new Dictionary<string, BitArray>
                    {
                        { "", new BitArray(new int[] { 0, 0, 0})  }
                        , { "JGT", new BitArray(new int[] { 0, 0, 1 }) }
                        , { "JEQ", new BitArray(new int[] { 0, 1, 0 }) }
                        , { "JGE", new BitArray(new int[] { 0, 1, 1 }) }
                        , { "JLT", new BitArray(new int[] { 1, 0, 0 }) }
                        , { "JNE", new BitArray(new int[] { 1, 0, 1 }) }
                        , { "JLE", new BitArray(new int[] { 1, 1, 0 }) }
                        , { "JMP", new BitArray(new int[] { 1, 1, 1 }) }
                    };
                }

                return _jumpCmdMapping;
            }
        }

        private static Dictionary<string, BitArray> _compCmdMapping;

        private static Dictionary<string, BitArray> CompCmdMapping
        {
            get
            {
                if (_compCmdMapping == null)
                {
                    _compCmdMapping = new Dictionary<string, BitArray>
                    {
                        { "0", new BitArray(new int[] { 0, 1, 0, 1, 0, 1, 0 }) }
                        , { "1", new BitArray(new int[] { 0, 1, 1, 1, 1, 1, 1 }) }
                        , { "-1", new BitArray(new int[] { 0, 1, 1, 1, 0, 1, 0 }) }
                        , { "D", new BitArray(new int[] { 0, 0, 0, 1, 1, 0, 0 }) }
                        , { "A", new BitArray(new int[] { 0, 1, 1, 0, 0, 0, 0 }) }
                        , { "M", new BitArray(new int[] { 1, 1, 1, 0, 0, 0, 0 }) }
                        , { "!D", new BitArray(new int[] { 0, 0, 0, 1, 1, 0, 1 }) }
                        , { "!A", new BitArray(new int[] { 0, 1, 1, 0, 0, 0, 1 }) }
                        , { "!M", new BitArray(new int[] { 1, 1, 1, 0, 0, 0, 1 }) }
                        , { "-D", new BitArray(new int[] { 0, 0, 0, 1, 1, 1, 1 }) }
                        , { "-A", new BitArray(new int[] { 0, 1, 1, 0, 0, 1, 1 }) }
                        , { "-M", new BitArray(new int[] { 1, 1, 1, 0, 0, 1, 1 }) }
                        , { "D+1", new BitArray(new int[] { 0, 0, 1, 1, 1, 1, 1 }) }
                        , { "A+1", new BitArray(new int[] { 0, 1, 1, 0, 1, 1, 1 }) }
                        , { "M+1", new BitArray(new int[] { 1, 1, 1, 0, 1, 1, 1 }) }
                        , { "D-1", new BitArray(new int[] { 0, 0, 0, 1, 1, 1, 0 }) }
                        , { "A-1", new BitArray(new int[] { 0, 1, 1, 0, 0, 1, 0 }) }
                        , { "M-1", new BitArray(new int[] { 1, 1, 1, 0, 0, 1, 0 }) }
                        , { "D+A", new BitArray(new int[] { 0, 0, 0, 0, 0, 1, 0 }) }
                        , { "D+M", new BitArray(new int[] { 1, 0, 0, 0, 0, 1, 0 }) }
                        , { "D-A", new BitArray(new int[] { 0, 0, 1, 0, 0, 1, 1 }) }
                        , { "D-M", new BitArray(new int[] { 1, 0, 1, 0, 0, 1, 1 }) }
                        , { "A-D", new BitArray(new int[] { 0, 0, 0, 0, 1, 1, 1 }) }
                        , { "M-D", new BitArray(new int[] { 1, 0, 0, 0, 1, 1, 1 }) }
                        , { "D&A", new BitArray(new int[] { 0, 0, 0, 0, 0, 0, 0 }) }
                        , { "D&M", new BitArray(new int[] { 1, 0, 0, 0, 0, 0, 0 }) }
                        , { "D|A", new BitArray(new int[] { 0, 0, 1, 0, 1, 0, 1 }) }
                        , { "D|M", new BitArray(new int[] { 1, 0, 1, 0, 1, 0, 1 }) }
                    };
                }

                return _compCmdMapping;
            }
        }

        private static long ConvertCompCmd(string cmd)
        {
            var compCmd = Parser.GetCompCmd(cmd);

            long value;
            if (!CompCmdMapping.TryGetValue(compCmd, out value))
            {
                throw new Exception("Key not found in jump cmd mapping dictionary");
            }

            return value;
        }

        private static BitArray ConvertJumpCmd(string cmd)
        {
            var jumpCmd = Parser.GetJumpCmd(cmd);

            long value;
            if (!JumpCmdMapping.TryGetValue(jumpCmd, out value))
            {
                throw new Exception("Key not found in jump cmd mapping dictionary");
            }

            return value;
        }

        private static byte[] ConvertToBinary(Int16 value)
        {
            var result = new byte[2];

            result[0] = (byte)value;
            result[1] = (byte)(value >> 8);

            return result;
        }

        public static List<long> ConvertInstructionsToBinary(List<string> instructions)
        {
            var result = new List<long>();

            foreach (var item in instructions)
            {
                CommandType cmd = Parser.CommandTypeIs(item);
                switch (cmd)
                {
                    case CommandType.A:
                        {
                            result.Add(ConvertACmd(item));
                            break;
                        }

                    case CommandType.L:
                        {
                            result.Add(ConvertLCmd(item));
                            break;
                        }

                    default:
                        {
                            throw new Exception("Command Type not found/not yet implemented");
                        }
                }

            }

            return result;
        }

        private static long ConvertLCmd(string item)
        {
            var lCmd = ConvertDestCmd(item) + ConvertCompCmd(item) + ConvertJumpCmd(item);
            return Convert.ToInt16(lCmd);
        }

        private static long ConvertACmd(string item)
        {
            var address = item.Substring(1);
            return Convert.ToInt16(address);
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
