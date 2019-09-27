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
    public class UserTests
    {
        [TestMethod()]
        public void HasSameNameTest()
        {
            User u0 = new User("Karel", "123456");
            User u1 = new User("Karel", "123456");
            User u2 = new User("Karel", "000000");
            User u3 = new User("Karel", "");

            User u4 = new User("Honza", "123456");
            User u5 = new User("Karel ", "123456");

            Assert.IsTrue(u0.HasSameName(u1));
            Assert.IsTrue(u0.HasSameName(u2));
            Assert.IsTrue(u0.HasSameName(u3));
            Assert.IsTrue(u2.HasSameName(u1));
            Assert.IsTrue(u3.HasSameName(u1));

            Assert.IsFalse(u0.HasSameName(u4));
            Assert.IsFalse(u0.HasSameName(u5));
            Assert.IsFalse(u5.HasSameName(u4));
        }

        [TestMethod()]
        public void EqualsTest()
        {
            User u0 = new User("Karel", "123456");
            User u1 = new User("Karel", "123456");
            User u2 = new User("Karel", "000000");
            User u3 = new User("Karel", "");

            User u4 = new User("Honza", "123456");
            User u5 = new User("Karel ", "123456");

            Assert.IsTrue(u0.Equals(u1));

            Assert.IsFalse(u0.Equals(u2));
            Assert.IsFalse(u0.Equals(u3));
            Assert.IsFalse(u0.Equals(u4));
            Assert.IsFalse(u0.Equals(u5));
        }
    }
}