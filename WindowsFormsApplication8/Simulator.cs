using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

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
            var pen = Pens.LightGray;
            this.Context.DrawLine(pen, lhs.Location.X, lhs.Location.Y, rhs.Location.X, rhs.Location.Y);
        }
        public void Update()
        {
            this.Clear();
            
            foreach (var child in this.Nodes)
            {
                foreach (var parent in this.Nodes)
                {
                    if (child.ParentAddress == parent.Address)
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

        }

        public void Start()
        {

        }
    }
}
