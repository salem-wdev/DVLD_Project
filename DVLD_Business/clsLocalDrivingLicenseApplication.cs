using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DVLD_Business.clsApplication;
using static DVLD_Business.clsLicense;
using DVLD_Business.Users;
using Microsoft.VisualStudio.Threading;

namespace DVLD_Business
{
    public class clsLocalDrivingLicenseApplication : clsApplication
    {
        public new enum enMode { AddNew = 0, Update = 1 }

        public new enMode Mode { get; protected set; } = enMode.AddNew;
        private new readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public int LocalDrivingLicenseApplicationID { private set; get; }

        private int _LicenseClassID = -1;
        public int LicenseClassID
        {

            set
            {
                if (value != _LicenseClassID)
                {
                    _LicenseClassInfoLazy = null;
                    _LicenseClassID = value;
                }
            }

            get => _LicenseClassID;
        }

        private AsyncLazy<clsLicenseClass> _LicenseClassInfoLazy = null;

        public Task<clsLicenseClass> LicenseClassInfoAsync
        {
            get
            {
                if (_LicenseClassInfoLazy == null)
                {
                    _LicenseClassInfoLazy = new AsyncLazy<clsLicenseClass>(async () =>
                    {
                        if (LicenseClassID == -1)
                        {
                            return null;
                        }
                        return await clsLicenseClass.FindAsync(LicenseClassID);
                    });
                }
                return _LicenseClassInfoLazy.GetValueAsync();
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

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>()
            {
                [enMode.AddNew] = async () =>
                {
                    base.Mode = clsApplication.enMode.AddNew;
                    return await base._AddNewApplicationAsync() && await this._AddNewLocalDrivingLicenseApplicationAsync();
                },

                [enMode.Update] = async () =>
                {
                    base.Mode = clsApplication.enMode.Update;
                    return await base._UpdateApplicationAsync() && await this._UpdateLocalDrivingLicenseApplicationAsync();
                }
            };

            Mode = enMode.AddNew;

        }

        private clsLocalDrivingLicenseApplication(int LicenseClassID, int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)

        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.LicenseClassID = LicenseClassID;
            this.CreatedByUserID = CreatedByUserID;
            this.ApplicantPersonID = ApplicantPersonID;
            this.ApplicationTypeID = ApplicationTypeID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>()
            {
                [enMode.AddNew] = async () =>
                {
                    base.Mode = clsApplication.enMode.AddNew;
                    return await base._AddNewApplicationAsync() && await this._AddNewLocalDrivingLicenseApplicationAsync();
                },

                [enMode.Update] = async () =>
                {
                    base.Mode = clsApplication.enMode.Update;
                    return await base._UpdateApplicationAsync() && await this._UpdateLocalDrivingLicenseApplicationAsync();
                }
            };


            Mode = enMode.AddNew;

        }

        private clsLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int LicenseClassID, clsApplication BaseApplication)
            : base(BaseApplication)

        {
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID; ;
            this.LicenseClassID = LicenseClassID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>()
            {
                [enMode.AddNew] = async () =>
                {
                    base.Mode = clsApplication.enMode.AddNew;
                    return await base._AddNewApplicationAsync() && await this._AddNewLocalDrivingLicenseApplicationAsync();
                },

                [enMode.Update] = async () =>
                {
                    base.Mode = clsApplication.enMode.Update;
                    return await base._UpdateApplicationAsync() && await this._UpdateLocalDrivingLicenseApplicationAsync();
                }
            };


            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewLocalDrivingLicenseApplicationAsync()
        {
            if (await clsLicense.GetActiveLicenseIDByPersonIDAsync(this.ApplicantPersonID, this.LicenseClassID) != -1)
            {
                return false;
            }

            //call DataAccess Layer 

            this.LocalDrivingLicenseApplicationID = await clsLocalDrivingLicenseApplicationData.AddNewLocalDrivingLicenseApplicationAsync
                (this.ApplicationID, this.LicenseClassID);

            if (this.LocalDrivingLicenseApplicationID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private async Task<bool> _UpdateLocalDrivingLicenseApplicationAsync()
        {
            //call DataAccess Layer 

            return await clsLocalDrivingLicenseApplicationData.UpdateLocalDrivingLicenseApplicationAsync
                (this.LocalDrivingLicenseApplicationID, this.ApplicationID, this.LicenseClassID);

        }

        public static async Task<clsLocalDrivingLicenseApplication> FindByLocalDrivingAppLicenseIDAsync(int LocalDrivingLicenseApplicationID)
        {
            // 

            var LocalDrivingLicenseApplicationInfo = await clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByIDAsync(LocalDrivingLicenseApplicationID);


            if (LocalDrivingLicenseApplicationInfo.IsFound)
            {
                //now we find the base application
                clsApplication BaseApplication = await clsApplication.FindAsync(LocalDrivingLicenseApplicationInfo.ApplicationID);

                //we return new object of that person with the right data
                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationID, LocalDrivingLicenseApplicationInfo.LicenseClassID, BaseApplication);
            }
            else
                return null;


        }

        public static async Task<clsLocalDrivingLicenseApplication> FindByApplicationID(int ApplicationID)
        {
            // 

            var LocalDrivingLicenseApplicationInfo = await clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByApplicationIDAsync(ApplicationID);


            if (LocalDrivingLicenseApplicationInfo.IsFound)
            {
                //now we find the base application
                clsApplication BaseApplication = await clsApplication.FindAsync(ApplicationID);

                //we return new object of that person with the right data
                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationInfo.LocalDrivingLicenseApplicationID, LocalDrivingLicenseApplicationInfo.LicenseClassID, BaseApplication);
            }
            else
                return null;


        }

        public override async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }

        public static async Task<DataTable> GetAllLocalDrivingLicenseApplications()
        {
            return await clsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicenseApplicationsAsync();
        }

        public override async Task<bool> DeleteAsync()
        {
            if (!await CanBeEditedAsync())
                return false;

            bool IsLocalDrivingApplicationDeleted = false;
            bool IsBaseApplicationDeleted = false;
            //First we delete the Local Driving License Application
            IsLocalDrivingApplicationDeleted = await clsLocalDrivingLicenseApplicationData.DeleteLocalDrivingLicenseApplicationAsync(this.LocalDrivingLicenseApplicationID);

            if (!IsLocalDrivingApplicationDeleted)
                return false;
            //Then we delete the base Application
            IsBaseApplicationDeleted = await base.DeleteAsync();
            return IsBaseApplicationDeleted;

        }

        public async Task<byte> GetPassedTestCountAsync()
        {
            return await clsTest.GetPassedTestCountAsync(this.LocalDrivingLicenseApplicationID);
        }

        public async Task<int> GetActiveLicenseIDAsync()
        {//this will get the license id that belongs to this application
            return await clsLicense.GetActiveLicenseIDByPersonIDAsync(this.ApplicantPersonID, this.LicenseClassID);
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
        [Obsolete("This method is unstable and pending refactoring.", true)]
        public async Task<int> GetApplicationLicenseIDAsync()
        {
            //throw new NotImplementedException("This method is unstable and pending refactoring.");

            // This will get the license id that belongs to this application
            return await clsLicense.GetLicenseIDByApplicationIDAsync(this.ApplicationID);
        }

        public async Task<byte> TotalTrialsPerTest(clsTestType.enTestType TestTypeID)
        {
            return await clsLocalDrivingLicenseApplicationData.TotalTrialsPerTestAsync(this.LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public async Task<bool> DosPassTestAsync(clsTestType.enTestType TestTypeID)
        {


            return await clsLocalDrivingLicenseApplication.DosPassTestAsync(this.LocalDrivingLicenseApplicationID, TestTypeID);
        }

        //public bool DosPassPreviousTest(clsTestType.enTestType CurrentTestType)
        //{
        //    return DosPassTest(this.Tes);
        //}

        public static async Task<bool> DosPassPreviousTestAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            if (TestTypeID == clsTestType.enTestType.None)
            {
                return false;
            }

            if (TestTypeID == clsTestType.enTestType.VisionTest)
            {
                return true;
            }

            return await clsLocalDrivingLicenseApplication.DosPassTestAsync(LocalDrivingLicenseApplicationID, TestTypeID - 1);

        }

        public static async Task<bool> DosPassTestAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            if (!await clsTestAppointment.IsTestAppointmentLockedAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }


            return await clsLocalDrivingLicenseApplicationData.DoesPassTestTypeAsync(LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public static async Task<bool> DoesPassAllTestsAsync(int LocalDrivingLicenseApplicationID)
        {
            return await clsLocalDrivingLicenseApplicationData.DoesPassAllTestsAsync(LocalDrivingLicenseApplicationID);
        }

        public async Task<bool> DoesPassAllTestsAsync()
        {
            return await DoesPassAllTestsAsync(this.LocalDrivingLicenseApplicationID);
        }

        public static async Task<clsLocalDrivingLicenseApplication> GetNewLocalDrivingLicenseAppAsync(int LicenseClassID, int CreatedByUserID, int ApplicantPersonID, clsApplication.enApplicationType ApplicationTypeID)
        {
            // if the application type is not new driving license
            // we should not allow to create new application.
            // other application types should be created by base application class.
            if (ApplicationTypeID != clsApplication.enApplicationType.NewDrivingLicense)
            {
                return null;
            }

            // check if the user and person exist
            if (clsUser.IsUserExists(CreatedByUserID) && await clsPerson.IsPersonExistsAsync(ApplicantPersonID))
            {

                // TODO: if there is active application for the same person
                // and license class we should not allow to create new application.
                if (await clsLicense.GetActiveLicenseIDByPersonIDAsync(ApplicantPersonID, LicenseClassID) != -1
                    || await clsApplication.GetActiveApplicationIDForLicenseClassAsync(ApplicantPersonID, ApplicationTypeID, LicenseClassID) != -1)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            decimal ApplicationTypeFees = 0;
            ApplicationTypeFees = (await clsApplicationType.FindAsync((int)ApplicationTypeID))?.ApplicationTypeFees ?? 0;

            return new clsLocalDrivingLicenseApplication(LicenseClassID, CreatedByUserID, ApplicantPersonID, ApplicationTypeID)
            {
                PaidFees = ApplicationTypeFees
            };
        }

        public static async Task<bool> IsLocalDrivingLicenseApplicationHasLicenseAsync(int LocalDrivingLicenseApplicationID, int LicenseClassID)
        {
            return await clsLocalDrivingLicenseApplicationData.GetIsLocalDrivingLicenseApplicationHasLicenseAsync(LocalDrivingLicenseApplicationID, LicenseClassID);
        }

        public async Task<bool> HasLicenseAsync()
        {
            return await IsLocalDrivingLicenseApplicationHasLicenseAsync(this.LocalDrivingLicenseApplicationID, this.LicenseClassID);
        }

        public static async Task<bool> HasActiveTestAppointmentAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return await clsLocalDrivingLicenseApplicationData.IsLocalDrivingLicenseApplicationHasActiveTestAppointmentAsync(LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public async Task<bool> HasActiveTestAppointmentAsync(clsTestType.enTestType TestTypeID)
        {
            return await HasActiveTestAppointmentAsync(this.LocalDrivingLicenseApplicationID, TestTypeID);
        }

        public async Task<clsLicense> IssueFirstTimeLocalLicenseAsync(int CreatedByUserID, string Notes)
        {
            clsLicense license = await clsLicense.IssueFirstTimeLocalLicenseAsync(this.LocalDrivingLicenseApplicationID, CreatedByUserID, Notes);
            if (license != null)
            {
                this.SetCompleteAsync();
                return license;
            }
            return null;
        }

        public static new async Task<enApplicationStatus> GetApplicationStatusAsync(int LocalDrivingLicenseApplicationID)
        {
            // Retrieve the integer status value from the Data Access Layer
            int statusValue = await clsLocalDrivingLicenseApplicationData.GetApplicationStatusAsync(LocalDrivingLicenseApplicationID);

            // Try to safely parse the integer value into the corresponding enum constant
            if (Enum.TryParse(statusValue.ToString(), out enApplicationStatus status))
            {
                return status;
            }

            // Return the default fallback value if the parsing process fails
            return enApplicationStatus.None;
        }

        public static async Task<bool> DoesAttendTestTypeAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return await clsLocalDrivingLicenseApplicationData.DoesAttendTestTypeAsync(LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public async Task<bool> DoesAttendTestTypeAsync(clsTestType.enTestType TestTypeID)
        {
            return await DoesAttendTestTypeAsync(this.LocalDrivingLicenseApplicationID, TestTypeID);
        }


    }
}
