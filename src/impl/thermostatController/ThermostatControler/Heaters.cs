using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class Heaters
    {
        private List<Heater> m_heaters = new List<Heater>();
        private const String startId = "aa";
        private static String m_lastId;

        public static string LastId { get => m_lastId; set => m_lastId = value; }

        public static string StartId => startId;

        public Heaters()
        {
            LastId = startId;
        }

        public void AddHeater(Heater heater)
        {
            m_heaters.Add(heater);
            AddId(heater.Id);
            m_heaters.Sort();
        }

        /// <summary>
        /// Checks if heater with same ID is saved.
        /// If is present pick the one from file.
        /// </summary>
        /// <param name="heater"></param>
        public void AddIfDoesntExist(Heater heater)
        {
            bool found = false;
            foreach (Heater h in m_heaters)
            {
                if (h.Id.Equals(heater.Id))
                {
                    found = true;
                }
            }
            
            if (!found)
            {
                AddHeater(heater);
            }
        }

        /// <summary>
        /// Replaces heater with same id.
        /// </summary>
        /// <param name="heater"></param>
        public void Replace(Heater heater)
        {
            for (int i = 0; i < m_heaters.Count; i++)
            {
                if (m_heaters[i].Id.Equals(heater.Id))
                {
                    m_heaters[i] = heater;
                }
            }
        }

        public void SetHeaters(List<Heater> heaters)
        {
            m_heaters = heaters;
            foreach (Heater h in heaters)
            {
                AddId(h.Id);
            }
            m_heaters.Sort();
        }

        /// <summary>
        /// Return sorted list by heater name.
        /// </summary>
        /// <returns>Return sorted list by heater name</returns>
        public List<Heater> GetHeaters()
        {
            return m_heaters;
        }

        public static String NextId()
        {
            char[] str = LastId.ToCharArray();
            if (str[1] < 'z')
            {
                str[1]++;
            }
            else
            {
                str[0]++;
                str[1] = 'a';
            }
            LastId = new String(str);

            return LastId;
        }

        /// <summary>
        /// Changes id if parametr id is higher
        /// </summary>
        /// <param name="id"></param>
        private void AddId(String id)
        {
            if (LastId[0] < id[0])
            {
                LastId = id;
                return;
            }
            if (LastId[1] < id[1] && LastId[0] == id[0])
            {
                LastId = id;
                return;
            }
            return;
        }
    }
}
