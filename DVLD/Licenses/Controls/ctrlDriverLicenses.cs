using DVLD.Global_Classes;
using DVLD.Licenses.International_Licenses;
using DVLD.Licenses.Local_Licenses;
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

namespace DVLD.Licenses.Controls
{
    public partial class ctrlDriverLicenses : UserControl
    {
        private int _DriverID = -1;
        private clsDriver _Driver;
        DataTable _dtAllLocalDrivingLicenses;
        DataTable _dtAllInternationalDrivingLicenses;

        public ctrlDriverLicenses()
        {
            InitializeComponent();
        }

        private void _LoadLocalLicenseInfo()
        {

            _dtAllLocalDrivingLicenses = clsDriver.GetLicenses(_DriverID);


            dgvLocalLicensesHistory.DataSource = _dtAllLocalDrivingLicenses;
            lblLocalLicensesRecords.Text = dgvLocalLicensesHistory.Rows.Count.ToString();

            if (dgvLocalLicensesHistory.Rows.Count > 0)
            {
                dgvLocalLicensesHistory.Columns[0].HeaderText = "Lic.ID";
                dgvLocalLicensesHistory.Columns[0].Width = 110;

                dgvLocalLicensesHistory.Columns[1].HeaderText = "App.ID";
                dgvLocalLicensesHistory.Columns[1].Width = 110;

                dgvLocalLicensesHistory.Columns[2].HeaderText = "Class Name";
                dgvLocalLicensesHistory.Columns[2].Width = 270;

                dgvLocalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvLocalLicensesHistory.Columns[3].Width = 170;

                dgvLocalLicensesHistory.Columns[4].HeaderText = "Expiration Date";
                dgvLocalLicensesHistory.Columns[4].Width = 170;

                dgvLocalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvLocalLicensesHistory.Columns[5].Width = 110;

            }
        }

        private void _LoadInternationalLicenseInfo()
        {

            _dtAllInternationalDrivingLicenses = clsDriver.GetInternationalLicenses(_DriverID);


            dgvInternationalLicensesHistory.DataSource = _dtAllInternationalDrivingLicenses;
            lblInternationalLicensesRecords.Text = dgvInternationalLicensesHistory.Rows.Count.ToString();

            if (dgvInternationalLicensesHistory.Rows.Count > 0)
            {
                dgvInternationalLicensesHistory.Columns[0].HeaderText = "Int.License ID";
                dgvInternationalLicensesHistory.Columns[0].Width = 160;

                dgvInternationalLicensesHistory.Columns[1].HeaderText = "Application ID";
                dgvInternationalLicensesHistory.Columns[1].Width = 130;

                dgvInternationalLicensesHistory.Columns[2].HeaderText = "L.License ID";
                dgvInternationalLicensesHistory.Columns[2].Width = 130;

                dgvInternationalLicensesHistory.Columns[3].HeaderText = "Issue Date";
                dgvInternationalLicensesHistory.Columns[3].Width = 180;

                dgvInternationalLicensesHistory.Columns[4].HeaderText = "Expiration Date";
                dgvInternationalLicensesHistory.Columns[4].Width = 180;

                dgvInternationalLicensesHistory.Columns[5].HeaderText = "Is Active";
                dgvInternationalLicensesHistory.Columns[5].Width = 120;

            }
        }

        private void _RefreshDataGrids()
        {
            _LoadLocalLicenseInfo();
            _LoadInternationalLicenseInfo();
        }

        private void _ResetData()
        {
            _dtAllLocalDrivingLicenses.Clear();
            _dtAllInternationalDrivingLicenses.Clear();
        }

        public void ResetData()
        {
            _ResetData();
        }

        public void LoadInfo(int DriverID)
        {
            _DriverID = DriverID;
            _Driver = clsDriver.FindByDriverID(_DriverID);

            if (_Driver == null)
            {
                MessageBox.Show("No driver information found for the provided DriverID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            _RefreshDataGrids();
        }

        public void LoadInfoByPersonID(int PersonID)
        {

            _Driver = clsDriver.FindByPersonID(PersonID);
            if (_Driver == null)
            {
                MessageBox.Show("No driver information found for the provided PersonID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _DriverID = _Driver.DriverID;

            _RefreshDataGrids();
        }

        private void showLicenseInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo((int)dgvLocalLicensesHistory.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
        }

        private void InternationalLicenseHistorytoolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowInternationalLicenseInfo frm = new frmShowInternationalLicenseInfo((int)dgvInternationalLicensesHistory.CurrentRow.Cells[0].Value); 
            frm.ShowDialog();
        }

        private void dgvLocalLicensesHistory_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            clsUtil.ConfigureDataGridViewContextMenu(e, dgvLocalLicensesHistory);
        }

        private void dgvInternationalLicensesHistory_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            clsUtil.ConfigureDataGridViewContextMenu(e, dgvInternationalLicensesHistory);
        }
    }
}
