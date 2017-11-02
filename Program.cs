using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HackAssembler
{

    internal enum ExitCode : int
    {
        Success = 0,
        InvalidFilename = 1,
        UnknownError = 10
    }

    internal enum CommandType
    {
        A,
        C,
        L
    }

    internal class Parser
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

    internal class Code
    {
        private static bool[] ConvertDestCmd(string cmd)
        {
            string destCmd = Parser.GetDestCmd(cmd);

            bool[] value;
            if (!DestCmdMapping.TryGetValue(destCmd, out value))
            {
                throw new Exception("Key not found in dest cmd mapping dictionary");
            }

            return value;
        }

        private static Dictionary<string, bool[]> _destCmdMapping;

        private static Dictionary<string, bool[]> DestCmdMapping
        {
            get
            {
                if (_destCmdMapping == null)
                {
                    _destCmdMapping = new Dictionary<string, bool[]>
                    {
                        { "", new bool[] { false, false, false}  }
                        , { "M", new bool[] { false, false, true } }
                        , { "D", new bool[] { false, true, false } }
                        , { "MD", new bool[] { false, true, true } }
                        , { "A", new bool[] { true, false, false } }
                        , { "AM", new bool[] { true, false, true } }
                        , { "AD", new bool[] { true, true, false } }
                        , { "AMD", new bool[] { true, true, true } }
                    };
                }

                return _destCmdMapping;
            }
        }

        private static Dictionary<string, bool[]> _jumpCmdMapping;

        private static Dictionary<string, bool[]> JumpCmdMapping
        {
            get
            {
                if (_jumpCmdMapping == null)
                {
                    _jumpCmdMapping = new Dictionary<string, bool[]>
                    {
                        { "", new bool[] { false, false, false}  }
                        , { "JGT", new bool[] { false, false, true } }
                        , { "JEQ", new bool[] { false, true, false } }
                        , { "JGE", new bool[] { false, true, true } }
                        , { "JLT", new bool[] { true, false, false } }
                        , { "JNE", new bool[] { true, false, true } }
                        , { "JLE", new bool[] { true, true, false } }
                        , { "JMP", new bool[] { true, true, true } }
                    };
                }

                return _jumpCmdMapping;
            }
        }

        private static Dictionary<string, bool[]> _compCmdMapping;

        private static Dictionary<string, bool[]> CompCmdMapping
        {
            get
            {
                if (_compCmdMapping == null)
                {
                    _compCmdMapping = new Dictionary<string, bool[]>
                    {
                        { "0", new bool[]{ false, true, false, true, false, true, false } }
                        , { "1", new bool[] { false, true, true, true, true, true, true } }
                        , { "-1", new bool[] { false, true, true, true, false, true, false } }
                        , { "D", new bool[] { false, false, false, true, true, false, false } }
                        , { "A", new bool[] { false, true, true, false, false, false, false }}
                        , { "M", new bool[] { true, true, true, false, false, false, false }}
                        , { "!D", new bool[] { false, false, false, true, true, false, true }}
                        , { "!A", new bool[] { false, true, true, false, false, false, true }}
                        , { "!M", new bool[] { true, true, true, false, false, false, true }}
                        , { "-D", new bool[] { false, false, false, true, true, true, true }}
                        , { "-A", new bool[] { false, true, true, false, false, true, true }}
                        , { "-M", new bool[] { true, true, true, false, false, true, true }}
                        , { "D+1", new bool[] { false, false, true, true, true, true, true }}
                        , { "A+1", new bool[] { false, true, true, false, true, true, true }}
                        , { "M+1", new bool[] { true, true, true, false, true, true, true }}
                        , { "D-1", new bool[] { false, false, false, true, true, true, false }}
                        , { "A-1", new bool[] { false, true, true, false, false, true, false }}
                        , { "M-1", new bool[] { true, true, true, false, false, true, false }}
                        , { "D+A", new bool[] { false, false, false, false, false, true, false }}
                        , { "D+M", new bool[] { true, false, false, false, false, true, false }}
                        , { "D-A", new bool[] { false, false, true, false, false, true, true }}
                        , { "D-M", new bool[] { true, false, true, false, false, true, true }}
                        , { "A-D", new bool[] { false, false, false, false, true, true, true }}
                        , { "M-D", new bool[] { true, false, false, false, true, true, true }}
                        , { "D&A", new bool[] { false, false, false, false, false, false, false }}
                        , { "D&M", new bool[] { true, false, false, false, false, false, false }}
                        , { "D|A", new bool[] { false, false, true, false, true, false, true }}
                        , { "D|M", new bool[] { true, false, true, false, true, false, true }}
                    };
                }

                return _compCmdMapping;
            }
        }

        private static bool[] ConvertCompCmd(string cmd)
        {
            string compCmd = Parser.GetCompCmd(cmd);

            bool[] value;
            if (!CompCmdMapping.TryGetValue(compCmd, out value))
            {
                throw new Exception("Key not found in jump cmd mapping dictionary");
            }

            return value;
        }

        private static bool[] ConvertJumpCmd(string cmd)
        {
            string jumpCmd = Parser.GetJumpCmd(cmd);

            bool[] value;
            if (!JumpCmdMapping.TryGetValue(jumpCmd, out value))
            {
                throw new Exception("Key not found in jump cmd mapping dictionary");
            }

            return value;
        }

        private static byte[] ConvertToBytes(Int32 value)
        {
            var result = new byte[2];

            result[0] = (byte)value;
            result[1] = (byte)(value >> 8);

            return result;
        }

        public static List<byte[]> ConvertInstructionsToBinary(List<string> instructions)
        {
            var result = new List<byte[]>();

            foreach (var item in instructions)
            {
                CommandType cmd = Parser.CommandTypeIs(item);
                switch (cmd)
                {
                    case CommandType.A:
                        {
                            Int32 aCmd = ConvertACmd(item);
                            result.Add(ConvertToBytes(aCmd));
                            break;
                        }

                    case CommandType.C:
                        {
                            Int32 cCmd = ConvertCCmd(item);
                            result.Add(ConvertToBytes(cCmd));
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

        private static Int32 ConvertCCmd(string item)
        {
            bool[] binaryCompCmd = ConvertCompCmd(item);
            bool[] binaryDestCmd = ConvertDestCmd(item);
            bool[] binaryJumpCmd = ConvertJumpCmd(item);

            bool[] cCmd = new bool[]
            {
                true,
                true,
                true,
                binaryCompCmd[0],
                binaryCompCmd[1],
                binaryCompCmd[2],
                binaryCompCmd[3],
                binaryCompCmd[4],
                binaryCompCmd[5],
                binaryCompCmd[6],
                binaryDestCmd[0],
                binaryDestCmd[1],
                binaryDestCmd[2],
                binaryJumpCmd[0],
                binaryJumpCmd[1],
                binaryJumpCmd[2]
            };

            return GetIntFromBitArray(new BitArray(cCmd));
        }

        private static Int32 ConvertACmd(string item)
        {
            var address = item.Substring(1);
            return Convert.ToInt16(address);
        }

        private static Int32 GetIntFromBitArray(BitArray bitArray)
        {
            if (bitArray.Length > 16)
                throw new ArgumentException("Bit Array is too long");

            Int32[] array = new Int32[1];
            bitArray.CopyTo(array, 0);

            return array[0];
        }
    }

    internal class SymbolTable
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

            List<string> instructions = Parser.ParseFile(inputFile);
            List<byte[]> byteInstructions = Code.ConvertInstructionsToBinary(instructions);

            const string fileName = "../ComputerArchitecture/nand2tetris/projects/06/add/Add8.hack";
            byte[] newLine = System.Text.ASCIIEncoding.ASCII.GetBytes(Environment.NewLine);

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (var bytes in byteInstructions)
                {
                    string bitsOutput = Convert.ToString(bytes[0] + bytes[1], 2).PadLeft(16, '0');
                    sw.WriteLine(bitsOutput);
                }
            }

            return (int)ExitCode.Success;
        }
    }


}
