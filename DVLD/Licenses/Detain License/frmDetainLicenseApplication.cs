using DVLD.Licenses.Local_Licenses;
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

namespace DVLD.Licenses.Detain_License
{
    public partial class frmDetainLicenseApplication : Form
    {
        private int _DetainID = -1;
        private int _SelectedLicenseID = -1;
        private clsDetainedLicense _DetainedLicense = null;
        public frmDetainLicenseApplication()
        {
            InitializeComponent();
        }

        private void frmDetainLicenseApplication_Load(object sender, EventArgs e)
        {
            AcceptButton = ctrlDriverLicenseInfoWithFilter1.AcceptButton;
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
        }

        private void txtFineFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox CurrentTextBox = (TextBox)sender;
            e.Handled = !clsValidation.IsInputValidDecimal(e.KeyChar, CurrentTextBox.Text, CurrentTextBox.SelectionStart);
        }

        private void btnDetain_Click(object sender, EventArgs e)
        {
            if(!this.ValidateChildren())
            {
                MessageBox.Show("Please enter Fine Fees.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _DetainedLicense = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Detain(float.Parse(txtFineFees.Text), clsGlobal.CurrentUser.UserID);
            if (_DetainedLicense == null)
            {
                llShowLicenseInfo.Enabled = false;
                MessageBox.Show("Failed to Detain License.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _DetainID = _DetainedLicense.DetainID;
            lblDetainID.Text = _DetainID.ToString();
            lblDetainDate.Text = _DetainedLicense.DetainDate.ToString();
            MessageBox.Show("License Detained Successfully with ID=" + _DetainID.ToString(), "License Detained", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnDetain.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
        }

        private void frmDetainLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID);
            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo(_SelectedLicenseID);
            frm.ShowDialog();
        }

        private void txtFineFees_Validating(object sender, CancelEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtFineFees.Text))
            {
                errorProvider1.SetError(txtFineFees, "Fine Fees is required.");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(txtFineFees, "");
                e.Cancel = false;
            }
        }

        private void ctrlDriverLicenseInfoWithFilter1_LicenseSelected(object sender, Local_Licenses.Controls.ctrlDriverLicenseInfoWithFilter.LicenseSelectedEventArgs e)
        {
            int licenseID = e.LicenseID;

            llShowLicenseHistory.Enabled = false;
            llShowLicenseInfo.Enabled = false;
            btnDetain.Enabled = false;

            if (licenseID > 0)
            {
                if (!ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.IsActive)
                {
                    MessageBox.Show("This License is not active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.IsDetained)
                {
                    MessageBox.Show("This License is already Detained.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _DetainID = -1;
                _DetainedLicense = null;
                _SelectedLicenseID = licenseID;
                llShowLicenseHistory.Enabled = true;
                llShowLicenseInfo.Enabled = true;
                btnDetain.Enabled = true;
                lblLicenseID.Text = licenseID.ToString();
            }
        }
    }
}
