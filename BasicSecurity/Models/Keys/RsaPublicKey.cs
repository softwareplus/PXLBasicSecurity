using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BasicSecurity.Models
{
    public class RsaPublicKey
    {
        public RSAParameters GeneratedRsaPublicKey { get; set; }
        public User User { get; set; }

        public RsaPublicKey(RSA rsa, User user)
        {
            GeneratedRsaPublicKey = rsa.ExportParameters(false);
            User = user;
        }
    }
}