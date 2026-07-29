using System;

namespace DVLD_Business
{
    [Flags]
    public enum enUserPermissions : int
    {
        None = 0,
        ManageUsers = 1,
        ManageDrivers = 2,
        ManageLicenses = 4,
        ManageApplications = 8,
        All = -1
    }
}