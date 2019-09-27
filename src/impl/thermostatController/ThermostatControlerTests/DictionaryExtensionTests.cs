using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThermostatControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThermostatControler.Tests
{
    [TestClass()]
    public class DictionaryExtensionTests
    {
        [TestMethod()]
        public void AddThreadSafeTest_Race()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            Thread t1 = new Thread(delegate ()
            {
                try
                {
                    dict.AddThreadSafe(1, "First");
                }
                catch
                {
                    return;
                }
            });
            t1.Start();

            Thread t2 = new Thread(delegate ()
            {
                Thread.Sleep(100);
                try
                {
                    dict.AddThreadSafe(1, "Second");
                }
                catch
                {
                    return;
                }
            });
            t2.Start();

            t1.Join();
            t2.Join();

            Assert.IsTrue(dict.ContainsValue("First"));
            Assert.IsFalse(dict.ContainsValue("Second"));
        }

        [TestMethod()]
        public void AddThreadSafeTest_SimpleAdd()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            Thread t1 = new Thread(delegate ()
            {
                dict.AddThreadSafe(1, "First");
            });
            
            Thread t2 = new Thread(delegate ()
            {
                dict.AddThreadSafe(2, "Second");
            });
            
            Thread t3 = new Thread(delegate ()
            {
                dict.AddThreadSafe(3, "Third");
            });

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            Assert.IsTrue(dict.ContainsValue("First"));
            Assert.IsTrue(dict.ContainsValue("Second"));
            Assert.IsTrue(dict.ContainsValue("Third"));
        }
    }
}