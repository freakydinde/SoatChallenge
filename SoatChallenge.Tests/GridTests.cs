namespace SoatChallenge.Tests
{
    using System.Text;
    using Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoatChallenge;

    /// <summary>Test Path class</summary>
    [TestClass]
    public class GridTests
    {
        private static Delivery delivery;

        /// <summary>Inialize a new static instance of delivery</summary>
        /// <param name="context">test context</param>
        [ClassInitialize]
        public static void InitializeDelivery(TestContext context)
        {
            GridTests.delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);
        }

        [TestMethod]
        public void GetSetPackets()
        {
            Cell cell = new Cell(8, 12);
            StringBuilder actual = new StringBuilder();

            Packet gridPacket = delivery.Grid.GetPacket(cell);
            GridTests.delivery.Grid.SetPacketDistance(cell, 10);

            actual.Append(gridPacket?.ToString());

            GridTests.delivery.Grid.AddToPacketDistance(cell, 40);

            gridPacket = delivery.Grid.GetPacket(cell);

            actual.Append(gridPacket?.ToString());

            string expected = $"R8C12(10)R8C12(50)";

            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void IsPacketRouteFree()
        {
            string expected = "True;True;True;False;False;False";

            Cell packetCell = new Cell(8, 12);
            Cell routeCell = new Cell(9, 12);
            Cell freeCell = new Cell(9, 11);

            string actual = $"{GridTests.delivery.Grid.IsPacket(packetCell)};{GridTests.delivery.Grid.IsRoute(routeCell)};{GridTests.delivery.Grid.IsFree(freeCell)};{GridTests.delivery.Grid.IsPacket(freeCell)};{GridTests.delivery.Grid.IsRoute(freeCell)};{GridTests.delivery.Grid.IsFree(routeCell)}";

            Assert.AreEqual(expected, actual);
        }
    }
}