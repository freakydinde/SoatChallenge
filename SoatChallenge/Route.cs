namespace SoatChallenge
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>cells collection to a reach cells</summary>
    public class Route
    {
        private List<RouteCell> cells;
        private List<Packet> packets;

        /// <summary>Initializes a new instance of the <see cref="Route"/> class.</summary>
        /// <param name="startCell">route start cell</param>
        public Route(Cell startCell)
        {
            this.StartCell = startCell;
            this.ReachCell = startCell;

            this.cells = new List<RouteCell>();
            this.packets = new List<Packet>();

            this.Distance = 0;
            this.MaxPackets = Drone.MaxPacketsToReachDistance(this.Distance);

            Write.Trace($"new Route {this}");
        }

        /// <summary>Sets route specifications, from 3 groups - Route | Free | All - Alternative | Opposite - Wait</summary>
        [Flags]
        public enum Specs
        {
            /// <summary>try to use route (delimited by column and row containing packets), try to catch packets (not assigned), never use free (group 1)</summary>
            Route = 1,

            /// <summary>try to go off routes, never take packets (group 1)</summary>
            Free = 2,

            /// <summary>use all cells to trace routes, never take packets (group 1)</summary>
            All = 4,

            /// <summary>always select second choice when multiple path are available (default is choice 1) (group 2)</summary>
            Alternative = 8,

            /// <summary>use opposite horizontal side (which is a longer path) (group 2)</summary>
            Opposite = 16,

            /// <summary>add a sleep round (at start) to delay cell passage (so you can wait the initial shipper deliver its packets) (group 3)</summary>
            Wait = 32
        }

        /// <summary>Gets a collection representing route cells</summary>
        public IEnumerable<RouteCell> Cells
        {
            get
            {
                return this.cells.Distinct();
            }
        }

        /// <summary>Gets the distance to reach destination packet</summary>
        public int Distance { get; private set; }

        /// <summary>Gets maximun packets a drone can hold to reach distance</summary>
        public int MaxPackets { get; private set; }

        /// <summary>Gets number of move</summary>
        public int MovesCount
        {
            get
            {
                return this.cells.Count();
            }
        }

        /// <summary>Gets the route's packets</summary>
        public IEnumerable<Packet> Packets
        {
            get
            {
                return this.packets.Distinct();
            }
        }

        /// <summary>Gets number of packets</summary>
        public int PacketsCount
        {
            get
            {
                return this.packets.Count();
            }
        }

        /// <summary>Gets or sets the route reach cell</summary>
        public Cell ReachCell { get; set; }

        /// <summary>Gets the route start cell</summary>
        public Cell StartCell { get; private set; }

        /// <summary>add a new routeCell to this route</summary>
        /// <param name="routeCell">route cell to add</param>
        /// <param name="grid">grid containing routeCells</param>
        public void AddCell(RouteCell routeCell, Grid grid)
        {
            if (routeCell != null && grid != null)
            {
                if (routeCell.Direction == Drone.Direction.Stay)
                {
                    this.cells.Insert(0, routeCell);

                    foreach (Packet routePacket in this.packets)
                    {
                        routePacket.Distance++;
                    }
                }
                else
                {
                    this.cells.Add(routeCell);
                    this.ReachCell = new Cell(routeCell.Row, routeCell.Column);

                    Packet packet = grid.GetPacket(routeCell);

                    // in bubbling mode, packet from previous route on startcell of this route should be Assigned so won't be add to this route packets
                    if (packet != null && packet.CurrentState == Packet.State.Pending)
                    {
                        packet.CurrentState = Packet.State.Willing;
                        packet.Distance = this.CellDistance(routeCell);

                        this.packets.Add(packet);
                    }
                }

                this.Distance++;
                this.MaxPackets = Drone.MaxPacketsToReachDistance(this.Distance);
            }
        }

        /// <summary>Merge a route with this route</summary>
        /// <param name="route">route to merge</param>
        public void AddRoute(Route route)
        {
            if (route != null)
            {
                Write.Trace($"merging route : {route}");

                foreach (Packet packet in route.packets)
                {
                    packet.Distance += this.Distance;
                }

                IEnumerable<RouteCell> stayCells = route.cells.Where(x => x.Direction == Drone.Direction.Stay);

                if (stayCells.Any())
                {
                    this.cells.AddRange(route.Cells.Except(stayCells));
                    this.cells.InsertRange(0, stayCells);

                    foreach (Packet packet in this.packets)
                    {
                        packet.Distance += stayCells.Count();
                    }
                }
                else
                {
                    this.cells.AddRange(route.Cells);
                }

                this.ReachCell = route.ReachCell;

                this.Distance += route.Distance;

                this.packets.AddRange(route.packets);

                this.MaxPackets = Drone.MaxPacketsToReachDistance(this.Distance);

                Write.Trace($"route merged : {this}");
            }
        }

        /// <summary>add a wait move to route</summary>
        public void AddWait()
        {
            Write.Trace($"adding a wait cell");

            this.cells.Insert(0, RouteCell.WaitCell(this.StartCell));

            foreach (Packet packet in this.Packets)
            {
                packet.Distance++;
            }

            Write.Trace($"{this}");
        }

        /// <summary>Set willing packets state to assigned</summary>
        public void AssignWilling()
        {
            Write.Trace($"Assign route packets : ", this.packets);

            foreach (Packet packet in this.packets.Where(x => x.CurrentState == Packet.State.Willing))
            {
                packet.CurrentState = Packet.State.Assigned;
            }
        }

        /// <summary>Gets route cell distance</summary>
        /// <param name="routeCell">input cell</param>
        /// <returns>cell distance as int</returns>
        public int CellDistance(RouteCell routeCell)
        {
            return this.cells.IndexOf(routeCell) + 1;
        }

        /// <summary>remove last cell of route cells list</summary>
        /// <param name="grid">grid containing cell</param>
        public void RemoveLastCell(Grid grid)
        {
            RouteCell cell = this.cells.Last();

            Write.Trace($"Removing cell : {cell}");

            if (grid.ResetPacket(cell))
            {
                this.packets.Remove(this.packets.Last());
            }

            this.cells.Remove(cell);

            cell = this.cells.Last();

            this.ReachCell = new Cell(cell.Row, cell.Column);
            this.Distance--;
            this.MaxPackets = Drone.MaxPacketsToReachDistance(this.Distance);
        }

        /// <summary>Set all route packets state back to pending</summary>
        /// <returns>always return null</returns>
        public Route Reset()
        {
            Write.Trace($"Reset route packets : ", this.packets);

            foreach (Packet packet in this.packets)
            {
                packet.CurrentState = Packet.State.Pending;
                packet.Distance = 0;
            }

            return null;
        }

        /// <summary>Set willing packets state back to pending</summary>
        /// <returns>always return null</returns>
        public Route ResetWilling()
        {
            Write.Trace($"Reset route willing packets : ", this.packets);

            foreach (Packet packet in this.packets.Where(x => x.CurrentState == Packet.State.Willing))
            {
                packet.CurrentState = Packet.State.Pending;
                packet.Distance = 0;
            }

            return null;
        }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"StartCell:{this.StartCell} ReachCell:{this.ReachCell} Cells:{Write.Collection(this.Cells)} Packets:{Write.Collection(this.Packets)} MaxPackets:{this.MaxPackets} Distance:{this.Distance}");
        }
    }
}