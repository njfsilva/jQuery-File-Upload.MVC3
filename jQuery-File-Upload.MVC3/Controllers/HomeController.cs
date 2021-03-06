﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQuery_File_Upload.MVC3.Controllers
{
    public class HomeController : Controller
    {
        private string StorageRoot => Path.Combine(Server.MapPath("~/Files/"));

        public ActionResult Index()
        {
            return View();
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        [HttpGet]
        public void Delete(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/Files"), filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/Files"), filename);

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        [HttpPost]
        public ActionResult UploadFiles()
        {
            var r = new List<ViewDataUploadFilesResult>();

            if (!Request.Files.Cast<object>().Any()) return Json(r);
            var statuses = new List<ViewDataUploadFilesResult>();
            var headers = Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(Request, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], Request, statuses);
            }

            var result = Json(statuses);
            result.ContentType = "text/plain";

            return result;
        }

        private void UploadPartialFile(string fileName, HttpRequestBase request, ICollection<ViewDataUploadFilesResult> statuses)
        {
            if (request.Files.Count != 1)
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");

            var file = request.Files[0];

            if (file != null)
            {
                var fileStreamFromRequest = file.InputStream;
                var fullName = StorageRoot + Path.GetFileName(fileName);

                var fileDescription = request.Form["fileDescription[]"].Split(',').Last();
                var maxChunkSize = Convert.ToInt64(request.Form["maxChunkSize[]"].Split(',').Last());
                var associatedSignatureFile = request.Form["certificateFileName[]"].Split(',').Last();
                var isSignatureFile = request.Form["useSignatureFile"] != null && request.Form["useSignatureFile"].Equals("on"); //este argumento so vem no envio do ficheiro de assinatura, nos normais nao
                var saveToCompanyDocs = request.Form["saveToCompanyDocs[]"] != null && request.Form["saveToCompanyDocs[]"].Equals("on"); //este argumento so vem no envio do ficheiro de assinatura, nos normais nao
                
                var last = fileStreamFromRequest.Length < maxChunkSize; //FUCK THIS LINE OF CODE IN PARTICULAR!!!!!!!

                using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
                {
                    fileStreamFromRequest.CopyTo(fs);
                    fs.Close();
                }

                if (last)
                {
                    //cenas e tal
                }

                statuses.Add(new ViewDataUploadFilesResult
                {
                    name = fileName,
                    size = file.ContentLength,
                    type = file.ContentType,
                    description = fileDescription,
                    associatedSignatureFile = associatedSignatureFile,
                    url = string.Empty,
                    delete_url = string.Empty,
                    delete_type = "GET",
                    isSignatureFile = isSignatureFile ? "yes" : "no",
                    saveToCompanyDocs = saveToCompanyDocs ? "Yes" : "No"
                });
            }
        }

        private void UploadWholeFile(HttpRequestBase request, ICollection<ViewDataUploadFilesResult> statuses)
        {
            for (var i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];

                if (file?.FileName == null) continue;

                var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.FileName));

                var fileDescription = request.Form["fileDescription[]"].Split(',').Last();
                var associatedSignatureFile = request.Form["certificateFileName[]"].Split(',').Last();
                var isSignatureFile = request.Form["useSignatureFile"] != null && request.Form["useSignatureFile"].Equals("on"); //este argumento so vem no envio do ficheiro de assinatura, nos normais nao
                var saveToCompanyDocs = request.Form["saveToCompanyDocs[]"] != null && request.Form["saveToCompanyDocs[]"].Equals("on"); //este argumento so vem no envio do ficheiro de assinatura, nos normais nao

                file.SaveAs(fullPath);

                statuses.Add(new ViewDataUploadFilesResult
                {
                    name = file.FileName,
                    size = file.ContentLength,
                    type = file.ContentType,
                    description = fileDescription,
                    associatedSignatureFile = associatedSignatureFile,
                    url = string.Empty,
                    delete_url = string.Empty,
                    delete_type = "GET",
                    isSignatureFile = isSignatureFile ? "yes" : "no",
                    saveToCompanyDocs = saveToCompanyDocs ? "Yes" : "No"
                });
            }
        }
    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string associatedSignatureFile { get; set; }
        public string url { get; set; }
        public string delete_url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_type { get; set; }
        public string isSignatureFile { get; set; }
        public string saveToCompanyDocs { get; set; }
    }
}
