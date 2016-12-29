using System;

namespace OtpSharp
{
    /// <summary>
    /// An generic ntp exception
    /// </summary>
    public class NtpException : Exception
    {
        /// <summary>
        /// NtpException with a message
        /// </summary>
        /// <param name="message">Message</param>
        public NtpException(string message)
            :base(message)
        {
            
        }
    }
}
