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

            ctrlShowPersonInfo1.LoadDataAsync(PersonID);
        }

        public frmShowPersonInfo(string NationalNo)
        {
            InitializeComponent();

            _LoadPersonDataAsync(NationalNo);
        }

        public frmShowPersonInfo(clsPerson Person)
        {
            InitializeComponent();

            ctrlShowPersonInfo1.LoadData(Person);

        }

        private async void _LoadPersonDataAsync(string NationalNo)
        {
            await ctrlShowPersonInfo1.LoadDataAsync(NationalNo);
        }

        private async void _LoadPersonDataAsync(int? PersonID)
        {
            await ctrlShowPersonInfo1.LoadDataAsync(PersonID);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
