using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DVLD
{
    public class clsValidation
    {
        static public bool ValidateEmail(string email)
        {
            var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            
            var regex = new Regex(pattern);
            
            return regex.IsMatch(email);
        }

        public static bool ValidateInteger(string Number)
        {
            var pattern = @"^[0-9]*$";

            var regex = new Regex(pattern);

            return regex.IsMatch(Number);
        }

        public static bool ValidateFloat(string Number)
        {
            var pattern = @"^[0-9]*(?:\.[0-9]*)?$";

            var regex = new Regex(pattern);

            return regex.IsMatch(Number);
        }

        public static bool IsNumber(string Number)
        {
            return (ValidateInteger(Number) || ValidateFloat(Number));
        }

        public static bool IsInputValidDecimal(char keyChar, string currentText, int SelectionStart)
        {

            if (keyChar != '.')
            {
                if (!char.IsDigit(keyChar) && !char.IsControl(keyChar))
                {
                    return false;
                    
                }
            }

            // Prevent the placement of a mark when replacing the entire text with it. 
            if (keyChar == '.' && SelectionStart == 0)
            {
                return false;
                
            }

            // Allow only one mark
            if (keyChar == '.' && currentText.Contains("."))
            {
                return false;
                
            }
            // prevent mark when text box is empty
            //if (KeyChar == '.' && string.IsNullOrWhiteSpace(((TextBox)sender).Text))
            //{
            //    e.Handled = true;
            //    return;
            //}
            // allow anything else

            return true;

        }

        public static bool IsValidCharForID(char ID_Digit)
        {
            return char.IsDigit(ID_Digit) || char.IsControl(ID_Digit);

        }

    }
}
