using DVLD.Global_Classes;
using DVLD.People.Forms;
using DVLD.Users;
using DVLD_Business.Global_Classes;
using DVLD_Business.Users;
using DVLD_Shared;
using DVLD_Shared.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DVLD
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if ((clsGlobal.CurrentUser =
                clsUser.Login(txtUserName.Text.Trim(), txtPassword.Text.Trim()))!= null)
            {
                if (chkRememberMe.Checked)
                {
                    //store username and password in registry.
                    clsRegistryManager.RegisterValues(
                        new Dictionary<string, string>
                        {
                            { "Username", txtUserName.Text.Trim() },
                            { "Password", txtPassword.Text.Trim() }
                        }
                        , @"SOFTWARE\DVLD");

                }
                else
                {
                    // delete username and password from registry.
                    clsRegistryManager.DeleteValues(new[] { "Username", "Password" }, @"SOFTWARE\DVLD");

                }

                frmMain frm = new frmMain(this);
                frm.Show();
                this.Hide();
            }
            else
            {
                

                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
            return;
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            clsLogger.Log("Login Form Loaded");

            string rememberedUsername = string.Empty;
            string rememberedPassword = string.Empty;
            Dictionary<string, string> storedCredentials = clsRegistryManager.GetRegisteredValues(new[] { "Username", "Username" }, @"SOFTWARE\DVLD");

            if (storedCredentials.Count > 0)
            {
                txtUserName.Text = storedCredentials.ContainsKey(rememberedUsername) ? storedCredentials["Username"] : string.Empty;
                txtPassword.Text = storedCredentials.ContainsKey(rememberedPassword) ? storedCredentials["Password"] : string.Empty;
                chkRememberMe.Checked = true;
            }
            else
            {
                chkRememberMe.Checked = false;
            }

        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
