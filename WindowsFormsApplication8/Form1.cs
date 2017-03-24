using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
namespace BLEMeshSimulation
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource TokenSouce;              
        private Simulator Simulator;
        private Logger SharedLogger;

        private Boolean SimulationStarted;

        public Form1()
        {
            InitializeComponent();
            var context = pictureBox1.CreateGraphics();
            this.Simulator = new Simulator(context);
            this.SharedLogger = new Logger(this.textBox1);

            this.SimulationStarted = false;
            this.button1.Text = "開始模擬";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            switch (this.SimulationStarted)
            {
                case true:
                    this.TokenSouce.Cancel();

                    this.SimulationStarted = false;
                    this.button1.Text = "開始模擬";
                    break;
                case false:
                    TokenSouce = new CancellationTokenSource();
                    this.Simulator.Start(this.TokenSouce.Token);

                    this.SimulationStarted = true;
                    this.button1.Text = "停止模擬";
                    break;
            }                                             
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs mouseEvent) {
                this.Simulator.AddNode((mouseEvent.X, mouseEvent.Y), false);
            }
        }
        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (e is MouseEventArgs mouseEvent)
            {
                this.Simulator.RemoveNodeAt((mouseEvent.X, mouseEvent.Y));
                this.Simulator.AddNode((mouseEvent.X, mouseEvent.Y), true);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.TokenSouce.Cancel();
        }
    }
    class Logger
    {
        public static Logger Instance;

        TextBox destination;
        public Logger(TextBox textBox)
        {
            Logger.Instance = this;
            this.destination = textBox;
        }

        public void LogLine(string data)
        {
            if (this.destination.InvokeRequired)
            {
                this.destination.BeginInvoke(new Action(() => {
                    this.destination.AppendText($"{data}\r\n");
                }));
            }
        }
    }
}
