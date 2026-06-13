using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsTestAppointment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

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
                if (value => clsUtilData.GetServerDate())
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

        private clsApplication _RetakeTestAppInfo = null;
        public clsApplication RetakeTestAppInfo
        {
            get
            { 
                if (_RetakeTestAppInfo == null && RetakeTestApplicationID != -1)
                {
                    _RetakeTestAppInfo = clsApplication.Find(RetakeTestApplicationID);
                }
                return _RetakeTestAppInfo;
            }
        }

        public int TestID
        {
            get { return _GetTestID(); }

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
            Mode = enMode.Update;
        }

        private bool _AddNewTestAppointment()
        {
            //call DataAccess Layer 

            this.TestAppointmentID = clsTestAppointmentData.AddNewTestAppointment((int)this.TestTypeID, this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate, this.PaidFees, this.CreatedByUserID, this.RetakeTestApplicationID);

            return (this.TestAppointmentID != -1);
        }

        private bool _UpdateTestAppointment()
        {
            //call DataAccess Layer 

            return clsTestAppointmentData.UpdateTestAppointment(this.TestAppointmentID, (int)this.TestTypeID, this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate, this.PaidFees, this.CreatedByUserID, this.IsLocked, this.RetakeTestApplicationID);
        }

        public static clsTestAppointment Find(int TestAppointmentID)
        {
            int TestTypeID = 1; int LocalDrivingLicenseApplicationID = -1;
            DateTime AppointmentDate = DateTime.Now; float PaidFees = 0;
            int CreatedByUserID = -1; bool IsLocked = false; int RetakeTestApplicationID = -1;

            if (clsTestAppointmentData.GetTestAppointmentInfoByID(TestAppointmentID, ref TestTypeID, ref LocalDrivingLicenseApplicationID,
            ref AppointmentDate, ref PaidFees, ref CreatedByUserID, ref IsLocked, ref RetakeTestApplicationID))

            {
                if (AppointmentDate < clsUtilData.GetServerDate())
                {
                    IsLocked = true;
                }

                return new clsTestAppointment(TestAppointmentID, (clsTestType.enTestType)TestTypeID, LocalDrivingLicenseApplicationID,
          AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
            }
            else
                return null;

        }

        public static clsTestAppointment FindByLocalDrivingLicenseApplicationID(int LocalDrivingLicenseApplicationID,clsTestType.enTestType TestTypeID)
        {
            int TestAppointmentID = -1; 
            DateTime AppointmentDate = DateTime.Now; float PaidFees = 0;
            int CreatedByUserID = -1; bool IsLocked = false; int RetakeTestApplicationID = -1;

            if (clsTestAppointmentData.GetTestAppointmentInfoByLocalDrivingLicenseApplicationID(LocalDrivingLicenseApplicationID, (int)TestTypeID, ref TestAppointmentID,
            ref AppointmentDate, ref PaidFees, ref CreatedByUserID, ref IsLocked, ref RetakeTestApplicationID))

            {
                if (AppointmentDate < clsUtilData.GetServerDate())
                {
                    IsLocked = true;
                }

                return new clsTestAppointment(TestAppointmentID, (clsTestType.enTestType)TestTypeID, LocalDrivingLicenseApplicationID,
          AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
            }
            else
                return null;

        }


        public static clsTestAppointment GetLastTestAppointment(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            int TestAppointmentID = -1;
            DateTime AppointmentDate = DateTime.Now; float PaidFees = 0;
            int CreatedByUserID = -1; bool IsLocked = false; int RetakeTestApplicationID = -1;

            if (clsTestAppointmentData.GetLastTestAppointment(LocalDrivingLicenseApplicationID, (int)TestTypeID,
                ref TestAppointmentID, ref AppointmentDate, ref PaidFees, ref CreatedByUserID, ref IsLocked, ref RetakeTestApplicationID))

            {
                if (AppointmentDate < clsUtilData.GetServerDate())
                {
                    IsLocked = true;
                }

                return new clsTestAppointment(TestAppointmentID, TestTypeID, LocalDrivingLicenseApplicationID,
         AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
            }
            else
                return null;

        }

        public static DataTable GetAllTestAppointments()
        {
            return clsTestAppointmentData.GetAllTestAppointments();

        }

        public DataTable GetApplicationTestAppointmentsPerTestType(clsTestType.enTestType TestTypeID)
        {
            return clsTestAppointmentData.GetApplicationTestAppointmentsPerTestType(this.LocalDrivingLicenseApplicationID, (int)TestTypeID);

        }

        public static DataTable GetApplicationTestAppointmentsPerTestType(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return clsTestAppointmentData.GetApplicationTestAppointmentsPerTestType(LocalDrivingLicenseApplicationID, (int)TestTypeID);

        }

        public bool Save()
        {


            if (RetakeTestAppInfo != null)
            {
                if (!RetakeTestAppInfo.Save())
                {
                    return false;
                }

                RetakeTestApplicationID = RetakeTestAppInfo.ApplicationID;
            }

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTestAppointment())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        if (this.RetakeTestApplicationID != -1)
                        {
                            // Delete the retake test application if the test appointment save fails.
                            clsApplication.Delete(this.RetakeTestApplicationID);
                        }
                        return false;
                    }

                case enMode.Update:

                    if (clsTestAppointmentData.GetIsAppointmentLockedByID(this.TestAppointmentID))
                    {
                        return false;
                    }
                    return _UpdateTestAppointment();

            }

            return false;
        }

        private int _GetTestID()
        {
            return clsTestAppointmentData.GetTestID(TestAppointmentID);
        }

        private static float _CalculateFees(clsApplication RetakeTestAppInfo, clsTestType.enTestType TestTypeID)
        {
            float paidFees = 0.0f;
            if (RetakeTestAppInfo != null)
            {
                RetakeTestAppInfo.PaidFees = clsApplicationType.Find((int)clsApplication.enApplicationType.RetakeTest).ApplicationTypeFees;
                paidFees += (float)RetakeTestAppInfo.PaidFees;
            }

            paidFees += (float)clsTestType.Find(TestTypeID).TestTypeFees;

            return paidFees;
        }

        private static bool _IsNextTestAppointmentScheduled(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // If the test type is less than StreetTest, then check if the next test appointment is already scheduled.
            if (TestTypeID == clsTestType.enTestType.StreetTest)
            {
                return false;
            }

            // Check if the next test appointment is already scheduled.
            return clsTestAppointmentData.GetIsAppointmentexists((int)TestTypeID + 1, LocalDrivingLicenseApplicationID);
        }

        public static bool IsPreviousTestAppointmentLocked(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return _IsPreviousTestAppointmentLocked(LocalDrivingLicenseApplicationID, TestTypeID);
        }

        public static bool IsTestAppointmentLocked(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return _IsTestAppointmentLocked(LocalDrivingLicenseApplicationID, TestTypeID);
        }

        private static bool _IsTestAppointmentLocked(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return clsTestAppointmentData.GetIsAppointmentLocked((int)TestTypeID, LocalDrivingLicenseApplicationID);
        }

        private static bool _IsPreviousTestAppointmentLocked(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // If the test type is greater than VisionTest, then check if the previous test appointment is locked.
            if (TestTypeID == clsTestType.enTestType.VisionTest)
            { 
                return true;
            }
            // Check if the previous test appointment is locked.
            return _IsTestAppointmentLocked(LocalDrivingLicenseApplicationID, TestTypeID - 1);

        }

        private static bool _DoesHaveActiveTestAppointment(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // Check if there is an active test appointment for the given test type and application ID.
            return clsTestAppointmentData.DoesHaveAnActiveAppointment((int)TestTypeID, LocalDrivingLicenseApplicationID);
        }

        private static bool _IsTestAppointmentInTheRightOrder(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // Check if the test appointment is in the right order based on the test type.

            // If there is active test appointment for the current test type, then it is not in the right order.
            if (_DoesHaveActiveTestAppointment(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }


            // If the previous test appointment is not locked, then it is not in the right order.
            if (!_IsPreviousTestAppointmentLocked(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }



            // If the next test appointment is already scheduled, then it is not in the right order.
            if (_IsNextTestAppointmentScheduled(LocalDrivingLicenseApplicationID, TestTypeID))
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
        public static bool IsTestAppointmentInTheRightOrder(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            // Check if the test appointment is in the right order based on the test type.
            // result not important here, only checking the sequence.

            if (TestTypeID == clsTestType.enTestType.None)
            {
                return false;
            }

            // If the appointment already exists, then it is in the right order.
            if (clsTestAppointmentData.GetIsAppointmentexists((int)TestTypeID, LocalDrivingLicenseApplicationID))
            {
                return true;
            }

            if (!_IsTestAppointmentInTheRightOrder(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return false;
            }

            
                return true;
            
        }

        private static clsTestAppointment _GetReadyObject(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)
        {
            clsTestAppointment testAppointment;

            testAppointment = new clsTestAppointment(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);
            if (clsTestAppointmentData.GetIsAppointmentexists((int)TestTypeID, LocalDrivingLicenseApplicationID))
            {
                testAppointment._RetakeTestAppInfo = _GetNewReTakeTestObj(testAppointment.CreatedByUserID, clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID).ApplicantPersonID);
            }
            testAppointment.PaidFees = _CalculateFees(testAppointment._RetakeTestAppInfo, TestTypeID);
            return testAppointment;
        }

        public static clsTestAppointment GetNewTestAppointmentObject(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID, int CreatedByUserID, DateTime AppointmentDate)
        {
            clsTestAppointment testAppointment;

            if(TestTypeID == clsTestType.enTestType.None)
            {
                return null;
            }

            if (!_IsTestAppointmentInTheRightOrder(LocalDrivingLicenseApplicationID, TestTypeID))
            {
                return null;
            }

            if (TestTypeID == clsTestType.enTestType.VisionTest)
            {
                return _GetReadyObject(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);
            }

            // check is passed preveous test.
            if (!clsLocalDrivingLicenseApplication.DosPassTest(LocalDrivingLicenseApplicationID, TestTypeID - 1))
            {
                return null;
            }



            return _GetReadyObject(LocalDrivingLicenseApplicationID, TestTypeID, CreatedByUserID, AppointmentDate);
        }

        private static clsApplication _GetNewReTakeTestObj(int CreatedByUserID, int ApplicantPersonID)
        {
            return clsApplication.GetNewApplicationobject(CreatedByUserID, ApplicantPersonID, clsApplication.enApplicationType.RetakeTest);
        }

    }
}
