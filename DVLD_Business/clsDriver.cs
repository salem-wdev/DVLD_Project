using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Business.Users;

namespace DVLD_Business
{
    public class clsDriver
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { private set; get; } = enMode.AddNew;
        private Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

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

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewDriverAsync},
                {enMode.Update,_UpdateDriverAsync}
            };

            Mode = enMode.AddNew;

        }

        private clsDriver(int DriverID, int? PersonID, int CreatedByUserID, DateTime CreatedDate)

        {
            this.DriverID = DriverID;
            this.PersonID = PersonID;
            this.CreatedByUserID = CreatedByUserID;
            this.CreatedDate = CreatedDate;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewDriverAsync},
                {enMode.Update,_UpdateDriverAsync}
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewDriverAsync()
        {
            //call DataAccess Layer 

            this.DriverID = await clsDriverData.AddNewDriverAsync(PersonID, CreatedByUserID).ConfigureAwait(false);
            this.CreatedDate = clsUtilData.GetServerDate();

            if (this.DriverID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private async Task<bool> _UpdateDriverAsync()
        {
            //call DataAccess Layer 

            return await clsDriverData.UpdateDriverAsync(this.DriverID, this.PersonID, this.CreatedByUserID);
        }

        public static async Task<clsDriver> FindByDriverIDAsync(int DriverID)
        {
            var driverInfo = await clsDriverData.GetDriverInfoByDriverIDAsync(DriverID).ConfigureAwait(false);

            if (driverInfo.IsFound)
                return new clsDriver(DriverID, driverInfo.PersonID, driverInfo.CreatedByUserID, driverInfo.CreatedDate);
            else
                return null;
        }

        public static async Task<clsDriver> FindByPersonIDAsync(int? PersonID)
        {

            if (PersonID == null || PersonID <= 0)
                return null;

            var DriverInfo = await clsDriverData.GetDriverInfoByPersonIDAsync(PersonID).ConfigureAwait(false);

            if (DriverInfo.IsFound)

                return new clsDriver(DriverInfo.DriverID, PersonID, DriverInfo.CreatedByUserID, DriverInfo.CreatedDate);
            else
                return null;

        }

        public static async Task<DataTable> GetAllDriversAsync()
        {
            return await clsDriverData.GetAllDriversAsync();

        }

        public static DataTable GetLicenses(int DriverID)
        {
            return clsLicenseData.GetDriverLicenses(DriverID);
        }

        public static DataTable GetInternationalLicenses(int DriverID)
        {
            return clsInternationalLicense.GetDriverInternationalLicenses(DriverID);
        }

        public async Task<bool> SaveAsync()
        {
           return await _saveDictionary[Mode]().ConfigureAwait(false);
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

            if (FindByPersonIDAsync(PersonID) != null)
            {
                return null;
            }

            return new clsDriver(PersonID, CreatedByUserID);
        }

        internal static async Task<clsDriver> CreateNewDriverAsync(int PersonID, int CreatedByUserID)
        {
            clsDriver driver = _PrepareDriver(PersonID, CreatedByUserID);

            if(driver != null)
            {
                if(await driver.SaveAsync().ConfigureAwait(false))
                {
                    return driver;
                }
            }
            return null;
        }
    }
}
