using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubeus.Commands
{
    public class Preauthscan : ICommand
    {
        public static string CommandName => "preauthscan";

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 83, 99, 97, 110, 32, 102, 111, 114, 32, 97, 99, 99, 111, 117, 110, 116, 115, 32, 110, 111, 116, 32, 114, 101, 113, 117, 105, 114, 105, 110, 103, 32, 75, 101, 114, 98, 101, 114, 111, 115, 32, 80, 114, 101, 45, 65, 117, 116, 104, 101, 110, 116, 105, 99, 97, 116, 105, 111, 110, 13, 10 }));

            List<string> users = new List<string>();
            string domain = null;
            string dc = null;
            string proxyUrl = null;

            if (arguments.ContainsKey("/users"))
            {
                if (System.IO.File.Exists(arguments["/users"]))
                {
                    string fileContent = Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(arguments["/users"]));
                    foreach (string u in fileContent.Split('\n'))
                    {
                        if (!String.IsNullOrWhiteSpace(u))
                        {
                            users.Add(u.Trim());
                        }
                    }
                }
                else
                {
                    foreach (string u in arguments["/users"].Split(','))
                    {
                        users.Add(u);
                    }
                }
            }

            if (users.Count < 1)
            {
                Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 78, 111, 32, 117, 115, 101, 114, 110, 97, 109, 101, 115, 32, 116, 111, 32, 116, 114, 121, 44, 32, 101, 120, 105, 116, 105, 110, 103, 46 }));
                return;
            }

            if (arguments.ContainsKey("/domain"))
            {
                domain = arguments["/domain"];
            }
            if (arguments.ContainsKey("/dc"))
            {
                dc = arguments["/dc"];
            }

            if (String.IsNullOrEmpty(domain))
            {
                domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
            }

            if (arguments.ContainsKey("/proxyurl"))
            {
                proxyUrl = arguments["/proxyurl"];
            }

            Ask.PreAuthScan(users, domain, dc, proxyUrl);
        }
    }
}
