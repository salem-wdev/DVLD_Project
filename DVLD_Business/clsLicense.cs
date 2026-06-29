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

            if (this.IssueReason == enIssueReason.FirstTime)
            {
                if (!_DriverInfo.Save())
                {
                    return false;
                }

                this.DriverID = _DriverInfo.DriverID;
            }
            else
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

        public static DataTable GetAllLicenses()
        {
            return clsLicenseData.GetAllLicenses();

        }

        public bool Save()
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

        private static clsLicense _PrepareNewLicense(clsApplication.enApplicationType ApplicationType,
             int PersonID, int LicenseClassID, int CreatedByUserID,
            int LocalDrivingLicenseApplicationID)
        {
            clsLicense NewLicense = null;

            if (ApplicationType == clsApplication.enApplicationType.NewDrivingLicense)
            {
                if (LocalDrivingLicenseApplicationID < 0)
                {
                    return null;
                }

                if (!IsValidAge(PersonID, LicenseClassID))
                {
                    return null;
                }

                if (clsLocalDrivingLicenseApplication.IsLocalDrivingLicenseApplicationHasLicense(LocalDrivingLicenseApplicationID, LicenseClassID))
                {
                    return null;
                }

                if (clsLicense.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID) != -1)
                {
                    return null;
                }

                if (!clsLocalDrivingLicenseApplication.DoesPassAllTests(LocalDrivingLicenseApplicationID))
                {
                    return null;
                }
                else
                {
                    NewLicense = new clsLicense
                        (_CreateNewApplicationID(CreatedByUserID, PersonID, ApplicationType)
                        , LicenseClassID, CreatedByUserID);
                    NewLicense.IssueReason = enIssueReason.FirstTime;
                    if ((NewLicense._DriverInfo = clsDriver.FindByPersonID(PersonID)) == null)
                    {
                        NewLicense._DriverInfo = clsDriver.CreateNewDriver(PersonID, CreatedByUserID);
                    }

                    NewLicense.PaidFees = _CalculatePaidFees(ApplicationType, LicenseClassID);


                    NewLicense.IsActive = true;
                    if (NewLicense._DriverInfo != null)
                    {
                        return NewLicense;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private static clsLicense _PrepareRenewLicense(int LicenseID, int CreatedByUserID)
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

                NewLicense.PaidFees = _CalculatePaidFees(clsApplication.enApplicationType.RenewDrivingLicense
                    , NewLicense.LicenseClassID);
                OldLicense.IsActive = false;
            }
            return NewLicense;
        }

        private static clsLicense _PrepareReplacementLicense
            (clsApplication.enApplicationType ApplicationType,
             int PersonID, int LicenseClassID)
        {
            clsLicense OldLicense = null;
            clsLicense NewLicense = null;
            if (ApplicationType == clsApplication.enApplicationType.ReplaceDamagedDrivingLicense ||
                ApplicationType == clsApplication.enApplicationType.ReplaceLostDrivingLicense)
            {
                OldLicense = clsLicense.Find(GetActiveLicenseIDByPersonID(PersonID, LicenseClassID));
                if (OldLicense == null || !OldLicense.IsActive)
                {
                    return null;
                }
                else
                {

                    NewLicense = new clsLicense(OldLicense);

                    if (ApplicationType == clsApplication.enApplicationType.ReplaceDamagedDrivingLicense)
                    {
                        NewLicense.PaidFees = _CalculatePaidFees(ApplicationType, LicenseClassID);
                        NewLicense.IssueReason = enIssueReason.DamagedReplacement;
                    }
                    else
                    {
                        NewLicense.PaidFees = _CalculatePaidFees(ApplicationType, LicenseClassID);
                        NewLicense.IssueReason = enIssueReason.LostReplacement;
                    }

                    OldLicense.IsActive = false;
                }
            }
            return NewLicense;
        }

        // TODO: Technical Debt - Implement a dedicated Service Layer (Business Manager) to handle
        // license state transitions and logic validation, decoupling business rules from DataAccess 
        // and moving towards a Unit of Work pattern for atomic database operations.
        private static clsLicense _PrepareObj(clsApplication.enApplicationType ApplicationType,
            int PersonID, int LicenseClassID, int CreatedByUserID,
            int LocalDrivingLicenseApplicationID)
        {

            switch (ApplicationType)
            {
                case clsApplication.enApplicationType.RetakeTest:
                case clsApplication.enApplicationType.ReleaseDetainedDrivingLicense:
                case clsApplication.enApplicationType.NewInternationalLicense:
                case clsApplication.enApplicationType.RenewDrivingLicense:
                    return null;
                case clsApplication.enApplicationType.NewDrivingLicense:
                    return _PrepareNewLicense(ApplicationType, PersonID, LicenseClassID, CreatedByUserID,
                    LocalDrivingLicenseApplicationID);
                case clsApplication.enApplicationType.ReplaceDamagedDrivingLicense:
                case clsApplication.enApplicationType.ReplaceLostDrivingLicense:
                    return _PrepareReplacementLicense(ApplicationType, PersonID, LicenseClassID);
                default:
                    return null;

            }

        }

        /// <summary>
        /// Prepares and initializes a new license object based on the application type (New, Renew, or Replacement).
        /// </summary>
        /// <param name="ApplicationID">The ID of the application associated with this license.</param>
        /// <param name="DriverID">The ID of the driver receiving the license.</param>
        /// <param name="LicenseClassID">The ID of the target license class.</param>
        /// <param name="CreatedByUserID">The ID of the user who is creating this license.</param>
        /// <param name="LocalDrivingLicenseApplicationID">
        /// Optional (Defaults to -1). Required only for first-time license issuance to verify that all required tests are passed. 
        /// Can be omitted or left as -1 for renewals and replacements.
        /// </param>
        /// <returns>A populated <see cref="clsLicense"/> object if validation passes; otherwise, returns <c>null</c>.</returns>

        // TODO: Overloading factory method to send License ID directly for renwal
        public static clsLicense GetNewLicenseObj(int ApplicationID, int PersonID, int LicenseClassID,
            int CreatedByUserID, int LocalDrivingLicenseApplicationID = -1)
        {
            if (clsApplication.GetApplicationStatus(ApplicationID) != clsApplication.enApplicationStatus.New)
            {
                return null;
            }

            clsApplication.enApplicationType ApplicationType = clsApplication.GetApplicationTypeID(ApplicationID);

            //throw new NotImplementedException("Most prevent international license to throw here");
            //throw new NotImplementedException("Most declare issue reason");

            clsLicense Newlicense = _PrepareObj(ApplicationType, PersonID, LicenseClassID,
                CreatedByUserID, LocalDrivingLicenseApplicationID);

            if (Newlicense == null)
            {
                return null;
            }
            else
            {
                Newlicense.ApplicationID = ApplicationID;
                Newlicense.LicenseClassID = LicenseClassID;
                Newlicense.CreatedByUserID = CreatedByUserID;

                Newlicense.IsActive = true;
                    return Newlicense;

            }
        }

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

        public static clsLicense RenewLicense(int LicenseID, int CreatedByUserID)
        {
            clsLicense license = _PrepareRenewLicense(LicenseID, CreatedByUserID);

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

    }

}
