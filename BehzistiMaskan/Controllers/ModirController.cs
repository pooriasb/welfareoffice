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
    [Authorize(Roles = RoleName.ModirKolOstan + "," + RoleName.MoavenMosharekat + "," + RoleName.MoavenOstan + "," + RoleName.ModirShahrestan)]
    public class ModirController : Controller
    {


        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ModirController()
        {
            _mapper = MapperUtils.MapperConfiguration.CreateMapper(); ;
            _dbContext = new ApplicationDbContext();
        }

        // GET: Modir 
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos
                .Include(ui => ui.Province)
                .Include(ui => ui.County)
                .Include(ui => ui.AssistanceType)
                .Single(ui => ui.Id == userId);

            var allFormsInDb = _dbContext.Forms
                .Include(f => f.FormPhysicalProgresses)
                .Include(f => f.FormPhysicalProgresses.Select(ph => ph.PhysicalProgress))
                .Include(f => f.ClientForms)
                .Include(f => f.ClientForms.Select(cf => cf.Client))
                .Where(f => f.ProvinceId == userInfoInDb.ProvinceId)
                .ToList();

            var formsInDb = allFormsInDb.Select(_mapper.Map<DashboardFormSimpleData>);

            DashboardViewModel viewModel = new DashboardViewModel();

            if (User.IsInRole(RoleName.ModirKolOstan) || User.IsInRole(RoleName.MoavenMosharekat))
            {

                viewModel = new DashboardViewModel
                {
                    ClientCount = _dbContext.Clients.Count(c => c.IsDeleted == false && c.City.District.County.ProvinceId == userInfoInDb.ProvinceId),
                    DashboardFormSimpleDatas = formsInDb,
                    ClientExecutedCount = _dbContext.Clients.Count(c => c.IsIncludedInComprehensiveReport == true && c.City.District.County.ProvinceId == userInfoInDb.ProvinceId),
                    ClientActionCount = _dbContext.Clients.Count(c => c.City.District.County.ProvinceId == userInfoInDb.ProvinceId) +
                                        _dbContext.ClientWaitingApplicants.Count(cw =>
                                            cw.Requests.Any(r => r.RequestType.IsExemption && cw.City.District.County.ProvinceId == userInfoInDb.ProvinceId)),
                    ClientWaitingApplicantCount = _dbContext.ClientWaitingApplicants.Count(cw => cw.City.District.County.ProvinceId == userInfoInDb.ProvinceId),
                };



                //temp code:
                viewModel.ClientExecutedCount = viewModel.ClientActionCount;

                //خلاصه آمار پیشرفت فیزیکی بر اساس طرح
                var formDataByPhysicalProgressesList = new List<DashboardFormDataByPhysicalProgress>();
                foreach (var form in allFormsInDb.Where(f => f.IsEnabled))
                {
                    var newItem = new DashboardFormDataByPhysicalProgress { FormName = form.Name };
                    var phList = new List<FormSimplePhysicalProgress>();

                    var thisFormId = form.Id;

                    foreach (var formFormPhysicalProgress in form.FormPhysicalProgresses)
                    {
                        var thisPhysicalProgressId = formFormPhysicalProgress.PhysicalProgressId;

                        // تعداد مددجویانی که در این طرح ثبت شده و تیک این مرحله از پیشرفت فیزیکی برای آنها خورده است
                        var allClients = _dbContext.Clients
                            .Include(c => c.ClientForms)
                            .Include(c => c.ClientForms.Select(cf => cf.Form))
                            .Include(c => c.ClientPhysicalProgresses)
                            .Include(c => c.ClientPhysicalProgresses.Select(cph => cph.PhysicalProgress));


                        //مددجویانی که در این طرح ثبت شده اند
                        allClients = allClients.Where(c => c.ClientForms.Any(cf => cf.FormId == thisFormId));

                        // و تیک انجام این مرحله پیشرفت فیزیکی برای آنها خورده باشد
                        allClients = allClients.Where(c => c.ClientPhysicalProgresses.Any(cph =>
                            cph.IsDone == true && cph.PhysicalProgressId == thisPhysicalProgressId));

                        // و تیک اتمام مرحله پیشرفت فیزیکی بالاتر از این مرحله را نداشته باشد
                        allClients = allClients.Where(c => !c.ClientPhysicalProgresses.Any(cph =>
                            cph.IsDone == true && cph.PhysicalProgressId > thisPhysicalProgressId));

                        var phItem = new FormSimplePhysicalProgress
                        {
                            PhysicalProgressName = formFormPhysicalProgress.PhysicalProgress.Name,
                            PersonCountInThisStep = allClients.Count()
                        };

                        phList.Add(phItem);
                    }

                    newItem.FormDataByPhysicalProgresses = phList;
                    formDataByPhysicalProgressesList.Add(newItem);
                }

                viewModel.DashboardFormDataByPhysicalProgresses = formDataByPhysicalProgressesList;
                //آخرین وضعیت اعتبارات طرح ها
                var dashboardFormByMoney = new List<DashboardFormSimpleData>();

                foreach (var form in allFormsInDb)
                {
                    var newItem = new DashboardFormSimpleData { Name = form.Name };

                    var formPhysicalProgressHelpsOfThisForm =
                        _dbContext.FormPhysicalProgressHelps.Where(fph => fph.FormPhysicalProgress.FormId == form.Id);

                    if (formPhysicalProgressHelpsOfThisForm.Any())
                        newItem.NeededMoney = formPhysicalProgressHelpsOfThisForm.Sum(f => f.HelpAmount);


                    var financialAidsOfThisForm =
                        _dbContext.FinancialAids.Where(f => f.Client.ClientForms.Any(cf => cf.FormId == form.Id));
                    if (financialAidsOfThisForm.Any())
                        newItem.PayedMoney = financialAidsOfThisForm.Sum(f => f.Amount);

                    dashboardFormByMoney.Add(newItem);
                }

                viewModel.DashboardFormByMoney = dashboardFormByMoney;
            }

            //اگر مدیر شهرستان بود
            else if (User.IsInRole(RoleName.ModirShahrestan))
            {
                viewModel = new DashboardViewModel
                {
                    ClientCount = _dbContext.Clients.Count(c => c.IsDeleted == false && c.City.District.CountyId == userInfoInDb.CountyId),
                    DashboardFormSimpleDatas = formsInDb,
                    ClientExecutedCount = _dbContext.Clients.Count(c => c.IsIncludedInComprehensiveReport == true && c.City.District.CountyId == userInfoInDb.CountyId),
                    ClientActionCount = _dbContext.Clients.Count(c => c.City.District.CountyId == userInfoInDb.CountyId) +
                                        _dbContext.ClientWaitingApplicants.Count(cw =>
                                            cw.Requests.Any(r => r.RequestType.IsExemption && cw.City.District.CountyId == userInfoInDb.CountyId)),
                    ClientWaitingApplicantCount = _dbContext.ClientWaitingApplicants.Count(cw => cw.City.District.CountyId == userInfoInDb.CountyId),
                };



                //temp code:
                viewModel.ClientExecutedCount = viewModel.ClientActionCount;

                //خلاصه آمار پیشرفت فیزیکی بر اساس طرح
                var formDataByPhysicalProgressesList = new List<DashboardFormDataByPhysicalProgress>();
                foreach (var form in allFormsInDb.Where(f => f.IsEnabled))
                {
                    var newItem = new DashboardFormDataByPhysicalProgress { FormName = form.Name };
                    var phList = new List<FormSimplePhysicalProgress>();

                    var thisFormId = form.Id;

                    foreach (var formFormPhysicalProgress in form.FormPhysicalProgresses)
                    {
                        var thisPhysicalProgressId = formFormPhysicalProgress.PhysicalProgressId;

                        // تعداد مددجویانی که در این طرح ثبت شده و تیک این مرحله از پیشرفت فیزیکی برای آنها خورده است
                        var allClients = _dbContext.Clients
                            .Include(c => c.ClientForms)
                            .Include(c => c.ClientForms.Select(cf => cf.Form))
                            .Include(c => c.ClientPhysicalProgresses)
                            .Include(c => c.ClientPhysicalProgresses.Select(cph => cph.PhysicalProgress))
                            .Where(c=>c.City.District.CountyId == userInfoInDb.CountyId);


                        //مددجویانی که در این طرح ثبت شده اند
                        allClients = allClients.Where(c => c.ClientForms.Any(cf => cf.FormId == thisFormId));

                        // و تیک انجام این مرحله پیشرفت فیزیکی برای آنها خورده باشد
                        allClients = allClients.Where(c => c.ClientPhysicalProgresses.Any(cph =>
                            cph.IsDone == true && cph.PhysicalProgressId == thisPhysicalProgressId));

                        // و تیک اتمام مرحله پیشرفت فیزیکی بالاتر از این مرحله را نداشته باشد
                        allClients = allClients.Where(c => !c.ClientPhysicalProgresses.Any(cph =>
                            cph.IsDone == true && cph.PhysicalProgressId > thisPhysicalProgressId));

                        var phItem = new FormSimplePhysicalProgress
                        {
                            PhysicalProgressName = formFormPhysicalProgress.PhysicalProgress.Name,
                            PersonCountInThisStep = allClients.Count()
                        };

                        phList.Add(phItem);
                    }

                    newItem.FormDataByPhysicalProgresses = phList;
                    formDataByPhysicalProgressesList.Add(newItem);
                }

                viewModel.DashboardFormDataByPhysicalProgresses = formDataByPhysicalProgressesList;
                //آخرین وضعیت اعتبارات طرح ها
                var dashboardFormByMoney = new List<DashboardFormSimpleData>();

                foreach (var form in allFormsInDb)
                {
                    var newItem = new DashboardFormSimpleData { Name = form.Name };

                    var formPhysicalProgressHelpsOfThisForm =
                        _dbContext.FormPhysicalProgressHelps.Where(fph => fph.FormPhysicalProgress.FormId == form.Id);

                    if (formPhysicalProgressHelpsOfThisForm.Any())
                        newItem.NeededMoney = formPhysicalProgressHelpsOfThisForm.Sum(f => f.HelpAmount);


                    var financialAidsOfThisForm =
                        _dbContext.FinancialAids.Where(f => f.Client.ClientForms.Any(cf => cf.FormId == form.Id));
                    if (financialAidsOfThisForm.Any())
                        newItem.PayedMoney = financialAidsOfThisForm.Sum(f => f.Amount);

                    dashboardFormByMoney.Add(newItem);
                }

                viewModel.DashboardFormByMoney = dashboardFormByMoney;
            }

            return View(viewModel);
        }

    }
}