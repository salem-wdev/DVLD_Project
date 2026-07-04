using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using DVLD_Business;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsInternationalLicense:clsApplication
    {

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { get; protected set; } = enMode.AddNew;

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
            this.DriverID = -1;
            this.IssuedUsingLocalLicenseID = -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now;
           
            this.IsActive = true;
            

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

            Mode = enMode.Update;
        }

        private bool _AddNewInternationalLicense()
        {

           
            //call DataAccess Layer 

            this.InternationalLicenseID = 
                clsInternationalLicenseData.AddNewInternationalLicense(this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID);


            return (this.InternationalLicenseID != -1);
        }

        private bool _UpdateInternationalLicense()
        {
            //call DataAccess Layer 

            return clsInternationalLicenseData.UpdateInternationalLicense(
                this.InternationalLicenseID,this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID);
        }

        public static clsInternationalLicense FindByInternationalLicenseID(int InternationalLicenseID)
        {
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

            //Because of inheritance first we call the save method in the base class,
            //it will take care of adding all information to the application table.
            base.Mode = (clsApplication.enMode)Mode;
            if (!base.Save())
                return false;

            switch (this.Mode)
            {
                case enMode.AddNew:
                    if (_AddNewInternationalLicense())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        clsApplication.Delete(this.ApplicationID);
                        return false;
                    }

                case enMode.Update:
                    clsLicense license = clsLicense.Find(IssuedUsingLocalLicenseID);

                    if (license == null || license.ExpirationDate < clsUtilData.GetServerDate()
                        || license.IsActive == false
                        || clsDetainedLicense.IsLicenseDetained(IssuedUsingLocalLicenseID))
                    {
                        IsActive = false;
                        return false;
                    }
                    
                        return _UpdateInternationalLicense();

            }

            return false;
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {

            return clsInternationalLicenseData.GetActiveInternationalLicenseIDByDriverID(DriverID);

        }

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            return clsInternationalLicenseData.GetDriverInternationalLicenses(DriverID);
        }

        private static bool _IsDriverEligibleForInternationalLicense(int DriverID,out int LocalLicenseID)
        {
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
            int LocalLicenseID;
            return _IsDriverEligibleForInternationalLicense(DriverID, out LocalLicenseID);
        }

        public static bool IsDriverEligibleForInternationalLicense(int DriverID, out int LocalLicenseID)
        {
            return _IsDriverEligibleForInternationalLicense(DriverID, out LocalLicenseID);
        }

        public static clsInternationalLicense GetNewInternationalLicense(int DriverID, int CreatedByUser)
        {
            clsInternationalLicense InternationalLicense = null;
            
            int LocalLicenseID;

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

        public static bool DeactvateExpiredLicenses()
        {
            return clsInternationalLicenseData.DeactvateInternationalLicensesforExpiredLocalLicenses();
        }

    }
}
