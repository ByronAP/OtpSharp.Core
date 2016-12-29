using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OtpSharp
{
    /// <summary>
    /// The type of one time password
    /// </summary>
    public enum OtpType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// HOTP
        /// </summary>
        Hotp,
        /// <summary>
        /// TOTP
        /// </summary>
        Totp
    }
}
