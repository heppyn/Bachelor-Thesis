using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class SimulatedController : Controller
    {
        private Heaters m_heaters;

        public SimulatedController()
        {
            m_heaters = new Heaters();
            m_data = new Data();
        }

        public override Heaters FindHeaters()
        {
            m_heaters = m_data.LoadHeaters();
            if (m_heaters.GetHeaters().Count < 8)
            {
                // add new heater in starting configuration
                m_heaters.AddIfDoesntExist(new Heater("aa", "", ""));
            }
            return m_heaters;
        }

        public override void SaveHeatersAndActiveProfile(List<Heater> heaters, string profileName)
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
                    continue;
                }
                h.Replace(heater);
            }

            m_data.SaveHeaters(h);
            SaveProfileAsActive(h, profileName);
            List<Schedule> schedules = m_data.LoadSchedules();
        }

        public override void SaveNewHeatersAndNonactiveProfile(List<Heater> heaters, string profileName)
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
                }
            }

            m_data.SaveHeaters(h);
            SaveProfile(h, profileName);
            List<Schedule> schedules = m_data.LoadSchedules();
        }

        public override void SetReturnDateAndTemperature(DateTime date, int temp)
        {
            return;
        }

        public override void EarlyReturn()
        {
            return;
        }

        public override void SetReturnDateAndProfile(DateTime date, Profile profile)
        {
            return;
        }

        public override Dictionary<String, double> GetTemperature()
        {
            Dictionary<String, double> res = new Dictionary<string, double>();
            Random random = new Random();
            m_heaters = m_data.LoadHeaters();

            foreach (Heater h in m_heaters.GetHeaters())
            {
                res.Add(h.Name, 18 + random.NextDouble() * 5);
            }

            return res;
        }
    }
}
