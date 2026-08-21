using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Infrastructure.Storage;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsPerson
    {
        public enum enGenderType { Male = 0, Female = 1 }
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; }

        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public Nullable<int> PersonID { get; private set; } = null;
        private bool _IsNationalNoChanged = false;
        private string _NationalNo = "";

        public string NationalNo {
            get
            {
                return _NationalNo;
            }
            set
            {
                if (value != _NationalNo)
                {
                    _IsNationalNoChanged = true;
                    _NationalNo = value;
                }
            }
        }
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
        public async Task<clsCountry> GetCountryAsync()
        {
            if (_Country == null && NationalityCountryID != -1)
            {
                _Country = await clsCountry.FindAsync(NationalityCountryID);
            }
            return _Country;
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
                if (value != _OldImagePath)
                {
                    _IsImagePathChanged = true;
                    _ImagePath = value;
                }
                else
                {
                    _IsImagePathChanged = false;
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

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewPersonAsync},
                {enMode.Update,_UpdatePersonAsync}
            };

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
            this._NationalNo = NationalNo;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this._ImagePath = ImagePath;
            this._OldImagePath = ImagePath;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewPersonAsync},
                {enMode.Update,_UpdatePersonAsync}
            };

            Mode = enMode.AddNew;
        }


        // New overload that sets PersonID so instances returned from Find have correct ID
        private clsPerson(int? PersonID, string FirstName, string SecondName, string ThirdName
            ,string LastName, string NationalNo, DateTime DateOfBirth, short Gender 
            ,string Address, string Phone, string Email, int NationalityCountryID
            ,string ImagePath)
        {
            this.PersonID = PersonID;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this._NationalNo = NationalNo;
            this.DateOfBirth = DateOfBirth;
            this.Gender = (enGenderType)Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this._ImagePath = ImagePath;
            this._OldImagePath = ImagePath;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewPersonAsync},
                {enMode.Update,_UpdatePersonAsync}
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewPersonAsync()
        {
            if (!string.IsNullOrWhiteSpace(this._ImagePath))
            {
                string sourceFilePath = this.ImagePath;
                if (!clsFileStorage.CopyFileToDestinationFolderWithGUID(ref sourceFilePath, @"C:\DVLD-People-Images\"))
                {
                    return false;
                }
                this._ImagePath = sourceFilePath;
            }
            
            this.PersonID = await clsPersonData.AddNewPersonAsync(this.FirstName,  this.SecondName,  this.ThirdName
                , this.LastName,  this.NationalNo,  this.DateOfBirth,  (short)this.Gender,  this.Address,  this.Phone,  this.Email
                , this.NationalityCountryID,  this.ImagePath);

            if (PersonID != -1)
            {
                this._OldImagePath = this._ImagePath;
                this._IsImagePathChanged = false;
                Mode = enMode.Update;
                return true;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_ImagePath) && this._IsImagePathChanged)
                {
                    clsFileStorage.DeleteFile(_ImagePath);
                    this._ImagePath = this._OldImagePath;
                    this._IsImagePathChanged = false;
                }
            }
            return false;
        }

        private async Task<bool> _UpdatePersonAsync()
        {
            if (string.IsNullOrWhiteSpace(_NationalNo))
            {
                return false;
            }

            if (_IsNationalNoChanged && await _IsNationalNoUsedAsync(this.PersonID, NationalNo))
            {
                return false;
            }

            if (this._IsImagePathChanged && !string.IsNullOrWhiteSpace(this.ImagePath))
            {
                string sourceFilePath = this.ImagePath;
                if (!clsFileStorage.CopyFileToDestinationFolderWithGUID(ref sourceFilePath, @"C:\DVLD-People-Images\"))
                {
                    return false;
                }
                this._ImagePath = sourceFilePath;
            }

            if (await clsPersonData.UpdatePersonAsync(PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName
                , DateOfBirth, (short)Gender, Address, Phone, Email, NationalityCountryID, ImagePath))
            {
                if (!string.IsNullOrEmpty(_OldImagePath) && this._IsImagePathChanged)
                {
                    clsFileStorage.DeleteFile(_OldImagePath);
                }
                _IsImagePathChanged = false;

                _OldImagePath = _ImagePath;
                return true;
            }
            else if (!string.IsNullOrEmpty(_ImagePath) && this._IsImagePathChanged)
            {
                if(clsFileStorage.DeleteFile(_ImagePath))
                {
                    _ImagePath = _OldImagePath;
                    _IsImagePathChanged = false;
                }
            }
                return false;
        }

        public static async Task<bool> DeleteAsync(int PersonID)
        {
            string ImagePath = "";
            ImagePath = (await FindAsync(PersonID).ConfigureAwait(false))?.ImagePath;
            if (await clsPersonData.DeletePersonAsync(PersonID))
            {
                if (!string.IsNullOrWhiteSpace(ImagePath))
                {
                    clsFileStorage.DeleteFile(ImagePath);
                }
                return true;
            }
            return false;
        }

        public static async Task<clsPerson> FindAsync(int? PersonID)
        {
            if (PersonID == null)
                return null;

            var PersonInfo = await clsPersonData.GetPersonInfoByIDAsync(PersonID).ConfigureAwait(false);

            if (PersonInfo.IsFound)
            {
                return new clsPerson(PersonID, PersonInfo.FirstName, PersonInfo.SecondName, PersonInfo.ThirdName, PersonInfo.LastName
                    , PersonInfo.NationalNo, PersonInfo.DateOfBirth, PersonInfo.Gender, PersonInfo.Address, PersonInfo.Phone, PersonInfo.Email
                    , PersonInfo.NationalityCountryID, PersonInfo.ImagePath);
            }
            else
            {
                return null;
            }
        }

        public static async Task<clsPerson> FindAsync(string NationalNo)
        {
            var PersonInfo = await clsPersonData.GetPersonInfoByNationalNoAsync(NationalNo);

            if (PersonInfo.IsFound)
            {
                return new clsPerson(PersonInfo.PersonID, PersonInfo.FirstName, PersonInfo.SecondName, PersonInfo.ThirdName, PersonInfo.LastName
                    , NationalNo, PersonInfo.DateOfBirth, PersonInfo.Gender, PersonInfo.Address, PersonInfo.Phone, PersonInfo.Email
                    , PersonInfo.NationalityCountryID, PersonInfo.ImagePath);
            }
            else
            {
                return null;
            }
        }

        public static async Task<bool> IsPersonExistsAsync(int PersonID)
        {
            return await clsPersonData.IsPersonExistsAsync(PersonID);
        }

        public static async Task<bool> IsPersonExistsAsync(string NationalNo)
        {
            return await clsPersonData.IsPersonExistsAsync(NationalNo);
        }

        private static async Task<bool> _IsNationalNoUsedAsync(int? PersonID, string NationalNo)
        {
            return await clsPersonData.IsNationalNoUsedAsync(PersonID, NationalNo);
        }

        public static async Task<DataTable> GetAllPeopleAsync()
        {
            return await clsPersonData.GetAllPeopleAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }

        public static async Task<bool> HasPeopleAsync()
        {
            return await clsPersonData.HasPeopleAsync();
        }

        private static async Task<bool> _IsValidInfoAsync(string NationalNo, string FirstName, string SecondName,
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

            if (await IsPersonExistsAsync(NationalNo))
            {
                return false;
            }

            return true;
        }

        private static async Task<clsPerson> _GetReadyObjAsync(string NationalNo, string FirstName, string SecondName,
             string LastName, DateTime DateOfBirth, enGenderType Gender, string Address,
            string Phone, int NationalityCountryID, string ThirdName = "", string Email = "", string ImagePath = "")
        {

            if (!await _IsValidInfoAsync(NationalNo, FirstName, SecondName, LastName, DateOfBirth
                , Address, Phone, NationalityCountryID, Email))
            {
                return null;
            }

            return new clsPerson(FirstName, SecondName, ThirdName, LastName, NationalNo, DateOfBirth
                , Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
        }

        public static async Task<clsPerson> CreateNewPersonAsync(string NationalNo, string FirstName, string SecondName,
             string LastName, DateTime DateOfBirth, enGenderType Gender, string Address,
            string Phone, int NationalityCountryID, string ThirdName = "", string Email = "", string ImagePath = "")
        {
            clsPerson NewPerson = await _GetReadyObjAsync(NationalNo, FirstName, SecondName, LastName, DateOfBirth, Gender
                , Address, Phone, NationalityCountryID, ThirdName, Email, ImagePath);
            if(NewPerson != null)
            {
                if(await NewPerson.SaveAsync())
                {
                    return NewPerson;
                }
            }
            return null;
        }
    }
}
