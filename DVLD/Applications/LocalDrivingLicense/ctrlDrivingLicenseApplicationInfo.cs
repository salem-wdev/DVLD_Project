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

        private int _LicenseID;

        public int LocalDrivingLicenseApplicationID
        {
            get { return _LocalDrivingLicenseApplicationID; }
        }

        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
        }

        private void _FillControlWithData()
        {

            if (_LocalDrivingLicenseApplication != null)
            {
                _LicenseID = _LocalDrivingLicenseApplication.GetActiveLicenseID();

                llShowLicenceInfo.Enabled = (_LicenseID != -1);

                lblLocalDrivingLicenseApplicationID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                lblAppliedFor.Text = _LocalDrivingLicenseApplication.ApplicationTypeInfo.ApplicationTypeTitle;
                lblPassedTests.Text = _LocalDrivingLicenseApplication.GetPassedTestCount().ToString();
                ctrlApplicationBasicInfo1.LoadApplicationBasicInfo(_LocalDrivingLicenseApplication.ApplicationID);
                llShowLicenceInfo.Enabled = true;
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

        public void LoadData(int LocalDrivingLicenseApplicationID)
        {
            lblLocalDrivingLicenseApplicationID.Text = LocalDrivingLicenseApplicationID.ToString();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(this.LocalDrivingLicenseApplicationID);

            if( _LocalDrivingLicenseApplication != null )
            {
                _FillControlWithData();
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
