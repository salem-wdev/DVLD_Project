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

namespace DVLD.Licenses.Local_Licenses.Controls
{
    public partial class ctrlDriverLicenseInfoWithFilter : UserControl
    {
        // Define a custom event handler delegate with parameters
        public event Action<int> OnLicenseSelected;
        // Create a protected method to raise the event with a parameter
        protected virtual void LicenseSelected(int DriverID)
        {
            Action<int> handler = OnLicenseSelected;
            if (handler != null)
            {
                handler(DriverID); // Raise the event with the parameter
            }
        }

        private bool _FilterEnabled = true; // Variable to store the filter state

        public bool FilterEnabled
        {
            get { return _FilterEnabled; }
            set
            {
                _FilterEnabled = value;
                gbFilters.Enabled = _FilterEnabled;
            }
        }

        private int _LicenseID = -1; // Variable to store the selected DriverID

        public int SelectedLicenseID
        {
            get { return _LicenseID; }
        }

        public clsLicense SelectedLicenseInfo
        {
            get
            {
                return ctrlDriverLicenseInfo1.SelectedLicenseInfo;
            }
        }

        public IButtonControl AcceptButton
        {
            get { return btnFind; }
        }

        public ctrlDriverLicenseInfoWithFilter()
        {
            InitializeComponent();
        }

        private void _Find()
        {
            _LicenseID = int.Parse(txtLicenseID.Text);
            if (clsLicense.Find(_LicenseID) != null)
            {
                ctrlDriverLicenseInfo1.LoadData(_LicenseID);
            }
            else
            {
                _LicenseID = -1;
                MessageBox.Show("License not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlDriverLicenseInfo1.ClearData();
            }
            LicenseSelected(_LicenseID); // Raise the event
        }

        public void LoadLicense(int LicenseID)
        {
            txtLicenseID.Text = LicenseID.ToString();
            FilterEnabled = false; // Disable the filter controls when loading a specific license.
            _Find();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtLicenseID.Text))
            {
                MessageBox.Show("Please enter a License ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _Find();
        }

        public void txtLicenseIDFocus()
        {
            txtLicenseID.Focus();
        }

        private void txtLicenseID_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !clsValidation.IsValidCharForID(e.KeyChar);
        }
    }
}
