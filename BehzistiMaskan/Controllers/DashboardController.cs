using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using MD.PersianDateTime;
using Microsoft.AspNet.Identity;

namespace BehzistiMaskan.Controllers
{
    //[Authorize(Roles = RoleName.KarshenasOstan + "," + RoleName.KarshenasShahrestan + "," + RoleName.KarshenasKeshvar)]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public DashboardController()
        {
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
            _dbContext = new ApplicationDbContext();
        }

        // GET: Karshenas
        public ActionResult Index()
        {

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos
                .Include(ui => ui.Province)
                .Include(ui => ui.County)
                .Single(ui => ui.Id == userId);

            var allFormsInDbRelatedToThisUser = _dbContext.Forms
                .Include(f => f.FormPhysicalProgresses)
                .Include(f => f.FormPhysicalProgresses.Select(ph => ph.PhysicalProgress))
                .Include(f => f.ClientForms)
                .Include(f => f.ClientForms.Select(cf => cf.Client))
                .Include(f => f.FormAccessLevels)
                .ToList();

            var allPhysicalProgressInDbRelatedToThisUser = _dbContext.PhysicalProgresses
                .Include(p => p.ClientPhysicalProgresses)
                .Include(p => p.FormPhysicalProgresses)
                .ToList();


            DashboardViewModel viewModel;
            if (User.IsInRole(RoleName.KarshenasOstan) || User.IsInRole(RoleName.KarshenasMasoolOstan) ||
                User.IsInRole(RoleName.ModirKolOstan) || User.IsInRole(RoleName.MoavenMosharekat))
            { // کارکنان معاونت مشارکت به همراه مدیر کل

                allFormsInDbRelatedToThisUser = allFormsInDbRelatedToThisUser.Where(f => f.ProvinceId == userInfoInDb.ProvinceId).ToList();

                viewModel = new DashboardViewModel
                {
                    ClientCount = _dbContext.Clients.Count(c =>c.IsDeleted == false && c.City.District.County.ProvinceId == userInfoInDb.ProvinceId),
                    DashboardFormSimpleDatas = allFormsInDbRelatedToThisUser.Select(_mapper.Map<DashboardFormSimpleData>),
                    ClientExecutedCount =
                        _dbContext.Clients.Count(c =>
                        c.IsIncludedInComprehensiveReport == true &&
                        c.City.District.County.ProvinceId == userInfoInDb.ProvinceId),
                    ClientActionCount =
                        _dbContext.Clients.Count(c => c.City.District.County.ProvinceId == userInfoInDb.ProvinceId) +
                        _dbContext.ClientWaitingApplicants.Count(cw =>
                            cw.Requests.Any(r => r.RequestType.IsExemption &&
                                cw.City.District.County.ProvinceId == userInfoInDb.ProvinceId)),
                    ClientWaitingApplicantCount = _dbContext.ClientWaitingApplicants.Count(cw =>
                        cw.City.District.County.ProvinceId == userInfoInDb.ProvinceId && cw.IsDeleted == false)
                };

                viewModel = GetExemptionDataForThisYear(viewModel, false, true, userInfoInDb);
                viewModel = GetExemptionDataForLastYear(viewModel, false, true, userInfoInDb);

            }
            else if (User.IsInRole(RoleName.KarshenasShahrestan) || User.IsInRole(RoleName.ModirShahrestan))
            {
                // فقط طرح های مربوط به شهرستان خودش را می بیند
                allFormsInDbRelatedToThisUser = allFormsInDbRelatedToThisUser
                    .Where(f => f.FormAccessLevels.Any(acl => acl.CountyId == userInfoInDb.CountyId)).ToList();

                viewModel = new DashboardViewModel
                {
                    ClientCount = _dbContext.Clients.Count(c => c.IsDeleted == false && c.City.District.CountyId == userInfoInDb.CountyId),
                    DashboardFormSimpleDatas = allFormsInDbRelatedToThisUser.Select(_mapper.Map<DashboardFormSimpleData>),
                    ClientExecutedCount = _dbContext.Clients.Count(c => c.IsIncludedInComprehensiveReport == true && c.City.District.CountyId == userInfoInDb.CountyId),
                    ClientActionCount = _dbContext.Clients.Count(c => c.City.District.CountyId == userInfoInDb.CountyId) +
                                        _dbContext.ClientWaitingApplicants.Count(cw =>
                                            cw.Requests.Any(r => r.RequestType.IsExemption && cw.City.District.CountyId == userInfoInDb.CountyId)),
                    ClientWaitingApplicantCount = _dbContext.ClientWaitingApplicants.Count(cw => cw.City.District.CountyId == userInfoInDb.CountyId && cw.IsDeleted == false),
                };
                viewModel = GetExemptionDataForThisYear(viewModel, true, false, userInfoInDb);
                viewModel = GetExemptionDataForLastYear(viewModel, true, false, userInfoInDb);
            }
            else// if (User.IsInRole(RoleName.KarshenasKeshvar))
            {// کارشناس کشور

                viewModel = new DashboardViewModel
                {
                    ClientCount = _dbContext.Clients.Count(c => c.IsDeleted == false),
                    DashboardFormSimpleDatas =  allFormsInDbRelatedToThisUser.Select(_mapper.Map<DashboardFormSimpleData>),
                    ClientExecutedCount = _dbContext.Clients.Count(c => c.IsIncludedInComprehensiveReport == true),
                    ClientActionCount = _dbContext.Clients.Count() +
                                        _dbContext.ClientWaitingApplicants.Count(cw => cw.Requests.Any(r => r.RequestType.IsExemption)),
                    ClientWaitingApplicantCount = _dbContext.ClientWaitingApplicants.Count(),
                };

                viewModel = GetExemptionDataForThisYear(viewModel);
                viewModel = GetExemptionDataForLastYear(viewModel);
            }

            //کد موقت
            viewModel.ClientExecutedCount = viewModel.ClientActionCount;

            //خلاصه آمار پیشرفت فیزیکی بر اساس طرح
            var formDataByPhysicalProgressesList = new List<DashboardFormDataByPhysicalProgress>();
            foreach (var form in allFormsInDbRelatedToThisUser.Where(f => f.IsEnabled))
            {
                var newItem = new DashboardFormDataByPhysicalProgress { FormName = form.Name };
                var phList = new List<FormSimplePhysicalProgress>();

                var thisFormId = form.Id;

                foreach (var physicalProgress in allPhysicalProgressInDbRelatedToThisUser)
                {
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
                        cph.IsDone && cph.PhysicalProgressId == physicalProgress.Id));

                    // و تیک اتمام مرحله پیشرفت فیزیکی بالاتر از این مرحله را نداشته باشد
                    allClients = allClients.Where(c => !c.ClientPhysicalProgresses.Any(cph =>
                        cph.IsDone && cph.PhysicalProgressId > physicalProgress.Id));

                    var phItem = new FormSimplePhysicalProgress
                    {
                        PhysicalProgressName = physicalProgress.Name,
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

            foreach (var form in allFormsInDbRelatedToThisUser)
            {
                var newItem = new DashboardFormSimpleData { Name = form.Name };

                var formPhysicalProgressHelpsOfThisForm =
                    _dbContext.FormPhysicalProgressHelps.Where(fph => fph.FormPhysicalProgress.FormId == form.Id);
                //دریافت سعم بهزیستی برای ثبت اطلاعات در گزارش
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                long behAmount = _dbContext.FormPhysicalProgresses.Where(x => x.FormId == form.Id)
                    .Sum(x => (long?) x.BehzistiHelpAmount) ?? 0;
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
              
                
                //اعتبار مورد نیاز
                if (formPhysicalProgressHelpsOfThisForm.Any())
                    newItem.NeededMoney = formPhysicalProgressHelpsOfThisForm.Sum(f => f.HelpAmount) +behAmount;


                // تمام کسانی که در این فرم ثبت شده اند
                //var clientsRegisteredInThisForm =
                //    _dbContext.Clients.Where(c => c.ClientForms.Any(cf => cf.FormId == form.Id));

                var financialAidsOfThisForm =
                    _dbContext.FinancialAids.Where(f => f.Client.ClientForms.Any(cf => cf.FormId == form.Id));
               
                
                // اعتبار پرداخت شده
                if (financialAidsOfThisForm.Any())
                    newItem.PayedMoney = financialAidsOfThisForm.Sum(f => f.Amount);

                dashboardFormByMoney.Add(newItem);
            }

            //foreach (var form in allFormsInDbRelatedToThisUser)
            //{
            //    var newItem = new DashboardFormSimpleData { Name = form.Name };

            //    // تمام کسانی که در این فرم ثبت شده اند
            //    var clientsRegisteredInThisForm =
            //        _dbContext.Clients.Where(c => c.ClientForms.Any(cf => cf.FormId == form.Id));

            //}

            viewModel.DashboardFormByMoney = dashboardFormByMoney;

            return View(viewModel);
        }

        public ActionResult Madadjoo()
        {
            return null;
        }


        private DashboardViewModel GetExemptionDataForThisYear(DashboardViewModel viewModel, bool isCountyUser = false, bool isProvinceUser = false, UserInfo userInfo = null)
        {
            var allExemptionRequests = _dbContext.RequestTypes.Where(rt => rt.IsExemption).ToList();

            // این بخش از کد برای به دست آوردن معادل تاریخ میلادی اول فروردین تا 28 اسفند ماه (سال جاری شمسی) می باشد
            var currentYearInShamsi = PersianDateTime.Today.Year;

            var dateFirstDayOfCurrentYearShamsiInMiladi = (new PersianDateTime(currentYearInShamsi, 1, 1)).ToDateTime();
            var dateLastDayOfCurrentYearShamsiInMiladi = (new PersianDateTime(currentYearInShamsi, 12, 28)).ToDateTime();

            // تعداد افرادی که معرفی نامه معافیت انشعابات دریافت کرده اند
            var clientRequestGetLettersInThisProvince = _dbContext.ClientRequestGetLetters
                .Where(gl => gl.LetterDate >= dateFirstDayOfCurrentYearShamsiInMiladi &&
                gl.LetterDate <= dateLastDayOfCurrentYearShamsiInMiladi &&
                (!isProvinceUser || gl.ClientRequest.Client.City.District.County.ProvinceId == userInfo.ProvinceId) &&
                (!isCountyUser || gl.ClientRequest.Client.City.District.CountyId == userInfo.CountyId))
                .Include(gl => gl.ClientRequest)
                .Include(gl => gl.ClientRequest.RequestType)
                .ToList();

            // تعداد افرادی که از معافیت انشعابات بهره مند شده اند
            var clientsWhoBenefitedExemptionInThisProvince = _dbContext.ClientExemptionBenefits
                .Where(eb =>
                    eb.BenefitDate >= dateFirstDayOfCurrentYearShamsiInMiladi &&
                    eb.BenefitDate <= dateLastDayOfCurrentYearShamsiInMiladi &&
                    (!isProvinceUser || eb.ClientRequest.Client.City.District.County.ProvinceId == userInfo.ProvinceId) &&
                    (!isCountyUser || eb.ClientRequest.Client.City.District.CountyId == userInfo.CountyId))
                .Include(eb => eb.ClientRequest)
                .Include(eb => eb.ClientRequest.RequestType)
                .ToList();

            viewModel.DashboardExemptionSimpleDataThisYear = (from requestType in allExemptionRequests
                                                              let getLetterForThisRequestType =
                                                                  clientRequestGetLettersInThisProvince.Where(cr => cr.ClientRequest.RequestTypeId == requestType.Id).ToList()
                                                              let benefitedForThisRequestType =
                                                                  clientsWhoBenefitedExemptionInThisProvince.Where(be => be.ClientRequest.RequestTypeId == requestType.Id).ToList()
                                                              select new DashboardExemptionSimpleData
                                                              {
                                                                  Name = requestType.PersianShortTitle,
                                                                  RequestedNumber = getLetterForThisRequestType.Count,
                                                                  BenefitedNumber = benefitedForThisRequestType.Count,
                                                                  BenefitAmount = benefitedForThisRequestType.Sum(b => b.BenefitAmount)
                                                              }).ToList();

            viewModel.NumberOfPersonWhoRequestExemptionThisYear = clientRequestGetLettersInThisProvince.Count;
            viewModel.NumberOfPersonWhoBenefitedFromExemptionThisYear = clientsWhoBenefitedExemptionInThisProvince.Count;
            viewModel.AmountOfAllExemptionBenefitThisYear = clientsWhoBenefitedExemptionInThisProvince.Sum(b => b.BenefitAmount);

            return viewModel;
        }

        private DashboardViewModel GetExemptionDataForLastYear(DashboardViewModel viewModel, bool isCountyUser = false, bool isProvinceUser = false, UserInfo userInfo = null)
        {

            // این بخش از کد برای به دست آوردن معادل تاریخ میلادی اول فروردین تا 28 اسفند ماه (سال گذشته شمسی) می باشد
            var lastYearInShamsi = PersianDateTime.Today.Year - 1;

            var firstDayOfLastYearShamsiInMiladi = (new PersianDateTime(lastYearInShamsi, 1, 1)).ToDateTime();
            var lastDayOfLastYearShamsiInMiladi = (new PersianDateTime(lastYearInShamsi, 12, 28)).ToDateTime();

            // لیست معرفی نامه های دریافت شده برای معافیت انشعابات
            var allRequestGetLetters = _dbContext.ClientRequestGetLetters
                .Where(gl => gl.LetterDate >= firstDayOfLastYearShamsiInMiladi &&
                             gl.LetterDate <= lastDayOfLastYearShamsiInMiladi &&
                             (!isProvinceUser || gl.ClientRequest.Client.City.District.County.ProvinceId == userInfo.ProvinceId) &&
                             (!isCountyUser || gl.ClientRequest.Client.City.District.CountyId == userInfo.CountyId))
                .Include(gl => gl.ClientRequest)
                .Include(gl => gl.ClientRequest.RequestType)
                .ToList();

            // لیست بهره مندی های ثبت شده برای معافیت انشعابات
            var allBenefitedExemption = _dbContext.ClientExemptionBenefits
                .Where(eb =>
                    eb.BenefitDate >= firstDayOfLastYearShamsiInMiladi &&
                    eb.BenefitDate <= lastDayOfLastYearShamsiInMiladi &&
                    (!isProvinceUser || eb.ClientRequest.Client.City.District.County.ProvinceId == userInfo.ProvinceId) &&
                    (!isCountyUser || eb.ClientRequest.Client.City.District.CountyId == userInfo.CountyId))
                .Include(eb => eb.ClientRequest)
                .Include(eb => eb.ClientRequest.RequestType)
                .ToList();

            var allExemptionRequests = _dbContext.RequestTypes.Where(rt => rt.IsExemption).ToList();

            viewModel.DashboardExemptionSimpleDataLastYear = (from requestType in allExemptionRequests
                                                              let getLetterForThisRequestType = allRequestGetLetters.Where(cr => cr.ClientRequest.RequestTypeId == requestType.Id).ToList()
                                                              let benefitedForThisRequestType = allBenefitedExemption.Where(be => be.ClientRequest.RequestTypeId == requestType.Id).ToList()
                                                              select new DashboardExemptionSimpleData
                                                              {
                                                                  Name = requestType.PersianShortTitle,
                                                                  RequestedNumber = getLetterForThisRequestType.Count,
                                                                  BenefitedNumber = benefitedForThisRequestType.Count,
                                                                  BenefitAmount = benefitedForThisRequestType.Sum(b => b.BenefitAmount)
                                                              }).ToList();

            viewModel.NumberOfPersonWhoRequestExemptionLastYear = allRequestGetLetters.Count;
            viewModel.NumberOfPersonWhoBenefitedFromExemptionLastYear = allBenefitedExemption.Count;
            viewModel.AmountOfAllExemptionBenefitLastYear = allBenefitedExemption.Sum(b => b.BenefitAmount);

            return viewModel;
        }

    }
}