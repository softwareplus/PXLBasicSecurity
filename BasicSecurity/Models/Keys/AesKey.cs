using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BasicSecurity.Models
{
    public class AesKey
    {

        public Aes GeneratedAesKey { get; set; }
        public UserPair UserPair { get; set; }

        public AesKey(Aes aes, UserPair userPair)
        {
            GeneratedAesKey = aes;
            UserPair = userPair;

        }
    }
}