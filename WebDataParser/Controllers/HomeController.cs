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
        public ActionResult Index(string testId) {

            if(testId == null) {
                testId = "average";
            }

            int totalCount = TestDataViewModelFactory.GetTotalTestCount();
            if(testId == "average") {
                ViewBag.NextLink = 1;
                ViewBag.PrevLink = totalCount;
            }
            else {
                int curr = Int32.Parse(testId);
                if(curr + 1 > totalCount) {
                    ViewBag.NextLink = "average";
                }
                else {
                    ViewBag.NextLink = (curr + 1).ToString();
                }
                if(curr - 1 < 1) {
                    ViewBag.PrevLink = "average";
                }
                else {
                    ViewBag.PrevLink = (curr - 1).ToString();
                }
            }

            return View(TestDataViewModelFactory.GetTest(testId));
        }

        public ActionResult LiveView() {            

            return View();
        }

        public JsonResult GetTechniqueData() {
            /*
            var test = new[] {
                new[] { 1, 2,5 },
                new[] { 3, 4,7 },
                new[] { 4, 5 ,9},
                new[] { 6, 7,0 }
            };
            return Json(test, JsonRequestBehavior.AllowGet);
            
            */
            var attempts = DataGenerator.Database.Attempts;
            var info = new TechniqueInformationViewModel(attempts);
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImage(string testId, string type) {

            return File(TestDataViewModelFactory.GetHitbox(testId, type).ToArray(), "image/png");
           
        }
    }
}