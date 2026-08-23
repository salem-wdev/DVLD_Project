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

namespace DVLD.Applications.Application_Types
{
    public partial class frmListApplicationTypes : Form
    {
        private DataTable _dtAllApplicationTypes;

        public frmListApplicationTypes()
        {
            InitializeComponent();
        }

        private void _RefreshNumberOfRecords()
        {
            lblRecordsCount.Text = _dtAllApplicationTypes.Rows.Count.ToString();
        }

        private async Task _RefreshDataAsync()
        {
            _dtAllApplicationTypes = await clsApplicationType.GetAllApplicationTypesAsync();
            dgvApplicationTypes.DataSource = _dtAllApplicationTypes;
            _RefreshNumberOfRecords();
        }

        private async void frmListApplicationTypes_Load(object sender, EventArgs e)
        {
            await _RefreshDataAsync();

            if(dgvApplicationTypes.Rows.Count > 0)
            {
                dgvApplicationTypes.Columns[0].HeaderText = "ID";
                dgvApplicationTypes.Columns[0].Width = 110;

                dgvApplicationTypes.Columns[1].HeaderText = "Title";
                dgvApplicationTypes.Columns[1].Width = 400;

                dgvApplicationTypes.Columns[2].HeaderText = "Fees";
                dgvApplicationTypes.Columns[2].Width = 100;
            }
        }

        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditApplicationType frm = new frmEditApplicationType((int)dgvApplicationTypes.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            await _RefreshDataAsync();
        }

        private void dgvApplicationTypes_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            clsUIHelper.ConfigureDataGridViewContextMenu(e, dgvApplicationTypes);
        }
    }
}
