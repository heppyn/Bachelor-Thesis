using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ThermostatControler;

namespace ThermostatControler.Tests
{
    class Generator
    {
        public static Schedule GetSchedule(int numOfNodes, String name)
        {
            Random rand = new Random();
            Schedule sch = new Schedule(name);
            for (int i = 0; i < Schedule.NUM_DAYS; i++)
            {
                for (int j = 0; j < numOfNodes; j++)
                {
                    sch.AddNode(i, GetNode(rand.Next()));
                }
            }
            return sch;
        }

        public static Schedule GetSchedule(int numOfNodes)
        {
            Random rand = new Random();
            byte[] name = new byte[10];
            rand.NextBytes(name);
            return GetSchedule(numOfNodes, Convert.ToBase64String(name));
        }

        public static List<TempNode> GetNodes(int numOfNodes)
        {
            Random rand = new Random();
            List<TempNode> res = new List<TempNode>();
            for (int i = 0; i < numOfNodes; i++)
            {
                res.Add(GetNode(rand.Next()));
            }
            return res;
        }

        public static TempNode GetNode(int seed)
        {
            Random rand = new Random(seed);
            TempNode n = new TempNode(100, 100);
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    n = new TempNode(rand.NextDouble() * 24, rand.NextDouble() * 20 + 10);
                }
                catch
                {
                    continue;
                }
                return n;
            }
            return n;
        }

    }
}
