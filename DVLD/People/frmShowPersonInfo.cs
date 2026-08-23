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

            _LoadPersonDataAsync(PersonID);
        }

        public frmShowPersonInfo(string NationalNo)
        {
            InitializeComponent();

            _LoadPersonDataAsync(NationalNo);
        }

        public frmShowPersonInfo(clsPerson Person)
        {
            InitializeComponent();

            _LoadPersonDataAsync(Person);

        }

        private async void _LoadPersonDataAsync(string NationalNo)
        {
            await ctrlShowPersonInfo1.LoadDataAsync(NationalNo);
        }

        private async void _LoadPersonDataAsync(int? PersonID)
        {
            await ctrlShowPersonInfo1.LoadDataAsync(PersonID);
        }

        private async void _LoadPersonDataAsync(clsPerson Person)
        {
            await ctrlShowPersonInfo1.LoadDataAsync(Person);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
