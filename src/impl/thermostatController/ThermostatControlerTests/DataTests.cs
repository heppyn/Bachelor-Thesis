using ThermostatControler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text;

namespace ThermostatControler.Tests
{
    [TestClass()]
    public class DataTests
    {
        [TestMethod()]
        public void SaveScheduleTest()
        {
            Data d = new Data();
            List<Schedule> l = new List<Schedule> { Generator.GetSchedule(10) };
            Schedule s = new Schedule("sss");
            s.AddNode(0, new TempNode(100, 100));
            l.Add(s);
            d.SaveSchedules(l);
            List<Schedule> l2 = d.LoadSchedules();
            Assert.IsTrue(l.Count == l2.Count);

            for (int i = 0; i < l.Count; i++)
            {
                Assert.AreEqual(l.ElementAt(i), l2.ElementAt(i));
            }
        }

        [TestMethod()]
        public void LoadHeatersTest()
        {
            List<Heater> l0 = new List<Heater>
            {
                new Heater("aa", "Kitchen", "Kitchen"),
                new Heater("ab", "Bathroom", "Bathroom"),
                new Heater("ac", "Bedroom", "Bedroom")
            };

            Heaters heaters = new Heaters();
            heaters.SetHeaters(l0);
            String lastId = Heaters.LastId;

            Data d = new Data();
            d.SaveHeaters(heaters);

            Heaters h = d.LoadHeaters();
            Assert.IsTrue(l0.Count == h.GetHeaters().Count);
            for (int i = 0; i < l0.Count; i++)
            {
                Assert.AreEqual(heaters.GetHeaters()[i], h.GetHeaters()[i]);
            }
            Assert.AreEqual(lastId, Heaters.LastId);
        }

        [TestMethod()]
        public void SaveProfilesTest()
        {
            List<Profile> profiles = new List<Profile>();

            for (int i = 0; i < 3; i++)
            {
                Profile p = new Profile("Profile" + i.ToString());
                for (int j = 0; j < 5; j++)
                {
                    p.Add(j.ToString(), "Schedule" + j.ToString());
                }

                profiles.Add(p);
            }

            Data d = new Data();
            d.SaveProfiles(profiles);
            List<Profile> profiles2 = d.LoadProfiles();

            Assert.AreEqual(profiles.Count, profiles2.Count);

            for (int i = 0; i < profiles.Count; i++)
            {
                Assert.AreEqual(profiles[i].Name, profiles2[i].Name);
                for (int j = 0; j < profiles[i].HeaterIdSchedule.Count; j++)
                {
                    var a = profiles[i].HeaterIdSchedule.ElementAt(j);
                    var b = profiles2[i].HeaterIdSchedule.ElementAt(j);
                    Assert.AreEqual(a.Key, b.Key);
                    Assert.AreEqual(a.Value, b.Value);
                }
            }
        }

        [TestMethod()]
        public void SaveProfilesTest_Active()
        {
            List<Profile> profiles = new List<Profile>();

            for (int i = 0; i < 3; i++)
            {
                Profile p = new Profile("Profile" + i.ToString());
                p.Active = i % 3 == 0;

                profiles.Add(p);
            }

            Data d = new Data();
            d.SaveProfiles(profiles);
            List<Profile> profiles2 = d.LoadProfiles();

            Assert.AreEqual(profiles.Count, profiles2.Count);

            for (int i = 0; i < profiles.Count; i++)
            {
                Assert.AreEqual(profiles[i].Name, profiles2[i].Name);
                Assert.AreEqual(profiles[i].Active, profiles2[i].Active);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveProfilesTest_TwoActiveProfilesThrows()
        {
            List<Profile> profiles = new List<Profile>();

            for (int i = 0; i < 3; i++)
            {
                Profile p = new Profile("Profile" + i.ToString());
                p.Active = i % 2 == 0;

                profiles.Add(p);
            }

            Data d = new Data();
            d.SaveProfiles(profiles);
        }

        [TestMethod()]
        public void LoadUsersTest()
        {
            Data data = new Data();
            List<User> users = new List<User>();
            Random random = new Random();
            Byte[] name, pass;

            for (int i = 0; i < 10; i++)
            {
                name = new byte[random.Next(255)];
                pass = new byte[random.Next(255)];
                random.NextBytes(name);
                random.NextBytes(pass);
                users.Add(new User(Convert.ToBase64String(name), Convert.ToBase64String(pass)));
            }

            data.SaveUsers(users);

            List<User> usersLoaded = data.LoadUsers();

            Assert.AreEqual(users.Count, usersLoaded.Count);
            for (int i = 0; i < users.Count; i++)
            {
                Assert.AreEqual(users[i], usersLoaded[i]);
            }
        }

        [TestMethod()]
        public void LoadUsersTest_SpecialSymbols()
        {
            Data data = new Data();
            List<User> users = new List<User>();

            users.Add(new User("My name is<>!---Karel\\/?][{}", "@Pass*&^%$#()"));

            data.SaveUsers(users);

            List<User> usersLoaded = data.LoadUsers();

            Assert.AreEqual(users.Count, usersLoaded.Count);
            Assert.AreEqual(users[0], usersLoaded[0]);
        }

        [TestMethod()]
        public void LoadUsersTest_SpecialSymbolsGenerated()
        {
            Data data = new Data();
            List<User> users = new List<User>();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 32; i <= 126; i++)
            {
                stringBuilder.Append((char)i);
            }
            users.Add(new User(stringBuilder.ToString(), "Password"));

            data.SaveUsers(users);

            List<User> usersLoaded = data.LoadUsers();

            Assert.AreEqual(users.Count, usersLoaded.Count);
            Assert.AreEqual(users[0], usersLoaded[0]);
        }
    }
}