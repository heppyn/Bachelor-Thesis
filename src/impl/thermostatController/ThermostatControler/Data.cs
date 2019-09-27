using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ThermostatControler
{
    /// <summary>
    /// class for storing data.
    /// saving deletes the old file.
    /// </summary>
    public class Data : IData
    {
        private const String m_fileUsers = "pass.xml";
        private const String m_fileSchedules = "schedules.xml";
        private const String m_fileHeaters = "heaters.xml";
        private const String m_fileProfiles = "profiles.xml";

        /// <summary>
        /// Saves list of schedules.
        /// Overrides previously existing file.
        /// </summary>
        /// <param name="schedules">list of schedules to be saved</param>
        public void SaveSchedules(List<Schedule> schedules)
        {
            XDocument doc = new XDocument();
            XElement xschedules = new XElement("Schedules");

            if (schedules != null)
            {
                foreach (Schedule s in schedules)
                {
                    XElement xsch = new XElement("Schedule");
                    xsch.Add(new XElement("Name", s.GetName()));

                    for (int i = 0; i < Schedule.NUM_DAYS; i++)
                    {
                        XElement xday = new XElement("Day", new XAttribute("Number", i));
                        foreach (TempNode t in s.GetDay(i))
                        {
                            XElement xnode = new XElement("Node");
                            xnode.Add(new XElement("Time", t.GetTime()));
                            xnode.Add(new XElement("Temperature", t.GetTemperature()));
                            xday.Add(xnode);
                        }
                        xsch.Add(xday);
                    }
                    xschedules.Add(xsch);
                }
            }
            doc.Add(xschedules);
            doc.Save(m_fileSchedules);
        }

        /// <summary>
        /// Returns list of schedules saved in file.
        /// If file doesn't exist, returns empty list.
        /// Can throw if file is in wrong format.
        /// </summary>
        /// <returns>Loaded list of schedules</returns>
        public List<Schedule> LoadSchedules()
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(m_fileSchedules);
            }
            catch
            {
                return new List<Schedule>();
            }
            
            List<Schedule> res = new List<Schedule>();
            try
            {
                XElement xschedules;
                xschedules = doc.Element("Schedules");
                foreach (XElement xsch in xschedules.Elements("Schedule"))
                {
                    Schedule sch = new Schedule(xsch.Element("Name").Value);

                    foreach (XElement xday in xsch.Elements("Day"))
                    {
                        foreach (XElement xnode in xday.Elements("Node"))
                        {
                            sch.AddNode(Convert.ToInt32(xday.FirstAttribute.Value),
                                        new TempNode(Convert.ToByte(xnode.Element("Time").Value),
                                                     Convert.ToByte(xnode.Element("Temperature").Value)));
                        }
                    }
                    res.Add(sch);
                }
            }
            catch
            {
                throw new InvalidOperationException(m_fileUsers + " is in wrong format");
            }
            return res;
        }

        /// <summary>
        /// Saves list of users to file.
        /// </summary>
        /// <param name="users">User to be saved</param>
        public void SaveUsers(List<User> users)
        {
            XDocument doc = new XDocument();
            XElement xusers = new XElement("Users");
            if (users != null)
            {
                foreach (User u in users)
                {
                    XElement xuser = new XElement("User");
                    xuser.Add(new XElement("Name", u.Name));
                    xuser.Add(new XElement("Pass", u.Pass));
                    xusers.Add(xuser);
                }
            }

            doc.Add(xusers);
            doc.Save(m_fileUsers);
        }

        /// <summary>
        /// Loads users from file.
        /// </summary>
        /// <returns>List of loaded users</returns>
        public List<User> LoadUsers()
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(m_fileUsers);
            }
            catch
            {
                throw new NullReferenceException(m_fileUsers + "doesn't exist");
            }

            List<User> res = new List<User>();

            XElement xusers = doc.Element("Users");
            foreach (XElement xuser in xusers.Elements("User"))
            {
                res.Add(
                    new User(xuser.Element("Name").Value, xuser.Element("Pass").Value)
                    );
            }

            return res;
        }

        public void SaveHeaters(Heaters heaters)
        {
            XDocument doc = new XDocument();
            XElement xheaters = new XElement("Heaters");
            xheaters.Add(new XElement("LastId", Heaters.LastId));
            if (heaters != null)
            {
                foreach (Heater h in heaters.GetHeaters())
                {
                    XElement xheater = new XElement("Heater");
                    xheater.Add(new XElement("Id", h.Id));
                    xheater.Add(new XElement("Name", h.Name));
                    xheater.Add(new XElement("Schedule", h.Schedule));
                    xheaters.Add(xheater);
                }
            }

            doc.Add(xheaters);
            doc.Save(m_fileHeaters);
        }

        public Heaters LoadHeaters()
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(m_fileHeaters);
            }
            catch
            {
                return new Heaters();
            }

            List<Heater> res = new List<Heater>();
            Heaters heaters = new Heaters();

            XElement xheaters = doc.Element("Heaters");
            Heaters.LastId = xheaters.Element("LastId").Value;

            foreach (XElement xheater in xheaters.Elements("Heater"))
            {
                res.Add(
                    new Heater(xheater.Element("Id").Value, xheater.Element("Name").Value, xheater.Element("Schedule").Value)
                    );
            }

            heaters.SetHeaters(res);
            return heaters;
        }

        public void SaveProfiles(List<Profile> profiles)
        {
            XDocument doc = new XDocument();
            XElement xcurrProf = new XElement("Active");
            XElement xprofiles = new XElement("Profiles");

            if (profiles != null)
            {
                foreach (Profile p in profiles)
                {
                    if (p.Active)
                    {
                        if (xcurrProf.Value != "")
                        {
                            throw new ArgumentException("There must be only one active profile");
                        }
                        xcurrProf.SetValue(p.Name);
                    }
                    XElement xprofile = new XElement("Profile");
                    xprofile.Add(new XElement("Name", p.Name));

                    foreach (var pair in p.HeaterIdSchedule)
                    {
                        XElement xpair = new XElement("Pair");
                        xpair.Add(new XElement("Heater", pair.Key));
                        xpair.Add(new XElement("Schedule", pair.Value));
                        xprofile.Add(xpair);
                    }

                    xprofiles.Add(xprofile);
                }
            }

            xprofiles.Add(xcurrProf);
            doc.Add(xprofiles);
            doc.Save(m_fileProfiles);
        }

        public List<Profile> LoadProfiles()
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(m_fileProfiles);
            }
            catch
            {
                return new List<Profile>();
            }

            List<Profile> res = new List<Profile>();

            XElement xprofiles = doc.Element("Profiles");
            String active = xprofiles.Element("Active").Value;

            foreach (XElement xprofile in xprofiles.Elements("Profile"))
            {
                Profile profile = new Profile(xprofile.Element("Name").Value);
                if (profile.Name.Equals(active))
                {
                    profile.Active = true;
                }

                foreach (XElement xpair in xprofile.Elements("Pair"))
                {
                    profile.Add(xpair.Element("Heater").Value, xpair.Element("Schedule").Value);
                }

                res.Add(profile);
            }
            
            return res;
        }
    }
}
