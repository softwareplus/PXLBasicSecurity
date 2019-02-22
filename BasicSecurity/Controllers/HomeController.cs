using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BasicSecurity.Models;
using System.IO;
using Ionic.Zip;
using System.Xml;

namespace BasicSecurity.Controllers
{
    public class HomeController : Controller
    {
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

            Response.Clear();
            Response.BufferOutput = false;
            string archiveName = String.Format("archive-{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=" + archiveName);


            KeyGenerator keygen = new KeyGenerator(new Models.User(id));

            var outputStream = new MemoryStream();

            //nuget dotnetzip gedaan
            using (var zip = new ZipFile())
            {
                List<String> filesToInclude = new List<string>();
                filesToInclude.Add(keygen.PrivateKey().fullIdentity());
                filesToInclude.Add(keygen.PublicKey().fullIdentity());
                filesToInclude.Add(keygen.AESKey().fullIdentity());

                zip.AddFiles(filesToInclude, "files");
                zip.Save(Response.OutputStream);
            }
        
            outputStream.Position = 0;
            return File(outputStream, "application/zip", "keys.zip");
        }

        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files, FormCollection collection)
        {
            int senderID = Convert.ToInt32(collection.Get("ddlFrom"));
            int receiverID = Convert.ToInt32(collection.Get("ddlTo"));
            int typeEncryptie = Convert.ToInt32(collection.Get("typeEncryptie"));

            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("Index");
        }


    

    }
}