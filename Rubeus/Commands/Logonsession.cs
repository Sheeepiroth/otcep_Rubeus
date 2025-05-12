using System;
using System.Collections.Generic;
using Rubeus.lib.Interop;

namespace Rubeus.Commands
{
    public class Logonsession : ICommand
    {
        public static string CommandName => "logonsession";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
            bool currentOnly = false;
            string targetLuidString = "";

            if (arguments.ContainsKey("/luid"))
            {
                targetLuidString = arguments["/luid"];
            }
            else if (arguments.ContainsKey("/current") || !Helpers.IsHighIntegrity())
            {
                currentOnly = true;
                Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 68,105, 115, 112, 108, 97, 121, 32, 99, 117, 114, 114, 101, 110, 116, 32, 108, 111, 103, 111, 110, 32, 115, 101, 115, 115, 105, 111, 110, 32, 105, 110, 102, 111, 114, 109, 97, 116, 105, 111, 110, 13, 10 }));
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 68, 105, 115, 112, 108, 97, 121, 32, 97, 108, 108, 32, 108, 111, 103, 111, 110, 32, 115, 101, 115, 115, 105, 111, 110, 32, 105, 110, 102, 111, 114, 109, 97, 116, 105, 111, 110, 13, 10 }));
            }

            List<LSA.LogonSessionData> logonSessions = new List<LSA.LogonSessionData>();

            if(!String.IsNullOrEmpty(targetLuidString))
            {
                try
                {
                    LUID targetLuid = new LUID(targetLuidString);
                    LSA.LogonSessionData logonData = LSA.GetLogonSessionData(targetLuid);
                    logonSessions.Add(logonData);
                }
                catch
                {
                    Console.WriteLine(S(new byte[] { 91, 33, 93, 32, 69, 114, 114, 111, 114, 32, 112, 97, 114, 115, 105, 110, 103, 32, 108, 117, 105, 100, 58, 32 }) + targetLuidString);
                    return;
                }
            }
            else if (currentOnly)
            {
                // not elevated, so only enumerate current logon session information
                LUID currentLuid = Helpers.GetCurrentLUID();
                LSA.LogonSessionData logonData = LSA.GetLogonSessionData(currentLuid);
                logonSessions.Add(logonData);
            }
            else
            {
                // elevated, so enumerate all logon session information
                List<LUID> sessionLUIDs = LSA.EnumerateLogonSessions();

                foreach(LUID luid in sessionLUIDs)
                {
                    LSA.LogonSessionData logonData = LSA.GetLogonSessionData(luid);
                    logonSessions.Add(logonData);
                }
            }

            foreach(LSA.LogonSessionData logonData in logonSessions)
            {
                Console.WriteLine($"    LUID          : {logonData.LogonID} ({(UInt64)logonData.LogonID})");
                Console.WriteLine($"    UserName      : {logonData.Username}");
                Console.WriteLine($"    LogonDomain   : {logonData.LogonDomain}");
                Console.WriteLine($"    SID           : {logonData.Sid}");
                Console.WriteLine($"    AuthPackage   : {logonData.AuthenticationPackage}");
                Console.WriteLine($"    LogonType     : {logonData.LogonType} ({(int)logonData.LogonType})");
                Console.WriteLine($"    Session       : {logonData.Session}");
                Console.WriteLine($"    LogonTime     : {logonData.LogonTime}");
                Console.WriteLine($"    LogonServer   : {logonData.LogonServer}");
                Console.WriteLine($"    DnsDomainName : {logonData.DnsDomainName}");
                Console.WriteLine($"    Upn           : {logonData.Upn}\r\n");
            }
        }
    }
}
