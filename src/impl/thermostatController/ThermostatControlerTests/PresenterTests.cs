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
    public class PresenterTests
    {
        [TestMethod()]
        public void SetDateOfReturnFromHolidaysTest()
        {
            Presenter p = new Presenter();
            DateTime early = new DateTime(2010, 1, 1);
            DateTime ok = new DateTime(3000, 7, 4);

            Assert.IsFalse(p.SetDateOfReturnFromHolidays(early, 4));
            Assert.IsTrue(p.SetDateOfReturnFromHolidays(ok, 6));
        }
    }
}