using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsTestType
    {


        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; }
        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public enum enTestType
        {
            None = -1,
            VisionTest = 1,
            WrittenTest = 2,
            StreetTest = 3
        }

        public clsTestType.enTestType ID { get; private set; }
        public string TestTypeTitle { get; set; }
        public string TestTypeDescription { get; set; }
        public decimal TestTypeFees { get; set; }

        private clsTestType()
        {
            ID = clsTestType.enTestType.None;
            TestTypeFees = 0.0m;
            TestTypeTitle = string.Empty;
            TestTypeDescription = string.Empty;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew, _AddNewTestTypeAsync },
                {enMode.Update, _UpdateTestTypeAsync},
            };

            Mode = enMode.AddNew;
        }

        // New overload that sets TestTypeID so instances returned from Find have correct ID
        private clsTestType(enTestType ID,
            string TestTypeTitle, string TestTypeDescription, decimal TestTypeFees)
        {
            this.ID = ID;
            this.TestTypeTitle = TestTypeTitle;
            this.TestTypeDescription = TestTypeDescription;
            this.TestTypeFees = TestTypeFees;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
                {enMode.AddNew, _AddNewTestTypeAsync },
                {enMode.Update, _UpdateTestTypeAsync},
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewTestTypeAsync()
        {
            
            this.ID = (enTestType)await clsTestTypeData.AddNewTestTypeAsync(TestTypeTitle, TestTypeDescription, TestTypeFees);

            if (ID != enTestType.None)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private async Task<bool> _UpdateTestTypeAsync()
        {
            return await clsTestTypeData.UpdateTestTypeAsync((int)ID, TestTypeTitle, TestTypeDescription, TestTypeFees);
        }

        public static async Task<clsTestType> FindAsync(enTestType ID)
        {
            
           var result = await clsTestTypeData.GetTestTypeInfoByIDAsync((int)ID);

            if (result.IsFound)
            {
                return new clsTestType(ID, result.TestTypeTitle, result.TestTypeDescription, result.TestTypeFees);
            }
            else
            {
                return null;
            }
        }

        public static async Task<DataTable> GetAllTestTypesAsync()
        {
            return await clsTestTypeData.GetAllTestTypesAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }


    }
}
