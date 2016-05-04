using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DataSetGenerator;
using WebDataParser.Models;
using Newtonsoft.Json;

namespace WebDataParser.Controllers {
    public class HomeController : Controller {
        public ActionResult Index(DataSource? source) {
            if (source == null) {
                var cookie = Request.Cookies["source"];
                if (IsValidCookie(cookie)) {
                    ViewBag.Source = cookie.Value;
                }
                else {
                    ViewBag.Source = "Old";
                }
            }
            else
                ViewBag.Source = source;
            return View();
        }

        private bool IsValidCookie(HttpCookie cookie) {
            if (cookie == null) return false;
            if (cookie.Value == "Old") return true;
            if (cookie.Value == "Target") return true;
            if (cookie.Value == "Field") return true;
            return false;
        }

        public ActionResult GetData(string data ="", DataSource source= DataSource.Old) {
            string fileName = $"{source}data", filePath = "";
            switch (data) {
                //case "csv": fileName = DataGenerator.GenerateCSVDocument(source, Path.GetTempPath()); fileName += ".csv";  break;
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

        public ContentResult GetAttemptData(DataSource source) {
            var tests = AttemptRepository.GetTests(source);
            var attemptInfos = new List<AttemptInfo>();
            attemptInfos.Add(new AttemptInfo(tests, GestureDirection.Pull));
            attemptInfos.Add(new AttemptInfo(tests, GestureDirection.Push));
            var json = $"{{\"Pull\": {JsonConvert.SerializeObject(attemptInfos[0])}, \"Push\": {JsonConvert.SerializeObject(attemptInfos[1])}}}";
            return Content(json, "application/json");
        }

        public ActionResult GetImage(string testId, GestureType type, DataSource source = DataSource.Old) {
            List<Attempt> attempts = new List<Attempt>();
            if(testId != "average") {
                attempts = AttemptRepository.GetTest(testId, source).Attempts[type];
            }
            else {
                var tests = AttemptRepository.GetTests(source);
                foreach(var test in tests) {
                    attempts.AddRange(test.Attempts[type]);
                }
            }
            MemoryStream ms;
            DataVisualizer.DrawHitBox(attempts, out ms);
            return File(ms, "image/png");

        }
    }
}