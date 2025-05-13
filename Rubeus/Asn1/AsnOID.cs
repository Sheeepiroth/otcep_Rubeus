using System;
using System.Collections.Generic;
using System.Text;

namespace Asn1 {

public class AsnOID {

    static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

    static Dictionary<string, string> OIDToName =
        new Dictionary<string, string>();
    static Dictionary<string, string> NameToOID =
        new Dictionary<string, string>();

    static AsnOID()
    {
        /*
         * From RFC 5280, PKIX1Explicit88 module.
         */
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55}), S(new byte[] {105,100,45,112,107,105,120}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,49}), S(new byte[] {105,100,45,112,101}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,50}), S(new byte[] {105,100,45,113,116}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,51}), S(new byte[] {105,100,45,107,112}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,52,56}), S(new byte[] {105,100,45,97,100}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,50,46,49}), S(new byte[] {105,100,45,113,116,45,99,112,115}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,50,46,50}), S(new byte[] {105,100,45,113,116,45,117,110,111,116,105,99,101}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,52,56,46,49}), S(new byte[] {105,100,45,97,100,45,111,99,115,112}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,52,56,46,50}), S(new byte[] {105,100,45,97,100,45,99,97,73,115,115,117,101,114,115}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,52,56,46,51}), S(new byte[] {105,100,45,97,100,45,116,105,109,101,83,116,97,109,112,105,110,103}));
        Reg(S(new byte[] {49,46,51,46,54,46,49,46,53,46,53,46,55,46,52,56,46,53}), S(new byte[] {105,100,45,97,100,45,99,97,82,101,112,111,115,105,116,111,114,121}));

        Reg(S(new byte[] {50,46,53,46,52}), S(new byte[] {105,100,45,97,116}));
        Reg(S(new byte[] {50,46,53,46,52,46,52,49}), S(new byte[] {105,100,45,97,116,45,110,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,52}), S(new byte[] {105,100,45,97,116,45,115,117,114,110,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,52,50}), S(new byte[] {105,100,45,97,116,45,103,105,118,101,110,78,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,52,51}), S(new byte[] {105,100,45,97,116,45,105,110,105,116,105,97,108,115}));
        Reg(S(new byte[] {50,46,53,46,52,46,52,52}), S(new byte[] {105,100,45,97,116,45,103,101,110,101,114,97,116,105,111,110,81,117,97,108,105,102,105,101,114}));
        Reg(S(new byte[] {50,46,53,46,52,46,51}), S(new byte[] {105,100,45,97,116,45,99,111,109,109,111,110,78,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,55}), S(new byte[] {105,100,45,97,116,45,108,111,99,97,108,105,116,121,78,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,56}), S(new byte[] {105,100,45,97,116,45,115,116,97,116,101,79,114,80,114,111,118,105,110,99,101,78,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,49,48}), S(new byte[] {105,100,45,97,116,45,111,114,103,97,110,105,122,97,116,105,111,110,78,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,49,49}), S(new byte[] {105,100,45,97,116,45,111,114,103,97,110,105,122,97,116,105,111,110,97,108,85,110,105,116,78,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,49,50}), S(new byte[] {105,100,45,97,116,45,116,105,116,108,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,52,54}), S(new byte[] {105,100,45,97,116,45,100,110,81,117,97,108,105,102,105,101,114}));
        Reg(S(new byte[] {50,46,53,46,52,46,54}), S(new byte[] {105,100,45,97,116,45,99,111,117,110,116,114,121,78,97,109,101}));
        Reg(S(new byte[] {50,46,53,46,52,46,53}), S(new byte[] {105,100,45,97,116,45,115,101,114,105,97,108,78,117,109,98,101,114}));
        Reg(S(new byte[] {50,46,53,46,52,46,54,53}), S(new byte[] {105,100,45,97,116,45,112,115,101,117,100,111,110,121,109}));
        Reg(S(new byte[] {48,46,57,46,50,51,52,50,46,49,57,50,48,48,51,48,48,46,49,48,48,46,49,46,50,53}), S(new byte[] {105,100,45,100,111,109,97,105,110,67,111,109,112,111,110,101,110,116}));

        Reg(S(new byte[] {49,46,50,46,56,52,48,46,49,49,51,53,52,57,46,49,46,57}), S(new byte[] {112,107,99,115,45,57}));
        Reg(S(new byte[] {49,46,50,46,56,52,48,46,49,49,51,53,52,57,46,49,46,57,46,49}), S(new byte[] {105,100,45,101,109,97,105,108,65,100,100,114,101,115,115}));

        /*
         * From RFC 5280, PKIX1Implicit88 module.
         */
        Reg("2.5.29", "id-ce");
        Reg("2.5.29.35", "id-ce-authorityKeyIdentifier");
        Reg("2.5.29.14", "id-ce-subjectKeyIdentifier");
        Reg("2.5.29.15", "id-ce-keyUsage");
        Reg("2.5.29.16", "id-ce-privateKeyUsagePeriod");
        Reg("2.5.29.32", "id-ce-certificatePolicies");
        Reg("2.5.29.33", "id-ce-policyMappings");
        Reg("2.5.29.17", "id-ce-subjectAltName");
        Reg("2.5.29.18", "id-ce-issuerAltName");
        Reg("2.5.29.9", "id-ce-subjectDirectoryAttributes");
        Reg("2.5.29.19", "id-ce-basicConstraints");
        Reg("2.5.29.30", "id-ce-nameConstraints");
        Reg("2.5.29.36", "id-ce-policyConstraints");
        Reg("2.5.29.31", "id-ce-cRLDistributionPoints");
        Reg("2.5.29.37", "id-ce-extKeyUsage");

        Reg("2.5.29.37.0", "anyExtendedKeyUsage");
        Reg("1.3.6.1.5.5.7.3.1", "id-kp-serverAuth");
        Reg("1.3.6.1.5.5.7.3.2", "id-kp-clientAuth");
        Reg("1.3.6.1.5.5.7.3.3", "id-kp-codeSigning");
        Reg("1.3.6.1.5.5.7.3.4", "id-kp-emailProtection");
        Reg("1.3.6.1.5.5.7.3.8", "id-kp-timeStamping");
        Reg("1.3.6.1.5.5.7.3.9", "id-kp-OCSPSigning");

        Reg("2.5.29.54", "id-ce-inhibitAnyPolicy");
        Reg("2.5.29.46", "id-ce-freshestCRL");
        Reg("1.3.6.1.5.5.7.1.1", "id-pe-authorityInfoAccess");
        Reg("1.3.6.1.5.5.7.1.11", "id-pe-subjectInfoAccess");
        Reg("2.5.29.20", "id-ce-cRLNumber");
        Reg("2.5.29.28", "id-ce-issuingDistributionPoint");
        Reg("2.5.29.27", "id-ce-deltaCRLIndicator");
        Reg("2.5.29.21", "id-ce-cRLReasons");
        Reg("2.5.29.29", "id-ce-certificateIssuer");
        Reg("2.5.29.23", "id-ce-holdInstructionCode");
        Reg("2.2.840.10040.2", "WRONG-holdInstruction");
        Reg("2.2.840.10040.2.1", "WRONG-id-holdinstruction-none");
        Reg("2.2.840.10040.2.2", "WRONG-id-holdinstruction-callissuer");
        Reg("2.2.840.10040.2.3", "WRONG-id-holdinstruction-reject");
        Reg("2.5.29.24", "id-ce-invalidityDate");

        /*
         * These are the "right" OID. RFC 5280 mistakenly defines
         * the first OID element as "2".
         */
        Reg("1.2.840.10040.2", "holdInstruction");
        Reg("1.2.840.10040.2.1", "id-holdinstruction-none");
        Reg("1.2.840.10040.2.2", "id-holdinstruction-callissuer");
        Reg("1.2.840.10040.2.3", "id-holdinstruction-reject");

        /*
         * From PKCS#1.
         */
        Reg("1.2.840.113549.1.1", "pkcs-1");
        Reg("1.2.840.113549.1.1.1", "rsaEncryption");
        Reg("1.2.840.113549.1.1.7", "id-RSAES-OAEP");
        Reg("1.2.840.113549.1.1.9", "id-pSpecified");
        Reg("1.2.840.113549.1.1.10", "id-RSASSA-PSS");
        Reg("1.2.840.113549.1.1.2", "md2WithRSAEncryption");
        Reg("1.2.840.113549.1.1.4", "md5WithRSAEncryption");
        Reg("1.2.840.113549.1.1.5", "sha1WithRSAEncryption");
        Reg("1.2.840.113549.1.1.11", "sha256WithRSAEncryption");
        Reg("1.2.840.113549.1.1.12", "sha384WithRSAEncryption");
        Reg("1.2.840.113549.1.1.13", "sha512WithRSAEncryption");
        Reg("1.3.14.3.2.26", "id-sha1");
        Reg("1.2.840.113549.2.2", "id-md2");
        Reg("1.2.840.113549.2.5", "id-md5");
        Reg("1.2.840.113549.1.1.8", "id-mgf1");

        /*
         * From NIST: http://csrc.nist.gov/groups/ST/crypto_apps_infra/csor/algorithms.html
         */
        Reg("2.16.840.1.101.3", "csor");
        Reg("2.16.840.1.101.3.4", "nistAlgorithms");
        Reg("2.16.840.1.101.3.4.0", "csorModules");
        Reg("2.16.840.1.101.3.4.0.1", "aesModule1");

        Reg("2.16.840.1.101.3.4.1", "aes");
        Reg("2.16.840.1.101.3.4.1.1", "id-aes128-ECB");
        Reg("2.16.840.1.101.3.4.1.2", "id-aes128-CBC");
        Reg("2.16.840.1.101.3.4.1.3", "id-aes128-OFB");
        Reg("2.16.840.1.101.3.4.1.4", "id-aes128-CFB");
        Reg("2.16.840.1.101.3.4.1.5", "id-aes128-wrap");
        Reg("2.16.840.1.101.3.4.1.6", "id-aes128-GCM");
        Reg("2.16.840.1.101.3.4.1.7", "id-aes128-CCM");
        Reg("2.16.840.1.101.3.4.1.8", "id-aes128-wrap-pad");
        Reg("2.16.840.1.101.3.4.1.21", "id-aes192-ECB");
        Reg("2.16.840.1.101.3.4.1.22", "id-aes192-CBC");
        Reg("2.16.840.1.101.3.4.1.23", "id-aes192-OFB");
        Reg("2.16.840.1.101.3.4.1.24", "id-aes192-CFB");
        Reg("2.16.840.1.101.3.4.1.25", "id-aes192-wrap");
        Reg("2.16.840.1.101.3.4.1.26", "id-aes192-GCM");
        Reg("2.16.840.1.101.3.4.1.27", "id-aes192-CCM");
        Reg("2.16.840.1.101.3.4.1.28", "id-aes192-wrap-pad");
        Reg("2.16.840.1.101.3.4.1.41", "id-aes256-ECB");
        Reg("2.16.840.1.101.3.4.1.42", "id-aes256-CBC");
        Reg("2.16.840.1.101.3.4.1.43", "id-aes256-OFB");
        Reg("2.16.840.1.101.3.4.1.44", "id-aes256-CFB");
        Reg("2.16.840.1.101.3.4.1.45", "id-aes256-wrap");
        Reg("2.16.840.1.101.3.4.1.46", "id-aes256-GCM");
        Reg("2.16.840.1.101.3.4.1.47", "id-aes256-CCM");
        Reg("2.16.840.1.101.3.4.1.48", "id-aes256-wrap-pad");

        Reg("2.16.840.1.101.3.4.2", "hashAlgs");
        Reg("2.16.840.1.101.3.4.2.1", "id-sha256");
        Reg("2.16.840.1.101.3.4.2.2", "id-sha384");
        Reg("2.16.840.1.101.3.4.2.3", "id-sha512");
        Reg("2.16.840.1.101.3.4.2.4", "id-sha224");
        Reg("2.16.840.1.101.3.4.2.5", "id-sha512-224");
        Reg("2.16.840.1.101.3.4.2.6", "id-sha512-256");

        Reg("2.16.840.1.101.3.4.3", "sigAlgs");
        Reg("2.16.840.1.101.3.4.3.1", "id-dsa-with-sha224");
        Reg("2.16.840.1.101.3.4.3.2", "id-dsa-with-sha256");

        Reg("1.2.840.113549", "rsadsi");
        Reg("1.2.840.113549.2", "digestAlgorithm");
        Reg("1.2.840.113549.2.7", "id-hmacWithSHA1");
        Reg("1.2.840.113549.2.8", "id-hmacWithSHA224");
        Reg("1.2.840.113549.2.9", "id-hmacWithSHA256");
        Reg("1.2.840.113549.2.10", "id-hmacWithSHA384");
        Reg("1.2.840.113549.2.11", "id-hmacWithSHA512");

        /*
         * From X9.57: http://oid-info.com/get/1.2.840.10040.4
         */
        Reg("1.2.840.10040.4", "x9algorithm");
        Reg("1.2.840.10040.4", "x9cm");
        Reg("1.2.840.10040.4.1", "dsa");
        Reg("1.2.840.10040.4.3", "dsa-with-sha1");

        /*
         * From SEC: http://oid-info.com/get/1.3.14.3.2
         */
        Reg("1.3.14.3.2.2", "md4WithRSA");
        Reg("1.3.14.3.2.3", "md5WithRSA");
        Reg("1.3.14.3.2.4", "md4WithRSAEncryption");
        Reg("1.3.14.3.2.12", "dsaSEC");
        Reg("1.3.14.3.2.13", "dsaWithSHASEC");
        Reg("1.3.14.3.2.27", "dsaWithSHA1SEC");

        /*
         * From Microsoft: http://oid-info.com/get/1.3.6.1.4.1.311.20.2
         */
        Reg("1.3.6.1.4.1.311.20.2", "ms-certType");
        Reg("1.3.6.1.4.1.311.20.2.2", "ms-smartcardLogon");
        Reg("1.3.6.1.4.1.311.20.2.3", "ms-UserPrincipalName");
        Reg("1.3.6.1.4.1.311.20.2.3", "ms-UPN");
    }

    static void Reg(string oid, string name)
    {
        if (!OIDToName.ContainsKey(oid)) {
            OIDToName.Add(oid, name);
        }
        string nn = Normalize(name);
        if (NameToOID.ContainsKey(nn)) {
            throw new Exception(S(new byte[] { 79, 73, 68, 32, 110, 97, 109, 101, 32, 99, 111, 108, 108, 105, 115, 105, 111, 110, 58, 32 }) + nn);
        }
        NameToOID.Add(nn, oid);

        /*
         * Many names start with 'id-??-' and we want to support
         * the short names (without that prefix) as aliases. But
         * we must take care of some collisions on short names.
         */
        if (name.StartsWith(S(new byte[] { 105, 100, 45 })) && name.Length >= 7 && name[5] == '-')
        {
            if (name.StartsWith(S(new byte[] {105,100,45,97,100,45}))) {
                Reg(oid, name.Substring(6) + S(new byte[] {45,73,65}));
            } else if (name.StartsWith(S(new byte[] {105,100,45,107,112,45}))) {
                Reg(oid, name.Substring(6) + S(new byte[] {45,69,75,85}));
            } else {
                Reg(oid, name.Substring(6));
            }
        }
    }

    static string Normalize(string name)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in name) {
            int d = (int)c;
            if (d <= 32 || d == '-') {
                continue;
            }
            if (d >= 'A' && d <= 'Z') {
                d += 'a' - 'A';
            }
            sb.Append((char)c);
        }
        return sb.ToString();
    }

    public static string ToName(string oid)
    {
        return OIDToName.ContainsKey(oid) ? OIDToName[oid] : oid;
    }

    public static string ToOID(string name)
    {
        if (IsNumericOID(name)) {
            return name;
        }
        string nn = Normalize(name);
        if (!NameToOID.ContainsKey(nn)) {
            throw new AsnException(
                S(new byte[] {117,110,114,101,99,111,103,110,105,122,101,100,32,79,73,68,32,110,97,109,101,58,32}) + name);
        }
        return NameToOID[nn];
    }

    public static bool IsNumericOID(string oid)
    {
        /*
         * An OID is in numeric format if:
         * -- it contains only digits and dots
         * -- it does not start or end with a dot
         * -- it does not contain two consecutive dots
         * -- it contains at least one dot
         */
        foreach (char c in oid) {
            if (!(c >= '0' && c <= '9') && c != '.') {
                return false;
            }
        }
        if (oid.StartsWith(S(new byte[] {46}))) { // "."
            return false;
        }
        if (oid.EndsWith(S(new byte[] {46}))) {
            return false;
        }
        if (oid.IndexOf(S(new byte[] {46,46})) >= 0) {
            return false;
        }
        if (oid.IndexOf('.') < 0) {
            return false;
        }
        return true;
    }
}

}
