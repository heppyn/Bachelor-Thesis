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
    public class HeatersTests
    {
        [TestMethod()]
        public void AddHeaterTest()
        {
            Heaters h = new Heaters();
            h.AddHeater(new Heater(Heaters.NextId(), "a", "a"));

            Assert.AreEqual(Heaters.LastId, "ab");

            h.AddHeater(new Heater("ac", "a", "a"));
            Assert.AreEqual(Heaters.LastId, "ac");

            h.AddHeater(new Heater("ab", "a", "a"));
            Assert.AreEqual(Heaters.LastId, "ac");

            h.AddHeater(new Heater("ba", "a", "a"));
            Assert.AreEqual(Heaters.LastId, "ba");

            h.AddHeater(new Heater("bc", "a", "a"));
            Assert.AreEqual(Heaters.LastId, "bc");
        }

        [TestMethod()]
        public void NextIdTest()
        {
            Heaters h = new Heaters();

            Assert.AreEqual(Heaters.LastId, "aa");
            Assert.AreEqual(Heaters.NextId(), "ab");
        }
    }
}