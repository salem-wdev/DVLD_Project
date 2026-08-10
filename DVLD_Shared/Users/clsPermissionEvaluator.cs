using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Shared.Users
{

    public class clsPermissionEvaluator
    {
        /// <summary>
        /// Checks whether a specific user has the required permission.
        /// </summary>
        /// <param name="userID">The unique identifier of the user.</param>
        /// <param name="permissionToCheck">The permission enum value to validate.</param>
        /// <returns>True if the user is authorized; otherwise, false.</returns>
        public static bool HasPermission(enUserPermissions userPermissionsValue, enUserPermissions permissionToCheck)
        {
            // 1. Fetch the latest up-to-date permissions value directly from the database using User ID.
            // Note: Consider implementing a caching strategy here in the future to reduce DB load.

            // 2. If the user has full administrative privileges (represented by -1), grant access automatically.
            if (userPermissionsValue == enUserPermissions.All)
                return true;

            // 3. Cast the enum permission value to an integer for the bitwise evaluation.
            enUserPermissions permissionValue = permissionToCheck;

            // 4. Perform a Bitwise AND operation to verify if the specific permission bit is set.
            return ((userPermissionsValue & permissionValue) == permissionValue);
        }

        public static bool ValidationUser(enUserPermissions userPermissionsValue)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase callingMethod = stackTrace.GetFrame(1)?.GetMethod();

            if (callingMethod == null)
                return false;

            EnforcePermissionAttribute attribute = callingMethod.GetCustomAttribute<EnforcePermissionAttribute>();

            if (attribute == null)
                return true;

            return HasPermission(userPermissionsValue, attribute.Permission);
        }
    }
}
