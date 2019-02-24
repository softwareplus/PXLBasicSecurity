using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace BasicSecurity.Models
{
    public class KeyGenerator
    {
        private Key publicKey;
        private Key privateKey;
        private Key aesKey;

        public KeyGenerator(User user)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            publicKey = new Key(user,Key.KeyType.IsPublic,rsa);
            privateKey = new Key(user,Key.KeyType.IsPrivate, rsa);
            aesKey = new Key(user, Key.KeyType.IsAES);

            //dit overloaden
            user.publicKey = publicKey.Content();
            BasicSecurity.Helpers.UsersDAL objUsersDAL = new Helpers.UsersDAL();
            objUsersDAL.Create(user);


        }

        public Key PublicKey()
        {
            return publicKey;
        }

        public Key PrivateKey()
        {
            return privateKey;
        }

        public Key AESKey()
        {
            return aesKey;
        }




    }
}