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
    public class HeaterTests
    {
        [TestMethod()]
        public void CompareToTest()
        {
            Heater h0 = new Heater("aa", "Kitchen", "Kitchen");
            Heater h1 = new Heater("ab", "Bathroom", "Bathroom");
            Heater h2 = new Heater("ac", "Bedroom", "Bedroom");

            Assert.IsTrue(h0.CompareTo(h1) > 0);
            Assert.IsTrue(h0.CompareTo(h2) > 0);
            Assert.IsTrue(h2.CompareTo(h1) > 0);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Heater h0 = new Heater("aa", "Kitchen", "Kitchen");
            Heater h1 = new Heater("aa", "Bathroom", "Kitchen");
            Heater h2 = new Heater("ac", "Kitchen", "Kitchen");
            Heater h3 = new Heater("aa", "Kitchen", "Kitchen");

            Assert.AreEqual(h0, h3);
            Assert.AreNotEqual(h0, h1);
            Assert.AreNotEqual(h0, h2);
        }
    }
}