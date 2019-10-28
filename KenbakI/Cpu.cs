using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenbakI
{
    public class Cpu
    {
        public const int LAMPS_INPUT = 0;
        public const int LAMPS_ADDRESS = 1;
        public const int LAMPS_MEMORY = 2;
        public const int LAMPS_RUN = 3;
        public const int REG_A = 0;
        public const int REG_B = 1;
        public const int REG_X = 2;
        public const int REG_P = 3;
        public const int REG_OUT = 0x80;
        public const int REG_AF = 0x81;
        public const int REG_BF = 0x82;
        public const int REG_XF = 0x83;
        public const int REG_IN = 0xff;
        public byte[] memory;
        public Boolean running;
        public Boolean powered;
        public int lampMode;
        public byte addressRegister;
        public byte memoryValue;
        protected byte inst;
        protected byte reg;
        protected byte mode;
        protected byte middle;
        public Boolean debugMode;
        public String debug;

        public Cpu()
        {
            memory = new byte[256];
            running = false;
            powered = true;
            lampMode = LAMPS_INPUT;
            addressRegister = 0;
            debug = "";
        }

        public void ClearMem()
        {
            for (var i = 0; i < memory.Length; i++) memory[i] = 0;
        }

        public void Run()
        {
            running = true;
        }

        public void Halt()
        {
            running = false;
        }

        protected byte fetch()
        {
            byte ret;
            ret = memory[memory[REG_P]];
            memory[REG_P]++;
            return ret;
        }

        protected String regToString(byte reg)
        {
            if (reg == 0) return "A";
            if (reg == 1) return "B";
            if (reg == 2) return "X";
            return "";
        }

        protected String addressToString(byte mode, byte ea)
        {
            switch (mode)
            {
                case 3: return "C=$" + memory[ea].ToString("X2");
                case 4: return "$" + ea.ToString("X2");
                case 5: return "($" + ea.ToString("X2") + ")";
                case 6: return "$" + ea.ToString("X2") + ",X";
                case 7: return "($" + ea.ToString("X2") + "),X";
            }
            return "";
        }

        protected void doJump()
        {
            byte ea;
            byte value;
            Boolean jump;
            jump = false;
            ea = fetch();
            if (debugMode)
            {
                debug += ((inst & 0x10) == 0) ? "JP" : "JM";
                debug += ((inst & 0x08) == 0) ? "D" : "I";
                switch (inst & 0xc0)
                {
                    case 0x00: debug += "A"; break;
                    case 0x40: debug += "B"; break;
                    case 0x80: debug += "X"; break;
                }
                debug += " ";
                if ((inst & 0xc0) != 0xc0)
                {
                    switch (inst & 0x07)
                    {
                        case 0x03: debug += "NZ,"; break;
                        case 0x04: debug += "Z,"; break;
                        case 0x05: debug += "M,"; break;
                        case 0x06: debug += "P,"; break;
                        case 0x07: debug += "PNZ,"; break;
                    }
                }
                debug += ((inst & 0x08) == 0x00) ? ea.ToString("X2") : "(" + ea.ToString("X2") + ")";
            }
            if ((inst & 0x08) != 0) ea = memory[ea];
            if (reg == 3) jump = true;                                   // unconditional
            else
            {
                switch (reg)
                {
                    case 0: value = memory[REG_A]; break;
                    case 1: value = memory[REG_B]; break;
                    case 2: value = memory[REG_X]; break;
                    default: value = 0; break;
                }
                switch (mode)
                {
                    case 3:
                        if (value != 0) jump = true;
                        break;
                    case 4:
                        if (value == 0) jump = true;
                        break;
                    case 5:
                        if (value >= 0x80) jump = true;
                        break;
                    case 6:
                        if (value < 0x80) jump = true;
                        break;
                    case 7: if (value > 0 && value < 0x80) jump = true;
                        break;
                }
            }
            if (jump)
            {
                if ((inst & 0x10) != 0)
                {
                    memory[ea] = memory[REG_P];
                    memory[REG_P] = (byte)(ea+1);
                }
                else
                {
                    memory[REG_P] = ea;
                }
            }
        }

        protected void doHalt()
        {
            running = false;
            if (debugMode) debug += "HALT";
        }

        protected void doNop()
        {
            if (debugMode) debug += "NOP";
        }

        protected void doShift()
        {
            int count;
            byte bit;
            count = (inst >> 3) & 0x3;
            if (count == 0) count = 4;
            reg = (byte)((inst >> 5) & 1);
            if (debugMode)
            {
                debug += (((inst & 0x40) != 0) ? "ROT" : "SFT");
                debug += (((inst & 0x80) != 0) ? "L" : "R");
                debug += " " + regToString(reg) + "," + count.ToString();
            }
            if ((inst & 0x80) == 0)                           // Right
            {
                for (var i = 0; i < count; i++)
                {
                    bit = (byte)((memory[reg] & 1) << 7);
                    memory[reg] = (byte)((memory[reg] >> 1) & 0x7f);
                    if ((inst & 0x40) == 0x00)                // Shift
                    {
                        memory[reg] |= (byte)((memory[reg] << 1) & 0x80);
                    }
                    else                                      // Rotate
                    {
                        memory[reg] |= bit;
                    }
                }
            }
            else                                              // Left
            {
                for (var i = 0; i < count; i++)
                {
                    bit = (byte)((memory[reg] & 0x80) >> 7);
                    memory[reg] = (byte)((memory[reg] << 1) & 0xfe);
                    if ((inst & 0x40) == 0x00)                // Shift
                    {
                    }
                    else                                      // Rotate
                    {
                        memory[reg] |= bit;
                    }
                }
            }
        }

        protected void doSkip()
        {
            byte mask;
            byte value;
            if (debugMode) debug += "SKP" + (((reg & 1) == 1) ? "1" : "0") + middle.ToString() + "," + addressToString(4, memory[REG_P]);
            mask = (byte)(1 << middle);
            value = (byte)(memory[fetch()] & mask);
            if (((reg & 1) == 0) && value == 0) memory[REG_P] += 2;
            if (((reg & 1) == 1) && value != 0) memory[REG_P] += 2;
        }

        protected void doSet()
        {
            byte mask;
            byte value;
            byte ea;
            mask = (byte)(1 << middle);
            ea = fetch();
            if (debugMode) debug += "SET" + (((reg & 1) == 1) ? "1" : "0") + middle.ToString() + "," + addressToString(4, ea);
            value = memory[ea];
            if ((reg & 1) == 1) value |= mask;
            else value &= (byte)(~mask);
            memory[ea] = value;
        }

        protected void doGroup1(byte ea)
        {
            byte a, b;
            int temp;
            switch (middle) {
                case 0:                                      // ADD
                    if (debugMode) debug += "ADD" + regToString(reg) + " " + addressToString(mode, ea);
                    a = memory[reg];
                    b = memory[ea];
                    temp = a + b;
                    memory[reg] = (byte)(temp);
                    memory[REG_AF + reg] = 0x00;
                    if (temp > 0xff) memory[REG_AF + reg] |= 2;
                    if ((a & 0x80) == (b & 0x80))
                    {
                        if ((temp & 0x80) != (a & 0x80)) memory[REG_AF + reg] |= 1;
                    }
                    break;
                case 1:                                      // SUB
                    if (debugMode) debug += "SUB" + regToString(reg) + " " + addressToString(mode, ea);
                    a = memory[reg];
                    b = (byte)(~memory[ea] + 1);
                    temp = a + b;
                    memory[reg] = (byte)(temp);
                    memory[REG_AF + reg] = 0x00;
                    if (temp > 0xff) memory[REG_AF + reg] |= 2;
                    if ((a & 0x80) == (b & 0x80))
                    {
                        if ((temp & 0x80) != (a & 0x80)) memory[REG_AF + reg] |= 1;
                    }
                    memory[REG_AF + reg] ^= 2;
                    break;
                case 2:                                      // LOAD
                    if (debugMode) debug += "LOAD" + regToString(reg) + " " + addressToString(mode, ea);
                    memory[reg] = memory[ea];
                    break;
                case 3:                                      // STORE
                    if (debugMode) debug += "STORE" + regToString(reg) + " " + addressToString(mode, ea);
                    memory[ea] = memory[reg];
                    break;
            }
        }

        protected void doGroup2(byte ea)
        {
            switch (middle)
            {
                case 0:                                      // OR
                    if (debugMode) debug += "OR " + addressToString(mode,ea);
                    memory[REG_A] |= memory[ea];
                    break;
                case 2:                                      // AND
                    if (debugMode) debug += "AND " + addressToString(mode,ea);
                    memory[REG_A] &= memory[ea];
                    break;
                case 3:                                      // LNEG
                    if (debugMode) debug += "LNEG " + addressToString(mode,ea);
                    memory[REG_A] = (byte)(~memory[ea] + 1);
                    break;
            }
        }

        public void cycle()
        {
            byte ea;
            if (!powered) return;
            if (!running) return;
            if (debugMode) debug += "[" + memory[REG_P].ToString("X2") + "] ";
            inst = fetch();
            if (debugMode) debug += inst.ToString("X2") + " ";
            reg = (byte)((inst >> 6) & 0x3);
            mode = (byte)(inst & 0x7);
            middle = (byte)((inst >> 3) & 0x7);
            if (middle >= 4 && mode >= 3) doJump();
            else switch (mode)
                {
                    case 0: if (reg > 1) doNop(); else doHalt();
                        break;
                    case 1: doShift();
                        break;
                    case 2: if (reg > 1) doSkip(); else doSet();
                        break;
                    case 3:
                        ea = memory[REG_P]++;
                        if (reg != 3) doGroup1(ea); else doGroup2(ea);
                        break;
                    case 4:
                        ea = fetch();
                        if (reg != 3) doGroup1(ea); else doGroup2(ea);
                        break;
                    case 5:
                        ea = fetch();
                        ea = memory[ea];
                        if (reg != 3) doGroup1(ea); else doGroup2(ea);
                        break;
                    case 6:
                        ea = (byte)(fetch() + memory[REG_X]);
                        if (reg != 3) doGroup1(ea); else doGroup2(ea);
                        break;
                    case 7:
                        ea = fetch();
                        ea = (byte)(memory[ea] + memory[REG_X]);
                        if (reg != 3) doGroup1(ea); else doGroup2(ea);
                        break;
                }
            if (debugMode) debug += "\r\n";
        }
    }
}
