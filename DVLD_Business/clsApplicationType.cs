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
        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public int ApplicationTypeID { get; private set; }
        public string ApplicationTypeTitle { get; set; }
        public decimal ApplicationTypeFees { get; set; }

        private clsApplicationType()
        {
            ApplicationTypeID = -1;
            ApplicationTypeFees = 0.0m;
            ApplicationTypeTitle = string.Empty;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                 {enMode.AddNew,_AddNewApplicationTypeAsync},
                 {enMode.Update,_UpdateApplicationTypeAsync}
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

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                 {enMode.AddNew,_AddNewApplicationTypeAsync},
                 {enMode.Update,_UpdateApplicationTypeAsync}
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewApplicationTypeAsync()
        {
            this.ApplicationTypeID = await clsApplicationTypeData.AddNewApplicationTypeAsync(ApplicationTypeTitle, ApplicationTypeFees);

            if (ApplicationTypeID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private async Task<bool> _UpdateApplicationTypeAsync()
        {
            return await clsApplicationTypeData.UpdateApplicationTypeAsync(ApplicationTypeID, ApplicationTypeTitle, ApplicationTypeFees);
        }

        public static async Task<clsApplicationType> FindAsync(int ApplicationTypeID)
        {
            var ApplicationTypeInfo = await clsApplicationTypeData.GetApplicationTypeInfoByIDAsync(ApplicationTypeID);

            if (ApplicationTypeInfo.IsFound)
            {
                return new clsApplicationType(ApplicationTypeID, ApplicationTypeInfo.ApplicationTypeTitle, ApplicationTypeInfo.ApplicationFees);
            }
            else
            {
                return null;
            }
        }

        public static async Task<DataTable> GetAllApplicationTypesAsync()
        {
            return await clsApplicationTypeData.GetAllApplicationTypesAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }

    }
}
