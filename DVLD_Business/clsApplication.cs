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
        private DateTime _ApplicationDate;
        public enApplicationType ApplicationTypeID { get; protected set; }
        private enApplicationStatus _ApplicationStatus;
        private DateTime _LastStatusDate;
        public decimal PaidFees { get; internal set; }
        public int CreatedByUserID { get; protected set; }

        private clsUser _CreatedByUserInfo = null;

        private clsApplicationType _ApplicationTypeInfo = null;

        private clsPerson _PersonInfo = null;

        public clsUser CreatedByUserInfo
        {
            get
            {
                if (_CreatedByUserInfo == null && CreatedByUserID != -1)
                {
                    _CreatedByUserInfo = clsUser.Find(CreatedByUserID);
                }
                return _CreatedByUserInfo;
            }
        }
        public clsApplicationType ApplicationTypeInfo
        {
            get
            {
                if (_ApplicationTypeInfo == null && (int)ApplicationTypeID > 0)
                {
                    _ApplicationTypeInfo = clsApplicationType.Find((int)ApplicationTypeID);
                }
                return _ApplicationTypeInfo;
            }
        }
        public clsPerson PersonInfo
        {
            get
            {
                if (_PersonInfo == null && ApplicantPersonID != -1)
                {
                    _PersonInfo = clsPerson.Find(ApplicantPersonID);
                }
                return _PersonInfo;
            }
        }
        public DateTime ApplicationDate
        {
            get
            {
                return _ApplicationDate;
            }
        }
        public enApplicationStatus ApplicationStatus
        {
            get
            {
                return _ApplicationStatus;
            }
        }
        public DateTime LastStatusDate
        {
            get
            {
                return _LastStatusDate;
            }
        }


        private clsApplication(int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = ApplicantPersonID;
            this._ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = ApplicationTypeID;
            this._ApplicationStatus = enApplicationStatus.New;
            this._LastStatusDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = CreatedByUserID;


            Mode = enMode.AddNew;
        }


        protected clsApplication()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this._ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = enApplicationType.NewDrivingLicense;
            this._ApplicationStatus = enApplicationStatus.New;
            this._LastStatusDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;


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
            this._ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this._ApplicationStatus = ApplicationStatus;
            this._LastStatusDate = LastStatusDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;


            Mode = enMode.Update;
        }

        protected clsApplication(clsApplication BaseApplication)
        {
            this.ApplicationID = BaseApplication.ApplicationID;
            this.ApplicantPersonID = BaseApplication.ApplicantPersonID;
            this._ApplicationDate = BaseApplication.ApplicationDate;
            this.ApplicationTypeID = BaseApplication.ApplicationTypeID;
            this._ApplicationStatus = BaseApplication.ApplicationStatus;
            this._LastStatusDate = BaseApplication.LastStatusDate;
            this.PaidFees = BaseApplication.PaidFees;
            this.CreatedByUserID = BaseApplication.CreatedByUserID;


            Mode = enMode.Update;
        }

        private bool _AddNewApplication()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            byte applicationStatus = 0;

            this.ApplicationID = clsApplicationData.AddNewApplication(this.ApplicantPersonID,
                ref this._ApplicationDate, (int)this.ApplicationTypeID, ref applicationStatus,
                ref this._LastStatusDate, this.PaidFees, this.CreatedByUserID);

            this._ApplicationStatus = (enApplicationStatus)applicationStatus;

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


            return clsApplicationData.UpdateApplication(this.ApplicationID,
                this.PaidFees, this.CreatedByUserID);
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

            if (clsApplicationData.UpdateStatus(ApplicationID, (byte)enApplicationStatus.Cancelled, clsBusinessSettings.GetServerDateTime()))
            {
                this._ApplicationStatus = enApplicationStatus.Cancelled;
                return true;
            }
            return false;
        }

        internal bool SetComplete()
        {
            if (SetComplete(ApplicationID))
            {
                this._ApplicationStatus = enApplicationStatus.Completed;
                return true;
            }
            return false;
        }

        internal static bool SetComplete(int ApplicationID)
        {
            if (!CanBeEdited(ApplicationID))
                return false;

            return clsApplicationData.UpdateStatus(ApplicationID, (byte)enApplicationStatus.Completed, clsBusinessSettings.GetServerDateTime());
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

        public static enApplicationStatus GetApplicationStatus(int ApplicationID)
        {
            return (enApplicationStatus)clsApplicationData.GetApplicationStatus(ApplicationID);
        }

        protected static clsApplication GetNewApplicationobject(int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            if (DoesPersonHaveActiveApplication(ApplicantPersonID, (int)ApplicationTypeID))
            {
                return null;
            }

            clsApplication application = new clsApplication(CreatedByUserID, ApplicantPersonID, ApplicationTypeID);
            return application;
        }

        internal static clsApplication GetNewApplication(int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            clsApplication application = GetNewApplicationobject(CreatedByUserID, ApplicantPersonID, ApplicationTypeID);
            if (application.Save())
            {
                return application;
            }
            else
            {
                return null;
            }
        }

    }
}
