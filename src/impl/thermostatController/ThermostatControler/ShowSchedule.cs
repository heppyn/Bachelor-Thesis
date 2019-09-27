using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermostatControler
{
    public partial class ShowSchedule : Form
    {
        private Presenter m_presenter = null;
        private List<Schedule> m_schedules;
        private List<String> m_names;
        private TemperaturegraphSimple[] m_tempGraphs = new TemperaturegraphSimple[7];

        public ShowSchedule(Schedule schedule)
        {
            this.Show();
            InitializeComponent();
            lblPickDay.Hide();
            DrawSchedule(schedule);
        }

        public ShowSchedule(Presenter presenter)
        {
            this.Show();
            InitializeComponent();
            // it doesn't show properly without this line -- you have to hide and then show the label
            lblPickDay.Hide();
            m_presenter = presenter;
            SetContentForComboBox();
            lblPickDay.Show();
            m_presenter.UncheckDaysFromGUI();
        }

        /// <summary>
        /// Prepares schedule names to show.
        /// </summary>
        private void SetContentForComboBox()
        {
            m_schedules = m_presenter.GetSchedules();
            m_names = m_presenter.GetScheduleNames(m_schedules);

            cBoxSchedules.Items.AddRange(m_names.ToArray());
            if (m_names.Count > 0)
            {
                cBoxSchedules.Text = m_names.First();
            }
        }

        private void DrawSchedule(Schedule schedule)
        {
            cBoxSchedules.Text = schedule.GetName();
            cBoxSchedules.Items.Add(schedule.GetName());
            m_schedules = new List<Schedule>
            {
                schedule
            };
            ScheduleSelected(schedule);
        }

        private void ScheduleSelected(Schedule schedule)
        {
            m_tempGraphs[0] = new TemperaturegraphSimple(pnlMon, 24, 20, 10, 0);
            m_tempGraphs[1] = new TemperaturegraphSimple(pnlTue, 24, 20, 10, 0);
            m_tempGraphs[2] = new TemperaturegraphSimple(pnlWed, 24, 20, 10, 0);
            m_tempGraphs[3] = new TemperaturegraphSimple(pnlThu, 24, 20, 10, 0);
            m_tempGraphs[4] = new TemperaturegraphSimple(pnlfri, 24, 20, 10, 0);
            m_tempGraphs[5] = new TemperaturegraphSimple(pnlSat, 24, 20, 10, 0);
            m_tempGraphs[6] = new TemperaturegraphSimple(pnlSun, 24, 20, 10, 0);

            for (int i = 0; i < 7; i++)
            {
                m_tempGraphs[i].ImportScheduleDay(schedule, i);
            }
        }

        private void cBoxSchedules_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Schedule s in m_schedules)
            {
                if (cBoxSchedules.Text == s.GetName())
                {
                    ScheduleSelected(s);
                    break;
                }
            }
        }

        private void pnlMon_Click(object sender, EventArgs e)
        {
            DaySelected(0);
        }

        private void pnlTue_Click(object sender, EventArgs e)
        {
            DaySelected(1);
        }

        private void pnlWed_Click(object sender, EventArgs e)
        {
            DaySelected(2);
        }

        private void pnlThu_Click(object sender, EventArgs e)
        {
            DaySelected(3);
        }

        private void pnlfri_Click(object sender, EventArgs e)
        {
            DaySelected(4);
        }

        private void pnlSat_Click(object sender, EventArgs e)
        {
            DaySelected(5);
        }

        private void pnlSun_Click(object sender, EventArgs e)
        {
            DaySelected(6);
        }

        /// <summary>
        /// Sends request to import schedule to graph.
        /// </summary>
        /// <param name="day">Select day to import 0--6.</param>
        private void DaySelected(int day)
        {
            if (m_presenter != null && m_names.Contains(cBoxSchedules.Text))
            {
                m_presenter.ImportScheduleToGraph(m_schedules.Find(x => x.GetName() == cBoxSchedules.Text), day);
            }
        }
    }
}
