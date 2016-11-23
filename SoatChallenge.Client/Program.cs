namespace SoatChallenge.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>Entry point for soat challenge client</summary>
    internal class Program
    {
        private const int AutonomyRatio = 40;
        private const int DronesMaxPacket = 4;

        private static void Main()
        {
            // create delivery
            Delivery delivery = Delivery.CreateDelivery(Inputs.ChallengeInput, DronesMaxPacket, AutonomyRatio);

            // process delivery
            delivery.MapRoutes();
            delivery.Start();

            // log delivery
            string filePath = Path.Combine(Environment.CurrentDirectory, delivery.Score() + ".txt");
            File.WriteAllLines(filePath, delivery.DronesMoves);

            Write.Print($"press l for more loggin, any other key to exit");

            ConsoleKeyInfo key = Console.ReadKey();

            if (key.KeyChar == 'l')
            {
                MoreLoggin(delivery);
            }
        }

        private static void MoreLoggin(Delivery delivery)
        {
            Console.WriteLine();

            IEnumerable<Route> routes = delivery.Routes;
            Write.Print($"route count : {routes.Count()}, average distance : {routes.Average(x => x.Distance)}, average packet count : {routes.Average(x => x.PacketsCount)}");
            File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "routes.txt"), from i in routes select i.ToString());

            List<Packet> packets = routes.SelectMany(x => x.Packets).ToList();
            Write.Print($"route packets count : {packets.Count()}, average distance : {packets.Average(x => x.Distance)}");
            File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "packets.txt"), from i in packets select i.ToString());

            IEnumerable<Cell> doublons = from i in packets group i by new Cell(i.Row, i.Column) into grp where grp.Count() > 1 select grp.Key;
            Write.Print($"doublons : {doublons.Count()}");
            if (doublons.Count() > 0)
            {
                File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "doublons.txt"), from i in doublons select i.ToString());
            }

            List<string> shippingErrors = new List<string>();

            foreach (Route route in routes)
            {
                IEnumerable<RouteCell> routeCells = from i in route.Cells.Where(x => x.Direction != Drone.Direction.Stay) where packets.Where(x => x.Row == i.Row && x.Column == i.Column).Count() > 0 select i;

                foreach (RouteCell cell in routeCells)
                {
                    int cellDistance = route.CellDistance(cell);
                    Packet packet = packets.Where(x => x.Row == cell.Row && x.Column == cell.Column).First();

                    if (cellDistance < packet.Distance)
                    {
                        shippingErrors.Add($"cell : {cell}({cellDistance}); packet : {packet}");
                    }
                }
            }

            Write.Print($"shipping errors : {shippingErrors.Count()}");
            if (shippingErrors.Count() > 0)
            {
                File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "shippingErrors.txt"), shippingErrors);
            }

            Write.Print($"press any key to exit");

            Console.ReadKey();
        }
    }
}