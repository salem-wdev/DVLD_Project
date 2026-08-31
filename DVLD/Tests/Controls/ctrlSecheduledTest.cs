using DVLD.Properties;
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

namespace DVLD.Tests.Controls
{
    public partial class ctrlSecheduledTest : UserControl
    {

        private clsTestType.enTestType _TestTypeID;
        private int _TestAppointmentID = -1;
        private int _LocalDrivingLicenseApplicationID = -1;
        private int _TestID = -1;

        private clsTestAppointment _TestAppointment;
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;

        public clsTestType.enTestType TestTypeID
        {
            get
            {
                return _TestTypeID;
            }
            set
            {
                _TestTypeID = value;

                switch (_TestTypeID)
                {
                    case clsTestType.enTestType.None:
                    case clsTestType.enTestType.VisionTest:
                        {
                            gbTestType.Text = "Vision Test";
                            pbTestTypeImage.Image = Resources.Vision_512;
                            break;
                        }

                    case clsTestType.enTestType.WrittenTest:
                        {
                            gbTestType.Text = "Written Test";
                            pbTestTypeImage.Image = Resources.Written_Test_512;
                            break;
                        }
                    case clsTestType.enTestType.StreetTest:
                        {
                            gbTestType.Text = "Street Test";
                            pbTestTypeImage.Image = Resources.driving_test_512;
                            break;


                        }
                }
            }
        }
        public int TestAppointmentID
        {
            get => _TestAppointmentID;
        }
        public int LocalDrivingLicenseApplicationID
        {
            get => _LocalDrivingLicenseApplicationID;
        }
        public int TestID
        {
            get => _TestID;
        }

        public clsTestAppointment TestAppointment
        {
            get => _TestAppointment;
        }
        public clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication
        {
            get => _LocalDrivingLicenseApplication;
        }


        public ctrlSecheduledTest()
        {
            InitializeComponent();
        }

        private async Task _FillControlWithDataAsync()
        {
            lblLocalDrivingLicenseAppID.Text = _TestAppointment.LocalDrivingLicenseApplicationID.ToString();
            lblDrivingClass.Text =(await _LocalDrivingLicenseApplication?.LicenseClassInfoAsync).ClassName;
            lblFullName.Text = _LocalDrivingLicenseApplication.PersonFullName;
            lblTrial.Text = _LocalDrivingLicenseApplication.TotalTrialsPerTest(_TestAppointment.TestTypeID).ToString();
            lblDate.Text = _TestAppointment.AppointmentDate.ToShortDateString();
            lblFees.Text = _TestAppointment.PaidFees.ToString();

            if (_TestID < 1)
            {
                lblTestID.Text = "Not Taken Yet";
            }
            else
            {
                lblTestID.Text = _TestID.ToString();
            }
        }

        private void _ResetDefaultData()
        {
            TestTypeID = clsTestType.enTestType.None;
            lblLocalDrivingLicenseAppID.Text = "[??]";
            lblDrivingClass.Text = "[???????]";
            lblFullName.Text = "[???????]";
            lblTrial.Text = "[??]";
            lblDate.Text = "[dd/mm/yyyy]";
            lblFees.Text = "[$$$]";
            lblTestID.Text = "Not Taken Yet";

            _TestTypeID = clsTestType.enTestType.None;
            _TestAppointmentID = -1;
            _LocalDrivingLicenseApplicationID = -1;
            _TestID = -1;

            _TestAppointment = null;
            _LocalDrivingLicenseApplication = null;
        }

        public void Reset()
        {
            _ResetDefaultData();
        }

        public async Task<bool> LoadDataAsync(int TestAppointmentID)
        {
            _TestAppointmentID = TestAppointmentID;

            _TestAppointment = await clsTestAppointment.FindAsync(_TestAppointmentID);

            if (_TestAppointment != null)
            {
                _LocalDrivingLicenseApplicationID = _TestAppointment.LocalDrivingLicenseApplicationID;
                _LocalDrivingLicenseApplication = await clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseIDAsync(_LocalDrivingLicenseApplicationID);
                TestTypeID = _TestAppointment.TestTypeID;
                _TestID = await _TestAppointment.TestIDAsync();
            }
            else
            {
                _ResetDefaultData();
                return false;
            }


            if (_LocalDrivingLicenseApplication != null)
            {
                await _FillControlWithDataAsync();
                return true;
            }
            else
            {
                _ResetDefaultData();
                return false;
            }
        }

    }
}
