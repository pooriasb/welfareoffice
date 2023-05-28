using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;

namespace BehzistiMaskan.Controllers
{
    [AllowAnonymous]
    public class MaskanController : Controller
    {
        private ApplicationDbContext _dbContext;

        public MaskanController()
        {
            _dbContext = new ApplicationDbContext();
        }

        // GET: Maskan
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Madadjoo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MadadjooRegister(string nationalCode)
        {
            return null;
        }

        [HttpPost]
        public ActionResult MadadjooLogin(string nationalCode, string activationCode)
        {
            return null;
        }

        public ActionResult Cooperator()
        {
            return View();
        }
    }
}