using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BasicSecurity.Models
{
    public class RsaPrivateKey
    {
        public RSAParameters GeneratedRsaPrivateKey { get; set; }
        public User User { get; set; }

        public RsaPrivateKey(RSA rsa, User user)
        {
            GeneratedRsaPrivateKey = rsa.ExportParameters(true);
            User = user;
        }
    }
}