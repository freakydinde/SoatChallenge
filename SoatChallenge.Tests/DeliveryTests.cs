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
            expected.AppendLine("StartCell:R4C16 ReachCell:R2C2 Cells:R3C16(Up);R2C16(Up);R2C17(Right);R2C18(Right);R2C19(Right);R2C0(Right);R2C1(Right);R2C2(Right) Packets:R2C2(8) MaxPackets:1 Distance:8");
            expected.AppendLine("StartCell:R4C16 ReachCell:R8C12 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R7C15(Left);R7C14(Left);R7C13(Left);R8C13(Down);R8C12(Left) Packets:R8C12(8) MaxPackets:1 Distance:8");
            expected.AppendLine("StartCell:R4C16 ReachCell:R14C17 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R8C16(Down);R9C16(Down);R10C16(Down);R11C16(Down);R12C16(Down);R13C16(Down);R14C16(Down);R14C17(Right) Packets:R14C17(11) MaxPackets:1 Distance:11");
            expected.Append("StartCell:R4C16 ReachCell:R16C7 Cells:R5C16(Down);R6C16(Down);R7C16(Down);R8C16(Down);R9C16(Down);R10C16(Down);R11C16(Down);R12C16(Down);R12C17(Right);R12C18(Right);R12C19(Right);R12C0(Right);R12C1(Right);R13C1(Down);R14C1(Down);R15C1(Down);R16C1(Down);R16C2(Right);R16C3(Right);R16C4(Right);R16C5(Right);R16C6(Right);R16C7(Right) Packets:R12C1(13);R16C7(23) MaxPackets:2 Distance:23");

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

            StringBuilder expected = new StringBuilder();
            expected.Append("1 2 2 3 3 3 3 3 3 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0;");
            expected.Append("1 4 4 4 1 1 1 4 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0;");
            expected.Append("1 4 4 4 4 4 4 4 4 4 4 3 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0;");
            expected.Append("2 4 4 4 4 4 4 4 4 3 3 3 3 3 4 4 4 4 3 3 3 3 3 3 0 0 0 0 0 0 0;");
            expected.Append("score : 400");

            Write.Trace(actual);

            Assert.AreEqual(expected.ToString(), actual);
        }
    }
}