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
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Applications.Local_Driving_License
{
    public partial class frmAddUpdateLocalDrivingLicenseApplication : Form
    {

        private bool _IsFormUpdated = false;
        private bool _isDataLoaded = false;
        private enum enMode
        {
            AddNew = 1,
            Update = 2
        }

        enMode _Mode;

        private int _ApplicationID;
        private clsLocalDrivingLicenseApplication _LocalLicenseApplication;

        public frmAddUpdateLocalDrivingLicenseApplication()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdateLocalDrivingLicenseApplication(int ApplicationID)
        {
            InitializeComponent();
            _Mode = enMode.Update;
            _ApplicationID = ApplicationID;
        }

        /// <summary>
        /// Logic Methods
        /// </summary>

        private void _ResetDefaultValues(bool ResetPersonInfo)
        {
            if (_Mode == enMode.AddNew)
            {
                _ApplicationID = -1;
                _LocalLicenseApplication = null;
                lblLocalDrivingLicebseApplicationID.Text = "[???]";
                lblApplicationDate.Text = DateTime.Now.ToShortDateString();
                cbLicenseClass.SelectedIndex = 0;
                lblFees.Text = "15";
                lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
                
                if(ResetPersonInfo)
                    ctrlPersonCardWithFilter1.ctrlPersonCard1.ResetPersonInfo();
                
                tpApplicationInfo.Enabled = false;
                btnApplicationInfoNext.Enabled = false;

                lblTitle.Text = "Add New Local Driving License Application";
                this.Text = "Add New Local Driving License Application";


            }
            else if (_Mode == enMode.Update)
            {

                _FillFormControls();
                btnSave.Enabled = true;

                ctrlPersonCardWithFilter1.gbFilters.Enabled = false;

                lblTitle.Text = "Update Local Driving License Application";
                this.Text = "Update Local Driving License Application";

            }
        }

        private bool _FillFormControls()
        {
            if (_LocalLicenseApplication != null)
            {
                lblLocalDrivingLicebseApplicationID.Text = _LocalLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                cbLicenseClass.SelectedValue = _LocalLicenseApplication.LicenseClassID;
                lblApplicationDate.Text = _LocalLicenseApplication.ApplicationDate.ToShortDateString();
                lblFees.Text = "15";
                lblCreatedByUser.Text = _LocalLicenseApplication.CreatedByUserInfo.UserName;
                ctrlPersonCardWithFilter1.ctrlPersonCard1.LoadData(_LocalLicenseApplication.PersonInfo);

                return true;
            }
            else
                return false;
            
        }

            
        private void _FillcbLicenseClassWithData()
        {
            cbLicenseClass.DataSource = clsLicenseClass.GetAllLicenseClasses();
            cbLicenseClass.DisplayMember = "ClassName";
            cbLicenseClass.ValueMember = "LicenseClassID";
            cbLicenseClass.SelectedIndex = 0;
        }

        private bool _SaveApplication()
        {
            if (_LocalLicenseApplication != null)
            {
                if (_Mode == enMode.Update)
                {
                    _LocalLicenseApplication.LicenseClassID = (int)cbLicenseClass.SelectedValue;
                }
                if (_LocalLicenseApplication.Save())
                {
                    _Mode = enMode.Update;
                    return true;
                }

                return false;
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Logic Event Handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void frmAddUpdateLocalDrivingLicesnseApplication_Load(object sender, EventArgs e)
        {
            _FillcbLicenseClassWithData();

            ctrlPersonCardWithFilter1.OnPersonSelected += CtrlPersonCardWithFilter1_OnPersonSelected;

            if (_Mode == enMode.Update)
            {
                _LocalLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_ApplicationID);
                if (_LocalLicenseApplication == null)
                {
                    MessageBox.Show("Application not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }
            _ResetDefaultValues(true);

        }

        private void CtrlPersonCardWithFilter1_OnPersonSelected(int obj)
        {
            if (_IsFormUpdated)
            {
                _Mode = enMode.AddNew;

                _ResetDefaultValues(false);
            }

            if (obj != -1)
            {
                btnApplicationInfoNext.Enabled = true;
                tpApplicationInfo.Enabled = true;
                btnSave.Enabled = true;
            }
            else
            {
                btnApplicationInfoNext.Enabled = false;
            }

            _LocalLicenseApplication = clsLocalDrivingLicenseApplication.GetNewLocalDrivingLicenseApp((int)cbLicenseClass.SelectedValue, clsGlobal.CurrentUser.UserID, ctrlPersonCardWithFilter1.ctrlPersonCard1.PersonID, clsApplication.enApplicationType.NewDrivingLicense);
            _isDataLoaded = true;
            _IsFormUpdated = true;

        }

        private void btnApplicationInfoNext_Click(object sender, EventArgs e)
        {
            tcApplicationInfo.SelectedIndex = 1;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _FillApplicationWithData();

            if (_Mode == enMode.AddNew && _LocalLicenseApplication.GetActiveApplicationID((clsApplication.enApplicationType)cbLicenseClass.SelectedValue) != -1)
            {
                MessageBox.Show("The selected person already has an active application of this type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_SaveApplication())
            {
                lblLocalDrivingLicebseApplicationID.Text = _LocalLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                lblTitle.Text = "Update Local Driving License Application";
                this.Text = "Update Local Driving License Application";
                MessageBox.Show("Application saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error saving application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cbLicenseClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!_isDataLoaded)
            {
                return;
            }

            if(_Mode == enMode.AddNew)
            {
                _LocalLicenseApplication = clsLocalDrivingLicenseApplication.GetNewLocalDrivingLicenseApp((int)cbLicenseClass.SelectedValue,
                    clsGlobal.CurrentUser.UserID, ctrlPersonCardWithFilter1.ctrlPersonCard1.PersonID, clsApplication.enApplicationType.NewDrivingLicense);
            }
        }
        ///////////////////////////////////////////////////////////////////

    }
}
