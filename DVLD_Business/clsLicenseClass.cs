using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsLicenseClass
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;


        public int LicenseClassID { private set; get; }
        public string ClassName { set; get; }
        public string ClassDescription { set; get; }
        public byte MinimumAllowedAge { private set; get; }
        public byte DefaultValidityLength { private set; get; }
        public float ClassFees { set; get; }

        private clsLicenseClass()

        {
            this.LicenseClassID = -1;
            this.ClassName = "";
            this.ClassDescription = "";
            this.MinimumAllowedAge = 18;
            this.DefaultValidityLength = 10;
            this.ClassFees = 0;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewLicenseClassAsync},
                {enMode.Update,_UpdateLicenseClassAsync}
            };

            Mode = enMode.AddNew;

        }

        private clsLicenseClass(int LicenseClassID, string ClassName,
            string ClassDescription,
            byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)

        {
            this.LicenseClassID = LicenseClassID;
            this.ClassName = ClassName;
            this.ClassDescription = ClassDescription;
            this.MinimumAllowedAge = MinimumAllowedAge;
            this.DefaultValidityLength = DefaultValidityLength;
            this.ClassFees = ClassFees;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew,_AddNewLicenseClassAsync},
                {enMode.Update,_UpdateLicenseClassAsync}
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewLicenseClassAsync()
        {
            //call DataAccess Layer 

            this.LicenseClassID = await clsLicenseClassData.AddNewLicenseClassAsync(this.ClassName, this.ClassDescription,
                this.MinimumAllowedAge, this.DefaultValidityLength, this.ClassFees);


            if (this.LicenseClassID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private async Task<bool> _UpdateLicenseClassAsync()
        {
            //call DataAccess Layer 

            return await clsLicenseClassData.UpdateLicenseClassAsync(this.LicenseClassID, this.ClassName, this.ClassDescription,
                this.MinimumAllowedAge, this.DefaultValidityLength, this.ClassFees);
        }

        public static async Task<clsLicenseClass> FindAsync(int LicenseClassID)
        {
            
            var result = await clsLicenseClassData.GetLicenseClassInfoByIDAsync(LicenseClassID);

            if(result.IsFound)
                return new clsLicenseClass(LicenseClassID, result.ClassName, result.ClassDescription,
                    result.MinimumAllowedAge, result.DefaultValidityLength, result.ClassFees);
            else
                return null;

        }

        public static async Task<clsLicenseClass> FindAsync(string ClassName)
        {
            
            var result = await clsLicenseClassData.GetLicenseClassInfoByClassNameAsync(ClassName);

            if(result.IsFound)
                return new clsLicenseClass(result.LicenseClassID, ClassName, result.ClassDescription,
                    result.MinimumAllowedAge, result.DefaultValidityLength, result.ClassFees);
            else
                return null;

        }

        public static async Task<DataTable> GetAllLicenseClassesAsync()
        {
            return await clsLicenseClassData.GetAllLicenseClassesAsync();

        }

        public async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }
    }
}
