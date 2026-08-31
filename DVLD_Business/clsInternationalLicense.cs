using DVLD_Business;
using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DVLD_Business
{
    public class clsInternationalLicense:clsApplication
    {
        public sealed class InternationalLicenseUpdatedEventArgs : EventArgs
        {
            public int InternationalLicenseID { get; }
            public int DriverID { get; }
            public DateTime ExpirationDate { get; }
            public bool IsActive { get; }

            public enApplicationStatus ApplicationStatus;
            public DateTime LastStatusDate;


            public InternationalLicenseUpdatedEventArgs(int InternationalLicenseID, int DriverID, DateTime ExpirationDate
                , bool IsActive, enApplicationStatus ApplicationStatus, DateTime LastStatusDate)
            {
                this.InternationalLicenseID = InternationalLicenseID;
                this.DriverID = DriverID;
                this.ExpirationDate = ExpirationDate;
                this.IsActive = IsActive;
                this.ApplicationStatus = ApplicationStatus;
                this.LastStatusDate = LastStatusDate;
            }


        }
        public event EventHandler<InternationalLicenseUpdatedEventArgs> InternationalLicenseUpdated;


        public new enum enMode { AddNew = 0, Update = 1 };
        public new enMode Mode { get; protected set; } = enMode.AddNew;
        private new readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        // Holds the cached driver information; backing field for the lazy-loaded DriverInfo property.
        private clsDriver _DriverInfo = null;

        public clsDriver DriverInfo
        {
            get
            {
                // Database query is deferred until this property is explicitly requested by the UI or other layers.
                if (_DriverInfo == null && this.DriverID != -1)
                {
                    _GetDriverInfo();
                }
                return _DriverInfo;
            }
        }

        public int InternationalLicenseID {  get; private set; }  
        public int DriverID { get; protected set; }
        public int IssuedUsingLocalLicenseID { get; protected set; }
        public DateTime IssueDate { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public bool IsActive { get; internal set; }
       

        private clsInternationalLicense(int DriverID, int IssuedUsingLocalLicenseID,
            DateTime IssueDate, DateTime ExpirationDate, bool IsActive, 
            clsApplication Application)
            : base(Application)

        {
            //here we set the applicaiton type to New International License.
            this.ApplicationTypeID = clsApplication.enApplicationType.NewInternationalLicense;
            
            this.InternationalLicenseID = -1;
            this.DriverID = DriverID;
            this.IssuedUsingLocalLicenseID = IssuedUsingLocalLicenseID;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
           
            this.IsActive = IsActive;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>()
            {
                [enMode.AddNew] = async () =>
                {
                    base.Mode = clsApplication.enMode.AddNew;
                    return await base._AddNewApplicationAsync() && await this._AddNewInternationalLicenseAsync();
                },

                [enMode.Update] = async () =>
                {
                    base.Mode = clsApplication.enMode.Update;
                    return await base._UpdateApplicationAsync() && await this._UpdateInternationalLicenseAsync();
                }
            };

            Mode = enMode.AddNew;

        }

        protected clsInternationalLicense(int InternationalLicenseID,  int DriverID,
            int IssuedUsingLocalLicenseID,
            DateTime IssueDate, DateTime ExpirationDate,bool IsActive, clsApplication Application)
            : base(Application)
        {
           
            this.InternationalLicenseID = InternationalLicenseID;
            this.DriverID = DriverID;
            this.IssuedUsingLocalLicenseID = IssuedUsingLocalLicenseID;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.IsActive = IsActive;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>()
            {
                [enMode.AddNew] = async () =>
                {
                    base.Mode = clsApplication.enMode.AddNew;
                    return await base._AddNewApplicationAsync() && await this._AddNewInternationalLicenseAsync();
                },

                [enMode.Update] = async () =>
                {
                    base.Mode = clsApplication.enMode.Update;
                    return await base._UpdateApplicationAsync() && await this._UpdateInternationalLicenseAsync();
                }
            }; 
            
            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewInternationalLicenseAsync()
        {

           
            //call DataAccess Layer 

            this.InternationalLicenseID = 
               await clsInternationalLicenseData.AddNewInternationalLicenseAsync(this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID);


            if (this.InternationalLicenseID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            clsApplication.DeleteAsync(this.ApplicationID);
            return false;
        }

        private async Task<bool> _UpdateInternationalLicenseAsync()
        {
            //call DataAccess Layer 

            clsLicense license = await clsLicense.FindAsync(IssuedUsingLocalLicenseID);

            if (license == null || license.ExpirationDate < clsUtilData.GetServerDate()
                || license.IsActive == false
                || await clsDetainedLicense.IsLicenseDetainedAsync(IssuedUsingLocalLicenseID))
            {
                IsActive = false;
                return false;
            }

            if (await clsInternationalLicenseData.UpdateInternationalLicenseAsync(
                this.InternationalLicenseID,this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID))
            {
                OnInternationalLicenseUpdated(new InternationalLicenseUpdatedEventArgs(InternationalLicenseID, DriverID
                    , ExpirationDate, IsActive, ApplicationStatus, LastStatusDate));
            }
            return true;
        }

        public static async Task<clsInternationalLicense> FindByInternationalLicenseIDAsync(int InternationalLicenseID)
        {
            if (InternationalLicenseID <= 0) return null; // To prevent unnessasry database connection.

            int ApplicationID = -1;
            int DriverID = -1; int IssuedUsingLocalLicenseID = -1;
            DateTime IssueDate = DateTime.Now; DateTime ExpirationDate = DateTime.Now;
             bool IsActive = true; int CreatedByUserID = 1;

           var result = await clsInternationalLicenseData.GetInternationalLicenseInfoByIDAsync(InternationalLicenseID);
            if (result.IsFound)
            {
                if (ExpirationDate < clsUtilData.GetServerDate())
                {
                    IsActive = false;
                }

                //now we find the base application
                clsApplication Application = await clsApplication.FindAsync(ApplicationID);


                return new clsInternationalLicense(InternationalLicenseID, DriverID,
                    IssuedUsingLocalLicenseID, IssueDate, ExpirationDate,
                    IsActive, Application);

            }
             
            else
                return null;

        }

        public static async Task<DataTable> GetAllInternationalLicensesAsync()
        {
            return await clsInternationalLicenseData.GetAllInternationalLicensesAsync();

        }

        public override async Task<bool> SaveAsync()
        {

            if(await GetApplicationStatusAsync(this.ApplicationID)!= enApplicationStatus.New)
            {
                return false;
            }

            return await _saveDictionary[this.Mode]();
        }

        public static async Task<int> GetActiveInternationalLicenseIDByDriverIDAsync(int DriverID)
        {
            if (DriverID <= 0) return -1; // To prevent unnessasry database connection.

            return await clsInternationalLicenseData.GetActiveInternationalLicenseIDByDriverIDAsync(DriverID);

        }

        public static async Task<DataTable> GetDriverInternationalLicensesAsync(int DriverID)
        {
            if (DriverID <= 0) return null; // To prevent unnessasry database connection.

            return await clsInternationalLicenseData.GetDriverInternationalLicensesAsync(DriverID);
        }

        private static async Task<(bool IsSucceeded, int LocalLicenseID)> _IsDriverEligibleForInternationalLicenseAsync(int DriverID, int LocalLicenseID)
        {
            LocalLicenseID = -1;
            if (DriverID <= 0) return (false, LocalLicenseID); // To prevent unnessasry database connection.

            LocalLicenseID = await clsLicense.GetActiveLicenseIDByDriverIDAsync(DriverID, 3);
            if (LocalLicenseID == -1)
            {
                return (false, LocalLicenseID);
            }

            if (await clsDetainedLicense.IsLicenseDetainedAsync(LocalLicenseID))
            {
                return (false, LocalLicenseID);
            }

            int InternationalLicenseID = await clsInternationalLicense.GetActiveInternationalLicenseIDByDriverIDAsync(DriverID);
            if (InternationalLicenseID != -1)
            {
                return (false, LocalLicenseID);
            }

            return (true, LocalLicenseID);
        }

        public static async Task<bool> IsDriverEligibleForInternationalLicenseAsync(int DriverID)
        {
            if (DriverID <= 0) return false; // To prevent unnessasry database connection.

            
            var result = await _IsDriverEligibleForInternationalLicenseAsync(DriverID, -1);
            return result.IsSucceeded;
        }

        public static async Task<(bool IsSucceeded, int LocalLicenseID)> IsDriverEligibleForInternationalLicenseAsync(int DriverID, int LocalLicenseID)
        {
            LocalLicenseID = -1;
            if (DriverID <= 0) return (false, LocalLicenseID); // To prevent unnessasry database connection.

            return await _IsDriverEligibleForInternationalLicenseAsync(DriverID, LocalLicenseID);
        }

        private static async Task<clsInternationalLicense> _GetNewInternationalLicenseAsync(int DriverID, int CreatedByUser)
        {

            clsInternationalLicense InternationalLicense = null;

            int LocalLicenseID = -1;
            if (DriverID <= 0) return null; // To prevent unnessasry database connection.

            var result = await IsDriverEligibleForInternationalLicenseAsync(DriverID, LocalLicenseID);
            if (!result.IsSucceeded)
            {
                return null;
            }

            clsLicense LocalLicense = await clsLicense.FindAsync(LocalLicenseID);

            if(LocalLicense == null)
            {
                return null;
            }

            DateTime IssueDate = clsBusinessSettings.GetServerDateTime();

            clsApplication application = await GetNewApplicationobjectAsync(CreatedByUser,
                LocalLicense.DriverInfo.PersonID, enApplicationType.NewInternationalLicense);

            if(application == null)
            {
                return null;
            }

            if (!await application.SaveAsync())
            {
                return null;
            }

            InternationalLicense = new clsInternationalLicense(DriverID, 
                LocalLicenseID, IssueDate, IssueDate.AddYears(1), true, application);

            return InternationalLicense;
        }

        public static async Task<clsInternationalLicense> IssueNewInternationalLicenseAsync(int DriverID, int CreatedByUser)
        {
            if (DriverID <= 0) return null; // To prevent unnessasry database connection.

            clsInternationalLicense InternationalLicense = await _GetNewInternationalLicenseAsync(DriverID, CreatedByUser);
            if (InternationalLicense != null)
            {
                if (await InternationalLicense.SaveAsync())
                {
                    return InternationalLicense;
                }
            }
            return null;
        }

        public static async Task<bool> DeactvateExpiredLicensesAsync()
        {
            return await clsInternationalLicenseData.DeactvateInternationalLicensesforExpiredLocalLicensesAsync();
        }

        protected virtual void OnInternationalLicenseUpdated(InternationalLicenseUpdatedEventArgs e)
        {
            InternationalLicenseUpdated?.Invoke(this, e);
        }

        private async void _GetDriverInfo()
        {
            // Database query is deferred until this property is explicitly requested by the UI or other layers.
            if (_DriverInfo == null && this.DriverID != -1)
            {
                _DriverInfo = await clsDriver.FindByDriverIDAsync(this.DriverID);
            }
        }
    }
}
