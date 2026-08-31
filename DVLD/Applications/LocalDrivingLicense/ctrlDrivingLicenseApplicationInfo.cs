using DVLD.Licenses.Local_Licenses;
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

namespace DVLD.Applications.LocalDrivingLicense
{
    public partial class ctrlDrivingLicenseApplicationInfo : UserControl
    {

        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;

        private int _LocalDrivingLicenseApplicationID = -1;

        private int _LicenseID = -1;

        public int LocalDrivingLicenseApplicationID
        {
            get { return _LocalDrivingLicenseApplicationID; }
        }

        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
        }

        private async Task _FillControlWithDataAsync()
        {

            if (_LocalDrivingLicenseApplication != null)
            {
                _LicenseID = await _LocalDrivingLicenseApplication.GetActiveLicenseIDAsync();

                llShowLicenceInfo.Enabled = (_LicenseID != -1);

                lblLocalDrivingLicenseApplicationID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                lblAppliedFor.Text = (await _LocalDrivingLicenseApplication?.LicenseClassInfoAsync).ClassName;
                lblPassedTests.Text = _LocalDrivingLicenseApplication.GetPassedTestCountAsync().ToString();
                await ctrlApplicationBasicInfo1.LoadApplicationBasicInfoAsync(_LocalDrivingLicenseApplication.ApplicationID);
            }
        }

        public void ResetDefaultData()
        {
            lblLocalDrivingLicenseApplicationID.Text = "[???]";
            lblPassedTests.Text = "0";
            lblAppliedFor.Text = "[???]";
            _LocalDrivingLicenseApplicationID = -1;
            _LicenseID = -1;
            _LocalDrivingLicenseApplication = null;
            llShowLicenceInfo.Enabled = false;
            ctrlApplicationBasicInfo1.Reset();
        }

        public async Task LoadDataAsync(int LocalDrivingLicenseApplicationID)
        {
            lblLocalDrivingLicenseApplicationID.Text = LocalDrivingLicenseApplicationID.ToString();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _LocalDrivingLicenseApplication = await clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseIDAsync(this.LocalDrivingLicenseApplicationID);

            if( _LocalDrivingLicenseApplication != null )
            {
                await _FillControlWithDataAsync();
            }
            else
            {
                ResetDefaultData();
            }
        }

        private void llShowLicenceInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo(_LicenseID);
            frm.ShowDialog();
        }
    }
}
