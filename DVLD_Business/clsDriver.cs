using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsDriver
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { private set; get; } = enMode.AddNew;
        private readonly Dictionary<enMode, Func<bool>> _saveDictionary;

        private clsPerson _PersonInfo = null;
        public clsPerson PersonInfo
        {
            get
            {
                if (_PersonInfo == null && this.PersonID != -1)
                {
                    _PersonInfo = clsPerson.Find(this.PersonID);
                }
                return _PersonInfo;
            }
        }

        public int DriverID { private set; get; }
        public int? PersonID { private set; get; }
        public int CreatedByUserID { private set; get; }
        public DateTime CreatedDate { private set; get; }

        private clsDriver(int? PersonID, int CreatedByUserID)

        {
            this.DriverID = -1;
            this.PersonID = PersonID;
            this.CreatedByUserID = CreatedByUserID;
            this.CreatedDate = DateTime.Now;

            _saveDictionary = new Dictionary<enMode, Func<bool>>
            {
                {enMode.AddNew,_AddNewDriver},
                {enMode.Update,_UpdateDriver}
            };

            Mode = enMode.AddNew;

        }

        private clsDriver(int DriverID, int? PersonID, int CreatedByUserID, DateTime CreatedDate)

        {
            this.DriverID = DriverID;
            this.PersonID = PersonID;
            this.CreatedByUserID = CreatedByUserID;
            this.CreatedDate = CreatedDate;

            _saveDictionary = new Dictionary<enMode, Func<bool>>
            {
                {enMode.AddNew,_AddNewDriver},
                {enMode.Update,_UpdateDriver}
            };

            Mode = enMode.Update;
        }

        private bool _AddNewDriver()
        {
            //call DataAccess Layer 

            this.DriverID = clsDriverData.AddNewDriver(PersonID, CreatedByUserID);
            this.CreatedDate = clsUtilData.GetServerDate();

            if (this.DriverID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateDriver()
        {
            //call DataAccess Layer 

            return clsDriverData.UpdateDriver(this.DriverID, this.PersonID, this.CreatedByUserID);
        }

        public static clsDriver FindByDriverID(int DriverID)
        {

            int PersonID = -1; int CreatedByUserID = -1; DateTime CreatedDate = DateTime.Now;

            if (clsDriverData.GetDriverInfoByDriverID(DriverID, ref PersonID, ref CreatedByUserID, ref CreatedDate))

                return new clsDriver(DriverID, PersonID, CreatedByUserID, CreatedDate);
            else
                return null;

        }

        public static clsDriver FindByPersonID(int? PersonID)
        {

            if (PersonID == null || PersonID <= 0)
                return null;

            int DriverID = -1; int CreatedByUserID = -1; DateTime CreatedDate = DateTime.Now;

            if (clsDriverData.GetDriverInfoByPersonID(PersonID, ref DriverID, ref CreatedByUserID, ref CreatedDate))

                return new clsDriver(DriverID, PersonID, CreatedByUserID, CreatedDate);
            else
                return null;

        }

        public static DataTable GetAllDrivers()
        {
            return clsDriverData.GetAllDrivers();

        }

        public static DataTable GetLicenses(int DriverID)
        {
            return clsLicenseData.GetDriverLicenses(DriverID);
        }

        public static DataTable GetInternationalLicenses(int DriverID)
        {
            return clsInternationalLicense.GetDriverInternationalLicenses(DriverID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDriver())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateDriver();

            }

            return false;
        }

        public static int GetLastLicenseID(int DriverID, int LicenseClassID)
        {
            return clsLicenseData.GetLastLicenseIDByDriverID(DriverID,LicenseClassID);
        }

        private static clsDriver _PrepareDriver(int PersonID, int CreatedByUserID)
        {
            if (!clsPerson.IsPersonExists(PersonID) || !clsUser.IsUserExists(CreatedByUserID))
            {
                return null;
            }

            if (FindByPersonID(PersonID) != null)
            {
                return null;
            }

            return new clsDriver(PersonID, CreatedByUserID);
        }

        internal static clsDriver CreateNewDriver(int PersonID, int CreatedByUserID)
        {
            clsDriver driver = _PrepareDriver(PersonID, CreatedByUserID);

            if(driver != null)
            {
                if(driver.Save())
                {
                    return driver;
                }
            }
            return null;
        }
    }
}
