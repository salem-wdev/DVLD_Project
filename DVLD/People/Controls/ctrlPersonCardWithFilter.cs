using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.People.Forms;
using DVLD_Business;

namespace DVLD.People.Controls
{
    public partial class ctrlPersonCardWithFilter : UserControl
    {

        // Define a custom event handler delegate with parameters
        //public event Action<int> OnPersonSelected;
        //// Create a protected method to raise the event with a parameter
        //protected virtual void PersonSelected(int PersonID)
        //{
        //    Action<int> handler = OnPersonSelected;
        //    if (handler != null)
        //    {
        //        handler(PersonID); // Raise the event with the parameter
        //    }
        //}

        public class PersonSelectedEventArgs:EventArgs
        {
            public int PersonID { get; } = -1;
            public string NationalNo { get; } = string.Empty;
            public PersonSelectedEventArgs(int PersonID, string NationalNo)
            {
                this.PersonID = PersonID;
                this.NationalNo = NationalNo;
            }
        }

        public event EventHandler<PersonSelectedEventArgs> PersonSelected;

        protected void OnPersonSelected(PersonSelectedEventArgs e)
        {
            PersonSelected?.Invoke(this, e);
        }

        private bool _ShowAddPerson = true;
        public bool ShowAddPerson
        {
            get
            {
                return _ShowAddPerson;
            }
            set
            {
                _ShowAddPerson = value;
                btnAddNewPerson.Visible = _ShowAddPerson;
            }
        }

        private bool _FilterEnabled = true;
        public bool FilterEnabled
        {
            get
            {
                return _FilterEnabled;
            }
            set
            {
                _FilterEnabled = value;
                gbFilters.Enabled = _FilterEnabled;
            }
        }

        private int _PersonID = -1;

        public int PersonID
        {
            get { return ctrlPersonCard1.PersonID; }
        }

        public IButtonControl AcceptButton
        {
            get { return btnFind; }
        }

        public clsPerson SelectedPerson
        {
            get { return ctrlPersonCard1.SelectedPerson; }
        }

        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();
        }

        private bool _IsValidInput()
        {
            if (string.IsNullOrWhiteSpace(txtFilterValue.Text))
            {
                MessageBox.Show("Please enter a value to filter by.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cbFilterBy.SelectedItem.ToString() == "ID" && !int.TryParse(txtFilterValue.Text, out _))
            {
                MessageBox.Show("Please enter a valid integer for ID.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cbFilterBy.SelectedItem.ToString() == "NationalNo" && txtFilterValue.Text.Length > 10)
            {
                MessageBox.Show("Please enter a valid 10-digit National Number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void _FindNow()
        {
            switch (cbFilterBy.SelectedItem.ToString())
            {
                case "Person ID":
                    ctrlPersonCard1.LoadData(int.Parse(txtFilterValue.Text.ToString()));
                    break;

                case "National No.":
                    ctrlPersonCard1.LoadData(txtFilterValue.Text.ToString());
                    break;

                default:
                    MessageBox.Show($"No person found with the given {cbFilterBy.SelectedItem.ToString()}.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }

            if (FilterEnabled)
                // Raise the event with a parameter
                OnPersonSelected
                    (new PersonSelectedEventArgs(ctrlPersonCard1?.PersonID ?? -1
                    , ctrlPersonCard1?.SelectedPerson?.NationalNo ?? string.Empty));

        }

        public void FilterFocus()
        {
            txtFilterValue.Focus();
        }

        public void LoadPersonInfo(int PersonID)
        {
            DataBackEvent(this, PersonID);
        }

        private void DataBackEvent(object sender, int personID)
        {
            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Text = personID.ToString();
            _FindNow();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("enter a valid value to filter by.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
                _FindNow();
        }

        private void ctrlPersonCardWithFilter_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 1;
            txtFilterValue.Focus();
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.SendPersonIDBack += DataBackEvent; // subscribe to the event
            frm.ShowDialog();
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {

            // ⚠ IMPORTANT: Validation works normally between controls.
            // To allow the form to close (e.g., via the X button) without being blocked by this validation,
            // the containing Form must handle the FormClosing event and temporarily disable CausesValidation 
            // or otherwise allow focus change. This UserControl alone cannot detect form closure.

            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFilterValue, "This field is required!");
            }
            else
            {
                //e.Cancel = false;
                errorProvider1.SetError(txtFilterValue, null);
            }

        }

        private void ctrlPersonCard1_Load(object sender, EventArgs e)
        {

        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is Enter (character code 13)
            if (e.KeyChar == (char)13)
            {

                btnFind.PerformClick();
            }

            //this will allow only digits if person id is selected
            if (cbFilterBy.Text == "Person ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }
    }
}
