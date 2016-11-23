namespace SoatChallenge.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoatChallenge;

    /// <summary>Test Path class</summary>
    [TestClass]
    public class PathTests
    {
        [TestMethod]
        public void ClosePaths()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);
            Path path = new Path(new Cell(8, 7), delivery.Grid);

            string actual = path.ClosestPendingPath(path.ReachCell).ToString();

            string expected = "StartCell:R8C7 ReachCell:R8C12 Distance:5";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Directions()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);

            StringBuilder actual = new StringBuilder();
            actual.AppendLine(new Path(new Cell(2, 2), delivery.Grid, new Cell(12, 7)).Directions.ToString());
            actual.AppendLine(new Path(new Cell(12, 1), delivery.Grid, new Cell(12, 7)).Directions.ToString());
            actual.AppendLine(new Path(new Cell(16, 7), delivery.Grid, new Cell(12, 7)).Directions.ToString());
            actual.AppendLine(new Path(new Cell(8, 12), delivery.Grid, new Cell(12, 7)).Directions.ToString());
            actual.AppendLine(new Path(new Cell(14, 17), delivery.Grid, new Cell(12, 7)).Directions.ToString());

            StringBuilder expected = new StringBuilder();
            expected.AppendLine("D15 (10Up 5Left)");
            expected.AppendLine("D6 (0Stay 6Left)");
            expected.AppendLine("D4 (4Down 0Stay)");
            expected.AppendLine("D9 (4Up 5Right)");
            expected.AppendLine("D12 (2Down 10Right)");

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        /// <summary>uggly performance :(((((((</summary>
        [TestMethod, Timeout(5000)]
        public void PerformanceFromFarthestPath()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            Delivery ndelivery = Delivery.CreateDelivery(Inputs.ChallengeInput, 4, 40);

            Write.Trace($"{Environment.NewLine} - - creating challenge delivery :{timer.ElapsedMilliseconds} ms{Environment.NewLine}");
            timer.Restart();

            List<Path> paths = new List<Path>();

            foreach (ICell cell in ndelivery.Grid.Packets)
            {
                paths.Add(new Path(cell, ndelivery.Grid));
            }

            Write.Trace($"{Environment.NewLine} - - mdpping all path :{timer.ElapsedMilliseconds} ms{Environment.NewLine}");

            // get farthest packet, should be distance 726
            Path path = paths.OrderByDescending(x => x.Distance).Take(1).First();

            timer.Restart();

            Route route = path.MapRoute(Route.Specs.Route);
            route?.Reset();

            Write.Trace($"{Environment.NewLine} - - Route ({path.Distance}) mdpping time :{timer.ElapsedMilliseconds} ms{Environment.NewLine}");
            timer.Restart();

            route = path.MapRoute(Route.Specs.Free);
            route?.Reset();

            Write.Trace($"{Environment.NewLine} - - Free ({path.Distance}) mdpping time :{timer.ElapsedMilliseconds} ms{Environment.NewLine}");
            timer.Restart();

            route = path.MapRoute(Route.Specs.All);

            Write.Trace($"{Environment.NewLine} - - All ({path.Distance}) mdpping time :{timer.ElapsedMilliseconds} ms{Environment.NewLine}");
        }

        [TestMethod]
        public void RouteOnMapBubble()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);

            Path path = new Path(new Cell(4, 16), delivery.Grid);

            Route route = path.MapBubbleRoute(Route.Specs.Free);

            string actual = route.ToString();

            string expected = "StartCell:R4C16 ReachCell:R14C17 Cells:R3C16(Up);R2C16(Up);R2C17(Right);R2C18(Right);R2C19(Right);R2C0(Right);R2C1(Right);R2C2(Right);R3C2(Down);R4C2(Down);R5C2(Down);R6C2(Down);R7C2(Down);R8C2(Down);R9C2(Down);R10C2(Down);R11C2(Down);R12C2(Down);R12C1(Left);R13C1(Down);R13C0(Left);R13C19(Left);R13C18(Left);R14C18(Down);R14C17(Left) Packets:R2C2(8);R12C1(19);R14C17(25) MaxPackets:3 Distance:25";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RouteOnMapBubbleWithStart()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);
            Path path = new Path(new Cell(8, 12), delivery.Grid);

            Route route = path.MapBubbleRoute(Route.Specs.Free);

            string actual = route.ToString();

            string expected = "StartCell:R4C16 ReachCell:R12C1 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R7C15(Left);R7C14(Left);R7C13(Left);R8C13(Down);R8C12(Left);R9C12(Down);R9C13(Right);R10C13(Down);R11C13(Down);R11C14(Right);R11C15(Right);R11C16(Right);R12C16(Down);R13C16(Down);R14C16(Down);R14C17(Right);R13C17(Up);R13C18(Right);R13C19(Right);R13C0(Right);R12C0(Up);R12C1(Right) Packets:R8C12(8);R14C17(19);R12C1(25) MaxPackets:3 Distance:25";
            Write.Trace(actual);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RouteOnMapFreeDownLeft()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);
            Path path = new Path(new Cell(16, 7), delivery.Grid);

            StringBuilder actual = new StringBuilder();
            Route route = path.MapRoute(Route.Specs.Free);
            actual.AppendLine(route?.ToString() ?? "null");
            route.Reset();
            route = path.MapRoute(Route.Specs.Free);
            actual.AppendLine(route?.ToString() ?? "null");
            route.Reset();
            route = path.MapRoute(Route.Specs.Free | Route.Specs.Alternative);
            actual.AppendLine(route?.ToString() ?? "null");
            route.Reset();
            route = path.MapRoute(Route.Specs.Free | Route.Specs.Opposite);
            actual.AppendLine(route?.ToString() ?? "null");

            StringBuilder expected = new StringBuilder();
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R7C15(Left);R7C14(Left);R7C13(Left);R8C13(Down);R9C13(Down);R10C13(Down);R11C13(Down);R12C13(Down);R13C13(Down);R14C13(Down);R15C13(Down);R16C13(Down);R16C12(Left);R16C11(Left);R16C10(Left);R16C9(Left);R16C8(Left);R16C7(Left) Packets:R16C7(21) MaxPackets:3 Distance:21");
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R7C15(Left);R7C14(Left);R7C13(Left);R8C13(Down);R9C13(Down);R10C13(Down);R11C13(Down);R12C13(Down);R13C13(Down);R14C13(Down);R15C13(Down);R16C13(Down);R16C12(Left);R16C11(Left);R16C10(Left);R16C9(Left);R16C8(Left);R16C7(Left) Packets:R16C7(21) MaxPackets:3 Distance:21");
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R4C15(Left);R4C14(Left);R4C13(Left);R5C13(Down);R6C13(Down);R7C13(Down);R7C12(Left);R7C11(Left);R7C10(Left);R7C9(Left);R7C8(Left);R7C7(Left);R8C7(Down);R9C7(Down);R10C7(Down);R11C7(Down);R12C7(Down);R13C7(Down);R14C7(Down);R15C7(Down);R16C7(Down) Packets:R16C7(21) MaxPackets:3 Distance:21");
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R8C16(Down);R9C16(Down);R10C16(Down);R11C16(Down);R12C16(Down);R13C16(Down);R14C16(Down);R15C16(Down);R16C16(Down);R16C17(Right);R16C18(Right);R16C19(Right);R16C0(Right);R16C1(Right);R16C2(Right);R16C3(Right);R16C4(Right);R16C5(Right);R16C6(Right);R16C7(Right) Packets:R16C7(23) MaxPackets:3 Distance:23");

            Write.Trace(actual.ToString());

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void RouteOnMapRouteDownLeft()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);
            Path path = new Path(new Cell(16, 7), delivery.Grid);

            StringBuilder actual = new StringBuilder();

            Route route = path.MapRoute(Route.Specs.Route);
            actual.AppendLine(route?.ToString() ?? "null");
            route?.Reset();

            route = path.MapRoute(Route.Specs.Free);
            actual.AppendLine(route?.ToString() ?? "null");
            route?.Reset();

            route = path.MapRoute(Route.Specs.Route | Route.Specs.Alternative);
            actual.AppendLine(route?.ToString() ?? "null");
            route?.Reset();

            route = path.MapRoute(Route.Specs.Route | Route.Specs.Opposite);
            actual.AppendLine(route?.ToString() ?? "null");
            route?.Reset();

            StringBuilder expected = new StringBuilder();
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R8C16(Down);R8C15(Left);R8C14(Left);R8C13(Left);R8C12(Left);R9C12(Down);R10C12(Down);R11C12(Down);R12C12(Down);R13C12(Down);R14C12(Down);R15C12(Down);R16C12(Down);R16C11(Left);R16C10(Left);R16C9(Left);R16C8(Left);R16C7(Left) Packets:R8C12(8);R16C7(21) MaxPackets:3 Distance:21");
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R7C15(Left);R7C14(Left);R7C13(Left);R8C13(Down);R9C13(Down);R10C13(Down);R11C13(Down);R12C13(Down);R13C13(Down);R14C13(Down);R15C13(Down);R16C13(Down);R16C12(Left);R16C11(Left);R16C10(Left);R16C9(Left);R16C8(Left);R16C7(Left) Packets:R16C7(21) MaxPackets:3 Distance:21");
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R4C15(Left);R4C14(Left);R4C13(Left);R4C12(Left);R5C12(Down);R6C12(Down);R7C12(Down);R8C12(Down);R8C11(Left);R8C10(Left);R8C9(Left);R8C8(Left);R8C7(Left);R9C7(Down);R10C7(Down);R11C7(Down);R12C7(Down);R13C7(Down);R14C7(Down);R15C7(Down);R16C7(Down) Packets:R8C12(8);R16C7(21) MaxPackets:3 Distance:21");
            expected.AppendLine("StartCell:R4C16 ReachCell:R16C7 Cells:R4C17(Right);R5C17(Down);R6C17(Down);R7C17(Down);R8C17(Down);R9C17(Down);R10C17(Down);R11C17(Down);R12C17(Down);R13C17(Down);R14C17(Down);R15C17(Down);R16C17(Down);R16C18(Right);R16C19(Right);R16C0(Right);R16C1(Right);R16C2(Right);R16C3(Right);R16C4(Right);R16C5(Right);R16C6(Right);R16C7(Right) Packets:R14C17(11);R16C7(23) MaxPackets:3 Distance:23");

            Write.Trace(actual.ToString());

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void RouteWait()
        {
            string editedExampleFile = System.IO.Path.Combine(Environment.CurrentDirectory, "newExample.txt");

            if (System.IO.File.Exists(editedExampleFile))
            {
                System.IO.File.Delete(editedExampleFile);
            }

            System.IO.File.Copy(Inputs.ExampleInput, editedExampleFile);
            System.IO.File.AppendAllText(editedExampleFile, $"{Environment.NewLine}9 13{Environment.NewLine}9 12{Environment.NewLine}8 11");

            Delivery delivery = Delivery.CreateDelivery(editedExampleFile, 4, 10);

            Cell dp = new Cell(8, 11);
            Cell ap = new Cell(8, 12);
            Cell cp = new Cell(9, 12);
            Cell bp = new Cell(9, 13);

            Packet pa = delivery.Grid.GetPacket(ap);
            delivery.Grid.SetPacketState(ap, Packet.State.Assigned);
            delivery.Grid.SetPacketDistance(ap, 0);

            Packet pb = delivery.Grid.GetPacket(bp);
            delivery.Grid.SetPacketState(bp, Packet.State.Assigned);
            delivery.Grid.SetPacketDistance(bp, 5);

            Packet pc = delivery.Grid.GetPacket(cp);
            delivery.Grid.SetPacketState(cp, Packet.State.Assigned);
            delivery.Grid.SetPacketDistance(cp, 5);

            Packet pd = delivery.Grid.GetPacket(dp);
            delivery.Grid.SetPacketState(dp, Packet.State.Assigned);
            delivery.Grid.SetPacketDistance(dp, 5);

            Write.Trace($"{pa}, state :{pa.CurrentState}");
            Write.Trace($"{pb}, state :{pb.CurrentState}");
            Write.Trace($"{pc}, state :{pc.CurrentState}");
            Write.Trace($"{pd}, state :{pd.CurrentState}");

            Path path = new Path(new Cell(16, 7), delivery.Grid, new Cell(8, 13));

            Route route = path.MapRoute(Route.Specs.Free | Route.Specs.Wait);

            string actual = route.ToString();

            Write.Trace(actual);

            string expected = "StartCell:R8C13 ReachCell:R16C7 Cells:R8C13(Stay);R8C13(Stay);R8C13(Stay);R8C13(Stay);R8C12(Left);R9C12(Down);R10C12(Down);R11C12(Down);R12C12(Down);R13C12(Down);R14C12(Down);R15C12(Down);R16C12(Down);R16C11(Left);R16C10(Left);R16C9(Left);R16C8(Left);R16C7(Left) Packets:R16C7(18) MaxPackets:4 Distance:18";
            Assert.AreEqual(expected, actual);
        }
    }
}