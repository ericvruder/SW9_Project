using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DataSetGenerator;
using WebDataParser.Models;

namespace WebDataParser.Controllers {
    public class HomeController : Controller {
        public ActionResult Index(DataSource source = DataSource.Old) {
            ViewBag.Source = source;
            return View();
        }

        public ActionResult GetData(string data ="", DataSource source= DataSource.Old) {

            string fileName = "", filePath = "";
            switch (data) {
                case "result": fileName = "AllResults.spv"; break;
                case "data": fileName = "AllData.sav"; break;
                case "datacsv": fileName = "target_data.csv";break;
                default: fileName = "AllData.sav"; break;
            }

            switch (source) {
                default: filePath = AppDomain.CurrentDomain.BaseDirectory + "/Data/SW9/" + fileName; break;
            }
            
            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            string contentType = MimeMapping.GetMimeMapping(filePath);

            var cd = new System.Net.Mime.ContentDisposition {
                FileName = fileName,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        public ActionResult UserInfo(string testId, DataSource source = DataSource.Old) {

            if (testId == null) {
                testId = "average";
            }

            int totalCount = AttemptRepository.GetTestCount(source);
            if (testId == "average") {
                ViewBag.NextLink = 1;
                ViewBag.PrevLink = totalCount;
            }
            else {
                int curr = Int32.Parse(testId);
                if (curr + 1 > totalCount) {
                    ViewBag.NextLink = "average";
                }
                else {
                    ViewBag.NextLink = (curr + 1).ToString();
                }
                if (curr - 1 < 1) {
                    ViewBag.PrevLink = "average";
                }
                else {
                    ViewBag.PrevLink = (curr - 1).ToString();
                }
            }

            return View(TestDataViewModelFactory.GetTest(testId, source));
        }

        public JsonResult GetTechniqueData(DataSource source = DataSource.Old) {
            var attempts = AttemptRepository.GetAttempts(source);
            var count = AttemptRepository.GetTestCount(source);
            var info = new TechniqueInformationViewModel(attempts, count);
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImage(string testId, string type) {

            return File(TestDataViewModelFactory.GetHitbox(testId, type).ToArray(), "image/png");
           
        }
    }
}