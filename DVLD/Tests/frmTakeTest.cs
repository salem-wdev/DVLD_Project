using DVLD.Global_Classes;
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
            if (_Test != null)
            {
                _Test.TestResult = rbPass.Checked;
                _Test.Notes = txtNotes.Text;
                _Test.TestAppointmentID = ctrlSecheduledTest1.TestAppointment.TestAppointmentID;
                _Test.CreatedByUserID = clsGlobal.CurrentUser.UserID;
                _Test.TestAppointmentInfo = ctrlSecheduledTest1.TestAppointment;
            }
        }

        private void _LockTestScreen()
        {
            btnSave.Enabled = false;
            lblUserMessage.Visible = true;
            lblUserMessage.Text = "This test has already been taken.";
            txtNotes.Enabled = false;
        }

        private void _LoadTest()
        {
            _TestID = ctrlSecheduledTest1.TestAppointment.TestID;

            if (_TestID != -1)
            {
                _Test = clsTest.Find(_TestID);

                _LockTestScreen();

                _FillFormWithData();
            }
            else
            {
                _Test = new clsTest();
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
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_Test != null)
            {
                if (clsTestAppointment.IsTestAppointmentLocked(ctrlSecheduledTest1.TestAppointment.LocalDrivingLicenseApplicationID, _TestType))
                {
                    MessageBox.Show("This test appointment is locked. You cannot save changes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _LockTestScreen();
                    return;
                }

                _FillTestObjectWithData();

                if (_Test.Save())
                {
                    _LockTestScreen();
                    _TestID = _Test.TestID;
                    MessageBox.Show("Test data saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("An error occurred while saving the test data. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No test data available to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _LockTestScreen();
                return;
            }
        }
    }
}
