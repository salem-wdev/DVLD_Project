using DVLD.People.Forms;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Controls
{
    public partial class ctrlApplicationBasicInfo : UserControl
    {

        private clsApplication _Application;

        private int _ApplicationID = -1;

        public int ApplicationID
        {
            get { return _ApplicationID; }
        }

        public ctrlApplicationBasicInfo()
        {
            InitializeComponent();
        }

        private void _FillApplicationBasicInfo()
        {
            lblApplicationID.Text = _Application.ApplicationID.ToString();
            lblStatus.Text = _Application.ApplicationStatus.ToString();
            lblFees.Text = _Application.PaidFees.ToString();
            lblType.Text = _Application.ApplicationTypeInfo.ApplicationTypeTitle;
            lblApplicant.Text = _Application.PersonInfo.FullName;
            lblDate.Text = _Application.ApplicationDate.ToString("dd/MM/yyyy");
            lblStatusDate.Text = _Application.LastStatusDate.ToString("dd/MM/yyyy");
            lblCreatedByUser.Text = _Application.CreatedByUserInfo.UserName;
            llViewPersonInfo.Enabled = true;
        }

        private void ResetAll()
        {
            lblApplicationID.Text = "[???]";
            lblStatus.Text = "[???]";
            lblFees.Text = "[$$$]";
            lblType.Text = "[???]";
            lblApplicant.Text = "[????]";
            lblDate.Text = "[??/??/????]";
            lblStatusDate.Text = "[??/??/????]";
            lblCreatedByUser.Text = "[????]";
            llViewPersonInfo.Enabled = false;
        }

        public void LoadApplicationBasicInfo(int ApplicationID)
        {
            _ApplicationID = ApplicationID;
            _Application = clsApplication.Find(ApplicationID);
            if (_Application != null)
            {
                _FillApplicationBasicInfo();
            }
            else
            {
                ResetAll();
            }
        }

        private void llViewPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonInfo frm = new frmShowPersonInfo(_Application.ApplicantPersonID);
            frm.ShowDialog();

            LoadApplicationBasicInfo(ApplicationID);
        }
    }
}
