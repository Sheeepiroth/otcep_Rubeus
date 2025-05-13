using System;
using System.Collections.Generic;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class Currentluid : ICommand
    {
        public static string CommandName => "currentluid";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 67, 117, 114, 114, 101, 110, 116, 32, 76, 85, 73, 68, 13, 10 }));

            LUID currentLuid = Helpers.GetCurrentLUID();
            Console.WriteLine("[*] Current LogonID (LUID) : {0} ({1})\r\n", currentLuid, (UInt64)currentLuid);
        }
    }
}
