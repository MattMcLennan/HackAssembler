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

    class Program
    {
        static int Main(string[] args)
        {
            foreach (string inputFile in args)
            {
                if (!File.Exists(inputFile))
                {
                    return (int)ExitCode.InvalidFilename;
                }
            }

            foreach (string inputFile in args)
            {
                SymbolTable symbols = new SymbolTable();
                Parser parseDoc = new Parser(symbols);
                Translator translator = new Translator(symbols);

                List<string> instructions = parseDoc.Parse(inputFile);
                List<byte[]> byteInstructions = translator.ConvertInstructionsToBinary(instructions);
                OutputBinaryFile(GetFileName(inputFile), byteInstructions);
            }

            return (int)ExitCode.Success;
        }

        private static string GetFileName(string inputFile)
        {
            var originalFileName = inputFile.Substring(inputFile.LastIndexOf("/") + 1);
            return originalFileName.Replace("asm", "hack");
        }

        private static void OutputBinaryFile(string fileName, List<byte[]> byteInstructions)
        {
            byte[] newLine = System.Text.ASCIIEncoding.ASCII.GetBytes(Environment.NewLine);

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (var bytes in byteInstructions)
                {
                    string firstByte = Convert.ToString(bytes[0], 2).PadLeft(8, '0');
                    string secondByte = Convert.ToString(bytes[1], 2).PadLeft(8, '0');
                    sw.Write(String.Format("{0}{1}{2}", secondByte, firstByte, Environment.NewLine));
                }
            }
        }


        
    }

}