namespace SoatChallenge.Tests
{
    using System.Linq;
    using System.Text;
    using Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoatChallenge;

    /// <summary>Test Path class</summary>
    [TestClass]
    public class DroneTests
    {
        private static Delivery delivery;

        /// <summary>Inialize a new static instance of delivery</summary>
        /// <param name="context">test context</param>
        [ClassInitialize]
        public static void InitializeDelivery(TestContext context)
        {
            DroneTests.delivery = Delivery.CreateDelivery(Inputs.ChallengeInput, 4, 40);
        }

        [TestMethod]
        public void CountStateOnDelivery()
        {
            int actual = (from i in DroneTests.delivery.Drones where (i.CurrentState == Drone.State.Pending) && (Drone.MaxPackets == 4) select i).Count();

            Assert.AreEqual(400, actual);
        }

        [TestMethod]
        public void DroneOnSetRoute()
        {
            Path path = new Path(new Cell(84, 39), DroneTests.delivery.Grid);
            Route route = path.MapRoute(Route.Specs.Route);

            Drone drone = new Drone(0, DroneTests.delivery.Grid);
            drone.SetRoute(route);

            string expected = "Id:0 State:Ready Position:R4C199 Round:0 PacketNumber:3 MaxPackets:4 Route:(StartCell:R4C199 ReachCell:R84C39 Cells:R5C199(Down);R6C199(Down);R7C199(Down);R8C199(Down);R9C199(Down);R10C199(Down);R11C199(Down);R12C199(Down);R13C199(Down);R14C199(Down);R15C199(Down);R16C199(Down);R17C199(Down);R18C199(Down);R19C199(Down);R20C199(Down);R21C199(Down);R22C199(Down);R23C199(Down);R24C199(Down);R25C199(Down);R26C199(Down);R27C199(Down);R28C199(Down);R29C199(Down);R30C199(Down);R31C199(Down);R32C199(Down);R33C199(Down);R34C199(Down);R35C199(Down);R36C199(Down);R37C199(Down);R38C199(Down);R39C199(Down);R40C199(Down);R41C199(Down);R42C199(Down);R43C199(Down);R44C199(Down);R45C199(Down);R46C199(Down);R47C199(Down);R48C199(Down);R49C199(Down);R50C199(Down);R51C199(Down);R52C199(Down);R53C199(Down);R54C199(Down);R55C199(Down);R56C199(Down);R57C199(Down);R58C199(Down);R59C199(Down);R60C199(Down);R61C199(Down);R62C199(Down);R63C199(Down);R64C199(Down);R65C199(Down);R66C199(Down);R67C199(Down);R68C199(Down);R69C199(Down);R70C199(Down);R71C199(Down);R72C199(Down);R73C199(Down);R74C199(Down);R75C199(Down);R76C199(Down);R77C199(Down);R78C199(Down);R79C199(Down);R80C199(Down);R81C199(Down);R82C199(Down);R83C199(Down);R84C199(Down);R84C198(Left);R84C197(Left);R84C196(Left);R84C195(Left);R84C194(Left);R84C193(Left);R84C192(Left);R84C191(Left);R84C190(Left);R84C189(Left);R84C188(Left);R84C187(Left);R84C186(Left);R84C185(Left);R84C184(Left);R84C183(Left);R84C182(Left);R84C181(Left);R84C180(Left);R84C179(Left);R84C178(Left);R84C177(Left);R84C176(Left);R84C175(Left);R84C174(Left);R84C173(Left);R84C172(Left);R84C171(Left);R84C170(Left);R84C169(Left);R84C168(Left);R84C167(Left);R84C166(Left);R84C165(Left);R84C164(Left);R84C163(Left);R84C162(Left);R84C161(Left);R84C160(Left);R84C159(Left);R84C158(Left);R84C157(Left);R84C156(Left);R84C155(Left);R84C154(Left);R84C153(Left);R84C152(Left);R84C151(Left);R84C150(Left);R84C149(Left);R84C148(Left);R84C147(Left);R84C146(Left);R84C145(Left);R84C144(Left);R84C143(Left);R84C142(Left);R84C141(Left);R84C140(Left);R84C139(Left);R84C138(Left);R84C137(Left);R84C136(Left);R84C135(Left);R84C134(Left);R84C133(Left);R84C132(Left);R84C131(Left);R84C130(Left);R84C129(Left);R84C128(Left);R84C127(Left);R84C126(Left);R84C125(Left);R84C124(Left);R84C123(Left);R84C122(Left);R84C121(Left);R84C120(Left);R84C119(Left);R84C118(Left);R84C117(Left);R84C116(Left);R84C115(Left);R84C114(Left);R84C113(Left);R84C112(Left);R84C111(Left);R84C110(Left);R84C109(Left);R84C108(Left);R84C107(Left);R84C106(Left);R84C105(Left);R84C104(Left);R84C103(Left);R84C102(Left);R84C101(Left);R84C100(Left);R84C99(Left);R84C98(Left);R84C97(Left);R84C96(Left);R84C95(Left);R84C94(Left);R84C93(Left);R84C92(Left);R84C91(Left);R84C90(Left);R84C89(Left);R84C88(Left);R84C87(Left);R84C86(Left);R84C85(Left);R84C84(Left);R84C83(Left);R84C82(Left);R84C81(Left);R84C80(Left);R84C79(Left);R84C78(Left);R84C77(Left);R84C76(Left);R84C75(Left);R84C74(Left);R84C73(Left);R84C72(Left);R84C71(Left);R84C70(Left);R84C69(Left);R84C68(Left);R84C67(Left);R84C66(Left);R84C65(Left);R84C64(Left);R84C63(Left);R84C62(Left);R84C61(Left);R84C60(Left);R84C59(Left);R84C58(Left);R84C57(Left);R84C56(Left);R84C55(Left);R84C54(Left);R84C53(Left);R84C52(Left);R84C51(Left);R84C50(Left);R84C49(Left);R84C48(Left);R84C47(Left);R84C46(Left);R84C45(Left);R84C44(Left);R84C43(Left);R84C42(Left);R84C41(Left);R84C40(Left);R84C39(Left) Packets:R84C178(101);R84C78(201);R84C39(240) MaxPackets:4 Distance:240)";

            Assert.AreEqual(expected, drone.ToString());
        }

        [TestMethod]
        public void MaxPacketsFromDistance()
        {
            StringBuilder expected = new StringBuilder();
            expected.AppendLine("distance 840 : 4");
            expected.AppendLine("distance 841 : 3");
            expected.AppendLine("distance 880 : 3");
            expected.AppendLine("distance 881 : 2");
            expected.AppendLine("distance 920 : 2");
            expected.AppendLine("distance 921 : 1");
            expected.AppendLine("distance 960 : 1");
            expected.AppendLine("distance 961 : 0");

            StringBuilder actual = new StringBuilder();
            actual.AppendLine($"distance 840 : {Drone.MaxPacketsToReachDistance(840)}");
            actual.AppendLine($"distance 841 : {Drone.MaxPacketsToReachDistance(841)}");
            actual.AppendLine($"distance 880 : {Drone.MaxPacketsToReachDistance(880)}");
            actual.AppendLine($"distance 881 : {Drone.MaxPacketsToReachDistance(881)}");
            actual.AppendLine($"distance 920 : {Drone.MaxPacketsToReachDistance(920)}");
            actual.AppendLine($"distance 921 : {Drone.MaxPacketsToReachDistance(921)}");
            actual.AppendLine($"distance 960 : {Drone.MaxPacketsToReachDistance(960)}");
            actual.AppendLine($"distance 961 : {Drone.MaxPacketsToReachDistance(961)}");

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void MoveStringOnMoves()
        {
            Delivery ndelivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);

            Path path = new Path(new Cell(16, 7), ndelivery.Grid);
            
            ndelivery.MapRoutes();
            ndelivery.Start();

            string actual = ndelivery.Drones.First().MoveString;
            string expected = "3 2 2 3 3 3 3 3 3 4 4 4 4 4 4 4 4 4 4 1 4 1 1 1 4 1 0 0 0 0 0";

            Assert.AreEqual(expected, actual);
        }
    }
}