using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BasicSecurity.Models;
using System.IO;
using Ionic.Zip;
using System.Xml;
using System.Security.Cryptography;
using BasicSecurity.Models.Encryption;
using BasicSecurity.Helpers;

namespace BasicSecurity.Controllers
{
    public class HomeController : Controller
    {
        private User sender;
        private User receiver;
        private List<User> ListOvz = new List<User>();
        private UserPair senderAndReceiver;
        private String Message;

        private AesKey aesKey;
        private byte[] EncryptedMessage;
        private EncryptedAes EncryptedAesKey;
        private byte[] EncryptedMessageHash;

        private Aes DecryptedAesKey;


        public ActionResult Index()
        {
            List<BasicSecurity.Models.User> ListOvz = new List<BasicSecurity.Models.User>();

            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/Database.xml"));

            XmlNodeList list = xml.SelectNodes("/Users/User");
        
            Models.User dummy = new User();
            dummy.Id = 0;
            dummy.Name = "-Selecteer-";
            ListOvz.Add(dummy);

            foreach (XmlNode xn in list)
            {
                Models.User u = new User();
                u.Id = Convert.ToInt32(xn["Id"].InnerText);
                u.Name = xn["Name"].InnerText;
                u.publicKey = xn["PublicKey"].InnerText;
                ListOvz.Add(u);
            }
            return View(ListOvz);
        }

        public ActionResult Download(string id)
        {
            KeyGenerator keygen = new KeyGenerator(new Models.User(id));
            string filename = Path.GetFileName(keygen.PrivateKey().fullIdentity());
            string filepath = keygen.PrivateKey().fullIdentity();
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        [HttpPost]
        public ActionResult Encrypt(IEnumerable<HttpPostedFileBase> files, FormCollection collection)
        {
            int senderID = Convert.ToInt32(collection.Get("ddlFrom"));
            int receiverID = Convert.ToInt32(collection.Get("ddlTo"));
            BasicSecurity.Models.Result.Result myResult = new BasicSecurity.Models.Result.Result();
            string appDataFolder = Server.MapPath("~/App_Data/Downloads/");

            string privateKey = "";

            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes(file.ContentLength);

                    if (fileName.Contains("Private"))
                    {
                        privateKey = System.Text.Encoding.UTF8.GetString(binData);
                    }
                    else
                    {
                        Message = System.Text.Encoding.UTF8.GetString(binData);
                    }

                }
            }


            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/Database.xml"));

            XmlNodeList list = xml.SelectNodes("/Users/User");
            foreach (XmlNode xn in list)
            {
                Models.User u = new User();
                u.Id = Convert.ToInt32(xn["Id"].InnerText);
                u.Name = xn["Name"].InnerText;
                u.publicKey = xn["PublicKey"].InnerText;
                ListOvz.Add(u);

            }


            // Create users and UserPair
            sender = ListOvz.Find(x => x.Id == senderID);
            receiver = ListOvz.Find(x => x.Id == receiverID);

            senderAndReceiver = new UserPair(sender, receiver);
  
            // Create Aes key using UserPair
            aesKey = new AesKey(Aes.Create(), senderAndReceiver);


            RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
            privateRsa.FromXmlString(privateKey);


            RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider();
            publicRsa.FromXmlString(CommonMethods.ReturnPublicKey(ListOvz,receiverID));


            //// MESSAGE ENCRYPTION
            //// ------------------
            //// Encrypt message using Aes
            EncryptedMessage = HybridEncryption.EncryptMessageWithAes(Message, aesKey.GeneratedAesKey.Key, aesKey.GeneratedAesKey.IV);

            ////save to FILE_1
            System.IO.File.WriteAllBytes(appDataFolder + "FILE_1.txt", EncryptedMessage);
            myResult.FILE_1 = appDataFolder + "FILE_1.txt";

            //// KEY ENCRYPTION
            //// --------------
            //// Encrypt Aes key using Rsa
            EncryptedAesKey = HybridEncryption.EncryptAesKeyWithPublicKeyReceiver(aesKey.GeneratedAesKey, publicRsa);
            ////save to FILE_2
            System.IO.File.WriteAllBytes(appDataFolder + "FILE_2_IV.txt", EncryptedAesKey.IV);
            myResult.FILE_2_IV = appDataFolder + "FILE_2_IV.txt";

            System.IO.File.WriteAllBytes(appDataFolder + "FILE_2_KEY.txt", EncryptedAesKey.Key);
            myResult.FILE_2_KEY = appDataFolder + "FILE_2_KEY.txt";

            //// HASH ENCRYPTION
            //// ---------------
            //// Hash the message and encrypt with private key of sender ( 'signature' )
            EncryptedMessageHash = HybridEncryption.HashAndSignMessage(Message, privateRsa);

            ////save to FILE_3
            System.IO.File.WriteAllBytes(appDataFolder + "FILE_3.txt", EncryptedMessageHash);
            myResult.FILE_3 = appDataFolder + "FILE_3.txt";


            Response.Clear();
            Response.BufferOutput = false;
            string archiveName = String.Format("result-{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=" + archiveName);


            var outputStream = new MemoryStream();

            //nuget dotnetzip gedaan
            using (var zip = new ZipFile())
            {
                List<String> filesToInclude = new List<string>();


                filesToInclude.Add(myResult.FILE_1);
                filesToInclude.Add(myResult.FILE_2_IV);
                filesToInclude.Add(myResult.FILE_2_KEY);
                filesToInclude.Add(myResult.FILE_3);

                zip.AddFiles(filesToInclude, "files");
                zip.Save(Response.OutputStream);
            }

            outputStream.Position = 0;
            return File(outputStream, "application/zip", "keys.zip");


        }


        [HttpPost]
        public ActionResult Decrypt(IEnumerable<HttpPostedFileBase> files, FormCollection collection)
        {
            List<BasicSecurity.Models.User> ListOvz = new List<BasicSecurity.Models.User>();
            int senderID = Convert.ToInt32(collection.Get("senderID"));
            Models.User u = new User();

            string privateKey = "";

            byte[] file_1 = new byte[0];
            byte[] file_2_IV = new byte[0];
            byte[] file_2_KEY = new byte[0];
            byte[] file_3 = new byte[0];


            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/Database.xml"));

            XmlNodeList list = xml.SelectNodes("/Users/User");
            foreach (XmlNode xn in list)
            {
                u = new User();
                u.Id = Convert.ToInt32(xn["Id"].InnerText);
                u.Name = xn["Name"].InnerText;
                u.publicKey = xn["PublicKey"].InnerText;
                ListOvz.Add(u);

            }

            try
            {

                foreach (var file in files)
                {
                    if (file.ContentLength > 0)
                    {

                        var fileName = Path.GetFileName(file.FileName);
                        BinaryReader b = new BinaryReader(file.InputStream);
                        byte[] binData = b.ReadBytes(file.ContentLength);

                        if (fileName.Contains("Private"))
                        {
                            privateKey = System.Text.Encoding.UTF8.GetString(binData);
                        }
                        else if (fileName.Contains("FILE_1"))
                        {
                            file_1 = binData;
                        }
                        else if (fileName.Contains("FILE_2"))
                        {
                            if (fileName.Contains("IV"))
                            {
                                file_2_IV = binData;
                            }
                            else
                            {
                                file_2_KEY = binData;
                            }
                        }
                        else if (fileName.Contains("FILE_3"))
                        {
                            file_3 = binData;
                        }
                        else
                        {
                            Message = System.Text.Encoding.UTF8.GetString(binData);
                        }
                    }
                }


                RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
                privateRsa.FromXmlString(privateKey);

                RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider();
                publicRsa.FromXmlString(CommonMethods.ReturnPublicKey(ListOvz, senderID));

                EncryptedAes EncryptedAesKey = new EncryptedAes(file_2_IV, file_2_KEY);
                DecryptedAesKey = HybridEncryption.DecryptAesKeyWithPrivateKeyReceiver(EncryptedAesKey, privateRsa);

                string roundtrip = HybridEncryption.DecryptMessageWithAes(file_1, DecryptedAesKey.Key, DecryptedAesKey.IV);

                if (HybridEncryption.VerifySignedHash(roundtrip, file_3, publicRsa))
                {
                    u.communicationResult = "SUCCESS --> " + roundtrip;
                    u.communicationMessage = "De hash van het verzonden bericht komt overeen met de hash van het ontvangen bericht.";
                }
                else
                {
                    u.communicationResult = "FAILED";
                    u.communicationMessage = "De hash van het verzonden bericht komt niet overeen met de hash van het ontvangen bericht. Het bericht komt dus niet van de verwachte verzender.";
                }
            }catch(Exception e)
            {
                u.communicationResult = "FAILED";
                u.communicationMessage = e.Message;
            }
            
  
            ListOvz.Add(u);

            return View("Index",ListOvz);


        }




    }
}