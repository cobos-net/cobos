// ----------------------------------------------------------------------------
// <copyright file="PrivilegeEnum.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Authentication
{
    /// <summary>
    /// User Access privilege level.
    /// </summary>
    public enum PrivilegeEnum
    {
        /// <summary>
        /// Guest - lowest privilege.
        /// </summary>
        Guest = 1,

        /// <summary>
        /// Normal user.
        /// </summary>
        User,

        /// <summary>
        /// Normal user with some special privileges.
        /// </summary>
        PowerUser,

        /// <summary>
        /// Supervisor access.
        /// </summary>
        Supervisor,

        /// <summary>
        /// Full access - highest level.
        /// </summary>
        Administrator,
    }
}
