using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace BasicSecurity.Models
{
    public class Key
    {
        public KeyType keyType;
        private string content;
        private string location = System.Web.HttpRuntime.CodegenDir;
        private string fileName;

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
                content = "KEY : " + keyType.ToString() + " blablabla";
            }
            if (keyType == KeyType.IsPrivate)
            {
                content = "KEY : " + keyType.ToString() + " blablabla";
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