using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class Profile : IComparable<Profile>, IEquatable<Profile>
    {
        private string m_name;
        private bool m_active = false;
        private SortedDictionary<string, string> m_HeaterIdSchedule;

        public string Name { get => m_name; set => m_name = value; }
        public SortedDictionary<string, string> HeaterIdSchedule { get => m_HeaterIdSchedule; }
        public bool Active { get => m_active; set => m_active = value; }

        /// <summary>
        /// Creates empty instance of Profile.
        /// </summary>
        /// <param name="m_name">Name of profile.</param>
        public Profile(string m_name)
        {
            this.m_name = m_name;
            m_HeaterIdSchedule = new SortedDictionary<string, string>();
        }

        /// <summary>
        /// Adds heater -- schedule to list.
        /// </summary>
        /// <param name="heaterId">Heater id.</param>
        /// <param name="schedule">Schedule name.</param>
        /// <returns>True if pair was added.</returns>
        public bool Add(string heaterId, string schedule)
        {
            if (m_HeaterIdSchedule.ContainsKey(heaterId))
                return false;

            m_HeaterIdSchedule.Add(heaterId, schedule);
            return true;
        }

        /// <summary>
        /// Replaces heaters schedule with new one.
        /// If doesn't heater - schedule is added.
        /// </summary>
        /// <param name="heaterId">Heater id.</param>
        /// <param name="schedule">Schedule name.</param>
        public void Replace(string heaterId, string schedule)
        {
            m_HeaterIdSchedule[heaterId] = schedule;
        }

        /// <summary>
        /// Compares profiles by name.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Profile other)
        {
            return m_name.CompareTo(other.m_name);
        }

        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Profile);
        }

        public bool Equals(Profile other)
        {
            return other != null &&
                   m_name == other.m_name;
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
        }
    }
}
