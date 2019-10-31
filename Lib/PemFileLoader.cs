using System;
using System.Text;

namespace SpbDotNet.PEM.Lib.ToShared
{
    public static class PemFileLoader
    {
        private const string PrivateKeyPemHeader = "-----BEGIN RSA PRIVATE KEY-----";
        private const string PrivateKeyPemFooter = "-----END RSA PRIVATE KEY-----";

        private static byte[] LoadFile(string filePath, string header, string footer)
        {
            var b = System.IO.File.ReadAllBytes(filePath);
            var s = Encoding.UTF8.GetString(b).Trim();

            if (!(s.StartsWith(header) && s.EndsWith(footer)))
            {
                throw new FormatException("Invalid PEM format");
            }

            var b64 = s.Replace(header, string.Empty)
                .Replace(footer, string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            return Convert.FromBase64String(b64);
        }

        public static byte[] LoadPrivateKey(string filePath)
        {
            return LoadFile(filePath, PrivateKeyPemHeader, PrivateKeyPemFooter);
        }
    }
}
