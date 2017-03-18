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


namespace WindowsFormsApplication8
{
    public partial class Form1 : Form
    {
        Graphics area;
        double[][] metric = new double[8][] { new double[8] { 0, 0,0,0,0,0,0,0 }, new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 }, new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 }, new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 }, new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 }, new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 }, new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 }, new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 } };
        
        int[][] rssi = new int[8][] { new int[8] {-100,-12,-25,-22,-16,-20,-8,-18 },
                                         new int[8] {-12,-100,-20,-18,-15,-15,-6,-10 },
                                         new int[8] {-25,-20,-100,-5,-20,-8,-22,-15 },
                                         new int[8] {-22,-18,-5,-100,-20,-4,-22,-16},
                                         new int[8] {-16,-15,-20,-20,-100,-10,-16,-2},
                                         new int[8] {-20,-15,-8,-4,-10,-100,-16,-1},
                                         new int[8] {-8,-6,-22,-22,-16,-16,-100,-20},
                                         new int[8] {-18,-10,-15,-16,-2,-2,-20,-100 }};
        int[][] point = new int[8][] { new int[2] {20,150 },
                                       new int[2] {75,75 },
                                       new int[2] {365,150 },
                                       new int[2] {300,125 },
                                       new int[2] {125,268 },
                                       new int[2] {240,150 },
                                       new int[2] {25,50 },
                                       new int[2] {150,200 }};
        Thread p1,p2,p3,p4,p5,p6,p7,p8;
        private object locker = new Object();
        public Form1()
        {
            InitializeComponent();
            area = pictureBox1.CreateGraphics();
        }

        public void DoWork(object i)
        {
            Pen black = new Pen(Color.Black);
            black.Width = 8.0F;
            SolidBrush red = new SolidBrush(Color.Red);
            SolidBrush blue = new SolidBrush(Color.Blue);
            int num = (int)i;
            lock (locker)
            {
                if (num > 5)
                {
                    area.FillEllipse(red, point[num][0], point[num][1], 30, 30);
                    Thread.Sleep(1000);
                }
                else
                {
                    area.FillEllipse(blue, point[num][0], point[num][1], 30, 30);
                    Thread.Sleep(1000);
                }
            }
            double min = 100;
            int y = 0;
            double metric_dis = 0;
            for(int j = 0; j < 8 ; j++)
            {
                metric[num][j] = (rssi[num][j] - 20) * -0.68 ;
                if (metric[num][j] < min)
                {
                    min = metric[num][j];
                    y = j;
                }
            }
            lock (locker)
            {
                area.DrawLine(black, point[num][0] + 15, point[num][1] + 15, point[y][0] + 15, point[y][1] + 15);
                Thread.Sleep(1000);
                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread p1 = new Thread(() => DoWork(0));
            Thread p2 = new Thread(() => DoWork(1));
            Thread p3 = new Thread(() => DoWork(2));
            Thread p4 = new Thread(() => DoWork(3));
            Thread p5 = new Thread(() => DoWork(4));
            Thread p6 = new Thread(() => DoWork(5));
            Thread p7 = new Thread(() => DoWork(6));
            Thread p8 = new Thread(() => DoWork(7));
            p1.Start();
            p2.Start();
            p3.Start();
            p4.Start();
            p5.Start();
            p6.Start();
            p7.Start();
            p8.Start();
        }

        private void DoWork()
        {
            throw new NotImplementedException();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (p1.IsAlive || p2.IsAlive || p3.IsAlive || p4.IsAlive || p5.IsAlive || p6.IsAlive || p7.IsAlive || p8.IsAlive)
            {
                p1.Abort();
                p2.Abort();
                p3.Abort();
                p4.Abort();
                p5.Abort();
                p6.Abort();
                p7.Abort();
                p8.Abort();
            }
        }

    }
}
