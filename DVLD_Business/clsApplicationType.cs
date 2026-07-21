using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsApplicationType
    {


        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; }
        private readonly Dictionary<enMode, Func<bool>> _saveDictionary;

        public int ApplicationTypeID { get; private set; }
        public string ApplicationTypeTitle { get; set; }
        public decimal ApplicationTypeFees { get; set; }

        public clsApplicationType()
        {
            ApplicationTypeID = -1;
            ApplicationTypeFees = 0.0m;
            ApplicationTypeTitle = string.Empty;

            _saveDictionary = new Dictionary<enMode, Func<bool>>
            {
                 {enMode.AddNew,_AddNewApplicationType},
                 {enMode.Update,_UpdateApplicationType}
            };

            Mode = enMode.AddNew;
        }



        // New overload that sets ApplicationTypeID so instances returned from Find have correct ID
        private clsApplicationType(int ApplicationTypeID,
            string ApplicationTypeTitle, decimal ApplicationTypeFees)
        {
            this.ApplicationTypeID = ApplicationTypeID;
            this.ApplicationTypeTitle = ApplicationTypeTitle;
            this.ApplicationTypeFees = ApplicationTypeFees;

            _saveDictionary = new Dictionary<enMode, Func<bool>>
            {
                 {enMode.AddNew,_AddNewApplicationType},
                 {enMode.Update,_UpdateApplicationType}
            };

            Mode = enMode.Update;
        }

        private bool _AddNewApplicationType()
        {
            this.ApplicationTypeID = clsApplicationTypeData.AddNewApplicationType(ApplicationTypeTitle, ApplicationTypeFees);

            if (ApplicationTypeID != -1)
            {
                Mode = enMode.AddNew;
                return true;
            }
            return false;
        }

        private bool _UpdateApplicationType()
        {
            return clsApplicationTypeData.UpdateApplicationType(ApplicationTypeID, ApplicationTypeTitle, ApplicationTypeFees);
        }

        public static clsApplicationType Find(int ApplicationTypeID)
        {
            string ApplicationTypeTitle = string.Empty;
            decimal ApplicationTypeFees = 0.0m;

            bool found = clsApplicationTypeData.GetApplicationTypeInfoByID(ApplicationTypeID, ref ApplicationTypeTitle, ref ApplicationTypeFees);

            if (found)
            {
                return new clsApplicationType(ApplicationTypeID, ApplicationTypeTitle, ApplicationTypeFees);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllApplicationTypes()
        {
            return clsApplicationTypeData.GetAllApplicationTypes();
        }

        public bool Save()
        {
            return _saveDictionary[this.Mode]();
        }

    }
}
