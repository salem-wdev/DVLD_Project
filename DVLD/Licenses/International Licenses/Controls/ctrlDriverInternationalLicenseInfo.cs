using DVLD.Classes;
using DVLD.Properties;
using DVLD_Business;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Licenses.International_Licenses.Controls
{
    public partial class ctrlDriverInternationalLicenseInfo : UserControl
    {
        private int _InternationalLicenseID;
        private clsInternationalLicense _InternationalLicense;

        public ctrlDriverInternationalLicenseInfo()
        {
            InitializeComponent();
        }

        public int LicenseID
        {
            get { return _InternationalLicenseID; }
        }

        public clsInternationalLicense SelectedLicenseInfo
        { get { return _InternationalLicense; } }

        private void _LoadPersonImage()
        {
            if (_InternationalLicense.DriverInfo.PersonInfo.Gender == 0)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            string ImagePath = _InternationalLicense.DriverInfo.PersonInfo.ImagePath;

            if (ImagePath != "")
                if (File.Exists(ImagePath))
                    pbPersonImage.Load(ImagePath);
                else
                    MessageBox.Show("Could not find this image: = " + ImagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void _FillControlWithData()
        {
            lblFullName.Text = _InternationalLicense.DriverInfo.PersonInfo.FullName;
            lblInternationalLicenseID.Text = _InternationalLicense.InternationalLicenseID.ToString();
            lblLocalLicenseID.Text = clsLicense.GetActiveLicenseIDByDriverID(_InternationalLicense.DriverID, 3).ToString();
            lblNationalNo.Text = _InternationalLicense.DriverInfo.PersonInfo.NationalNo;
            lblGendor.Text = _InternationalLicense.DriverInfo.PersonInfo.Gender == 0 ? "Male" : "Female";
            lblIssueDate.Text = clsFormat.DateToShort(_InternationalLicense.IssueDate);
            lblApplicationID.Text = _InternationalLicense.ApplicationID.ToString();
            lblIsActive.Text = _InternationalLicense.IsActive ? "Yes" : "No";
            lblDateOfBirth.Text = clsFormat.DateToShort(_InternationalLicense.DriverInfo.PersonInfo.DateOfBirth);

            lblDriverID.Text = _InternationalLicense.DriverID.ToString();
            lblExpirationDate.Text = clsFormat.DateToShort(_InternationalLicense.ExpirationDate);
            _LoadPersonImage();
        }

        private void _ResetControl()
        {
            _InternationalLicenseID = -1;
            _InternationalLicense = null;

            lblFullName.Text = "[????]";
            lblInternationalLicenseID.Text = "[???]";
            lblLocalLicenseID.Text = "[????]";
            lblNationalNo.Text = "[????]";
            lblGendor.Text = "[????]";
            lblIssueDate.Text = "[????]";
            lblApplicationID.Text = "[???]";
            lblIsActive.Text = "[????]";
            lblDateOfBirth.Text = "[????]";

            lblDriverID.Text = "[????]";
            lblExpirationDate.Text = "[????]";
            

            pbPersonImage.Image = Resources.Male_512;
        }

        public void ClearData()
        {
            _ResetControl();
        }

        public bool LoadData(int LicenseID)
        {
            _InternationalLicenseID = LicenseID;
            _InternationalLicense = clsInternationalLicense.Find(_InternationalLicenseID);
            if (_InternationalLicense == null)
            {
                MessageBox.Show("Could not find License ID = " + _InternationalLicenseID.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _InternationalLicenseID = -1;
                return false;
            }
            else
            {
                _FillControlWithData();
                return true;
            }

        }
    }
}
