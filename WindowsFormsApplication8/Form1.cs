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
namespace WindowsFormsApplication8
{
    public partial class Form1 : Form
    {
        Graphics area;
        double[][] metric = new double[8][] { new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 }, new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 }, new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 }, new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 }, new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 }, new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 }, new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 }, new double[8] { 100, 100, 100, 100, 100, 100, 100, 100 } };

        int[][] rssi = new int[8][] { new int[8] {-100,-12,-25,-22,-16,-20,-8,-18 },
                                         new int[8] {-12,-100,-20,-18,-15,-15,-6,-10 },
                                         new int[8] {-25,-20,-100,-5,-20,-8,-22,-15 },
                                         new int[8] {-22,-18,-5,-100,-20,-4,-22,-16},
                                         new int[8] {-16,-15,-20,-20,-100,-10,-16,-2},
                                         new int[8] {-20,-15,-8,-4,-10,-100,-16,-1},
                                         new int[8] {-8,-6,-22,-22,-16,-16,-100,-20},
                                         new int[8] {-18,-10,-15,-16,-2,-2,-20,-100 }};
        int[][] point = new int[8][] { new int[2] {125,150 },
                                       new int[2] {75,75 },
                                       new int[2] {270,200 },//360,150
                                       new int[2] {300,125 },
                                       new int[2] {125,268 },
                                       new int[2] {240,150 },
                                       new int[2] {25,50 },
                                       new int[2] {175,200 }};
        int[] flag = new int[7];
        Thread p1, p2, p3, p4, p5, p6, p7, p8;
        double metric_dis = 0;
        private object locker = new Object();
        public Form1()
        {
            InitializeComponent();
            area = pictureBox1.CreateGraphics();
        }

        public void DoWork(object i)
        {
            Pen black = new Pen(Color.Black);
            Pen white = new Pen(Color.White);
            Pen green = new Pen(Color.Green);
            black.Width = 8.0F;
            white.Width = 8.0F;
            Pen r = new Pen(Color.Red);
            r.Width = 8.0F;
            SolidBrush red = new SolidBrush(Color.Red);
            SolidBrush blue = new SolidBrush(Color.Blue);
            int num = (int)i;
            lock (locker)
            {
                if (num > 6)
                {
                    area.FillEllipse(red, point[num][0], point[num][1], 30, 30);
                    area.DrawEllipse(green, point[num][0] - 35, point[num][1] - 35, 100, 100);
                    Thread.Sleep(100);
                }
                else
                {
                    area.FillEllipse(blue, point[num][0], point[num][1], 30, 30);
                    area.DrawEllipse(green, point[num][0] - 35, point[num][1] - 35, 100, 100);
                    Thread.Sleep(100);
                }
            }
            lock (locker)//gateway
            {
                double d1;
                for (int m = 0; m < 6; m++)
                {
                    d1 = Math.Pow(Math.Pow((point[7][0] - point[m][0]), 2) + Math.Pow((point[7][1] - point[m][1]), 2), 0.5);
                    if (d1 < 100)
                    {
                        metric[7][m] = (rssi[7][m] - 20) * -0.68;
                        metric[m][7] = metric[7][m];
                        flag[m] = 1;
                        area.DrawLine(black, point[7][0] + 15, point[7][1] + 15, point[m][0] + 15, point[m][1] + 15);
                    }
                    
                }
            }
            lock (locker)
            {
                double d2;
                int y = 0;
                double tmp = 100;
                for (int m = 0; m < 7; m++)
                {
                    for (int n = 0; n < 7; n++)
                    {
                        
                            d2 = Math.Pow(Math.Pow((point[m][0] - point[n][0]), 2) + Math.Pow((point[m][1] - point[n][1]), 2), 0.5);
                            if (d2 < 100)
                            {
                            
                                if (flag[m] != 1 || flag[n] != 1)
                                {
                                    metric[m][n] = (rssi[m][n] - 20) * -0.68 + metric[n][7];
                                    area.DrawLine(black, point[m][0] + 15, point[m][1] + 15, point[n][0] + 15, point[n][1] + 15);
                                    
                                                       
                                }
                            }
                    }
                }
                
                    
                
                
                //double min = 100;
                /*int y = 0;
                for (int j = 0; j < 6; j++)
                {
                    if (metric[num][j] < min)
                    {
                        min = metric[num][j];
                        y = j;
                    }
                }
                lock (locker)
                {
                    //area.DrawLine(black, point[num][0] + 15, point[num][1] + 15, point[y][0] + 15, point[y][1] + 15);
                    Thread.Sleep(1000);
                }*/
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
    public enum State
    {
        Receving, Sending
    }
    public partial class node
    {
        public State state = State.Sending;
        System.Timers.Timer receivingTimer;
        System.Timers.Timer broadcastTimer;

        public void start()
        {
            DateTime start = new DateTime();
            broadcastTimer = new System.Timers.Timer(40);
            broadcastTimer.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                // 如果在時間內
                // 呼叫 Broadcast
                // 時間外，開第二個 timer
            };
            broadcastTimer.AutoReset = true;
            receivingTimer = new System.Timers.Timer(1000);
            receivingTimer.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                // 把第一個 Timer 打開
            };
            // 開始第一個 Timer
        }
    }
}





/*lock (locker)
{
    double d;
    for (int m = 0; m < 6; m++)
    {
        d = Math.Pow(Math.Pow((point[num][0] - point[m][0]), 2) + Math.Pow((point[num][1] - point[m][1]), 2), 0.5);
        if (d < 100)
        {
            metric[6][m] = (rssi[6][m] - 20) * -0.68;
            metric[m][6] = metric[6][m];
            // area.DrawLine(black, point[num][0] + 15, point[num][1] + 15, point[m][0] + 15, point[m][1] + 15);
        }
    }
}
lock (locker)
{
    double d;
    for (int m = 0; m < 6; m++)
    {
        d = Math.Pow(Math.Pow((point[num][0] - point[m][0]), 2) + Math.Pow((point[num][1] - point[m][1]), 2), 0.5);
        if (d < 100)
        {
            if(num < 6)
            {
                metric[num][m] = (rssi[num][m] - 20) * -0.68 + metric[m][6];
                metric[num][6] = metric[num][m];
            }
           // area.DrawLine(black, point[num][0] + 15, point[num][1] + 15, point[m][0] + 15, point[m][1] + 15);
        }
    }
}*/

/* lock (locker)
 {
     for (int k = 0; k < 6; k++)
     {
         for (int m = k+1; m < 6; m++)
         {
             double d;
             d = Math.Pow(Math.Pow((point[k][0] - point[m][0]), 2) + Math.Pow((point[k][1] - point[m][1]), 2), 0.5);
             if (d < 100)
             {
                 metric[k][m] = (rssi[k][m] - 20) * -0.68 + metric[m][6] + metric[m][7];
             }
         }
         double min = 100;
         int y = 0;

         for(int j = 0; j < 6 ; j++)
         {
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
 }*/
