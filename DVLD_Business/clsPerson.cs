using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Business.Global_Classes;
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
                if (_Country == null && NationalityCountryID != -1)
                {
                    _Country = clsCountry.Find(NationalityCountryID);
                }
                return _Country;
            }
        }
        private bool _IsImagePathChanged = false;

        private string _OldImagePath = "";
        private string _ImagePath = "";
        public string ImagePath
        {
            get
            {
                return _ImagePath;
            }
            set
            {
                if (value != _ImagePath)
                {
                    _IsImagePathChanged = true;
                    _OldImagePath = _ImagePath;
                    _ImagePath = value;
                }
            }
        }

        private clsPerson()
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

        private clsPerson(string FirstName, string SecondName, string ThirdName
           , string LastName, string NationalNo, DateTime DateOfBirth, enGenderType Gender
           , string Address, string Phone, string Email, int NationalityCountryID
           , string ImagePath)
        {
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.NationalNo = NationalNo;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this._ImagePath = ImagePath;

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
            this._ImagePath = ImagePath;

            Mode = enMode.Update;
        }

        private bool _AddNewPerson()
        {
            if (!string.IsNullOrWhiteSpace(this._ImagePath))
            {
                string sourceFilePath = this.ImagePath;
                if (!clsFileStorage.CopyImageToProjectImagesFolder(ref sourceFilePath, @"C:\DVLD-People-Images\"))
                {
                    return false;
                }
                this.ImagePath = sourceFilePath;
            }
            
            this.PersonID = clsPersonData.AddNewPerson(this.FirstName,  this.SecondName,  this.ThirdName
                , this.LastName,  this.NationalNo,  this.DateOfBirth,  (short)this.Gender,  this.Address,  this.Phone,  this.Email
                , this.NationalityCountryID,  this.ImagePath);

            return (PersonID != -1);
        }

        private bool _UpdatePerson()
        {
            if (IsPersonExists(NationalNo))
            {
                return false;
            }

            if (this._IsImagePathChanged && !string.IsNullOrWhiteSpace(this.ImagePath))
            {
                string sourceFilePath = this.ImagePath;
                if (!clsFileStorage.CopyImageToProjectImagesFolder(ref sourceFilePath, @"C:\DVLD-People-Images\"))
                {
                    return false;
                }
                this.ImagePath = sourceFilePath;
            }

            if ( clsPersonData.UpdatePerson(PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName
                , DateOfBirth, (short)Gender, Address, Phone, Email, NationalityCountryID, ImagePath))
            {
                if (!string.IsNullOrEmpty(_OldImagePath))
                    if (clsFileStorage.DeleteFile(_OldImagePath))
                    {
                        _IsImagePathChanged = false;
                        _OldImagePath = "";
                    }
                return true;
            }
            return false;
        }

        public static bool Delete(int PersonID)
        {
            string ImagePath = "";
            ImagePath = Find(PersonID)?.ImagePath;
            if ( clsPersonData.DeletePerson(PersonID))
            {
                if (!string.IsNullOrWhiteSpace(ImagePath))
                {
                    clsFileStorage.DeleteFile(ImagePath);
                }
                return true;
            }
            return false;
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

        public static bool HasPeople()
        {
            return clsPersonData.HasPeople();
        }

        private static bool _IsValidInfo(string NationalNo, string FirstName, string SecondName,
             string LastName, DateTime DateOfBirth,string Address,
            string Phone, int NationalityCountryID, string Email = "")
        {
            string[] Array = new string[] { NationalNo, FirstName, SecondName, LastName, Address, Phone, };
            if (Array.Any(string.IsNullOrWhiteSpace))
            {
                return false;
            }

            if (NationalityCountryID < 1 || NationalityCountryID > 191)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
            {
                return false;
            }

            if (DateOfBirth > clsBusinessSettings.GetServerDateTime().AddYears(-18))
            {
                return false;
            }

            if (IsPersonExists(NationalNo))
            {
                return false;
            }

            return true;
        }

        private static clsPerson _GetReadyObj(string NationalNo, string FirstName, string SecondName,
             string LastName, DateTime DateOfBirth, enGenderType Gender, string Address,
            string Phone, int NationalityCountryID, string ThirdName = "", string Email = "", string ImagePath = "")
        {

            if (!_IsValidInfo(NationalNo, FirstName, SecondName, LastName, DateOfBirth
                , Address, Phone, NationalityCountryID, Email))
            {
                return null;
            }

            return new clsPerson(FirstName, SecondName, ThirdName, LastName, NationalNo, DateOfBirth
                , Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
        }

        public static clsPerson CreateNewPerson(string NationalNo, string FirstName, string SecondName,
             string LastName, DateTime DateOfBirth, enGenderType Gender, string Address,
            string Phone, int NationalityCountryID, string ThirdName = "", string Email = "", string ImagePath = "")
        {
            clsPerson NewPerson = _GetReadyObj(NationalNo, FirstName, SecondName, LastName, DateOfBirth, Gender
                , Address, Phone, NationalityCountryID, ThirdName, Email, ImagePath);
            if(NewPerson != null)
            {
                if(NewPerson.Save())
                {
                    return NewPerson;
                }
            }
            return null;
        }
    }
}
