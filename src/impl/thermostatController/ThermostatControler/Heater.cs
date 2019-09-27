using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class Heater : IComparable<Heater>, IEquatable<Heater>
    {
        private String m_id;
        private String m_name;
        private String m_schedule;

        public Heater(string id, string name, string schedule)
        {
            Id = id;
            Name = name;
            Schedule = schedule;
        }

        public string Id { get => m_id; set => m_id = value; }
        public string Name { get => m_name; set => m_name = value; }
        public string Schedule { get => m_schedule; set => m_schedule = value; }

        /// <summary>
        /// Compares by name.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Heater other)
        {
            if (other == null)
                return 1;
            return m_name.CompareTo(other.m_name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Heater);
        }

        public bool Equals(Heater other)
        {
            return other != null &&
                   m_id == other.m_id &&
                   m_name == other.m_name &&
                   m_schedule == other.m_schedule;
        }

        public override int GetHashCode()
        {
            var hashCode = -1722864798;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_schedule);
            return hashCode;
        }

        public static bool operator ==(Heater heater1, Heater heater2)
        {
            return EqualityComparer<Heater>.Default.Equals(heater1, heater2);
        }

        public static bool operator !=(Heater heater1, Heater heater2)
        {
            return !(heater1 == heater2);
        }
    }
}
