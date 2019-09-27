using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermostatControler
{
    public class Presenter
    {
        private Panel m_panelLogin;
        private Panel m_panelFindHeaters;
        private Panel m_panelCreateSchedule;
        private Panel m_panelAssignSchedule;
        private Panel m_panelHolidaysAndTemp;
        private Panel m_panelAddUser;
        private Panel m_panelBack;
        private Panel m_panelMenu;
        private Panel m_panelHolidaysBlock;

        private Panel m_current;

        //private IController m_controller = new Controller();
        // controller for testing purposes
        private IController m_controller = new SimulatedController();
        private TemperatureGraph m_temperatureGraph;

        public Panel PanelLogin { get => m_panelLogin; set => m_panelLogin = value; }
        public Panel PanelFindHeaters { get => m_panelFindHeaters; set => m_panelFindHeaters = value; }
        public Panel PanelCreateSchedule { get => m_panelCreateSchedule; set => m_panelCreateSchedule = value; }
        public Panel PanelAssignSchedule { get => m_panelAssignSchedule; set => m_panelAssignSchedule = value; }
        public Panel PanelHolidaysAndTemp { get => m_panelHolidaysAndTemp; set => m_panelHolidaysAndTemp = value; }
        public Panel PanelAddUser { get => m_panelAddUser; set => m_panelAddUser = value; }
        internal TemperatureGraph TemperatureGraph { get => m_temperatureGraph; }
        public Panel PanelBack { get => m_panelBack; set => m_panelBack = value; }
        public Panel PanelMenu { get => m_panelMenu; set => m_panelMenu = value; }
        public Panel PanelHolidaysBlock { get => m_panelHolidaysBlock; set => m_panelHolidaysBlock = value; }

        public Presenter()
        {
            m_current = PanelLogin;
        }

        public void UserLogin(String name, String pass)
        {
            switch (m_controller.Login(name, pass))
            {
                case Users.Response.OK:
                    break;
                case Users.Response.FAIL: MessageBox.Show("Wrong name or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                case Users.Response.DEFAULT: EditUser(name);
                    break;

                default: throw new InvalidOperationException();
            }
            SetCurrent(PanelMenu);

            // disable all but temperature button for not admin users
            if (!m_controller.IsUserAdmin())
            {
                foreach (Button b in PanelMenu.Controls.OfType<Button>())
                {
                    if (b.Name != "btnMenuTemp")
                    {
                        b.Enabled = false;
                    }
                }
            }
        }

        public bool CreateUser(String name, String pass)
        {
            return m_controller.CreateUser(name, pass);
        }

        /// <summary>
        /// Shows dialog for changing password
        /// </summary>
        /// <param name="name"></param>
        public void EditUser(String name)
        {
            ChangePassword changePassword = new ChangePassword(this, name);
            changePassword.ShowDialog();
        }

        /// <summary>
        /// Creates windows, for picking schedule.
        /// </summary>
        public void LoadSchedule()
        {
            ShowSchedule showSchedule = new ShowSchedule(this);
        }

        public void ImportScheduleToGraph(Schedule schedule, int day)
        {
            m_temperatureGraph.ImportScheduleDay(schedule, day);
            PanelCreateSchedule.Controls.Find("txtNewSchName", false)[0].Text = schedule.GetName();

            CheckBox checkBox = PanelCreateSchedule.Controls.Find("chcNewSchDay" + day.ToString(), false)[0] as CheckBox;
            checkBox.Checked = !checkBox.Checked;
        }

        /// <summary>
        /// Goes thru all checkboxes in create schedule menu and unchecks them.
        /// </summary>
        public void UncheckDaysFromGUI()
        {
            for (int i = 0; i < 7; i++)
            {
                (PanelCreateSchedule.Controls.Find("chcNewSchDay" + i.ToString(), false)[0] as CheckBox).Checked = false;
            }
        }

        /// <summary>
        /// Creates temperature graph.
        /// </summary>
        /// <param name="panel">Panel to create graph on</param>
        public void SetGraph(Panel panel, int y, int tempStart)
        {
            m_temperatureGraph = new TemperatureGraph(panel, 24, y, tempStart);
            TemperatureGraph.Initialize();
        }

        /// <summary>
        /// Changes graph to have differnet y axis.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="y"></param>
        /// <param name="tempStart"></param>
        public void ChangeGraph(Panel panel, int y, int tempStart)
        {
            m_temperatureGraph = new TemperatureGraph(panel, 24, y, tempStart, m_temperatureGraph.GetPoints());
            TemperatureGraph.Initialize();
        }

        public void SetCurrent(Panel panel)
        {
            // Only admin acces
            if (!m_controller.IsUserAdmin() && IsAdminPanel(panel))
            {
                return;
            }
           
            // Delete controls from Temperature and Device panel
            //if (m_current == PanelHolidaysAndTemp || m_current == PanelFindHeaters)
            //{
            //    RemoveAddedControlsFrom(m_current);
            //}

            m_current = panel;
            // Displays back button
            if (m_current != PanelMenu)
            {
                m_panelBack.BringToFront();
            }
            m_current.BringToFront();

            // block holidays menu
            if (!m_controller.IsUserAdmin() && panel == PanelHolidaysAndTemp)
            {
                PanelHolidaysBlock.BringToFront();
            }
        }

        /// <summary>
        /// Calls controller to save schedule
        /// </summary>
        /// <param name="name">Name of schedule</param>
        /// <param name="toAdd">List of days to which schedule belongs</param>
        public void SaveSchedule(String name, List<bool> toAdd)
        {
            List<TempNode> nodes = m_temperatureGraph.ExportPoints();
            Schedule schedule = new Schedule(name);
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (toAdd[i])
                {
                    foreach (TempNode n in nodes)
                    {
                        schedule.AddNode(i, n);
                    }
                }
            }
            m_controller.SaveSchedule(schedule);
        }

        /// <summary>
        /// Lists all heaters, checks if user is adding only one new heater.
        /// </summary>
        public void DisplayHeaters()
        {
            Heaters heaters = m_controller.FindHeaters();
            List<Profile> profiles = m_controller.GetProfiles();
            SortedSet<String> profileNames = GetProfileNames(profiles);
            Profile activeProfile = null;

            ComboBox cBoxProfiles = (PanelFindHeaters.Controls.Find("comBoxHetersProfile", false).FirstOrDefault() as ComboBox);
            // clear from previous run
            cBoxProfiles.Items.Clear();
            cBoxProfiles.Text = "";

            cBoxProfiles.Items.AddRange(profileNames.ToArray());
            foreach (Profile p in profiles)
            {
                if (p.Active)
                {
                    activeProfile = p;
                    cBoxProfiles.Text = p.Name;
                    break;
                }
            }

            DisplayHeaters(heaters, activeProfile);
        }

        public void ProfileChanged()
        {
            Heaters heaters = m_controller.FindHeaters();
            List<Profile> profiles = m_controller.GetProfiles();
            ComboBox cBoxProfiles = (PanelFindHeaters.Controls.Find("comBoxHetersProfile", false).FirstOrDefault() as ComboBox);

            DisplayHeaters(heaters, profiles.Find(x => x.Name == cBoxProfiles.Text));
        }

        private void DisplayHeaters(Heaters heaters, Profile profile)
        {
            RemoveAddedControlsFrom(m_current);
            List<Schedule> schedules = m_controller.GetSchedules();
            List<String> scheduleNames = GetScheduleNames(schedules);

            ComboBox comBoxAll = PanelFindHeaters.Controls.Find("comboBoxHeatersAll", false)[0] as ComboBox;
            comBoxAll.Items.Clear();
            comBoxAll.Items.AddRange(scheduleNames.ToArray());

            int i = 1;
            bool newHeater = false;
            foreach (Heater h in heaters.GetHeaters())
            {
                if (h.Id.Equals(Heaters.LastId))
                {
                    if (!newHeater)
                    {
                        newHeater = true;
                    }
                    else
                    {
                        MessageBox.Show("Please connect only one new heater at time", "Multiple new heaters", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                }

                Label lId = new Label();
                TextBox txtName = new TextBox();
                ComboBox cBoxSchedule = new ComboBox();
                Button btnShowSchedule = new Button();

                lId.Text = h.Id;
                lId.Name = "label" + i.ToString();
                lId.AutoSize = true;
                lId.Location = new Point(6, 35 + i * 30);

                txtName.Text = h.Name;
                txtName.Name = "txtBox" + i.ToString();
                txtName.Location = new Point(38, 35 + i * 30);

                if (!scheduleNames.Contains(h.Schedule))
                    scheduleNames.Insert(0, h.Schedule);
            
                cBoxSchedule.Items.AddRange(scheduleNames.ToArray());
                cBoxSchedule.Name = "cBox" + i.ToString();
                cBoxSchedule.Location = new Point(207, 35 + i * 30);

                if (profile != null && profile.HeaterIdSchedule.ContainsKey(h.Id))
                {
                    cBoxSchedule.Text = profile.HeaterIdSchedule[h.Id];
                }

                btnShowSchedule.Name = "btnShowShedule" + i.ToString();
                btnShowSchedule.Text = "Show";
                btnShowSchedule.Location = new Point(350, 35 + i * 30);
                btnShowSchedule.Click += new EventHandler(ButtonShow);

                i++;
                m_panelFindHeaters.Controls.Add(lId);
                m_panelFindHeaters.Controls.Add(txtName);
                m_panelFindHeaters.Controls.Add(cBoxSchedule);
                m_panelFindHeaters.Controls.Add(btnShowSchedule);

                
            }
        }

        /// <summary>
        /// Saves new setings for heaters and profile.
        /// </summary>
        public void SaveHeatersAndSetActiveProfile()
        {
            List<Heater> heaters;
            ComboBox cBoxProfiles = (PanelFindHeaters.Controls.Find("comBoxHetersProfile", false).FirstOrDefault() as ComboBox);

            try
            {
                heaters = GetHeatersFromGUI();
            }
            catch
            {
                return;
            }

            m_controller.SaveHeatersAndActiveProfile(heaters, cBoxProfiles.Text);
        }

        public void SaveProfile()
        {
            List<Heater> heaters;
            ComboBox cBoxProfiles = (PanelFindHeaters.Controls.Find("comBoxHetersProfile", false).FirstOrDefault() as ComboBox);

            try
            {
                heaters = GetHeatersFromGUI();
            }
            catch
            {
                return;
            }

            m_controller.SaveNewHeatersAndNonactiveProfile(heaters, cBoxProfiles.Text);
        }

        /// <summary>
        /// Calls controller to start change heating to normal schedule.
        /// </summary>
        public void EarlyReturn()
        {
            m_controller.EarlyReturn();
        }
        /// <summary>
        /// Checks if date is correct.
        /// Calls controller to set date and temperature.
        /// </summary>
        /// <param name="date">Return date</param>
        /// <param name="temp">Desired temperature</param>
        /// <returns></returns>
        public bool SetDateOfReturnFromHolidays(DateTime date, int temp)
        {
            if (date < DateTime.Now)
                return false;

            m_controller.SetReturnDateAndTemperature(date, temp);
            return true;
        }

        public bool SetDateOfReturnAndProfile(DateTime date, Profile profile)
        {
            if (date < DateTime.Now)
                return false;

            if (!m_controller.GetProfiles().Contains(profile))
            {
                return false;
            }

            m_controller.SetReturnDateAndProfile(date, profile);
            return true;
        }

        /// <summary>
        /// List all heaters and temperatures that they are reporting.
        /// </summary>
        public void GetCurrentTemperature()
        {
            Dictionary<String, double> heaters = m_controller.GetTemperature();

            int i = 1;
            foreach (KeyValuePair<String, double> pair in heaters)
            {
                Label heater = new Label();
                Label temp = new Label();

                heater.Name = "labelHeater" + i.ToString();
                heater.Text = pair.Key;
                heater.AutoSize = true;
                heater.Location = new Point(22, 12 + i * 25);

                temp.Name = "labelTemp" + i.ToString();
                temp.Text = Math.Round(pair.Value, 2).ToString() + " °C";
                temp.AutoSize = true;
                temp.Location = new Point(150, 12 + i * 25);

                PanelHolidaysAndTemp.Controls.Add(heater);
                PanelHolidaysAndTemp.Controls.Add(temp);

                i++;
            }
        }

        public List<Profile> GetProfiles()
        {
            return m_controller.GetProfiles();
        }

        public List<Schedule> GetSchedules()
        {
            return m_controller.GetSchedules();
        }

        /// <summary>
        /// Creates list of schedule names.
        /// </summary>
        /// <param name="schedules">List of schedules</param>
        /// <returns>List of names</returns>
        public List<String> GetScheduleNames(List<Schedule> schedules)
        {
            List<String> names = new List<string>();
            foreach (Schedule s in schedules)
            {
                names.Add(s.GetName());
            }

            return names;
        }

        private bool IsAdminPanel(Panel panel)
        {
            return (panel.Equals(m_panelAddUser) ||
                    panel.Equals(m_panelAssignSchedule) ||
                    panel.Equals(m_panelCreateSchedule) ||
                    panel.Equals(m_panelFindHeaters));
        }

        /// <summary>
        /// Removes all controls which contains defined names.
        /// Removes labels, textBoxes, ComboBoxes.
        /// </summary>
        /// <param name="panel"></param>
        private void RemoveAddedControlsFrom(Panel panel)
        {
            List<string> names = new List<string>
            {
                "label",
                "txtBox",
                "cBox",
                "btnShowShedule"
            };

            // safely removes controls from list
            for (int i = panel.Controls.Count - 1; i >= 0; i--)
            {
                foreach (string s in names)
                {
                    if (panel.Controls[i].Name.Contains(s))
                    {
                        panel.Controls.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Extracts names from list of profiles.
        /// </summary>
        /// <param name="profiles"></param>
        /// <returns></returns>
        private SortedSet<String> GetProfileNames(List<Profile> profiles)
        {
            SortedSet<String> res = new SortedSet<string>();

            foreach (Profile p in profiles)
            {
                res.Add(p.Name);
            }

            return res;
        }

        private List<Heater> GetHeatersFromGUI()
        {
            List<Heater> heaters = new List<Heater>();
            List<String> names = GetScheduleNames(m_controller.GetSchedules());
            String scheduleName;
            ComboBox cBoxProfiles = (PanelFindHeaters.Controls.Find("cBoxHetersProfile", false).FirstOrDefault() as ComboBox);

            int i = 1;
            while (m_panelFindHeaters.Controls.Find("label" + i.ToString(), true).Length > 0)
            {
                try
                {
                    scheduleName = m_panelFindHeaters.Controls["cBox" + i.ToString()].Text;
                }
                catch
                {
                    MessageBox.Show("Please select correct schedule name", "Wrong schedule name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new FormatException();
                }
                if (!names.Contains(scheduleName))
                {
                    MessageBox.Show("Please select correct schedule name", "Wrong schedule name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new FormatException();
                }
                heaters.Add(
                    new Heater(
                        m_panelFindHeaters.Controls["label" + i.ToString()].Text,
                        m_panelFindHeaters.Controls["txtBox" + i.ToString()].Text,
                        scheduleName
                    ));
                i++;
            }

            return heaters;
        }

        public void SetScheduleToComBoxes(String name)
        {
            for (int i = 1; PanelFindHeaters.Controls.Find("cBox" + i.ToString(), false).Length > 0; i++)
            {
                PanelFindHeaters.Controls["cBox" + i.ToString()].Text = name;
            }
        }

        private void ButtonShow(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int index;
            try
            {
                index = Convert.ToInt32(btn.Name.TrimStart("btnShowShedule".ToCharArray()));
            }
            catch
            {
                throw new ArgumentException("Cannot get button index.");
            }

            String schedule = (m_panelFindHeaters.Controls.Find("cBox" + index.ToString(), true)[0] as ComboBox).Text;

            Schedule s = GetSchedules().Find(x => x.GetName() == schedule);

            ShowSchedule showSchedule = new ShowSchedule(s);
        }
    }
}
