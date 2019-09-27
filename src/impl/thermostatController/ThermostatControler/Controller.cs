using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class Controller : IController
    {
        protected IData m_data;
        protected WifiDevices m_wifiDevices;
        protected Users m_users;

        public Controller()
        {
            this.m_data = new Data();
            m_wifiDevices = new WifiDevices();
            m_users = new Users();
            //PrepareHeaters();
        }

        /// <summary>
        /// Checks if user is admin
        /// </summary>
        /// <returns>True if user is admin</returns>
        public bool IsUserAdmin()
        {
            return m_users.IsUserAdmin();
        }

        /// <summary>
        /// Checks if user exists
        /// </summary>
        /// <param name="name">Name of user</param>
        /// <param name="pass">Password</param>
        /// <returns>Enum with response</returns>
        public Users.Response Login(string name, string pass)
        {
            Users.Response res = m_users.UserExists(new User(name, pass));

            if (res != Users.Response.FAIL)
            {
                m_users.CurrentUser = new User(name, pass);
            }

            return res;
        }

        /// <summary>
        /// Creates user.
        /// </summary>
        /// <param name="name">Name of user</param>
        /// <param name="pass">Password</param>
        /// <returns>True if user was created</returns>
        public bool CreateUser(string name, string pass)
        {
            return m_users.CreateUser(new User(name, pass));
        }

        /// <summary>
        /// Checks heaters on wifi.
        /// Creates new device, if one doesnt exist.
        /// </summary>
        /// <returns>Found heaters</returns>
        public virtual Heaters FindHeaters()
        {
            Heaters heaters = m_data.LoadHeaters();

            Dictionary<IPAddress, String> found = m_wifiDevices.FindHeaters();
            foreach (String s in found.Values)
            {
                heaters.AddIfDoesntExist(new Heater(s, "", ""));
            }

            return heaters;
        }

        /// <summary>
        /// Starts communication with heaters.
        /// </summary>
        public void PrepareHeaters()
        {
            Thread t = new Thread(delegate ()
            {
                m_wifiDevices.FindHeaters();
            });
            t.Start();
        }

        public void SaveSchedule(Schedule schedule)
        {
            List<Schedule> schedules = m_data.LoadSchedules();
            bool exists = false;

            foreach (Schedule s in schedules)
            {
                if (s.GetName() == schedule.GetName())
                {
                    s.MergeToMe(schedule);
                    exists = true;
                }
            }
            if (!exists)
            {
                schedules.Add(schedule);
            }
            m_data.SaveSchedules(schedules);
        }

        public List<Schedule> GetSchedules()
        {
            return m_data.LoadSchedules();
        }

        /// <summary>
        /// Saves heaters and sends new schedules.
        /// Saves profile as active to file.
        /// </summary>
        /// <param name="heaters">Must contain max one new heater.</param>
        public virtual void SaveHeatersAndActiveProfile(List<Heater> heaters, string profileName)
        {
            Heaters h = m_data.LoadHeaters();
            List<Heater> changed = new List<Heater>();

            foreach (Heater heater in heaters)
            {
                if (!h.GetHeaters().Contains(heater))
                    changed.Add(heater);
            }

            foreach (Heater heater in changed)
            {
                if (heater.Id.Equals(Heaters.StartId))
                {
                    heater.Id = Heaters.NextId();
                    h.AddHeater(heater);
                    m_wifiDevices.ChangeIdfromStartId(Heaters.LastId);
                    continue;
                }
                h.Replace(heater);
            }

            m_data.SaveHeaters(h);
            SaveProfileAsActive(h, profileName);
            List<Schedule> schedules = m_data.LoadSchedules();
            m_wifiDevices.SendSchedules(changed, schedules);
        }

        /// <summary>
        /// Changes new heaters id and saves schedule.
        /// </summary>
        /// <param name="heaters"></param>
        /// <param name="profileName"></param>
        public virtual void SaveNewHeatersAndNonactiveProfile(List<Heater> heaters, string profileName)
        {
            Heaters h = m_data.LoadHeaters();
            List<Heater> newHeater = new List<Heater>();

            foreach (Heater heater in heaters)
            {
                if (heater.Id.Equals(Heaters.StartId))
                {
                    heater.Id = Heaters.NextId();
                    h.AddHeater(heater);
                    newHeater.Add(heater);
                    m_wifiDevices.ChangeIdfromStartId(Heaters.LastId);
                }
            }

            m_data.SaveHeaters(h);
            SaveProfile(h, profileName);
            List<Schedule> schedules = m_data.LoadSchedules();
            m_wifiDevices.SendSchedules(newHeater, schedules);
        }

        /// <summary>
        /// Calls method in WifiDevices to send date and temperature
        /// </summary>
        /// <param name="date">Return date</param>
        /// <param name="temp">Desired temperature</param>
        public virtual void SetReturnDateAndTemperature(DateTime date, int temp)
        {
            m_wifiDevices.SendReturnDateAndTemperature(date, temp);
        }

        /// <summary>
        /// Sets return date to earlier date.
        /// Heaters will work bz normal schedule.
        /// </summary>
        public virtual void EarlyReturn()
        {
            SetReturnDateAndTemperature(new DateTime(1900, 1, 1), 6);
        }

        /// <summary>
        /// Calls method in WifiDevices to get temperature
        /// </summary>
        /// <returns>Dictionary key = heater, value = temperature</returns>
        public virtual Dictionary<String, double> GetTemperature()
        {
            return m_wifiDevices.GetTemperature();
        }

        /// <summary>
        /// Saves profile to file, replaces profile with same name.
        /// </summary>
        /// <param name="profile">Profile to save.</param>
        protected void SaveProfile(Profile profile)
        {
            List<Profile> profiles = m_data.LoadProfiles();
            SaveProfileToFile(profile, profiles);
        }

        protected void SaveProfileAsActive(Profile profile)
        {
            List<Profile> profiles = m_data.LoadProfiles();
            profile.Active = true;

            foreach (Profile p in profiles)
            {
                p.Active = false;
            }

            SaveProfileToFile(profile, profiles);
        }

        /// <summary>
        /// Saves profile to file, replaces profile with same name.
        /// </summary>
        /// <param name="heaters">Heaters in profile.</param>
        /// <param name="name">Profile name.</param>
        protected void SaveProfile(Heaters heaters, string name)
        {
            Profile profile = new Profile(name);
            foreach(Heater h in heaters.GetHeaters())
            {
                profile.Add(h.Id, h.Schedule);
            }

            SaveProfile(profile);
        }

        protected void SaveProfileAsActive(Heaters heaters, string name)
        {
            Profile profile = new Profile(name);
            profile.Active = true;
            foreach (Heater h in heaters.GetHeaters())
            {
                profile.Add(h.Id, h.Schedule);
            }

            SaveProfileAsActive(profile);
        }

        /// <summary>
        /// Loads profiles from file.
        /// </summary>
        /// <returns></returns>
        public List<Profile> GetProfiles()
        {
            return m_data.LoadProfiles();
        }

        /// <summary>
        /// Finds profile in list. Replaces profile with the same name.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="profiles"></param>
        protected void SaveProfileToFile(Profile profile, List<Profile> profiles)
        {
            bool found = false;
            foreach (Profile p in profiles)
            {
                if (p.Name == profile.Name)
                {
                    if(!profile.Active)
                        profile.Active = p.Active;
                    profiles.Remove(p);
                    profiles.Add(profile);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                profiles.Add(profile);
            }

            m_data.SaveProfiles(profiles);
        }

        public virtual void SetReturnDateAndProfile(DateTime date, Profile profile)
        {
            Heaters h = m_data.LoadHeaters();
            List<Schedule> schedules = m_data.LoadSchedules();

            m_wifiDevices.SendAlternativeSchedules(h.GetHeaters(), schedules);

            // sends invalid temperature -- alternative schedule is used
            m_wifiDevices.SendReturnDateAndTemperature(date, 0);


        }
    }
}
