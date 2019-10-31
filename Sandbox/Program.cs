using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using SpbDotNet.PEM.Lib.ToShared;

namespace SpbDotNet.PEM.Sandbox
{
    internal class Program
    {
        internal static int Main()
        {
            Console.WriteLine("SpbDotNet Meetup PEM example");

            const string privateKeyPemFile = "../../../../dev.privateKey.rsa.pem";

            var privateKeyBinaryData = PemFileLoader.LoadPrivateKey(privateKeyPemFile);

            var rsa = RSA.Create();

            rsa.ImportRSAPrivateKey(privateKeyBinaryData, out var privateKeyBytesImported);

            Console.WriteLine($"Private key bytes imported: {privateKeyBytesImported}");

            var privateKey = new RsaSecurityKey(rsa);

            Console.WriteLine($"Private key size: {privateKey.KeySize} bits");

            var a = rsa.ExportRSAPublicKey();
            var sha1 = SHA1.Create();

            privateKey.KeyId = BitConverter.ToString(sha1.ComputeHash(a)).Replace('-', ':');

            Console.WriteLine($"Private key id: \"{privateKey.KeyId}\"");

            var signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

            var jwtHeader = new JwtHeader(signingCredentials);

            var jwtPayload = new JwtPayload
            {
                {JwtRegisteredClaimNames.Iss, typeof(Program).Namespace},
                {JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds()},
            };

            var token = new JwtSecurityToken(jwtHeader, jwtPayload);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            Console.WriteLine(tokenString);

            return 0;
        }
    }
}
