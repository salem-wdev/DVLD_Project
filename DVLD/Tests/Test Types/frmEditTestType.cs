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

namespace DVLD.Tests.Test_Types
{
    public partial class frmEditTestType : Form
    {
        private clsTestType.enTestType _TestTypeID = clsTestType.enTestType.None;
        private clsTestType _TestType;

        public frmEditTestType(clsTestType.enTestType TestTypeID)
        {
            InitializeComponent();
            _TestTypeID = TestTypeID;
        }

        private void txtFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            e.Handled = !clsValidation.IsInputValidDecimal(e.KeyChar, txt.Text, txt.SelectionStart);

        }

        private void text_Validating(object sender, CancelEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                errorProvider1.SetError(txt, txt?.Tag.ToString() + " is required");
                e.Cancel = true;
                return;
            }
            else
            {
                errorProvider1.SetError(txt, string.Empty);
                e.Cancel = false;
            }

            if (txt == txtFees)
            {
                if (!clsValidation.IsNumber(txt.Text))
                {
                    errorProvider1.SetError(txt, "Please enter a valid number");
                    e.Cancel = true;
                }
                else
                {
                    errorProvider1.SetError(txt, string.Empty);
                    e.Cancel = false;
                }
            }
        }

        private async void frmEditTestType_Load(object sender, EventArgs e)
        {
            _TestType = await clsTestType.FindAsync(_TestTypeID);
            if (_TestType != null)
            {
                lblTestTypeID.Text = ((int)_TestType.ID).ToString();
                txtTitle.Text = _TestType.TestTypeTitle;
                txtDescription.Text = _TestType.TestTypeDescription;
                txtFees.Text = _TestType.TestTypeFees.ToString();
            }
            else
            {
                MessageBox.Show("Test Type not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                _TestType.TestTypeTitle = txtTitle.Text.Trim();
                _TestType.TestTypeDescription = txtDescription.Text.Trim();
                if (decimal.TryParse(txtFees?.Text.Trim(), out decimal fees))
                {
                    _TestType.TestTypeFees = fees;
                    if (await _TestType.SaveAsync())
                    {
                        MessageBox.Show("Test Type updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update Test Type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid number for fees.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
