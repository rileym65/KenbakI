using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KenbakI
{
    public class Assembler
    {
        //            imp    #      zp     zp,x   zp,y   abs    abs,x  abs,y  (zp,x) (zp),y (zp)   (abs)  Acc    Rel    (abs,x)
        protected const int AM_IMMEDIATE = 3;
        protected const int AM_MEMORY = 4;
        protected const int AM_INDIRECT = 5;
        protected const int AM_INDEXED = 6;
        protected const int AM_INDIRECTINDEXED = 7;

        protected List<List<object>> labels;
        protected List<String> tokens;
        protected int pass;
        protected int address;
        public String results;
        public int errors;
        public Boolean showSymbolTable;
        public Boolean crossReference;
        public Boolean showMemoryMap;
        protected int lineNumber;
        protected String outputLine;
        protected Boolean labelFound;
        protected String lastLabel;
        protected Boolean dsectMode;
        protected int linesAssembled;
        protected int bytesAssembled;
        protected int startAddress;
        protected byte[] memory;
        protected char[] memoryMap;
        protected int minAddress;
        protected int maxAddress;
        protected byte opcodeAddress;

        public Assembler()
        {
            init();
        }
         
        protected void init()
        {
        }

        protected void error(String msg)
        {
            if (pass == 2)
            {
                errors++;
                results += msg + "\r\n";
            }

        }

        protected void sortLabels()
        {
            List<object> temp;
            Boolean flag;
            flag = true;
            while (flag)
            {
                flag = false;
                for (var i = 0; i < labels.Count - 1; i++)
                {
                    if (((String)labels[i][0]).CompareTo(((String)labels[i + 1][0])) > 0)
                    {
                        temp = labels[i];
                        labels[i] = labels[i + 1];
                        labels[i + 1] = temp;
                        flag = true;
                    }
                }
            }
        }

        protected Boolean addLabel(String label, int value)
        {
            List<object> newEntry;
            lastLabel = label;
            foreach (var entry in labels)
            {
                if (label.ToUpper().Equals(((String)entry[0]).ToUpper()))
                {
                    entry[1] = value;
                    return true;
                }
            }
            newEntry = new List<object>();
            newEntry.Add(label);
            newEntry.Add(value);
            newEntry.Add(lineNumber);
            labels.Add(newEntry);
            return false;
        }

        protected int findLabel(String label)
        {
            labelFound = false;
            foreach (var entry in labels)
            {
                if (label.ToUpper().Equals(((String)entry[0]).ToUpper()))
                {
                    labelFound = true;
                    if (pass == 2) entry.Add(lineNumber);
                    return (int)entry[1];
                }
            }
            return -1;
        }

        protected void tokenize(String input, String separators)
        {
            String current;
            tokens = new List<String>();
            if (input.Length < 1) return;
            if (input[0] == 32 || input[0] == 9)
            {
                tokens.Add("");
            }
            input = input.Trim();
            current = "";
            while (input.Length > 0)
            {
                if (separators.IndexOf(input[0]) >= 0)
                {
                    if (current.Length > 0)
                    {
                        tokens.Add(current);
                        current = "";
                    }
                    if (input[0] == ';')
                    {
                        input = " ";
                    }
                    else if (input[0] != ' ' && input[0] != '\t') tokens.Add(input[0].ToString());
                }
                else
                {
                    current = current + input[0].ToString();
                }
                input = input.Substring(1);
            }
            if (current.Length > 0) tokens.Add(current);
        }

        protected Boolean isHex(String value)
        {
            foreach (var chr in value)
            {
                if (chr < '0') return false;
                if (chr > 'f') return false;
                if (chr > '9' && chr < 'A') return false;
                if (chr > 'F' && chr < 'a') return false;
            }
            return true;
        }

        protected Boolean isDecimal(String value)
        {
            foreach (var chr in value)
            {
                if (chr < '0' || chr > '9') return false;
            }
            return true;
        }

        protected Boolean isOctal(String value)
        {
            foreach (var chr in value)
            {
                if (chr < '0' || chr > '7') return false;
            }
            return true;
        }

        protected Boolean isBinary(String value)
        {
            foreach (var chr in value)
            {
                if (chr < '0' || chr > '1') return false;
            }
            return true;
        }

        protected String hexToDecimal(String input)
        {
            int value;
            value = 0;
            while (input.Length > 0)
            {
                if (input[0] >= '0' && input[0] <= '9') value = (value << 4) + input[0] - '0';
                if (input[0] >= 'A' && input[0] <= 'F') value = (value << 4) + input[0] - 'A' + 10;
                if (input[0] >= 'a' && input[0] <= 'f') value = (value << 4) + input[0] - 'a' + 10;
                input = input.Substring(1);
            }
            return value.ToString();
        }

        protected String octalToDecimal(String input)
        {
            int value;
            value = 0;
            while (input.Length > 0)
            {
                if (input[0] >= '0' && input[0] <= '7') value = (value << 3) + input[0] - '0';
                input = input.Substring(1);
            }
            return value.ToString();
        }

        protected byte hexToInt(String input)
        {
            int value;
            value = 0;
            while (input.Length > 0)
            {
                if (input[0] >= '0' && input[0] <= '9') value = (value << 4) + input[0] - '0';
                if (input[0] >= 'A' && input[0] <= 'F') value = (value << 4) + input[0] - 'A' + 10;
                if (input[0] >= 'a' && input[0] <= 'f') value = (value << 4) + input[0] - 'a' + 10;
                input = input.Substring(1);
            }
            return (byte)value;
        }

        protected int decimalToInt(String input)
        {
            int value;
            value = 0;
            while (input.Length > 0)
            {
                if (input[0] >= '0' && input[0] <= '9') value = (value * 10) + (input[0] - '0');
                input = input.Substring(1);
            }
            return value;
        }

        protected int octalToInt(String input)
        {
            int value;
            value = 0;
            while (input.Length > 0)
            {
                if (input[0] >= '0' && input[0] <= '7') value = (value << 3) + (input[0] - '0');
                input = input.Substring(1);
            }
            return value;
        }

        protected String binaryToDecimal(String input)
        {
            int value;
            value = 0;
            while (input.Length > 0)
            {
                if (input[0] == '0') value = (value << 1) + 0;
                if (input[0] == '1') value = (value << 1) + 1;
                input = input.Substring(1);
            }
            return value.ToString();
        }

        protected Boolean isLabel(String input)
        {
            if (input.Length < 1) return false;
            if (input.ToUpper().Equals("A")) return false;
            if (input.ToUpper().Equals("B")) return false;
            if (input.ToUpper().Equals("X")) return false;
            foreach (var chr in input)
            {
                if (chr >= 'a' && chr <= 'z') return true;
                if (chr >= 'A' && chr <= 'Z') return true;
                if (chr == '_') return true;
            }
            return false;
        }


        protected void writeByte(byte value)
        {
            outputLine += value.ToString("x2") + " ";
            if (pass == 2)
            {
                memory[address] = value;
                bytesAssembled++;
            }
            address++;
        }

        protected byte convertValue(String value)
        {
            int v;
            if (isDecimal(value)) return (byte)decimalToInt(value);
            if (isLabel(value))
            {
                v = findLabel(value);
                if (v < 0)
                {
                    if (pass == 2) error("Label not found");
                    return 0xff;
                }
                return (byte)(v & 0xff);
            }
            return 0xff;
        }

        protected byte getAddressingMode(int pos)
        {
            if (tokens.Count <= pos) return 0xff;
            if (tokens[pos].ToUpper().Equals("("))
            {
                pos++;
                if (tokens.Count <= pos) return 0xff;
                opcodeAddress = convertValue(tokens[pos]);
                pos++;
                if (tokens.Count <= pos) return 0xff;
                if (!tokens[pos].Equals(")")) return 0xff;
                pos++;
                if (tokens.Count <= pos)
                {
                    return AM_INDIRECT;
                }
                if (tokens.Count <= pos) return 0xff;
                if (!tokens[pos].Equals(",")) return 0xff;
                pos++;
                if (tokens.Count <= pos) return 0xff;
                if (!tokens[pos].ToUpper().Equals("X")) return 0xff;
                return AM_INDIRECTINDEXED;
            }
            if (tokens[pos].ToUpper().Equals("C"))
            {
                pos++;
                if (tokens.Count <= pos) return 0xff;
                if (tokens[pos].ToUpper().Equals("="))
                {
                    pos++;
                    if (tokens.Count <= pos) return 0xff;
                    opcodeAddress = convertValue(tokens[pos]);
                    return AM_IMMEDIATE;
                }
                return 0xff;
            }
            opcodeAddress = convertValue(tokens[pos]);
            pos++;
            if (tokens.Count <= pos)
            {
                return AM_MEMORY;
            }
            if (!tokens[pos].Equals(",")) return 0xff;
            pos++;
            if (!tokens[pos].ToUpper().Equals("X")) return 0xff;
            return AM_INDEXED;
        }

        protected void reduce()
        {
            int pos;
            pos = 0;
            while (pos < tokens.Count)
            {
                if (tokens[pos].Equals("$"))
                {
                    if (isHex(tokens[pos+1]))
                    {
                        tokens[pos+1] = hexToDecimal(tokens[pos+1]);
                        tokens.RemoveAt(pos);
                    }
                    else
                    {
                        if (pass == 2) error("Invalid hex constant");
                    }
                }
                else if (tokens[pos].Equals("%"))
                {
                    if (isBinary(tokens[pos + 1]))
                    {
                        tokens[pos + 1] = binaryToDecimal(tokens[pos + 1]);
                        tokens.RemoveAt(pos);
                    }
                    else
                    {
                        if (pass == 2) error("Invalid hex constant");
                    }
                }
                else if (tokens[pos].Equals("!"))
                {
                    if (isOctal(tokens[pos + 1]))
                    {
                        tokens[pos + 1] = octalToDecimal(tokens[pos + 1]);
                        tokens.RemoveAt(pos);
                    }
                    else
                    {
                        if (pass == 2) error("Invalid hex constant");
                    }
                }
                else pos++;
            }
        }

        protected void assembleType1(byte opcode)
        {
            byte mode;
            int pos;
            pos = 2;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Incomplete line");
                return;
            }
            switch (tokens[2].ToUpper())
            {
                case "A": opcode += 0x00; break;
                case "B": opcode += 0x40; break;
                case "X": opcode += 0x80; break;
                default:
                    if (pass == 2) error("Missing register");
                    return;
            }
            mode = getAddressingMode(pos+1);
            if (mode == 0xff)
            {
                if (pass == 2) error("Invalid addressing mode");
                return;
            }
            opcode += mode;
            writeByte(opcode);
            writeByte(opcodeAddress);
        }

        protected void assembleType2(byte opcode)
        {
            byte mode;
            int pos;
            pos = 2;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Incomplete line");
                return;
            }
            mode = getAddressingMode(pos);
            if (mode == 0xff)
            {
                if (pass == 2) error("Invalid addressing mode");
                return;
            }
            opcode += mode;
            writeByte(opcode);
            writeByte(opcodeAddress);
        }

        protected void assembleShifts(byte opcode)
        {
            byte mode;
            int pos;
            pos = 2;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Incomplete line");
                return;
            }
            switch (tokens[2].ToUpper())
            {
                case "A": opcode += 0x00; break;
                case "B": opcode += 0x20; break;
                default:
                    if (pass == 2) error("Missing register");
                    return;
            }
            pos++;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Missing shift count");
                return;
            }
            mode = convertValue(tokens[pos]);
            if (mode < 1 || mode > 4)
            {
                if (pass == 2) error("Invalid shift count");
                return;
            }
            if (mode == 4) mode = 0;
            opcode |= (byte)(mode << 3);
            writeByte(opcode);
        }

        protected void assembleSet(byte opcode)
        {
            int pos;
            byte mode;
            byte addr;
            pos = 2;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Incomplete line");
                return;
            }
            mode = convertValue(tokens[pos]);
            if (mode < 0 || mode > 7)
            {
                if (pass == 2) error("Invalid bit position");
                return;
            }
            mode <<= 3;
            opcode |= mode;
            pos++;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Incomplete line");
                return;
            }
            if (!tokens[pos].Equals(","))
            {
                if (pass == 2) error("Syntax error");
                return;
            }
            pos++;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Incomplete line");
                return;
            }
            addr = convertValue(tokens[pos]);
            writeByte(opcode);
            writeByte(addr);
        }

        protected void assembleJumps(byte opcode)
        {
            int pos;
            byte reg;
            byte mode;
            byte addr;
            pos = 2;
            if (tokens.Count <= pos)
            {
                if (pass == 2) error("Incomplete line");
                return;
            }
            reg = 0xc0;
            mode = 0x03;
            if (tokens[pos].ToUpper().Equals("A")) reg = 0x00;
            else if (tokens[pos].ToUpper().Equals("B")) reg = 0x40;
            else if (tokens[pos].ToUpper().Equals("X")) reg = 0x80;
            opcode |= reg;
            if (reg != 0xc0)
            {
                pos++;
                if (tokens.Count <= pos)
                {
                    if (pass == 2) error("Incomplete line");
                    return;
                }
                if (tokens[pos].ToUpper().Equals("NZ")) mode = 0x03;
                else if (tokens[pos].ToUpper().Equals("Z")) mode = 0x04;
                else if (tokens[pos].ToUpper().Equals("M")) mode = 0x05;
                else if (tokens[pos].ToUpper().Equals("P")) mode = 0x06;
                else if (tokens[pos].ToUpper().Equals("PNZ")) mode = 0x07;
                pos++;
                if (tokens.Count <= pos)
                {
                    if (pass == 2) error("Incomplete line");
                    return;
                }
                if (!tokens[pos].Equals(","))
                {
                    if (pass == 2) error("Syntax error");
                    return;
                }
                pos++;
                if (tokens.Count <= pos)
                {
                    if (pass == 2) error("Incomplete line");
                    return;
                }
            }
            if ((opcode & 0x08) != 0)
            {
                if (!tokens[pos].Equals("("))
                {
                    if (pass == 2) error("Indirect jump requires indirect address");
                    return;
                }
                pos++;
                if (tokens.Count <= pos)
                {
                    if (pass == 2) error("Syntax error");
                    return;
                }
                addr = convertValue(tokens[pos]);
                pos++;
                if (!tokens[pos].Equals(")"))
                {
                    if (pass == 2) error("Syntax error");
                    return;
                }
            }
            else
            {
                if (tokens[pos].Equals("("))
                {
                    if (pass == 2) error("Direct jump requires a direct address");
                    return;
                }
                addr = convertValue(tokens[pos]);
            }
            opcode |= mode;
            writeByte(opcode);
            writeByte(addr);
        }

        protected void assembleData()
        {
            if (tokens.Count <= 2)
            {
                if (pass == 2) error("Invalid format for DATA");
                return;
            }
            writeByte(convertValue(tokens[2]));
        }

        protected void assembleOrg()
        {
            if (tokens.Count <= 2)
            {
                if (pass == 2) error("Invalid format for ORG");
                return;
            }
            address = (convertValue(tokens[2]) & 0xff);
        }

        protected void assembleStart()
        {
            if (tokens.Count <= 2)
            {
                if (pass == 2) error("Invalid format for ORG");
                return;
            }
            memory[3] = (byte)(convertValue(tokens[2]) & 0xff);

        }

        protected void assembleLine(String line)
        {
            String label;
            String opcode;
            Boolean result;
            linesAssembled++;
            outputLine = address.ToString("x2") + " ";
            tokenize(line, "!@#$%^&*()_+`'-=<>,./?[]{}\\| \t;:~");
            reduce();
            if (tokens.Count < 1) return;
            if (tokens[0].Length > 0)
            {
                label = tokens[0];
                result = addLabel(label, address);
                if (pass == 1 && result)
                {
                    errors++;
                    results += "Error: Line " + lineNumber.ToString() + " Multiply defined label: " + label + "\r\n";
                }
            }
            if (tokens.Count < 2) return;
            opcode = tokens[1];
            if (opcode.ToUpper().Equals("ADD")) assembleType1(0x00);
            else if (opcode.ToUpper().Equals("SUB")) assembleType1(0x08);
            else if (opcode.ToUpper().Equals("LOAD")) assembleType1(0x10);
            else if (opcode.ToUpper().Equals("STORE")) assembleType1(0x18);
            else if (opcode.ToUpper().Equals("AND")) assembleType2(0xd0);
            else if (opcode.ToUpper().Equals("OR")) assembleType2(0xc0);
            else if (opcode.ToUpper().Equals("LNEG")) assembleType2(0xd8);
            else if (opcode.ToUpper().Equals("SFTL")) assembleShifts(0x81);
            else if (opcode.ToUpper().Equals("SFTR")) assembleShifts(0x01);
            else if (opcode.ToUpper().Equals("ROTL")) assembleShifts(0xc1);
            else if (opcode.ToUpper().Equals("ROTR")) assembleShifts(0x41);
            else if (opcode.ToUpper().Equals("SET0")) assembleSet(0x02);
            else if (opcode.ToUpper().Equals("SET1")) assembleSet(0x42);
            else if (opcode.ToUpper().Equals("SKP0")) assembleSet(0x82);
            else if (opcode.ToUpper().Equals("SKP1")) assembleSet(0xc2);
            else if (opcode.ToUpper().Equals("JPD")) assembleJumps(0x20);
            else if (opcode.ToUpper().Equals("JPI")) assembleJumps(0x28);
            else if (opcode.ToUpper().Equals("JMD")) assembleJumps(0x30);
            else if (opcode.ToUpper().Equals("JMI")) assembleJumps(0x38);
            else if (opcode.ToUpper().Equals("NOOP")) writeByte(0x80);
            else if (opcode.ToUpper().Equals("HALT")) writeByte(0x00);
            else if (opcode.ToUpper().Equals("DATA")) assembleData();
            else if (opcode.ToUpper().Equals("ORG")) assembleOrg();
            else if (opcode.ToUpper().Equals("START")) assembleStart();
            if (pass == 2)
            {
                while (outputLine.Length < 10) outputLine += " ";
                results += outputLine + line + "\r\n";
            }
        }

        protected String expandTabs(String input)
        {
            String ret;
            ret = "";
            foreach (var c in input)
            {
                if (c == 9)
                {
                    ret += " ";
                    while (ret.Length % 8 != 0) ret += " ";
                }
                else ret += c;
            }
            return ret;
        }

        protected void assemblyPass(int n, String[] lines)
        {
            pass = n;
            address = 0;
            lineNumber = 0;
            linesAssembled = 0;
            bytesAssembled = 0;
            results += "Pass " + pass.ToString() + ":\r\n";
            foreach (var line in lines)
            {
                lineNumber++;
                assembleLine(expandTabs(line));
            }
        }

        public String Assemble(String[] lines, byte[] mem)
        {
            results = "";
            errors = 0;
            startAddress = 0x0000;
            memory = mem;
            labels = new List<List<object>>();
            assemblyPass(1, lines);
            assemblyPass(2, lines);
            results += "\r\n";
            results += "Lines Assembled: " + linesAssembled.ToString() + "\r\n";
            results += "Bytes Assembled: " + bytesAssembled.ToString() + "\r\n";
            results += "Errors         : " + errors.ToString() + "\r\n";
            return results;
        }
    }
}
