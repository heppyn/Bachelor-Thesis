using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThermostatControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ThermostatControler.Tests
{
    [TestClass()]
    public class UsersTests
    {
        /// <summary>
        /// Delete pass.xml file
        /// </summary>
        private void Reset()
        {
            File.Delete("pass.xml");
        }

        [TestMethod()]
        public void CreateUserTestTrue()
        {
            Reset();
            Users users = new Users();

            Assert.IsTrue(users.CreateUser(new User("Karel", "karlik123")));
            Assert.IsTrue(users.CreateUser(new User("Karel Vomacka", "karlik123")));
            Assert.IsTrue(users.CreateUser(new User("Karel ", "karlik123")));
            Assert.IsTrue(users.CreateUser(new User(" Karel", "karlik123")));
        }

        [TestMethod()]
        public void CreateUserTestFalse()
        {
            Reset();
            Users users = new Users();
            Assert.IsTrue(users.CreateUser(new User("Karel", "karlik123")));
            Assert.IsTrue(users.CreateUser(new User("Libor", "best password")));

            Assert.IsFalse(users.CreateUser(new User("Karel", "karlik123")));
            Assert.IsFalse(users.CreateUser(new User("Karel", "karlik")));
            Assert.IsFalse(users.CreateUser(new User("Libor", "karlik123")));
            Assert.IsFalse(users.CreateUser(new User("Libor", "")));
        }

        [TestMethod()]
        public void UserExistsTest()
        {
            Reset();
            Users users = new Users();

            Assert.IsTrue(users.CreateUser(new User("Admin", "admin")));
            Assert.IsTrue(users.CreateUser(new User("Honza", "")));

            Assert.IsTrue(users.UserExists(new User("Admin", "admin")) == Users.Response.OK);
            Assert.IsTrue(users.UserExists(new User("Honza", "")) == Users.Response.OK);
        }

        [TestMethod()]
        public void UserExistsTestWrongName()
        {
            Reset();
            Users users = new Users();

            Assert.IsTrue(users.CreateUser(new User("Admin", "admin")));
            Assert.IsTrue(users.CreateUser(new User("Honza", "")));

            Assert.IsTrue(users.UserExists(new User("Admin", "admin")) == Users.Response.OK);
            Assert.IsTrue(users.UserExists(new User("Honza", "")) == Users.Response.OK);

            Assert.IsTrue(users.UserExists(new User("admin", "admin")) == Users.Response.FAIL);
            Assert.IsTrue(users.UserExists(new User("Admin ", "admin")) == Users.Response.FAIL);
            Assert.IsTrue(users.UserExists(new User(" Admin", "admin")) == Users.Response.FAIL);
            Assert.IsTrue(users.UserExists(new User("Honza", "admin")) == Users.Response.FAIL);
        }

        [TestMethod()]
        public void UserExistsTestWrongPass()
        {
            Reset();
            Users users = new Users();

            Assert.IsTrue(users.CreateUser(new User("Admin", "admin")));
            Assert.IsTrue(users.CreateUser(new User("Honza", "")));

            Assert.IsTrue(users.UserExists(new User("Admin", "admin")) == Users.Response.OK);
            Assert.IsTrue(users.UserExists(new User("Honza", "")) == Users.Response.OK);

            Assert.IsTrue(users.UserExists(new User("Admin", "")) == Users.Response.FAIL);
            Assert.IsTrue(users.UserExists(new User("Admin", "a")) == Users.Response.FAIL);
            Assert.IsTrue(users.UserExists(new User("Admin", "Admin")) == Users.Response.FAIL);
            Assert.IsTrue(users.UserExists(new User("Admin", " admin")) == Users.Response.FAIL);
        }

        [TestMethod()]
        public void UserExistsTestDefaultPass()
        {
            Reset();
            Users users = new Users();

            Assert.IsTrue(users.UserExists(new User("Admin", "admin")) == Users.Response.DEFAULT);

        }
    }
}