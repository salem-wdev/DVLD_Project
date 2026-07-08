using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DVLD_Business.clsApplication;
using static DVLD_Business.clsLicense;

namespace DVLD_Business
{
    public class clsLocalDrivingLicenseApplication : clsApplication
    {
        public enum enMode { AddNew = 0, Update = 1 }

        public enMode Mode { get; protected set; } = enMode.AddNew;

        public int LocalDrivingLicenseApplicationID { private set; get; }

        private int _LicenseClassID = -1;
        public int LicenseClassID
        {

            set
            {
                if (value != _LicenseClassID)
                {
                    _LicenseClassInfo = null;
                    _LicenseClassID = value;
                }
            }

            get => _LicenseClassID;
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
        public string PersonFullName
        {
            get
            {
                // if lazy loading was failed will return empty string

                if (PersonInfo != null)
                {
                    return PersonInfo.FullName;
                }
                else
                {
                    return string.Empty;
                }
            }

        }

        private clsLocalDrivingLicenseApplication()

        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.LicenseClassID = -1;


            Mode = enMode.AddNew;

        }

        private clsLocalDrivingLicenseApplication(int LicenseClassID, int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)

        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.LicenseClassID = LicenseClassID;
            this.CreatedByUserID = CreatedByUserID;
            this.ApplicantPersonID = ApplicantPersonID;
            this.ApplicationTypeID = ApplicationTypeID;


            Mode = enMode.AddNew;

        }

        private clsLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int LicenseClassID, clsApplication BaseApplication)
            : base(BaseApplication)

        {
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID; ;
            this.LicenseClassID = LicenseClassID;

            Mode = enMode.Update;
        }

        private bool _AddNewLocalDrivingLicenseApplication()
        {
            //call DataAccess Layer 

            this.LocalDrivingLicenseApplicationID = clsLocalDrivingLicenseApplicationData.AddNewLocalDrivingLicenseApplication
                (
                this.ApplicationID, this.LicenseClassID);

            return (this.LocalDrivingLicenseApplicationID != -1);
        }

        private bool _UpdateLocalDrivingLicenseApplication()
        {
            //call DataAccess Layer 

            return clsLocalDrivingLicenseApplicationData.UpdateLocalDrivingLicenseApplication
                (
                this.LocalDrivingLicenseApplicationID, this.ApplicationID, this.LicenseClassID);

        }

        public static clsLocalDrivingLicenseApplication FindByLocalDrivingAppLicenseID(int LocalDrivingLicenseApplicationID)
        {
            // 
            int ApplicationID = -1, LicenseClassID = -1;

            bool IsFound = clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByID
                (LocalDrivingLicenseApplicationID, ref ApplicationID, ref LicenseClassID);


            if (IsFound)
            {
                //now we find the base application
                clsApplication BaseApplication = clsApplication.Find(ApplicationID);

                //we return new object of that person with the right data
                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationID, LicenseClassID, BaseApplication);
            }
            else
                return null;


        }

        public static clsLocalDrivingLicenseApplication FindByApplicationID(int ApplicationID)
        {
            // 
            int LocalDrivingLicenseApplicationID = -1, LicenseClassID = -1;

            bool IsFound = clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByApplicationID
                (ApplicationID, ref LocalDrivingLicenseApplicationID, ref LicenseClassID);


            if (IsFound)
            {
                //now we find the base application
                clsApplication BaseApplication = clsApplication.Find(ApplicationID);

                //we return new object of that person with the right data
                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationID, LicenseClassID, BaseApplication);
            }
            else
                return null;


        }

        public override bool Save()
        {
            if (Mode == enMode.AddNew && clsLicense.GetActiveLicenseIDByPersonID(this.ApplicantPersonID, this.LicenseClassID) != -1)
            {
                return false;
            }


            //Because of inheritance first we call the save method in the base class,
            //it will take care of adding all information to the application table.
            if (!base.Save())
                return false;



            //After we save the main application now we save the sub application.
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLocalDrivingLicenseApplication())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateLocalDrivingLicenseApplication();

            }

            return false;
        }

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            return clsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicenseApplications();
        }

        public override bool Delete()
        {
            if (!CanBeEdited())
                return false;

            bool IsLocalDrivingApplicationDeleted = false;
            bool IsBaseApplicationDeleted = false;
            //First we delete the Local Driving License Application
            IsLocalDrivingApplicationDeleted = clsLocalDrivingLicenseApplicationData.DeleteLocalDrivingLicenseApplication(this.LocalDrivingLicenseApplicationID);

            if (!IsLocalDrivingApplicationDeleted)
                return false;
            //Then we delete the base Application
            IsBaseApplicationDeleted = base.Delete();
            return IsBaseApplicationDeleted;

        }

        public byte GetPassedTestCount()
        {
            return clsTest.GetPassedTestCount(this.LocalDrivingLicenseApplicationID);
        }

        public int GetActiveLicenseID()
        {//this will get the license id that belongs to this application
            return clsLicense.GetActiveLicenseIDByPersonID(this.ApplicantPersonID, this.LicenseClassID);
        }

        /// <summary>
        /// Retrieves the license ID associated with the current application.
        /// </summary>
        /// <remarks>
        /// <para>⚠️ **TECHNICAL DEBT / UNSTABLE METHOD:**</para>
        /// <para>This method is currently marked as unstable or contains an architectural flaw that requires attention.</para>
        /// <para>**TODO:** Refactor this method to address logic/performance issues before moving to production.</para>
        /// </remarks>
        /// <exception cref="NotImplementedException">Thrown because the method is unstable and pending refactoring.</exception>
        public int GetApplicationLicenseID()
        {
            throw new NotImplementedException("This method is unstable and pending refactoring.");

            // This will get the license id that belongs to this application
            return clsLicense.GetLicenseIDByApplicationID(this.ApplicationID);
        }

        public byte TotalTrialsPerTest(clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.TotalTrialsPerTest(this.LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public bool DosPassTest(clsTestType.enTestType TestTypeID)
        {


            return clsLocalDrivingLicenseApplication.DosPassTest(this.LocalDrivingLicenseApplicationID, TestTypeID);
        }

        //public bool DosPassPreviousTest(clsTestType.enTestType CurrentTestType)
        //{
        //    return DosPassTest(this.Tes);
        //}

        public static bool DosPassPreviousTest(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            if (TestTypeID == clsTestType.enTestType.None)
            {
                return false;
            }

            if (TestTypeID == clsTestType.enTestType.VisionTest)
            {
                return true;
            }

            return clsLocalDrivingLicenseApplication.DosPassTest(LocalDrivingLicenseApplicationID, TestTypeID - 1);

        }

        public static bool DosPassTest(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            if (!clsTestAppointment.IsTestAppointmentLocked(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }


            return clsLocalDrivingLicenseApplicationData.DoesPassTestType(LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public static bool DoesPassAllTests(int LocalDrivingLicenseApplicationID)
        {
            return clsLocalDrivingLicenseApplicationData.DoesPassAllTests(LocalDrivingLicenseApplicationID);
        }

        public bool DoesPassAllTests()
        {
            return DoesPassAllTests(this.LocalDrivingLicenseApplicationID);
        }

        public static clsLocalDrivingLicenseApplication GetNewLocalDrivingLicenseApp(int LicenseClassID, int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            // if the application type is not new driving license
            // we should not allow to create new application.
            // other application types should be created by base application class.
            if (ApplicationTypeID != clsApplication.enApplicationType.NewDrivingLicense)
            {
                return null;
            }

            // check if the user and person exist
            if (clsUser.IsUserExists(CreatedByUserID) && clsPerson.IsPersonExists(ApplicantPersonID))
            {

                // TODO: if there is active application for the same person
                // and license class we should not allow to create new application.
                if (clsLicense.GetActiveLicenseIDByPersonID(ApplicantPersonID, LicenseClassID) != -1
                    || clsApplication.GetActiveApplicationIDForLicenseClass(ApplicantPersonID, ApplicationTypeID, LicenseClassID) != -1)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            decimal ApplicationTypeFees = 0;
            ApplicationTypeFees = clsApplicationType.Find((int)ApplicationTypeID)?.ApplicationTypeFees ?? 0;

            return new clsLocalDrivingLicenseApplication(LicenseClassID, CreatedByUserID, ApplicantPersonID, ApplicationTypeID)
            {
                PaidFees = ApplicationTypeFees
            };
        }

        public static bool IsLocalDrivingLicenseApplicationHasLicense(int LocalDrivingLicenseApplicationID, int LicenseClassID)
        {
            return clsLocalDrivingLicenseApplicationData.GetIsLocalDrivingLicenseApplicationHasLicense(LocalDrivingLicenseApplicationID, LicenseClassID);
        }

        public bool HasLicense()
        {
            return IsLocalDrivingLicenseApplicationHasLicense(this.LocalDrivingLicenseApplicationID, this.LicenseClassID);
        }

        public static bool HasActiveTestAppointment(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.IsLocalDrivingLicenseApplicationHasActiveTestAppointment(LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public bool HasActiveTestAppointment(clsTestType.enTestType TestTypeID)
        {
            return HasActiveTestAppointment(this.LocalDrivingLicenseApplicationID, TestTypeID);
        }

        public clsLicense IssueFirstTimeLocalLicense(int CreatedByUserID, string Notes)
        {
            clsLicense license = clsLicense.IssueFirstTimeLocalLicense(this.LocalDrivingLicenseApplicationID, CreatedByUserID, Notes);
            if (license != null)
            {
                this.SetComplete();
                return license;
            }
            return null;
        }

        public static new enApplicationStatus GetApplicationStatus(int LocalDrivingLicenseApplicationID)
        {
            // Retrieve the integer status value from the Data Access Layer
            int statusValue = clsLocalDrivingLicenseApplicationData.GetApplicationStatus(LocalDrivingLicenseApplicationID);

            // Try to safely parse the integer value into the corresponding enum constant
            if (Enum.TryParse(statusValue.ToString(), out enApplicationStatus status))
            {
                return status;
            }

            // Return the default fallback value if the parsing process fails
            return enApplicationStatus.None;
        }

        public static bool DoesAttendTestType(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.DoesAttendTestType(LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public bool DoesAttendTestType(clsTestType.enTestType TestTypeID)
        {
            return DoesAttendTestType(this.LocalDrivingLicenseApplicationID, TestTypeID);
        }


    }
}
