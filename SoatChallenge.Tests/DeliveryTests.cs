namespace SoatChallenge.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoatChallenge;

    /// <summary>Test Delivery class</summary>
    [TestClass]
    public class DeliveryTests
    {
        [TestMethod]
        public void AutonomyOnPacketsNumber()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ChallengeInput, 4, 40);

            string actual = Write.Invariant($"{Delivery.Autonomy(0)};{Delivery.Autonomy(1)};{Delivery.Autonomy(2)};{Delivery.Autonomy(3)};{Delivery.Autonomy(4)}");
            string expected = "1000;960;920;880;840";

            delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 4, 10);

            actual += Write.Invariant($"{Delivery.Autonomy(0)};{Delivery.Autonomy(1)};{Delivery.Autonomy(2)};{Delivery.Autonomy(3)};{Delivery.Autonomy(4)}");
            expected += "60;50;40;30;20";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DeliveryOnBigInput()
        {
            string actual = Delivery.CreateDelivery(Inputs.ChallengeInput, 4, 40).ToString();
            string expected = "PacketsNumber:1500 DronesNumber:400 Round:0 MaxRound:1000 autonomyRatio:40 maxDistance:1000 Drone.MaxPackets:4 Grid:(Rows:499 Columns:499 StartCell:R4C199 PacketNumber:1500)";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MapRoutes()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 2, 10);

            for (int i = 0; i < 3; i++)
            {
                Write.Trace($"max distance with {i} packets : {Delivery.Autonomy(i)}");
            }

            delivery.MapRoutes();

            string actual = Write.Collection(delivery.Routes, Environment.NewLine);

            StringBuilder expected = new StringBuilder();
            expected.AppendLine("StartCell:R4C16 ReachCell:R12C1 Cells:R3C16(Up);R2C16(Up);R2C17(Right);R2C18(Right);R2C19(Right);R2C0(Right);R2C1(Right);R2C2(Right);R3C2(Down);R4C2(Down);R5C2(Down);R6C2(Down);R7C2(Down);R8C2(Down);R9C2(Down);R10C2(Down);R11C2(Down);R12C2(Down);R12C1(Left) Packets:R2C2(8);R12C1(19) MaxPackets:2 Distance:19");
            expected.AppendLine("StartCell:R4C16 ReachCell:R14C17 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R7C15(Left);R7C14(Left);R7C13(Left);R8C13(Down);R8C12(Left);R9C12(Down);R9C13(Right);R10C13(Down);R11C13(Down);R11C14(Right);R11C15(Right);R11C16(Right);R12C16(Down);R13C16(Down);R14C16(Down);R14C17(Right) Packets:R8C12(8);R14C17(19) MaxPackets:2 Distance:19");
            expected.Append("StartCell:R4C16 ReachCell:R16C7 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R7C15(Left);R7C14(Left);R7C13(Left);R8C13(Down);R9C13(Down);R10C13(Down);R11C13(Down);R12C13(Down);R13C13(Down);R14C13(Down);R15C13(Down);R16C13(Down);R16C12(Left);R16C11(Left);R16C10(Left);R16C9(Left);R16C8(Left);R16C7(Left) Packets:R16C7(21) MaxPackets:2 Distance:21");

            Write.Trace(actual);

            Assert.AreEqual(expected.ToString(), actual);
        }

        [TestMethod]
        public void StartScorePrint()
        {
            Delivery delivery = Delivery.CreateDelivery(Inputs.ExampleInput, 2, 10);

            delivery.MapRoutes();
            delivery.Start();

            int score = delivery.Score();
            List<string> moves = delivery.DronesMoves.ToList();

            string filePath = $"{Environment.CurrentDirectory}\\{score}_{DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.CurrentCulture)}.txt";

            File.WriteAllLines(filePath, moves);

            string actual = Write.Collection(File.ReadAllLines(filePath), ";") + ";score : " + score;

            string expected = "2 2 2 3 3 3 3 3 3 4 4 4 4 4 4 4 4 4 4 1 0 0 0 0 0 0 0 0 0 0 0;2 4 4 4 1 1 1 4 1 4 3 4 4 3 3 3 4 4 4 3 0 0 0 0 0 0 0 0 0 0 0;1 4 4 4 1 1 1 4 4 4 4 4 4 4 4 4 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0;0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0;score : 355";

            Assert.AreEqual(expected, actual);
        }
    }
}