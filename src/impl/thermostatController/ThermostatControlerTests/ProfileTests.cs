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
    public class ProfileTests
    {
        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual("Summer", new Profile("Summer").ToString());
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.AreEqual(new Profile("Summer"), new Profile("Summer"));
        }

        [TestMethod()]
        public void EqualsTest_DifferentMembers()
        {
            Profile p1 = new Profile("Winter");
            Profile p2 = new Profile("Winter");

            p1.Add("ab", "Work");
            Assert.AreEqual(p1, p2);

            p2.Add("ac", "Weekend");
            Assert.AreEqual(p1, p2);
        }
    }
}