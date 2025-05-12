using System;
using System.Collections.Generic;
using System.IO;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class Asktgs : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 97, 115, 107, 116, 103, 115 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 65, 115, 107, 32, 84, 71, 83, 13, 10 }));

            string outfile = S(new byte[] {});
            bool ptt = false;
            string dc = S(new byte[] {});
            string service = S(new byte[] {});
            bool enterprise = false;
            bool opsec = false;
            Interop.KERB_ETYPE requestEnctype = Interop.KERB_ETYPE.subkey_keymaterial;
            KRB_CRED tgs = null;
            string targetDomain = S(new byte[] {});
            string servicekey = S(new byte[] {});
            string asrepkey = S(new byte[] {});
            bool u2u = false;
            string targetUser = S(new byte[] {});
            bool printargs = false;
            bool keyList = false;
            string proxyUrl = S(new byte[] {});
            bool dmsa = false;
            string serviceType = S(new byte[] { 115, 114, 118, 95, 105, 110, 115, 116 });

            LUID targetLuid = new LUID();
            if (arguments.ContainsKey(S(new byte[] { 47, 108, 117, 105, 100 }))) {
                try {
                    targetLuid = new LUID(arguments[S(new byte[] { 47, 108, 117, 105, 100 })]);
                } catch {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 76, 85, 73, 68, 32, 102, 111, 114, 109, 97, 116, 32, 40 }) + arguments[S(new byte[] { 47, 108, 117, 105, 100 })] + S(new byte[] { 41, 13, 10 }));
                    return;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 107, 101, 121, 76, 105, 115, 116 })))
            {
                keyList = true;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })))
            {
                outfile = arguments[S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 116, 116 })))
            {
                ptt = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 101, 110, 116, 101, 114, 112, 114, 105, 115, 101 })))
            {
                enterprise = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 111, 112, 115, 101, 99 })))
            {
                opsec = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 100, 99 })))
            {
                dc = arguments[S(new byte[] { 47, 100, 99 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 })))
            {
                string encTypeString = arguments[S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 })].ToUpper();

                if (encTypeString.Equals(S(new byte[] { 82, 67, 52 })) || encTypeString.Equals(S(new byte[] { 78, 84, 76, 77 })))
                {
                    requestEnctype = Interop.KERB_ETYPE.rc4_hmac;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 49, 50, 56 })))
                {
                    requestEnctype = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 50, 53, 54 })) || encTypeString.Equals(S(new byte[] { 65, 69, 83 })))
                {
                    requestEnctype = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 68, 69, 83 })))
                {
                    requestEnctype = Interop.KERB_ETYPE.des_cbc_md5;
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 85, 110, 115, 117, 112, 112, 111, 114, 116, 101, 100, 32, 101, 116, 121, 112, 101, 32, 58, 32 }) + encTypeString);
                    return;
                }
            }

            // for U2U requests
            if (arguments.ContainsKey(S(new byte[] { 47, 117, 50, 117 })))
            {
                u2u = true;
            }
            
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101 })))
            {
                service = arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101 })];
            }
            else if (!u2u)
            {
                Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 79, 110, 101, 32, 111, 114, 32, 109, 111, 114, 101, 32, 39, 47, 115, 101, 114, 118, 105, 99, 101, 58, 115, 110, 97, 109, 101, 47, 115, 101, 114, 118, 101, 114, 46, 100, 111, 109, 97, 105, 110, 46, 99, 111, 109, 39, 32, 115, 112, 101, 99, 105, 102, 105, 99, 97, 116, 105, 111, 110, 115, 32, 97, 114, 101, 32, 110, 101, 101, 100, 101, 100 }));
                return;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 116, 121, 112, 101 }))) {
                serviceType = arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 116, 121, 112, 101 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 107, 101, 121 }))) {
                servicekey = arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 107, 101, 121 })];
            }

            if (u2u || !String.IsNullOrEmpty(servicekey))
            {
                // print command arguments for forging tickets
                if (arguments.ContainsKey(S(new byte[] { 47, 112, 114, 105, 110, 116, 97, 114, 103, 115 })))
                {
                    printargs = true;
                }
            }


            if (arguments.ContainsKey(S(new byte[] { 47, 97, 115, 114, 101, 112, 107, 101, 121 }))) {
                asrepkey = arguments[S(new byte[] { 47, 97, 115, 114, 101, 112, 107, 101, 121 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 103, 115 })))
            {
                string kirbi64 = arguments[S(new byte[] { 47, 116, 103, 115 })];

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

            }

            // for manually specifying domain in requests
            if (arguments.ContainsKey(S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 100, 111, 109, 97, 105, 110 })))
            {
                targetDomain = arguments[S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 100, 111, 109, 97, 105, 110 })];
            }

            // for adding a PA-for-User PA data section
            if (arguments.ContainsKey(S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 117, 115, 101, 114 })))
            {
                targetUser = arguments[S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 117, 115, 101, 114 })];
            }

            // for using a KDC proxy
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 114, 111, 120, 121, 117, 114, 108 })))
            {
                proxyUrl = arguments[S(new byte[] { 47, 112, 114, 111, 120, 121, 117, 114, 108 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 100, 109, 115, 97 })))
            {
                dmsa = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })))
            {
                string kirbi64 = arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    Ask.TGS(kirbi, service, requestEnctype, outfile, ptt, dc, true, enterprise, false, opsec, tgs, targetDomain, servicekey, asrepkey, u2u, targetUser, printargs, proxyUrl, keyList, dmsa, serviceType, default);
                    return;
                }
                else if (File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = File.ReadAllBytes(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    Ask.TGS(kirbi, service, requestEnctype, outfile, ptt, dc, true, enterprise, false, opsec, tgs, targetDomain, servicekey, asrepkey, u2u, targetUser, printargs, proxyUrl, keyList, dmsa, serviceType, default);
                    return;
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                }
                return;
            }
            else
            {
                if(arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 114 })) || arguments.ContainsKey(S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })) || arguments.ContainsKey(S(new byte[] { 47, 100, 99 })) || arguments.ContainsKey(S(new byte[] { 47, 117, 50, 117 })) || arguments.ContainsKey(S(new byte[] { 47, 116, 103, 115 })))
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 65, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 110, 101, 101, 100, 115, 32, 116, 111, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33, 13, 10 }));
                else
                    Ask.TGS(null, service, requestEnctype, outfile, ptt, dc, true, enterprise, false, opsec, tgs, targetDomain, servicekey, asrepkey, u2u, targetUser, printargs, proxyUrl, keyList, dmsa, serviceType, targetLuid);
            }
        }
    }
}