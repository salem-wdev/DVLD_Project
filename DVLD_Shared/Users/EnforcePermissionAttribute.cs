using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Shared.Users
{
    // Restriction: This attribute can only be applied to methods, not classes or properties.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EnforcePermissionAttribute : Attribute
    {
        // Property to hold the required permission for the decorated method
        public enUserPermissions Permission { get; }

        // Constructor that accepts the specific enum value as the security "lock"
        public EnforcePermissionAttribute(enUserPermissions permission)
        {
            Permission = permission;
        }
    }
}

