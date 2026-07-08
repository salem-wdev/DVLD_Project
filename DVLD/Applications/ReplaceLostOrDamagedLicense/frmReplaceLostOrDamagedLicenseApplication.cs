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
        clsApplication.enApplicationType ReplaceReason = clsApplication.enApplicationType.ReplaceLostDrivingLicense;
        clsLicense.enIssueReason _IssueReason = clsLicense.enIssueReason.LostReplacement;
        public frmReplaceLostOrDamagedLicenseApplication()
        {
            InitializeComponent();
        }

        private void SetReason()
        {
            if (rbDamagedLicense.Checked)
            {
                ReplaceReason = clsApplication.enApplicationType.ReplaceDamagedDrivingLicense;
                _IssueReason = clsLicense.enIssueReason.DamagedReplacement;
                lblTitle.Text = "Replacement for Damaged License";
                this.Text = lblTitle.Text;
            }
            else
            {
                ReplaceReason = clsApplication.enApplicationType.ReplaceLostDrivingLicense;
                _IssueReason = clsLicense.enIssueReason.LostReplacement;
                lblTitle.Text = "Replacement for Lost License";
                this.Text = lblTitle.Text;
            }

            lblApplicationFees.Text = clsApplicationType.Find
                    ((int)ReplaceReason).ApplicationTypeFees.ToString("0.##");

        }

        private void frmReplaceLostOrDamagedLicenseApplication_Load(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected;

            lblApplicationDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
            SetReason();

            AcceptButton = ctrlDriverLicenseInfoWithFilter1.AcceptButton;
        }

        private void CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int obj)
        {
            if (obj > 0)
            {
                if(!clsLicense.IsLicenseActive(obj))
                {
                    MessageBox.Show("License not active!" +
                        "\nSelect another one", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnIssueReplacement.Enabled = false;
                    return;
                }

                SetReason();

                _ReplacedLicenseID = -1;
                _ReplacedLicense = null;
                llShowLicenseHistory.Enabled = true;
                btnIssueReplacement.Enabled = true;
                lblOldLicenseID.Text = obj.ToString();
                
            }

        }

        private void frmReplaceLostOrDamagedLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }

        private void rbReplaceLicenseReason_CheckedChanged(object sender, EventArgs e)
        {
            SetReason();
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

        private void btnIssueReplacement_Click(object sender, EventArgs e)
        {
            _ReplacedLicense = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Replace
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
    }
}
