using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsDataAccessSettings
    {
        public static string ConnectionString { get; } = "Server=.;Database=DVLD;User=sa;Password=123456";
        public static string DataBaseName { get; } = "DVLD";
    }
}
