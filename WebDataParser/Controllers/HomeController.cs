using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DataSetGenerator;

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

            var allAttempts = (from attempt in DataGenerator.Database.Attempts
                                       select attempt).ToList();

            List<TechniqueInfo> techInfo = new List<TechniqueInfo>();

            foreach(var technique in DataGenerator.AllTechniques) {
                foreach(var direction in DataGenerator.AllDirections) {
                    var selected = allAttempts.Where(x => x.Type == technique && x.Direction == direction); 

                    techInfo.Add(new TechniqueInfo(selected.ToList()));
                }
            }                  

            return View(techInfo);
        }

        public ActionResult GetImage(string testId, string type) {

            return File(TestDataViewModelFactory.GetHitbox(testId, type).ToArray(), "image/png");
           
        }
    }
}