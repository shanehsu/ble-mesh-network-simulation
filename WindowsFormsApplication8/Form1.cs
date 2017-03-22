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
        private object locker = new Object();
        private Simulator Simulator;
        
        public Form1()
        {
            InitializeComponent();
            var context = pictureBox1.CreateGraphics();
            this.Simulator = new Simulator(context);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Simulator.Start();
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
            
        }
    }
}
