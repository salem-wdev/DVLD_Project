using DVLD.Global_Classes;
using DVLD.Properties;
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

namespace DVLD.Tests.Controls
{
    public partial class ctrlScheduleTest : UserControl
    {

        // ===================================================================================================
        // TODO: UI REFACTOR & TECH DEBT CLEANUP (Execute after project completion)
        // ===================================================================================================
        // 1. Remove dead/legacy business variables (e.g., _IsPassedCurrentTest, IsPassedPreviosTest).
        // 2. Delete '_HandelBusinessRolls()' method completely; UI should not enforce business rules.
        // 3. Simplify 'LoadData()' to rely strictly on the object returned from BLL (if null -> block save).
        // 4. Implement 'out string errorMessage' in BLL to pass exact rejection reasons to 'lblUserMessage'.
        // 5. Move any remaining fee arithmetic or financial calculations from '_DisplayData' to the BLL.
        // ===================================================================================================

        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;
        public enum enCreationMode { FirstTimeSchedule = 0, RetakeTestSchedule = 1 };
        private enCreationMode _CreationMode = enCreationMode.FirstTimeSchedule;


        private clsTestType.enTestType _TestTypeID = clsTestType.enTestType.VisionTest;
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;
        private int _LocalDrivingLicenseApplicationID = -1;
        private clsTestAppointment _TestAppointment;
        private int _TestAppointmentID = -1;

        private bool _IsPassedCurrentTest = false;
        private bool IsPassedPreviosTest = false;

        // Using interface instead of solid class for more extensibility
        public IButtonControl SaveButtonAction => btnSave;


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

        public ctrlScheduleTest()
        {
            InitializeComponent();

            dtpTestDate.MinDate = DateTime.Now.AddMinutes(5);
            dtpTestDate.Value = DateTime.Now.AddMinutes(10);
            btnSave.Enabled = false;
        }


        ///////////////////////////////////////////////////////////
        // Data

        
        //private void _FillFees()
        //{
        //    if (_TestAppointment != null)
        //    {
        //        lblFees.Text = 
        //    }
        //}
        private void _ClearData()
        {
            _LocalDrivingLicenseApplicationID = -1;
            _TestAppointmentID = -1;
            _LocalDrivingLicenseApplication = null;
            _TestAppointment = null;
            dtpTestDate.Value = DateTime.Today;
        }

        public void ResetDefaultData()
        {
            _ClearData();
            TestTypeID = clsTestType.enTestType.None;
            lblLocalDrivingLicenseAppID.Text = "[??]";
            lblDrivingClass.Text = "[???????]";
            lblFullName.Text = "[???????]";
            lblTrial.Text = "[??]";
            dtpTestDate.Value = DateTime.Today;
            lblFees.Text = "[N/A]";
            lblRetakeTestAppID.Text = "[N/A]";
            lblRetakeAppFees.Text = "[$$$]";
            lblTotalFees.Text = "[$$$]";

            btnSave.Enabled = false;
            lblUserMessage.Visible = true;
        }

        private void _DisplayRetakeTestData()
        {

            if (_CreationMode == enCreationMode.RetakeTestSchedule)
            {
                if (_TestAppointment.RetakeTestAppInfo != null)
                {
                    int appID = _TestAppointment.RetakeTestAppInfo.ApplicationID;
                    lblRetakeTestAppID.Text = appID > 0 ? appID.ToString() : "N/A";
                    lblRetakeAppFees.Text = $"${_TestAppointment.RetakeTestAppInfo.PaidFees}";
                    lblFees.Text = $"${_TestAppointment.PaidFees - (float)_TestAppointment.RetakeTestAppInfo.PaidFees}";                    

                }
            }
            else
            {
                lblRetakeTestAppID.Text = "N/A";
                lblRetakeAppFees.Text = "N/A";
            }
            lblTotalFees.Text = $"${_TestAppointment.PaidFees}";
            //lblFees.Text =
        }

        private void _DisplayAppointmentData()
        {
            if(_TestAppointment != null)
            {
                btnSave.Enabled = true;
                lblFees.Text = $"${_TestAppointment.PaidFees}";

                _DisplayRetakeTestData();
            }
        }

        private void _DisplayData()
        {
            if (_LocalDrivingLicenseApplication != null)
            {
                lblLocalDrivingLicenseAppID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                lblDrivingClass.Text = _LocalDrivingLicenseApplication.LicenseClassInfo.ClassName;
                lblFullName.Text = _LocalDrivingLicenseApplication.PersonFullName;
                lblTrial.Text = _LocalDrivingLicenseApplication.TotalTrialsPerTest(TestTypeID).ToString();


                _DisplayAppointmentData();

                lblUserMessage.Visible = false;
            }
            else
            {
                ResetDefaultData();
            }
        }

        private void _FillTestAppointmentObj()
        {
            if (_TestAppointment == null)
            {
                _TestAppointment = clsTestAppointment.GetNewTestAppointmentObject(_LocalDrivingLicenseApplicationID, TestTypeID, clsGlobal.CurrentUser.UserID, dtpTestDate.Value);
            }
            _TestAppointment.AppointmentDate = dtpTestDate.Value;
        }

        private bool _SaveData()
        {
            _FillTestAppointmentObj();

            if (_TestAppointment.Save())
            {
                _Mode = enMode.Update;

                return true;
            }
            else
            {
                return false;
            }
        }

        // Data
        //////////////////////////////////////////////////////////////
        

        public bool LoadData(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this.TestTypeID = TestTypeID;
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_LocalDrivingLicenseApplicationID);

            if (_LocalDrivingLicenseApplication == null)
            {
                MessageBox.Show("Error: No Local Driving License Application with ID = " + _LocalDrivingLicenseApplicationID.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                return false;
            }

            _TestAppointment = clsTestAppointment.GetNewTestAppointmentObject(LocalDrivingLicenseApplicationID,
                    TestTypeID, clsGlobal.CurrentUser.UserID, dtpTestDate.Value);
            _Mode = enMode.AddNew;

            if (_TestAppointment != null)
            {
                if (_TestAppointment.RetakeTestAppInfo != null)
                {
                    _CreationMode = enCreationMode.RetakeTestSchedule;
                }
                else
                {
                    _CreationMode = enCreationMode.FirstTimeSchedule;
                }


            }
            else
            {
                lblUserMessage.Text = "TestAppointment Can not load Test Appointment data";
                lblUserMessage.Visible = true;
                btnSave.Enabled = false;
                return false;
            }

            
            _DisplayData();

            return true;
        }

        public bool LoadData(int TestAppointmentID)
        {


            _TestAppointment = clsTestAppointment.Find(TestAppointmentID);
            this.TestTypeID = _TestAppointment.TestTypeID;
            if (_TestAppointment != null)
            {
                if (_TestAppointment.RetakeTestAppInfo != null)
                {
                    _CreationMode = enCreationMode.RetakeTestSchedule;
                }
                else
                {
                    _CreationMode = enCreationMode.FirstTimeSchedule;
                }

            }
            else
            {
                lblUserMessage.Text = "TestAppointment Can not load Test Appointment data";
                lblUserMessage.Visible = true;
                btnSave.Enabled = false;
                return false;
            }

            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_TestAppointment.LocalDrivingLicenseApplicationID);
            _LocalDrivingLicenseApplicationID = _TestAppointment.LocalDrivingLicenseApplicationID;


            _DisplayData();

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save changes? \nYou can't change this later.", "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            if (_SaveData())
            {
                MessageBox.Show("Data saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (_CreationMode == enCreationMode.RetakeTestSchedule)
                {
                    int appID = _TestAppointment.RetakeTestAppInfo.ApplicationID;
                    lblRetakeTestAppID.Text = appID > 0 ? appID.ToString() : "N/A";
                }
            }
            else
            {
                MessageBox.Show("Failed to save data. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
