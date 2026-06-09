using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsPerson
    {
        public enum enGenderType { Male = 0, Female = 1 }
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; }

        public int PersonID { get; private set; }
        public string NationalNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; } 
        public string ThirdName { get; set; }
        public string LastName { get; set; } 
        public string FullName
        {
            get
            {
                return $"{FirstName} {SecondName} {ThirdName} {LastName}".Trim();
            }
        }
        public DateTime DateOfBirth { get; set; }
        public enGenderType Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int NationalityCountryID { get; set; }

        private clsCountry _Country = null;
        clsCountry Country
        {
            get
            {
                if (_Country != null && NationalityCountryID != -1)
                {
                    _Country = clsCountry.Find(NationalityCountryID);
                }
                return _Country;
            }
        }
        public string ImagePath { get; set; }

        public clsPerson()
        {
            PersonID = -1;
            FirstName = string.Empty;
            SecondName = string.Empty;
            ThirdName = string.Empty;
            LastName = string.Empty;
            NationalNo = string.Empty;
            DateOfBirth = DateTime.Now;
            Gender = 0;
            Address = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            NationalityCountryID = 1;
            ImagePath = string.Empty;

            Mode = enMode.AddNew;
        }

        

        // New overload that sets PersonID so instances returned from Find have correct ID
        private clsPerson(int PersonID, string FirstName, string SecondName, string ThirdName
            ,string LastName, string NationalNo, DateTime DateOfBirth, short Gender 
            ,string Address, string Phone, string Email, int NationalityCountryID
            ,string ImagePath)
        {
            this.PersonID = PersonID;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.NationalNo = NationalNo;
            this.DateOfBirth = DateOfBirth;
            this.Gender = (enGenderType)Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this.ImagePath = ImagePath;

            Mode = enMode.Update;
        }

        private bool _AddNewPerson()
        {
            this.PersonID = clsPersonData.AddNewPerson(this.FirstName,  this.SecondName,  this.ThirdName
                , this.LastName,  this.NationalNo,  this.DateOfBirth,  (short)this.Gender,  this.Address,  this.Phone,  this.Email
                , this.NationalityCountryID,  this.ImagePath);

            return (PersonID != -1);
        }

        private bool _UpdatePerson()
        {
            return clsPersonData.UpdatePerson(PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName
                , DateOfBirth, (short)Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
        }

        public static bool Delete(int PersonID)
        {
            return clsPersonData.DeletePerson(PersonID);
        }

        public static clsPerson Find(int PersonID)
        {
            string FirstName = string.Empty;
            string SecondName = string.Empty;
            string ThirdName = string.Empty;
            string LastName = string.Empty;
            string NationalNo = string.Empty;
            DateTime DateOfBirth = DateTime.Now;
            short Gender = 0;
            string Address = string.Empty;
            string Phone = string.Empty;
            string Email = string.Empty;
            int NationalityCountryID = 1;
            string ImagePath = string.Empty;

            bool found = clsPersonData.GetPersonInfoByID(PersonID, ref NationalNo, ref FirstName
                , ref SecondName, ref ThirdName, ref LastName,  ref DateOfBirth, ref Gender, ref Address
                , ref Phone, ref Email, ref NationalityCountryID, ref ImagePath);

            if (found)
            {
                return new clsPerson(PersonID, FirstName, SecondName, ThirdName, LastName
                    , NationalNo, DateOfBirth, Gender, Address, Phone, Email
                    , NationalityCountryID, ImagePath);
            }
            else
            {
                return null;
            }
        }

        public static clsPerson Find(string NationalNo)
        {
            int PersonID = -1;
            string FirstName = string.Empty;
            string SecondName = string.Empty;
            string ThirdName = string.Empty;
            string LastName = string.Empty;
            DateTime DateOfBirth = DateTime.Now;
            short Gender = 0;
            string Address = string.Empty;
            string Phone = string.Empty;
            string Email = string.Empty;
            int NationalityCountryID = 1;
            string ImagePath = string.Empty;

            bool found = clsPersonData.GetPersonInfoByNationalNo(NationalNo, ref PersonID, ref FirstName
                , ref SecondName, ref ThirdName, ref LastName, ref DateOfBirth, ref Gender, ref Address
                , ref Phone, ref Email, ref NationalityCountryID, ref ImagePath);

            if (found)
            {
                return new clsPerson(PersonID, FirstName, SecondName, ThirdName, LastName
                    , NationalNo, DateOfBirth, Gender, Address, Phone, Email
                    , NationalityCountryID, ImagePath);
            }
            else
            {
                return null;
            }
        }

        public static bool IsPersonExists(int PersonID)
        {
            return clsPersonData.IsPersonExists(PersonID);
        }

        public static bool IsPersonExists(string NationalNo)
        {
            return clsPersonData.IsPersonExists(NationalNo);
        }

        public static DataTable GetAllPeople()
        {
            return clsPersonData.GetAllPeople();
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewPerson())
                        {
                            Mode = enMode.Update;
                            return true;
                        }

                        return false;
                    }
                case enMode.Update:
                    {
                        return _UpdatePerson();
                    }
                default:
                    {
                        return false; 
                    }
            }
        }

    }
}
