using System;
using System.Collections.Generic;


namespace Rubeus.Commands
{
    public class Hash : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 104, 97, 115, 104 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 67, 97, 108, 99, 117, 108, 97, 116, 101, 32, 80, 97, 115, 115, 119, 111, 114, 100, 32, 72, 97, 115, 104, 40, 101, 115, 41, 13, 10 }));

            string user = S(new byte[] {});
            string domain = S(new byte[] {});
            string password = S(new byte[] {});

            if (arguments.ContainsKey(S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })))
            {
                domain = arguments[S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })];
            }

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

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })))
            {
                password = arguments[S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })];
            }
            else
            {
                Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 47, 112, 97, 115, 115, 119, 111, 114, 100, 58, 88, 32, 109, 117, 115, 116, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33 }));
                return;
            }

            Crypto.ComputeAllKerberosPasswordHashes(password, user, domain);
        }
    }
}