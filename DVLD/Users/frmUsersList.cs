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

namespace DVLD.Users
{
    public partial class frmUsersList : Form
    {
        private static DataTable _dtAllUsers;
        private void _RefreshRecordsNumber()
        {
            lblRecord.Text = dgvUsers.Rows.Count.ToString();
        }

        private void _RefreshUsersList()
        {
            _dtAllUsers = clsUser.GetAllUsers();
            dgvUsers.DataSource = _dtAllUsers;
            _RefreshRecordsNumber();
        }

        public frmUsersList()
        {
            InitializeComponent();
        }

        private void frmUsersList_Load(object sender, EventArgs e)
        {
            _RefreshUsersList();

            cbFilter.SelectedIndex = 0;
            cbIsActive.SelectedIndex = 0;

            if (dgvUsers.Columns.Count > 0)
            {
                dgvUsers.Columns[0].HeaderText = "User ID";
                dgvUsers.Columns[0].Width = 110;

                dgvUsers.Columns[1].HeaderText = "Person ID";
                dgvUsers.Columns[1].Width = 120;

                dgvUsers.Columns[2].HeaderText = "Full Name";
                dgvUsers.Columns[2].Width = 350;

                dgvUsers.Columns[3].HeaderText = "UserName";
                dgvUsers.Columns[3].Width = 120;

                dgvUsers.Columns[4].HeaderText = "Is Active";
                dgvUsers.Columns[4].Width = 120;
            }
            
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbIsActive.Visible = false;
            cbIsActive.SelectedIndex = 0;
            txtFilter.Text = string.Empty;
            txtFilter.Visible = false;


            if (cbFilter.SelectedItem.ToString() == "None")
            {
               
                return;
            }

            if (cbFilter.SelectedItem.ToString() == "Is Active")
            {
                cbIsActive.Visible = true;
                cbFilter.Focus();
                return;
            }

            txtFilter.Visible = true;
            txtFilter.Text = string.Empty;
            txtFilter.Focus();


        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbIsActive.SelectedItem)
            {
                case "All":
                    _dtAllUsers.DefaultView.RowFilter = string.Empty;
                    break;
                case "Yes":
                    _dtAllUsers.DefaultView.RowFilter = "IsActive = true";
                    break;
                case "No":
                    _dtAllUsers.DefaultView.RowFilter = "IsActive = false";
                    break;
            }

            _RefreshRecordsNumber();
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            //this will allow only digits if person id is selected
            if (cbFilter.Text == "Person ID" || cbFilter.Text == "User ID")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
                return;
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            string SelectedString;

            switch (cbFilter.SelectedItem.ToString())
            {
                case "User ID":
                    SelectedString = "UserID";
                    break;
                case "Person ID":
                    SelectedString = "PersonID";
                    break;
                case "Full Name":
                    SelectedString = "FullName";
                    break;
                case "UserName":
                    SelectedString = "UserName";
                    break;
                default:
                    SelectedString = "None";
                    break;

            }

            if (SelectedString == "None")
            {
                _dtAllUsers.DefaultView.RowFilter = string.Empty;
                _RefreshRecordsNumber();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFilter.Text.ToString().Trim()))
            {
                _dtAllUsers.DefaultView.RowFilter = string.Empty;
                _RefreshRecordsNumber();
                return;
            }

            if (cbFilter.SelectedItem.ToString() == "User ID" || cbFilter.SelectedItem.ToString() == "Person ID")
            {
                _dtAllUsers.DefaultView.RowFilter = $"{SelectedString} = {txtFilter.Text.ToString()}";
                _RefreshRecordsNumber();
                return;
            }

            if (cbFilter.SelectedItem.ToString() == "UserName")
            {
                _dtAllUsers.DefaultView.RowFilter = $"{SelectedString} LIKE '{txtFilter.Text.ToString().Trim()}%'";
                _RefreshRecordsNumber();
                return;
            }

            _dtAllUsers.DefaultView.RowFilter = $"{SelectedString} LIKE '%{txtFilter.Text.ToString().Trim()}%'";

            _RefreshRecordsNumber();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddNewUser_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser();
            frm.ShowDialog();
            _RefreshUsersList();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUserInfo frm = new frmUserInfo((int)dgvUsers.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
        }

        private void AddNewUsertoolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser();
            frm.ShowDialog();
            _RefreshUsersList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((int)dgvUsers.CurrentRow.Cells[0].Value == clsGlobal.CurrentUser.UserID)
            {
                MessageBox.Show("You can't edit your own account from here, please go to account settings or change password to edit your account", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            frmAddUpdateUser frm = new frmAddUpdateUser((int)dgvUsers.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            _RefreshUsersList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if(clsUser.Delete((int)dgvUsers.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show("User deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete user!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            _RefreshUsersList();
        }

        private void ChangePasswordtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword((int)dgvUsers.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not emplemnted yet", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not emplemnted yet", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgvUsers_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            clsUtil.ConfigureDataGridViewContextMenu(e, dgvUsers);

        }
    }
}
