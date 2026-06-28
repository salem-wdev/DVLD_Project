using DVLD.Global_Classes;
using DVLD.Licenses.Local_Licenses;
using DVLD.People.Forms;
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

namespace DVLD.Drivers
{
    public partial class frmListDrivers : Form
    {
        private DataTable _dtAllDrivers;

        public frmListDrivers()
        {
            InitializeComponent();
            txtFilterValue.Visible = false;
        }

        private void _RefreshRecordsNumber()
        {
            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();
        }

        private void _RefreshTable()
        {
            _dtAllDrivers = clsDriver.GetAllDrivers();
            dgvDrivers.DataSource = _dtAllDrivers;
            _RefreshRecordsNumber();
        }

        private void frmListDrivers_Load(object sender, EventArgs e)
        {
            _RefreshTable();
            cbFilterBy.SelectedIndex = 0;

            if (dgvDrivers.Rows.Count > 0)
            {
                dgvDrivers.Columns[0].HeaderText = "Driver ID";
                dgvDrivers.Columns[0].Width = 120;

                dgvDrivers.Columns[1].HeaderText = "Person ID";
                dgvDrivers.Columns[1].Width = 120;

                dgvDrivers.Columns[2].HeaderText = "National No.";
                dgvDrivers.Columns[2].Width = 140;

                dgvDrivers.Columns[3].HeaderText = "Full Name";
                dgvDrivers.Columns[3].Width = 320;

                dgvDrivers.Columns[4].HeaderText = "Date";
                dgvDrivers.Columns[4].Width = 170;

                dgvDrivers.Columns[5].HeaderText = "Active Licenses";
                dgvDrivers.Columns[5].Width = 150;
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbFilterBy.SelectedItem.ToString() == "None")
            {
                _dtAllDrivers.DefaultView.RowFilter = null;
                txtFilterValue.Visible = false;
            }
            else
            {
                txtFilterValue.Visible = true;
            }

            txtFilterValue.Text = string.Empty;
        }

        private string _GetSelectedString()
        {
            string SelectedString = cbFilterBy.SelectedItem.ToString();

            switch (SelectedString)
            {
                case "None":
                    return "None";
                case "Driver ID":
                    return "DriverID";
                case "Person ID":
                    return "PersonID";
                case "National No.":
                    return "NationalNo";
                case "Full Name":
                    return "FullName";
                default:
                    return "None";
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string SelectedString = _GetSelectedString();

            if (SelectedString == "None")
            {
                _dtAllDrivers.DefaultView.RowFilter = string.Empty;
                txtFilterValue.Visible = false;
                _RefreshRecordsNumber();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFilterValue.Text.ToString().Trim()))
            {
                _dtAllDrivers.DefaultView.RowFilter = string.Empty;
                _RefreshRecordsNumber();
                return;
            }

            if (SelectedString == "DriverID" || SelectedString == "PersonID")
            {
                _dtAllDrivers.DefaultView.RowFilter = $"{SelectedString} = {txtFilterValue.Text.ToString()}";
                _RefreshRecordsNumber();
                return;
            }

            if (SelectedString == "NationalNo")
            {
                _dtAllDrivers.DefaultView.RowFilter = $"{SelectedString} LIKE '{txtFilterValue.Text.ToString()}%'";
                _RefreshRecordsNumber();
                return;
            }

            _dtAllDrivers.DefaultView.RowFilter = $"{SelectedString} LIKE '%{txtFilterValue.Text.ToString()}%'";
            _RefreshRecordsNumber();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonInfo frm = new frmShowPersonInfo((int)dgvDrivers.CurrentRow.Cells[1].Value);
            frm.ShowDialog();
        }

        private void issueInternationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature not emplemented yet","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory((int)dgvDrivers.CurrentRow.Cells[1].Value);
            frm.ShowDialog();
        }

        private void dgvDrivers_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            clsUtil.ConfigureDataGridViewContextMenu(e, dgvDrivers);
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            string SelectedString = _GetSelectedString();

            if (SelectedString == "DriverID" || SelectedString == "PersonID")
            {
                e.Handled = !clsValidation.IsValidCharForID(e.KeyChar);
            }
        }
    }
}
