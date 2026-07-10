using DVLD.Licenses.International_Licenses;
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

namespace DVLD.Applications.International_License
{
    public partial class frmNewInternationalLicenseApplication : Form
    {
        private int _NewInternationalLicenseID = -1;
        private clsInternationalLicense _NewInternationalLicense = null;

        public frmNewInternationalLicenseApplication()
        {
            InitializeComponent();
        }

        private void frmNewInternationalLicenseApplication_Load(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected;

            AcceptButton = ctrlDriverLicenseInfoWithFilter1.AcceptButton;

            lblApplicationDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblIssueDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
            lblFees.Text = clsApplicationType.Find
                ((int)clsApplication.enApplicationType.NewInternationalLicense)
                .ApplicationTypeFees.ToString("0.##");
        }

        private void CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int obj)
        {
            llShowLicenseHistory.Enabled = false;
            btnIssueLicense.Enabled = false;
            if (obj > 0)
            {
                llShowLicenseHistory.Enabled = true;
                int LicenseClass = (clsLicense.Find(obj)?.LicenseClassID ?? -1);
                if (LicenseClass < 1)
                {
                    MessageBox.Show("License not Existes!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (LicenseClass != 3)
                {
                    MessageBox.Show("License most be class (3)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int InternationalLicenseID = -1;
                InternationalLicenseID
                    = clsInternationalLicense.GetActiveInternationalLicenseIDByDriverID
                    (ctrlDriverLicenseInfoWithFilter1?.SelectedLicenseInfo?.DriverID ?? -1);
                if (InternationalLicenseID > 0)
                {
                    MessageBox.Show("This Driver already has an Active International License with ID = " + InternationalLicenseID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _NewInternationalLicenseID = -1;
                _NewInternationalLicense = null;
                btnIssueLicense.Enabled = true;
                lblLocalLicenseID.Text = obj.ToString();
            }
        }

        private void frmNewInternationalLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }

        private void btnIssueLicense_Click(object sender, EventArgs e)
        {
            _NewInternationalLicense = clsInternationalLicense.IssueNewInternationalLicense(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverID, clsGlobal.CurrentUser.UserID);
            if (_NewInternationalLicense == null)
            {
                llShowLicenseInfo.Enabled = false;
                MessageBox.Show("Failed to Issue New International License.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblApplicationID.Text = _NewInternationalLicense.ApplicationID.ToString();
            _NewInternationalLicenseID = _NewInternationalLicense.InternationalLicenseID;
            lblInternationalLicenseID.Text = _NewInternationalLicenseID.ToString();
            lblExpirationDate.Text = _NewInternationalLicense.ExpirationDate.ToString("dd/MM/yyyy");
            MessageBox.Show("International License Issued Successfully with ID=" + _NewInternationalLicenseID.ToString(), "International License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnIssueLicense.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            llShowLicenseInfo.Enabled = true;
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID);
            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowInternationalLicenseInfo frm = new frmShowInternationalLicenseInfo(_NewInternationalLicenseID);
            frm.ShowDialog();
        }
    }
}
