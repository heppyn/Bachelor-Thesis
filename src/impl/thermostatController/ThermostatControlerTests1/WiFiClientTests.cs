using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThermostatControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler.Tests
{
    [TestClass()]
    public class WiFiClientTests
    {
        // se this before sending
        private const String ip = "192.168.1.194";
        private WiFiClient client = new WiFiClient(ip);

        // 25 C all the time
        [TestMethod()]
        public void SendScheduleTest()
        {
            Schedule sch = new Schedule("25 all the time");
            sch.AddNode(6, new TempNode(23.5, 25.0));

            client.SendSchedule(sch);
        }

        [TestMethod()]
        public void ChangeIdTest()
        {
            client.ChangeId("aa");
        }

        [TestMethod()]
        public void SendAlternativeScheduleTest()
        {
            Schedule sch = new Schedule("25 all the time");
            sch.AddNode(6, new TempNode(23.5, 25.0));

            client.SendAlternativeSchedule(sch);
        }

        [TestMethod()]
        public void SendReturnDateAndTemperatureTest()
        {
            client.SendReturnDateAndTemperature(DateTime.Today, 0);
        }

        [TestMethod()]
        public void IdentifyHeaterTest()
        {
            String res = client.IdentifyHeater();
        }

        [TestMethod()]
        public void IdentifyHeaterTest1()
        {
            Assert.AreEqual("aa", client.IdentifyHeater());
        }

        [TestMethod()]
        public void GetTemperatureTest()
        {
            double res = client.GetTemperature();
            Assert.IsTrue(res < 25.0);
            Assert.IsTrue(res > 15.0);
        }
    }
}