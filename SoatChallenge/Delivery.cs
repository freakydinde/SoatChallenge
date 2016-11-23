namespace SoatChallenge
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>Class representing one delivery process</summary>
    public class Delivery
    {
        private static int autonomyRatio;
        private static int maxDistance;

        /// <summary>Initializes a new instance of the <see cref="Delivery"/> class.</summary>
        /// <param name="deliveryGrid">delivery grid</param>
        /// <param name="deliveryDrones">drones collection</param>
        /// <param name="deliveryRounds">Number of round</param>
        /// <param name="deliveryPacketsNumber">Number of packets</param>
        /// <param name="deliveryMaxDistance">Maximun number of move for a drone</param>
        /// <param name="dronesMaxPackets">maximum numbers of packets a drone can hold</param>
        /// <param name="ratioAutonomy">dynamic number representing autonomy ratio between grid size</param>
        public Delivery(Grid deliveryGrid, IEnumerable<Drone> deliveryDrones, int deliveryRounds, int deliveryPacketsNumber, int deliveryMaxDistance, int dronesMaxPackets, int ratioAutonomy)
        {
            this.Drones = deliveryDrones;
            this.Grid = deliveryGrid;
            this.MaxRound = deliveryRounds;

            Delivery.autonomyRatio = ratioAutonomy;
            Delivery.maxDistance = deliveryMaxDistance;

            Drone.MaxPackets = dronesMaxPackets;

            this.Round = 0;
            this.PacketsNumber = deliveryPacketsNumber;

            Write.Trace($"new Delivery : {this}");
        }

        /// <summary>Gets delivery drones collection</summary>
        public IEnumerable<Drone> Drones { get; private set; }

        /// <summary>Gets drones moves as list of string line, starting by number of packet taken</summary>
        public IEnumerable<string> DronesMoves
        {
            get
            {
                return from i in this.Drones select i.MoveString;
            }
        }

        /// <summary>Gets delivery Grid</summary>
        public Grid Grid { get; private set; }

        /// <summary>Gets maximum rounds number</summary>
        public int MaxRound { get; private set; }

        /// <summary>Gets number of packets to deliver</summary>
        public int PacketsNumber { get; private set; }

        /// <summary>Gets drones with pending state</summary>
        public IEnumerable<Drone> PendingDrones
        {
            get
            {
                return this.Drones.Where(x => x.CurrentState == Drone.State.Pending);
            }
        }

        /// <summary>Gets current round number</summary>
        public int Round { get; private set; }

        /// <summary>Gets delivery routes collection</summary>
        public IEnumerable<Route> Routes
        {
            get
            {
                return from i in this.Drones where i.Route != null select i.Route;
            }
        }

        /// <summary>Get the maximun distance allowed with a packet number</summary>
        /// <param name="packetsNumber">packet number to test</param>
        /// <returns>maximun distance as int</returns>
        public static int Autonomy(int packetsNumber)
        {
            return Delivery.maxDistance - (packetsNumber * Delivery.autonomyRatio);
        }

        /// <summary>Create a <see cref="Delivery"/> object from an input text file</summary>
        /// <param name="inputFilePath">input file full path</param>
        /// <param name="dronesMaxPackets">maximum numbers of packets a drone can hold</param>
        /// <param name="autonomyRatio">dynamic number representing autonomy ratio between grid size</param>
        /// <returns>Instance of a <see cref="Delivery"/> object</returns>
        public static Delivery CreateDelivery(string inputFilePath, int dronesMaxPackets, int autonomyRatio)
        {
            // get input file lines as int arrays into a IEnumerable
            IEnumerable<int[]> inputs = from i in File.ReadAllLines(inputFilePath) select (from j in i.Split(' ') select Convert.ToInt32(j, CultureInfo.InvariantCulture)).ToArray();

            int gridRows = inputs.ElementAt(0)[0];
            int gridColumns = inputs.ElementAt(0)[1];

            int packetsNumber = inputs.ElementAt(1)[0];
            int pendingDrones = inputs.ElementAt(1)[1];
            int maxDistance = inputs.ElementAt(1)[2];
            int roundsNumber = inputs.ElementAt(1)[3];

            Cell startCell = new Cell(inputs.ElementAt(2)[0], inputs.ElementAt(2)[1]);
            List<Packet> packets = (from i in inputs.Skip(3).Take(inputs.Count() - 3) select new Packet(i.ElementAt(0), i.ElementAt(1))).OrderBy(x => x.Row).ThenBy(x => x.Column).ToList();

            Grid grid = new Grid(gridRows, gridColumns, packets, startCell);

            // create drones
            List<Drone> drones = new List<Drone>();

            for (int i = 0; i < pendingDrones; i++)
            {
                drones.Add(new Drone(i, grid));
            }

            return new Delivery(grid, drones.Distinct(), roundsNumber, packetsNumber, maxDistance, dronesMaxPackets, autonomyRatio);
        }

        /// <summary>try to set a route to all drones</summary>
        /// <return>Cells which failed to map a route</return>
        public IEnumerable<Cell> MapRoutes()
        {
            // following route type will be tried in that order
            List<Route.Specs> routesSpecs = new List<Route.Specs>()
            {
                Route.Specs.Free,
                Route.Specs.All | Route.Specs.Alternative,
                Route.Specs.Free | Route.Specs.Wait,
                Route.Specs.All | Route.Specs.Alternative | Route.Specs.Wait,
                Route.Specs.Free | Route.Specs.Opposite,
                Route.Specs.All | Route.Specs.Alternative | Route.Specs.Opposite,
                Route.Specs.Free | Route.Specs.Opposite | Route.Specs.Wait,
                Route.Specs.All | Route.Specs.Alternative | Route.Specs.Opposite | Route.Specs.Wait
            };

            return MapRouteWhile(routesSpecs, true).Distinct();
        }

        /// <summary>Print drones route to a text file</summary>
        /// <param name="outputFile">output file full path</param>
        public void Print(string outputFile)
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            File.WriteAllLines(outputFile, from i in this.Drones select i.MoveString);
        }

        /// <summary>reset delivery</summary>
        public void Reset()
        {
            foreach (Drone drone in this.Drones)
            {
                drone.Reset();
            }

            this.Round = 0;
        }

        /// <summary>Gets delivery score from estimation</summary>
        /// <returns>score as int</returns>
        public int Score()
        {
            IEnumerable<Drone> drones = from i in this.Drones where i.Route != null select i;

            List<Route> routes = (from i in drones select i.Route).ToList();

            int movesNumber = (from i in routes select i.MovesCount).Sum();

            int score = this.Grid.DeliveredPacketsNumber * ((this.MaxRound * drones.Count()) - movesNumber);

            if (this.Grid.DeliveredPacketsNumber == this.Grid.Packets.Count())
            {
                score += this.Grid.Packets.Count() * 10;

                Write.Print($"score:{score} = deliveredPacketNumber:{this.Grid.DeliveredPacketsNumber} * ((maxRound:{this.MaxRound} * dronesCount:{drones.Count()}) - movesCount:{movesNumber}) + bonus:Packets*10:{this.Grid.Packets.Count() * 10}");
            }
            else
            {
                Write.Print($"score:{score} = deliveredPacketNumber:{this.Grid.DeliveredPacketsNumber} * ((maxRound:{this.MaxRound} * dronesCount:{drones.Count()}) - movesCount:{movesNumber})");
            }

            return score;
        }

        /// <summary>start delivery process</summary>
        public void Start()
        {
            foreach (Drone drone in this.Drones)
            {
                drone.Start();
            }

            Write.Trace($"total round : {this.MaxRound}");

            while (this.Round < this.MaxRound)
            {
                foreach (Drone drone in this.Drones)
                {
                    drone.NextMove();
                }

                this.Round++;

                Write.Trace($"current round : {this.Round}");
            }

            // if a drone has moved until last roud, it may stick to shipping state
            foreach (Drone drone in from i in this.Drones where i.CurrentState == Drone.State.Shipping select i)
            {
                drone.CurrentState = Drone.State.Stopped;
            }
        }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"PacketsNumber:{this.PacketsNumber} DronesNumber:{this.Drones.Count()} Round:{this.Round} MaxRound:{this.MaxRound} autonomyRatio:{Delivery.autonomyRatio} maxDistance:{Delivery.maxDistance} Drone.MaxPackets:{Drone.MaxPackets} Grid:({this.Grid})");
        }

        private List<Cell> MapRouteWhile(List<Route.Specs> routesSpecs, bool bubble)
        {
            List<Cell> missingCells = new List<Cell>();

            while (this.Grid.PendingPacketsNumber > 0 && this.PendingDrones.Count() > 0)
            {
                Path path = new Path(this.Grid.StartCell, this.Grid);

                Route route = null;

                if (bubble == true)
                {
                    route = path.MapBubbleRoute(routesSpecs);
                }
                else
                {
                    route = path.MapRoute(routesSpecs);
                }

                if (route != null)
                {
                    this.PendingDrones.FirstOrDefault()?.SetRoute(route);

                    Write.Print($"route mapped : {this.Routes.Count()}, packet assigned : {this.Grid.AssignedPacketsNumber} packets left : {this.Grid.PendingPacketsNumber} missing cells : {missingCells.Count()}, drone left : {this.PendingDrones.Count()}");
                }
                else
                {
                    Cell missingCell = path.ClosestPendingPath(this.Grid.StartCell).ReachCell;
                    missingCells.Add(missingCell);

                    this.Grid.SetPacketState(missingCell, Packet.State.Missing);

                    Write.Print($"missing cells : {missingCell} failed to get any route from : {string.Join(" or ", routesSpecs)}");
                }
            }

            return missingCells;
        }
    }
}