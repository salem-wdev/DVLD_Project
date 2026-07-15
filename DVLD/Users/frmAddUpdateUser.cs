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
    public partial class frmAddUpdateUser : Form
    {
        enum enMode { AddNew, Update }

        enMode _Mode;

        private int _UserID;

        clsUser _User;

        public frmAddUpdateUser()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdateUser(int UserID)
        {
            InitializeComponent();
            _Mode = enMode.Update;
            _UserID = UserID;
        }

        private void _ResetValues()
        {
            //this will initialize the reset the defaule values

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New User";
                this.Text = "Add New User";
                _User = null;

                tpLoginInfo.Enabled = false;

                ctrlPersonCardWithFilter1.gbFilters.Focus();
            }
            else
            {
                lblTitle.Text = "Update User";
                this.Text = "Update User";

                tpLoginInfo.Enabled = true;
                btnSave.Enabled = true;


            }

            txtUserName.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            chkIsActive.Checked = true;
        }

        private void _LoadData()
        {
            _User = clsUser.Find(_UserID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;

            if (_User == null)
            {
                MessageBox.Show("User not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            btnNext.Enabled = true;
            lblUserID.Text = _User.UserID.ToString();
            txtUserName.Text = _User.UserName;
            txtPassword.Text = _User.Password;
            chkIsActive.Checked = _User.IsActive;
            ctrlPersonCardWithFilter1.LoadPersonInfo(_User.PersonID);

        }

        private bool _IsPersonUser()
        {

            return (_Mode == enMode.AddNew && ctrlPersonCardWithFilter1.PersonID > 0)
                                                  ? clsUser.IsUserExistsForPersonID(ctrlPersonCardWithFilter1.PersonID)
                                                  : false;
        }

        private bool _IsbtnSaveReadyToEnable()
        {
            if(ctrlPersonCardWithFilter1.SelectedPerson==null)
            {
                return false;
            }

            if(txtUserName.Text == string.Empty)
            {
                return false;
            }

            if(txtPassword.Text == string.Empty)
            {
                return false;
            }

            if(txtConfirmPassword.Text == string.Empty)
            {
                return false;
            }
            AcceptButton = btnSave;
            return true;
        }

        private void frmAddUpdateUser_Load(object sender, EventArgs e)
        {
            _ResetValues();
            AcceptButton = ctrlPersonCardWithFilter1.AcceptButton;

            if(_Mode == enMode.Update)
            {
                _LoadData();
                ctrlPersonCardWithFilter1.FilterEnabled = false;
                AcceptButton = btnSave;
                tcInfo.SelectedTab = tcInfo.TabPages["tpLoginInfo"];
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                btnSave.Enabled = true;
                tpLoginInfo.Enabled = true;
                tcInfo.SelectedTab = tcInfo.TabPages["tpLoginInfo"];
                return;
            }

            if (ctrlPersonCardWithFilter1.PersonID > 0)
            {
                // check is person is a user
                if (_IsPersonUser())
                {
                    MessageBox.Show("Person already is a User", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnNext.Enabled = false;
                    return;
                }
                else
                {
                    btnSave.Enabled = true;
                    tpLoginInfo.Enabled = true;
                    tcInfo.SelectedTab = tcInfo.TabPages["tpLoginInfo"];
                }
            }
            else

            {
                MessageBox.Show("Please Select a Person", "Select a Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.gbFilters.Focus();

            }

        }

        private void Password_Validating(object sender, CancelEventArgs e)
        {
            TextBox text = (TextBox)sender;

            if (text.Text== string.Empty)
            {
                errorProvider1.SetError(text, $"{text?.Tag.ToString()} is Required");
                e.Cancel = true;
            }

            

        }

        private void User_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = _IsbtnSaveReadyToEnable();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!this.ValidateChildren())
            {
                MessageBox.Show("Please fix validation errors", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // check if password and confirm password match
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password must match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                return;
            }

            // check is person is a user if mode is add new
            if (_Mode == enMode.AddNew && _IsPersonUser())
            {
                MessageBox.Show("Person already is a User", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                return;
            }
            if (_Mode == enMode.AddNew)
            {
                _User = clsUser.CreateNewUser(ctrlPersonCardWithFilter1.PersonID, txtUserName.Text.Trim(), txtPassword.Text.Trim());
                _User.IsActive = chkIsActive.Checked;

                if (_User.Save())
                {
                    _Mode = enMode.Update;
                    MessageBox.Show("Saved successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblTitle.Text = "Update User";
                    this.Text = "Update User";
                }
                else
                {
                    MessageBox.Show("Save failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (_User.ChangeUserCredentials(txtUserName.Text.Trim(), txtPassword.Text.Trim()) && _User.ChangeUserActivity(chkIsActive.Checked) && _User.ChangeUserActivity(chkIsActive.Checked))
                {
                    MessageBox.Show("Updated successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                { 
                    MessageBox.Show("Update failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                
            }



            ctrlPersonCardWithFilter1.FilterEnabled = false;
            txtPassword.Focus();

        }

        private void txtUserName_Validating(object sender, CancelEventArgs e)
        {
            TextBox text = (TextBox)sender;
            string currentinput = text.Text.Trim();

            if (string.IsNullOrWhiteSpace(currentinput))
            {
                errorProvider1.SetError(text, $"{text?.Tag.ToString()} is Required");
                e.Cancel = true;
                return;
            }

            clsUser user = clsUser.Find(currentinput);

            if(user != null)
            {
                if (_Mode == enMode.Update && user.UserID == _User.UserID)
                {
                    errorProvider1.SetError(text, string.Empty);
                    e.Cancel = false;
                    return;
                }

                errorProvider1.SetError(text, $"{text?.Tag.ToString()} is Already Taken");
                e.Cancel = true;
                return;
            }

            errorProvider1.SetError(text, string.Empty);
            e.Cancel = false;

        }

        private void ctrlPersonCardWithFilter1_PersonSelected(object sender, People.Controls.ctrlPersonCardWithFilter.PersonSelectedEventArgs e)
        {
            tpLoginInfo.Enabled = false;
            btnNext.Enabled = false;

            if (_IsPersonUser())
            {
                MessageBox.Show("Person already is a User", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AcceptButton = ctrlPersonCardWithFilter1.AcceptButton;
                ctrlPersonCardWithFilter1.FilterFocus();
                return;
            }

            tpLoginInfo.Enabled = true;
            btnNext.Enabled = true;



            btnSave.Enabled = _IsbtnSaveReadyToEnable();
            AcceptButton = btnNext;
        }

        private void frmAddUpdateUser_Activated(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.FilterFocus();
        }
    }
}
