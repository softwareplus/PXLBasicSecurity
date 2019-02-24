using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;


namespace BasicSecurity.Models
{
    public class Key
    {
        public KeyType keyType;
        private string content;
        private string location = System.Web.HttpRuntime.CodegenDir;
        private string fileName;
        private User currentUser;
        private RSACryptoServiceProvider _rsa;

        public enum KeyType
        {
            IsPrivate,
            IsPublic,
            IsAES

        }

        public Key(User u, KeyType whichType)
        {
            keyType = whichType;
            fileName = keyType.ToString() + "_" + u.Name + ".KEY";
            currentUser = u;
            CreateContent();
        }

        //heb ik overloaden voor de shared RSACryptoSP voor de  private/public keys
        public Key(User u, KeyType whichType, RSACryptoServiceProvider rsa)
        {
            keyType = whichType;
            fileName = keyType.ToString() + "_" + u.Name + ".KEY";
            _rsa = rsa;
            currentUser = u;
            CreateContent();
        }



        private void CreateContent()
        {
            //hier nog modellen schrijven voor Public/Private
            
           
            if (keyType==KeyType.IsAES)
            {
                //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged?view=netframework-4.7.2
                //Rijndael kan ook 

                System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
                aes.GenerateKey();

                content = System.Text.Encoding.Default.GetString(aes.Key);
            }
            if (keyType == KeyType.IsPublic)
            {
                content = _rsa.ToXmlString(false);
            }
            if (keyType == KeyType.IsPrivate)
            {
                content = _rsa.ToXmlString(true);
            }

            File.WriteAllText(location + "\\" + fileName, content);

        }

        public string Content()
        {
            return content;
        }

        public string fullIdentity()
        {
            return location + "\\" + fileName;
        }

        public string FileName()
        {
            return fileName;
        }


    }
}