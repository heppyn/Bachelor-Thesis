using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThermostatControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace ThermostatControler.Tests
{
    [TestClass()]
    public class WifiDevicesTests
    {
        // set address of device on network
        private byte[] existingAddress = { 192, 168, 1, 194 };
        private WifiDevices wifiDevices = new WifiDevices();

        [TestMethod()]
        public void FindHeatersTest()
        {
            Dictionary<IPAddress, String> res = wifiDevices.FindHeaters();

            Assert.IsTrue(res.Keys.Contains(new IPAddress(existingAddress)));
        }

        [TestMethod()]
        public void PingDevicesTest()
        {
            List<IPAddress> devices = wifiDevices.PingDevices();

            Assert.IsTrue(devices.Contains(new IPAddress(existingAddress)));
        }
    }
}