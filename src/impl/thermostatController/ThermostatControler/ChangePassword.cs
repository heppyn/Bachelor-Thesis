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
    public partial class ChangePassword : Form
    {
        private Presenter m_caller = null;
        private String m_name = null;

        public ChangePassword()
        {
            InitializeComponent();
        }

        public ChangePassword(Presenter caller, String name)
        {
            m_caller = caller;
            m_name = name;
            InitializeComponent();
        }

        private void btnPassOk_Click(object sender, EventArgs e)
        {
            if (txtPass1.Text.Equals(txtPass2.Text))
            {
                m_caller.CreateUser(m_name, txtPass1.Text);
                this.Close();
            }
            else
                MessageBox.Show("Passwords don't match", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);   
        }
    }
}
