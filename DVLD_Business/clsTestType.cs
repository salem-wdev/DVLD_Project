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
        private readonly Dictionary<enMode, Func<bool>> _saveDictionary;

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

        public clsTestType()
        {
            ID = clsTestType.enTestType.None;
            TestTypeFees = 0.0m;
            TestTypeTitle = string.Empty;
            TestTypeDescription = string.Empty;

            _saveDictionary = new Dictionary<enMode, Func<bool>>
            {
                {enMode.AddNew, _AddNewTestType },
                {enMode.Update, _UpdateTestType},
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

            _saveDictionary = new Dictionary<enMode, Func<bool>>
            {
                {enMode.AddNew, _AddNewTestType },
                {enMode.Update, _UpdateTestType},
            };

            Mode = enMode.Update;
        }

        private bool _AddNewTestType()
        {
            
            this.ID = (enTestType)clsTestTypeData.AddNewTestType(TestTypeTitle, TestTypeDescription, TestTypeFees);

            if (ID != enTestType.None)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateTestType()
        {
            return clsTestTypeData.UpdateTestType((int)ID, TestTypeTitle, TestTypeDescription, TestTypeFees);
        }

        public static clsTestType Find(enTestType ID)
        {
            string TestTypeTitle = string.Empty;
            string TestTypeDescription = string.Empty;
            decimal TestTypeFees = 0.0m;

            bool found = clsTestTypeData.GetTestTypeInfoByID((int)ID, ref TestTypeTitle, ref TestTypeDescription, ref TestTypeFees);

            if (found)
            {
                return new clsTestType(ID, TestTypeTitle, TestTypeDescription, TestTypeFees);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllTestTypes()
        {
            return clsTestTypeData.GetAllTestTypes();
        }

        public bool Save()
        {
            return _saveDictionary[this.Mode]();
        }


    }
}
