using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BasicSecurity.Models;
using System.IO;
using Ionic.Zip;

namespace BasicSecurity.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/BLABLABLA/"), fileName);
                    file.SaveAs(path);
                }
            }

            return RedirectToAction("UploadDocument");
        }


    

    }
}