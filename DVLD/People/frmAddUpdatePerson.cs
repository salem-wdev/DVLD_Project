using DVLD.Global_Classes;
using DVLD.Properties;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.People.Forms
{
    public partial class frmAddUpdatePerson : Form
    {

        enum enMode { AddNew, Update }
        enum enGenderType { Male , Female }

        private enGenderType _GenderType;
        private enMode _Mode;

        clsPerson _Person;
        int? _PersonID = -1;

        public delegate void PersonDataReceivedEventHandlerInForm(object sender, clsPerson Person);

        public delegate void PersonIDReceivedEventHandlerInForm(object sender, int? PersonID);

        public event PersonIDReceivedEventHandlerInForm SendPersonIDBack;

        public event PersonDataReceivedEventHandlerInForm SendDataBack;


        public frmAddUpdatePerson()
        {
            InitializeComponent();
            
            _Mode = enMode.AddNew;

        }

        public frmAddUpdatePerson(clsPerson Person)
        {
            InitializeComponent();
            _Mode = enMode.Update;
            this._Person = Person;
            this._PersonID = Person.PersonID;
        }

        public frmAddUpdatePerson(int PersonID)
        {
            InitializeComponent();
            _Mode = enMode.Update;
            _PersonID = PersonID;
            _Person = clsPerson.Find(_PersonID);
        }


        /// <logic>
        //////////////////////////////////////////////////////////////////



        /// <Private>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        private void _FillCountriesInComboBox()
        {
            DataTable Countries = clsCountry.GetAllCountries();
            cmbNationality.DataSource = Countries;
            cmbNationality.DisplayMember = "CountryName";
            cmbNationality.ValueMember = "CountryID";
            cmbNationality.SelectedValue = 191;   // Default to Yemen
        }

        private void _RestDefaultValues()
        {
            _FillCountriesInComboBox();

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Person";
                _Person = null;
            }
            else
            {
                lblTitle.Text = "Update Person";
            }

            //set default image for the person.
            pbPersonPhoto.Image = Resources.Male_512;

            //hide/show the remove linke incase there is no image for the person.
            llRemoveImage.Visible = (pbPersonPhoto.ImageLocation != null);

            //we set the max date to 18 years from today, and set the default value the same.
            dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-18);
            dtpDateOfBirth.Value = dtpDateOfBirth.MaxDate;

            //should not allow adding age more than 100 years
            dtpDateOfBirth.MinDate = DateTime.Now.AddYears(-100);

            //this will set default country to yemen.
            cmbNationality.SelectedIndex = 191;

            txtFirstName.Text = string.Empty;
            txtSecondName.Text = string.Empty;
            txtThirdName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtNationalNo.Text = string.Empty;
            rbMale.Checked = true;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtAddress.Text = string.Empty;

        }

        private bool _SavePerson()
        {

            if (_Mode == enMode.AddNew)
            {
                clsPerson.enGenderType Gender = rbMale.Checked ? clsPerson.enGenderType.Male
                    : clsPerson.enGenderType.Female;

                _Person = clsPerson.CreateNewPerson(txtNationalNo.Text, txtFirstName.Text, txtSecondName.Text,
                    txtLastName.Text, dtpDateOfBirth.Value, Gender, txtAddress.Text, txtPhone.Text
                    , (int)cmbNationality.SelectedValue, txtThirdName.Text, txtEmail.Text, pbPersonPhoto.ImageLocation);
                if (_Person != null)
                {

                    _Mode = enMode.Update;
                    lblTitle.Text = "Edit Person";
                    return true;
                }
            }

            if (_Mode == enMode.Update)
            {
                return _Person.Save();
            }

            return false;
        }

        private void _FillPersonWithData()
        {
            _Person.FirstName = txtFirstName.Text.Trim();
            _Person.SecondName = txtSecondName.Text.Trim();
            _Person.ThirdName = txtThirdName.Text.Trim();
            _Person.LastName = txtLastName.Text.Trim();
            _Person.NationalNo = txtNationalNo.Text.Trim();
            _Person.DateOfBirth = dtpDateOfBirth.Value;
            _Person.Gender = (clsPerson.enGenderType)_GenderType;
            _Person.Address = txtAddress.Text.Trim();
            _Person.Phone = txtPhone.Text.Trim();
            _Person.Email = txtEmail.Text.Trim();
            _Person.NationalityCountryID = clsCountry.Find(cmbNationality.Text).CountryID;
           
            if (!string.IsNullOrWhiteSpace(pbPersonPhoto.ImageLocation))
                _Person.ImagePath = pbPersonPhoto.ImageLocation;
            else
                _Person.ImagePath = "";

        }

        private void _FillFormWithPersonData()
        {
            // If Mode is AddNew, then _Person should be initialized with default values, so we can fill the form with those default values.
            // If Mode is EditExisting, then _Person should be loaded with existing data, so we can fill the form with that existing data.

            // Set the DateOfBirth DateTimePicker's MaxDate to 18 years ago from today, and MinDate to 100 years ago from today, to ensure that the person is between 18 and 100 years old.
            this.dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-18);
            this.dtpDateOfBirth.MinDate = DateTime.Now.AddYears(-100);

            // Fill the form controls with the data from _Person object based on the Mode (AddNew or EditExisting).
            if (_Person.Mode == clsPerson.enMode.Update)
            {
                this.lblPersonID.Text = _Person.PersonID.ToString();
                this.dtpDateOfBirth.Value = _Person.DateOfBirth;
                this.cmbNationality.SelectedValue = _Person.NationalityCountryID;

            }
            else
            {
                this.lblPersonID.Text = "N/A";
                this.cmbNationality.SelectedValue = 191;   // Default to Yemen
                this.dtpDateOfBirth.Value = this.dtpDateOfBirth.MaxDate;
            }

            // Fill the form controls with the data from _Person object
            this.txtFirstName.Text = _Person.FirstName;
            this.txtSecondName.Text = _Person.SecondName;
            this.txtThirdName.Text = _Person.ThirdName;
            this.txtLastName.Text = _Person.LastName;
            this.txtNationalNo.Text = _Person.NationalNo;
            this.txtEmail.Text = _Person.Email;
            this.txtPhone.Text = _Person.Phone;
            this.txtAddress.Text = _Person.Address;

            if(!string.IsNullOrWhiteSpace(_Person.ImagePath))
                this.pbPersonPhoto.ImageLocation = _Person.ImagePath;

            this.llRemoveImage.Visible = !(string.IsNullOrWhiteSpace(pbPersonPhoto.ImageLocation));

            // Set the Gender radio buttons and default photo based on the Gender value in _Person
            if (_Person.Gender == clsPerson.enGenderType.Male)
            {
                this.rbMale.Checked = true;
                if (string.IsNullOrWhiteSpace(pbPersonPhoto.ImageLocation))
                    pbPersonPhoto.Image = Properties.Resources.Male_512;
            }
            else
            {
                this.rbFemale.Checked = true;
                if (string.IsNullOrWhiteSpace(pbPersonPhoto.ImageLocation))
                    pbPersonPhoto.Image = Properties.Resources.Female_512;
            }
        }

        private bool _IsValidToSave()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                errorProvider1.SetError(txtFirstName, "First Name is required.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtSecondName.Text))
            {
                errorProvider1.SetError(txtSecondName, "Second Name is required.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                errorProvider1.SetError(txtLastName, "Last Name is required.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtNationalNo.Text))
            {
                errorProvider1.SetError(txtNationalNo, "National No is required.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errorProvider1.SetError(txtPhone, "Phone is required.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                errorProvider1.SetError(txtAddress, "Address is required.");
                isValid = false;
            }
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !txtEmail.Text.Contains("@"))
            {
                errorProvider1.SetError(txtEmail, "Invalid Email.");
                isValid = false;
            }
            return isValid;
        }

        private bool _LoadData()
        {
            return LoadData(_Person);
        }


        private void ReceivePersonDataFromControl(object sender, clsPerson Person)
        {
            _Person = Person;
            lblPersonID.Text = _Person.PersonID.ToString();
            SendDataBack?.Invoke(this, _Person);
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <Private>


        public bool LoadData(clsPerson Person)
        {
            _Person = Person;
            if (_Person != null)
            {
                _FillFormWithPersonData();
                return _IsValidToSave();
            }
            
            MessageBox.Show("No Person With ID: " + _PersonID, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            this.Close();
            return false;
        }




        //////////////////////////////////////////////////////////////
        /// </logic>
        private void frmAddNewPerson_Load(object sender, EventArgs e)
        {
            _RestDefaultValues();
            if (_Mode == enMode.Update)
            {
                _LoadData();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_Person != null && _Person.PersonID != -1)
            {
                SendDataBack?.Invoke(this, _Person);
                SendPersonIDBack?.Invoke(this, _Person.PersonID);
            }

            base.OnFormClosed(e);
        }

        /// </Validating>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                return;

            if(!clsValidation.ValidateEmail(txtEmail.Text))
            {
                errorProvider1.SetError(txtEmail, "Invalid email format.");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(txtEmail, string.Empty);
            }
        }

        private void txtNationalNo_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNationalNo.Text))
            {
                errorProvider1.SetError(txtNationalNo, "National No is required.");
                e.Cancel = true;
                return;
            }
            
            if (_Mode == enMode.Update && txtNationalNo.Text.Trim() != _Person.NationalNo && clsPerson.IsPersonExists(txtNationalNo.Text.Trim()))
            {
                errorProvider1.SetError(txtNationalNo, "Invalid National No.");
                e.Cancel = true;
                return;
            }

            if (_Mode == enMode.AddNew && clsPerson.IsPersonExists(txtNationalNo.Text.Trim()))
            {
                errorProvider1.SetError(txtNationalNo, "Invalid National No.");
                e.Cancel = true;
                return;
            }

            errorProvider1.SetError(txtNationalNo, string.Empty);
            
        }
      
        private void txt_Validating(object sender, CancelEventArgs e)
        {
            // First: set AutoValidate property of your Form to EnableAllowFocusChange in designer 
            TextBox Temp = ((TextBox)sender);
            if (string.IsNullOrEmpty(Temp.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(Temp, $"{Temp.Tag} is required!");
            }
            else
            {
                //e.Cancel = false;
                errorProvider1.SetError(Temp, null);
            }

        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// </Validating>

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please correct the errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_Mode == enMode.Update)
                _FillPersonWithData();


            if (_SavePerson())
            {
                MessageBox.Show("Saved Successfully", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                lblPersonID.Text = _Person.PersonID.ToString();
            }
            else
            {
                MessageBox.Show("Failed to save", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            this.Close();
        }

        private void llSetImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Process the selected file

                pbPersonPhoto.ImageLocation = openFileDialog1.FileName;
                llRemoveImage.Visible = true;
                // ...
            }
        }

        private void rbMaleFemale_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMale.Checked)
            {
                if(string.IsNullOrWhiteSpace(pbPersonPhoto.ImageLocation))
                {
                    pbPersonPhoto.Image = Properties.Resources.Male_512;
                }
                _GenderType = enGenderType.Male;
            }
            else if (rbFemale.Checked)
            {
                if (string.IsNullOrWhiteSpace(pbPersonPhoto.ImageLocation))
                {
                    pbPersonPhoto.Image = Properties.Resources.Female_512;
                }
                _GenderType = enGenderType.Female;
            }
        }

        private void llRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pbPersonPhoto.ImageLocation = null;
            

            pbPersonPhoto.Image = (rbMale.Checked)
                ? Properties.Resources.Male_512
                : Properties.Resources.Female_512;
            llSetImage.Focus();
            llRemoveImage.Visible = false;

        }

    }
}
