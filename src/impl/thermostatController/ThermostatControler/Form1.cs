using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermostatControler
{
    public partial class ThermostatController : Form
    {
        private Presenter m_presenter = new Presenter();

        public ThermostatController()
        {
            InitializeComponent();
            pnlLogin.BringToFront();
            m_presenter.PanelLogin = pnlLogin;
            m_presenter.PanelCreateSchedule = pnlCreateSchedule;
            m_presenter.PanelFindHeaters = pnlFindHeaters;
            m_presenter.PanelBack = pnlBack;
            m_presenter.PanelMenu = pnlMenu;
            m_presenter.PanelAddUser = pnlAddUser;
            m_presenter.PanelHolidaysAndTemp = pnlTempHoli;
            m_presenter.PanelHolidaysBlock = pnlHolidaysBlock;

            m_presenter.SetGraph(pnlGraph, 10, 15);
        }

        // login
        private void btnLoginLogin_Click(object sender, EventArgs e)
        {
            m_presenter.UserLogin(txtLoginName.Text, txtLoginPass.Text);
        }

        private void txtLoginPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                m_presenter.UserLogin(txtLoginName.Text, txtLoginPass.Text);
            }
        }

        // Schedule
        private void pnlGraph_Click(object sender, EventArgs e)
        {
            MouseEventArgs e1 = (MouseEventArgs)e;
            m_presenter.TemperatureGraph.ChangeLine(new Point(e1.X, e1.Y));
        }

        private void btnSchAddNode_Click(object sender, EventArgs e)
        {
            m_presenter.TemperatureGraph.AddNode((int) numSchHour.Value, (int) numSchMin.Value, Decimal.ToDouble(numSchTemp.Value));
        }

        private void btnNewSchNew_Click(object sender, EventArgs e)
        {
            if (radButt1525.Checked)
            {
                m_presenter.SetGraph(pnlGraph, 10, 15);
            }
            else if (radButt1530.Checked)
            {
                m_presenter.SetGraph(pnlGraph, 15, 15);
            }
            else if (radButt1030.Checked)
            {
                m_presenter.SetGraph(pnlGraph, 20, 10);
            }
            else
            {
                throw new KeyNotFoundException("Radio button wasn't checked");
            }
        }

        private void btnNewSchSave_Click(object sender, EventArgs e)
        {
            if (txtNewSchName.Text.Equals(""))
            {
                MessageBox.Show("Please fill the name", "Wrong name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<bool> toAdd = new List<bool>
            {
                chcNewSchDay0.Checked,
                chcNewSchDay1.Checked,
                chcNewSchDay2.Checked,
                chcNewSchDay3.Checked,
                chcNewSchDay4.Checked,
                chcNewSchDay5.Checked,
                chcNewSchDay6.Checked,
            };
            m_presenter.SaveSchedule(txtNewSchName.Text, toAdd);
        }

        private void btnHeatersSaveProfAndSetActive_Click(object sender, EventArgs e)
        {
            if (IsSelectedProfile())
                m_presenter.SaveHeatersAndSetActiveProfile();
        }

        private void btnHeaterSaveProfile_Click(object sender, EventArgs e)
        {
            if (IsSelectedProfile())
                m_presenter.SaveProfile();
        }

        private bool IsSelectedProfile()
        {
            if (comBoxHetersProfile.Text == "")
            {
                MessageBox.Show("Fill in the profile name.", "Wrong profile name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnNewSchLoad_Click(object sender, EventArgs e)
        {
            m_presenter.LoadSchedule();
        }

        // Menu buttons
        private void btnMenuCreatSch_Click(object sender, EventArgs e)
        {
            m_presenter.SetCurrent(m_presenter.PanelCreateSchedule);
        }

        private void btnMenuAssSch_Click(object sender, EventArgs e)
        {
            m_presenter.SetCurrent(m_presenter.PanelFindHeaters);
            m_presenter.DisplayHeaters();
        }

        private void btnMenuHeaters_Click(object sender, EventArgs e)
        {
            m_presenter.SetCurrent(m_presenter.PanelFindHeaters);
            m_presenter.DisplayHeaters();
        }

        private void btnMenuTemp_Click(object sender, EventArgs e)
        {
            m_presenter.SetCurrent(m_presenter.PanelHolidaysAndTemp);
            m_presenter.GetCurrentTemperature();
        }

        private void btnMenuHolidays_Click(object sender, EventArgs e)
        {
            m_presenter.SetCurrent(m_presenter.PanelHolidaysAndTemp);
            m_presenter.GetCurrentTemperature();

            comBoxHolProf.Items.AddRange(m_presenter.GetProfiles().ToArray());
        }

        private void btnMenuAddUser_Click(object sender, EventArgs e)
        {
            m_presenter.SetCurrent(m_presenter.PanelAddUser);
        }

        // Back button
        private void btnBackBack_Click(object sender, EventArgs e)
        {
            m_presenter.SetCurrent(m_presenter.PanelMenu);
        }

        // Refreshing temperature graph
        private void pnlGraph_MouseEnter(object sender, EventArgs e)
        {
            m_presenter.TemperatureGraph.DrawAxis();
            m_presenter.TemperatureGraph.DrawPoints();
        }

        // Create new user
        private void btnUserAdd_Click(object sender, EventArgs e)
        {
            if (!txtUserPass1.Text.Equals(txtUserPass2.Text))
            {
                MessageBox.Show("Passwords don't match", "Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!m_presenter.CreateUser(txtUserName.Text, txtUserPass1.Text))
            {
                MessageBox.Show("User already exists", "Wrong user", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // holidays buttons
        private void btnHolidaysSave_Click(object sender, EventArgs e)
        {
            if (!m_presenter.SetDateOfReturnFromHolidays(dateHolidays.Value, Convert.ToInt32(numHoliTemp.Value)))
            {
                MessageBox.Show("Date can not be earlier than tomorrow", "Wrong date", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHolidaysSaveProfile_Click(object sender, EventArgs e)
        {
            if(!m_presenter.SetDateOfReturnAndProfile(dateHolidays.Value, comBoxHolProf.SelectedItem as Profile))
            {
                MessageBox.Show("Date can not be earlier than tomorrow. And you must select correct profile.", "Wrong date od profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHolidaysNormalSch_Click(object sender, EventArgs e)
        {
            m_presenter.EarlyReturn();
        }

        // radio buttons for changing graph
        private void radButt1525_CheckedChanged(object sender, EventArgs e)
        {
            if (radButt1525.Checked)
            {
                m_presenter.ChangeGraph(pnlGraph, 10, 15);
            }
        }

        private void radButt1530_CheckedChanged(object sender, EventArgs e)
        {
            if (radButt1530.Checked)
            {
                m_presenter.ChangeGraph(pnlGraph, 15, 15);
            }
        }

        private void radButt1030_CheckedChanged(object sender, EventArgs e)
        {
            if (radButt1030.Checked)
            {
                m_presenter.ChangeGraph(pnlGraph, 20, 10);
            }
        }

        // changing profile
        private void cBoxHetersProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_presenter.ProfileChanged();
        }

        private void chcNewSchAll_CheckedChanged(object sender, EventArgs e)
        {
            bool check = chcNewSchAll.Checked;

            chcNewSchDay0.Checked = check;
            chcNewSchDay1.Checked = check;
            chcNewSchDay2.Checked = check;
            chcNewSchDay3.Checked = check;
            chcNewSchDay4.Checked = check;
            chcNewSchDay5.Checked = check;
            chcNewSchDay6.Checked = check;

            chcNewSchAll.CheckedChanged -= chcNewSchAll_CheckedChanged;
            chcNewSchAll.Checked = check;
            chcNewSchAll.CheckedChanged += chcNewSchAll_CheckedChanged;
        }

        private void chcNewSchDay0_CheckedChanged(object sender, EventArgs e)
        {
            chcNewSchAll.CheckedChanged -= chcNewSchAll_CheckedChanged;
            chcNewSchAll.Checked = false;
            chcNewSchAll.CheckedChanged += chcNewSchAll_CheckedChanged;
        }

        // selecting schedule for all heaters
        private void comboBoxHeatersAll_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_presenter.SetScheduleToComBoxes(comboBoxHeatersAll.Text);
        }
    }
}
