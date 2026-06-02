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

        public clsDriver DriverInfo { get; protected set; }
        public int InternationalLicenseID {  get; private set; }  
        public int DriverID { get; protected set; }
        public int IssuedUsingLocalLicenseID { get; protected set; }
        public DateTime IssueDate { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public bool IsActive { set; get; }
       

        private clsInternationalLicense()

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

            this.DriverInfo = clsDriver.FindByDriverID(this.DriverID);

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

        public bool Save()
        {

            if (Mode == enMode.AddNew && clsInternationalLicense.GetActiveInternationalLicenseIDByDriverID(this.DriverID) != -1)
            {
                return false;
            }

            //Because of inheritance first we call the save method in the base class,
            //it will take care of adding all information to the application table.
            base.Mode = (clsApplication.enMode)Mode;
            if (!base.Save())
                return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewInternationalLicense())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

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

        public static clsInternationalLicense GetNewInternationalLicense(int DriverID)
        {
            clsInternationalLicense InternationalLicense = new clsInternationalLicense();
            int LocalLicenseID = -1;

            LocalLicenseID = clsLicense.GetActiveLicenseIDByDriverID(DriverID, 3);

            if(LocalLicenseID == -1)
            {
                return null;
            }

            InternationalLicense.DriverID = DriverID;
            InternationalLicense.IssuedUsingLocalLicenseID = LocalLicenseID;
            InternationalLicense.IsActive = true;

            return InternationalLicense;
        }

    }
}
