using System;
using System.Security.Cryptography;
using System.Linq;

namespace OtpSharp
{
    /// <summary>
    /// Helpers to work with keys
    /// </summary>
    public static class KeyGeneration
    {
        /// <summary>
        /// Generates a random key in accordance with the RFC recommened length for each algorithm
        /// </summary>
        /// <param name="length">Key length</param>
        /// <returns>The generated key</returns>
        public static byte[] GenerateRandomKey(int length)
        {
            byte[] key = new byte[length];
#if NET35
            var rnd = System.Security.Cryptography.RandomNumberGenerator.Create();
#else
            using (var rnd = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
#endif
                rnd.GetBytes(key);
                return key;
#if NET35
#else
            }
#endif
        }

        /// <summary>
        /// Generates a random key in accordance with the RFC recommened length for each algorithm
        /// </summary>
        /// <param name="mode">HashMode</param>
        /// <returns>Key</returns>
        public static byte[] GenerateRandomKey(OtpHashMode mode = OtpHashMode.Sha1)
        {
            return GenerateRandomKey(LengthForMode(mode));
        }

        /// <summary>
        /// Uses the procedure defined in RFC 4226 section 7.5 to derive a key from the master key
        /// </summary>
        /// <param name="masterKey">The master key from which to derive a device specific key</param>
        /// <param name="publicIdentifier">The public identifier that is unique to the authenticating device</param>
        /// <param name="mode">The hash mode to use.  This will determine the resulting key lenght.  The default is sha-1 (as per the RFC) which is 20 bytes</param>
        /// <returns>Derived key</returns>
        public static byte[] DeriveKeyFromMaster(IKeyProvider masterKey, byte[] publicIdentifier, OtpHashMode mode = OtpHashMode.Sha1)
        {
            if (masterKey == null)
                throw new ArgumentNullException("masterKey");
            return masterKey.ComputeHmac(mode, publicIdentifier);
        }

        /// <summary>
        /// Uses the procedure defined in RFC 4226 section 7.5 to derive a key from the master key
        /// </summary>
        /// <param name="masterKey">The master key from which to derive a device specific key</param>
        /// <param name="serialNumber">A serial number that is unique to the authenticating device</param>
        /// <param name="mode">The hash mode to use.  This will determine the resulting key lenght.  The default is sha-1 (as per the RFC) which is 20 bytes</param>
        /// <returns>Derived key</returns>
        public static byte[] DeriveKeyFromMaster(IKeyProvider masterKey, int serialNumber, OtpHashMode mode = OtpHashMode.Sha1)
        {
            return DeriveKeyFromMaster(masterKey, KeyUtilities.GetBigEndianBytes(serialNumber), mode);
        }

        private static HashAlgorithm GetHashAlgorithmForMode(OtpHashMode mode)
        {
            switch (mode)
            {
                case OtpHashMode.Sha256:
                    return SHA256.Create();
                case OtpHashMode.Sha512:
                    return SHA512.Create();
                default: //case OtpHashMode.Sha1:
                    return SHA1.Create();
            }
        }

        private static int LengthForMode(OtpHashMode mode)
        {
            switch (mode)
            {
                case OtpHashMode.Sha256:
                    return 32;
                case OtpHashMode.Sha512:
                    return 64;
                default: //case OtpHashMode.Sha1:
                    return 20;
            }
        }
    }
}