using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsDetainedLicense
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { private set; get; } = enMode.AddNew;


        public int DetainID { private set; get; }
        public int LicenseID { private set; get; }
        public DateTime DetainDate { private set; get; }

        public float FineFees { private set; get; }
        public int CreatedByUserID { private set; get; }

        private clsUser _CreatedByUserInfo = null;
        public clsUser CreatedByUserInfo
        {
            get
            {
                if (_CreatedByUserInfo == null && CreatedByUserID != -1)
                {
                    _CreatedByUserInfo = clsUser.Find(this.CreatedByUserID);
                }
                return _CreatedByUserInfo;
            }
        }

        public bool IsReleased { private set; get; }
        public DateTime ReleaseDate { private set; get; }
        public int ReleasedByUserID { private set; get; }

        private clsUser _ReleasedByUserInfo = null;
        public clsUser ReleasedByUserInfo
        {
            get
            {
                if (_ReleasedByUserInfo == null && ReleasedByUserID != -1)
                {
                    _ReleasedByUserInfo = clsUser.Find(this.ReleasedByUserID);
                }
                return _ReleasedByUserInfo;
            }
        }

        public int ReleaseApplicationID { private set; get; }

        private clsDetainedLicense(int LicenseID, DateTime DetainDate, float FineFees, int CreatedByUserID)

        {
            this.DetainID = -1;
            this.LicenseID = LicenseID;
            this.DetainDate = DetainDate;
            this.FineFees = FineFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsReleased = false;
            this.ReleaseDate = DateTime.MaxValue;
            this.ReleasedByUserID = 0;
            this.ReleaseApplicationID = -1;



            Mode = enMode.AddNew;

        }

        private clsDetainedLicense(int DetainID,
            int LicenseID, DateTime DetainDate,
            float FineFees, int CreatedByUserID,
            bool IsReleased, DateTime ReleaseDate,
            int ReleasedByUserID, int ReleaseApplicationID)

        {
            this.DetainID = DetainID;
            this.LicenseID = LicenseID;
            this.DetainDate = DetainDate;
            this.FineFees = FineFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsReleased = IsReleased;
            this.ReleaseDate = ReleaseDate;
            this.ReleasedByUserID = ReleasedByUserID;
            this.ReleaseApplicationID = ReleaseApplicationID;
            Mode = enMode.Update;
        }

        private bool _AddNewDetainedLicense()
        {
            //call DataAccess Layer 

            this.DetainID = clsDetainedLicenseData.AddNewDetainedLicense(
                this.LicenseID, this.DetainDate, this.FineFees, this.CreatedByUserID);

            return (this.DetainID != -1);
        }

        private bool _UpdateDetainedLicense()
        {
            //call DataAccess Layer 

            return clsDetainedLicenseData.UpdateDetainedLicense(
                this.DetainID, this.LicenseID, this.DetainDate, this.FineFees, this.CreatedByUserID);
        }

        public static clsDetainedLicense Find(int DetainID)
        {
            int LicenseID = -1; DateTime DetainDate = DateTime.Now;
            float FineFees = 0; int CreatedByUserID = -1;
            bool IsReleased = false; DateTime ReleaseDate = DateTime.MaxValue;
            int ReleasedByUserID = -1; int ReleaseApplicationID = -1;

            if (clsDetainedLicenseData.GetDetainedLicenseInfoByID(DetainID,
            ref LicenseID, ref DetainDate,
            ref FineFees, ref CreatedByUserID,
            ref IsReleased, ref ReleaseDate,
            ref ReleasedByUserID, ref ReleaseApplicationID))

                return new clsDetainedLicense(DetainID,
                     LicenseID, DetainDate,
                     FineFees, CreatedByUserID,
                     IsReleased, ReleaseDate,
                     ReleasedByUserID, ReleaseApplicationID);
            else
                return null;

        }

        public static DataTable GetAllDetainedLicenses()
        {
            return clsDetainedLicenseData.GetAllDetainedLicenses();

        }

        public static clsDetainedLicense FindByLicenseID(int LicenseID)
        {
            int DetainID = -1; DateTime DetainDate = DateTime.Now;
            float FineFees = 0; int CreatedByUserID = -1;
            bool IsReleased = false; DateTime ReleaseDate = DateTime.MaxValue;
            int ReleasedByUserID = -1; int ReleaseApplicationID = -1;

            if (clsDetainedLicenseData.GetDetainedLicenseInfoByLicenseID(LicenseID,
            ref DetainID, ref DetainDate,
            ref FineFees, ref CreatedByUserID,
            ref IsReleased, ref ReleaseDate,
            ref ReleasedByUserID, ref ReleaseApplicationID))

                return new clsDetainedLicense(DetainID,
                     LicenseID, DetainDate,
                     FineFees, CreatedByUserID,
                     IsReleased, ReleaseDate,
                     ReleasedByUserID, ReleaseApplicationID);
            else
                return null;

        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDetainedLicense())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateDetainedLicense();

            }

            return false;
        }

        public static bool IsLicenseDetained(int LicenseID)
        {
            return clsDetainedLicenseData.IsLicenseDetained(LicenseID);
        }

        public bool ReleaseDetainedLicense(int ReleasedByUserID)
        {
            if (!clsUser.IsUserExists(ReleasedByUserID)
                || !IsLicenseDetained(this.LicenseID))
            {
                return false;
            }

            clsLicense license = clsLicense.Find(this.LicenseID);
            if (license == null || license.DriverInfo == null)
            {
                return false;
            }

            clsApplication ReleaseApplication
                = clsApplication.GetNewApplication(ReleasedByUserID,
                license.DriverInfo.PersonID, clsApplication.enApplicationType.ReleaseDetainedDrivingLicense);

            if (ReleaseApplication == null)
            {
                return false;
            }

            return clsDetainedLicenseData.ReleaseDetainedLicense(this.DetainID,
                   ReleasedByUserID, ReleaseApplication.ApplicationID);
        }

        private static clsDetainedLicense _CreateNewDetainedLicense(int LicenseID, float FineFees, int CreatedByUserID)
        {
            if(!clsLicense.IsLicenseActive(LicenseID) || !clsUser.IsUserExists(CreatedByUserID))
            {
                return null;
            }

            if(IsLicenseDetained(LicenseID))
            {
                return null;
            }
            DateTime CurrentDate = clsBusinessSettings.GetServerDateTime();
            if(CurrentDate == DateTime.MinValue)
            {
                return null;
            }

            return new clsDetainedLicense(LicenseID, CurrentDate, FineFees, CreatedByUserID);
        }

        public static clsDetainedLicense DetainedLicense(int LicenseID, float FineFees, int CreatedByUserID)
        {
            clsDetainedLicense license = _CreateNewDetainedLicense(LicenseID, FineFees, CreatedByUserID);
            if (license != null)
            {
                if(license.Save())
                {
                    return license;
                }
            }
            return null;
        }

    }
}
