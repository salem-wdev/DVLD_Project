using DVLD.Global_Classes;
using DVLD_Business;
using DVLD_Business.Global_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                clsUser.Login(txtUserName.Text.Trim(), txtPassword.Text.Trim(),
                chkRememberMe.Checked))!= null)
            {
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
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            string rememberedUsername = string.Empty;
            string rememberedPassword = string.Empty;
            if(clsGlobal.GetStoredCredential(ref rememberedUsername, ref rememberedPassword))
            {
                txtUserName.Text = rememberedUsername;
                txtPassword.Text = rememberedPassword;
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
