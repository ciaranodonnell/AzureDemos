using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzureDemos.QStorage.WritingRecognizer.Web.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace AzureDemos.QStorage.WritingRecognizer.Web.Controllers
{
    public class HomeController : Controller
    {
        private WritingRecognizerClient writingRecognizerClient;

        public HomeController(WritingRecognizerClient client)
        {
            this.writingRecognizerClient = client;
        }



        public IActionResult Index()
        {
            return View();
        }


        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            MemoryStream ms = new MemoryStream();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    await formFile.CopyToAsync(ms);

                    await writingRecognizerClient.SendMessageAsync(
                         new WritingRecognizerRequest
                         {
                             Image = ms.GetBuffer(),
                             OriginLocation = formFile.FileName,
                             RequestId = Guid.NewGuid()
                         });
                    ;
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }





        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
