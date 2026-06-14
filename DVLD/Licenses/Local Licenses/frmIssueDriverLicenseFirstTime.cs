using DVLD.Global_Classes;
using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD.Licenses.Local_Licenses
{
    public partial class frmIssueDriverLicenseFirstTime : Form
    {
        int _LocalDriverLicenseAppID = -1;
        int _LicenseID = -1;
        int _DriverID = -1;
        clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;
        clsLicense _License;
        clsDriver _Driver;

        public frmIssueDriverLicenseFirstTime(int LocalDriverLicenseAppID)
        {
            InitializeComponent();
            _LocalDriverLicenseAppID = LocalDriverLicenseAppID;
        }

        private bool _GetDeiver()
        {
            _Driver = clsDriver.FindByPersonID(_LocalDrivingLicenseApplication.ApplicantPersonID);

            if (_Driver == null)
            {
                _Driver = clsDriver.CreateNewDriver(_LocalDrivingLicenseApplication.ApplicantPersonID, clsGlobal.CurrentUser.UserID);
                if (_Driver.Save())
                {
                _DriverID = _Driver.DriverID;
                    return true;
                }
            }

            _DriverID = _Driver.DriverID;
            return _DriverID > -1;
        }

        private void _FillLicenseObj()
        {
            _License = clsLicense.GetNewLicenseObj(_LocalDrivingLicenseApplication.ApplicationID,
                _DriverID, _LocalDrivingLicenseApplication.LicenseClassID, clsGlobal.CurrentUser.UserID, _LocalDriverLicenseAppID);
            if (_License != null)
            {

                
                _License.Notes = txtNotes.Text;
            }
            else
            {
                MessageBox.Show("Error preparing license information.");
            }
        }

        private void frmIssueDriverLicenseFirstTime_Load(object sender, EventArgs e)
        {
            ctrlDrivingLicenseApplicationInfo1.LoadData(_LocalDriverLicenseAppID);
            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_LocalDriverLicenseAppID);
            if (_LocalDrivingLicenseApplication != null)
            {
                if (clsLicense.GetActiveLicenseIDByPersonID(_LocalDrivingLicenseApplication.ApplicantPersonID, _LocalDrivingLicenseApplication.LicenseClassID) != -1)
                {
                    MessageBox.Show("There is an active license for this person.");
                    btnIssueLicense.Enabled = false;
                    return;
                }
            }
            else
            {
                MessageBox.Show("Error loading the local driving license application.");
                btnIssueLicense.Enabled = false;
                return;
            }
            
        }

        private void btnIssueLicense_Click(object sender, EventArgs e)
        {
            if (!_GetDeiver())
            {
                MessageBox.Show("Error saving driver information.");
                btnIssueLicense.Enabled = false;
                return;
            }

            _FillLicenseObj();

            if (_License.Save())
            {
                MessageBox.Show("License issued successfully.");
                btnIssueLicense.Enabled = false;
                _LicenseID = _License.LicenseID;
                return;
            }
            else
            {
                MessageBox.Show("Error saving license information.");
                btnIssueLicense.Enabled = false;
            }
        }
    }
}
