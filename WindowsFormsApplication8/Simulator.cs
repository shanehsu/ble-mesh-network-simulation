using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace BLEMeshSimulation
{
    class SimulationConfig
    {
        public Color BackgroundColor { get; private set; }
        public Color GatewayNodeColor { get; private set; }
        public Color ConnectedNodeColor { get; private set; }
        public Color NotConnectedNodeColor { get; private set; }

        public SimulationConfig()
        {
            this.BackgroundColor = SystemColors.Control;
            this.GatewayNodeColor = Color.Green;
            this.ConnectedNodeColor = Color.Olive;
            this.NotConnectedNodeColor = Color.Red;
        }
    }
    class Simulator
    {
        private Graphics Context;
        private SimulationConfig Config;
        private List<Node> Nodes;
        public Simulator(Graphics context) : this(context, new SimulationConfig()) { }
        public Simulator(Graphics context, SimulationConfig config)
        {
            this.Context = context;
            this.Config = config;
            this.Nodes = new List<Node>();
        }
        public void Clear() { this.Context.Clear(SystemColors.Control); }
        public void DrawNode(Node node)
        {
            var (x, y) = node.Location;
            var color = node.State == State.Gateway ? this.Config.GatewayNodeColor :
                node.State == State.Connected ? this.Config.ConnectedNodeColor :
                this.Config.NotConnectedNodeColor;

            this.Context.DrawEllipse(new Pen(color), x - 15.0f, y - 15.0f, 30.0f, 30.0f);
            this.Context.FillEllipse(new SolidBrush(color), x - 15.0f, y - 15.0f, 30.0f, 30.0f);
        }
        public void DrawLineBetween(Node lhs, Node rhs)
        {
            var pen = Pens.Aqua;
            this.Context.DrawLine(pen, lhs.Location.X, lhs.Location.Y, rhs.Location.X, rhs.Location.Y);
        }
        public void Update()
        {
            this.Clear();
            
            foreach (var child in this.Nodes)
            {
                foreach (var parent in this.Nodes)
                {
                    if (child.ParentAddress != null && child.ParentAddress.Equals(parent.Address))
                    {
                        this.DrawLineBetween(child, parent);
                    }
                }
            }
            foreach (var Node in this.Nodes)
            {
                this.DrawNode(Node);
            }
        }

        public void AddNode((float x, float y) location, bool isGateway)
        {
            this.Nodes.Add(new Node(this, location, isGateway));
            this.Update();
        }

        public void RemoveNodeAt((float x, float y) location)
        {
            foreach (var node in this.Nodes)
            {
                var dx = location.x - node.Location.X;
                var dy = location.y - node.Location.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance < 15.0)
                {
                    this.Nodes.Remove(node);
                    break;
                }
            }

            this.Update();
        }

        public void Broadcast(Node sender, byte[] message)
        {
            var modeString = "";
            foreach (var node in this.Nodes)
            {
                if (modeString.Length > 0)
                {
                    modeString += ", ";
                }
                modeString += $"{node.State}:{node.Mode}";
            }
            Logger.Instance.LogLine($"推播函數執行：目前有 {this.Nodes.Count} 個節點，模式：{modeString}");
            foreach (var node in this.Nodes)
            {
                var dx = sender.Location.X - node.Location.X;
                var dy = sender.Location.Y - node.Location.Y;

                if (node.Mode == Mode.Receiving && Math.Sqrt(dx * dx + dy * dy) < 175.0)
                {
                    int rssi = (int)(Math.Sqrt(dx * dx + dy * dy) / -0.68 + 20); 
                    node.Receive(message, rssi);
                }
            }
        }

        public void Start(CancellationToken token)
        {
            // 另外每 20 毫秒重新繪圖
            new Task(async () =>
            {        
                while (!token.IsCancellationRequested)
                {
                    this.Update();
                    await Task.Delay(100);
                }
            }, token).Start();
            foreach (var node in this.Nodes)
            {
                node.Start(token).Start();
            }
        }
    }
}
