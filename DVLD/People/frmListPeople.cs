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

namespace DVLD.People.Forms
{
    public partial class frmListPeople : Form
    {
        public frmListPeople()
        {
            InitializeComponent();

        }

        /////////////////////////////////////////////////////////////////////
        // Data

        private static DataTable _dtAllPeople = clsPerson.GetAllPeople();

        //only select the columns that you want to show in the grid
        private DataTable _dtPeople = _dtAllPeople.DefaultView.ToTable(false, "PersonID", "NationalNo",
                                                         "FirstName", "SecondName", "ThirdName", "LastName",
                                                         "GendorCaption", "DateOfBirth", "CountryName",
                                                         "Phone", "Email");

        private static DataTable _dtCountries = clsCountry.GetAllCountries();

        // Data
        /////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////
        // Logic

        private void _RefreshdgvPeople()
        {
            dgvPeople.DataSource = _dtPeople;
            lblRecords.Text = dgvPeople.Rows.Count.ToString();
        }

        private void _RefreshPeopleList()
        {
            _dtAllPeople = clsPerson.GetAllPeople();
            _dtPeople = _dtAllPeople.DefaultView.ToTable(false, "PersonID", "NationalNo",
                                                       "FirstName", "SecondName", "ThirdName", "LastName",
                                                       "GendorCaption", "DateOfBirth", "CountryName",
                                                       "Phone", "Email");
            _RefreshDefaultVeiw();
            _RefreshdgvPeople();
        }

        private void _RefreshDefaultVeiw()
        {
            _dtPeople.DefaultView.Sort = "PersonID";
        }

        private void _FillcbFilterBy()
        {
            cbFilterBy.Items.Add("None");
            cbFilterBy.Items.Add("Person ID");
            cbFilterBy.Items.Add("National No.");
            cbFilterBy.Items.Add("First Name");
            cbFilterBy.Items.Add("Second Name");
            cbFilterBy.Items.Add("Third Name");
            cbFilterBy.Items.Add("Last Name");
            cbFilterBy.Items.Add("Gendor");
            cbFilterBy.Items.Add("Date Of Birth");
            cbFilterBy.Items.Add("Nationality");
            cbFilterBy.Items.Add("Phone");
            cbFilterBy.Items.Add("Email");


            cbFilterBy.DisplayMember = "ColumnName";
            cbFilterBy.SelectedIndex = 0;

        }

        private void _Filtering(string SelectedColumn)
        {
            string Column = SelectedColumn;

            if(!string.IsNullOrWhiteSpace(txtFilterBy.Text))
            {
                if (cbFilterBy.SelectedItem.ToString() == "Person ID")
                {

                    _dtPeople.DefaultView.RowFilter = $"PersonID = {Convert.ToInt32(txtFilterBy.Text)}";
                    lblRecords.Text = dgvPeople.Rows.Count.ToString();
                    return;
                }

                if (cbFilterBy.SelectedItem.ToString() == "National No.")
                {
                    _dtPeople.DefaultView.RowFilter = $"NationalNo LIKE '{txtFilterBy.Text}%'";
                    lblRecords.Text = dgvPeople.Rows.Count.ToString();
                    return;
                }
                string input = txtFilterBy.Text.Trim();

                _dtPeople.DefaultView.RowFilter = string.Format($"Convert({SelectedColumn}, 'System.String') LIKE '%{input}%'");
                lblRecords.Text = dgvPeople.Rows.Count.ToString();
            }
            else
            {
                _dtPeople.DefaultView.RowFilter = string.Empty; // Clear the filter when the search box is empty
                _RefreshDefaultVeiw();
                _RefreshdgvPeople();
            }
        }

        // Logic
        /////////////////////////////////////////////////////////////////////


        private void frmShowAllPeople_Load(object sender, EventArgs e)
        {
            _RefreshPeopleList();
            _FillcbFilterBy();

            if (dgvPeople.Rows.Count > 0)
            {

                dgvPeople.Columns[0].HeaderText = "Person ID";
                dgvPeople.Columns[0].Width = 110;

                dgvPeople.Columns[1].HeaderText = "National No.";
                dgvPeople.Columns[1].Width = 120;


                dgvPeople.Columns[2].HeaderText = "First Name";
                dgvPeople.Columns[2].Width = 120;

                dgvPeople.Columns[3].HeaderText = "Second Name";
                dgvPeople.Columns[3].Width = 140;


                dgvPeople.Columns[4].HeaderText = "Third Name";
                dgvPeople.Columns[4].Width = 120;

                dgvPeople.Columns[5].HeaderText = "Last Name";
                dgvPeople.Columns[5].Width = 120;

                dgvPeople.Columns[6].HeaderText = "Gendor";
                dgvPeople.Columns[6].Width = 120;

                dgvPeople.Columns[7].HeaderText = "Date Of Birth";
                dgvPeople.Columns[7].Width = 140;

                dgvPeople.Columns[8].HeaderText = "Nationality";
                dgvPeople.Columns[8].Width = 120;


                dgvPeople.Columns[9].HeaderText = "Phone";
                dgvPeople.Columns[9].Width = 120;


                dgvPeople.Columns[10].HeaderText = "Email";
                dgvPeople.Columns[10].Width = 170;
            }


        }

        /////////////////////////////////////////////////////////////////////
        // ToolStripMenuItem
        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = int.Parse(dgvPeople.CurrentRow.Cells[0].Value.ToString());

            frmShowPersonInfo frm = new frmShowPersonInfo(PersonID);
            frm.ShowDialog();
            frm.Dispose();
        }

        private void addNewPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
            frm.Close();
            _RefreshPeopleList();
        }

        private void editeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson(int.Parse(dgvPeople.CurrentRow.Cells[0].Value.ToString()));
            frm.ShowDialog();
            frm.Close();
            _RefreshPeopleList();
            
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = int.Parse(dgvPeople.CurrentRow.Cells[0].Value.ToString());
            if (MessageBox.Show("Are you sure you want to delete this person?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (clsPerson.Delete(PersonID))
                {
                    MessageBox.Show($"Person with ID {PersonID} deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshPeopleList();
                }
                else
                {
                    MessageBox.Show($"Failed to delete person with ID {PersonID}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                lblRecords.Text = dgvPeople.RowCount.ToString();
            }
            else
            {
                MessageBox.Show("Deletion cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not implemented yet.", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not implemented yet.", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ToolStripMenuItem
        /////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////
        // Buttons

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
            frm.Close();
            _RefreshPeopleList();


        }

        // Buttons
        /////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////
        // Miscellaneous

        private void txtSearchBy_TextChanged(object sender, EventArgs e)
        {
            string SelectedColumn = cbFilterBy.Text.Trim();

            switch(SelectedColumn)
            {
                case "Person ID":
                    SelectedColumn = "PersonID";
                    break;
                case "National No.":
                    SelectedColumn = "NationalNo";
                    break;
                case "First Name":
                    SelectedColumn = "FirstName";
                    break;
                case "Second Name":
                    SelectedColumn = "SecondName";
                    break;
                case "Third Name":
                    SelectedColumn = "ThirdName";
                    break;
                case "Last Name":
                    SelectedColumn = "LastName";
                    break;
                case "Gendor":
                    SelectedColumn = "GendorCaption";
                    break;
                case "Date Of Birth":
                    SelectedColumn = "DateOfBirth";
                    break;
                case "Nationality":
                    SelectedColumn = "CountruName";
                    break;
                case "Phone":
                    SelectedColumn = "Phone";
                    break;
                case "Email":
                    SelectedColumn = "Email";
                    break;
            }


            _Filtering(SelectedColumn);
            
        }

        private void dgvPeople_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            clsUtil.ConfigureDataGridViewContextMenu(e, dgvPeople);
        }


        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterBy.Visible = cbFilterBy.SelectedItem.ToString() != "None";


            if (!txtFilterBy.Visible)
            {
                txtFilterBy.Text = string.Empty;

                _RefreshDefaultVeiw();

                return;
            }

            txtFilterBy.Text = string.Empty;
            txtFilterBy.Visible = true;

        }

        private void txtFilterBy_KeyPress(object sender, KeyPressEventArgs e)
        {
            //this will allow only digits if person id is selected
            if (cbFilterBy.Text == "Person ID")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
                return;
            }

            if (cbFilterBy.Text == "Date Of Birth")
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '/';
                return;
            }

            //e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);


        }

        // Miscellaneous
        /////////////////////////////////////////////////////////////////////


    }
}
