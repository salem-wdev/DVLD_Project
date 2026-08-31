using DVLD_DataAccess;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsTest
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public int TestID { private set; get; }
        public int TestAppointmentID { private set; get; }
        public bool TestResult { private set; get; }
        public string Notes { set; get; }
        public int CreatedByUserID { private set; get; }

        private AsyncLazy<clsTestAppointment> _TestAppointmentInfoLazy = null;
        public Task<clsTestAppointment> TestAppointmentInfoAsync
        {
            get
            {
                if (_TestAppointmentInfoLazy == null)
                {
                    _TestAppointmentInfoLazy = new AsyncLazy<clsTestAppointment>(async () =>
                    {
                        if (TestAppointmentID == -1)
                            return null;

                        return await clsTestAppointment.FindAsync(TestAppointmentID); });
                }
                return _TestAppointmentInfoLazy.GetValueAsync();
            }
        }

        private clsTest()

        {
            this.TestID = -1;
            this.TestAppointmentID = -1;
            this.TestResult = false;
            this.Notes = "";
            this.CreatedByUserID = -1;

            Mode = enMode.AddNew;

        }

        // To Get new constructor internaly
        private clsTest( int TestAppointmentID,
            bool TestResult, string Notes, int CreatedByUserID)
        {
            this.TestAppointmentID = TestAppointmentID;
            this.TestResult = TestResult;
            this.Notes = Notes;
            this.CreatedByUserID = CreatedByUserID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew, _AddNewTestAsync },
                {enMode.Update, _UpdateTestAsync },
            };

            Mode = enMode.AddNew;
        }

        private clsTest(int TestID, int TestAppointmentID,
            bool TestResult, string Notes, int CreatedByUserID)

        {
            this.TestID = TestID;
            this.TestAppointmentID = TestAppointmentID;
            this.TestResult = TestResult;
            this.Notes = Notes;
            this.CreatedByUserID = CreatedByUserID;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew, _AddNewTestAsync },
                {enMode.Update, _UpdateTestAsync },
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewTestAsync()
        {
            //call DataAccess Layer 

            this.TestID = await clsTestData.AddNewTestAsync(this.TestAppointmentID,
                this.TestResult, this.Notes, this.CreatedByUserID);


            if (this.TestID == -1) return false;

            clsTestAppointment appointment = await this.TestAppointmentInfoAsync;

            if (appointment?.RetakeTestAppInfo != null && TestResult == true)
            {
                clsApplication retakeTestAppInfo = await appointment.RetakeTestAppInfo;
                if (retakeTestAppInfo != null)
                    await retakeTestAppInfo.SetCompleteAsync();
            }
            Mode = enMode.Update;
            return true;
        }

        private async Task<bool> _UpdateTestAsync()
        {
            //call DataAccess Layer 

            return await clsTestData.UpdateTestAsync(this.TestID, this.TestAppointmentID,
                this.TestResult, this.Notes, this.CreatedByUserID);
        }

        public static async Task<clsTest> FindAsync(int TestID)
        {
            
            var result = await clsTestData.GetTestInfoByIDAsync(TestID);

            if(result.isFound)
                return new clsTest(TestID,
                        result.TestAppointmentID, result.TestResult,
                        result.Notes, result.CreatedByUserID);
            else
                return null;

        }

        public static async Task<clsTest> FindLastTestPerPersonAndLicenseClassAsync
            (int PersonID, int LicenseClassID, clsTestType.enTestType TestTypeID)
        {

            var result = await clsTestData.GetLastTestByPersonAndTestTypeAndLicenseClassAsync(PersonID, LicenseClassID, (int)TestTypeID);

            if(result.isFound)
                return new clsTest(result.TestID,
                        result.TestAppointmentID, result.TestResult,
                        result.Notes, result.CreatedByUserID);
            else
                return null;

        }

        public static async Task<DataTable> GetAllTestsAsync()
        {
            return await clsTestData.GetAllTestsAsync();

        }

        public async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }

        public static async Task<byte> GetPassedTestCountAsync(int LocalDrivingLicenseApplicationID)
        {
            return await clsTestData.GetPassedTestCountAsync(LocalDrivingLicenseApplicationID);
        }

        public static async Task<bool> PassedAllTestsAsync(int LocalDrivingLicenseApplicationID)
        {
            //if total passed test less than 3 it will return false otherwise will return true
            return await GetPassedTestCountAsync(LocalDrivingLicenseApplicationID) == 3;
        }

        public static async Task<bool> IsTestPassedAsync(int TestAppointmentID)
        {
            return await clsTestData.GetIsPassedTestByTestAppointmentIDAsync(TestAppointmentID);
        }

        public static async Task<clsTest> GetNewTestObjAsync(int TestAppointmentID, bool TestResult, string Notes, int CreatedByUserID)
        {
            // prevent creating a test object if the TestAppointmentID or CreatedByUserID is invalid,
            // or if the related TestAppointment is invalid or locked.
            if (CreatedByUserID <= 0 || TestAppointmentID <= 0)
            {
                return null;
            }

            clsTestAppointment TestAppointmentInfo = await clsTestAppointment.FindAsync(TestAppointmentID);

            if (TestAppointmentInfo == null || TestAppointmentInfo.TestTypeID == clsTestType.enTestType.None)
            {
                return null;
            }

            if (TestAppointmentInfo.IsLocked)
            {
                return null;
            }


            return new clsTest(TestAppointmentID, TestResult, Notes, CreatedByUserID);
        }

    }
}
