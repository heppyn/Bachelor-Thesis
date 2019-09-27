using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class Schedule : IEquatable<Schedule>
    {
        public const int NUM_DAYS = 7;

        private List<List<TempNode>> m_tempNodes;
        private String m_name;

        public Schedule(String name)
        {
            m_name = name;
            m_tempNodes = new List<List<TempNode>>();
            for (int i = 0; i < 7; i++)
            {
                m_tempNodes.Add(new List<TempNode>());
            }
        }
        /// <summary>
        /// Adds node, if same node is present the it is replaced
        /// </summary>
        /// <param name="day">Day of week</param>
        /// <param name="node">Node to add</param>
        /// <returns></returns>
        public bool AddNode(int day, TempNode node)
        {
            if (ValidDate(day))
            {
                m_tempNodes.ElementAt(day).Remove(node);
                m_tempNodes.ElementAt(day).Add(node);
                m_tempNodes.ElementAt(day).Sort();
                return true;
            }
            return false;
        }

        public void MergeToMe(Schedule schedule)
        {
            for (int i = 0; i < NUM_DAYS; i++)
            {
                if (schedule.GetDayList(i).Count > 0)
                {
                    this.m_tempNodes.RemoveAt(i);
                    this.m_tempNodes.Insert(i, schedule.GetDayList(i));
                }
            }
        }

        /// <summary>
        /// Removes node if one is present
        /// </summary>
        /// <param name="day">Day of week</param>
        /// <param name="node">Node to delete</param>
        /// <returns></returns>
        public bool RemoveNode(int day, TempNode node)
        {
            if (ValidDate(day))
            {
                m_tempNodes.ElementAt(day).Remove(node);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Removes node if one is present
        /// </summary>
        /// <param name="day">Day of week</param>
        /// <param name="idx">index of node</param>
        /// <returns></returns>
        public bool RemoveNode(int day, int idx)
        {
            if (ValidDate(day))
            {
                m_tempNodes.ElementAt(day).RemoveAt(idx);
                m_tempNodes.ElementAt(day).Sort();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns immutable version of list.
        /// </summary>
        /// <param name="day">0 - 6</param>
        /// <returns></returns>
        public ReadOnlyCollection<TempNode> GetDay(int day)
        {
            if (!ValidDate(day))
                throw new IndexOutOfRangeException();
            return m_tempNodes.ElementAt(day).AsReadOnly();
        }

        public int getNumOfNodes()
        {
            int sum = 0;
            foreach (List<TempNode> l in m_tempNodes)
                sum += l.Count;
            return sum;
        }

        public String GetName()
        {
            return m_name;
        }

        private bool ValidDate(int day)
        {
            return day < NUM_DAYS || day >= 0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Schedule);
        }

        public bool Equals(Schedule other)
        {
            if (other == null || m_name != other.m_name)
                return false;

            for (int i = 0; i < NUM_DAYS; i++)
            {
                if (m_tempNodes.ElementAt(i).Count != other.m_tempNodes.ElementAt(i).Count)
                    return false;
                for (int j = 0; j < NUM_DAYS; j++)
                {
                    try
                    {
                        if (!m_tempNodes.ElementAt(i).ElementAt(j).Equals(other.m_tempNodes.ElementAt(i).ElementAt(j)))
                            return false;
                    }
                    catch { }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 892152130;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<List<TempNode>>>.Default.GetHashCode(m_tempNodes);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_name);
            return hashCode;
        }

        public static bool operator ==(Schedule schedule1, Schedule schedule2)
        {
            return EqualityComparer<Schedule>.Default.Equals(schedule1, schedule2);
        }

        public static bool operator !=(Schedule schedule1, Schedule schedule2)
        {
            return !(schedule1 == schedule2);
        }

        private List<TempNode> GetDayList(int day)
        {
            return m_tempNodes.ElementAt(day);
        }
    }
}
