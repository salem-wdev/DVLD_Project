using DVLD.Applications.Application_Types;
using DVLD.Applications.International_License;
using DVLD.Applications.Local_Driving_License;
using DVLD.Applications.LocalDrivingLicense;
using DVLD.Applications.Renew_Local_License;
using DVLD.Applications.ReplaceLostOrDamagedLicense;
using DVLD.Applications.Rlease_Detained_License;
using DVLD.Drivers;
using DVLD.Licenses.Detain_License;
using DVLD.Licenses.Local_Licenses;
using DVLD.People.Forms;
using DVLD.Tests.Test_Types;
using DVLD.Users;
using DVLD_Business;
using DVLD_Business.Global_Classes;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmMain : Form
    {
        frmLogin _LoginForm;

        public frmMain(frmLogin login)
        {
            InitializeComponent();
            _LoginForm = login;
        }

        private void accountSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void peopleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListPeople frm = new frmListPeople();
            frm.ShowDialog();
            frm.Dispose();
        }

        private void currentUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUserInfo frm = new frmUserInfo(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();
            clsGlobal.CurrentUser = clsUser.Find(clsGlobal.CurrentUser.UserID);
        }

        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsGlobal.CurrentUser = null;
            _LoginForm.Show();
            this.Hide();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUsersList frm = new frmUsersList();
            frm.ShowDialog();
            frm.Dispose();
        }

        private void driversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListDrivers frm = new frmListDrivers();
            frm.ShowDialog();
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void manageApplicationTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListApplicationTypes frm = new frmListApplicationTypes();
            frm.ShowDialog();
        }

        private void manageTestTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListTestTypes frm = new frmListTestTypes();
            frm.ShowDialog();
        }

        private void localLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicenseApplication frm = new frmAddUpdateLocalDrivingLicenseApplication();
            frm.ShowDialog();
        }

        private void manageLocalDrivingLicenseApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListLocalDrivinglicenseApplications frm = new frmListLocalDrivinglicenseApplications();
            frm.ShowDialog();
        }

        private void retakeTestToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmListLocalDrivinglicenseApplications frm = new frmListLocalDrivinglicenseApplications();
            frm.ShowDialog();
        }

        private void internationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewInternationalLicenseApplication frm = new frmNewInternationalLicenseApplication();
            frm.ShowDialog();
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void renewDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRenewLocalDrivingLicenseApplication frm = new frmRenewLocalDrivingLicenseApplication();
            frm.ShowDialog();
        }

        private void ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmReplaceLostOrDamagedLicenseApplication frm = new frmReplaceLostOrDamagedLicenseApplication();
            frm.ShowDialog();
        }

        private void releaseDetainedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmReleaseDetainedLicenseApplication frm = new frmReleaseDetainedLicenseApplication();
            frm.ShowDialog();
        }

        private void ManageDetainedLicensestoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmListDetainedLicenses frm = new frmListDetainedLicenses();
            frm.ShowDialog();
        }

        private void detainLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmDetainLicenseApplication frm = new frmDetainLicenseApplication();
            frm.ShowDialog();
        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmReleaseDetainedLicenseApplication frm = new frmReleaseDetainedLicenseApplication();
            frm.ShowDialog();
        }

        private void ManageInternationaDrivingLicenseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("This feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        frmListInternationalLicesnseApplications frm = new frmListInternationalLicesnseApplications();
            frm.ShowDialog();
        }

        private void licenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory();
            frm.ShowDialog();
        }
    }
}
