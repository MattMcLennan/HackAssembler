using System;
using System.Collections;
using System.Collections.Generic;

namespace HackAssembler
{
    internal class Translator
    {
        public Translator(SymbolTable symbolTable)
        {
            Symbols = symbolTable;
        }

        public List<byte[]> ConvertInstructionsToBinary(List<string> instructions)
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

                    case CommandType.L:
                        {
                            Int32 cCmd = ConvertLCmd(item);
                            result.Add(ConvertToBytes(cCmd));
                            break;
                        }

                    default:
                        {
                            throw new Exception("Command Type not found");
                        }
                }

            }

            return result;
        }

        private Int16 ConvertACmd(string item)
        {
            if (Parser.IsVariable(item))
            {
                if (!Symbols.Contains(item.Substring(1)))
                {
                    Symbols.AddEntry(item.Substring(1), variableBaseAddress);
                    variableBaseAddress++;
                }

                return Convert.ToInt16(Symbols.GetAddress(item.Substring(1)));
            }

            string address = item.Substring(1);
            return Convert.ToInt16(address);
        }

        private Int32 ConvertCCmd(string item)
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

            Array.Reverse(cCmd);

            return GetIntFromBitArray(new BitArray(cCmd));
        }

        private Int16 ConvertLCmd(string item)
        {
            return Convert.ToInt16(Symbols.GetAddress(item.Substring(1)));
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


        private static Int32 GetIntFromBitArray(BitArray bitArray)
        {
            if (bitArray.Length > 16)
                throw new ArgumentException("Bit Array is too long");

            Int32[] array = new Int32[1];
            bitArray.CopyTo(array, 0);

            return array[0];
        }

        private int variableBaseAddress = 16;

        private SymbolTable Symbols { get; set; }

        private static Dictionary<string, bool[]> _compCmdMapping;

        private static Dictionary<string, bool[]> CompCmdMapping
        {
            get
            {
                if (_compCmdMapping == null)
                {
                    _compCmdMapping = new Dictionary<string, bool[]>
                    { { "0", new bool[] { false, true, false, true, false, true, false } },
                    { "1", new bool[] { false, true, true, true, true, true, true } },
                    { "-1", new bool[] { false, true, true, true, false, true, false } },
                    { "D", new bool[] { false, false, false, true, true, false, false } },
                    { "A", new bool[] { false, true, true, false, false, false, false } },
                    { "M", new bool[] { true, true, true, false, false, false, false } },
                    { "!D", new bool[] { false, false, false, true, true, false, true } },
                    { "!A", new bool[] { false, true, true, false, false, false, true } },
                    { "!M", new bool[] { true, true, true, false, false, false, true } },
                    { "-D", new bool[] { false, false, false, true, true, true, true } },
                    { "-A", new bool[] { false, true, true, false, false, true, true } },
                    { "-M", new bool[] { true, true, true, false, false, true, true } },
                    { "D+1", new bool[] { false, false, true, true, true, true, true } },
                    { "A+1", new bool[] { false, true, true, false, true, true, true } },
                    { "M+1", new bool[] { true, true, true, false, true, true, true } },
                    { "D-1", new bool[] { false, false, false, true, true, true, false } },
                    { "A-1", new bool[] { false, true, true, false, false, true, false } },
                    { "M-1", new bool[] { true, true, true, false, false, true, false } },
                    { "D+A", new bool[] { false, false, false, false, false, true, false } },
                    { "D+M", new bool[] { true, false, false, false, false, true, false } },
                    { "D-A", new bool[] { false, false, true, false, false, true, true } },
                    { "D-M", new bool[] { true, false, true, false, false, true, true } },
                    { "A-D", new bool[] { false, false, false, false, true, true, true } },
                    { "M-D", new bool[] { true, false, false, false, true, true, true } },
                    { "D&A", new bool[] { false, false, false, false, false, false, false } },
                    { "D&M", new bool[] { true, false, false, false, false, false, false } },
                    { "D|A", new bool[] { false, false, true, false, true, false, true } },
                    { "D|M", new bool[] { true, false, true, false, true, false, true } }
                    };
                }

                return _compCmdMapping;
            }
        }

        private static Dictionary<string, bool[]> _destCmdMapping;

        private static Dictionary<string, bool[]> DestCmdMapping
        {
            get
            {
                if (_destCmdMapping == null)
                {
                    _destCmdMapping = new Dictionary<string, bool[]>
                        { { "", new bool[] { false, false, false } },
                        { "M", new bool[] { false, false, true } },
                        { "D", new bool[] { false, true, false } },
                        { "MD", new bool[] { false, true, true } },
                        { "A", new bool[] { true, false, false } },
                        { "AM", new bool[] { true, false, true } },
                        { "AD", new bool[] { true, true, false } },
                        { "AMD", new bool[] { true, true, true } }
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
                    { { "", new bool[] { false, false, false } },
                    { "JGT", new bool[] { false, false, true } },
                    { "JEQ", new bool[] { false, true, false } },
                    { "JGE", new bool[] { false, true, true } },
                    { "JLT", new bool[] { true, false, false } },
                    { "JNE", new bool[] { true, false, true } },
                    { "JLE", new bool[] { true, true, false } },
                    { "JMP", new bool[] { true, true, true } }
                    };
                }

                return _jumpCmdMapping;
            }
        }
    }
}