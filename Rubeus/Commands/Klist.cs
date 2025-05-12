using System;
using System.Collections.Generic;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class Klist : ICommand
    {
        public static string CommandName => "klist";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            if (Helpers.IsHighIntegrity())
            {
                Console.WriteLine(S(new byte[] { 13, 10, 65, 99, 116, 105, 111, 110, 58, 32, 76, 105, 115, 116, 32, 75, 101, 114, 98, 101, 114, 111, 115, 32, 84, 105, 99, 107, 101, 116, 115, 32, 40, 65, 108, 108, 32, 85, 115, 101, 114, 115, 41, 13, 10 }));
            }
            else
            {
                Console.WriteLine(S(new byte[] { 13, 10, 65, 99, 116, 105, 111, 110, 58, 32, 76, 105, 115, 116, 32, 75, 101, 114, 98, 101, 114, 111, 115, 32, 84, 105, 99, 107, 101, 116, 115, 32, 40, 67, 117, 114, 114, 101, 110, 116, 32, 85, 115, 101, 114, 41, 13, 10 }));
            }

            LUID targetLuid = new LUID();
            string targetUser = "";
            string targetService = "";
            string targetServer = "";

            if (arguments.ContainsKey("/luid"))
            {
                try
                {
                    targetLuid = new LUID(arguments["/luid"]);
                }
                catch
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 76, 85, 73, 68, 32, 102, 111, 114, 109, 97, 116, 32, 40 }) + arguments[S(new byte[] { 47, 108, 117, 105, 100 })] + S(new byte[] { 41, 13, 10 }));
                    return;
                }
            }

            if (arguments.ContainsKey("/user"))
            {
                targetUser = arguments["/user"];
            }

            if (arguments.ContainsKey("/service"))
            {
                targetService = arguments["/service"];
            }

            if (arguments.ContainsKey("/server"))
            {
                targetServer = arguments["/server"];
            }

            // extract out the tickets (w/ full data) with the specified targeting options
            List<LSA.SESSION_CRED> sessionCreds = LSA.EnumerateTickets(false, targetLuid, targetService, targetUser, targetServer, true);
            // display tickets with the "Full" format
            LSA.DisplaySessionCreds(sessionCreds, LSA.TicketDisplayFormat.Klist);
        }
    }
}