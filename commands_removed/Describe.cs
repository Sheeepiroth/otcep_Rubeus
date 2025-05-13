using System;
using System.Collections.Generic;
using System.IO;


namespace Rubeus.Commands
{
    public class Describe : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 100, 101, 115, 99, 114, 105, 98, 101 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 68, 101, 115, 99, 114, 105, 98, 101, 32, 84, 105, 99, 107, 101, 116, 13, 10 }));
            byte[] serviceKey = null;
            byte[] asrepKey = null;
            byte[] krbKey = null;
            string serviceUser = S(new byte[] {});
            string serviceDomain = S(new byte[] {});
            string desPlainText = S(new byte[] {});

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 107, 101, 121 })))
            {
                serviceKey = Helpers.StringToByteArray(arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 107, 101, 121 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 97, 115, 114, 101, 112, 107, 101, 121 })))
            {
                asrepKey = Helpers.StringToByteArray(arguments[S(new byte[] { 47, 97, 115, 114, 101, 112, 107, 101, 121 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 107, 114, 98, 107, 101, 121 })))
            {
                krbKey = Helpers.StringToByteArray(arguments[S(new byte[] { 47, 107, 114, 98, 107, 101, 121 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 101, 115, 112, 108, 97, 105, 110, 116, 101, 120, 116 })))
            {
                desPlainText = arguments[S(new byte[] { 47, 100, 101, 115, 112, 108, 97, 105, 110, 116, 101, 120, 116 })];
            }

            // for generating service ticket hash when using AES256
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 117, 115, 101, 114 })))
            {
                serviceUser = arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 117, 115, 101, 114 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 100, 111, 109, 97, 105, 110 })))
            {
                serviceDomain = arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 100, 111, 109, 97, 105, 110 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })))
            {
                string kirbi64 = arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    LSA.DisplayTicket(kirbi, 2, false, false, true, false, serviceKey, asrepKey, serviceUser, serviceDomain, krbKey, null, desPlainText);
                }
                else if (File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = File.ReadAllBytes(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    LSA.DisplayTicket(kirbi, 2, false, false, true, false, serviceKey, asrepKey, serviceUser, serviceDomain, krbKey, null, desPlainText);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
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
