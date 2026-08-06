using DVLD_Business.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Users.Controls
{
    public partial class ctrlUserCard : UserControl
    {

        private clsUser _User;

        private int _UserID;

        public int UserID
        {
            get { return _UserID; }
        }

        public ctrlUserCard()
        {
            InitializeComponent();
        }


        public void LoadUserInfo(int UserID)
        {
            _UserID = UserID;
            _User = clsUser.Find(UserID);

            if (_User == null)
            {
                _ResetInfo();
                MessageBox.Show("No User with UserID = " + UserID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillUserInfo();
        }

        private void _ResetInfo()
        {
            ctrlPersonCard1.ResetPersonInfo();

            _UserID = -1;

            lblUserID.Text = "???";
            lblUserName.Text = "???";
            lblUserID.Text = "???";
        }

        private void _FillUserInfo()
        {
            ctrlPersonCard1.LoadData(_User.PersonID);
            lblUserID.Text = _User.UserID.ToString();
            lblUserName.Text = _User.UserName;
            lblIsActive.Text = (_User.IsActive) ? "Active" : "Inactive";

        }

    }
}
