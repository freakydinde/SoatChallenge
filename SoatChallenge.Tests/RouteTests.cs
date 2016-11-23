namespace SoatChallenge.Tests
{
    using System.Linq;
    using System.Text;
    using Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoatChallenge;

    /// <summary>Test Path class</summary>
    [TestClass]
    public class RouteTests
    {
        [TestMethod]
        public void AddRemoveCell()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 3, 10);
            Path path = new Path(new Cell(8, 12), delivery.Grid);
            Route route = path.MapRoute(Route.Specs.Route);

            route.AddCell(new RouteCell(8, 13, Drone.Direction.Right), delivery.Grid);

            StringBuilder actual = new StringBuilder();
            actual.AppendLine(route?.ToString() ?? "null");

            StringBuilder expected = new StringBuilder();
            expected.AppendLine("StartCell:R4C16 ReachCell:R8C13 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R8C16(Down);R8C15(Left);R8C14(Left);R8C13(Left);R8C12(Left);R8C13(Right) Packets:R8C12(8) MaxPackets:3 Distance:9");
            route.RemoveLastCell(delivery.Grid);

            actual.AppendLine(route?.ToString() ?? "null");
            expected.AppendLine("StartCell:R4C16 ReachCell:R8C12 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R8C16(Down);R8C15(Left);R8C14(Left);R8C13(Left);R8C12(Left) Packets:R8C12(8) MaxPackets:3 Distance:8");

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void AddRoute()
        {
            StringBuilder actual = new StringBuilder();

            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 3, 10);
            Path path = new Path(new Cell(8, 12), delivery.Grid);

            Route currentRoute = path.MapRoute(Route.Specs.Route);

            Cell cell = new Cell(14, 17);
            Packet gridPacket = delivery.Grid.GetPacket(cell);

            actual.Append(delivery.Grid.GetPacket(cell).CurrentState.ToString() + ";");
            actual.Append(delivery.Grid.GetPacket(cell).Distance + "°");

            Path newRoutePath = new Path(cell, delivery.Grid, currentRoute.ReachCell);
            Route newRoute = newRoutePath.MapRoute(Route.Specs.Route);

            actual.Append(gridPacket.CurrentState.ToString() + ";");
            actual.Append(gridPacket.Distance + ";");

            currentRoute.AddRoute(newRoute);

            actual.Append(gridPacket.CurrentState.ToString() + ";");
            actual.Append(gridPacket.Distance + ";");
            actual.Append(currentRoute?.ToString() ?? "null");

            Write.Trace($"{actual.ToString()}");

            string expected = "Pending;0°Assigned;11;Assigned;19;StartCell:R4C16 ReachCell:R14C17 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R8C16(Down);R8C15(Left);R8C14(Left);R8C13(Left);R8C12(Left);R9C12(Down);R10C12(Down);R11C12(Down);R12C12(Down);R13C12(Down);R14C12(Down);R14C13(Right);R14C14(Right);R14C15(Right);R14C16(Right);R14C17(Right) Packets:R8C12(8);R14C17(19) MaxPackets:3 Distance:19";
            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void RouteAndGridPacketsBehaviors()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 3, 10);

            Cell cell = new Cell(8, 12);

            Path path = new Path(cell, delivery.Grid);
            Route route = path.MapRoute(Route.Specs.Route);

            StringBuilder actual = new StringBuilder();
            StringBuilder expected = new StringBuilder();

            Packet gridPacket = delivery.Grid.GetPacket(cell);
            Packet routePacket = route.Packets.Last();

            actual.AppendLine($"{gridPacket};{routePacket}");
            expected.AppendLine($"{gridPacket};{routePacket}");

            actual.AppendLine($"{gridPacket};{routePacket}");
            expected.AppendLine($"{gridPacket};{routePacket}");

            path.MapRoute(Route.Specs.Route);

            actual.AppendLine($"{gridPacket};{routePacket}");
            expected.AppendLine($"{gridPacket};{routePacket}");

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}