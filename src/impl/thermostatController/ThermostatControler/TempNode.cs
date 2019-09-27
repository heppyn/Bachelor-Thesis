using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class TempNode : IEquatable<TempNode>, IComparable<TempNode>
    {
        private const byte MAX_TEMPERATURE = 200;
        private const byte MAX_TIME = 235;

        private byte m_time;
        private byte m_temperature;
        /// <summary>
        /// Checks if values are in boundary
        /// </summary>
        /// <param name="time">Time value</param>
        /// <param name="temp">Temperature value</param>
        public TempNode(byte time, byte temp)
        {
            Initialize(time, temp);
        }
        /// <summary>
        /// Checks if values are in boundary
        /// </summary>
        /// <param name="time">Time value</param>
        /// <param name="temp">Temperature value</param>
        public TempNode(double time, double temp)
        {
            if (time < 0.0 || time > 23.5 || temp < 10.0 || temp > 30.0)
                throw new ArgumentOutOfRangeException();
            Initialize(Convert.ToByte(Math.Round(time, 1) * 10), Convert.ToByte(Math.Round(temp, 1) * 10 - 100));
        }

        public TempNode(TempNode other)
        {
            m_time = other.m_time;
            m_temperature = other.m_temperature;
        }
        
        private void Initialize(byte time, byte temp)
        {
            if (time > MAX_TIME || temp > MAX_TEMPERATURE)
                throw new ArgumentOutOfRangeException();

            double t = Convert.ToDouble(time) / 10;
            if (t - Math.Floor(t) > 0.5)
                throw new ArgumentOutOfRangeException("Time must be in range x.0 to x.5");

            m_time = time;
            m_temperature = temp;
        }

        public byte GetTime()
        {
            return m_time;
        }

        public byte GetNumOfMinutes()
        {
            return (byte) ((m_time / 10) * 6 + (m_time % 10));
        }

        public byte GetTemperature()
        {
            return m_temperature;
        }

        public int GetTimeHours()
        {
            return m_time / 10;
        }

        public int GetTimeMinutes()
        {
            return (m_time % 10) * 10;
        }

        public double GetTemperatureDouble()
        {
            return (m_temperature + 100) / 10.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return Equals(obj as TempNode);
        }

        public bool Equals(TempNode other)
        {
            return other != null &&
                   m_time == other.m_time;
        }

        public override int GetHashCode()
        {
            return 1980952868 + m_time.GetHashCode();
        }

        public int CompareTo(TempNode other)
        {
            if (other == null)
                return 1;
            return m_time.CompareTo(other.m_time);
        }

        public static bool operator ==(TempNode a, TempNode b)
        {
            if (((object)a) == null || ((object)b) == null)
                return Object.Equals(a, b);

            return a.Equals(b);
        }

        public static bool operator !=(TempNode a, TempNode b)
        {
            return !(a == b);
        }

        public static bool operator <(TempNode a, TempNode b)
        {
            if (((object)a) == null || ((object)b) == null)
                return false;

            return a.m_time < b.m_time;
        }

        public static bool operator >(TempNode a, TempNode b)
        {
            return (b < a) && (b != a);
        }

        public static bool operator <=(TempNode a, TempNode b)
        {
            return a < b || a == b;
        }

        public static bool operator >=(TempNode a, TempNode b)
        {
            return a > b || a == b;
        }
    }
}
