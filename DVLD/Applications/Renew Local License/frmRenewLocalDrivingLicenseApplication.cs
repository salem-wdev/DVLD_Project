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

namespace DVLD.Applications.Renew_Local_License
{
    public partial class frmRenewLocalDrivingLicenseApplication : Form
    {
        int _NewLicenseID = -1;
        clsLicense _NewLicense = null;
        public frmRenewLocalDrivingLicenseApplication()
        {
            InitializeComponent();
        }

        private void frmRenewLocalDrivingLicenseApplication_Load(object sender, EventArgs e)
        {
        
            lblApplicationDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblIssueDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
            lblApplicationFees.Text = clsApplicationType.Find
                ((int)clsApplication.enApplicationType.RenewDrivingLicense)
                .ApplicationTypeFees.ToString("0.##");
            AcceptButton = ctrlDriverLicenseInfoWithFilter1.AcceptButton;
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID);
            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(_NewLicense != null)
            {
                frmShowLicenseInfo frm = new frmShowLicenseInfo(_NewLicense.LicenseID);
                frm.ShowDialog();
            }
        }

        private void btnRenewLicense_Click(object sender, EventArgs e)
        {
            _NewLicense = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Renew(txtNotes.Text, clsGlobal.CurrentUser.UserID);
            if(_NewLicense == null)
            {
                llShowLicenseInfo.Enabled = false;
                MessageBox.Show("Failed to renew license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblApplicationID.Text = _NewLicense.ApplicationID.ToString();
            _NewLicenseID = _NewLicense.LicenseID;
            lblRenewedLicenseID.Text = _NewLicenseID.ToString();
            lblExpirationDate.Text = _NewLicense.ExpirationDate.ToString("dd/MM/yyyy");
            MessageBox.Show("Licensed Renewed Successfully with ID=" + _NewLicenseID.ToString(), "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnRenewLicense.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            llShowLicenseInfo.Enabled = true;
        }

        private void frmRenewLocalDrivingLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ctrlDriverLicenseInfoWithFilter1_LicenseSelected(object sender, Licenses.Local_Licenses.Controls.ctrlDriverLicenseInfoWithFilter.LicenseSelectedEventArgs e)
        {
            int licensesID = e.LicenseID;
            if (licensesID > 0)
            {
                DateTime ExpirationDate = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ExpirationDate;
                DateTime CurrentDate = clsBusinessSettings.GetServerDateTime();
                if (CurrentDate > ExpirationDate.AddMonths(3) || CurrentDate < ExpirationDate.AddMonths(-3))
                {
                    MessageBox.Show("License is not eligible for renewal." +
                        "\nLicense can only be renewed within 3 months before or after the expiration date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _NewLicenseID = -1;
                _NewLicense = null;
                llShowLicenseHistory.Enabled = true;
                btnRenewLicense.Enabled = true;
                lblOldLicenseID.Text = licensesID.ToString();
                float ClassFees = clsLicenseClass.Find(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseClassID).ClassFees;
                lblLicenseFees.Text = ClassFees.ToString();
                lblTotalFees.Text = (ClassFees + (float)clsApplicationType.Find((int)clsApplication.enApplicationType.RenewDrivingLicense).ApplicationTypeFees).ToString();
            }

        }
    }
}
