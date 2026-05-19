using DVLD.Global_Classes;
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
    public partial class ctrlScheduleTest : UserControl
    {
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

            dtpTestDate.MinDate = DateTime.Today;
            btnSave.Enabled = false;
        }


        ///////////////////////////////////////////////////////////
        // Data

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
            lblFees.Text = "[$$$]";
            lblRetakeTestAppID.Text = "[??]";
            lblRetakeAppFees.Text = "[$$$]";
            lblTotalFees.Text = "[$$$]";

            btnSave.Enabled = false;
            lblUserMessage.Visible = true;
        }

        private void _DisplayRetakeTestData()
        {
            decimal fees = _LocalDrivingLicenseApplication.PaidFees;

            if (_CreationMode == enCreationMode.RetakeTestSchedule)
            {
                lblRetakeTestAppID.Text = _TestAppointment.RetakeTestAppInfo.ApplicationID.ToString();
                decimal retakeFees = _TestAppointment.RetakeTestAppInfo.PaidFees;
                lblRetakeAppFees.Text = $"${retakeFees}";
                decimal totalFees = fees + retakeFees;
                lblTotalFees.Text = $"${totalFees}";
            }
            else
            {
                lblRetakeTestAppID.Text = "N/A";
                lblRetakeAppFees.Text = "N/A";
                lblTotalFees.Text = $"${fees}";
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
                decimal fees = _LocalDrivingLicenseApplication.PaidFees;
                lblFees.Text = $"${fees}";

                _DisplayRetakeTestData();

                lblUserMessage.Visible = false;
            }
            else
            {
                ResetDefaultData();
            }
        }

        private void _FillNewReTakeTestAppObj()
        {
            if (_TestAppointment.RetakeTestAppInfo.Mode == clsApplication.enMode.AddNew)
            {
                _TestAppointment.RetakeTestAppInfo.PersonInfo = _LocalDrivingLicenseApplication.PersonInfo;
                _TestAppointment.RetakeTestAppInfo.ApplicationTypeInfo = clsApplicationType.Find((int)clsApplication.enApplicationType.RetakeTest);
                _TestAppointment.RetakeTestAppInfo.CreatedByUserInfo = clsGlobal.CurrentUser;

                _TestAppointment.RetakeTestAppInfo.ApplicantPersonID = _TestAppointment.RetakeTestAppInfo.PersonInfo.PersonID;
                _TestAppointment.RetakeTestAppInfo.ApplicationTypeID = (clsApplication.enApplicationType)_TestAppointment.RetakeTestAppInfo.ApplicationTypeInfo.ApplicationTypeID;
                _TestAppointment.RetakeTestAppInfo.CreatedByUserID = clsGlobal.CurrentUser.UserID;

                _TestAppointment.RetakeTestAppInfo.ApplicationStatus = clsApplication.enApplicationStatus.New;
                _TestAppointment.RetakeTestAppInfo.PaidFees = _TestAppointment.RetakeTestAppInfo.ApplicationTypeInfo.ApplicationTypeFees;
                _TestAppointment.RetakeTestAppInfo.LastStatusDate = DateTime.Now;
                _TestAppointment.RetakeTestAppInfo.ApplicationDate = DateTime.Now;

            }
        }

        private void _FillTestAppointmentObj()
        {
            if (_Mode == enMode.AddNew)
            {
                _TestAppointment.CreatedByUserID = clsGlobal.CurrentUser.UserID;
                _TestAppointment.LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID;
                _TestAppointment.TestTypeID = _TestTypeID;
            }
            _TestAppointment.AppointmentDate = dtpTestDate.Value;
        }

        private bool _SaveData()
        {
            _FillTestAppointmentObj();

            if (_CreationMode == enCreationMode.RetakeTestSchedule)
            {
                if (!_TestAppointment.RetakeTestAppInfo.Save())
                {
                    return false;
                }

                _TestAppointment.RetakeTestApplicationID = _TestAppointment.RetakeTestAppInfo.ApplicationID;
            }


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
        // Business

        private void _HandleMode()
        {
            // I have to add method to check if faild in test
            //throw new NotImplementedException();



            _IsPassedCurrentTest = clsLocalDrivingLicenseApplication.DosPassTest(_LocalDrivingLicenseApplicationID, _TestTypeID);
            bool IsLocked = clsTestAppointment.IsTestAppointmentLocked(_LocalDrivingLicenseApplicationID, _TestTypeID);

            if (_Mode == enMode.AddNew)
            {
                if (!_IsPassedCurrentTest && IsLocked)
                    _CreationMode = enCreationMode.RetakeTestSchedule;
                else
                    _CreationMode = enCreationMode.FirstTimeSchedule;
            }

            if (_Mode == enMode.Update)
            {
                if (_TestAppointment.RetakeTestApplicationID != -1)
                    _CreationMode = enCreationMode.RetakeTestSchedule;
                else
                    _CreationMode = enCreationMode.FirstTimeSchedule;
            }
            
        }

        private void _HandleReTakeTestApp()
        {
            if (_CreationMode == enCreationMode.RetakeTestSchedule)
            {
                if (_TestAppointment.RetakeTestAppInfo == null)
                {
                    _TestAppointment.RetakeTestAppInfo = clsTestAppointment.GetNewReTakeTestObj(LocalApp: _LocalDrivingLicenseApplication, user: clsGlobal.CurrentUser);
                    _FillNewReTakeTestAppObj();
                }
            }
        }

        private bool _HandelObjects(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestType, int TestAppointmentID)
        {
            _Mode = enMode.Update;

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestTypeID = TestType;
            _TestAppointmentID = TestAppointmentID;

            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);
            _TestAppointment = clsTestAppointment.Find(_TestAppointmentID);

            if (_TestAppointment != null)
            {
                _TestAppointmentID = _TestAppointment.TestAppointmentID;
            }

            return _LocalDrivingLicenseApplication != null && _TestAppointment != null;

        }

        private bool _HandelObjects(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestType)
        {
            _Mode = enMode.AddNew;

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestTypeID = TestType;

            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

           
                _TestAppointment = clsTestAppointment.GetNewTestAppointmentObject(LocalDrivingLicenseApplicationID, TestTypeID);

            return _LocalDrivingLicenseApplication != null && _TestAppointment != null;


        }


        private bool _HandelBusinessRolls()
        {


            if (TestTypeID == clsTestType.enTestType.None)
            {
                lblUserMessage.Text = "Please select a test type.";
                return false;
            }

            btnSave.Enabled = !_TestAppointment.IsLocked;

            if (TestTypeID == clsTestType.enTestType.VisionTest)
            {
                if (_IsPassedCurrentTest)
                {
                    lblUserMessage.Text = "You have already passed the vision test, you cannot schedule it again.";
                    return false;
                }
                else
                    return true;
            }

            if (clsTestAppointment.IsTestAppointmentInTheRightOrder(_LocalDrivingLicenseApplicationID, TestTypeID))
            {
                IsPassedPreviosTest = clsLocalDrivingLicenseApplication.DosPassPreviousTest(_LocalDrivingLicenseApplicationID, TestTypeID);

                if (_CreationMode == enCreationMode.FirstTimeSchedule)
                {
                    if (!IsPassedPreviosTest)
                    {
                        lblUserMessage.Text = "You must pass the previous test before scheduling a new one.";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (_CreationMode == enCreationMode.RetakeTestSchedule)
                {
                    if (_IsPassedCurrentTest)
                    {
                        lblUserMessage.Text = "You have already passed the previous test, you cannot schedule a retake test.";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Business
        //////////////////////////////////////////////////////////////

        public bool LoadData(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int TestAppointmentID = -1)
        {

            // TODO: most overloading this method to handle mode status.
            if (TestAppointmentID != -1)
            {
                if (!_HandelObjects(LocalDrivingLicenseApplicationID, TestTypeID, TestAppointmentID))
                {
                    lblUserMessage.Text = "Failed to load data. Please check the provided IDs.";
                    return false;
                }
            }
            else
            {
                if (!_HandelObjects(LocalDrivingLicenseApplicationID, TestTypeID))
                {
                    lblUserMessage.Text = "Failed to load data. Please check the provided IDs.";
                    return false;
                }
            }

            _HandleMode();

            _HandleReTakeTestApp();


            if (!_HandelBusinessRolls())
            {
                ResetDefaultData();
                return false;
            }


            if (_LocalDrivingLicenseApplication != null && _TestAppointment != null)
            {
                _DisplayData();
                return true;
            }
            else
            {
                ResetDefaultData();
                return false;
            }
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
                    lblRetakeTestAppID.Text = _TestAppointment.RetakeTestAppInfo.ApplicationID.ToString();
            }
            else
            {
                MessageBox.Show("Failed to save data. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
