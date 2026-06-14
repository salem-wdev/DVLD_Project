using DVLD_Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!clsLicense.DeactivateExpiredLicenses())
            {
                //MessageBox.Show("An error occurred while deactivating expired licenses. Please contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }

            if (!clsInternationalLicense.DeactvateExpiredLicenses())
            {
                //MessageBox.Show("An error occurred while deactivating expired international licenses. Please contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }

            if (!clsTestAppointment.LockExpiredTestAppointments())
            {
                //MessageBox.Show("An error occurred while locking expired test appointments. Please contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmMain());
            //Application.Run(new test());
            Application.Run(new frmLogin());

        }
    }
}
