using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsLicense : IDisposable
    {
        public sealed class LicenseUpdatedEventArgs : EventArgs
        {
            public int LicenseID { get; }
            public int DriverID { get; }
            public DateTime ExpirationDate { get; }
            public string Notes { get; }
            public float PaidFees { get; }
            public bool IsActive { get; }
            public bool IsDetained { get; }
            
            public LicenseUpdatedEventArgs(int LicenseID, int DriverID, DateTime ExpirationDate, string Notes,
                float PaidFees, bool IsActive, bool IsDetained)
            {
                this.LicenseID = LicenseID;
                this.DriverID = DriverID;
                this.ExpirationDate = ExpirationDate;
                this.Notes = Notes;
                this.PaidFees = PaidFees;
                this.IsActive = IsActive;
                this.IsDetained = IsDetained;
            }


        }
        public event EventHandler<LicenseUpdatedEventArgs> LicenseUpdated;

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDispatcher;

        public enum enIssueReason { FirstTime = 1, Renew = 2, DamagedReplacement = 3, LostReplacement = 4 };

        public int LicenseID { private set; get; }
        public int ApplicationID { private set; get; }
        public int DriverID { private set; get; }
        public int LicenseClassID { private set; get; }

        private DateTime _IssueDate;
        private DateTime _ExpirationDate;

        public DateTime IssueDate { get => _IssueDate; }
        public DateTime ExpirationDate { get => _ExpirationDate; }
        public string Notes { set; get; }
        public float PaidFees { private set; get; }
        public bool IsActive { private set; get; }
        public enIssueReason IssueReason { private set; get; }
        public string IssueReasonText
        {
            get
            {
                return GetIssueReasonText(this.IssueReason);
            }
        }

        public int CreatedByUserID { private set; get; }

        private bool _IsDetained = false;

        public bool IsDetained
        {
            get
            {
                _GetIsDenidedInfo();
                return _IsDetained;
            }
        }

        private clsDriver _DriverInfo = null;
        public clsDriver DriverInfo
        {
            get
            {
                if (_DriverInfo == null && DriverID != -1)
                {
                    _GetDriverInfo();
                }
                return _DriverInfo;
            }
        }

        private clsDetainedLicense _DetainedInfo = null;
        public clsDetainedLicense DetainedInfo
        {
            get
            {
                if (_DetainedInfo == null && this.LicenseID != -1)
                {
                    _GetDetainedInfo();
                }
                return _DetainedInfo;
            }
        }

        private clsLicenseClass _LicenseClassInfo = null;
        public clsLicenseClass LicenseClassInfo
        {
            get
            {
                if (_LicenseClassInfo == null && LicenseClassID != -1)
                {
                    _LicenseClassInfo = clsLicenseClass.Find(LicenseClassID);
                }
                return _LicenseClassInfo;
            }
        }
        private bool _isSubscribed = false;
        private bool _disposed = false;
        private clsLicense(int ApplicationID, int LicenseClassID, int CreatedByUserID,
            string Notes, float PaidFees, enIssueReason IssueReason)

        {
            this.LicenseID = -1;
            this.ApplicationID = ApplicationID;
            this.DriverID = -1;
            this.LicenseClassID = LicenseClassID;
            this._IssueDate = DateTime.Now;
            this._ExpirationDate = DateTime.Now;
            this.Notes = Notes;
            this.PaidFees = PaidFees;
            this.IsActive = true;
            this.IssueReason = IssueReason;
            this.CreatedByUserID = CreatedByUserID;

            _saveDispatcher = new Dictionary<enMode, Func<Task<bool>>>
            {
                { enMode.AddNew, _AddNewLicenseAsync },
                { enMode.Update, _UpdateLicenseAsync }
            };

            Mode = enMode.AddNew;

        }

        protected clsLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClass,
            DateTime IssueDate, DateTime ExpirationDate, string Notes,
            float PaidFees, bool IsActive, enIssueReason IssueReason, int CreatedByUserID)

        {
            this.LicenseID = LicenseID;
            this.ApplicationID = ApplicationID;
            this.DriverID = DriverID;
            this.LicenseClassID = LicenseClass;
            this._IssueDate = IssueDate;
            this._ExpirationDate = ExpirationDate;
            this.Notes = Notes;
            this.PaidFees = PaidFees;
            this.IsActive = IsActive;
            this.IssueReason = IssueReason;
            this.CreatedByUserID = CreatedByUserID;

            _saveDispatcher = new Dictionary<enMode, Func<Task<bool>>>
            {
                { enMode.AddNew, _AddNewLicenseAsync },
                { enMode.Update, _UpdateLicenseAsync }
            };

            Mode = enMode.Update;
        }

        protected clsLicense(clsLicense OldLicense)

        {
            this.LicenseID = OldLicense.LicenseID;
            this.ApplicationID = OldLicense.ApplicationID;
            this.DriverID = OldLicense.DriverID;
            this.LicenseClassID = OldLicense.LicenseClassID;
            this._IssueDate = OldLicense.IssueDate;
            this._ExpirationDate = OldLicense.ExpirationDate;
            this.Notes = OldLicense.Notes;
            this.PaidFees = OldLicense.PaidFees;
            this.IsActive = OldLicense.IsActive;
            this.IssueReason = OldLicense.IssueReason;
            this.CreatedByUserID = OldLicense.CreatedByUserID;

            _saveDispatcher = new Dictionary<enMode, Func<Task<bool>>>
            {
                { enMode.AddNew, _AddNewLicenseAsync },
                { enMode.Update, _UpdateLicenseAsync }
            };

            clsDetainedLicense.LicenseDetained += ClsDetainedLicense_LicenseDetained;
            clsDetainedLicense.LicenseReleased += ClsDetainedLicense_LicenseReleased;

            Mode = enMode.AddNew;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                UnSubscribeToEvents();
            }

            _disposed = true;
        }

        ~clsLicense()
        {
            Dispose(false);
        }

        private async void _GetIsDenidedInfo()
        {
            // Database query is deferred until this property is explicitly requested by the UI or other layers.
            _IsDetained = await clsDetainedLicense.IsLicenseDetainedAsync(this.LicenseID);
        }

        private async void _GetDriverInfo()
        {
            // Database query is deferred until this property is explicitly requested by the UI or other layers.
            if (_DriverInfo == null && this.DriverID != -1)
            {
                _DriverInfo = await clsDriver.FindByDriverIDAsync(this.DriverID);
            }
        }

        private async void _GetDetainedInfo()
        {
            // Database query is deferred until this property is explicitly requested by the UI or other layers.
            if (_DriverInfo == null && this.DriverID != -1)
            {
                _DetainedInfo = await clsDetainedLicense.FindByLicenseIDAsync(this.LicenseID);
            }
        }


        // TODO: Refactor date handling. 
        // Currently relying on DataAccess layer to override dates via ref parameters.
        // Need to move expiration logic (Class Validity) here to comply with Rich Domain Model.
        private async Task<bool> _AddNewLicenseAsync()
        {
            if (IssueReason != enIssueReason.FirstTime)
            {
                // To deactivate the old license upon renewal
                if (!clsLicenseData.DeactivateLicenseIDByDriverID(this.DriverID, this.LicenseClassID))
                {

                    return false;

                }
            }

            int OldLicenseID = -1;
            if (await clsApplication.GetApplicationStatusAsync(ApplicationID) != clsApplication.enApplicationStatus.New)
            {
                return false;
            }

            //call DataAccess Layer 

            if (this.IssueReason != enIssueReason.FirstTime)
            {
                OldLicenseID = this.LicenseID;
            }

            this.LicenseID = clsLicenseData.AddNewLicense(this.ApplicationID, this.DriverID, this.LicenseClassID,
               ref this._IssueDate, ref this._ExpirationDate, this.Notes, this.PaidFees,
               this.IsActive, (byte)this.IssueReason, this.CreatedByUserID);


            if (this.LicenseID != -1 && await clsApplication.SetCompleteAsync(this.ApplicationID))
            {
                if (OldLicenseID > 0)
                {
                    clsLicense.DeactivateLicense(OldLicenseID);
                }
                Mode = enMode.Update;
                return true;
            }

            return false;
        }

        private void SubscribeToEvents()
        {
            if (_isSubscribed)
                return;
            clsDetainedLicense.LicenseDetained += ClsDetainedLicense_LicenseDetained;
            clsDetainedLicense.LicenseReleased += ClsDetainedLicense_LicenseReleased;
            _isSubscribed = true;
        }

        private void UnSubscribeToEvents()
        {
            if (!_isSubscribed)
                return;
            clsDetainedLicense.LicenseDetained -= ClsDetainedLicense_LicenseDetained;
            clsDetainedLicense.LicenseReleased -= ClsDetainedLicense_LicenseReleased;
            _isSubscribed = false;
        }

        private void ClsDetainedLicense_LicenseReleased(object sender, clsDetainedLicense.LicenseReleasedEventArgs e)
        {
            if (e.LicenseID != this.LicenseID)
                return;
            OnLicenseUpdated(new LicenseUpdatedEventArgs(this.LicenseID, this.DriverID, this.ExpirationDate, this.Notes,
                this.PaidFees, this.IsActive, this.IsDetained));
        }

        private void ClsDetainedLicense_LicenseDetained(object sender, clsDetainedLicense.LicenseDetainedEventArgs e)
        {
            if (e.LicenseID != this.LicenseID)
                return;

            OnLicenseUpdated(new LicenseUpdatedEventArgs(this.LicenseID, this.DriverID, this.ExpirationDate, this.Notes,
                this.PaidFees, this.IsActive, this.IsDetained));
        }

        private async Task<bool> _UpdateLicenseAsync()
        {
            //call DataAccess Layer 

            if (clsLicenseData.UpdateLicense(this.ApplicationID, this.LicenseID, this.DriverID, this.LicenseClassID,
               this.Notes, this.PaidFees, this.IsActive, (byte)this.IssueReason,
               this.CreatedByUserID))
            {
                OnLicenseUpdated(new LicenseUpdatedEventArgs(this.LicenseID, this.DriverID, this.ExpirationDate, this.Notes,
                        this.PaidFees, this.IsActive, this.IsDetained));
                return true;
            }
            return false;
        }

        public static clsLicense Find(int LicenseID)
        {
            int ApplicationID = -1; int DriverID = -1; int LicenseClass = -1;
            DateTime IssueDate = DateTime.Now; DateTime ExpirationDate = DateTime.Now;
            string Notes = "";
            float PaidFees = 0; bool IsActive = true; int CreatedByUserID = 1;
            byte IssueReason = 1;


            if (clsLicenseData.GetLicenseInfoByID(LicenseID, ref ApplicationID, ref DriverID, ref LicenseClass,
            ref IssueDate, ref ExpirationDate, ref Notes,
            ref PaidFees, ref IsActive, ref IssueReason, ref CreatedByUserID))
            {
                if (ExpirationDate < clsUtilData.GetServerDate())
                {
                    IsActive = false;
                }
                clsLicense license = new clsLicense(LicenseID, ApplicationID, DriverID, LicenseClass,
                                 IssueDate, ExpirationDate, Notes,
                                 PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
                license.SubscribeToEvents();
                return license;
            }

            else
                return null;

        }

        public static clsLicense FindByApplicationID(int ApplicationID)

        {
            int LicenseID = -1; int DriverID = -1; int LicenseClass = -1;
            DateTime IssueDate = DateTime.Now; DateTime ExpirationDate = DateTime.Now;
            string Notes = "";
            float PaidFees = 0; bool IsActive = true; int CreatedByUserID = 1;
            byte IssueReason = 1;


            if (clsLicenseData.GetLicenseInfoByApplicationID(ApplicationID, ref LicenseID, ref DriverID, ref LicenseClass,
            ref IssueDate, ref ExpirationDate, ref Notes,
            ref PaidFees, ref IsActive, ref IssueReason, ref CreatedByUserID))
            {
                if (ExpirationDate < clsUtilData.GetServerDate())
                {
                    IsActive = false;
                }

                clsLicense license = new clsLicense(LicenseID, ApplicationID, DriverID, LicenseClass,
                                  IssueDate, ExpirationDate, Notes,
                                  PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
                license.SubscribeToEvents();

                return license;
            }

            else
                return null;

        }

        public static DataTable GetAllLicenses()
        {
            return clsLicenseData.GetAllLicenses();

        }

        private async Task<bool> SaveAsync()
        {

            return await _saveDispatcher[this.Mode]();
        }

        public static string GetIssueReasonText(enIssueReason IssueReason)
        {

            switch (IssueReason)
            {
                case enIssueReason.FirstTime:
                    return "First Time";
                case enIssueReason.Renew:
                    return "Renew";
                case enIssueReason.DamagedReplacement:
                    return "Replacement for Damaged";
                case enIssueReason.LostReplacement:
                    return "Replacement for Lost";
                default:
                    return "First Time";
            }
        }

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {

            return clsLicenseData.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID);

        }

        public static int GetActiveLicenseIDByDriverID(int DriverID, int LicenseClassID)
        {
            return clsLicenseData.GetActiveLicenseIDByDriverID(DriverID, LicenseClassID);
        }

        public static int GetLicenseIDByApplicationID(int ApplicationID)
        {
            return clsLicenseData.GetLicenseIDByApplicationID(ApplicationID);
        }

        public Boolean IsLicensExpired()
        {
            return this.ExpirationDate < clsUtilData.GetServerDate();
        }

        protected virtual void OnLicenseUpdated(LicenseUpdatedEventArgs e)
        {
            LicenseUpdated?.Invoke(this, e);
        }

        private static async Task<float> _CalculatePaidFeesAsync(clsApplication.enApplicationType ApplicationType, int LicenseClassID)
        { 
            if(ApplicationType == clsApplication.enApplicationType.ReplaceLostDrivingLicense
                || ApplicationType == clsApplication.enApplicationType.ReplaceDamagedDrivingLicense)
            {
                return (float)(await clsApplicationType.FindAsync((int)ApplicationType)).ApplicationTypeFees;
            }
            return clsLicenseClass.Find(LicenseClassID).ClassFees + (float)(await clsApplicationType.FindAsync((int)ApplicationType)).ApplicationTypeFees;
        }

        private static async Task<int?> _CreateNewApplicationIDAsync(int CreatedByUserID, int? PersonID, clsApplication.enApplicationType ApplicationType)
        {
            if (!PersonID.HasValue || PersonID <= 0)
                return null;

            clsApplication application = await clsApplication.GetNewApplicationAsync(CreatedByUserID, PersonID, ApplicationType);
            if(application != null)
            {
                return application.ApplicationID;
            }
            return -1;
        }

        public static async Task<bool> IsValidAgeAsync(int PersonID, int LicenseClassID)
        {
            clsPerson person = await clsPerson.FindAsync(PersonID);

            DateTime DateOfBirth = DateTime.Today;
            DateTime today = clsBusinessSettings.GetServerDateTime();

            int years = 0;

            if (person != null)
            {
                DateOfBirth = person.DateOfBirth;
                years = today.Year - DateOfBirth.Year;
                if (years < clsLicenseClass.Find(LicenseClassID)?.MinimumAllowedAge)
                {
                    return false;
                }
                return true;
}
            else
            {
                return false;
            }
        }

        private static async Task<clsLicense> _PrepareNewLicenseAsync(int LocalDrivingLicenseApplicationID, int CreatedByUserID, string Notes)
        {
            if (LocalDrivingLicenseApplicationID < 0)
            {
                return null;
            }

            clsLicense NewLicense = null;
            clsLocalDrivingLicenseApplication localDrivingLicenseApplication
                = await clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseIDAsync(LocalDrivingLicenseApplicationID);

            if (localDrivingLicenseApplication == null)
            {
                return null;
            }

            if (!await IsValidAgeAsync(localDrivingLicenseApplication.ApplicantPersonID, localDrivingLicenseApplication.LicenseClassID))
            {
                return null;
            }

            if (await localDrivingLicenseApplication.HasLicenseAsync())
            {
                return null;
            }

            if (clsLicense.GetActiveLicenseIDByPersonID(localDrivingLicenseApplication.ApplicantPersonID, localDrivingLicenseApplication.LicenseClassID) != -1)
            {
                return null;
            }

            if (!await localDrivingLicenseApplication.DoesPassAllTestsAsync())
            {
                return null;
            }


            float PaidFees = await _CalculatePaidFeesAsync(clsApplication.enApplicationType.NewDrivingLicense, localDrivingLicenseApplication.LicenseClassID);
            NewLicense = new clsLicense(localDrivingLicenseApplication.ApplicationID, localDrivingLicenseApplication.LicenseClassID,
                CreatedByUserID, Notes, PaidFees, enIssueReason.FirstTime);

            if ((NewLicense._DriverInfo = await clsDriver.FindByPersonIDAsync(localDrivingLicenseApplication.ApplicantPersonID)) == null)
            {
                NewLicense._DriverInfo = await clsDriver.CreateNewDriverAsync(localDrivingLicenseApplication.ApplicantPersonID, CreatedByUserID);
            }

            NewLicense.DriverID = NewLicense._DriverInfo.DriverID;
            NewLicense.IsActive = true;

            return (NewLicense._DriverInfo != null) ? NewLicense : null;
        }
       
        private static async Task<clsLicense> _PrepareRenewLicenseAsync(int LicenseID, string Notes, int CreatedByUserID)
        {
            clsLicense OldLicense = Find(LicenseID);
            clsLicense NewLicense = null;

            // TODO: Get License ID if it's active or not for renewal
            if (OldLicense != null)
            {
                DateTime ServerDate = clsUtilData.GetServerDate();

                if (ServerDate < OldLicense.ExpirationDate.AddMonths(-3) || ServerDate > OldLicense.ExpirationDate.AddMonths(3))
                {
                    return null;
                }

                if(clsDriver.GetLastLicenseID(OldLicense.DriverID, OldLicense.LicenseClassID) != LicenseID)
                {
                    return null;
                }

                if (!IsLicenseActive(OldLicense.LicenseID) && ServerDate < OldLicense.ExpirationDate)
                {
                    return null;
                }

                if (await clsDetainedLicense.IsLicenseDetainedAsync(LicenseID))
                {
                    return null;
                }

                NewLicense = new clsLicense(OldLicense);
                NewLicense.IssueReason = enIssueReason.Renew;
                int? ApplicationID = await _CreateNewApplicationIDAsync(CreatedByUserID,
                    NewLicense.DriverInfo.PersonID, clsApplication.enApplicationType.RenewDrivingLicense);
                NewLicense.ApplicationID = ApplicationID.HasValue ? (int)ApplicationID : -1;
                NewLicense.CreatedByUserID = CreatedByUserID;
                NewLicense.Notes = Notes;
                NewLicense.PaidFees = await _CalculatePaidFeesAsync(clsApplication.enApplicationType.RenewDrivingLicense
                    , NewLicense.LicenseClassID);
                NewLicense.IsActive = true;
                OldLicense.IsActive = false;
            }
            return NewLicense;
        }

        private static async Task<clsLicense> _PrepareReplacementLicenseAsync(int LicenseID, int CreatedByUserID, string Notes, enIssueReason IssueReason)
        {
            if (IssueReason != enIssueReason.DamagedReplacement && IssueReason != enIssueReason.LostReplacement)
            {
                return null;
            }

            clsLicense OldLicense = clsLicense.Find(LicenseID);
            clsLicense NewLicense = null;

            if (OldLicense == null || !IsLicenseActive(OldLicense.LicenseID))
            {
                return null;
            }

            if (clsUtilData.GetServerDate() > OldLicense.ExpirationDate)
            {
                return null;
            }

            NewLicense = new clsLicense(OldLicense);
            clsApplication.enApplicationType applicationType = (IssueReason == enIssueReason.DamagedReplacement) ?
                clsApplication.enApplicationType.ReplaceDamagedDrivingLicense :
                clsApplication.enApplicationType.ReplaceLostDrivingLicense;


            NewLicense.PaidFees = await _CalculatePaidFeesAsync(applicationType, NewLicense.LicenseClassID);
            NewLicense.IssueReason = IssueReason;
            int? ApplicationID = await _CreateNewApplicationIDAsync(CreatedByUserID,
                NewLicense.DriverInfo.PersonID, applicationType);
            NewLicense.ApplicationID = ApplicationID.HasValue ? (int)ApplicationID : -1;
            NewLicense.CreatedByUserID = CreatedByUserID;
            NewLicense.Notes = Notes;
            NewLicense.IsActive = true;

            OldLicense.IsActive = false;


            return NewLicense;
        }

        // TODO: Technical Debt - Implement a dedicated Service Layer (Business Manager) to handle
        // license state transitions and logic validation, decoupling business rules from DataAccess 
        // and moving towards a Unit of Work pattern for atomic database operations.
        
        public static bool IsLicenseActive(int LicenseID)
        {
            return clsLicenseData.IsLicenseActive(LicenseID);
        }

        public static bool DeactivateLicense(int LicenseID)
        {
            return clsLicenseData.DeactivateLicense(LicenseID);
        }

        public static bool DeactivateExpiredLicenses()
        {
            return clsLicenseData.DeactivateExpiredLicenses();
        }

        public async Task<clsLicense> RenewAsync(string Notes, int CreatedByUserID)
        {
            clsLicense license = await _PrepareRenewLicenseAsync(this.LicenseID, Notes, CreatedByUserID);

            if (license != null)
            {
                if (await license.SaveAsync())
                {
                    OnLicenseUpdated(new LicenseUpdatedEventArgs(this.LicenseID, this.DriverID, this.ExpirationDate, this.Notes,
                                    this.PaidFees, this.IsActive, this.IsDetained));
                    return license;
                }
                return null;
            }
            return null;
        }

        internal static async Task<clsLicense> IssueFirstTimeLocalLicenseAsync(int LocalDrivingLicenseApplicationID, int CreatedByUserID, string Notes)
        {
            clsLicense license = await _PrepareNewLicenseAsync(LocalDrivingLicenseApplicationID, CreatedByUserID, Notes);

            if (license != null)
            {
                if (await license.SaveAsync())
                {
                    license.SubscribeToEvents();
                    return license;
                }
                return null;
            }
            return null;
        }

        public async Task<clsLicense> ReplaceAsync(int CreatedByUserID, enIssueReason IssueReason)
        {
            clsLicense license = await _PrepareReplacementLicenseAsync(this.LicenseID, CreatedByUserID, Notes, IssueReason);
            if (license != null)
            {
                if (await license.SaveAsync())
                {
                    OnLicenseUpdated(new LicenseUpdatedEventArgs(this.LicenseID, this.DriverID, this.ExpirationDate, this.Notes,
                this.PaidFees, this.IsActive, this.IsDetained));
                    return license;
                }
            }
            return null;
        }

        public async Task<clsDetainedLicense> DetainAsync(float FineFees, int CreatedByUserID)
        {
            return await clsDetainedLicense.DetainedLicenseAsync(this.LicenseID, FineFees, CreatedByUserID);
        }

        public async Task<bool> ReleaseAsync(int ReleasedByUserID)
        {
            clsDetainedLicense DetainedLicense = await clsDetainedLicense.ReleaseDetainedLicenseAsync(this.LicenseID, ReleasedByUserID);
            if(DetainedLicense != null)
            {
                this._DetainedInfo = DetainedLicense;
                return true;
            }
            return false;
        }


    }

}
