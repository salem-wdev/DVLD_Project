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

        public int TestAppointmentID { set; get; }
        public clsTestType.enTestType TestTypeID { set; get; }
        public int LocalDrivingLicenseApplicationID { set; get; }
        public DateTime AppointmentDate { set; get; }
        public float PaidFees { set; get; }
        public int CreatedByUserID { set; get; }
        public bool IsLocked { set; get; }
        public int RetakeTestApplicationID { set; get; }
        public clsApplication RetakeTestAppInfo { set; get; }

        public int TestID
        {
            get { return _GetTestID(); }

        }

        protected clsTestAppointment()

        {
            this.TestAppointmentID = -1;
            this.TestTypeID = clsTestType.enTestType.VisionTest;
            this.AppointmentDate = DateTime.Now;
            this.PaidFees = 0;
            this.IsLocked = false;
            this.CreatedByUserID = -1;
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
            this.AppointmentDate = AppointmentDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsLocked = IsLocked;
            this.RetakeTestApplicationID = RetakeTestApplicationID;
            this.RetakeTestAppInfo = clsApplication.Find(RetakeTestApplicationID);
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

                return new clsTestAppointment(TestAppointmentID, (clsTestType.enTestType)TestTypeID, LocalDrivingLicenseApplicationID,
             AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
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

                return new clsTestAppointment(TestAppointmentID, (clsTestType.enTestType)TestTypeID, LocalDrivingLicenseApplicationID,
             AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
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

                return new clsTestAppointment(TestAppointmentID, TestTypeID, LocalDrivingLicenseApplicationID,
             AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
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
                        return false;
                    }

                case enMode.Update:

                    return _UpdateTestAppointment();

            }

            return false;
        }

        private int _GetTestID()
        {
            return clsTestAppointmentData.GetTestID(TestAppointmentID);
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

        public static bool IsTestAppointmentInTheRightOrder(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            if (TestTypeID == clsTestType.enTestType.None)
            {
                return false;
            }

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

        public static clsTestAppointment GetNewTestAppointmentObject(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {

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
                return new clsTestAppointment();
            }

            // check is passed preveous test.
            if (!clsLocalDrivingLicenseApplication.DosPassTest(LocalDrivingLicenseApplicationID, TestTypeID-1))
            {
                return null;
            }


            return new clsTestAppointment();
        }

        public static clsApplication GetNewReTakeTestObj(int CreatedByUserID, int ApplicantPersonID)
        {
            return clsApplication.GetNewApplicationobject(CreatedByUserID, ApplicantPersonID, clsApplication.enApplicationType.RetakeTest);
        }

    }
}
