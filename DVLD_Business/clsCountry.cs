using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsCountry
    {

        public int CountryID { get; }
        public string CountryName { get; }

        public clsCountry()
        {
            // Private constructor to prevent instantiation without parameters
            CountryID = -1;
            CountryName = string.Empty;
        }

        private clsCountry(int CountryID, string CountryName)
        {
            this.CountryID = CountryID;
            this.CountryName = CountryName;
        }

        public static async Task<DataTable> GetAllCountriesAsync()
        {
            //await Task.Delay(5000); // Simulate a delay for demonstration purposes
            return await clsCountryData.GetAllCountriesAsync();
        }

        public static async Task<clsCountry> FindAsync(int CountryID)
        {
            string CountryName = string.Empty;
            CountryName = await clsCountryData.GetCountryByIDAsync(CountryID);
            if (!string.IsNullOrEmpty(CountryName))
            {
                return new clsCountry(CountryID, CountryName);
            }
            else
            {
                return null;
            }
        }

        public static async Task<clsCountry> FindAsync(string CountryName)
        {
            int? CountryID = await clsCountryData.GetCountryByCountryNameAsync(CountryName);
            if (CountryID.HasValue)
            {
                return new clsCountry(CountryID.Value, CountryName);
            }
            else
            {
                return null;
            }
        }
    }
}
