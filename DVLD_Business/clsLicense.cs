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
    public class clsLicense
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

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
        public bool IsDetained
        {
            get { return clsDetainedLicense.IsLicenseDetained(this.LicenseID); }
        }

        private clsDriver _DriverInfo = null;
        public clsDriver DriverInfo
        {
            get
            {
                if (_DriverInfo == null && DriverID != -1)
                {
                    _DriverInfo = clsDriver.FindByDriverID(this.DriverID);
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
                    _DetainedInfo = clsDetainedLicense.FindByLicenseID(this.LicenseID);
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

            Mode = enMode.AddNew;
        }

        // TODO: Refactor date handling. 
        // Currently relying on DataAccess layer to override dates via ref parameters.
        // Need to move expiration logic (Class Validity) here to comply with Rich Domain Model.
        private bool _AddNewLicense()
        {
            int OldLicenseID = -1;
            if (clsApplication.GetApplicationStatus(ApplicationID) != clsApplication.enApplicationStatus.New)
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


            if (this.LicenseID != -1 && clsApplication.SetComplete(this.ApplicationID))
            {
                clsLicense.DeactivateLicense(OldLicenseID);
                return true;
            }

            return false;
        }

        private bool _UpdateLicense()
        {
            //call DataAccess Layer 

            return clsLicenseData.UpdateLicense(this.ApplicationID, this.LicenseID, this.DriverID, this.LicenseClassID,
               this.Notes, this.PaidFees, this.IsActive, (byte)this.IssueReason,
               this.CreatedByUserID);
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

                return new clsLicense(LicenseID, ApplicationID, DriverID, LicenseClass,
                                 IssueDate, ExpirationDate, Notes,
                                 PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
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

                return new clsLicense(LicenseID, ApplicationID, DriverID, LicenseClass,
                                 IssueDate, ExpirationDate, Notes,
                                 PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
            }

            else
                return null;

        }

        public static DataTable GetAllLicenses()
        {
            return clsLicenseData.GetAllLicenses();

        }

        private bool Save()
        {

            switch (Mode)
            {
                case enMode.AddNew:
                    if (IssueReason != enIssueReason.FirstTime)
                    {
                        // To deactivate the old license upon renewal
                        if (!clsLicenseData.DeactivateLicenseIDByDriverID(this.DriverID, this.LicenseClassID))
                        {

                            return false;

                        }
                    }

                    if (_AddNewLicense())
                    {

                        Mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateLicense();

            }

            return false;
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

        private static float _CalculatePaidFees(clsApplication.enApplicationType ApplicationType, int LicenseClassID)
        { 
            if(ApplicationType == clsApplication.enApplicationType.ReplaceLostDrivingLicense
                || ApplicationType == clsApplication.enApplicationType.ReplaceDamagedDrivingLicense)
            {
                return (float)clsApplicationType.Find((int)ApplicationType).ApplicationTypeFees;
            }
            return clsLicenseClass.Find(LicenseClassID).ClassFees + (float)clsApplicationType.Find((int)ApplicationType).ApplicationTypeFees;
        }

        private static int _CreateNewApplicationID(int CreatedByUserID, int PersonID, clsApplication.enApplicationType ApplicationType)
        {
            clsApplication application = clsApplication.GetNewApplicationobject(CreatedByUserID, PersonID, ApplicationType);
            if(application != null)
            {
                if(application.Save())
                {
                    return application.ApplicationID;
                }
            }
            return -1;
        }

        public static bool IsValidAge(int PersonID, int LicenseClassID)
        {
            clsPerson person = clsPerson.Find(PersonID);

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

        private static clsLicense _PrepareNewLicense(int LocalDrivingLicenseApplicationID, int CreatedByUserID, string Notes)
        {
            if (LocalDrivingLicenseApplicationID < 0)
            {
                return null;
            }

            clsLicense NewLicense = null;
            clsLocalDrivingLicenseApplication localDrivingLicenseApplication
                = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            if (localDrivingLicenseApplication == null)
            {
                return null;
            }

            if (!IsValidAge(localDrivingLicenseApplication.ApplicantPersonID, localDrivingLicenseApplication.LicenseClassID))
            {
                return null;
            }

            if (localDrivingLicenseApplication.HasLicense())
            {
                return null;
            }

            if (clsLicense.GetActiveLicenseIDByPersonID(localDrivingLicenseApplication.ApplicantPersonID, localDrivingLicenseApplication.LicenseClassID) != -1)
            {
                return null;
            }

            if (!localDrivingLicenseApplication.DoesPassAllTests())
            {
                return null;
            }


            float PaidFees = _CalculatePaidFees(clsApplication.enApplicationType.NewDrivingLicense, localDrivingLicenseApplication.LicenseClassID);
            NewLicense = new clsLicense(localDrivingLicenseApplication.ApplicationID, localDrivingLicenseApplication.LicenseClassID,
                CreatedByUserID, Notes, PaidFees, enIssueReason.FirstTime);

            if ((NewLicense._DriverInfo = clsDriver.FindByPersonID(localDrivingLicenseApplication.ApplicantPersonID)) == null)
            {
                NewLicense._DriverInfo = clsDriver.CreateNewDriver(localDrivingLicenseApplication.ApplicantPersonID, CreatedByUserID);
            }

            NewLicense.DriverID = NewLicense._DriverInfo.DriverID;
            NewLicense.IsActive = true;

            return (NewLicense._DriverInfo != null) ? NewLicense : null;
        }
       
        private static clsLicense _PrepareRenewLicense(int LicenseID, string Notes, int CreatedByUserID)
        {
            clsLicense OldLicense = Find(LicenseID);
            clsLicense NewLicense = null;

            // TODO: Get License ID if it's active or not for renewal
            if (OldLicense != null)
            {

                if (clsUtilData.GetServerDate() < OldLicense.ExpirationDate.AddMonths(-3) || clsUtilData.GetServerDate() > OldLicense.ExpirationDate.AddMonths(3))
                {
                    return null;
                }

                if (!IsLicenseActive(OldLicense.LicenseID) && clsUtilData.GetServerDate() < OldLicense.ExpirationDate)
                {
                    return null;
                }

                if (clsDetainedLicense.IsLicenseDetained(LicenseID))
                {
                    return null;
                }

                NewLicense = new clsLicense(OldLicense);
                NewLicense.IssueReason = enIssueReason.Renew;
                NewLicense.ApplicationID = _CreateNewApplicationID(CreatedByUserID,
                    NewLicense.DriverInfo.PersonID, clsApplication.enApplicationType.RenewDrivingLicense);
                NewLicense.CreatedByUserID = CreatedByUserID;
                NewLicense.Notes = Notes;
                NewLicense.PaidFees = _CalculatePaidFees(clsApplication.enApplicationType.RenewDrivingLicense
                    , NewLicense.LicenseClassID);
                OldLicense.IsActive = false;
            }
            return NewLicense;
        }

        private static clsLicense _PrepareReplacementLicense(int LicenseID, int CreatedByUserID, string Notes, enIssueReason IssueReason)
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


            NewLicense.PaidFees = _CalculatePaidFees(applicationType, NewLicense.LicenseClassID);
            NewLicense.IssueReason = IssueReason;
            NewLicense.ApplicationID = _CreateNewApplicationID(CreatedByUserID,
                NewLicense.DriverInfo.PersonID, applicationType);
            NewLicense.CreatedByUserID = CreatedByUserID;
            NewLicense.Notes = Notes;

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

        public static clsLicense RenewLicense(int LicenseID, string Notes, int CreatedByUserID)
        {
            clsLicense license = _PrepareRenewLicense(LicenseID, Notes, CreatedByUserID);

            if (license != null)
            {
                if (license.Save())
                {
                    return license;
                }
                return null;
            }
            return null;
        }

        internal static clsLicense IssueFirstTimeLocalLicense(int LocalDrivingLicenseApplicationID, int CreatedByUserID, string Notes)
        {
            clsLicense license = _PrepareNewLicense(LocalDrivingLicenseApplicationID, CreatedByUserID, Notes);

            if (license != null)
            {
                if (license.Save())
                {
                    return license;
                }
                return null;
            }
            return null;
        }

        public static clsLicense Replace(int LicenseID, int CreatedByUserID, string Notes, enIssueReason IssueReason)
        {
            clsLicense license = _PrepareReplacementLicense(LicenseID, CreatedByUserID, Notes, IssueReason);
            if (license != null)
            {
                if (license.Save())
                {
                    return license;
                }
            }
            return null;
        }

    }

}
