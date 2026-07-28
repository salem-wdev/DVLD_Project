using DVLD.People.Forms;
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

namespace DVLD
{
    public partial class ctrlPersonCard : UserControl
    {
        public ctrlPersonCard()
        {
            InitializeComponent();
            llEditPersonInfo.Enabled = false;
        }

        private int _PersonID = -1;

        private clsPerson _Person;

        public int PersonID
        {
            get
            {
               return _PersonID;
            }
        }

        public clsPerson SelectedPerson
        {
            get
            {
                return _Person;
            }
        }

        public bool LoadData(int? PersonID)
        {
            if (!PersonID.HasValue || PersonID <= 0)
                return false;

            _Person = clsPerson.Find(PersonID);
            return LoadData(_Person);
        }

        public bool LoadData(string NationalNo)
        {
            _Person = clsPerson.Find(NationalNo);
            return LoadData(_Person);
        }

        public bool LoadData(clsPerson Person)
        {
            _Person = Person;
            if (_Person != null)
            {
                _PersonID = Person.PersonID.HasValue ? (int)Person.PersonID : -1;
                _FillPersonInfo();
                return true;
            }
            else
            {
                MessageBox.Show("Could not load this person.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetPersonInfo();
                return false;
            }
        }

        private void _LoadPersonImage()
        {
            if (_Person.Gender == 0)
                pbPersonPhoto.Image = Resources.Male_512;
            else
                pbPersonPhoto.Image = Resources.Female_512;

            string ImagePath = _Person.ImagePath;
            if (ImagePath != "")
                if (File.Exists(ImagePath))
                    pbPersonPhoto.ImageLocation = ImagePath;
                else
                    MessageBox.Show("Could not find this image: = " + ImagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void _FillPersonInfo()
        {
            llEditPersonInfo.Enabled = true;
            _PersonID = _Person.PersonID.HasValue ? (int)_Person.PersonID : -1;
            lblPersonID.Text = _Person.PersonID.ToString();
            lblNationalNo.Text = _Person.NationalNo;
            lblFullName.Text = _Person.FullName;
            lblGender.Text = _Person.Gender == 0 ? "Male" : "Female";
            lblEmail.Text = _Person.Email;
            lblPhone.Text = _Person.Phone;
            lblDateOfBirth.Text = _Person.DateOfBirth.ToShortDateString();
            lblCountry.Text = clsCountry.Find(_Person.NationalityCountryID).CountryName;
            lblAddress.Text = _Person.Address;
            _LoadPersonImage();




        }

        public void ResetPersonInfo()
        {
            _PersonID = -1;
            lblPersonID.Text = "[????]";
            lblNationalNo.Text = "[????]";
            lblFullName.Text = "[????]";
            pbGender.Image = Resources.Man_32;
            lblGender.Text = "[????]";
            lblEmail.Text = "[????]";
            lblPhone.Text = "[????]";
            lblDateOfBirth.Text = "[????]";
            lblCountry.Text = "[????]";
            lblAddress.Text = "[????]";
            pbPersonPhoto.Image = Resources.Male_512;
            _Person = null;
            llEditPersonInfo.Enabled = false;

        }

        private void BackData(object sender, int? personID)
        {
            if (!personID.HasValue || personID <= 0)
                return;
            LoadData(personID);
        }

        private void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson(_Person);
            frm.SendPersonIDBack += BackData;
            frm.ShowDialog();
            frm.Close();
        }

        }
}
