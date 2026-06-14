using DVLD.Global_Classes;
using DVLD.People.Controls;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Users
{
    public partial class frmChangePassword : Form
    {

        private int _UserID;

        private clsUser _User;

        public frmChangePassword(int UserID)
        {
            InitializeComponent();
            _UserID = UserID;
        }

        private void _ResetDefualtValues()
        {
            txtCurrentPassword.Text = "";
            txtNewPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtCurrentPassword.Focus();
        }


        private bool _IsPasswordReadyToSave()
        {
            if (_User == null)
            {
                return false;
            }

            if (txtCurrentPassword.Text == string.Empty)
            {
                return false;
            }

            if (txtNewPassword.Text == string.Empty)
            {
                return false;
            }

            if (txtConfirmPassword.Text == string.Empty)
            {
                return false;
            }

            return true;
        }

        private void frmChangePassword_Load(object sender, EventArgs e)
        {
            _ResetDefualtValues();

            _User = clsUser.Find(_UserID);

            if(_User == null)
            {
                MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            ctrlUserCard1.LoadUserInfo(_UserID);
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = _IsPasswordReadyToSave();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (_User.Password != txtCurrentPassword.Text.Trim())
            {
                MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(txtNewPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
            {
                MessageBox.Show("Password is not matching.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            _User.Password = txtNewPassword.Text;


            if (_User.ChangeUserCredentials(_User.UserName, txtNewPassword.Text.Trim()))
            {
                MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this._ResetDefualtValues();
            }
            else
            {
                MessageBox.Show("Failed to change password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCurrentPassword_Validating(object sender, CancelEventArgs e)
        {
            TextBox text = sender as TextBox;

            if (string.IsNullOrWhiteSpace(text.Text.Trim()))
            {
                errorProvider1.SetError(text, $"{text.Tag} is required.");
                e.Cancel = true;
                return;
            }
            else
            {
                errorProvider1.SetError(text, string.Empty);
                e.Cancel = false;
            }

            if (this._User.Password != txtCurrentPassword.Text)
            {
                errorProvider1.SetError(text, $"{text.Tag} is incorrect.");
                e.Cancel = true;
            }
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            TextBox text = sender as TextBox;

            if (string.IsNullOrWhiteSpace(text.Text.Trim()))
            {
                errorProvider1.SetError(text, $"{text.Tag} is required.");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(text, string.Empty);
                e.Cancel = false;
            }
        }

        private void txtConfirmPassword_Validating(object sender, CancelEventArgs e)
        {
            TextBox text = sender as TextBox;

            if (txtNewPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
            {
                errorProvider1.SetError(text, $"Password is not matching.");
                e.Cancel = true;
                return;
            }
            else
            {
                errorProvider1.SetError(text, string.Empty);
                e.Cancel = false;
            }


            if (string.IsNullOrWhiteSpace(text.Text.Trim()))
            {
                errorProvider1.SetError(text, $"{text.Tag} is required.");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(text, string.Empty);
                e.Cancel = false;
            }
        }
    }
}
