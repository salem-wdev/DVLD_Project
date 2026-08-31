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

namespace DVLD.Applications.ReplaceLostOrDamagedLicense
{
    public partial class frmReplaceLostOrDamagedLicenseApplication : Form
    {

        int _ReplacedLicenseID = -1;
        clsLicense _ReplacedLicense = null;
        clsApplication.enApplicationType _ReplaceReason = clsApplication.enApplicationType.ReplaceLostDrivingLicense;
        clsLicense.enIssueReason _IssueReason = clsLicense.enIssueReason.LostReplacement;
        public frmReplaceLostOrDamagedLicenseApplication()
        {
            InitializeComponent();
        }

        private async Task SetReasonAsync()
        {
            if (rbDamagedLicense.Checked)
            {
                _ReplaceReason = clsApplication.enApplicationType.ReplaceDamagedDrivingLicense;
                _IssueReason = clsLicense.enIssueReason.DamagedReplacement;
                lblTitle.Text = "Replacement for Damaged License";
                this.Text = lblTitle.Text;
            }
            else
            {
                _ReplaceReason = clsApplication.enApplicationType.ReplaceLostDrivingLicense;
                _IssueReason = clsLicense.enIssueReason.LostReplacement;
                lblTitle.Text = "Replacement for Lost License";
                this.Text = lblTitle.Text;
            }

            lblApplicationFees.Text = (await clsApplicationType.FindAsync
                    ((int)_ReplaceReason)).ApplicationTypeFees.ToString("0.##");

        }

        private async void frmReplaceLostOrDamagedLicenseApplication_Load(object sender, EventArgs e)
        {
            lblApplicationDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
            await SetReasonAsync();

            AcceptButton = ctrlDriverLicenseInfoWithFilter1.AcceptButton;
        }

        private void frmReplaceLostOrDamagedLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }

        private async void rbReplaceLicenseReason_CheckedChanged(object sender, EventArgs e)
        {
            await SetReasonAsync();
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseID >0)
            {
                frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("History not exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(_ReplacedLicense != null)
            {
                frmShowLicenseInfo frm = new frmShowLicenseInfo(_ReplacedLicenseID);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("License not existes!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnIssueReplacement_Click(object sender, EventArgs e)
        {
            _ReplacedLicense = await ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ReplaceAsync
                (clsGlobal.CurrentUser.UserID, _IssueReason);

            if( _ReplacedLicense == null)
            {
                btnIssueReplacement.Enabled = false;
                MessageBox.Show("Could not replace!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblApplicationID.Text = _ReplacedLicense.ApplicationID.ToString();
            _ReplacedLicenseID = _ReplacedLicense.LicenseID;
            lblRreplacedLicenseID.Text = _ReplacedLicenseID.ToString();
            MessageBox.Show("Licensed Replaced Successfully with ID=" + _ReplacedLicense.LicenseID.ToString(), "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnIssueReplacement.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            llShowLicenseInfo.Enabled = true;

        }

        private async void ctrlDriverLicenseInfoWithFilter1_LicenseSelected(object sender, Licenses.Local_Licenses.Controls.ctrlDriverLicenseInfoWithFilter.LicenseSelectedEventArgs e)
        {
            int licenseID = e.LicenseID;
            if (licenseID > 0)
            {
                if (!await clsLicense.IsLicenseActiveAsync(licenseID))
                {
                    MessageBox.Show("License not active!" +
                        "\nSelect another one", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnIssueReplacement.Enabled = false;
                    return;
                }

                await SetReasonAsync();

                _ReplacedLicenseID = -1;
                _ReplacedLicense = null;
                llShowLicenseHistory.Enabled = true;
                btnIssueReplacement.Enabled = true;
                lblOldLicenseID.Text = licenseID.ToString();

            }

        }
    }
}
