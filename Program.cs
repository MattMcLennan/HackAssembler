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

                result.Add(line);
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

        private static void UnpackInstruction(string line)
        {
            throw new NotImplementedException();
        }

        private static void InsertIntoField(string line)
        {
            throw new NotImplementedException();
        }
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

            Parser.ParseFile(inputFile);

            return (int)ExitCode.Success;
        }
    }


}
