﻿
namespace OtpSharp
{
    /// <summary>
    /// Indicates which HMAC hashing algorithm should be used
    /// </summary>
    public enum OtpHashMode
    {
        /// <summary>
        /// Sha1 is used as the HMAC hashing algorithm
        /// </summary>
        Sha1,
        /// <summary>
        /// Sha256 is used as the HMAC hashing algorithm
        /// </summary>
        Sha256,
        /// <summary>
        /// Sha512 is used as the HMAC hashing algorithm
        /// </summary>
        Sha512
    }
}
