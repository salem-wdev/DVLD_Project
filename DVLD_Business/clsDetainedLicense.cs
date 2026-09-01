using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Business.Users;
using DVLD_Shared.Utilities;


namespace DVLD_Business
{
    public class clsDetainedLicense
    {
        public sealed class LicenseDetainedEventArgs : EventArgs
        {
            public int DetainID { get; }
            public int LicenseID { get; }
            public DateTime DetainDate { get; }
            public float FineFees { get; }
            public int CreatedByUserID { get; }
            public bool IsReleased { get; }

            public LicenseDetainedEventArgs(int DetainID, int LicenseID, DateTime DetainDate,
                float FineFees, int CreatedByUserID, bool IsReleased)
            {
                this.DetainID = DetainID;
                this.LicenseID = LicenseID;
                this.DetainDate = DetainDate;
                this.FineFees = FineFees;
                this.CreatedByUserID = CreatedByUserID;
                this.IsReleased = IsReleased;
            }
        }

        public static event EventHandler<LicenseDetainedEventArgs> LicenseDetained;

        public sealed class LicenseReleasedEventArgs : EventArgs
        {
            public int DetainID { get; }
            public int LicenseID { get; }
            public DateTime ReleaseDate { get; }
            public int ReleasedByUserID { get; }
            public int ReleaseApplicationID { get; }
            public bool IsReleased { get; }

            public LicenseReleasedEventArgs(int DetainID, int LicenseID, DateTime ReleaseDate,
            int ReleasedByUserID, int ReleaseApplicationID, bool IsReleased)
            {
                this.DetainID = DetainID;
                this.LicenseID = LicenseID;
                this.ReleaseDate = ReleaseDate;
                this.ReleasedByUserID = ReleasedByUserID;
                this.ReleaseApplicationID = ReleaseApplicationID;
                this.IsReleased = IsReleased;
            }
        }

        public static event EventHandler<LicenseReleasedEventArgs> LicenseReleased;

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { private set; get; } = enMode.AddNew;

        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public int DetainID { private set; get; }
        public int LicenseID { private set; get; }
        public DateTime DetainDate { private set; get; }

        public float FineFees { private set; get; }
        public int CreatedByUserID { private set; get; }

        private clsUser _CreatedByUserInfo = null;
        public async Task<clsUser> CreatedByUserInfoAsync()
        {
            if (_CreatedByUserInfo == null && CreatedByUserID != -1)
            {
                _CreatedByUserInfo = await clsUser.FindAsync(CreatedByUserID);
            }
            return _CreatedByUserInfo;

        }

        public bool IsReleased { private set; get; }
        public DateTime ReleaseDate { private set; get; }
        public int ReleasedByUserID { private set; get; }

        private clsUser _ReleasedByUserInfo = null;
        public async Task<clsUser> ReleasedByUserInfoAsync()
        {
            if (_ReleasedByUserInfo == null && ReleasedByUserID != -1)
            {
                _ReleasedByUserInfo = await clsUser.FindAsync(ReleasedByUserID);
            }
            return _ReleasedByUserInfo;
        }

        public int ReleaseApplicationID { private set; get; }
        private clsApplication _ReleaseApplicationInfo = null;
        public clsApplication ReleaseApplicationInfo
        {
            get
            {
                if (_ReleaseApplicationInfo == null && ReleaseApplicationID != -1)
                {
                    _LoadReleaseApplicationInfo();
                }
                return _ReleaseApplicationInfo;
            }
        }

        private clsDetainedLicense(int LicenseID, DateTime DetainDate, float FineFees, int CreatedByUserID)

        {
            this.DetainID = -1;
            this.LicenseID = LicenseID;
            this.DetainDate = DetainDate;
            this.FineFees = FineFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsReleased = false;
            this.ReleaseDate = DateTime.MaxValue;
            this.ReleasedByUserID = 0;
            this.ReleaseApplicationID = -1;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewDetainedLicenseAsync},
                {enMode.Update,_UpdateDetainedLicenseAsync}
            };

            Mode = enMode.AddNew;

        }

        private clsDetainedLicense(int DetainID,
            int LicenseID, DateTime DetainDate,
            float FineFees, int CreatedByUserID,
            bool IsReleased, DateTime ReleaseDate,
            int ReleasedByUserID, int ReleaseApplicationID)

        {
            this.DetainID = DetainID;
            this.LicenseID = LicenseID;
            this.DetainDate = DetainDate;
            this.FineFees = FineFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsReleased = IsReleased;
            this.ReleaseDate = ReleaseDate;
            this.ReleasedByUserID = ReleasedByUserID;
            this.ReleaseApplicationID = ReleaseApplicationID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewDetainedLicenseAsync},
                {enMode.Update,_UpdateDetainedLicenseAsync}
            };

            Mode = enMode.Update;
        }

        private async void _LoadReleaseApplicationInfo()
        {
            _ReleaseApplicationInfo = await clsApplication.FindAsync(this.ReleaseApplicationID);
        }

        private async Task<bool> _AddNewDetainedLicenseAsync()
        {
            //call DataAccess Layer 

            this.DetainID = await clsDetainedLicenseData.AddNewDetainedLicenseAsync(
                this.LicenseID, this.DetainDate, this.FineFees, this.CreatedByUserID);

            if (this.DetainID != -1) 
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private async Task<bool> _UpdateDetainedLicenseAsync()
        {
            //call DataAccess Layer 

            return await clsDetainedLicenseData.UpdateDetainedLicenseAsync(
                this.DetainID, this.LicenseID, this.DetainDate, this.FineFees, this.CreatedByUserID);
        }

        public static async Task<clsDetainedLicense> FindAsync(int DetainID)
        {

            var result = await clsDetainedLicenseData.GetDetainedLicenseInfoByIDAsync(DetainID);

            if(result.IsFound)
                return new clsDetainedLicense(DetainID,
                     result.LicenseID, result.DetainDate,
                     result.FineFees, result.CreatedByUserID,
                     result.IsReleased, result.ReleaseDate,
                     result.ReleasedByUserID, result.ReleaseApplicationID);
            else
                return null;

        }

        public static async Task<DataTable> GetAllDetainedLicensesAsync()
        {
            return await clsDetainedLicenseData.GetAllDetainedLicensesAsync();

        }

        public static async Task<clsDetainedLicense> FindByLicenseIDAsync(int LicenseID)
        {
           
            var result = await clsDetainedLicenseData.GetDetainedLicenseInfoByLicenseIDAsync(LicenseID);
            if(result.IsFound)
                return new clsDetainedLicense(result.DetainID,
                     LicenseID, result.DetainDate,
                     result.FineFees, result.CreatedByUserID,
                     result.IsReleased, result.ReleaseDate,
                     result.ReleasedByUserID, result.ReleaseApplicationID);
            else
                return null;

        }

        public async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }

        public static async Task<bool> IsLicenseDetainedAsync(int LicenseID)
        {
            return await clsDetainedLicenseData.IsLicenseDetainedAsync(LicenseID);
        }

        protected void OnLicenseDetained(LicenseDetainedEventArgs e)
        {
            LicenseDetained?.Invoke(this, e);
        }

        protected void OnLicenseReleased(LicenseReleasedEventArgs e)
        {
            LicenseReleased?.Invoke(this, e);
        }

        internal static async Task<clsDetainedLicense> ReleaseDetainedLicenseAsync(int LicenseID, int ReleasedByUserID)
        {
            if (!await clsUser.IsUserExistsAsync(ReleasedByUserID)
                || !await IsLicenseDetainedAsync(LicenseID))
            {
                return null;
            }

            clsLicense license = await clsLicense.FindAsync(LicenseID);
            if (license == null || license.DriverInfo == null)
            {
                return null;
            }

            clsApplication ReleaseApplication
                = await clsApplication.GetNewApplicationAsync(ReleasedByUserID,
                license.DriverInfo.PersonID, clsApplication.enApplicationType.ReleaseDetainedDrivingLicense);

            if (ReleaseApplication == null)
            {
                return null;
            }

            DateTime ReleaseDate = clsDateTime.GetCurrentDateTime();
            if(ReleaseDate == DateTime.MinValue)
            {
                return null;
            }

            clsDetainedLicense DetainedLicense = await FindByLicenseIDAsync(LicenseID);
            if(DetainedLicense == null )
            {
                return null;
            }

            if (await clsDetainedLicenseData.ReleaseDetainedLicenseAsync(DetainedLicense.DetainID,
                  ReleaseDate, ReleasedByUserID, ReleaseApplication.ApplicationID))
            {
                DetainedLicense.IsReleased = true;
                DetainedLicense.ReleaseApplicationID = ReleaseApplication.ApplicationID;
                DetainedLicense.ReleasedByUserID = ReleasedByUserID;
                DetainedLicense.ReleaseDate = ReleaseDate;
                DetainedLicense?.ReleaseApplicationInfo?.SetCompleteAsync();

                DetainedLicense?.OnLicenseReleased(new LicenseReleasedEventArgs(DetainedLicense.DetainID, DetainedLicense.LicenseID,
                    DetainedLicense.ReleaseDate, DetainedLicense.ReleasedByUserID, DetainedLicense.ReleaseApplicationID,
                    DetainedLicense.IsReleased));
                return DetainedLicense;
            }

            return null;
        }

        private static async Task<clsDetainedLicense> _CreateNewDetainedLicenseAsync(int LicenseID, float FineFees, int CreatedByUserID)
        {
            if(!await clsLicense.IsLicenseActiveAsync(LicenseID) || !await clsUser.IsUserExistsAsync(CreatedByUserID))
            {
                return null;
            }

            if(await IsLicenseDetainedAsync(LicenseID))
            {
                return null;
            }
            DateTime CurrentDate = clsDateTime.GetCurrentDateTime();
            if (CurrentDate == DateTime.MinValue)
            {
                return null;
            }

            return new clsDetainedLicense(LicenseID, CurrentDate, FineFees, CreatedByUserID);
        }

        internal static async Task<clsDetainedLicense> DetainedLicenseAsync(int LicenseID, float FineFees, int CreatedByUserID)
        {
            clsDetainedLicense license = await _CreateNewDetainedLicenseAsync(LicenseID, FineFees, CreatedByUserID);
            if (license != null)
            {
                if(await license.SaveAsync())
                {
                    license?.OnLicenseDetained(new LicenseDetainedEventArgs(license.DetainID, license.LicenseID, license.DetainDate,
                        license.FineFees, license.CreatedByUserID, license.IsReleased));
                    return license;
                }
            }
            return null;
        }

    }
}
