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

namespace DVLD.Licenses.Local_Licenses
{
    public partial class frmShowPersonLicenseHistory : Form
    {
        private int _PersonID;

        public frmShowPersonLicenseHistory()
        {
            InitializeComponent();
        }

        public frmShowPersonLicenseHistory(int personID)
        {
            InitializeComponent();
            _PersonID = personID;
        }

        private void frmShowPersonLicenseHistory_Load(object sender, EventArgs e)
        {
            if (_PersonID > 0)
            {
                ctrlPersonCardWithFilter1.FilterEnabled = false;
                ctrlPersonCardWithFilter1.LoadPersonInfo(_PersonID);
                ctrlDriverLicenses1.LoadInfoByPersonID(_PersonID);
            }
            else
            {
                ctrlPersonCardWithFilter1.FilterEnabled = true;
                AcceptButton = ctrlPersonCardWithFilter1.AcceptButton;
            }

            ctrlPersonCardWithFilter1.OnPersonSelected += CtrlPersonCardWithFilter1_OnPersonSelected;
        }

        private void CtrlPersonCardWithFilter1_OnPersonSelected(int obj)
        {
            _PersonID = obj;
            clsDriver driver = clsDriver.FindByPersonID(_PersonID);
            ctrlDriverLicenses1.ResetData();
            if (driver == null)
            {
                MessageBox.Show("Selected person is not a driver!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FilterFocus();
                return;
            }

            if (_PersonID > 0)
            {
                ctrlDriverLicenses1.LoadInfoByPersonID(_PersonID);
            }
            else
            {
                MessageBox.Show("Could not Find selected person!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FilterEnabled = true;
                ctrlPersonCardWithFilter1.FilterFocus();
                return;
            }
        }

        private void frmShowPersonLicenseHistory_Activated(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.FilterFocus();
        }
    }
}
