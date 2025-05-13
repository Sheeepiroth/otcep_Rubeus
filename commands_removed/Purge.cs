using System;
using System.Collections.Generic;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class Purge : ICommand
    {
        public static string CommandName => "purge";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 80, 117, 114, 103, 101, 32, 84, 105, 99, 107, 101, 116, 115 }));

            LUID luid = new LUID();

            if (arguments.ContainsKey("/luid"))
            {
                try
                {
                    luid = new LUID(arguments["/luid"]);
                }
                catch
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 76, 85, 73, 68, 32, 102, 111, 114, 109, 97, 116, 32, 40 }) + arguments[S(new byte[] { 47, 108, 117, 105, 100 })] + S(new byte[] { 41, 13, 10 }));
                    return;
                }
            }

            LSA.Purge(luid);
        }
    }
}