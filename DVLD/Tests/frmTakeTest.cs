using DVLD.Global_Classes;
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

namespace DVLD.Tests
{
    public partial class frmTakeTest : Form
    {
        private int _AppointmentID;
        private clsTestType.enTestType _TestType;

        private int _TestID = -1;
        private clsTest _Test;

        public frmTakeTest(int AppointmentID, clsTestType.enTestType TestType)
        {
            InitializeComponent();
            _AppointmentID = AppointmentID;
            _TestType = TestType;
        }

        private void _FillFormWithData()
        {
            if (_Test != null)
            {
                rbPass.Checked = (_Test.TestResult == true);
                rbFail.Checked = !rbPass.Checked;

                txtNotes.Text = _Test.Notes;
            }
        }

        private void _FillTestObjectWithData()
        {
            _Test = clsTest.GetNewTestObj(_AppointmentID, rbPass.Checked, txtNotes.Text, clsGlobal.CurrentUser.UserID);
        }

        private void _LockTestScreen(bool LockAllScreen, string message)
        {
            btnSave.Enabled = !LockAllScreen;
            txtNotes.Enabled = !LockAllScreen;

            rbPass.Enabled = false;
            rbFail.Enabled = false;

            lblUserMessage.Visible = true;
            lblUserMessage.Text = message;
        }

        private void _LoadTest()
        {
            _TestID = ctrlSecheduledTest1.TestAppointment.TestID;

            if(ctrlSecheduledTest1.TestAppointment.AppointmentDate<clsGlobal.GetServerDateTime())
            {
                _LockTestScreen(true, "Appointment has already passed.");
                return;
            }

            if (_TestID != -1)
            {
                _Test = clsTest.Find(_TestID);
                if (_Test != null)
                {
                    this.Text = _TestType.ToString() + " Test";

                    _LockTestScreen(false, "This test has already been taken.");

                    _FillFormWithData();
                }
                else
                {
                    _LockTestScreen(true, "Can not load test data");
                }
            }
            else
            {
                btnSave.Enabled = true;
                lblUserMessage.Visible = false;
                this.Text = "Take " + _TestType.ToString() + " Test";
            }
        }

        private void frmTakeTest_Load(object sender, EventArgs e)
        {
            if(ctrlSecheduledTest1.LoadData(_AppointmentID))
            {
                _LoadTest();
            }
            else
            {
                _LockTestScreen(true, "Unable to load test appointment data. Please try again later.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (clsTestAppointment.IsTestAppointmentLocked(ctrlSecheduledTest1.TestAppointment.LocalDrivingLicenseApplicationID, _TestType))
            //{
            //    MessageBox.Show("This test appointment is locked. You cannot save changes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    _LockTestScreen(false, "This test appointment is locked. You cannot save changes.");
            //    return;
            //}


            if (MessageBox.Show("Are you sure you want to save this result." +
                "\nCan not change this result.", "Warning", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) 
                == DialogResult.No)
            {
                return;
            }

            if (_Test == null)
            {
                _FillTestObjectWithData();
            }
            else
            {
                _Test.Notes = txtNotes.Text;
            }

            if (_Test != null)
            {
                if (_Test.Save())
                {
                    _LockTestScreen(false, "Test data saved successfully.");
                    _TestID = _Test.TestID;
                }
                else
                {
                    MessageBox.Show("An error occurred while saving the test data. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No test data available to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _LockTestScreen(true, "No test data available to save.");
                return;
            }
        }
    }
}
