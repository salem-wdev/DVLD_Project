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

        // TODO: 
        // 1. Add DriverID and ApplicationID properties to this EventArgs class.
        // 2. Update invocation points to pass the actual IDs when raising the event.
        // 3. Update subscriber methods (Event Handlers) to consume and sync with these new IDs.

        public sealed class LicenseSelectedEventArgs : EventArgs
        {
            public int LicenseID { get; }

            public LicenseSelectedEventArgs(int licenseID)
            {
                LicenseID = licenseID;
            }
        }

        public event EventHandler<LicenseSelectedEventArgs> LicenseSelected;

        protected virtual void OnLicenseSelected(LicenseSelectedEventArgs e)
        {
            LicenseSelected?.Invoke(this, e);
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

        private async Task _FindAsync()
        {
            _LicenseID = int.Parse(txtLicenseID.Text);
            if (clsLicense.FindAsync(_LicenseID) != null)
            {
               if(await ctrlDriverLicenseInfo1.LoadDataAsync(_LicenseID))
                    ctrlDriverLicenseInfo1.SelectedLicenseInfo.LicenseUpdated += SelectedLicenseInfo_LicenseUpdated;
            }
            else
            {
                _LicenseID = -1;
                MessageBox.Show("License not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlDriverLicenseInfo1.ClearData();
            }
            OnLicenseSelected(new LicenseSelectedEventArgs(_LicenseID)); // Raise the event
        }

        private async void SelectedLicenseInfo_LicenseUpdated(object sender, clsLicense.LicenseUpdatedEventArgs e)
        {
            ctrlDriverLicenseInfo1?.SelectedLicenseInfo?.Dispose();
            await _FindAsync();

        }

        public async void LoadLicense(int LicenseID)
        {
            txtLicenseID.Text = LicenseID.ToString();
            FilterEnabled = false; // Disable the filter controls when loading a specific license.
            await _FindAsync();
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtLicenseID.Text))
            {
                MessageBox.Show("Please enter a License ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await _FindAsync();
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
