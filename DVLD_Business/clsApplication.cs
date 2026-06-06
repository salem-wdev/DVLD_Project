using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsApplication
    {


        public enum enMode { AddNew = 0, Update = 1 }
        public enum enApplicationStatus : byte
        {
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
        public DateTime ApplicationDate { get; private set; }
        public enApplicationType ApplicationTypeID { get; protected set; }
        public enApplicationStatus ApplicationStatus { get; private set; }
        public DateTime LastStatusDate { get; private set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }

        public clsUser CreatedByUserInfo { get; set; }
        public clsApplicationType ApplicationTypeInfo { get; set; }
        public clsPerson PersonInfo { get; set; }



        public clsApplication()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = enApplicationType.NewDrivingLicense;
            this.ApplicationStatus = enApplicationStatus.New;
            this.LastStatusDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;

            this.PersonInfo = new clsPerson();
            this.ApplicationTypeInfo = new clsApplicationType();
            this.CreatedByUserInfo = new clsUser();

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
            this.ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this.ApplicationStatus = ApplicationStatus;
            this.LastStatusDate = LastStatusDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;

            this.ApplicationTypeInfo = clsApplicationType.Find((int)ApplicationTypeID);
            this.PersonInfo = clsPerson.Find(ApplicantPersonID);
            this.CreatedByUserInfo = clsUser.Find(CreatedByUserID);

            Mode = enMode.Update;
        }

        protected clsApplication(clsApplication BaseApplication)
        {
            this.ApplicationID = BaseApplication.ApplicationID;
            this.ApplicantPersonID = BaseApplication.ApplicantPersonID;
            this.ApplicationDate = BaseApplication.ApplicationDate;
            this.ApplicationTypeID = BaseApplication.ApplicationTypeID;
            this.ApplicationStatus = BaseApplication.ApplicationStatus;
            this.LastStatusDate = BaseApplication.LastStatusDate;
            this.PaidFees = BaseApplication.PaidFees;
            this.CreatedByUserID = BaseApplication.CreatedByUserID;

            this.ApplicationTypeInfo = BaseApplication.ApplicationTypeInfo;
            this.PersonInfo = BaseApplication.PersonInfo;
            this.CreatedByUserInfo = BaseApplication.CreatedByUserInfo;

            Mode = enMode.Update;
        }

        private bool _AddNewApplication()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}
            this.ApplicantPersonID = PersonInfo.PersonID;
            this.ApplicationTypeID = (enApplicationType)ApplicationTypeInfo.ApplicationTypeID;
            this.CreatedByUserID = CreatedByUserInfo.UserID;

            this.ApplicationID = clsApplicationData.AddNewApplication(this.ApplicantPersonID,
                this.ApplicationDate, (int)this.ApplicationTypeID, (byte)this.ApplicationStatus,
                this.LastStatusDate, this.PaidFees, this.CreatedByUserID);

            return (ApplicationID != -1);
        }

        private bool _UpdateApplication()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            if (!CanBeEdited())
                return false;

            this.ApplicantPersonID = PersonInfo.PersonID;
            this.ApplicationTypeID = (enApplicationType)ApplicationTypeInfo.ApplicationTypeID;
            this.CreatedByUserID = CreatedByUserInfo.UserID;

            return clsApplicationData.UpdateApplication(this.ApplicationID, this.ApplicantPersonID,
                this.ApplicationDate, (int)this.ApplicationTypeID, (byte)this.ApplicationStatus,
                this.LastStatusDate, this.PaidFees, this.CreatedByUserID);
        }

        public static bool Delete(int ApplicationID)
        {
            if (!CanBeEdited(ApplicationID))
                return false;

            return clsApplicationData.DeleteApplication(ApplicationID);
        }

        public virtual bool Delete()
        {
            if (!CanBeEdited())
                return false;

            return clsApplicationData.DeleteApplication(this.ApplicationID);
        }

        public static clsApplication Find(int ApplicationID)
        {
            int ApplicantPersonID = -1;
            DateTime ApplicationDate = DateTime.Now;
            int ApplicationTypeID = -1;
            byte ApplicationStatus = 0;
            DateTime LastStatusDate = DateTime.Now;
            decimal PaidFees = 0.0m;
            int CreatedByUserID = -1;

            bool found = clsApplicationData.GetApplicationInfoByApplicationID(ApplicationID, ref ApplicantPersonID,
                ref ApplicationDate, ref ApplicationTypeID, ref ApplicationStatus,
                ref LastStatusDate, ref PaidFees, ref CreatedByUserID);


            if (found)
            {
                return new clsApplication(ApplicationID, ApplicantPersonID,
                ApplicationDate, (enApplicationType)ApplicationTypeID, (enApplicationStatus)ApplicationStatus,
                LastStatusDate, PaidFees, CreatedByUserID);

            }
            else
            {
                return null;
            }
        }

        public static DataTable GetApplicationsPersonList(int ApplicantPersonID)
        {
            return clsApplicationData.GetApplicationsPersonList(ApplicantPersonID);
        }

        public static DataTable GetApplicationsCreatedByUserList(int CreatedByUserID)
        {
            return clsApplicationData.GetApplicationsCreatedByUserList(CreatedByUserID);
        }

        public static bool IsApplicationExists(int ApplicationID)
        {
            return clsApplicationData.IsApplicationExist(ApplicationID);
        }

        public static DataTable GetAllApplications()
        {
            return clsApplicationData.GetAllApplications();
        }

        public bool Cancel()
        {
            if (!CanBeEdited())
                return false;

            return clsApplicationData.UpdateStatus(ApplicationID, (byte)enApplicationStatus.Cancelled);
        }

        public bool SetComplete()
        {
            if (!CanBeEdited())
                return false;

            return clsApplicationData.UpdateStatus(ApplicationID, (byte)enApplicationStatus.Completed);
        }

        public virtual bool Save()
        {
            

            switch (Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewApplication())
                        {
                            Mode = enMode.Update;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case enMode.Update:
                    {
                        return _UpdateApplication();
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public static bool DoesPersonHaveActiveApplication(int PersonID, int ApplicationTypeID)
        {
            return clsApplicationData.DoesPersonHaveActiveApplication(PersonID, ApplicationTypeID);
        }

        public bool DoesPersonHaveActiveApplication(int ApplicationTypeID)
        {
            return DoesPersonHaveActiveApplication(this.ApplicantPersonID, ApplicationTypeID);
        }

        public static int GetActiveApplicationID(int PersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            return clsApplicationData.GetActiveApplicationID(PersonID, (int)ApplicationTypeID);
        }

        public static int GetActiveApplicationIDForLicenseClass(int PersonID, clsApplication.enApplicationType ApplicationTypeID, int LicenseClassID)
        {
            return clsApplicationData.GetActiveApplicationIDForLicenseClass(PersonID, (int)ApplicationTypeID, LicenseClassID);
        }

        public int GetActiveApplicationID(clsApplication.enApplicationType ApplicationTypeID)
        {
            return GetActiveApplicationID(this.ApplicantPersonID, ApplicationTypeID);
        }

        public bool CanBeEdited()
        {
            return CanBeEdited(this.ApplicationID);
        }

        public static bool CanBeEdited(int ApplicationID)
        {
            return clsApplicationData.CanApplicationBeEdited(ApplicationID);
        }

        public static enApplicationType GetApplicationTypeID(int ApplicationID)
        {
            return (enApplicationType)clsApplicationData.GetApplicationTypeID(ApplicationID);
        }

    }
}
