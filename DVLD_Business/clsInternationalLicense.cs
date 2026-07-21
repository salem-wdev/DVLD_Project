using DVLD_Business;
using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;

namespace DVLD_Business
{
    public class clsInternationalLicense:clsApplication
    {

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { get; protected set; } = enMode.AddNew;
        private readonly Dictionary<enMode, Func<bool>> _saveDictionary;

        // Holds the cached driver information; backing field for the lazy-loaded DriverInfo property.
        private clsDriver _DriverInfo = null;

        public clsDriver DriverInfo
        {
            get
            {
                // Database query is deferred until this property is explicitly requested by the UI or other layers.
                if (_DriverInfo == null && this.DriverID != -1)
                {
                    _DriverInfo = clsDriver.FindByDriverID(this.DriverID);
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

            _saveDictionary = new Dictionary<enMode, Func<bool>>()
            {
                [enMode.AddNew] = () => base._AddNewApplication() && this._AddNewInternationalLicense(),
                [enMode.Update] = () => base._UpdateApplication() && this._UpdateInternationalLicense()
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
            _saveDictionary = new Dictionary<enMode, Func<bool>>()
            {
                [enMode.AddNew] = () => base._AddNewApplication() && this._AddNewInternationalLicense(),
                [enMode.Update] = () => base._UpdateApplication() && this._UpdateInternationalLicense()
            };
            Mode = enMode.Update;
        }

        private bool _AddNewInternationalLicense()
        {

           
            //call DataAccess Layer 

            this.InternationalLicenseID = 
                clsInternationalLicenseData.AddNewInternationalLicense(this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID);


            if (this.InternationalLicenseID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            clsApplication.Delete(this.ApplicationID);
            return false;
        }

        private bool _UpdateInternationalLicense()
        {
            //call DataAccess Layer 

            clsLicense license = clsLicense.Find(IssuedUsingLocalLicenseID);

            if (license == null || license.ExpirationDate < clsUtilData.GetServerDate()
                || license.IsActive == false
                || clsDetainedLicense.IsLicenseDetained(IssuedUsingLocalLicenseID))
            {
                IsActive = false;
                return false;
            }

            return clsInternationalLicenseData.UpdateInternationalLicense(
                this.InternationalLicenseID,this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID);
        }

        public static clsInternationalLicense FindByInternationalLicenseID(int InternationalLicenseID)
        {
            if (InternationalLicenseID <= 0) return null; // To prevent unnessasry database connection.

            int ApplicationID = -1;
            int DriverID = -1; int IssuedUsingLocalLicenseID = -1;
            DateTime IssueDate = DateTime.Now; DateTime ExpirationDate = DateTime.Now;
             bool IsActive = true; int CreatedByUserID = 1;

            if (clsInternationalLicenseData.GetInternationalLicenseInfoByID(InternationalLicenseID,ref ApplicationID, ref DriverID, 
                ref IssuedUsingLocalLicenseID,
            ref IssueDate, ref ExpirationDate, ref IsActive, ref CreatedByUserID))
            {
                if (ExpirationDate < clsUtilData.GetServerDate())
                {
                    IsActive = false;
                }

                //now we find the base application
                clsApplication Application = clsApplication.Find(ApplicationID);


                return new clsInternationalLicense(InternationalLicenseID, DriverID,
                    IssuedUsingLocalLicenseID, IssueDate, ExpirationDate,
                    IsActive, Application);

            }
             
            else
                return null;

        }

        public static DataTable GetAllInternationalLicenses()
        {
            return clsInternationalLicenseData.GetAllInternationalLicenses();

        }

        public override bool Save()
        {

            if(GetApplicationStatus(this.ApplicationID)!= enApplicationStatus.New)
            {
                return false;
            }

            return _saveDictionary[this.Mode]();
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            if (DriverID <= 0) return -1; // To prevent unnessasry database connection.

            return clsInternationalLicenseData.GetActiveInternationalLicenseIDByDriverID(DriverID);

        }

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            if (DriverID <= 0) return null; // To prevent unnessasry database connection.

            return clsInternationalLicenseData.GetDriverInternationalLicenses(DriverID);
        }

        private static bool _IsDriverEligibleForInternationalLicense(int DriverID,out int LocalLicenseID)
        {
            LocalLicenseID = -1;
            if (DriverID <= 0) return false; // To prevent unnessasry database connection.

            LocalLicenseID = clsLicense.GetActiveLicenseIDByDriverID(DriverID, 3);
            if (LocalLicenseID == -1)
            {
                return false;
            }

            if (clsDetainedLicense.IsLicenseDetained(LocalLicenseID))
            {
                return false;
            }

            int InternationalLicenseID = clsInternationalLicense.GetActiveInternationalLicenseIDByDriverID(DriverID);
            if (InternationalLicenseID != -1)
            {
                return false;
            }

            return true;
        }

        public static bool IsDriverEligibleForInternationalLicense(int DriverID)
        {
            if (DriverID <= 0) return false; // To prevent unnessasry database connection.

            int LocalLicenseID;
            return _IsDriverEligibleForInternationalLicense(DriverID, out LocalLicenseID);
        }

        public static bool IsDriverEligibleForInternationalLicense(int DriverID, out int LocalLicenseID)
        {
            LocalLicenseID = -1;
            if (DriverID <= 0) return false; // To prevent unnessasry database connection.

            return _IsDriverEligibleForInternationalLicense(DriverID, out LocalLicenseID);
        }

        private static clsInternationalLicense _GetNewInternationalLicense(int DriverID, int CreatedByUser)
        {

            clsInternationalLicense InternationalLicense = null;

            int LocalLicenseID = -1;
            if (DriverID <= 0) return null; // To prevent unnessasry database connection.

            if (!IsDriverEligibleForInternationalLicense(DriverID, out LocalLicenseID))
            {
                return null;
            }

            clsLicense LocalLicense = clsLicense.Find(LocalLicenseID);

            if(LocalLicense == null)
            {
                return null;
            }

            DateTime IssueDate = clsBusinessSettings.GetServerDateTime();

            clsApplication application = GetNewApplicationobject(CreatedByUser,
                LocalLicense.DriverInfo.PersonID, enApplicationType.NewInternationalLicense);

            if(application == null)
            {
                return null;
            }

            if (!application.Save())
            {
                return null;
            }

            InternationalLicense = new clsInternationalLicense(DriverID, 
                LocalLicenseID, IssueDate, IssueDate.AddYears(1), true, application);

            return InternationalLicense;
        }

        public static clsInternationalLicense IssueNewInternationalLicense(int DriverID, int CreatedByUser)
        {
            if (DriverID <= 0) return null; // To prevent unnessasry database connection.

            clsInternationalLicense InternationalLicense = _GetNewInternationalLicense(DriverID, CreatedByUser);
            if (InternationalLicense != null)
            {
                if (InternationalLicense.Save())
                {
                    return InternationalLicense;
                }
            }
            return null;
        }

        public static bool DeactvateExpiredLicenses()
        {
            return clsInternationalLicenseData.DeactvateInternationalLicensesforExpiredLocalLicenses();
        }

    }
}
