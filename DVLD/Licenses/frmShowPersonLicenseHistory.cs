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
                ctrlPersonCardWithFilter1.FilterFocus();
            }

        }
    }
}
