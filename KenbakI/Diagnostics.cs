using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KenbakI
{
    public class Diagnostics
    {
        protected Cpu cpu;
        protected int goodCount;
        protected int badCount;
        protected TextBox output;

        public Diagnostics(TextBox o)
        {
            cpu = new Cpu();
            cpu.powered = true;
            goodCount = 0;
            badCount = 0;
            output = o;
        }

        protected void good(String message)
        {
            goodCount++;
            output.AppendText("Good: " + message + "\r\n");
        }

        protected void bad(String message)
        {
            badCount++;
            output.AppendText("BAD : " + message + "\r\n");
        }

        protected void addATests()
        {
            output.AppendText("\r\n");
            output.AppendText("Add A Tests\r\n");
            output.AppendText("===========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x03;                   // ADD C=$0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[Cpu.REG_A] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x16) good("A was correct after ADD C=$0c");
            else bad("A was not correct after ADD C=$0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD C=$0c");
            else bad("P was not correct after ADD C=$0c");
            cpu.memory[0x80] = 0x04;                   // ADD $0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[0x0c] = 0x04;
            cpu.memory[Cpu.REG_A] = 0x02;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x06) good("A was correct after ADD $0c");
            else bad("A was not correct after ADD $0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD $0c");
            else bad("P was not correct after ADD $0c");
            cpu.memory[0x80] = 0x05;                   // ADD ($0d)
            cpu.memory[0x81] = 0x0d;
            cpu.memory[0x0d] = 0x14;
            cpu.memory[0x14] = 0x07;
            cpu.memory[Cpu.REG_A] = 0x08;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x0f) good("A was correct after ADD ($0d)");
            else bad("A was not correct after ADD ($0d)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD ($0d)");
            else bad("P was not correct after ADD ($0d)");
            cpu.memory[0x80] = 0x06;                   // ADD $0e,X
            cpu.memory[0x81] = 0x0e;
            cpu.memory[0x10] = 0x04;
            cpu.memory[Cpu.REG_A] = 0x08;
            cpu.memory[Cpu.REG_X] = 0x02;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x0c) good("A was correct after ADD $0e,X");
            else bad("A was not correct after ADD $0e,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD $0e,X");
            else bad("P was not correct after ADD $0e,X");
            cpu.memory[0x80] = 0x07;                   // ADD ($0f),X
            cpu.memory[0x81] = 0x0f;
            cpu.memory[0x0f] = 0x20;
            cpu.memory[0x24] = 0x23;
            cpu.memory[Cpu.REG_A] = 0x34;
            cpu.memory[Cpu.REG_X] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x57) good("A was correct after ADD ($0f),X");
            else bad("A was not correct after ADD ($0f),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD ($0f),X");
            else bad("P was not correct after ADD ($0f),X");
            cpu.memory[0x80] = 0x03;                   // ADD C=$03
            cpu.memory[0x81] = 0x03;
            cpu.memory[Cpu.REG_A] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x00) good("Carry was clear after 4+3");
            else bad("Carry was not clear after 4+3");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x00) good("Overflow was clear after 4+3");
            else bad("Overflow was not clear after 4+3");
            cpu.memory[0x80] = 0x03;                   // ADD C=$FF
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_A] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x02) good("Carry was set after 255+1");
            else bad("Carry was not set after 255+1");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x00) good("Overflow was clear after 255+1");
            else bad("Overflow was not clear after 255+1");
            cpu.memory[0x80] = 0x03;                   // ADD C=$7F
            cpu.memory[0x81] = 0x7f;
            cpu.memory[Cpu.REG_A] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x00) good("Carry was clear after 127+1");
            else bad("Carry was not clear after 127+1");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x01) good("Overflow was set after 127+1");
            else bad("Overflow was not set after 127+1");
            cpu.memory[0x80] = 0x03;                   // ADD C=$7F
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_A] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x02) good("Carry was set after -128+-1");
            else bad("Carry was not set after -129+-1");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x01) good("Overflow was set after -128+-1");
            else bad("Overflow was not set after -128+-1");
        }

        protected void addBTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Add B Tests\r\n");
            output.AppendText("===========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x43;                   // ADD C=$0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[Cpu.REG_B] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x16) good("B was correct after ADD C=$0c");
            else bad("B was not correct after ADD C=$0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD C=$0c");
            else bad("P was not correct after ADD C=$0c");
            cpu.memory[0x80] = 0x44;                   // ADD $0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[0x0c] = 0x04;
            cpu.memory[Cpu.REG_B] = 0x02;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x06) good("B was correct after ADD $0c");
            else bad("B was not correct after ADD $0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD $0c");
            else bad("P was not correct after ADD $0c");
            cpu.memory[0x80] = 0x45;                   // ADD ($0d)
            cpu.memory[0x81] = 0x0d;
            cpu.memory[0x0d] = 0x14;
            cpu.memory[0x14] = 0x07;
            cpu.memory[Cpu.REG_B] = 0x08;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x0f) good("B was correct after ADD ($0d)");
            else bad("B was not correct after ADD ($0d)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD ($0d)");
            else bad("P was not correct after ADD ($0d)");
            cpu.memory[0x80] = 0x46;                   // ADD $0e,X
            cpu.memory[0x81] = 0x0e;
            cpu.memory[0x10] = 0x04;
            cpu.memory[Cpu.REG_B] = 0x08;
            cpu.memory[Cpu.REG_X] = 0x02;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x0c) good("B was correct after ADD $0e,X");
            else bad("B was not correct after ADD $0e,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD $0e,X");
            else bad("P was not correct after ADD $0e,X");
            cpu.memory[0x80] = 0x47;                   // ADD ($0f),X
            cpu.memory[0x81] = 0x0f;
            cpu.memory[0x0f] = 0x20;
            cpu.memory[0x24] = 0x23;
            cpu.memory[Cpu.REG_B] = 0x34;
            cpu.memory[Cpu.REG_X] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x57) good("B was correct after ADD ($0f),X");
            else bad("B was not correct after ADD ($0f),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD ($0f),X");
            else bad("P was not correct after ADD ($0f),X");
            cpu.memory[0x80] = 0x43;                   // ADD C=$03
            cpu.memory[0x81] = 0x03;
            cpu.memory[Cpu.REG_B] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x00) good("Carry was clear after 4+3");
            else bad("Carry was not clear after 4+3");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x00) good("Overflow was clear after 4+3");
            else bad("Overflow was not clear after 4+3");
            cpu.memory[0x80] = 0x43;                   // ADD C=$FF
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_B] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x02) good("Carry was set after 255+1");
            else bad("Carry was not set after 255+1");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x00) good("Overflow was clear after 255+1");
            else bad("Overflow was not clear after 255+1");
            cpu.memory[0x80] = 0x43;                   // ADD C=$7F
            cpu.memory[0x81] = 0x7f;
            cpu.memory[Cpu.REG_B] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x00) good("Carry was clear after 127+1");
            else bad("Carry was not clear after 127+1");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x01) good("Overflow was set after 127+1");
            else bad("Overflow was not set after 127+1");
            cpu.memory[0x80] = 0x43;                   // ADD C=$7F
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_B] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x02) good("Carry was set after -128+-1");
            else bad("Carry was not set after -129+-1");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x01) good("Overflow was set after -128+-1");
            else bad("Overflow was not set after -128+-1");
        }

        protected void addXTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Add X Tests\r\n");
            output.AppendText("===========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x83;                   // ADD C=$0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[Cpu.REG_X] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x16) good("X was correct after ADD C=$0c");
            else bad("X was not correct after ADD C=$0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD C=$0c");
            else bad("P was not correct after ADD C=$0c");
            cpu.memory[0x80] = 0x84;                   // ADD $0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[0x0c] = 0x04;
            cpu.memory[Cpu.REG_X] = 0x02;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x06) good("X was correct after ADD $0c");
            else bad("X was not correct after ADD $0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD $0c");
            else bad("P was not correct after ADD $0c");
            cpu.memory[0x80] = 0x85;                   // ADD ($0d)
            cpu.memory[0x81] = 0x0d;
            cpu.memory[0x0d] = 0x14;
            cpu.memory[0x14] = 0x07;
            cpu.memory[Cpu.REG_X] = 0x08;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x0f) good("X was correct after ADD ($0d)");
            else bad("X was not correct after ADD ($0d)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD ($0d)");
            else bad("P was not correct after ADD ($0d)");
            cpu.memory[0x80] = 0x86;                   // ADD $0e,X
            cpu.memory[0x81] = 0x0e;
            cpu.memory[0x10] = 0x04;
            cpu.memory[Cpu.REG_X] = 0x02;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x06) good("X was correct after ADD $0e,X");
            else bad("X was not correct after ADD $0e,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD $0e,X");
            else bad("P was not correct after ADD $0e,X");
            cpu.memory[0x80] = 0x87;                   // ADD ($0f),X
            cpu.memory[0x81] = 0x0f;
            cpu.memory[0x0f] = 0x20;
            cpu.memory[0x24] = 0x23;
            cpu.memory[Cpu.REG_X] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x27) good("X was correct after ADD ($0f),X");
            else bad("X was not correct after ADD ($0f),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after ADD ($0f),X");
            else bad("P was not correct after ADD ($0f),X");
            cpu.memory[0x80] = 0x83;                   // ADD C=$03
            cpu.memory[0x81] = 0x03;
            cpu.memory[Cpu.REG_X] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x00) good("Carry was clear after 4+3");
            else bad("Carry was not clear after 4+3");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x00) good("Overflow was clear after 4+3");
            else bad("Overflow was not clear after 4+3");
            cpu.memory[0x80] = 0x83;                   // ADD C=$FF
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_X] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x02) good("Carry was set after 255+1");
            else bad("Carry was not set after 255+1");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x00) good("Overflow was clear after 255+1");
            else bad("Overflow was not clear after 255+1");
            cpu.memory[0x80] = 0x83;                   // ADD C=$7F
            cpu.memory[0x81] = 0x7f;
            cpu.memory[Cpu.REG_X] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x00) good("Carry was clear after 127+1");
            else bad("Carry was not clear after 127+1");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x01) good("Overflow was set after 127+1");
            else bad("Overflow was not set after 127+1");
            cpu.memory[0x80] = 0x83;                   // ADD C=$7F
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_X] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x02) good("Carry was set after -128+-1");
            else bad("Carry was not set after -129+-1");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x01) good("Overflow was set after -128+-1");
            else bad("Overflow was not set after -128+-1");
        }

        protected void andTests()
        {
            output.AppendText("\r\n");
            output.AppendText("And Tests\r\n");
            output.AppendText("=========\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0xd3;                   // AND C=$0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[Cpu.REG_A] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x08) good("A was correct after AND C=$0c");
            else bad("A was not correct after AND C=$0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after AND C=$1c");
            else bad("P was not correct after AND C=$1c");
            cpu.memory[0x80] = 0xd4;                   // AND $50
            cpu.memory[0x81] = 0x50;
            cpu.memory[0x50] = 0x18;
            cpu.memory[Cpu.REG_A] = 0x14;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x10) good("A was correct after AND $50");
            else bad("A was not correct after AND $50");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after AND $50");
            else bad("P was not correct after AND $50");
            cpu.memory[0x80] = 0xd5;                   // AND ($51)
            cpu.memory[0x81] = 0x51;
            cpu.memory[0x51] = 0x19;
            cpu.memory[0x19] = 0x30;
            cpu.memory[Cpu.REG_A] = 0x28;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x20) good("A was correct after AND ($51)");
            else bad("A was not correct after AND ($51)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after AND ($51)");
            else bad("P was not correct after AND ($51)");
            cpu.memory[0x80] = 0xd6;                   // AND $52,X
            cpu.memory[0x81] = 0x52;
            cpu.memory[0x5a] = 0x60;
            cpu.memory[Cpu.REG_A] = 0x50;
            cpu.memory[Cpu.REG_X] = 0x08;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x40) good("A was correct after AND $52,X");
            else bad("A was not correct after AND $52,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after AND $52,X");
            else bad("P was not correct after AND $52,X");
            cpu.memory[0x80] = 0xd7;                   // AND ($53),X
            cpu.memory[0x81] = 0x53;
            cpu.memory[0x53] = 0xc0;
            cpu.memory[0xc9] = 0xc0;
            cpu.memory[Cpu.REG_A] = 0xa0;
            cpu.memory[Cpu.REG_X] = 0x09;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x80) good("A was correct after AND ($53),X");
            else bad("A was not correct after AND ($53),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after AND ($53),X");
            else bad("P was not correct after AND ($53),X");
        }

        protected void haltTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Halt Tests\r\n");
            output.AppendText("==========\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0x00;                   // HALT
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after HALT");
            else bad("P was not correct after HALT");
            if (cpu.running == false) good("Machine was halted");
            else bad("Machine was not halted");
        }

        protected void jumpTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Jump Tests\r\n");
            output.AppendText("==========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0xe3;                   // jpd $40
            cpu.memory[0x81] = 0x40;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x40) good("P was correct after unconditional direct jump");
            else bad("P was not correct after unconditional direct jump");
            cpu.memory[0x80] = 0xeb;                   // jpi $40
            cpu.memory[0x81] = 0x40;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0xa5) good("P was correct after unconditional indirect jump");
            else bad("P was not correct after unconditional indirect jump");
            
            // *********** With Mark **********
            cpu.memory[0x80] = 0xf3;                   // jmd $40
            cpu.memory[0x81] = 0x40;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x41) good("P was correct after unconditional direct jump with mark");
            else bad("P was not correct after unconditional direct jump with mark");
            if (cpu.memory[0x40] == 0x82) good("P was written correctly after jump with mark");
            else bad("P was not written correctly after jump with mark");
            cpu.memory[0x80] = 0xfb;                   // jmi $40
            cpu.memory[0x81] = 0x40;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0xa6) good("P was correct after unconditional indirect jump with mark");
            else bad("P was not correct after unconditional indirect jump with mark");
            if (cpu.memory[0xa5] == 0x82) good("P was written correctly after indirect jump with mark");
            else bad("P was not written correctly after indirect jump with mark");

            // *********** Tests with A **********
            cpu.memory[0x80] = 0x23;                   // jpda nz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDA NZ jumped when A was nonzero");
            else bad("JPDA NZ did not jump when A was nonzero");
            cpu.memory[0x80] = 0x23;                   // jpda nz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDA NZ did not jump when A was zero");
            else bad("JPDA NZ jumped when A was zero");

            cpu.memory[0x80] = 0x24;                   // jpda z $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDA Z jumped when A was zero");
            else bad("JPDA Z did not jump when A was zero");
            cpu.memory[0x80] = 0x24;                   // jpda z $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDA Z did not jump when A was nonzero");
            else bad("JPDA Z jumped when A was nonzero");

            cpu.memory[0x80] = 0x25;                   // jpda n $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDA N jumped when A was negative");
            else bad("JPDA N did not jump when A was negative");
            cpu.memory[0x80] = 0x25;                   // jpda n $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDA N did not jump when A was positive");
            else bad("JPDA N jumped when A was positive");

            cpu.memory[0x80] = 0x26;                   // jpda p $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x06;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDA P jumped when A was positive");
            else bad("JPDA P did not jump when A was positive");
            cpu.memory[0x80] = 0x26;                   // jpda p $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x81;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDA P did not jump when A was negative");
            else bad("JPDA P jumped when P was negative");

            cpu.memory[0x80] = 0x27;                   // jpda pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x06;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDA PNZ jumped when A was positive");
            else bad("JPDA PNZ did not jump when A was positive");
            cpu.memory[0x80] = 0x27;                   // jpda pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x81;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDA PNZ did not jump when A was negative");
            else bad("JPDA PNZN jumped when P was negative");
            cpu.memory[0x80] = 0x27;                   // jpda pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDA PNZ did not jump when A was zero");
            else bad("JPDA PNZN jumped when P was zero");

            // *********** Tests with B **********
            cpu.memory[0x80] = 0x63;                   // jpdb nz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDB NZ jumped when B was nonzero");
            else bad("JPDB NZ did not jump when B was nonzero");
            cpu.memory[0x80] = 0x63;                   // jpdb nz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDB NZ did not jump when B was zero");
            else bad("JPDB NZ jumped when B was zero");

            cpu.memory[0x80] = 0x64;                   // jpdb z $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDB Z jumped when B was zero");
            else bad("JPDB Z did not jump when B was zero");
            cpu.memory[0x80] = 0x64;                   // jpdb z $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDB Z did not jump when B was nonzero");
            else bad("JPDB Z jumped when B was nonzero");

            cpu.memory[0x80] = 0x65;                   // jpdb n $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDB N jumped when B was negative");
            else bad("JPDB N did not jump when B was negative");
            cpu.memory[0x80] = 0x65;                   // jpdb n $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDB N did not jump when B was positive");
            else bad("JPDB N jumped when B was positive");

            cpu.memory[0x80] = 0x66;                   // jpdb p $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x06;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDB P jumped when B was positive");
            else bad("JPDB P did not jump when B was positive");
            cpu.memory[0x80] = 0x66;                   // jpdb p $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x81;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDB P did not jump when B was negative");
            else bad("JPDB P jumped when P was negative");

            cpu.memory[0x80] = 0x67;                   // jpdb pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x06;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDB PNZ jumped when B was positive");
            else bad("JPDB PNZ did not jump when B was positive");
            cpu.memory[0x80] = 0x67;                   // jpdb pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x81;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDB PNZ did not jump when B was negative");
            else bad("JPDB PNZN jumped when P was negative");
            cpu.memory[0x80] = 0x67;                   // jpdb pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDB PNZ did not jump when B was zero");
            else bad("JPDB PNZN jumped when P was zero");

            // *********** Tests with X **********
            cpu.memory[0x80] = 0xa3;                   // jpdx nz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDX NZ jumped when X was nonzero");
            else bad("JPDX NZ did not jump when X was nonzero");
            cpu.memory[0x80] = 0xa3;                   // jpdx nz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDX NZ did not jump when X was zero");
            else bad("JPDX NZ jumped when X was zero");

            cpu.memory[0x80] = 0xa4;                   // jpdx z $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDX Z jumped when X was zero");
            else bad("JPDX Z did not jump when X was zero");
            cpu.memory[0x80] = 0xa4;                   // jpdx z $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDX Z did not jump when X was nonzero");
            else bad("JPDX Z jumped when X was nonzero");

            cpu.memory[0x80] = 0xa5;                   // jpdx n $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDX N jumped when X was negative");
            else bad("JPDX N did not jump when X was negative");
            cpu.memory[0x80] = 0xa5;                   // jpdx n $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDX N did not jump when X was positive");
            else bad("JPDX N jumped when X was positive");

            cpu.memory[0x80] = 0xa6;                   // jpdx p $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x06;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDX P jumped when X was positive");
            else bad("JPDX P did not jump when X was positive");
            cpu.memory[0x80] = 0xa6;                   // jpdx p $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x81;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDX P did not jump when X was negative");
            else bad("JPDX P jumped when P was negative");

            cpu.memory[0x80] = 0xa7;                   // jpdx pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x06;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x45) good("JPDX PNZ jumped when X was positive");
            else bad("JPDX PNZ did not jump when X was positive");
            cpu.memory[0x80] = 0xa7;                   // jpdx pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x81;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDX PNZ did not jump when X was negative");
            else bad("JPDX PNZN jumped when P was negative");
            cpu.memory[0x80] = 0xa7;                   // jpdx pnz $45
            cpu.memory[0x81] = 0x45;
            cpu.memory[0x40] = 0xa5;
            cpu.memory[Cpu.REG_X] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("JPDX PNZ did not jump when X was zero");
            else bad("JPDX PNZN jumped when P was zero");
        }

        protected void lnegTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Lneg Tests\r\n");
            output.AppendText("==========\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0xdb;                   // LNEG C=$5
            cpu.memory[0x81] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xfb) good("A was correct after LNEG C=$05");
            else bad("A was not correct after LNEG C=$05");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LNEG");
            else bad("P was not correct after LNEG");
            cpu.memory[0x80] = 0xdc;                   // LNEG $12
            cpu.memory[0x81] = 0x12;
            cpu.memory[0x12] = 0x03;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xfd) good("A was correct after LNEG $12");
            else bad("A was not correct after LNEG $12");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LNEG");
            else bad("P was not correct after LNEG");
            cpu.memory[0x80] = 0xdd;                   // LNEG ($13)
            cpu.memory[0x81] = 0x13;
            cpu.memory[0x13] = 0x70;
            cpu.memory[0x70] = 0xf5;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x0b) good("A was correct after LNEG ($13)");
            else bad("A was not correct after LNEG ($13)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LNEG");
            else bad("P was not correct after LNEG");
            cpu.memory[0x80] = 0xde;                   // LNEG $14,X
            cpu.memory[0x81] = 0x14;
            cpu.memory[0x1b] = 0x72;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.memory[Cpu.REG_X] = 0x07;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x8e) good("A was correct after LNEG $14,X");
            else bad("A was not correct after LNEG $14,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LNEG");
            else bad("P was not correct after LNEG");
            cpu.memory[0x80] = 0xdf;                   // LNEG ($15),X
            cpu.memory[0x81] = 0x15;
            cpu.memory[0x15] = 0xa0;
            cpu.memory[0xa7] = 0x19;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.memory[Cpu.REG_X] = 0x07;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xe7) good("A was correct after LNEG ($15),X");
            else bad("A was not correct after LNEG ($15),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LNEG");
            else bad("P was not correct after LNEG");
            cpu.memory[0x80] = 0xdb;                   // LNEG C=$80
            cpu.memory[0x81] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x80) good("A was correct after LNEG C=$80");
            else bad("A was not correct after LNEG C=$80");
        }
        
        protected void loadATests()
        {
            output.AppendText("\r\n");
            output.AppendText("Load A Tests\r\n");
            output.AppendText("============\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0x13;                   // LOAD A,C=$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x10) good("A was correct after LOAD A,C=$10");
            else bad("A was not correct after LOAD A,C=$10");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD A,C=$10");
            else bad("P was not correct after LOAD A,C=$10");
            cpu.memory[0x80] = 0x14;                   // LOAD A,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x6a;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x6a) good("A was correct after LOAD A,$10");
            else bad("A was not correct after LOAD A,$10");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD A,$10");
            else bad("P was not correct after LOAD A,$10");
            cpu.memory[0x80] = 0x15;                   // LOAD A,($12)
            cpu.memory[0x81] = 0x12;
            cpu.memory[0x12] = 0x7c;
            cpu.memory[0x7c] = 0xc9;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xc9) good("A was correct after LOAD A,($10)");
            else bad("A was not correct after LOAD A,($10)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD A,($10)");
            else bad("P was not correct after LOAD A,($10)");
            cpu.memory[0x80] = 0x16;                   // LOAD A,$14,X
            cpu.memory[0x81] = 0x14;
            cpu.memory[0x17] = 0xe4;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_X] = 0x03;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xe4) good("A was correct after LOAD A,$14,X");
            else bad("A was not correct after LOAD A,$14,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD A,$14,X");
            else bad("P was not correct after LOAD A,$14,X");
            cpu.memory[0x80] = 0x17;                   // LOAD A,($16),X
            cpu.memory[0x81] = 0x16;
            cpu.memory[0x16] = 0xa0;
            cpu.memory[0xa5] = 0x3d;
            cpu.memory[Cpu.REG_A] = 0x00;
            cpu.memory[Cpu.REG_X] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x3d) good("A was correct after LOAD A,($16),X");
            else bad("A was not correct after LOAD A,($16),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD A,($16),X");
            else bad("P was not correct after LOAD A,($16),X");

        }

        protected void loadBTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Load B Tests\r\n");
            output.AppendText("============\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0x53;                   // LOAD A,C=$17
            cpu.memory[0x81] = 0x17;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x17) good("B was correct after LOAD B,C=$17");
            else bad("B was not correct after LOAD B,C=$17");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD B,C=$17");
            else bad("P was not correct after LOAD B,C=$17");
            cpu.memory[0x80] = 0x54;                   // LOAD B,$18
            cpu.memory[0x81] = 0x18;
            cpu.memory[0x18] = 0x6a;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x6a) good("B was correct after LOAD B,$18");
            else bad("B was not correct after LOAD B,$18");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD B,$18");
            else bad("P was not correct after LOAD B,$18");
            cpu.memory[0x80] = 0x55;                   // LOAD B,($19)
            cpu.memory[0x81] = 0x19;
            cpu.memory[0x19] = 0xc7;
            cpu.memory[0xc7] = 0x9c;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x9c) good("B was correct after LOAD B,($19)");
            else bad("B was not correct after LOAD B,($19)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD B,($19)");
            else bad("P was not correct after LOAD B,($19)");
            cpu.memory[0x80] = 0x56;                   // LOAD B,$20,X
            cpu.memory[0x81] = 0x20;
            cpu.memory[0x2a] = 0x4e;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_X] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x4e) good("B was correct after LOAD B,$20,X");
            else bad("B was not correct after LOAD B,$20,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD B,$20,X");
            else bad("P was not correct after LOAD B,$20,X");
            cpu.memory[0x80] = 0x57;                   // LOAD B,($21),X
            cpu.memory[0x81] = 0x21;
            cpu.memory[0x21] = 0xb0;
            cpu.memory[0xbd] = 0xd3;
            cpu.memory[Cpu.REG_B] = 0x00;
            cpu.memory[Cpu.REG_X] = 0x0d;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0xd3) good("B was correct after LOAD B,($21),X");
            else bad("B was not correct after LOAD B,($21),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD B,($21),X");
            else bad("P was not correct after LOAD B,($21),X");
        }

        protected void loadXTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Load X Tests\r\n");
            output.AppendText("============\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0x93;                   // LOAD A,C=$30
            cpu.memory[0x81] = 0x30;
            cpu.memory[Cpu.REG_X] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x30) good("X was correct after LOAD X,C=$30");
            else bad("X was not correct after LOAD X,C=$30");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD X,C=$17");
            else bad("P was not correct after LOAD X,C=$17");
            cpu.memory[0x80] = 0x94;                   // LOAD X,$31
            cpu.memory[0x81] = 0x31;
            cpu.memory[0x31] = 0x8a;
            cpu.memory[Cpu.REG_X] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x8a) good("X was correct after LOAD X,$31");
            else bad("X was not correct after LOAD X,$31");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD X,$31");
            else bad("P was not correct after LOAD X,$31");
            cpu.memory[0x80] = 0x95;                   // LOAD X,($32)
            cpu.memory[0x81] = 0x32;
            cpu.memory[0x32] = 0xe4;
            cpu.memory[0xe4] = 0xbe;
            cpu.memory[Cpu.REG_X] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0xbe) good("X was correct after LOAD X,($32)");
            else bad("X was not correct after LOAD X,($32)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD X,($32)");
            else bad("P was not correct after LOAD X,($32)");
            cpu.memory[0x80] = 0x96;                   // LOAD X,$33,X
            cpu.memory[0x81] = 0x33;
            cpu.memory[0x39] = 0x61;
            cpu.memory[Cpu.REG_X] = 0x06;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x61) good("X was correct after LOAD X,$33,X");
            else bad("X was not correct after LOAD X,$33,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD X,$33,X");
            else bad("P was not correct after LOAD X,$33,X");
            cpu.memory[0x80] = 0x97;                   // LOAD X,($34),X
            cpu.memory[0x81] = 0x34;
            cpu.memory[0x34] = 0xa2;
            cpu.memory[0xa4] = 0x7a;
            cpu.memory[Cpu.REG_X] = 0x02;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x7a) good("X was correct after LOAD X,($34),X");
            else bad("X was not correct after LOAD X,($34),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after LOAD X,($34),X");
            else bad("P was not correct after LOAD X,($34),X");
        }

        protected void nopTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Nop Tests\r\n");
            output.AppendText("=========\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0x80;                   // NOOP
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after NOOP");
            else bad("P was not correct after NOOP");
        }

        protected void orTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Or Tests\r\n");
            output.AppendText("========\r\n");
            cpu.Run();
            cpu.memory[0x80] = 0xc3;                   // OR C=$0c
            cpu.memory[0x81] = 0x0c;
            cpu.memory[Cpu.REG_A] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x0e) good("A was correct after OR C=$0c");
            else bad("A was not correct after OR C=$0c");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after OR C=$1c");
            else bad("P was not correct after OR C=$1c");
            cpu.memory[0x80] = 0xc4;                   // OR $50
            cpu.memory[0x81] = 0x50;
            cpu.memory[0x50] = 0x18;
            cpu.memory[Cpu.REG_A] = 0x14;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x1c) good("A was correct after OR $50");
            else bad("A was not correct after OR $50");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after OR $50");
            else bad("P was not correct after OR $50");
            cpu.memory[0x80] = 0xc5;                   // OR ($51)
            cpu.memory[0x81] = 0x51;
            cpu.memory[0x51] = 0x19;
            cpu.memory[0x19] = 0x30;
            cpu.memory[Cpu.REG_A] = 0x28;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x38) good("A was correct after OR ($51)");
            else bad("A was not correct after OR ($51)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after OR ($51)");
            else bad("P was not correct after OR ($51)");
            cpu.memory[0x80] = 0xc6;                   // OR $52,X
            cpu.memory[0x81] = 0x52;
            cpu.memory[0x5a] = 0x60;
            cpu.memory[Cpu.REG_A] = 0x50;
            cpu.memory[Cpu.REG_X] = 0x08;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x70) good("A was correct after OR $52,X");
            else bad("A was not correct after OR $52,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after OR $52,X");
            else bad("P was not correct after OR $52,X");
            cpu.memory[0x80] = 0xc7;                   // OR ($53),X
            cpu.memory[0x81] = 0x53;
            cpu.memory[0x53] = 0xc0;
            cpu.memory[0xc9] = 0xc0;
            cpu.memory[Cpu.REG_A] = 0xa0;
            cpu.memory[Cpu.REG_X] = 0x09;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xe0) good("A was correct after OR ($53),X");
            else bad("A was not correct after OR ($53),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after OR ($53),X");
            else bad("P was not correct after OR ($53),X");
        }

        protected void rotTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Rotate Tests\r\n");
            output.AppendText("============\r\n");
            cpu.Run();
            cpu.ClearMem();
            // ********** Right A **********
            cpu.memory[0x80] = 0x49;                   // ROTR A,1
            cpu.memory[Cpu.REG_A] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x44) good("A was correct after ROTR A,1");
            else bad("A was not correct after ROTR A,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0x51;                   // ROTR A,2
            cpu.memory[Cpu.REG_A] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x50) good("A was correct after ROTR A,2");
            else bad("A was not correct after ROTR A,2");
            cpu.memory[0x80] = 0x59;                   // ROTR A,3
            cpu.memory[Cpu.REG_A] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x13) good("A was correct after ROTR A,3");
            else bad("A was not correct after ROTR A,3");
            cpu.memory[0x80] = 0x41;                   // ROTR A,4
            cpu.memory[Cpu.REG_A] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x45) good("A was correct after ROTR A,4");
            else bad("A was not correct after ROTR A,4");
            // ********** Right B **********
            cpu.memory[0x80] = 0x69;                   // ROTR B,1
            cpu.memory[Cpu.REG_B] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x44) good("B was correct after ROTR B,1");
            else bad("B was not correct after ROTR B,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0x71;                   // ROTR B,2
            cpu.memory[Cpu.REG_B] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x50) good("B was correct after ROTR B,2");
            else bad("B was not correct after ROTR B,2");
            cpu.memory[0x80] = 0x79;                   // ROTR B,3
            cpu.memory[Cpu.REG_B] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x13) good("B was correct after ROTR B,3");
            else bad("B was not correct after ROTR B,3");
            cpu.memory[0x80] = 0x61;                   // ROTR B,4
            cpu.memory[Cpu.REG_B] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x45) good("B was correct after ROTR B,4");
            else bad("B was not correct after ROTR B,4");
            // ********** Left A **********
            cpu.memory[0x80] = 0xc9;                   // ROTL A,1
            cpu.memory[Cpu.REG_A] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x11) good("A was correct after ROTL A,1");
            else bad("A was not correct after ROTL A,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0xd1;                   // ROTL A,2
            cpu.memory[Cpu.REG_A] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x05) good("A was correct after ROTL A,2");
            else bad("A was not correct after ROTL A,2");
            cpu.memory[0x80] = 0xd9;                   // ROTL A,3
            cpu.memory[Cpu.REG_A] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xc4) good("A was correct after ROTL A,3");
            else bad("A was not correct after ROTL A,3");
            cpu.memory[0x80] = 0xc1;                   // ROTL A,4
            cpu.memory[Cpu.REG_A] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x45) good("A was correct after ROTL A,4");
            else bad("A was not correct after ROTL A,4");
            // ********** Left B **********
            cpu.memory[0x80] = 0xe9;                   // ROTL B,1
            cpu.memory[Cpu.REG_B] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x11) good("B was correct after ROTL B,1");
            else bad("B was not correct after ROTL B,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0xf1;                   // ROTL B,2
            cpu.memory[Cpu.REG_B] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x05) good("B was correct after ROTL B,2");
            else bad("B was not correct after ROTL B,2");
            cpu.memory[0x80] = 0xf9;                   // ROTL B,3
            cpu.memory[Cpu.REG_B] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0xc4) good("B was correct after ROTL B,3");
            else bad("B was not correct after ROTL B,3");
            cpu.memory[0x80] = 0xe1;                   // ROTL B,4
            cpu.memory[Cpu.REG_B] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x45) good("B was correct after ROTL B,4");
            else bad("B was not correct after ROTL B,4");
        }

        protected void setTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Set Tests\r\n");
            output.AppendText("=========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x02;                   // SET0 0,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0xfe) good("Memory was correct after SET0 0,$10");
            else bad("Memory was not correct after SET0 0,$10");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SET");
            else bad("P was not correct after SET");
            cpu.memory[0x80] = 0x0a;                   // SET0 1,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0xfd) good("Memory was correct after SET0 1,$10");
            else bad("Memory was not correct after SET0 1,$10");
            cpu.memory[0x80] = 0x12;                   // SET0 2,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0xfb) good("Memory was correct after SET0 2,$10");
            else bad("Memory was not correct after SET0 2,$10");
            cpu.memory[0x80] = 0x1a;                   // SET0 3,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0xf7) good("Memory was correct after SET0 3,$10");
            else bad("Memory was not correct after SET0 3,$10");
            cpu.memory[0x80] = 0x22;                   // SET0 4,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0xef) good("Memory was correct after SET0 4,$10");
            else bad("Memory was not correct after SET0 4,$10");
            cpu.memory[0x80] = 0x2a;                   // SET0 5,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0xdf) good("Memory was correct after SET0 5,$10");
            else bad("Memory was not correct after SET0 5,$10");
            cpu.memory[0x80] = 0x32;                   // SET0 6,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0xbf) good("Memory was correct after SET0 6,$10");
            else bad("Memory was not correct after SET0 6,$10");
            cpu.memory[0x80] = 0x3a;                   // SET0 7,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x7f) good("Memory was correct after SET0 7,$10");
            else bad("Memory was not correct after SET0 7,$10");
            cpu.memory[0x80] = 0x42;                   // SET1 0,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x01) good("Memory was correct after SET1 0,$10");
            else bad("Memory was not correct after SET1 0,$10");
            cpu.memory[0x80] = 0x4a;                   // SET1 1,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x02) good("Memory was correct after SET1 1,$10");
            else bad("Memory was not correct after SET1 1,$10");
            cpu.memory[0x80] = 0x52;                   // SET1 2,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x04) good("Memory was correct after SET1 2,$10");
            else bad("Memory was not correct after SET1 2,$10");
            cpu.memory[0x80] = 0x5a;                   // SET1 3,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x08) good("Memory was correct after SET1 3,$10");
            else bad("Memory was not correct after SET1 3,$10");
            cpu.memory[0x80] = 0x62;                   // SET1 4,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x10) good("Memory was correct after SET1 4,$10");
            else bad("Memory was not correct after SET1 4,$10");
            cpu.memory[0x80] = 0x6a;                   // SET1 5,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x20) good("Memory was correct after SET1 5,$10");
            else bad("Memory was not correct after SET1 5,$10");
            cpu.memory[0x80] = 0x72;                   // SET1 6,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x40) good("Memory was correct after SET1 6,$10");
            else bad("Memory was not correct after SET1 6,$10");
            cpu.memory[0x80] = 0x7a;                   // SET1 7,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x00;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x10] == 0x80) good("Memory was correct after SET1 7,$10");
            else bad("Memory was not correct after SET1 7,$10");
        }

        protected void shiftTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Shift Tests\r\n");
            output.AppendText("===========\r\n");
            cpu.Run();
            cpu.ClearMem();
            // ********** Right A **********
            cpu.memory[0x80] = 0x09;                   // SFTR A,1
            cpu.memory[Cpu.REG_A] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xc4) good("A was correct after SFTR A,1");
            else bad("A was not correct after SFTR A,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0x11;                   // SFTR A,2
            cpu.memory[Cpu.REG_A] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x10) good("A was correct after SFTR A,2");
            else bad("A was not correct after SFTR A,2");
            cpu.memory[0x80] = 0x19;                   // SFTR A,3
            cpu.memory[Cpu.REG_A] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xf3) good("A was correct after SFTR A,3");
            else bad("A was not correct after SFTR A,3");
            cpu.memory[0x80] = 0x01;                   // SFTR A,4
            cpu.memory[Cpu.REG_A] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x05) good("A was correct after SFTR A,4");
            else bad("A was not correct after SFTR A,4");
            // ********** Right B **********
            cpu.memory[0x80] = 0x29;                   // SFTR B,1
            cpu.memory[Cpu.REG_B] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0xc4) good("B was correct after SFTR B,1");
            else bad("B was not correct after SFTR B,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0x31;                   // SFTR B,2
            cpu.memory[Cpu.REG_B] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x10) good("B was correct after SFTR B,2");
            else bad("B was not correct after SFTR B,2");
            cpu.memory[0x80] = 0x39;                   // SFTR B,3
            cpu.memory[Cpu.REG_B] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0xf3) good("B was correct after SFTR B,3");
            else bad("B was not correct after SFTR B,3");
            cpu.memory[0x80] = 0x21;                   // SFTR B,4
            cpu.memory[Cpu.REG_B] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x05) good("B was correct after SFTR B,4");
            else bad("B was not correct after SFTR B,4");
            // ********** Left A **********
            cpu.memory[0x80] = 0x89;                   // SFTL A,1
            cpu.memory[Cpu.REG_A] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x10) good("A was correct after SFTL A,1");
            else bad("A was not correct after SFTL A,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0x91;                   // SFTL A,2
            cpu.memory[Cpu.REG_A] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x04) good("A was correct after SFTL A,2");
            else bad("A was not correct after SFTL A,2");
            cpu.memory[0x80] = 0x99;                   // SFTL A,3
            cpu.memory[Cpu.REG_A] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0xc0) good("A was correct after SFTL A,3");
            else bad("A was not correct after SFTL A,3");
            cpu.memory[0x80] = 0x81;                   // SFTL A,4
            cpu.memory[Cpu.REG_A] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x40) good("A was correct after SFTL A,4");
            else bad("A was not correct after SFTL A,4");
            // ********** Left B **********
            cpu.memory[0x80] = 0xa9;                   // SFTL B,1
            cpu.memory[Cpu.REG_B] = 0x88;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x10) good("B was correct after SFTL B,1");
            else bad("B was not correct after SFTL B,1");
            if (cpu.memory[Cpu.REG_P] == 0x81) good("P was correctly incremented by 1 after SHIFT");
            else bad("P was not correct after SHIFT");
            cpu.memory[0x80] = 0xb1;                   // SFTL B,2
            cpu.memory[Cpu.REG_B] = 0x41;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x04) good("B was correct after SFTL B,2");
            else bad("B was not correct after SFTL B,2");
            cpu.memory[0x80] = 0xb9;                   // SFTL B,3
            cpu.memory[Cpu.REG_B] = 0x98;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0xc0) good("B was correct after SFTL B,3");
            else bad("B was not correct after SFTL B,3");
            cpu.memory[0x80] = 0xa1;                   // SFTL B,4
            cpu.memory[Cpu.REG_B] = 0x54;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x40) good("B was correct after SFTL B,4");
            else bad("B was not correct after SFTL B,4");
        }

        protected void skipTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Skip Tests\r\n");
            output.AppendText("==========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x82;                   // SKP0 0,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xfe;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 0 when bit clear");
            else bad("P was not correct after SKP0 0 when bit clear");
            cpu.memory[0x80] = 0x82;                   // SKP0 0,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 0 when bit set");
            else bad("P was not correct after SKP0 0 when bit set");

            cpu.memory[0x80] = 0x8a;                   // SKP0 1,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xfd;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 1 when bit clear");
            else bad("P was not correct after SKP0 1 when bit clear");
            cpu.memory[0x80] = 0x8a;                   // SKP0 1,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 1 when bit set");
            else bad("P was not correct after SKP0 1 when bit set");

            cpu.memory[0x80] = 0x92;                   // SKP0 2,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xfb;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 2 when bit clear");
            else bad("P was not correct after SKP0 2 when bit clear");
            cpu.memory[0x80] = 0x92;                   // SKP0 2,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 2 when bit set");
            else bad("P was not correct after SKP0 2 when bit set");

            cpu.memory[0x80] = 0x9a;                   // SKP0 3,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xf7;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 3 when bit clear");
            else bad("P was not correct after SKP0 3 when bit clear");
            cpu.memory[0x80] = 0x9a;                   // SKP0 3,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 3 when bit set");
            else bad("P was not correct after SKP0 3 when bit set");

            cpu.memory[0x80] = 0xa2;                   // SKP0 4,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xef;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 4 when bit clear");
            else bad("P was not correct after SKP0 4 when bit clear");
            cpu.memory[0x80] = 0xa2;                   // SKP0 4,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 4 when bit set");
            else bad("P was not correct after SKP0 4 when bit set");

            cpu.memory[0x80] = 0xaa;                   // SKP0 5,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xdf;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 5 when bit clear");
            else bad("P was not correct after SKP0 5 when bit clear");
            cpu.memory[0x80] = 0xaa;                   // SKP0 5,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 5 when bit set");
            else bad("P was not correct after SKP0 5 when bit set");

            cpu.memory[0x80] = 0xb2;                   // SKP0 6,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xbf;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 6 when bit clear");
            else bad("P was not correct after SKP0 6 when bit clear");
            cpu.memory[0x80] = 0xb2;                   // SKP0 6,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 6 when bit set");
            else bad("P was not correct after SKP0 6 when bit set");

            cpu.memory[0x80] = 0xba;                   // SKP0 7,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x7f;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP0 7 when bit clear");
            else bad("P was not correct after SKP0 7 when bit clear");
            cpu.memory[0x80] = 0xba;                   // SKP0 7,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP0 7 when bit set");
            else bad("P was not correct after SKP0 7 when bit set");
            
            cpu.memory[0x80] = 0xc2;                   // SKP1 0,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 0 when bit set");
            else bad("P was not correct after SKP1 0 when bit clear");
            cpu.memory[0x80] = 0xc2;                   // SKP1 0,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xfe;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 0 when bit clear");
            else bad("P was not correct after SKP1 0 when bit set");

            cpu.memory[0x80] = 0xca;                   // SKP1 1,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 1 when bit set");
            else bad("P was not correct after SKP1 1 when bit clear");
            cpu.memory[0x80] = 0xca;                   // SKP1 1,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xfd;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 1 when bit clear");
            else bad("P was not correct after SKP1 1 when bit set");

            cpu.memory[0x80] = 0xd2;                   // SKP1 2,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 2 when bit set");
            else bad("P was not correct after SKP1 2 when bit clear");
            cpu.memory[0x80] = 0xd2;                   // SKP1 2,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xfb;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 2 when bit clear");
            else bad("P was not correct after SKP1 2 when bit set");

            cpu.memory[0x80] = 0xda;                   // SKP1 3,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 3 when bit set");
            else bad("P was not correct after SKP1 3 when bit clear");
            cpu.memory[0x80] = 0xda;                   // SKP1 3,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xf7;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 3 when bit clear");
            else bad("P was not correct after SKP1 3 when bit set");

            cpu.memory[0x80] = 0xe2;                   // SKP1 4,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 4 when bit set");
            else bad("P was not correct after SKP1 4 when bit clear");
            cpu.memory[0x80] = 0xe2;                   // SKP1 4,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xef;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 4 when bit clear");
            else bad("P was not correct after SKP1 4 when bit set");

            cpu.memory[0x80] = 0xea;                   // SKP1 5,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 5 when bit set");
            else bad("P was not correct after SKP1 5 when bit clear");
            cpu.memory[0x80] = 0xea;                   // SKP1 5,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xdf;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 5 when bit clear");
            else bad("P was not correct after SKP1 5 when bit set");

            cpu.memory[0x80] = 0xf2;                   // SKP1 6,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 6 when bit set");
            else bad("P was not correct after SKP1 6 when bit clear");
            cpu.memory[0x80] = 0xf2;                   // SKP1 6,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xbf;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 6 when bit clear");
            else bad("P was not correct after SKP1 6 when bit set");

            cpu.memory[0x80] = 0xfa;                   // SKP1 7,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0xff;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x84) good("P was correct after SKP1 7 when bit set");
            else bad("P was not correct after SKP1 7 when bit clear");
            cpu.memory[0x80] = 0xfa;                   // SKP1 7,$10
            cpu.memory[0x81] = 0x10;
            cpu.memory[0x10] = 0x7f;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correct after SKP1 7 when bit clear");
            else bad("P was not correct after SKP1 7 when bit set");
        }

        protected void storeATests()
        {
            output.AppendText("\r\n");
            output.AppendText("Store A Tests\r\n");
            output.AppendText("=============\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x1b;                   // STOREA C=$45
            cpu.memory[0x81] = 0x45;
            cpu.memory[Cpu.REG_A] = 0x78;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x81] == 0x78) good("Memory was correct after STORE A C=$45");
            else bad("Memory was not correct after STORE A C=$45");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE A");
            else bad("P was not correct after STORE A");
            cpu.memory[0x80] = 0x1c;                   // STOREA $46
            cpu.memory[0x81] = 0x46;
            cpu.memory[Cpu.REG_A] = 0x89;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x46] == 0x89) good("Memory was correct after STORE A $46");
            else bad("Memory was not correct after STORE A $46");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE A");
            else bad("P was not correct after STORE A");
            cpu.memory[0x80] = 0x1d;                   // STOREA ($47)
            cpu.memory[0x81] = 0x47;
            cpu.memory[0x47] = 0x36;
            cpu.memory[Cpu.REG_A] = 0x9a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x36] == 0x9a) good("Memory was correct after STORE A ($47)");
            else bad("Memory was not correct after STORE A ($47)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE A");
            else bad("P was not correct after STORE A");
            cpu.memory[0x80] = 0x1e;                   // STOREA $48,X
            cpu.memory[0x81] = 0x48;
            cpu.memory[Cpu.REG_A] = 0xab;
            cpu.memory[Cpu.REG_X] = 0x11;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x59] == 0xab) good("Memory was correct after STORE A $48,X");
            else bad("Memory was not correct after STORE A $48,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE A");
            else bad("P was not correct after STORE A");
            cpu.memory[0x80] = 0x1f;                   // STOREA ($49),X
            cpu.memory[0x81] = 0x49;
            cpu.memory[0x49] = 0xb2;
            cpu.memory[Cpu.REG_A] = 0xbc;
            cpu.memory[Cpu.REG_X] = 0x12;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0xc4] == 0xbc) good("Memory was correct after STORE A ($49),X");
            else bad("Memory was not correct after STORE A ($49),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE A");
            else bad("P was not correct after STORE A");
        }

        protected void storeBTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Store B Tests\r\n");
            output.AppendText("=============\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x5b;                   // STOREB C=$45
            cpu.memory[0x81] = 0x45;
            cpu.memory[Cpu.REG_B] = 0x78;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x81] == 0x78) good("Memory was correct after STORE B C=$45");
            else bad("Memory was not correct after STORE B C=$45");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE B");
            else bad("P was not correct after STORE B");
            cpu.memory[0x80] = 0x5c;                   // STOREB $46
            cpu.memory[0x81] = 0x46;
            cpu.memory[Cpu.REG_B] = 0xab;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x46] == 0xab) good("Memory was correct after STORE B $46");
            else bad("Memory was not correct after STORE B $46");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE B");
            else bad("P was not correct after STORE B");
            cpu.memory[0x80] = 0x5d;                   // STOREB ($47)
            cpu.memory[0x81] = 0x47;
            cpu.memory[0x47] = 0x36;
            cpu.memory[Cpu.REG_B] = 0xbc;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x36] == 0xbc) good("Memory was correct after STORE B ($47)");
            else bad("Memory was not correct after STORE B ($47)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE B");
            else bad("P was not correct after STORE B");
            cpu.memory[0x80] = 0x5e;                   // STOREB $48,X
            cpu.memory[0x81] = 0x48;
            cpu.memory[Cpu.REG_B] = 0xcd;
            cpu.memory[Cpu.REG_X] = 0x11;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x59] == 0xcd) good("Memory was correct after STORE B $48,X");
            else bad("Memory was not correct after STORE B $48,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE B");
            else bad("P was not correct after STORE B");
            cpu.memory[0x80] = 0x5f;                   // STOREB ($49),X
            cpu.memory[0x81] = 0x49;
            cpu.memory[0x49] = 0xb2;
            cpu.memory[Cpu.REG_B] = 0xde;
            cpu.memory[Cpu.REG_X] = 0x12;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0xc4] == 0xde) good("Memory was correct after STORE B ($49),X");
            else bad("Memory was not correct after STORE B ($49),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE B");
            else bad("P was not correct after STORE B");
        }

        protected void storeXTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Store X Tests\r\n");
            output.AppendText("=============\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x9b;                   // STOREX C=$45
            cpu.memory[0x81] = 0x45;
            cpu.memory[Cpu.REG_X] = 0xef;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x81] == 0xef) good("Memory was correct after STORE X C=$45");
            else bad("Memory was not correct after STORE X C=$45");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE X");
            else bad("P was not correct after STORE X");
            cpu.memory[0x80] = 0x9c;                   // STOREX $46
            cpu.memory[0x81] = 0x46;
            cpu.memory[Cpu.REG_X] = 0xf0;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x46] == 0xf0) good("Memory was correct after STORE X $46");
            else bad("Memory was not correct after STORE X $46");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE X");
            else bad("P was not correct after STORE X");
            cpu.memory[0x80] = 0x9d;                   // STOREX ($47)
            cpu.memory[0x81] = 0x47;
            cpu.memory[0x47] = 0x36;
            cpu.memory[Cpu.REG_X] = 0x01;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x36] == 0x01) good("Memory was correct after STORE X ($47)");
            else bad("Memory was not correct after STORE X ($47)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE X");
            else bad("P was not correct after STORE X");
            cpu.memory[0x80] = 0x9e;                   // STOREX $48,X
            cpu.memory[0x81] = 0x48;
            cpu.memory[Cpu.REG_X] = 0x12;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0x5a] == 0x12) good("Memory was correct after STORE X $48,X");
            else bad("Memory was not correct after STORE X $48,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE X");
            else bad("P was not correct after STORE X");
            cpu.memory[0x80] = 0x9f;                   // STOREX ($49),X
            cpu.memory[0x81] = 0x49;
            cpu.memory[0x49] = 0xb2;
            cpu.memory[Cpu.REG_X] = 0x23;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[0xd5] == 0x23) good("Memory was correct after STORE X ($49),X");
            else bad("Memory was not correct after STORE X ($49),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after STORE X");
            else bad("P was not correct after STORE X");
        }

        protected void subATests()
        {
            output.AppendText("\r\n");
            output.AppendText("Sub A Tests\r\n");
            output.AppendText("===========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x0b;                   // SUB C=$04
            cpu.memory[0x81] = 0x04;
            cpu.memory[Cpu.REG_A] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x06) good("A was correct after SUB C=$04");
            else bad("A was not correct after SUB C=$04");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x0c;                   // SUB $05
            cpu.memory[0x81] = 0x05;
            cpu.memory[0x05] = 0x02;
            cpu.memory[Cpu.REG_A] = 0x0c;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x0a) good("A was correct after SUB $05");
            else bad("A was not correct after SUB $05");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x0d;                   // SUB ($06)
            cpu.memory[0x81] = 0x06;
            cpu.memory[0x06] = 0xaa;
            cpu.memory[0xaa] = 0x34;
            cpu.memory[Cpu.REG_A] = 0x77;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x43) good("A was correct after SUB ($06)");
            else bad("A was not correct after SUB ($06)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x0e;                   // SUB $07,X
            cpu.memory[0x81] = 0x07;
            cpu.memory[0x0c] = 0x12;
            cpu.memory[Cpu.REG_A] = 0x99;
            cpu.memory[Cpu.REG_X] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x87) good("A was correct after SUB $07,X");
            else bad("A was not correct after SUB $07,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x0f;                   // SUB ($08),X
            cpu.memory[0x81] = 0x08;
            cpu.memory[0x08] = 0xc0;
            cpu.memory[0xc5] = 0x62;
            cpu.memory[Cpu.REG_A] = 0xcc;
            cpu.memory[Cpu.REG_X] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_A] == 0x6a) good("A was correct after SUB ($08),X");
            else bad("A was not correct after SUB ($08),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x0b;                   // SUB C=$04
            cpu.memory[0x81] = 0x04;
            cpu.memory[Cpu.REG_A] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x00) good("Carry was clear after 10-4");
            else bad("Carry was not clear after 10-4");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x00) good("Overflow was clear after 10-4");
            else bad("Overflow was not clear after 10-4");
            cpu.memory[0x80] = 0x0b;                   // SUB C=$0a
            cpu.memory[0x81] = 0x0a;
            cpu.memory[Cpu.REG_A] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x02) good("Carry was set after 4-10");
            else bad("Carry was not set after 4-10");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x00) good("Overflow was clear after 4-10");
            else bad("Overflow was not clear after 4-10");
            cpu.memory[0x80] = 0x0b;                   // SUB C=$01
            cpu.memory[0x81] = 0x01;
            cpu.memory[Cpu.REG_A] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x00) good("Carry was clear after -128 - 1");
            else bad("Carry was not clear after -128 - 1");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x01) good("Overflow was set after -128 - 1");
            else bad("Overflow was not set after -128 - 1");
            cpu.memory[0x80] = 0x0b;                   // SUB C=$ff
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_A] = 0x7f;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_AF] & 2) == 0x02) good("Carry was set after 127 - -1");
            else bad("Carry was not set after 127 - -1");
            if ((cpu.memory[Cpu.REG_AF] & 1) == 0x01) good("Overflow was set after 127 - -1");
            else bad("Overflow was not set after 127 - -1");
        }

        protected void subBTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Sub B Tests\r\n");
            output.AppendText("===========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x4b;                   // SUB C=$04
            cpu.memory[0x81] = 0x04;
            cpu.memory[Cpu.REG_B] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x06) good("B was correct after SUB C=$04");
            else bad("B was not correct after SUB C=$04");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x4c;                   // SUB $05
            cpu.memory[0x81] = 0x05;
            cpu.memory[0x05] = 0x02;
            cpu.memory[Cpu.REG_B] = 0x0c;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x0a) good("B was correct after SUB $05");
            else bad("B was not correct after SUB $05");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x4d;                   // SUB ($06)
            cpu.memory[0x81] = 0x06;
            cpu.memory[0x06] = 0xaa;
            cpu.memory[0xaa] = 0x34;
            cpu.memory[Cpu.REG_B] = 0x77;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x43) good("B was correct after SUB ($06)");
            else bad("B was not correct after SUB ($06)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x4e;                   // SUB $07,X
            cpu.memory[0x81] = 0x07;
            cpu.memory[0x0c] = 0x12;
            cpu.memory[Cpu.REG_B] = 0x99;
            cpu.memory[Cpu.REG_X] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x87) good("B was correct after SUB $07,X");
            else bad("B was not correct after SUB $07,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x4f;                   // SUB ($08),X
            cpu.memory[0x81] = 0x08;
            cpu.memory[0x08] = 0xc0;
            cpu.memory[0xc5] = 0x62;
            cpu.memory[Cpu.REG_B] = 0xcc;
            cpu.memory[Cpu.REG_X] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_B] == 0x6a) good("B was correct after SUB ($08),X");
            else bad("B was not correct after SUB ($08),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x4b;                   // SUB C=$04
            cpu.memory[0x81] = 0x04;
            cpu.memory[Cpu.REG_B] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x00) good("Carry was clear after 10-4");
            else bad("Carry was not clear after 10-4");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x00) good("Overflow was clear after 10-4");
            else bad("Overflow was not clear after 10-4");
            cpu.memory[0x80] = 0x4b;                   // SUB C=$0a
            cpu.memory[0x81] = 0x0a;
            cpu.memory[Cpu.REG_B] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x02) good("Carry was set after 4-10");
            else bad("Carry was not set after 4-10");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x00) good("Overflow was clear after 4-10");
            else bad("Overflow was not clear after 4-10");
            cpu.memory[0x80] = 0x4b;                   // SUB C=$01
            cpu.memory[0x81] = 0x01;
            cpu.memory[Cpu.REG_B] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x00) good("Carry was clear after -128 - 1");
            else bad("Carry was not clear after -128 - 1");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x01) good("Overflow was set after -128 - 1");
            else bad("Overflow was not set after -128 - 1");
            cpu.memory[0x80] = 0x4b;                   // SUB C=$ff
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_B] = 0x7f;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_BF] & 2) == 0x02) good("Carry was set after 127 - -1");
            else bad("Carry was not set after 127 - -1");
            if ((cpu.memory[Cpu.REG_BF] & 1) == 0x01) good("Overflow was set after 127 - -1");
            else bad("Overflow was not set after 127 - -1");
        }

        protected void subXTests()
        {
            output.AppendText("\r\n");
            output.AppendText("Sub X Tests\r\n");
            output.AppendText("===========\r\n");
            cpu.Run();
            cpu.ClearMem();
            cpu.memory[0x80] = 0x8b;                   // SUB C=$04
            cpu.memory[0x81] = 0x04;
            cpu.memory[Cpu.REG_X] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x06) good("X was correct after SUB C=$04");
            else bad("X was not correct after SUB C=$04");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x8c;                   // SUB $05
            cpu.memory[0x81] = 0x05;
            cpu.memory[0x05] = 0x02;
            cpu.memory[Cpu.REG_X] = 0x0c;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x0a) good("X was correct after SUB $05");
            else bad("X was not correct after SUB $05");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x8d;                   // SUB ($06)
            cpu.memory[0x81] = 0x06;
            cpu.memory[0x06] = 0xaa;
            cpu.memory[0xaa] = 0x34;
            cpu.memory[Cpu.REG_X] = 0x77;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x43) good("X was correct after SUB ($06)");
            else bad("X was not correct after SUB ($06)");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x8e;                   // SUB $07,X
            cpu.memory[0x81] = 0x07;
            cpu.memory[0x0c] = 0x02;
            cpu.memory[Cpu.REG_X] = 0x99;
            cpu.memory[Cpu.REG_X] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x03) good("X was correct after SUB $07,X");
            else bad("X was not correct after SUB $07,X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x8f;                   // SUB ($08),X
            cpu.memory[0x81] = 0x08;
            cpu.memory[0x08] = 0xc0;
            cpu.memory[0xc5] = 0x01;
            cpu.memory[Cpu.REG_X] = 0x05;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if (cpu.memory[Cpu.REG_X] == 0x04) good("X was correct after SUB ($08),X");
            else bad("X was not correct after SUB ($08),X");
            if (cpu.memory[Cpu.REG_P] == 0x82) good("P was correctly incremented by 2 after SUB");
            else bad("P was not correct after SUB");
            cpu.memory[0x80] = 0x8b;                   // SUB C=$04
            cpu.memory[0x81] = 0x04;
            cpu.memory[Cpu.REG_X] = 0x0a;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x00) good("Carry was clear after 10-4");
            else bad("Carry was not clear after 10-4");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x00) good("Overflow was clear after 10-4");
            else bad("Overflow was not clear after 10-4");
            cpu.memory[0x80] = 0x8b;                   // SUB C=$0a
            cpu.memory[0x81] = 0x0a;
            cpu.memory[Cpu.REG_X] = 0x04;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x02) good("Carry was set after 4-10");
            else bad("Carry was not set after 4-10");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x00) good("Overflow was clear after 4-10");
            else bad("Overflow was not clear after 4-10");
            cpu.memory[0x80] = 0x8b;                   // SUB C=$01
            cpu.memory[0x81] = 0x01;
            cpu.memory[Cpu.REG_X] = 0x80;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x00) good("Carry was clear after -128 - 1");
            else bad("Carry was not clear after -128 - 1");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x01) good("Overflow was set after -128 - 1");
            else bad("Overflow was not set after -128 - 1");
            cpu.memory[0x80] = 0x8b;                   // SUB C=$ff
            cpu.memory[0x81] = 0xff;
            cpu.memory[Cpu.REG_X] = 0x7f;
            cpu.memory[Cpu.REG_P] = 0x80;
            cpu.cycle();
            if ((cpu.memory[Cpu.REG_XF] & 2) == 0x02) good("Carry was set after 127 - -1");
            else bad("Carry was not set after 127 - -1");
            if ((cpu.memory[Cpu.REG_XF] & 1) == 0x01) good("Overflow was set after 127 - -1");
            else bad("Overflow was not set after 127 - -1");
        }

        public void Run()
        {
            goodCount = 0;
            badCount = 0;
            output.Clear();
            addATests();
            addBTests();
            addXTests();
            andTests();
            haltTests();
            jumpTests();
            lnegTests();
            loadATests();
            loadBTests();
            loadXTests();
            nopTests();
            orTests();
            rotTests();
            setTests();
            shiftTests();
            skipTests();
            storeATests();
            storeBTests();
            storeXTests();
            subATests();
            subBTests();
            subXTests();
            output.AppendText("\r\n");
            output.AppendText("Good Tests: " + goodCount.ToString() + "\r\n");
            output.AppendText("Bad Tests : " + badCount.ToString() + "\r\n");
        }
    }
}
