using DVLD.Global_Classes;
using DVLD_Business;
using DVLD_Business.Global_Classes;
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
            _License = _LocalDrivingLicenseApplication.IssueFirstTimeLocalLicense(clsGlobal.CurrentUser.UserID, txtNotes.Text);

            if (_License != null)
            {
                MessageBox.Show("License issued successfully.");
                btnIssueLicense.Enabled = false;
                _LicenseID = _License.LicenseID;
                _DriverID = _License.DriverID;
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
