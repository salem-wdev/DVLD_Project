using DVLD.People.Forms;
using DVLD.Users;
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

            while (!clsUser.HasUsers())
            {
                //this.Hide();

                while (!clsPerson.HasPeople())
                {

                    if (MessageBox.Show("Ther is no People in the system.\nDo you want to add person?"
                        , "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                        , MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        frmAddUpdatePerson frm = new frmAddUpdatePerson();
                        frm.ShowDialog();
                    }
                    else
                    {
                        Application.Exit();
                        return;
                    }
                }

                if (MessageBox.Show("Ther is no Users in the system.\nDo you want to add User?"
                        , "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                        , MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    frmAddUpdateUser frm = new frmAddUpdateUser();
                    frm.ShowDialog();
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }

            //Application.Run(new frmMain());
            //Application.Run(new test());
            Application.Run(new frmLogin());

        }
    }
}
