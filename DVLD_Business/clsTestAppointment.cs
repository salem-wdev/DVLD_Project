using DVLD_DataAccess;
using DVLD_Shared.Utilities;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsTestAppointment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public int TestAppointmentID { private set; get; }
        public clsTestType.enTestType TestTypeID { private set; get; }
        public int LocalDrivingLicenseApplicationID { private set; get; }

        private DateTime _AppointmentDate;
        public DateTime AppointmentDate
        {
            get
            {
                return _AppointmentDate;
            }
            set
            {
                if (value >= clsDateTime.GetCurrentDateTime())
                {
                    if (IsLocked == false)
                        _AppointmentDate = value;
                }
            }
        }
        public float PaidFees { private set; get; } = 0.0f;
        public int CreatedByUserID { private set; get; }
        public bool IsLocked { protected set; get; }
        public int RetakeTestApplicationID { private set; get; }

        private AsyncLazy<clsApplication> _RetakeTestAppInfo = null;
        public Task<clsApplication>RetakeTestAppInfo
        {
            get
            {
                if (_RetakeTestAppInfo == null && RetakeTestApplicationID != -1)
                {
                    _RetakeTestAppInfo = new AsyncLazy<clsApplication>(async () =>
                    {
                        return await clsApplication.FindAsync(this.RetakeTestApplicationID);
                    });
                }
                return _RetakeTestAppInfo.GetValueAsync();
            }
        }

        public async Task<int> TestIDAsync()
        {
           return await _GetTestIDAsync(this.TestAppointmentID);
        }

        protected clsTestAppointment(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)

        {
            this.TestAppointmentID = -1;
            this.TestTypeID = TestTypeID;
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this._AppointmentDate = AppointmentDate;
            this.PaidFees = 0;
            this.IsLocked = false;
            this.CreatedByUserID = CreatedByUserID;
            this.RetakeTestApplicationID = -1;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew, _AddNewTestAppointmentAsync },
                {enMode.Update, _UpdateTestAppointmentAsync },
            };

            Mode = enMode.AddNew;

        }

        protected clsTestAppointment(int TestAppointmentID, clsTestType.enTestType TestTypeID,
           int LocalDrivingLicenseApplicationID, DateTime AppointmentDate, float PaidFees,
           int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)

        {
            this.TestAppointmentID = TestAppointmentID;
            this.TestTypeID = TestTypeID;
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this._AppointmentDate = AppointmentDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsLocked = IsLocked;
            this.RetakeTestApplicationID = RetakeTestApplicationID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew, _AddNewTestAppointmentAsync },
                {enMode.Update, _UpdateTestAppointmentAsync },
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewTestAppointmentAsync()
        {
            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = await clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseIDAsync(LocalDrivingLicenseApplicationID);
            if (LocalDrivingLicenseApplication.ApplicationStatus != clsApplication.enApplicationStatus.New)
            {
                return false;
            }

            //call DataAccess Layer 

            this.TestAppointmentID = await clsTestAppointmentData.AddNewTestAppointmentAsync((int)this.TestTypeID, this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate, this.PaidFees, this.CreatedByUserID, this.RetakeTestApplicationID);

            if (this.TestAppointmentID != -1)
            {
                Mode = enMode.Update;
                return true;
            }

            if (this.RetakeTestApplicationID != -1)
            {
                // Delete the retake test application if the test appointment save fails.
                await clsApplication.DeleteAsync(this.RetakeTestApplicationID);
            }
            return false;
        }

        private async Task<bool> _UpdateTestAppointmentAsync()
        {
            if (this._AppointmentDate < clsDateTime.GetCurrentDateTime())
            {
                this.IsLocked = true;
                return false;
            }

            if (await clsTestAppointmentData.GetIsAppointmentLockedByIDAsync(this.TestAppointmentID))
            {
                return false;
            }

            //call DataAccess Layer 

            return await clsTestAppointmentData.UpdateTestAppointmentAsync(this.TestAppointmentID, (int)this.TestTypeID, this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate, this.PaidFees, this.CreatedByUserID, this.IsLocked, this.RetakeTestApplicationID);
        }

        public static async Task<clsTestAppointment> FindAsync(int TestAppointmentID)
        {
           
            var result = await clsTestAppointmentData.GetTestAppointmentInfoByIDAsync(TestAppointmentID);

            if (result.isFound)
            {
                if (result.AppointmentDate < clsDateTime.GetCurrentDateTime())
                {
                    result.IsLocked = true;
                }

                return new clsTestAppointment(TestAppointmentID, (clsTestType.enTestType)result.TestTypeID, result.LocalDrivingLicenseApplicationID,
          result.AppointmentDate, result.PaidFees, result.CreatedByUserID, result.IsLocked, result.RetakeTestApplicationID);
            }
            else
                return null;

        }

        public static async Task<clsTestAppointment> FindByLocalDrivingLicenseApplicationIDAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            var result = await clsTestAppointmentData.GetTestAppointmentInfoByLocalDrivingLicenseApplicationIDAsync(LocalDrivingLicenseApplicationID, (int)TestTypeID);

            if(result.isFound)
            {
                if (result.AppointmentDate < clsDateTime.GetCurrentDateTime())
                {
                    result.IsLocked = true;
                }

                return new clsTestAppointment(result.TestAppointmentID, (clsTestType.enTestType)TestTypeID, LocalDrivingLicenseApplicationID,
          result.AppointmentDate, result.PaidFees, result.CreatedByUserID, result.IsLocked, result.RetakeTestApplicationID);
            }
            else
                return null;

        }


        public static async Task<clsTestAppointment> GetLastTestAppointmentAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {

            var result = await clsTestAppointmentData.GetLastTestAppointmentAsync(LocalDrivingLicenseApplicationID, (int)TestTypeID);

            if(result.isFound)
            {
                if (result.AppointmentDate < clsDateTime.GetCurrentDateTime())
                {
                    result.IsLocked = true;
                }

                return new clsTestAppointment(result.TestAppointmentID, TestTypeID, LocalDrivingLicenseApplicationID,
         result.AppointmentDate, result.PaidFees, result.CreatedByUserID, result.IsLocked, result.RetakeTestApplicationID);
            }
            else
                return null;

        }

        public static async Task<DataTable> GetAllTestAppointmentsAsync()
        {
            return await clsTestAppointmentData.GetAllTestAppointmentsAsync();

        }

        public async Task<DataTable> GetApplicationTestAppointmentsPerTestTypeAsync(clsTestType.enTestType TestTypeID)
        {
            return await clsTestAppointmentData.GetApplicationTestAppointmentsPerTestTypeAsync(this.LocalDrivingLicenseApplicationID, (int)TestTypeID);

        }

        public static async Task<DataTable> GetApplicationTestAppointmentsPerTestTypeAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return await clsTestAppointmentData.GetApplicationTestAppointmentsPerTestTypeAsync(LocalDrivingLicenseApplicationID, (int)TestTypeID);

        }

        public async Task<bool> SaveAsync()
        {
            if (RetakeTestAppInfo != null)
            {
                RetakeTestApplicationID = (await RetakeTestAppInfo).ApplicationID;
            }

            return await _saveDictionary[this.Mode]();
        }

        private static async Task<int> _GetTestIDAsync(int testAppointmentID)
        {
            return await clsTestAppointmentData.GetTestIDAsync(testAppointmentID);
        }

        public static async Task<int> GetTestIDAsync(int testAppointmentID)
        {
            return await _GetTestIDAsync(testAppointmentID);
        }

        private static async Task<float> _CalculateFeesAsync(clsApplication RetakeTestAppInfo, clsTestType.enTestType TestTypeID)
        {
            float paidFees = 0.0f;
            if (RetakeTestAppInfo != null)
            {
                RetakeTestAppInfo.PaidFees = (await clsApplicationType.FindAsync((int)clsApplication.enApplicationType.RetakeTest)).ApplicationTypeFees;
                paidFees += (float)RetakeTestAppInfo.PaidFees;
            }

            paidFees += (float)(await clsTestType.FindAsync(TestTypeID)).TestTypeFees;

            return paidFees;
        }

        private static async Task<bool> _IsNextTestAppointmentScheduledAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // If the test type is less than StreetTest, then check if the next test appointment is already scheduled.
            if (TestTypeID == clsTestType.enTestType.StreetTest)
            {
                return false;
            }

            // Check if the next test appointment is already scheduled.
            return await clsTestAppointmentData.GetIsAppointmentexistsAsync((int)TestTypeID + 1, LocalDrivingLicenseApplicationID);
        }

        public static async Task<bool> IsPreviousTestAppointmentLockedAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return await _IsPreviousTestAppointmentLockedAsync(LocalDrivingLicenseApplicationID, TestTypeID);
        }

        public static async Task<bool> IsTestAppointmentLockedAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return await _IsTestAppointmentLockedAsync(LocalDrivingLicenseApplicationID, TestTypeID);
        }

        private static async Task<bool> _IsTestAppointmentLockedAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return await clsTestAppointmentData.GetIsAppointmentLockedAsync((int)TestTypeID, LocalDrivingLicenseApplicationID);
        }

        private static async Task<bool> _IsPreviousTestAppointmentLockedAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // If the test type is greater than VisionTest, then check if the previous test appointment is locked.
            if (TestTypeID == clsTestType.enTestType.VisionTest)
            {
                return true;
            }
            // Check if the previous test appointment is locked.
            return await _IsTestAppointmentLockedAsync(LocalDrivingLicenseApplicationID, TestTypeID - 1);

        }

        private static async Task<bool> _DoesHaveActiveTestAppointmentAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // Check if there is an active test appointment for the given test type and application ID.
            return await clsTestAppointmentData.DoesHaveAnActiveAppointmentAsync((int)TestTypeID, LocalDrivingLicenseApplicationID);
        }

        private static async Task<bool> _IsTestAppointmentInTheRightOrderAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // Check if the test appointment is in the right order based on the test type.

            // If there is active test appointment for the current test type, then it is not in the right order.
            if (await _DoesHaveActiveTestAppointmentAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }


            // If the previous test appointment is not locked, then it is not in the right order.
            if (!await _IsPreviousTestAppointmentLockedAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }



            // If the next test appointment is already scheduled, then it is not in the right order.
            if (await _IsNextTestAppointmentScheduledAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the test appointment follows the correct structural sequence (Vision -> Theory -> Practical).
        /// </summary>
        /// <param name="LocalDrivingLicenseApplicationID">The ID of the local driving license application.</param>
        /// <param name="TestTypeID">The type of the test to validate its sequence.</param>
        /// <returns>Returns <c>true</c> if the sequence is correct or if the appointment already exists; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>NOTE FOR UI DEVELOPER:</para>
        /// This function only validates the structural order and sequence of appointments. 
        /// It DOES NOT check whether the applicant passed or failed the previous test.
        /// </remarks>
        public static async Task<bool> IsTestAppointmentInTheRightOrderAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // Check if the test appointment is in the right order based on the test type.
            // result not important here, only checking the sequence.

            if (TestTypeID == clsTestType.enTestType.None)
            {
                return false;
            }

            // If the appointment already exists, then it is in the right order.
            if (await clsTestAppointmentData.GetIsAppointmentexistsAsync((int)TestTypeID, LocalDrivingLicenseApplicationID))
            {
                return true;
            }

            if (!await _IsTestAppointmentInTheRightOrderAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }


            return true;

        }

        private static async Task<clsTestAppointment> _GetReadyObjectAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)
        {
            clsTestAppointment testAppointment;
            clsApplication retakeTestApp = null;

            testAppointment = new clsTestAppointment(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);
            if (await clsLocalDrivingLicenseApplication.DoesAttendTestTypeAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                if (await clsLocalDrivingLicenseApplication.DosPassTestAsync(LocalDrivingLicenseApplicationID, TestTypeID))
                {
                    return null;
                }

                var applicantPersonID = (await clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseIDAsync(LocalDrivingLicenseApplicationID))?.ApplicantPersonID ?? -1;
                if (applicantPersonID <= 0)
                {
                    return null;
                }

                retakeTestApp = await _GetNewReTakeTestObjAsync(testAppointment.CreatedByUserID, applicantPersonID);
                testAppointment._RetakeTestAppInfo = new AsyncLazy<clsApplication>(() => Task.FromResult(retakeTestApp));

            }
            testAppointment.PaidFees = await _CalculateFeesAsync(retakeTestApp, TestTypeID);
            return testAppointment;
        }

        private static async Task<bool> _CanBookAppointmentAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)
        {

            if (await clsLocalDrivingLicenseApplication.GetApplicationStatusAsync(LocalDrivingLicenseApplicationID) != clsApplication.enApplicationStatus.New)
            {
                return false;
            }

            if (AppointmentDate < clsDateTime.GetCurrentDateTime())
            {
                return false;
            }

            if (TestTypeID == clsTestType.enTestType.None)
            {
                return false;
            }

            if (!await _IsTestAppointmentInTheRightOrderAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }

            if (TestTypeID == clsTestType.enTestType.VisionTest)
            {
                return true;
            }

            // check is passed preveous test.
            if (!await clsLocalDrivingLicenseApplication.DosPassTestAsync(LocalDrivingLicenseApplicationID, TestTypeID - 1))
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> CanBookAppointmentAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)
        {
            return await _CanBookAppointmentAsync(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);
        }

        private static async Task<clsTestAppointment> _GetNewTestAppointmentObjectAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)
        {
            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication = await clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseIDAsync(LocalDrivingLicenseApplicationID);
            if(LocalDrivingLicenseApplication.ApplicationStatus != clsApplication.enApplicationStatus.New)
            {
                return null;
            }

            if (AppointmentDate < clsDateTime.GetCurrentDateTime())
            {
                return null;
            }

            if (TestTypeID == clsTestType.enTestType.None)
            {
                return null;
            }

            if (!await _IsTestAppointmentInTheRightOrderAsync(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return null;
            }

            if (TestTypeID == clsTestType.enTestType.VisionTest)
            {
                return await _GetReadyObjectAsync(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);
            }

            // check is passed preveous test.
            if (!await clsLocalDrivingLicenseApplication.DosPassTestAsync(LocalDrivingLicenseApplicationID, TestTypeID - 1))
            {
                return null;
            }



            return await _GetReadyObjectAsync(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);
        }

        public static async Task<clsTestAppointment> CreateNewTestAppointmentAsync(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)
        {
            clsTestAppointment appointment = await _GetNewTestAppointmentObjectAsync(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);

            if (appointment != null)
            {
                if(await appointment.SaveAsync())
                {
                    return appointment;
                }
            }
            return null;
        }

        private static async Task<clsApplication> _GetNewReTakeTestObjAsync(int CreatedByUserID, int ApplicantPersonID)
        {
            return await clsApplication.GetNewApplicationAsync(CreatedByUserID, ApplicantPersonID, clsApplication.enApplicationType.RetakeTest);
        }

        public static async Task<bool> LockExpiredTestAppointmentsAsync()
        {
            return await clsTestAppointmentData.LockExpiredTestAppointmentsAsync();
        }
    }
}
