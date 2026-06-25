using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Global_Classes
{
    public class clsUtil
    {
        
        public static void ConfigureDataGridViewContextMenu(DataGridViewCellMouseEventArgs e, DataGridView dgv)
        {
            if (e.RowIndex >= 0)
            {
                dgv.ClearSelection();
                dgv.Rows[e.RowIndex].Selected = true;
                dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[0];
            }

        }
    }
}
