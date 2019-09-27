using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThermostatControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler.Tests
{
    [TestClass()]
    public class ScheduleTests
    {
        [TestMethod()]
        public void EqualsTest_1()
        {
            Schedule s1 = new Schedule("Kitchen");
            Schedule s2 = new Schedule("Kitchen");

            Assert.AreEqual(s1, s2);
        }

        [TestMethod()]
        public void EqualsTest_2()
        {
            List<TempNode> nodes = Generator.GetNodes(10);
            Schedule s1 = new Schedule("Kitchen");
            Schedule s2 = new Schedule("Kitchen");

            foreach (TempNode n in nodes)
            {
                s1.AddNode(1, n);
                s2.AddNode(1, n);
            }

            Assert.AreEqual(s1, s2);
        }

        [TestMethod()]
        public void SortingTest()
        {
            List<TempNode> nodes = Generator.GetNodes(30);
            Schedule s1 = new Schedule("Kitchen");
            Schedule s2 = new Schedule("Kitchen");

            s1.AddNode(0, new TempNode(100, 100));
            s1.AddNode(0, new TempNode(50, 50));

            s2.AddNode(0, new TempNode(50, 50));
            s2.AddNode(0, new TempNode(100, 100));

            Assert.AreEqual(s1, s2);

            foreach (TempNode n in nodes)
            {
                s1.AddNode(1, n);
            }

            nodes.Reverse();
            foreach (TempNode n in nodes)
            {
                s2.AddNode(1, n);
            }

            Assert.AreEqual(s1, s2);
        }

        [TestMethod()]
        public void AddSameTest()
        {
            List<TempNode> nodes = Generator.GetNodes(10);
            Schedule s1 = new Schedule("Kitchen");
            Schedule s2 = new Schedule("Kitchen");

            foreach (TempNode n in nodes)
            {
                s1.AddNode(1, n);
                s1.AddNode(1, n);
                s2.AddNode(1, n);
            }

            Assert.AreEqual(s1, s2);
        }

        [TestMethod()]
        public void RemoveNodeTest()
        {
            List<TempNode> nodes = Generator.GetNodes(10);
            Schedule s1 = new Schedule("Kitchen");
            Schedule s2 = new Schedule("Kitchen");

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                s1.AddNode(1, nodes.ElementAt(i));
                s2.AddNode(1, nodes.ElementAt(i));
            }

            s1.AddNode(1, nodes.ElementAt(nodes.Count - 1));
            s1.RemoveNode(1, nodes.ElementAt(nodes.Count - 1));

            Assert.AreEqual(s1, s2);
        }

        [TestMethod()]
        public void getNumOfNodes()
        {
            List<TempNode> nodes = Generator.GetNodes(10);
            Schedule s1 = new Schedule("Kitchen");
            int cnt = 0;

            foreach (TempNode n in nodes)
            {
                if (s1.AddNode(1, n))
                {
                    cnt++;
                }
            }

            Assert.IsTrue(s1.getNumOfNodes() == cnt);
        }

        [TestMethod()]
        public void getNumOfNodes_OneNode()
        {
            Schedule s1 = new Schedule("Kitchen");

            s1.AddNode(6, new TempNode(20.0, 20.0));

            Assert.IsTrue(s1.getNumOfNodes() == 1);
        }

        [TestMethod()]
        public void MergeToMeTest()
        {
            List<TempNode> nodes = Generator.GetNodes(10);
            Schedule s1 = new Schedule("Kitchen");
            Schedule s2 = new Schedule("Kitchen");
            Schedule s3 = new Schedule("Kitchen");

            foreach (TempNode n in nodes)
            {
                s1.AddNode(1, n);
                s1.AddNode(2, n);
            }
            foreach (TempNode n in nodes)
            {
                s2.AddNode(1, n);
            }
            foreach (TempNode n in nodes)
            {
                s3.AddNode(2, n);
            }

            s2.MergeToMe(s3);

            Assert.AreEqual(s1, s2);
            Assert.AreNotEqual(s2, s3);
        }
    }
}