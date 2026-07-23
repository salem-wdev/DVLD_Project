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
    public partial class frmShowPersonInfo : Form
    {
        public frmShowPersonInfo()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            // Never Use New With Controls!!!!!

        }

        public frmShowPersonInfo(int? PersonID)
        {
            InitializeComponent();

            ctrlShowPersonInfo1.LoadData(PersonID);
        }

        public frmShowPersonInfo(string NationalNo)
        {
            InitializeComponent();

            ctrlShowPersonInfo1.LoadData(NationalNo);
        }

        public frmShowPersonInfo(clsPerson Person)
        {
            InitializeComponent();

            ctrlShowPersonInfo1.LoadData(Person);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
