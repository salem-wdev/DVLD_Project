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

        public clsDriver DriverInfo;
        public int LicenseID { private set; get; }
        public int ApplicationID { private set; get; }
        public int DriverID { private set; get; } 
        public int LicenseClass { protected set; get; }

        public clsLicenseClass LicenseClassIfo;
        private DateTime _IssueDate; 
        private DateTime _ExpirationDate;

        public DateTime IssueDate { get => _IssueDate;}
        public DateTime ExpirationDate { get => _ExpirationDate;}
        public string Notes { set; get; }
        public float PaidFees { set; get; }
        public bool IsActive { protected set; get; }
        public enIssueReason IssueReason { private set; get; }
        public string IssueReasonText
        {
            get
            {
                return GetIssueReasonText(this.IssueReason);
            }
        }
        public clsDetainedLicense DetainedInfo {  get; }
        public int CreatedByUserID { private set; get; }
        public bool IsDetained
        {
            get { return clsDetainedLicense.IsLicenseDetained(this.LicenseID); }
        }

        private clsLicense()

        {
            this.LicenseID = -1;
            this.ApplicationID = -1;
            this.DriverID = -1;
            this.LicenseClass = -1;
            this._IssueDate = DateTime.Now;
            this._ExpirationDate = DateTime.Now;
            this.Notes = "";
            this.PaidFees = 0;
            this.IsActive = true;
            this.IssueReason = enIssueReason.FirstTime;
            this.CreatedByUserID = -1;

            Mode = enMode.AddNew;

        }

        protected clsLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClass,
            DateTime IssueDate, DateTime ExpirationDate, string Notes,
            float PaidFees, bool IsActive, enIssueReason IssueReason, int CreatedByUserID)

        {
            this.LicenseID = LicenseID;
            this.ApplicationID = ApplicationID;
            this.DriverID = DriverID;
            this.LicenseClass = LicenseClass;
            this._IssueDate = IssueDate;
            this._ExpirationDate = ExpirationDate;
            this.Notes = Notes;
            this.PaidFees = PaidFees;
            this.IsActive = IsActive;
            this.IssueReason = IssueReason;
            this.CreatedByUserID = CreatedByUserID;

            this.DriverInfo = clsDriver.FindByDriverID(this.DriverID);
            this.LicenseClassIfo = clsLicenseClass.Find(this.LicenseClass);
            this.DetainedInfo = clsDetainedLicense.FindByLicenseID(this.LicenseID);

            Mode = enMode.Update;
        }

        protected clsLicense(clsLicense NewLicense)

        {
            this.ApplicationID = NewLicense.ApplicationID;
            this.DriverID = NewLicense.DriverID;
            this.LicenseClass = NewLicense.LicenseClass;
            this._IssueDate = NewLicense.IssueDate;
            this._ExpirationDate = NewLicense.ExpirationDate;
            this.Notes = NewLicense.Notes;
            this.PaidFees = NewLicense.PaidFees;
            this.IsActive = NewLicense.IsActive;
            this.IssueReason = NewLicense.IssueReason;
            this.CreatedByUserID = NewLicense.CreatedByUserID;

            this.DriverInfo = NewLicense.DriverInfo;
            this.LicenseClassIfo = NewLicense.LicenseClassIfo;
            this.DetainedInfo = NewLicense.DetainedInfo;

            Mode = enMode.AddNew;
        }


        private bool _AddNewLicense()
        {
            //call DataAccess Layer 

            this.LicenseID = clsLicenseData.AddNewLicense(this.ApplicationID, this.DriverID, this.LicenseClass,
               ref this._IssueDate, ref this._ExpirationDate, this.Notes, this.PaidFees,
               this.IsActive, (byte)this.IssueReason, this.CreatedByUserID);


            return (this.LicenseID != -1);
        }

        private bool _UpdateLicense()
        {
            //call DataAccess Layer 

            return clsLicenseData.UpdateLicense(this.ApplicationID, this.LicenseID, this.DriverID, this.LicenseClass,
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

                return new clsLicense(LicenseID, ApplicationID, DriverID, LicenseClass,
                                     IssueDate, ExpirationDate, Notes,
                                     PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
            else
                return null;

        }

        public static DataTable GetAllLicenses()
        {
            return clsLicenseData.GetAllLicenses();

        }

        

        public bool Save()
        {

            //throw new NotImplementedException("Most deactivate old licenses for renew and replace scenarios");


            switch (Mode)
            {
                case enMode.AddNew:
                    if (IssueReason != enIssueReason.FirstTime)
                    {
                        if (!clsLicenseData.DeactivateLicenseIDByDriverID(this.DriverID, this.LicenseClass))
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

        private static clsLicense _PrepareObj(clsApplication.enApplicationType ApplicationType,
            int ApplicationID, int DriverID, int LicenseClassID,
            int LocalDrivingLicenseApplicationID)
        {
            clsLicense OldLicense = null;
            clsLicense NewLicense = null;


            if (ApplicationType == clsApplication.enApplicationType.RetakeTest ||
        ApplicationType == clsApplication.enApplicationType.ReleaseDetainedDrivingLicense ||
        ApplicationType == clsApplication.enApplicationType.NewInternationalLicense || 
        ApplicationType == clsApplication.enApplicationType.NewInternationalLicense)
            {
                return null;
            }

            if (ApplicationType == clsApplication.enApplicationType.RenewDrivingLicense)
            {
                OldLicense = clsLicense.Find(GetActiveLicenseIDByDriverID(DriverID, LicenseClassID));
                if (OldLicense != null)
                {
                    if(!OldLicense.IsActive)
                    {
                        return null;
                    }

                    if (OldLicense.ExpirationDate > DateTime.Now)
                    {
                        return null;
                    }
                    else
                    {

                        NewLicense = new clsLicense(OldLicense);
                        NewLicense.IssueReason = enIssueReason.Renew;

                        OldLicense.IsActive = false;

                    }
                }

            }


            if (ApplicationType == clsApplication.enApplicationType.NewDrivingLicense)
            {
                if (!clsLocalDrivingLicenseApplication.DoesPassAllTests(LocalDrivingLicenseApplicationID))
                {
                    return null;
                }
                else
                {
                    NewLicense = new clsLicense();
                    NewLicense.IssueReason = enIssueReason.FirstTime;
                }
            }


            if (ApplicationType == clsApplication.enApplicationType.ReplaceDamagedDrivingLicense ||
                ApplicationType == clsApplication.enApplicationType.ReplaceLostDrivingLicense)
            {
                OldLicense = clsLicense.Find(GetActiveLicenseIDByDriverID(DriverID, LicenseClassID));
                if (OldLicense == null || !OldLicense.IsActive)
                {
                    return null;
                }
                else
                {

                    NewLicense = new clsLicense(OldLicense);

                    NewLicense.IssueReason = (ApplicationType == clsApplication.enApplicationType.ReplaceDamagedDrivingLicense)
                             ? enIssueReason.DamagedReplacement
                             : enIssueReason.LostReplacement;

                    OldLicense.IsActive = false;
                }
            }

            return NewLicense;

        }



        public static clsLicense GetNewLicenseObj(int ApplicationID, int DriverID, int LicenseClassID, int LocalDrivingLicenseApplicationID = -1)
        {

            clsApplication.enApplicationType ApplicationType = clsApplication.GetApplicationTypeID(ApplicationID);

            //throw new NotImplementedException("Most prevent international license to throw here");
            //throw new NotImplementedException("Most declare issue reason");

            clsLicense Newlicense = _PrepareObj(ApplicationType, ApplicationID, DriverID, LicenseClassID, LocalDrivingLicenseApplicationID);

            if (Newlicense == null)
            {
                return null;
            }
            else
            {
                Newlicense.ApplicationID = ApplicationID;
                Newlicense.DriverID = DriverID;
                Newlicense.LicenseClass = LicenseClassID;

                
                
                Newlicense.IsActive = true;
                return Newlicense;

            }
        }

    }

}
