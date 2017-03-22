using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Timers;
using System.Threading;

namespace BLEMeshSimulation
{
    public enum State { Gateway, Connected, NotConnected }
    public enum Mode { Broadcasting, Receiving, Sleeping }
    class Node
    {
        static Random RandomGenerator = new Random();

#region 模擬軟體用 - 硬體本身提供
        public (float X, float Y) Location { get; private set; }
        public State State { get; private set; }
        public Mode  Mode { get; private set; }
        public PhysicalAddress Address { get; private set; }
        private Simulator Simulator;
#endregion
#region 模擬軟體用 - 硬體本身不提供
        public PhysicalAddress ParentAddress { get; private set; }
        public UInt16 MetricDistance;
#endregion

        public Node(Simulator simulator, (float X, float Y) location, bool isGateway)
        {
            this.Simulator = simulator;
            this.Location = location;
            this.State = isGateway ? State.Gateway : State.NotConnected;
            this.Mode = Mode.Sleeping;

            var MACAddressBytes = new byte[6];            
            Node.RandomGenerator.NextBytes(MACAddressBytes);
            this.Address = new PhysicalAddress(MACAddressBytes);
            this.ParentAddress = null;
            this.MetricDistance = isGateway ? (ushort)0 : (ushort)65535;
        }

        private Task broadcastingTask()
        {
            return new Task(() =>
            {
                var packet = new byte[6 + 2];
                var address = this.Address.GetAddressBytes();
                var metricDistance = BitConverter.GetBytes(this.MetricDistance);

                address.CopyTo(packet, 0);
                metricDistance.CopyTo(packet, 6);

                this.Simulator.Broadcast(this, packet);
            }, token);
        }
        public Task Start(CancellationToken token)
        {
            var broadcastInterval = 400 + Node.RandomGenerator.Next() % 251;
            var scaningInterval = 400 + Node.RandomGenerator.Next() % 451;
            return new Task(async () =>
            {
                // 推播
                this.Mode = Mode.Broadcasting;
                var broadcastBegin = DateTime.Now;
                while (!token.IsCancellationRequested && DateTime.Now.Subtract(broadcastBegin).Milliseconds < broadcastInterval) {
                    await this.broadcastingTask();
                    await Task.Delay(60);
                }

                // 搜尋
                var scanBegin = DateTime.Now;
                while (!token.IsCancellationRequested && DateTime.Now.Subtract(scanBegin).Milliseconds < scaningInterval)
                {
                    this.Mode = Mode.Receiving;
                    await Task.Delay(40);
                    this.Mode = Mode.Sleeping;
                    await Task.Delay(60);
                }
            }, token);
        }
        public void Receive(byte[] message, int rssi)
        {
            var sender = new PhysicalAddress(message.Take(6).ToArray());
            var metric = BitConverter.ToUInt16(message.Skip(6).Take(2).ToArray(), 0);

            
        }
    }
}
