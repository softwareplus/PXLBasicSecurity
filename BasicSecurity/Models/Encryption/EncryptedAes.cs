using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicSecurity.Models.Encryption
{
    public class EncryptedAes
    {
        public EncryptedAes(byte[] IV, byte[] Key)
        {
            this.IV = IV;
            this.Key = Key;
        }

        public byte[] IV { get; set; }
        public byte[] Key { get; set; }
    }
}