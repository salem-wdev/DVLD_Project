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

namespace DVLD.Applications.Rlease_Detained_License
{
    public partial class frmReleaseDetainedLicenseApplication : Form
    {
        private int _SelectedLicenseID = -1;
        private int _DetainLicenseID = -1;
        private bool _IsLicenseSelected = false;

        public frmReleaseDetainedLicenseApplication()
        {
            InitializeComponent();
        }

        public frmReleaseDetainedLicenseApplication(int LicenseID)
        {
            InitializeComponent();
            _SelectedLicenseID = LicenseID;
            _IsLicenseSelected = true;
        }

        private void frmReleaseDetainedLicenseApplication_Load(object sender, EventArgs e)
        {
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
            AcceptButton = ctrlDriverLicenseInfoWithFilter1.AcceptButton;
            if(_IsLicenseSelected)
            {
                ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
                ctrlDriverLicenseInfoWithFilter1.LoadLicense(_SelectedLicenseID);
            }
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo(_SelectedLicenseID);
            frm.ShowDialog();
        }

        private void btnRelease_Click(object sender, EventArgs e)
        {

            if (!ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Release(clsGlobal.CurrentUser.UserID))
            {
                MessageBox.Show("Failed to release license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblApplicationID.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedInfo.ReleaseApplicationID.ToString();
            MessageBox.Show("License Released Successfully with ID="
                + ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedInfo.ReleaseApplicationID.ToString(),
                "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnRelease.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID);
            frm.ShowDialog();
        }

        private void frmReleaseDetainedLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }

        private void ctrlDriverLicenseInfoWithFilter1_LicenseSelected(object sender, Licenses.Local_Licenses.Controls.ctrlDriverLicenseInfoWithFilter.LicenseSelectedEventArgs e)
        {
            int licenseID = e.LicenseID;
            if (licenseID > 0)
            {
                _SelectedLicenseID = licenseID;
                if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null || ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedInfo.IsReleased)
                {
                    MessageBox.Show("The selected license is not detained.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _DetainLicenseID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedInfo.DetainID;
                llShowLicenseHistory.Enabled = true;
                btnRelease.Enabled = true;
                llShowLicenseInfo.Enabled = true;
                lblLicenseID.Text = _SelectedLicenseID.ToString();
                lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
                lblDetainID.Text = _DetainLicenseID.ToString();
                lblDetainDate.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedInfo.DetainDate.ToString("dd/MM/yyyy");
                float FineFees = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedInfo.FineFees;
                float ApplicationFees = (float)clsApplicationType.Find((int)clsApplication.enApplicationType.ReleaseDetainedDrivingLicense).ApplicationTypeFees;
                lblFineFees.Text = FineFees.ToString();
                lblApplicationFees.Text = ApplicationFees.ToString();
                lblTotalFees.Text = (FineFees + ApplicationFees).ToString();
            }

        }
    }
}
