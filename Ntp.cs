using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace OtpSharp
{
    /// <summary>
    /// Class to aide with NTP (network time protocol) time corrections
    /// </summary>
    /// <remarks>
    /// This is experimental and doesn't have great test coverage
    /// nor is this idea fully developed.
    /// This API may change
    /// </remarks>
    public static class Ntp
    {
#if NET35
        // use NO_TPL to remove any reliance on the TPL library introduced in .net 4.0
#else
        /// <summary>
        /// Get a time correction factor against NIST
        /// </summary>
        /// <returns>Time Correction</returns>
        /// <remarks>
        /// This implementation is experimental and doesn't have any tests against it.
        /// This isn't even close to a robust and reliable implementation.
        /// </remarks>
        public static System.Threading.Tasks.Task<TimeCorrection> GetTimeCorrectionFromNistAsync(CancellationToken token = default(CancellationToken))
        {
            return (new System.Threading.Tasks.TaskFactory<TimeCorrection>()).StartNew(() => GetTimeCorrectionFromNist(token));
        }

        /// <summary>
        /// Get a time correction factor using Google's webservers as the time source.  Extremely fast and reliable but not authoritative.
        /// </summary>
        /// <returns>Time Correction</returns>
        public static System.Threading.Tasks.Task<TimeCorrection> GetTimeCorrectionFromGoogleAsync()
        {
            return (new System.Threading.Tasks.TaskFactory<TimeCorrection>()).StartNew(() => GetTimeCorrectionFromGoogle());
        }

#endif
        /// <summary>
        /// Get a time correction factor against NIST
        /// </summary>
        /// <returns>Time Correction</returns>
        /// <remarks>
        /// This implementation is experimental and doesn't have any tests against it.
        /// This isn't even close to a robust and reliable implementation.
        /// </remarks>
#if NET35
        public static TimeCorrection GetTimeCorrectionFromNist()
#else
        public static TimeCorrection GetTimeCorrectionFromNist(CancellationToken token = default(CancellationToken))
#endif
        {
            var servers = GetNistServers();

            foreach (string server in servers)
            {
#if NET35
#else
                token.ThrowIfCancellationRequested();
#endif
                try
                {
                    string response = null;
                    using (var client = new TcpClient())
                    {
                        client.ConnectAsync(server, 13).Wait();

                        var stream = client.GetStream();

                        using (var reader = new StreamReader(stream))
                        {
                            response = reader.ReadToEnd();
                        }
                    }

                    if (TryParseResponse(response, out var networkTime))
                    {
                        return new TimeCorrection(networkTime);
                    }
                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                    continue; // Loop around and try again on a different endpoint
                }
            }

            throw new NtpException("Couldn't get network time");
        }

        /// <summary>
        /// Get a time correction factor using Google's webservers as the time source.  Extremely fast and reliable but not authoritative.
        /// </summary>
        /// <returns>Time Correction</returns>
        public static TimeCorrection GetTimeCorrectionFromGoogle()
        {
            using (var wc = new HttpClient())
            {
                var res = wc.GetAsync("https://www.google.com").Result;
                var date =  res.Headers.Date.Value.DateTime;

                return new TimeCorrection(date);
            }
        }

        private static string[] GetNistServers()
        {
            return new[]
            {
                "time.nist.gov", // round robbin

                "nist1-ny.ustiming.org",
                "nist1-nj.ustiming.org",
                "nist1-pa.ustiming.org",
                "time-a.nist.gov",
                "time-b.nist.gov",
                "nist1.aol-va.symmetricom.com",
                "nist1.columbiacountyga.gov",
                "nist1-atl.ustiming.org",
                "nist1-chi.ustiming.org",
                "nist-chicago (No DNS)",
                "nist.time.nosc.us",
                "nist.expertsmi.com",
                "nist.netservicesgroup.com",
                "nisttime.carsoncity.k12.mi.us",
                "nist1-lnk.binary.net",
                "wwv.nist.gov",
                "time-a.timefreq.bldrdoc.gov",
                "time-b.timefreq.bldrdoc.gov",
                "time-c.timefreq.bldrdoc.gov",
                "utcnist.colorado.edu",
                "utcnist2.colorado.edu",
                "ntp-nist.ldsbc.edu",
                "nist1-lv.ustiming.org",
                "time-nw.nist.gov",
                "nist-time-server.eoni.com",
                "nist1.aol-ca.symmetricom.com",
                "nist1.symmetricom.com",
                "nist1-sj.ustiming.org",
                "nist1-la.ustiming.org",
            };
        }

        const string pattern = @"([0-9]{2}\-[0-9]{2}\-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2})";
        internal static bool TryParseResponse(string response, out DateTime ntpUtc)
        {
            if (response.ToUpperInvariant().Contains("UTC(NIST)"))
            {
                var match = Regex.Match(response, pattern);
                if (match.Success)
                {
                    ntpUtc = DateTime.Parse("20" + match.Groups[0].Value, CultureInfo.InvariantCulture.DateTimeFormat);
                    return true;
                }
                else
                {
                    ntpUtc = DateTime.MinValue;
                    return false;
                }
            }
            else
            {
                ntpUtc = DateTime.MinValue;
                return false;
            }
        }
    }
}