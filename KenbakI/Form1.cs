using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace KenbakI
{
    public partial class Form1 : Form
    {
        Diagnostics diagnostics;
        Cpu computer;
        protected byte lastDataLamps;
        protected Boolean allowStep;
        protected Assembler assembler;

        public Form1()
        {
            Font = new Font(Font.Name, 8.25f * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            InitializeComponent();
            diagnostics = new Diagnostics(DebugOutput);
            computer = new Cpu();
            assembler = new Assembler();
            DataLamp7.Image = images30x30.Images[0];
            DataLamp6.Image = images30x30.Images[0];
            DataLamp5.Image = images30x30.Images[0];
            DataLamp4.Image = images30x30.Images[0];
            DataLamp3.Image = images30x30.Images[0];
            DataLamp2.Image = images30x30.Images[0];
            DataLamp1.Image = images30x30.Images[0];
            DataLamp0.Image = images30x30.Images[0];
            InputLamp.Image = images30x30.Images[2];
            AddressLamp.Image = images30x30.Images[2];
            MemoryLamp.Image = images30x30.Images[2];
            RunningLamp.Image = images30x30.Images[2];
            lastDataLamps = 0;
            allowStep = true;
        }

        private void DiagnosticsButton_Click(object sender, EventArgs e)
        {
            diagnostics.Run();
        }

        private void DataButton_Click(object sender, EventArgs e)
        {
            int tag;
            tag = Convert.ToInt32(((Button)sender).Tag);
            computer.memory[0xff] |= (byte)tag;
            computer.lampMode = Cpu.LAMPS_INPUT;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            computer.memory[0xff] = 0x00;
            computer.lampMode = Cpu.LAMPS_INPUT;
        }

        private void SystemTimer_Tick(object sender, EventArgs e)
        {
            byte value;
            value = 0;
            if (!SingleStep.Checked || allowStep) computer.cycle();
            if (SingleStep.Checked) allowStep = false;
            if (computer.debugMode)
            {
                DebugOutput.AppendText(computer.debug);
                computer.debug = "";
                RegA.Text = computer.memory[0x00].ToString("X2");
                RegB.Text = computer.memory[0x01].ToString("X2");
                RegX.Text = computer.memory[0x02].ToString("X2");
                RegP.Text = computer.memory[0x03].ToString("X2");
                RegAF.Text = computer.memory[0x81].ToString("X2");
                RegBF.Text = computer.memory[0x82].ToString("X2");
                RegXF.Text = computer.memory[0x83].ToString("X2");
            }
            if (computer.running) value = computer.memory[0x80];
            else switch (computer.lampMode)
            {
                case Cpu.LAMPS_INPUT: value = computer.memory[0xff]; break;
                case Cpu.LAMPS_ADDRESS: value = computer.addressRegister; break;
                case Cpu.LAMPS_MEMORY: value = computer.memoryValue; break;
                default: value = computer.memory[0x80]; break;
            }
            if (value != lastDataLamps)
            {
                DataLamp7.Image = ((value & 0x80) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                DataLamp6.Image = ((value & 0x40) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                DataLamp5.Image = ((value & 0x20) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                DataLamp4.Image = ((value & 0x10) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                DataLamp3.Image = ((value & 0x08) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                DataLamp2.Image = ((value & 0x04) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                DataLamp1.Image = ((value & 0x02) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                DataLamp0.Image = ((value & 0x01) != 0) ? images30x30.Images[1] : images30x30.Images[0];
                lastDataLamps = value;
            }
            InputLamp.Image = (computer.lampMode == Cpu.LAMPS_INPUT) ? images30x30.Images[3] : images30x30.Images[2];
            AddressLamp.Image = (computer.lampMode == Cpu.LAMPS_ADDRESS) ? images30x30.Images[3] : images30x30.Images[2];
            MemoryLamp.Image = (computer.lampMode == Cpu.LAMPS_MEMORY) ? images30x30.Images[3] : images30x30.Images[2];
            RunningLamp.Image = (computer.running) ? images30x30.Images[3] : images30x30.Images[2];
        }

        private void AddressSetButton_Click(object sender, EventArgs e)
        {
            if (computer.running) return;
            computer.addressRegister = computer.memory[0xff];
        }

        private void AddressDisplayButton_Click(object sender, EventArgs e)
        {
            if (computer.running) return;
            computer.lampMode = Cpu.LAMPS_ADDRESS;
        }

        private void MemoryReadButton_Click(object sender, EventArgs e)
        {
            if (computer.running) return;
            computer.lampMode = Cpu.LAMPS_MEMORY;
            computer.memoryValue = computer.memory[computer.addressRegister];
            computer.addressRegister++;
        }

        private void MemoryStoreButton_Click(object sender, EventArgs e)
        {
            if (computer.running) return;
            computer.memory[computer.addressRegister] = computer.memory[0xff];
            computer.addressRegister++;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            computer.running = true;
            computer.lampMode = Cpu.LAMPS_RUN;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            computer.running = false;
        }

        private void DebugMode_CheckedChanged(object sender, EventArgs e)
        {
            computer.debugMode = DebugMode.Checked;
        }

        private void DebugClearButton_Click(object sender, EventArgs e)
        {
            DebugOutput.Clear();
        }

        private void SingleStep_CheckedChanged(object sender, EventArgs e)
        {
            if (SingleStep.Checked) allowStep = false;
        }

        private void StepButton_Click(object sender, EventArgs e)
        {
            allowStep = true;
        }

        private void DataButton_MouseDown(object sender, MouseEventArgs e)
        {
            int tag;
            tag = Convert.ToInt32(((Button)sender).Tag);
            computer.memory[0xff] |= (byte)tag;
            computer.lampMode = Cpu.LAMPS_INPUT;
        }

        private void DataButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (computer.running) computer.lampMode = Cpu.LAMPS_RUN;
        }

        private void AssembleButton_Click(object sender, EventArgs e)
        {
            AssemblerResults.Text = assembler.Assemble(AssemblerSource.Lines, computer.memory);
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            AssemblerSource.Clear();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            StreamWriter file;
            saveFileDialog.Filter = "Assembly Files (*.asm)|*.asm|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = new StreamWriter(saveFileDialog.FileName);
                foreach (var line in AssemblerSource.Lines) file.WriteLine(line);
                file.Close();
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            String line;
            StreamReader file;
            openFileDialog.Filter = "Assembly Files (*.asm)|*.asm|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = new StreamReader(openFileDialog.FileName);
                AssemblerSource.Clear();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    AssemblerSource.AppendText(line + "\r\n");
                }
                file.Close();
            }

        }
    }
}
