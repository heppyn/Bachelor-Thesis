using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermostatControler
{
    public class TemperaturegraphSimple : TemperatureGraph
    {
        public TemperaturegraphSimple(Panel panel, int x, int y, int tempStart, int padding = 25) : base(panel, x, y, tempStart, padding)
        {
        }

        protected override void ReDraw()
        {
            Clear();
            DrawAxis();
            DrawPoints();
        }

        public override void AddLabels()
        {
            AddLabels(START_X, 7, m_tempStart, 10);
        }
    }
}
