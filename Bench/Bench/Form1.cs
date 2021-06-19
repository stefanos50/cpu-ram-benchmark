using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bench
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        PerformanceCounter pRAM;
        PerformanceCounter pCPU;
        int k=100;
        int running = 0;
        Thread t1;
        Thread[] cpuLoadThread;
        string multitime;
        int count = 0;
        string GetComponent(string class1,string s)
        {
            ManagementObjectSearcher MOS = new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM "+class1);
            foreach(ManagementObject mj in MOS.Get())
            {
                return Convert.ToString(mj[s]);
            }
            return "Uknown";
        }
        public Form1()
        {
            InitializeComponent();
            label10.Text = GetComponent("Win32_Processor", "Name");
            label11.Text = GetComponent("Win32_VideoController", "Name");
            label12.Text = GetComponent("Win32_BaseBoard", "Product");
            label13.Text = GetComponent("Win32_PhysicalMemory", "PartNumber");
            label16.Text= "("+GetComponent("Win32_Processor", "ThreadCount")+" CPUs)"+"~"+(float.Parse(GetComponent("Win32_Processor", "CurrentClockSpeed"))/1000).ToString()+"GHz"; 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             pCPU= new PerformanceCounter("Processor", "% Processor Time", "_Total");
             pRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            timer1.Start();

            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(k>0)
                k = k - 1;
            else
                running = 1;
            if (running == 1)
            {
                running = 0;
                label18.Text = "Not Running";
                label18.ForeColor = Color.OliveDrab;
            }
            float cpu = pCPU.NextValue();
            float ram = pRAM.NextValue();
            progressBar1.Value = (int)cpu;
            progressBar2.Value = (int)ram;
            label3.Text = string.Format("{0:0,00}%", cpu);
            label4.Text = string.Format("{0:0.00}%", ram);
            chart1.Series["CPU"].Points.AddY(cpu);
            chart1.Series["RAM"].Points.AddY(ram);
        }
        void testCPU()
        {
            long _x = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < int.MaxValue; i++)
               _x += i;
            stopwatch.Stop();
            running = 1;
            MessageBox.Show(stopwatch.ElapsedMilliseconds.ToString(),"Result");
            t1.Abort();
        }
        void testRAM()
        {
            ArrayList array1 = new ArrayList();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i <100000000; i++)
            {
                try
                {
                    array1.Add(i);
                }
                catch (System.OutOfMemoryException)
                {
                    break;
                }
            }
            running = 1;
            stopwatch.Stop();
            MessageBox.Show(stopwatch.ElapsedMilliseconds.ToString(),"Result");
            t1.Abort();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label18.Text = "Running";
            label18.ForeColor = Color.Red;
            t1 = new Thread(() => testCPU());
            t1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label18.Text = "Running";
            label18.ForeColor = Color.Red;
            t1 = new Thread(testRAM);
            t1.Start();
        }
        void Multithread()
        {
            Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            long _y = 0;
            for (int i = 0; i < int.MaxValue; i++)
                _y += i;
            stopwatch.Stop();
            count++;
            multitime += Thread.CurrentThread.Name + stopwatch.ElapsedMilliseconds.ToString()+Environment.NewLine;
            if (count == Environment.ProcessorCount)
            {
                running = 1;
                MessageBox.Show(multitime, "Result");
            }
            
        }
        void Multithread2()
        {

            long _y = 0;
            for (int i = 0; i < int.MaxValue; i++)
                _y += i;
            count++;
            if((count == Environment.ProcessorCount) && k != 0)
            {
                stresstest();
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            label18.Text = "Running";
            label18.ForeColor = Color.Red;
            count = 0;
            multitime = " ";
           cpuLoadThread = new Thread[Environment.ProcessorCount];
             for (int i = 0; i < cpuLoadThread.Length; i++)
             {
                   cpuLoadThread[i] = new Thread(new ThreadStart(Multithread));
                   cpuLoadThread[i].Name = "CPU[" + i.ToString() + "]";
                   cpuLoadThread[i].IsBackground = true;
                   cpuLoadThread[i].Start();
             }
            
        }
        void stresstest()
        {
            count = 0;
            cpuLoadThread = new Thread[Environment.ProcessorCount];

                for (int i = 0; i < cpuLoadThread.Length; i++)
                {
                    cpuLoadThread[i] = new Thread(new ThreadStart(Multithread2));
                    cpuLoadThread[i].Name = "CPU[" + i.ToString() + "]";
                    cpuLoadThread[i].IsBackground = true;
                    cpuLoadThread[i].Start();
                }
            
        }
        private void button4_Click(object sender, EventArgs e)
        {
            k = (int)numericUpDown1.Value;
            if (k != 0)
            {
                label18.Text = "Running";
                label18.ForeColor = Color.Red;
            }
            if (k!=0)
                stresstest();
            if (k == 0)
            {
                k = 100;
            }
        }
    }
}
