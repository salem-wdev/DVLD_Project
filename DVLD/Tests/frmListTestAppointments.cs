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

namespace DVLD.Tests
{
    public partial class frmListTestAppointments : Form
    {

        DataTable _dtAllTestAppointmentsList;

        int _LocalDrivingLicenseApplicationID = -1;
        clsTestType.enTestType _TestTypeID = clsTestType.enTestType.None;
        clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;
        

        public frmListTestAppointments(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            InitializeComponent();

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestTypeID = TestTypeID;
        }

        private void _RefreshData()
        {
            _dtAllTestAppointmentsList = clsTestAppointment.GetApplicationTestAppointmentsPerTestType(_LocalDrivingLicenseApplicationID, _TestTypeID);
            dgvLicenseTestAppointments.DataSource = _dtAllTestAppointmentsList;
        }

        private void _ResetDefautData()
        {
            pbTestTypeImage.Image = Properties.Resources.Vision_512;
            dgvLicenseTestAppointments.DataSource = null;
            lblRecordsCount.Text = "??";
            lblTitle.Text = "Vision Test Appointments";
            ctrlDrivingLicenseApplicationInfo1.ResetDefaultData();
        }

        private void _LoadTestTypeImageAndTitle()
        {
            switch (_TestTypeID)
            {

                case clsTestType.enTestType.None:
                    {
                        _ResetDefautData();
                        break;
                    }

                case clsTestType.enTestType.VisionTest:
                    {
                        lblTitle.Text = "Vision Test Appointments";
                        this.Text = lblTitle.Text;
                        pbTestTypeImage.Image = Resources.Vision_512;
                        break;
                    }

                case clsTestType.enTestType.WrittenTest:
                    {
                        lblTitle.Text = "Written Test Appointments";
                        this.Text = lblTitle.Text;
                        pbTestTypeImage.Image = Resources.Written_Test_512;
                        break;
                    }
                case clsTestType.enTestType.StreetTest:
                    {
                        lblTitle.Text = "Street Test Appointments";
                        this.Text = lblTitle.Text;
                        pbTestTypeImage.Image = Resources.driving_test_512;
                        break;
                    }
            }
        }

        private void _FillFormWithData()
        {

            _LoadTestTypeImageAndTitle();

            ctrlDrivingLicenseApplicationInfo1.LoadData(_LocalDrivingLicenseApplicationID);

            _RefreshData();

        }

        private void frmListTestAppointments_Load(object sender, EventArgs e)
        {
            bool IsPassedPreviousTest = false;

            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_LocalDrivingLicenseApplicationID);

            IsPassedPreviousTest = clsLocalDrivingLicenseApplication.DosPassPreviousTest(_LocalDrivingLicenseApplicationID, _TestTypeID);

            if (_LocalDrivingLicenseApplication != null && IsPassedPreviousTest)
            {
                _FillFormWithData();
            }
            else
            {
                MessageBox.Show("Error loading test appointments. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void btnAddNewAppointment_Click(object sender, EventArgs e)
        {
            frmScheduleTest frm = new frmScheduleTest(_LocalDrivingLicenseApplicationID, _TestTypeID);
            frm.ShowDialog();
        }
    }
}
