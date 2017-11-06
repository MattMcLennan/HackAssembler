using System;
using System.Collections.Generic;

namespace HackAssembler
{
    internal class SymbolTable
    {
        public SymbolTable()
        {
            Symbols = new Dictionary<string, int>();
            InitDefaultSymbols();
        }

        private void InitDefaultSymbols()
        {
            Symbols.Add("SP", 0);
            Symbols.Add("LCL", 1);
            Symbols.Add("ARG", 2);
            Symbols.Add("THIS", 3);
            Symbols.Add("THAT", 4);
            Symbols.Add("R0", 0);
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

        public void AddEntry(string symbol, int address)
        {
            Symbols[symbol] = address;
        }

        public bool Contains(string symbol)
        {
            return Symbols.ContainsKey(symbol);
        }

        public int GetAddress(string symbol)
        {
            int address;
            if (!Symbols.TryGetValue(symbol, out address))
            {
                throw new Exception("Can't find symbol in symbol dictionary");
            }

            return address;
        }

        public Dictionary<string, int> Symbols { get; set; }
    }
}