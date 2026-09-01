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
    public class clsApplication
    {


        public enum enMode { AddNew = 0, Update = 1 }
        protected readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public enum enApplicationStatus : sbyte
        {
            None = -1,
            New = 1,
            Cancelled = 2,
            Completed = 3
        }
        public enum enApplicationType
        {
            NewDrivingLicense = 1, RenewDrivingLicense = 2, ReplaceLostDrivingLicense = 3,
            ReplaceDamagedDrivingLicense = 4, ReleaseDetainedDrivingLicense = 5, NewInternationalLicense = 6, RetakeTest = 7
        };
        public virtual enMode Mode { get; protected set; }

        public int ApplicationID { get; private set; }
        public int ApplicantPersonID { get; protected set; }
        private DateTime _ApplicationDate;
        public enApplicationType ApplicationTypeID { get; protected set; }
        private enApplicationStatus _ApplicationStatus;
        private DateTime _LastStatusDate;
        public decimal PaidFees { get; internal set; }
        public int CreatedByUserID { get; protected set; }

        private clsUser _CreatedByUserInfo = null;

        private clsApplicationType _ApplicationTypeInfo = null;

        private clsPerson _PersonInfo = null;

        public async Task<clsUser> CreatedByUserInfoAsync()
        {
            if (_CreatedByUserInfo == null && CreatedByUserID != -1)
            {
                _CreatedByUserInfo = await clsUser.FindAsync(CreatedByUserID);
            }
            return _CreatedByUserInfo;
        }
        public clsApplicationType ApplicationTypeInfo
        {
            get
            {
                if (_ApplicationTypeInfo == null && (int)ApplicationTypeID > 0)
                {
                    _GetApplicationTypeInfoAsync();
                }
                return _ApplicationTypeInfo;
            }
        }
        public clsPerson PersonInfo
        {
            get
            {
                if (_PersonInfo == null && ApplicantPersonID != -1)
                {
                     _FindPersonAsync();
                }
                return _PersonInfo;
            }
        }
        public DateTime ApplicationDate
        {
            get
            {
                return _ApplicationDate;
            }
        }
        public enApplicationStatus ApplicationStatus
        {
            get
            {
                return _ApplicationStatus;
            }
        }
        public DateTime LastStatusDate
        {
            get
            {
                return _LastStatusDate;
            }
        }


        private clsApplication(int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = ApplicantPersonID;
            this._ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = ApplicationTypeID;
            this._ApplicationStatus = enApplicationStatus.New;
            this._LastStatusDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = CreatedByUserID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewApplicationAsync},
                {enMode.Update,_UpdateApplicationAsync}
            };

            Mode = enMode.AddNew;
        }


        protected clsApplication()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this._ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = enApplicationType.NewDrivingLicense;
            this._ApplicationStatus = enApplicationStatus.New;
            this._LastStatusDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewApplicationAsync},
                {enMode.Update,_UpdateApplicationAsync}
            };

            Mode = enMode.AddNew;
        }


        // New overload that sets ApplicationID so instances returned from Find have correct ID
        private clsApplication(int ApplicationID, int ApplicantPersonID,
            DateTime ApplicationDate, enApplicationType ApplicationTypeID,
            enApplicationStatus ApplicationStatus, DateTime LastStatusDate,
            decimal PaidFees, int CreatedByUserID)
        {
            this.ApplicationID = ApplicationID;
            this.ApplicantPersonID = ApplicantPersonID;
            this._ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this._ApplicationStatus = ApplicationStatus;
            this._LastStatusDate = LastStatusDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewApplicationAsync},
                {enMode.Update,_UpdateApplicationAsync}
            };

            Mode = enMode.Update;
        }

        protected clsApplication(clsApplication BaseApplication)
        {
            this.ApplicationID = BaseApplication.ApplicationID;
            this.ApplicantPersonID = BaseApplication.ApplicantPersonID;
            this._ApplicationDate = BaseApplication.ApplicationDate;
            this.ApplicationTypeID = BaseApplication.ApplicationTypeID;
            this._ApplicationStatus = BaseApplication.ApplicationStatus;
            this._LastStatusDate = BaseApplication.LastStatusDate;
            this.PaidFees = BaseApplication.PaidFees;
            this.CreatedByUserID = BaseApplication.CreatedByUserID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewApplicationAsync},
                {enMode.Update,_UpdateApplicationAsync}
            };

            Mode = enMode.Update;
        }

        private async void _GetApplicationTypeInfoAsync()
        {
            _ApplicationTypeInfo = await clsApplicationType.FindAsync((int)ApplicationTypeID).ConfigureAwait(false);
        }

        protected async Task<bool> _AddNewApplicationAsync()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            var ApplicationInfo = await clsApplicationData.AddNewApplicationAsync(this.ApplicantPersonID, (int)this.ApplicationTypeID,
                this.PaidFees, this.CreatedByUserID);

            if (ApplicationInfo.IsSucceeded)
            {
                this.ApplicationID = ApplicationInfo.ApplicationID;
                this._ApplicationDate = ApplicationInfo.ApplicationDate;
                this._ApplicationStatus = (enApplicationStatus)ApplicationInfo.ApplicationStatus;
                this._LastStatusDate = ApplicationInfo.LastStatusDate;

                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        protected async Task<bool> _UpdateApplicationAsync()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            if (!await CanBeEditedAsync())
                return false;

            return await clsApplicationData.UpdateApplicationAsync(this.ApplicationID,
                this.PaidFees, this.CreatedByUserID);
        }

        private async Task _FindPersonAsync()
        {
            _PersonInfo = await clsPerson.FindAsync(ApplicantPersonID).ConfigureAwait(false);
        }

        public static async Task<bool> DeleteAsync(int ApplicationID)
        {
            if (!await CanBeEditedAsync(ApplicationID))
                return false;

            return await clsApplicationData.DeleteApplicationAsync(ApplicationID);
        }

        public virtual async Task<bool> DeleteAsync()
        {
            if (!await CanBeEditedAsync())
                return false;

            return await clsApplicationData.DeleteApplicationAsync(this.ApplicationID);
        }

        public static async Task<clsApplication> FindAsync(int ApplicationID)
        {
           
            var ApplicationInfo = await clsApplicationData.GetApplicationInfoByApplicationIDAsync(ApplicationID);

            if (ApplicationInfo.IsFound)
            {
                return new clsApplication(ApplicationID, ApplicationInfo.ApplicantPersonID,
                ApplicationInfo.ApplicationDate, (enApplicationType)ApplicationInfo.ApplicationTypeID, (enApplicationStatus)ApplicationInfo.ApplicationStatus,
                ApplicationInfo.LastStatusDate, ApplicationInfo.PaidFees, ApplicationInfo.CreatedByUserID);

            }
            else
            {
                return null;
            }
        }

        public static async Task<DataTable> GetApplicationsPersonListAsync(int ApplicantPersonID)
        {
            return await clsApplicationData.GetApplicationsPersonListAsync(ApplicantPersonID);
        }

        public static async Task<DataTable> GetApplicationsCreatedByUserListAsync(int CreatedByUserID)
        {
            return await clsApplicationData.GetApplicationsCreatedByUserListAsync(CreatedByUserID);
        }

        public static async Task<bool> IsApplicationExistsAsync(int ApplicationID)
        {
            return await clsApplicationData.IsApplicationExistAsync(ApplicationID);
        }

        public static async Task<DataTable> GetAllApplicationsAsync()
        {
            return await clsApplicationData.GetAllApplicationsAsync();
        }

        public async Task<bool> CancelAsync()
        {
            if (!await CanBeEditedAsync())
                return false;

            if (await clsApplicationData.UpdateStatusAsync(ApplicationID, (byte)enApplicationStatus.Cancelled, clsBusinessSettings.GetServerDateTime()))
            {
                this._ApplicationStatus = enApplicationStatus.Cancelled;
                return true;
            }
            return false;
        }

        internal async Task<bool> SetCompleteAsync()
        {
            if (await SetCompleteAsync(ApplicationID))
            {
                this._ApplicationStatus = enApplicationStatus.Completed;
                return true;
            }
            return false;
        }

        internal static async Task<bool> SetCompleteAsync(int ApplicationID)
        {
            if (!await CanBeEditedAsync(ApplicationID))
                return false;

            return await clsApplicationData.UpdateStatusAsync(ApplicationID, (byte)enApplicationStatus.Completed, clsBusinessSettings.GetServerDateTime());
        }

        public virtual async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }

        public static async Task<bool> DoesPersonHaveActiveApplicationAsync(int PersonID, int ApplicationTypeID)
        {
            return await clsApplicationData.DoesPersonHaveActiveApplicationAsync(PersonID, ApplicationTypeID);
        }

        public async Task<bool> DoesPersonHaveActiveApplicationAsync(int ApplicationTypeID)
        {
            return await DoesPersonHaveActiveApplicationAsync(this.ApplicantPersonID, ApplicationTypeID);
        }

        public static async Task<int> GetActiveApplicationIDAsync(int PersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            return await clsApplicationData.GetActiveApplicationIDAsync(PersonID, (int)ApplicationTypeID);
        }

        public static async Task<int> GetActiveApplicationIDForLicenseClassAsync(int PersonID, clsApplication.enApplicationType ApplicationTypeID, int LicenseClassID)
        {
            return await clsApplicationData.GetActiveApplicationIDForLicenseClassAsync(PersonID, (int)ApplicationTypeID, LicenseClassID);
        }

        public async Task<int> GetActiveApplicationIDAsync(clsApplication.enApplicationType ApplicationTypeID)
        {
            return await GetActiveApplicationIDAsync(this.ApplicantPersonID, ApplicationTypeID);
        }

        public async Task<bool> CanBeEditedAsync()
        {
            return await CanBeEditedAsync(this.ApplicationID);
        }

        public static async Task<bool> CanBeEditedAsync(int ApplicationID)
        {
            return await clsApplicationData.CanApplicationBeEditedAsync(ApplicationID);
        }

        public static async Task<enApplicationType> GetApplicationTypeIDAsync(int ApplicationID)
        {
            return (enApplicationType)await clsApplicationData.GetApplicationTypeIDAsync(ApplicationID);
        }

        public static async Task<enApplicationStatus> GetApplicationStatusAsync(int ApplicationID)
        {
            return (enApplicationStatus)await clsApplicationData.GetApplicationStatusAsync(ApplicationID);
        }

        protected static async Task<clsApplication> GetNewApplicationobjectAsync(int CreatedByUserID, int? ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            if (!ApplicantPersonID.HasValue || ApplicantPersonID <= 0)
                return null;

            if (await DoesPersonHaveActiveApplicationAsync((int)ApplicantPersonID, (int)ApplicationTypeID))
            {
                return null;
            }

            clsApplication application = new clsApplication(CreatedByUserID, (int)ApplicantPersonID, ApplicationTypeID);
            return application;
        }

        internal static async Task<clsApplication> GetNewApplicationAsync(int CreatedByUserID, int? ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {

            if (!ApplicantPersonID.HasValue || ApplicantPersonID <= 0)
                return null;

            clsApplication application = await GetNewApplicationobjectAsync(CreatedByUserID, ApplicantPersonID, ApplicationTypeID);
            if (application != null)
            {
                if (await application.SaveAsync())
                {
                    return application;
                }
            }
            return null;
        }

    }
}
