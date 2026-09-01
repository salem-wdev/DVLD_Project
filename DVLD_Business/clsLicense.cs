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
                    _GetLicenseClassInfo();
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
            _IsDetained = await clsDetainedLicense.IsLicenseDetainedAsync(this.LicenseID).ConfigureAwait(false);
        }

        private async void _GetDriverInfo()
        {
            // Database query is deferred until this property is explicitly requested by the UI or other layers.
            if (_DriverInfo == null && this.DriverID != -1)
            {
                _DriverInfo = await clsDriver.FindByDriverIDAsync(this.DriverID).ConfigureAwait(false);
            }
        }

        private async void _GetDetainedInfo()
        {
            // Database query is deferred until this property is explicitly requested by the UI or other layers.
            if (_DriverInfo == null && this.DriverID != -1)
            {
                _DetainedInfo = await clsDetainedLicense.FindByLicenseIDAsync(this.LicenseID).ConfigureAwait(false);
            }
        }

        private async void _GetLicenseClassInfo()
        {
            // Database query is deferred until this property is explicitly requested by the UI or other layers.
            _LicenseClassInfo = await clsLicenseClass.FindAsync(LicenseClassID).ConfigureAwait(false);
        }


        // TODO: Refactor date handling. 
        // Currently relying on DataAccess layer to override dates via ref parameters.
        // Need to move expiration logic (Class Validity) here to comply with Rich Domain Model.
        private async Task<bool> _AddNewLicenseAsync()
        {
            if (IssueReason != enIssueReason.FirstTime)
            {
                // To deactivate the old license upon renewal
                if (!await clsLicenseData.DeactivateLicenseIDByDriverIDAsync(this.DriverID, this.LicenseClassID).ConfigureAwait(false))
                {

                    return false;

                }
            }

            int OldLicenseID = -1;
            if (await clsApplication.GetApplicationStatusAsync(ApplicationID).ConfigureAwait(false) != clsApplication.enApplicationStatus.New)
            {
                return false;
            }

            //call DataAccess Layer 

            if (this.IssueReason != enIssueReason.FirstTime)
            {
                OldLicenseID = this.LicenseID;
            }

            var result = await clsLicenseData.AddNewLicenseAsync(this.ApplicationID, this.DriverID, this.LicenseClassID, this.Notes, this.PaidFees,
               this.IsActive, (byte)this.IssueReason, this.CreatedByUserID).ConfigureAwait(false);

            this.LicenseID = result.LicenseID;
            this._IssueDate = result.IssueDate;
            this._ExpirationDate = result.ExpirationDate;

            if (this.LicenseID != -1 && await clsApplication.SetCompleteAsync(this.ApplicationID).ConfigureAwait(false))
            {
                if (OldLicenseID > 0)
                {
                    await clsLicense.DeactivateLicenseAsync(OldLicenseID).ConfigureAwait(false);
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

            if (await clsLicenseData.UpdateLicenseAsync(this.ApplicationID, this.LicenseID, this.DriverID, this.LicenseClassID,
               this.Notes, this.PaidFees, this.IsActive, (byte)this.IssueReason,
               this.CreatedByUserID).ConfigureAwait(false))
            {
                OnLicenseUpdated(new LicenseUpdatedEventArgs(this.LicenseID, this.DriverID, this.ExpirationDate, this.Notes,
                        this.PaidFees, this.IsActive, this.IsDetained));
                return true;
            }
            return false;
        }

        public static async Task<clsLicense> FindAsync(int LicenseID)
        {
           
            var result = await clsLicenseData.GetLicenseInfoByIDAsync(LicenseID).ConfigureAwait(false);

            if(result.IsFound)
            {
                if (result.ExpirationDate < clsUtilData.GetServerDate())
                {
                    result.IsActive = false;
                }
                clsLicense license = new clsLicense(LicenseID, result.ApplicationID, result.DriverID, result.LicenseClass,
                                 result.IssueDate, result.ExpirationDate, result.Notes,
                                 result.PaidFees, result.IsActive, (enIssueReason)result.IssueReason, result.CreatedByUserID);
                license.SubscribeToEvents();
                return license;
            }

            else
                return null;

        }

        public static async Task<clsLicense> FindByApplicationIDAsync(int ApplicationID)
        {
            
            var result = await clsLicenseData.GetLicenseInfoByApplicationIDAsync(ApplicationID).ConfigureAwait(false);
            
            if(result.IsFound)
            {
                if (result.ExpirationDate < clsUtilData.GetServerDate())
                {
                    result.IsActive = false;
                }

                clsLicense license = new clsLicense(result.LicenseID, ApplicationID, result.DriverID, result.LicenseClass,
                                  result.IssueDate, result.ExpirationDate, result.Notes,
                                  result.PaidFees, result.IsActive, (enIssueReason)result.IssueReason, result.CreatedByUserID);
                license.SubscribeToEvents();

                return license;
            }

            else
                return null;

        }

        public static async Task<DataTable> GetAllLicensesAsync()
        {
            return await clsLicenseData.GetAllLicensesAsync().ConfigureAwait(false);

        }

        private async Task<bool> SaveAsync()
        {

            return await _saveDispatcher[this.Mode]().ConfigureAwait(false);
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

        public static async Task<int> GetActiveLicenseIDByPersonIDAsync(int PersonID, int LicenseClassID)
        {

            return await clsLicenseData.GetActiveLicenseIDByPersonIDAsync(PersonID, LicenseClassID).ConfigureAwait(false);

        }

        public static async Task<int> GetActiveLicenseIDByDriverIDAsync(int DriverID, int LicenseClassID)
        {
            return await clsLicenseData.GetActiveLicenseIDByDriverIDAsync(DriverID, LicenseClassID).ConfigureAwait(false);
        }

        public static async Task<int> GetLicenseIDByApplicationIDAsync(int ApplicationID)
        {
            return await clsLicenseData.GetLicenseIDByApplicationIDAsync(ApplicationID).ConfigureAwait(false);
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
                return (float)(await clsApplicationType.FindAsync((int)ApplicationType).ConfigureAwait(false)).ApplicationTypeFees;
            }
            return (await clsLicenseClass.FindAsync(LicenseClassID).ConfigureAwait(false)).ClassFees + (float)(await clsApplicationType.FindAsync((int)ApplicationType).ConfigureAwait(false)).ApplicationTypeFees;
        }

        private static async Task<int?> _CreateNewApplicationIDAsync(int CreatedByUserID, int? PersonID, clsApplication.enApplicationType ApplicationType)
        {
            if (!PersonID.HasValue || PersonID <= 0)
                return null;

            clsApplication application = await clsApplication.GetNewApplicationAsync(CreatedByUserID, PersonID, ApplicationType).ConfigureAwait(false);
            if(application != null)
            {
                return application.ApplicationID;
            }
            return -1;
        }

        public static async Task<bool> IsValidAgeAsync(int PersonID, int LicenseClassID)
        {
            clsPerson person = await clsPerson.FindAsync(PersonID).ConfigureAwait(false);

            DateTime DateOfBirth = DateTime.Today;
            DateTime today = clsBusinessSettings.GetServerDateTime();

            int years = 0;

            if (person != null)
            {
                DateOfBirth = person.DateOfBirth;
                years = today.Year - DateOfBirth.Year;
                if (years < (await clsLicenseClass.FindAsync(LicenseClassID).ConfigureAwait(false))?.MinimumAllowedAge)
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
                = await clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseIDAsync(LocalDrivingLicenseApplicationID).ConfigureAwait(false);

            if (localDrivingLicenseApplication == null)
            {
                return null;
            }

            if (!await IsValidAgeAsync(localDrivingLicenseApplication.ApplicantPersonID, localDrivingLicenseApplication.LicenseClassID).ConfigureAwait(false))
            {
                return null;
            }

            if (await localDrivingLicenseApplication.HasLicenseAsync().ConfigureAwait(false))
            {
                return null;
            }

            if (await clsLicense.GetActiveLicenseIDByPersonIDAsync(localDrivingLicenseApplication.ApplicantPersonID, localDrivingLicenseApplication.LicenseClassID).ConfigureAwait(false) != -1)
            {
                return null;
            }

            if (!await localDrivingLicenseApplication.DoesPassAllTestsAsync().ConfigureAwait(false))
            {
                return null;
            }


            float PaidFees = await _CalculatePaidFeesAsync(clsApplication.enApplicationType.NewDrivingLicense, localDrivingLicenseApplication.LicenseClassID).ConfigureAwait(false);
            NewLicense = new clsLicense(localDrivingLicenseApplication.ApplicationID, localDrivingLicenseApplication.LicenseClassID,
                CreatedByUserID, Notes, PaidFees, enIssueReason.FirstTime);

            if ((NewLicense._DriverInfo = await clsDriver.FindByPersonIDAsync(localDrivingLicenseApplication.ApplicantPersonID).ConfigureAwait(false)) == null)
            {
                NewLicense._DriverInfo = await clsDriver.CreateNewDriverAsync(localDrivingLicenseApplication.ApplicantPersonID, CreatedByUserID).ConfigureAwait(false);
            }

            NewLicense.DriverID = NewLicense._DriverInfo.DriverID;
            NewLicense.IsActive = true;

            return (NewLicense._DriverInfo != null) ? NewLicense : null;
        }
       
        private static async Task<clsLicense> _PrepareRenewLicenseAsync(int LicenseID, string Notes, int CreatedByUserID)
        {
            clsLicense OldLicense = await FindAsync(LicenseID).ConfigureAwait(false);
            clsLicense NewLicense = null;

            // TODO: Get License ID if it's active or not for renewal
            if (OldLicense != null)
            {
                DateTime ServerDate = clsUtilData.GetServerDate();

                if (ServerDate < OldLicense.ExpirationDate.AddMonths(-3) || ServerDate > OldLicense.ExpirationDate.AddMonths(3))
                {
                    return null;
                }

                if(await clsDriver.GetLastLicenseIDAsync(OldLicense.DriverID, OldLicense.LicenseClassID).ConfigureAwait(false) != LicenseID)
                {
                    return null;
                }

                if (!await IsLicenseActiveAsync(OldLicense.LicenseID).ConfigureAwait(false) && ServerDate < OldLicense.ExpirationDate)
                {
                    return null;
                }

                if (await clsDetainedLicense.IsLicenseDetainedAsync(LicenseID).ConfigureAwait(false))
                {
                    return null;
                }

                NewLicense = new clsLicense(OldLicense);
                NewLicense.IssueReason = enIssueReason.Renew;
                int? ApplicationID = await _CreateNewApplicationIDAsync(CreatedByUserID,
                    NewLicense.DriverInfo.PersonID, clsApplication.enApplicationType.RenewDrivingLicense).ConfigureAwait(false);
                NewLicense.ApplicationID = ApplicationID.HasValue ? (int)ApplicationID : -1;
                NewLicense.CreatedByUserID = CreatedByUserID;
                NewLicense.Notes = Notes;
                NewLicense.PaidFees = await _CalculatePaidFeesAsync(clsApplication.enApplicationType.RenewDrivingLicense
                    , NewLicense.LicenseClassID).ConfigureAwait(false);
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

            clsLicense OldLicense = await clsLicense.FindAsync(LicenseID).ConfigureAwait(false);
            clsLicense NewLicense = null;

            if (OldLicense == null || !await IsLicenseActiveAsync(OldLicense.LicenseID).ConfigureAwait(false))
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


            NewLicense.PaidFees = await _CalculatePaidFeesAsync(applicationType, NewLicense.LicenseClassID).ConfigureAwait(false);
            NewLicense.IssueReason = IssueReason;
            int? ApplicationID = await _CreateNewApplicationIDAsync(CreatedByUserID,
                NewLicense.DriverInfo.PersonID, applicationType).ConfigureAwait(false);
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
        
        public static async Task<bool> IsLicenseActiveAsync(int LicenseID)
        {
            return await clsLicenseData.IsLicenseActiveAsync(LicenseID).ConfigureAwait(false);
        }
        
        public static async Task<bool> DeactivateLicenseAsync(int LicenseID)
        {
            return await clsLicenseData.DeactivateLicenseAsync(LicenseID).ConfigureAwait(false);
        }

        public static async Task<bool> DeactivateExpiredLicensesAsync()
        {
            return await clsLicenseData.DeactivateExpiredLicensesAsync().ConfigureAwait(false);
        }

        public async Task<clsLicense> RenewAsync(string Notes, int CreatedByUserID)
        {
            clsLicense license = await _PrepareRenewLicenseAsync(this.LicenseID, Notes, CreatedByUserID).ConfigureAwait(false);

            if (license != null)
            {
                if (await license.SaveAsync().ConfigureAwait(false))
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
            clsLicense license = await _PrepareNewLicenseAsync(LocalDrivingLicenseApplicationID, CreatedByUserID, Notes).ConfigureAwait(false);

            if (license != null)
            {
                if (await license.SaveAsync().ConfigureAwait(false))
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
            clsLicense license = await _PrepareReplacementLicenseAsync(this.LicenseID, CreatedByUserID, Notes, IssueReason).ConfigureAwait(false);
            if (license != null)
            {
                if (await license.SaveAsync().ConfigureAwait(false))
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
            return await clsDetainedLicense.DetainedLicenseAsync(this.LicenseID, FineFees, CreatedByUserID).ConfigureAwait(false);
        }

        public async Task<bool> ReleaseAsync(int ReleasedByUserID)
        {
            clsDetainedLicense DetainedLicense = await clsDetainedLicense.ReleaseDetainedLicenseAsync(this.LicenseID, ReleasedByUserID).ConfigureAwait(false);
            if(DetainedLicense != null)
            {
                this._DetainedInfo = DetainedLicense;
                return true;
            }
            return false;
        }


    }

}
