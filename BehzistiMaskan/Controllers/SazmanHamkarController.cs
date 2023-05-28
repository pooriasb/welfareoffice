using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using Microsoft.AspNet.Identity;

namespace BehzistiMaskan.Controllers
{
    [Authorize(Roles = RoleName.SazmanHamkar)]
    public class SazmanHamkarController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public SazmanHamkarController()
        {
            _mapper = MapperUtils.MapperConfiguration.CreateMapper(); ;
            _dbContext = new ApplicationDbContext();
        }


        // GET: SazmanHamkar
        public ActionResult Index()
        {

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos.Include(uInf => uInf.CoOrganizationType)
                .Include(u => u.Province)
                .Include(u => u.County)
                .Single(uInf => uInf.User.Id == userId);
            //var coOrganizationTypeId = userInfoInDb.CoOrganizationTypeId;
            //var provinceId = userInfoInDb.ProvinceId;
            //var countyId = userInfoInDb.CountyId;

            // فقط لیست مددجویانی که شرایط زیر را داشته باشند
            // شرط اول: در طرح ثبت شده باشند و سازمان این کاربر در آن طرح همکار بهزیستی باشد
            // شرط دوم: فقط مددجویان مربوط به استان خود را می بینند
            // شرط سوم: فقط مددجویان مربوط به شهرستان خود را می بینند

            
            //var clientIdsInScopeOfSazmanHamkarUser = _dbContext.ClientForms.Where(cf => formIdsByCoOrganization.Contains(cf.FormId)).Select(cf => cf.ClientId);

            //var clientCount = _dbContext.Clients.Count(c => clientIdsInScopeOfSazmanHamkarUser.Contains(c.Id));


            var clientsInDb = _dbContext.Clients
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.ClientForms)
                .Where(c => c.ClientForms.Any(cf =>
                                cf.Form.FormCoOrganizationRoles.Any(fco =>
                                    fco.CoOrganizationTypeId == userInfoInDb.CoOrganizationTypeId)) &&
                            c.City.District.County.ProvinceId == userInfoInDb.ProvinceId);



            if (userInfoInDb.CountyId != null)
            {
                clientsInDb = clientsInDb.Where(c => c.City.District.CountyId == userInfoInDb.CountyId);
            }


            var clientCount = clientsInDb.Count();

            var formIdsByCoOrganization = _dbContext.FormCoOrganizationRoles
                .Where(fc => fc.CoOrganizationTypeId == userInfoInDb.CoOrganizationTypeId && fc.Form.ProvinceId == userInfoInDb.ProvinceId).Select(fc => fc.FormId);

            var allFormsInDb = _dbContext.Forms
                .Include(f => f.FormPhysicalProgresses)
                .Include(f => f.FormPhysicalProgresses.Select(ph => ph.PhysicalProgress))
                .Include(f => f.ClientForms)
                .Include(f => f.ClientForms.Select(cf => cf.Client))
                .Where(f => formIdsByCoOrganization.Contains(f.Id))
                .ToList();
            var formsInDb = allFormsInDb.Select(_mapper.Map<DashboardFormSimpleData>);

            //آخرین وضعیت اعتبارات طرح ها
            var dashboardFormByMoney = new List<DashboardFormSimpleData>();

            foreach (var form in allFormsInDb)
            {
                var newItem = new DashboardFormSimpleData { Name = form.Name };
                double needed = 0;
                foreach (var formFormPhysicalProgress in form.FormPhysicalProgresses)
                {
                    needed += formFormPhysicalProgress.BehzistiHelpAmount;
                    foreach (var formPhysicalProgressHelp in formFormPhysicalProgress.FormPhysicalProgressHelps)
                    {
                        needed += formPhysicalProgressHelp.HelpAmount;
                    }
                }

                newItem.NeededMoney = needed;
                dashboardFormByMoney.Add(newItem);
            }

            var viewModel = new DashboardSazmanHamkarViewModel
            {
                ClientCount = clientCount,
                DashboardFormSimpleDatas = formsInDb,
                DashboardFormByMoney = dashboardFormByMoney,
            };
            return View(viewModel);
        }
    }
}