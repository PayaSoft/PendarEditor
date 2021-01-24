using System;
using System.Collections.Generic;
using System.Text;

namespace HttpNamespaceManager.Lib.AccessControl
{
    using System.Text.RegularExpressions;

    /// <summary>
    ///     Security Descriptor
    /// </summary>
    /// <remarks>
    ///     The Security Descriptor is the top level of the Access
    ///     Control API. It represents all the Access Control data that is
    ///     associated with the secured object.
    /// </remarks>
    internal sealed class SecurityDescriptor
    {
        #region Constants

        /// <summary>
        ///     Regular Expression used to parse SDDL strings
        /// </summary>
        private const string sddlExpr = @"^(O:(?'owner'[A-Z]+?|S(-[0-9]+)+)?)?(G:(?'group'[A-Z]+?|S(-[0-9]+)+)?)?(D:(?'dacl'[A-Z]*(\([^\)]*\))*))?(S:(?'sacl'[A-Z]*(\([^\)]*\))*))?$";

        #endregion

        #region Fields

        public SecurityDescriptor()
        {
            this.SACL = null;
            this.Owner = null;
            this.Group = null;
            this.DACL = null;
        }

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or Sets the DACL
        /// </summary>
        /// <remarks>
        ///     The DACL (Discretionary Access Control List) is the
        ///     Access Control List that grants or denies various types of access
        ///     for different users and groups.
        /// </remarks>
        public AccessControlList DACL { get; set; }

        /// <summary>
        ///     Gets or Sets the Group
        /// </summary>
        /// <remarks>Security Descriptor Groups are present for Posix compatibility reasons and are usually ignored.</remarks>
        public SecurityIdentity Group { get; set; }

        /// <summary>
        ///     Gets or Sets the Owner
        /// </summary>
        public SecurityIdentity Owner { get; set; }

        /// <summary>
        ///     Gets or Sets the SACL
        /// </summary>
        /// <remarks>
        ///     The SACL (System Access Control List) is the Access
        ///     Control List that specifies what actions should be auditted
        /// </remarks>
        public AccessControlList SACL { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates a Security Descriptor from an SDDL string
        /// </summary>
        /// <param name="sddl">The SDDL string that represents the Security Descriptor</param>
        /// <returns>The Security Descriptor represented by the SDDL string</returns>
        /// <exception cref="System.FormatException" />
        public static SecurityDescriptor SecurityDescriptorFromString(string sddl)
        {
            Regex sddlRegex = new Regex(sddlExpr, RegexOptions.IgnoreCase);

            Match m = sddlRegex.Match(sddl);

            if (!m.Success) throw new FormatException("Invalid SDDL String Format");

            SecurityDescriptor sd = new SecurityDescriptor();

            if (m.Groups["owner"] != null && m.Groups["owner"].Success && !string.IsNullOrEmpty(m.Groups["owner"].Value))
            {
                sd.Owner = SecurityIdentity.SecurityIdentityFromString(m.Groups["owner"].Value);
            }

            if (m.Groups["group"] != null && m.Groups["group"].Success && !string.IsNullOrEmpty(m.Groups["group"].Value))
            {
                sd.Group = SecurityIdentity.SecurityIdentityFromString(m.Groups["group"].Value);
            }

            if (m.Groups["dacl"] != null && m.Groups["dacl"].Success && !string.IsNullOrEmpty(m.Groups["dacl"].Value))
            {
                sd.DACL = AccessControlList.AccessControlListFromString(m.Groups["dacl"].Value);
            }

            if (m.Groups["sacl"] != null && m.Groups["sacl"].Success && !string.IsNullOrEmpty(m.Groups["sacl"].Value))
            {
                sd.SACL = AccessControlList.AccessControlListFromString(m.Groups["sacl"].Value);
            }

            return sd;
        }

        /// <summary>
        ///     Renders the Security Descriptor as an SDDL string
        /// </summary>
        /// <remarks>
        ///     For more info on SDDL see
        ///     <a href="http://msdn.microsoft.com/library/en-us/secauthz/security/security_descriptor_string_format.asp">
        ///         MSDN:
        ///         Security Descriptor String Format.
        ///     </a>
        /// </remarks>
        /// <returns>An SDDL string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Owner != null)
            {
                sb.AppendFormat("O:{0}", this.Owner);
            }

            if (this.Group != null)
            {
                sb.AppendFormat("G:{0}", this.Group);
            }

            if (this.DACL != null)
            {
                sb.AppendFormat("D:{0}", this.DACL);
            }

            if (this.SACL != null)
            {
                sb.AppendFormat("S:{0}", this.SACL);
            }

            return sb.ToString();
        }

        #endregion
    }
}
