using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public static class DictionaryExtension
    {
        private static object m_lock = new object();

        public static void AddThreadSafe<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            lock (m_lock)
            {
                dictionary.Add(key, value);
            }
        }
    }
}
