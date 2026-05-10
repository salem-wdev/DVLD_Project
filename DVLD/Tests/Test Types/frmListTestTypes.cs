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

namespace DVLD.Tests.Test_Types
{
    public partial class frmListTestTypes : Form
    {
        private static DataTable _dtAllTestTypes;

        public frmListTestTypes()
        {
            InitializeComponent();
        }

        private void _Refresh()
        {
            _dtAllTestTypes = clsTestType.GetAllTestTypes();
            dgvTestTypes.DataSource = _dtAllTestTypes;
            lblRecordsCount.Text = _dtAllTestTypes.Rows.Count.ToString();
        }

        private void frmTestTypes_Load(object sender, EventArgs e)
        {
            _Refresh();

            if (dgvTestTypes.Columns.Count > 0)
            {
                dgvTestTypes.Columns[0].HeaderText = "ID";
                dgvTestTypes.Columns[0].Width = 120;

                dgvTestTypes.Columns[1].HeaderText = "Title";
                dgvTestTypes.Columns[1].Width = 200;

                dgvTestTypes.Columns[2].HeaderText = "Description";
                dgvTestTypes.Columns[2].Width = 400;

                dgvTestTypes.Columns[3].HeaderText = "Fees";
                dgvTestTypes.Columns[3].Width = 100;
            }
            else
            {
                MessageBox.Show("No columns found in the DataGridView.");
            }

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvTestTypes_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            clsUtil.ConfigureDataGridViewContextMenu(e, dgvTestTypes);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditTestType frm = new frmEditTestType((clsTestType.enTestType)dgvTestTypes.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            _Refresh();
        }
    }
}
