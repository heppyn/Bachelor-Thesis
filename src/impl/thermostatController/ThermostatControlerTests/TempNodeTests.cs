using ThermostatControler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ThermostatControler.Tests
{
    [TestClass()]
    public class TempNodeTests
    {
        [TestMethod()]
        public void TempNodeGet()
        {
            TempNode n1, n2;
            n1 = new TempNode(71, 111);
            n2 = new TempNode(7.1, 21.1);

            Assert.IsTrue(n1.GetTemperature() == (byte)111);
            Assert.IsTrue(n2.GetTemperature() == (byte)111);
            Assert.IsTrue(n2.GetTime() == (byte)71);
        }

        [TestMethod()]
        public void GetNumOfMinutes()
        {
            Assert.IsTrue(new TempNode(0.0, 11.1).GetNumOfMinutes() == (byte)0);
            Assert.IsTrue(new TempNode(0.5, 11.1).GetNumOfMinutes() == (byte)5);
            Assert.IsTrue(new TempNode(1.5, 11.1).GetNumOfMinutes() == (byte)1 * 6 + 5);
            Assert.IsTrue(new TempNode(23.5, 11.1).GetNumOfMinutes() == (byte)23 * 6 + 5);
            Assert.IsTrue(new TempNode(10.4, 11.1).GetNumOfMinutes() == (byte)10 * 6 + 4);
        }

        [TestMethod()]
        public void TempNodeEquals()
        {
            TempNode n1, n2, n3, n4;
            n1 = new TempNode(71, 111);
            n2 = new TempNode(7.1, 21.1);
            n3 = new TempNode(71, 200);
            n4 = new TempNode(n1);

            Assert.AreEqual(n1, n2);
            Assert.AreEqual(n1, n3);
            Assert.AreEqual(n3, n2);
            Assert.AreEqual(n1, n4);
            Assert.IsTrue(n1.GetTemperature() == n2.GetTemperature());
            Assert.IsFalse(n3.GetTemperature() == n2.GetTemperature());
        }

        [TestMethod()]
        public void TempNodeOperator_Equals()
        {
            TempNode n1, n2, n3, n4;
            n1 = new TempNode(235, 73);
            n2 = new TempNode(23.5, 17.3);
            n3 = new TempNode(23.5, 10.7);
            n4 = new TempNode(23.4, 17.3);

            Assert.IsTrue(n1 == n2);
            Assert.IsTrue(n1 == n3);
            Assert.IsTrue(n3 == n2);

            Assert.IsFalse(n1 != n2);
            Assert.IsFalse(n3 != n2);
            Assert.IsFalse(n1 != n3);

            Assert.IsTrue(n1 != n4);
            Assert.IsTrue(n2 != n4);
        }

        [TestMethod()]
        public void TempNodeOperator_Less()
        {
            TempNode n1, n2, n3, n4, n5;
            n1 = new TempNode(235, 73);
            n2 = new TempNode(23.5, 17.3);
            n3 = new TempNode(17.4, 25.5);
            n4 = new TempNode(23.4, 17.3);
            n5 = new TempNode(7.4, 10.0);

            Assert.IsTrue(n3 < n2);
            Assert.IsTrue(n2 > n3);
            Assert.IsTrue(n5 < n2);

            Assert.IsTrue(n2 <= n1);
            Assert.IsTrue(n3 <= n4);

            Assert.IsTrue(n4 >= n3);

            Assert.IsFalse(n1 < n2);
            Assert.IsTrue(n4 < n2);
        }

        [TestMethod()]
        public void TempNodeCompareTo()
        {
            TempNode n1, n2, n3;
            n1 = new TempNode(71, 111);
            n2 = new TempNode(7.1, 21.1);
            n3 = new TempNode(17.4, 25.5);

            Assert.IsTrue(n1.CompareTo(n2) == 0);
            Assert.IsTrue(n1.CompareTo(n3) < 0);
            Assert.IsTrue(n3.CompareTo(n2) > 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TempNodeBoundries_1()
        {
            TempNode n1;
            n1 = new TempNode(-0.1, 20.0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TempNodeBoundries_2()
        {
            TempNode n1;
            n1 = new TempNode(0.1, 30.1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TempNodeBoundries_3()
        {
            TempNode n1;
            n1 = new TempNode(0, 201);
        }

        [TestMethod()]
        public void TempNodeBoundries_4()
        {
            TempNode n1;
            n1 = new TempNode(0, 200);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TempNodeBoundries_5()
        {
            TempNode n1;
            n1 = new TempNode(7.6, 20.0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TempNodeBoundries_6()
        {
            TempNode n1;
            n1 = new TempNode(24.0, 20.0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TempNodeBoundries_7()
        {
            TempNode n1;
            n1 = new TempNode(240, 20.0);
        }

        [TestMethod()]
        public void GetTimeHoursTest()
        {
            TempNode n = new TempNode(.0, 10);
            Assert.AreEqual(n.GetTimeHours(), 0);

            n = new TempNode(23.5, 10);
            Assert.AreEqual(n.GetTimeHours(), 23);

            n = new TempNode(11.0, 10);
            Assert.AreEqual(n.GetTimeHours(), 11);

            n = new TempNode(2.2, 10);
            Assert.AreEqual(n.GetTimeHours(), 2);

            n = new TempNode(8.4, 10);
            Assert.AreEqual(n.GetTimeHours(), 8);
        }

        [TestMethod()]
        public void GetTimeMinutesTest()
        {
            TempNode n = new TempNode(.0, 10);
            Assert.AreEqual(n.GetTimeMinutes(), 0);

            n = new TempNode(23.5, 10);
            Assert.AreEqual(n.GetTimeMinutes(), 50);

            n = new TempNode(4.1, 10);
            Assert.AreEqual(n.GetTimeMinutes(), 10);

            n = new TempNode(14.2, 10);
            Assert.AreEqual(n.GetTimeMinutes(), 20);

            n = new TempNode(17.4, 10);
            Assert.AreEqual(n.GetTimeMinutes(), 40);
        }

        [TestMethod()]
        public void GetTemperatureDoubleTest()
        {
            TempNode n = new TempNode(.0, 10);
            Assert.AreEqual(n.GetTemperatureDouble(), 10);

            n = new TempNode(.0, 30);
            Assert.AreEqual(n.GetTemperatureDouble(), 30);

            n = new TempNode(.0, 10.1);
            Assert.AreEqual(n.GetTemperatureDouble(), 10.1);

            n = new TempNode(.0, 11.1);
            Assert.AreEqual(n.GetTemperatureDouble(), 11.1);

            n = new TempNode(.0, 29.9);
            Assert.AreEqual(n.GetTemperatureDouble(), 29.9);
        }
    }
}