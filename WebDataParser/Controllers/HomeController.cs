using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebDataParser;

namespace WebDataParser.Controllers {
    public class HomeController : Controller {
        public ActionResult Index(string testId) {
            HttpServerUtilityBase utilityBase = HttpContext.Server;

            if(testId == null) {
                testId = "average";
            }

            int totalCount = TestDataViewModelFactory.GetTotalTestCount(utilityBase);
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

            return View(TestDataViewModelFactory.GetTest(utilityBase, testId));
        }

        public ActionResult GetImage(string testId, string type) {

            return File(TestDataViewModelFactory.GetHitbox(testId, type).ToArray(), "image/png");
           
        }
    }
}