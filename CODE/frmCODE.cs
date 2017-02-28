using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.IO;
//using System.Collections;

namespace CODE
{
    public partial class frmCODE : Form
    {
        private const int MAX_TASK = 3;
        private BackgroundWorker bgWorker1;
        private bool[] abMyThreadStop = new bool[MAX_TASK];
        private Thread[] aMyThread = new Thread[MAX_TASK];
        private ulong g_uGlobalTickCnt = 0;
        private uint g_uTryCnt0 = 0;
        private uint g_uTryCnt1 = 0;

        //private Stopwatch watch = new Stopwatch();

        private void bgWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //this.Text = "Using Task Parallel & Background Worker";
                this.Text = g_uGlobalTickCnt.ToString();

                if (g_uGlobalTickCnt % 2 == 0)
                    this.button1.BackColor = Color.Red;
                else
                    this.button1.BackColor = Color.Blue;

                this.label1.Text = g_uGlobalTickCnt.ToString();
                this.button1.Text = string.Format("{0} vs {1}", g_uTryCnt0, g_uTryCnt1);
            }
            finally
            {
            }
        }

        private void ThMyThread0()
        {
            long subb = 0;
            while (abMyThreadStop[0] == false)
            {
                for (int i = 0; i < 10000; i++)
                    for (int j = 0; j < 10000; j++)
                        subb = i * j;

                Thread.Sleep(100);
                g_uTryCnt0++;
            }
        }

        private void ThMyThread1()
        {
            long subb = 0;
            while (abMyThreadStop[1] == false)
            {
                for (int i = 0; i < 1000; i++)
                    for (int j = 0; j < 1000; j++ )
                        subb = i*j;

                Thread.Sleep(100);
                g_uTryCnt1++;
            }
        }

        private void ThMyThread2()
        {
            while (abMyThreadStop[2] == false)
            {
                g_uGlobalTickCnt++;
                Thread.Sleep(100);
            }
        }

        private void MyTask0()
        {
            this.aMyThread[0] = new Thread(new ThreadStart(this.ThMyThread0));
            abMyThreadStop[0] = false;
            this.aMyThread[0].Start();
        }

        private void MyTask1()
        {
            this.aMyThread[1] = new Thread(new ThreadStart(this.ThMyThread1));
            abMyThreadStop[1] = false;
            this.aMyThread[1].Start();
        }

        private void MyTask2()
        {
            MainTimer.Enabled = true;

            this.aMyThread[2] = new Thread(new ThreadStart(this.ThMyThread2));
            abMyThreadStop[2] = false;
            this.aMyThread[2].Start();            
        }

        public frmCODE()
        {
            InitializeComponent();

            // Task Parallel Method
            Parallel.For(0, MAX_TASK, i =>
            {
                var idx = i;

                switch (idx)
                {
                    case 0:
                        Task.Factory.StartNew(new Action(MyTask0));
                        break;
                    case 1:
                        Task.Factory.StartNew(new Action(MyTask1));
                        break;
                    case 2:
                        Task.Factory.StartNew(new Action(MyTask2));
                        break;
                }
            }); // Parallel.For
        }
 
        private void frmCODE_FormClosing(object sender, FormClosingEventArgs e)
        {
            //watch.Reset();
            MainTimer.Enabled = false;

            bgWorker1.Dispose();
            for (int i = 0; i < MAX_TASK; i++)
            {
                abMyThreadStop[i] = true;
            }
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            this.bgWorker1.RunWorkerAsync();
        }
    }
}
