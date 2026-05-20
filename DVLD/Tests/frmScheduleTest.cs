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
    public partial class frmScheduleTest : Form
    {
        private int _LocalDrivingLicenseApplicationID = -1;
        private int _TestAppointmentID = -1;
        public int LocalDrivingLicenseApplicationID
        {
            get => _LocalDrivingLicenseApplicationID;
        }
        public int TestAppointmentID
        {
            get => _TestAppointmentID;
        }
        private clsTestType.enTestType _TestType;

        public frmScheduleTest(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestType,int TestAppointmentID = -1)
        {
            InitializeComponent();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestAppointmentID = TestAppointmentID;
            _TestType = TestType;
        }


        private void frmScheduleTest_Load(object sender, EventArgs e)
        {
            if (_TestAppointmentID == -1)
            {
                ctrlScheduleTest1.LoadData(_LocalDrivingLicenseApplicationID, _TestType);
            }
            else
            {
                ctrlScheduleTest1.LoadData(_LocalDrivingLicenseApplicationID, _TestType, _TestAppointmentID);

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
