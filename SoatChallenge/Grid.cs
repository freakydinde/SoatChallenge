namespace SoatChallenge
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Type representing a world grid</summary>
    public class Grid
    {
        private List<Packet> packets;

        /// <summary>Initializes a new instance of the <see cref="Grid"/> class.</summary>
        /// <param name="rows">how many grid rows</param>
        /// <param name="columns">how many grid columns</param>
        /// <param name="gridPackets">Cells containing a packets</param>
        /// <param name="startCell">start position on the grid</param>
        public Grid(int rows, int columns, IEnumerable<Packet> gridPackets, Cell startCell)
        {
            if (rows > 0 && columns > 0)
            {
                this.Columns = columns - 1;
                this.Rows = rows - 1;
            }

            this.StartCell = startCell;

            this.packets = gridPackets.ToList();
        }

        /// <summary>Gets the number of grid packets with assigned state</summary>
        public int AssignedPacketsNumber
        {
            get
            {
                return (from i in this.packets where i.CurrentState == Packet.State.Assigned select i).Count();
            }
        }

        /// <summary>Gets grid's maximum columns</summary>
        public int Columns { get; private set; }

        /// <summary>Gets the number of grid packets with delivered state</summary>
        public int DeliveredPacketsNumber
        {
            get
            {
                return (from i in this.packets where i.CurrentState == Packet.State.Delivered select i).Count();
            }
        }

        /// <summary>Gets grid packets</summary>
        public IEnumerable<Packet> Packets
        {
            get
            {
                return this.packets.Distinct();
            }
        }

        /// <summary>Gets grid packets with pending state</summary>
        public IEnumerable<Packet> PendingPackets
        {
            get
            {
                return this.Packets.Where(x => x.CurrentState == Packet.State.Pending);
            }
        }

        /// <summary>Gets the number of packets pending</summary>
        public int PendingPacketsNumber
        {
            get
            {
                return (from i in this.packets where i.CurrentState == Packet.State.Pending select i).Count();
            }
        }

        /// <summary>Gets grid's maximum rows</summary>
        public int Rows { get; private set; }

        /// <summary>Gets Grid start Cell</summary>
        public Cell StartCell { get; private set; }

        /// <summary>Gets grid packets with willing state</summary>
        public IEnumerable<Packet> WillingPackets
        {
            get
            {
                return this.Packets.Where(x => x.CurrentState == Packet.State.Willing);
            }
        }

        /// <summary>set packet state</summary>
        /// <param name="packet">input packet</param>
        /// <param name="distance">distance to addt</param>
        /// <returns>bool indicating whether operation was possible (will not if packet is not a packet)</returns>
        public bool AddToPacketDistance(ICell packet, int distance)
        {
            IEnumerable<Packet> gridPackets = this.packets.Where(x => x.Row == packet.Row && x.Column == packet.Column);

            if (gridPackets.Any())
            {
                gridPackets.FirstOrDefault().Distance += distance;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Gets packet corresponding to packet</summary>
        /// <param name="packet">input packet</param>
        /// <returns>null if packet is not a packet</returns>
        public Packet GetPacket(ICell packet)
        {
            IEnumerable<Packet> gridPackets = this.packets.Where(x => x.Row == packet.Row && x.Column == packet.Column);

            if (gridPackets.Any())
            {
                return gridPackets.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>Gets a bool indicating if cell is out of route and packet</summary>
        /// <param name="cell">input cell</param>
        /// <returns>bool indicating whether this cell is free</returns>
        public bool IsFree(ICell cell)
        {
            if (!this.IsPacket(cell) && !this.IsRoute(cell))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Gets a bool indicating if a packet exists on that row and column</summary>
        /// <param name="cell">input cell</param>
        /// <returns>bool indicating whether this cell is a packet</returns>
        public bool IsPacket(ICell cell)
        {
            return this.packets.Where(x => x.Row == cell.Row && x.Column == cell.Column).Any();
        }

        /// <summary>Gets a bool indicating if that row and column belong to a route</summary>
        /// <param name="cell">input cell</param>
        /// <returns>bool indicating whether this cell belong to a route</returns>
        public bool IsRoute(ICell cell)
        {
            return this.packets.Where(x => x.Row == cell.Row || x.Column == cell.Column).Any();
        }

        /// <summary>Gets a bool indicating if that row and column belong to a route</summary>
        /// <param name="cell">input cell</param>
        /// <returns>bool indicating whether this cell belong to a route</returns>
        public bool IsStartRoute(ICell cell)
        {
            return cell.Row == this.StartCell.Row || cell.Column == this.StartCell.Column;
        }

        /// <summary>Reset packet to pending state</summary>
        /// <param name="packet">input packet</param>
        /// <returns>false if input packet is not a real packet</returns>
        public bool ResetPacket(ICell packet)
        {
            IEnumerable<Packet> gridPackets = this.packets.Where(x => x.Row == packet.Row && x.Column == packet.Column);

            if (gridPackets.Any())
            {
                gridPackets.First().CurrentState = Packet.State.Pending;
                gridPackets.First().Distance = 0;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Reset packet to pending state</summary>
        public void ResetMissingPackets()
        {
            foreach (Packet packet in this.packets.Where(x => x.CurrentState == Packet.State.Missing))
            {
                packet.CurrentState = Packet.State.Pending;
            }
        }

        /// <summary>set packet state</summary>
        /// <param name="packet">input packet</param>
        /// <param name="distance">distance to set</param>
        /// <returns>bool indicating whether operation was possible (will not if packet is not a packet)</returns>
        public bool SetPacketDistance(ICell packet, int distance)
        {
            IEnumerable<Packet> gridPackets = this.packets.Where(x => x.Row == packet.Row && x.Column == packet.Column);

            if (gridPackets.Any())
            {
                gridPackets.FirstOrDefault().Distance = distance;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>set packet state</summary>
        /// <param name="packet">input packet</param>
        /// <param name="state">state to set</param>
        /// <returns>bool indicating whether operation was possible (will not if packet is not a packet)</returns>
        public bool SetPacketState(ICell packet, Packet.State state)
        {
            IEnumerable<Packet> gridPackets = this.packets.Where(x => x.Row == packet.Row && x.Column == packet.Column);

            if (gridPackets.Any())
            {
                gridPackets.FirstOrDefault().CurrentState = state;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"Rows:{this.Rows} Columns:{this.Columns} StartCell:{this.StartCell} PacketNumber:{this.packets.Count()}");
        }

        /// <summary>Gets a boolean indicating whether distance allow to pass the cell without break delivery</summary>
        /// <param name="packet">input packet</param>
        /// <param name="distance">input distance</param>
        /// <returns>bool indicating whether packet is an available cell</returns>
        public bool WillBreakDelivery(ICell packet, int distance)
        {
            Packet testPacket = this.GetPacket(packet);

            if (testPacket != null)
            {
                return testPacket.Distance >= distance;
            }
            else
            {
                return false;
            }
        }
    }
}