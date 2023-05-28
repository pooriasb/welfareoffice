using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BehzistiMaskan.Controllers
{
    [Authorize(Roles = RoleName.SystemAdministrator)]
    public class SettingController : Controller
    {
        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EditCounty(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveCounty(CountyDto county)
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteCounty()
        {
            return View();
        }
        public ActionResult EditCity(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveCity()
        {
            return View();
        }
        public ActionResult EditDistrict(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveDistrict()
        {
            return View();
        }
    }
}