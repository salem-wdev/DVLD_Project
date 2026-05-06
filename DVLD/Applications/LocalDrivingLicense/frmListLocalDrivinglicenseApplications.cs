using DVLD.Applications.Local_Driving_License;
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

namespace DVLD.Applications.LocalDrivingLicense
{
    public partial class frmListLocalDrivinglicenseApplications : Form
    {
        DataTable _dtAllLocalApplications;

        public frmListLocalDrivinglicenseApplications()
        {
            InitializeComponent();
        }

        private void _RefreshData()
        {
            _dtAllLocalApplications = clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplications();
            dgvLocalDrivingLicenseApplications.DataSource = _dtAllLocalApplications;
            lblRecordsCount.Text = _dtAllLocalApplications.Rows.Count.ToString();
        }

        private void frmListLocalDrivingLicesnseApplications_Load(object sender, EventArgs e)
        {
            _RefreshData();

            if (dgvLocalDrivingLicenseApplications.Rows.Count > 0)
            {

                dgvLocalDrivingLicenseApplications.Columns[0].HeaderText = "L.D.L.AppID";
                dgvLocalDrivingLicenseApplications.Columns[0].Width = 120;

                dgvLocalDrivingLicenseApplications.Columns[1].HeaderText = "Driving Class";
                dgvLocalDrivingLicenseApplications.Columns[1].Width = 300;

                dgvLocalDrivingLicenseApplications.Columns[2].HeaderText = "National No.";
                dgvLocalDrivingLicenseApplications.Columns[2].Width = 150;

                dgvLocalDrivingLicenseApplications.Columns[3].HeaderText = "Full Name";
                dgvLocalDrivingLicenseApplications.Columns[3].Width = 350;

                dgvLocalDrivingLicenseApplications.Columns[4].HeaderText = "Application Date";
                dgvLocalDrivingLicenseApplications.Columns[4].Width = 170;

                dgvLocalDrivingLicenseApplications.Columns[5].HeaderText = "Passed Tests";
                dgvLocalDrivingLicenseApplications.Columns[5].Width = 150;
            }

            cbFilterBy.SelectedIndex = 0;


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddNewApplication_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicenseApplication frm = new frmAddUpdateLocalDrivingLicenseApplication();
            frm.ShowDialog();
            _RefreshData();
        }

        private void DeleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure do want to delete this application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            if (LocalDrivingLicenseApplication != null)
            {
                if (LocalDrivingLicenseApplication.Delete())
                {
                    MessageBox.Show("Application Deleted Successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //refresh the form again.
                    frmListLocalDrivingLicesnseApplications_Load(null, null);
                }
                else
                {
                    MessageBox.Show("Could not delete applicatoin, other data depends on it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _RefreshData();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicenseApplication frm = new frmAddUpdateLocalDrivingLicenseApplication((int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);
            frm.Show();
            _RefreshData();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string filterBy = cbFilterBy.SelectedItem.ToString();

            switch (filterBy)
            {
                case "L.D.L.AppID":
                    filterBy = "LocalDrivingLicenseApplicationID";
                    break;
                case "National No.":
                    filterBy = "NationalNo";
                    break;
                case "Full Name":
                    filterBy = "FullName";
                    break;
                case "Status":
                    filterBy = "Status";
                    break;
                default:
                    filterBy = "None";
                    break;
            }



            if (!string.IsNullOrWhiteSpace(txtFilterValue.Text))
            {

                if (cbFilterBy.SelectedItem.ToString() == "L.D.L.AppID")
                {
                    if(!clsValidation.ValidateInteger(txtFilterValue.Text))
                    {
                        errorProvider1.SetError(txtFilterValue, "Enter valid integer");
                        return;
                    }
                    else
                    {
                        errorProvider1.SetError(txtFilterValue, string.Empty);
                    }

                    _dtAllLocalApplications.DefaultView.RowFilter = $"{filterBy} = {txtFilterValue.Text}";
                    lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
                    return;
                }

                _dtAllLocalApplications.DefaultView.RowFilter = $"{filterBy} LIKE '%{txtFilterValue.Text}%'";

            }
            else
            {
                _dtAllLocalApplications.DefaultView.RowFilter = string.Empty;
            }

            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();

        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.SelectedItem.ToString() == "L.D.L.AppID")
            {
                e.Handled = !clsValidation.IsValidCharForID(e.KeyChar);
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilterBy.SelectedItem.ToString() == "None")
            {
                _dtAllLocalApplications.DefaultView.RowFilter = string.Empty;
                txtFilterValue.Visible = false;
            }
            else
            {
                txtFilterValue.Visible = true;
            }

            txtFilterValue.Text = string.Empty;
            
        }
    }
}
