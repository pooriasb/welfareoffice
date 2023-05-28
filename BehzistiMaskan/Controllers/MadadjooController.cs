using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Services.Description;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using MD.PersianDateTime;

namespace BehzistiMaskan.Controllers
{
    public class MadadjooController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public MadadjooController()
        {
            _dbContext = new ApplicationDbContext();
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();

        }


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            const string culture = "en-US";
            var ci = CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        // GET: Madadjoo
        public ActionResult Index(MessageE? message)
        {
            var question = new SecurityCaptcha().GenerateQuestion();
            var viewModel = new MadadjooLoginRegisterViewModel
            {
                SecurityQuestionDto = new SecurityQuestionDto { Question = question.Question, QuestionId = question.Id }
            };
            ViewBag.Message = message;
            return View(viewModel);
        }


        public ActionResult test()
        {
            return View();
        }


        public ActionResult Register()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(MadadjooLoginRegisterViewModel viewModel)
        {

            // چون در قسمت ثبت نام هستیم فیلدهای ضروری مربوط به ورود از بررسی حذف می گردد.
            ModelState.Remove("LoginActivationCode");
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            // بررسی کد امنیتی
            var securityCaptcha = new SecurityCaptcha();
            var isSecurityCodeValid = securityCaptcha.IsValid(viewModel.SecurityQuestionDto.QuestionId, viewModel.SecurityAnswer);
            if (!isSecurityCodeValid)
            {
                ModelState.AddModelError("SecurityAnswer", "کد امنیتی به درستی وارد نشده است");
                var question = securityCaptcha.GenerateQuestion();
                viewModel.SecurityQuestionDto.Question = question.Question;
                viewModel.SecurityQuestionDto.QuestionId = question.Id;

                return View("Index", viewModel);
            }

            // اگر به عنوان مددجوی تایید شده در سامانه ثبت نام شده بود
            var isApprovedAsClient = _dbContext.Clients.Any(c => c.Person.NationalCode == viewModel.NationalCode);

            if (isApprovedAsClient)
            {
                return RedirectToAction("Index", new { Message = MessageE.ClientAlreadyApproved });
            }

            //اگر به عنوان مددجوی موجود در لیست انتظار ثبت شده بود
            var isAlreadyRegistered = _dbContext.ClientWaitingApplicants.Any(c => c.NationalCode == viewModel.NationalCode);

            if (isAlreadyRegistered)
            {
                return RedirectToAction("Index", new { Message = MessageE.ClientAlreadyRegistered });
            }

            // اگر به عنوان عضوی از خانواده یک مددجو ثبت شده بود
            var isStoreAsClientFamily = _dbContext.FamilyRelations.Any(f => f.PersonMinor.NationalCode == viewModel.NationalCode);

            if (isStoreAsClientFamily)
            {
                return RedirectToAction("Index", new { Message = MessageE.UsedAsClientFamily });
            }




            // -------------------------------------------------------------
            //بعدا تاییدیه نرم افزار پیمنت باید در این قسمت قرار داده شود
            // --------------------------------------------------------------


            var mapper = MapperUtils.MapperConfiguration.CreateMapper();

            var allCounty = _dbContext.Counties.Select(mapper.Map<CountyDto>).ToList();
            var newViewModel = new MadadjooRegisterViewModel
            {
                ClientWaitingApplicant = new ClientWaitingApplicantDto { NationalCode = viewModel.NationalCode, Birthdate = PersianDateTime.Today.ToString() },
                AllProvinces = _dbContext.Provinces.Select(mapper.Map<ProvinceDto>).ToList(),
                MarriageTypes = _dbContext.MarriageTypes.ToList(),
                GenderTypes = _dbContext.GenderTypes.ToList(),
                AllCounties = allCounty,

                CountyOfBirthList = allCounty,
                DistrictOfBirthList = new List<DistrictDto>(),
                CityOfBirthList = new List<CityDto>(),

                ClientTypes = _dbContext.ClientTypes.ToList(),
                CurrentHouseTypes = _dbContext.CurrentHouseTypes.ToList(),


                CountyOfBehzistiDocumentList = allCounty,
                DistrictOfBehzistiDocumentList = new List<DistrictDto>(),
                CityOfBehzistiDocumentList = new List<CityDto>(),

                RequestTypes = _dbContext.RequestTypes.ToList(),


            };

            return View("MadadjooForm", newViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(MadadjooLoginRegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            // بررسی کد امنیتی
            var securityCaptcha = new SecurityCaptcha();
            var isSecurityCodeValid = securityCaptcha.IsValid(viewModel.SecurityQuestionDto.QuestionId, viewModel.SecurityAnswer);
            if (!isSecurityCodeValid)
            {
                ModelState.AddModelError("SecurityAnswer", "کد امنیتی به درستی وارد نشده است");
                var question = securityCaptcha.GenerateQuestion();
                viewModel.SecurityQuestionDto.Question = question.Question;
                viewModel.SecurityQuestionDto.QuestionId = question.Id;
                return View("Index", viewModel);
            }


            var clientUserInDb = _dbContext.ClientUsers.SingleOrDefault(c => c.NationalCode == viewModel.NationalCode && c.ActivationCode == viewModel.LoginActivationCode);
            if (clientUserInDb == null)
            {
                return RedirectToAction("Index", new { message = MessageE.ClientUserNotFound });
            }


            var waitingApplicantInDb = _dbContext.ClientWaitingApplicants
                .Include(w => w.CityOfBirth)
                .Include(w => w.CityOfBirth.District)
                .Include(w => w.CityOfBirth.District.County)
                .Include(w => w.City)
                .Include(w => w.City.District)
                .Include(w => w.City.District.County)
                .Include(w => w.HouseCity)
                .Include(w => w.HouseCity.District)
                .Include(w => w.HouseCity.District.County)
                .Include(w=>w.ClientWaitingApplicantLogs)
                .SingleOrDefault(c => c.NationalCode == viewModel.NationalCode);

            if (waitingApplicantInDb != null)
            {

                //waitingApplicantInDb.Birthdate = ConvertDate.ToJalali(waitingApplicantInDb.Birthdate);
                var allCounty = _dbContext.Counties.Select(_mapper.Map<CountyDto>).ToList();

                var allRequests =
                    _dbContext.ClientWaitingApplicantRequests.Where(c => c.ClientWaitingApplicantId == waitingApplicantInDb.Id)
                        .Include(c => c.RequestType)
                        .ToList();


                var editViewModel = new MadadjooRegisterViewModel
                {
                    ClientWaitingApplicant = _mapper.Map<ClientWaitingApplicantDto>(waitingApplicantInDb),
                    AllProvinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList(),
                    MarriageTypes = _dbContext.MarriageTypes.ToList(),
                    GenderTypes = _dbContext.GenderTypes.ToList(),
                    AllCounties = allCounty,


                    CountyOfBirthList = allCounty,
                    DistrictOfBirthList = _dbContext.Districts.Where(d => d.CountyId == waitingApplicantInDb.CityOfBirth.District.CountyId)
                        .AsEnumerable().Select(_mapper.Map<DistrictDto>).ToList(),
                    CityOfBirthList = _dbContext.Cities.Where(c => c.DistrictId == waitingApplicantInDb.CityOfBirth.DistrictId)
                        .AsEnumerable().Select(_mapper.Map<CityDto>).ToList(),

                    ClientTypes = _dbContext.ClientTypes.ToList(),

                    CurrentHouseTypes = _dbContext.CurrentHouseTypes.ToList(),

                    HouseCountyId = waitingApplicantInDb.HouseCity.District.CountyId,
                    HouseDistrictId = waitingApplicantInDb.HouseCity.DistrictId,

                    ProvinceOfBirthId = waitingApplicantInDb.CityOfBirth.District.County.ProvinceId,
                    CountyOfBirthId = waitingApplicantInDb.CityOfBirth.District.CountyId,
                    DistrictOfBirthId = waitingApplicantInDb.CityOfBirth.DistrictId,

                    ProvinceOfBehzistiDocumentId = waitingApplicantInDb.City.District.County.ProvinceId,
                    CountyOfBehzistiDocumentList = allCounty,
                    DistrictOfBehzistiDocumentList = _dbContext.Districts.Where(d => d.CountyId == waitingApplicantInDb.City.District.CountyId)
                        .AsEnumerable().Select(_mapper.Map<DistrictDto>),
                    CityOfBehzistiDocumentList = _dbContext.Cities.Where(c => c.DistrictId == waitingApplicantInDb.City.DistrictId)
                        .AsEnumerable().Select(_mapper.Map<CityDto>).ToList(),

                    CountyOfBehzistiDocumentId = waitingApplicantInDb.City.District.CountyId,
                    DistrictOfBehzistiDocumentId = waitingApplicantInDb.City.DistrictId,

                    RequestTypes = _dbContext.RequestTypes.ToList(),

                    RequestTypeBuildingId = allRequests.Single(r => r.RequestType.IsExemption == false).RequestTypeId,

                    IsRequestWaterExemption = allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionWater),
                    IsRequestGasExemption = allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionGas),
                    IsRequestElectricalExemption = allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionElectrical),
                    IsRequestProductionLicenseExemption = allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionProductionLicense),
                    ClientWaitingApplicantLogs = waitingApplicantInDb.ClientWaitingApplicantLogs,
                };

                return View("MadadjooForm", editViewModel);
            }

            var clientInDb = _dbContext.Clients.SingleOrDefault(c => c.Person.NationalCode == viewModel.NationalCode);

            if (clientInDb != null)
            {
                ViewBag.clientInDb = clientInDb;
                ViewBag.IsClientLogin = true;

                //return RedirectToAction("EditMadadjoo")
            }
            else
            {
                // بعدا باید با صفحه چنین مددجویی وجود ندارد تعویض شود
                return HttpNotFound();
            }

            //Session["ClientUser"]
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(MadadjooRegisterViewModel viewModel)
        {
            //در صورت وجود خطا
            if (!ModelState.IsValid)
            {
                var mapper = MapperUtils.MapperConfiguration.CreateMapper();

                viewModel.AllProvinces = _dbContext.Provinces.Select(mapper.Map<ProvinceDto>).ToList();
                viewModel.MarriageTypes = _dbContext.MarriageTypes.ToList();
                viewModel.GenderTypes = _dbContext.GenderTypes.ToList();
                viewModel.AllCounties = _dbContext.Counties.Select(mapper.Map<CountyDto>).ToList();
                viewModel.ClientTypes = _dbContext.ClientTypes.ToList();
                viewModel.CurrentHouseTypes = _dbContext.CurrentHouseTypes.ToList();


                viewModel.CountyOfBirthList = viewModel.AllCounties;
                viewModel.DistrictOfBirthList = new List<DistrictDto>();
                viewModel.CityOfBirthList = new List<CityDto>();

                viewModel.ClientTypes = _dbContext.ClientTypes.ToList();
                viewModel.CurrentHouseTypes = _dbContext.CurrentHouseTypes.ToList();


                viewModel.CountyOfBehzistiDocumentList = viewModel.AllCounties;
                viewModel.DistrictOfBehzistiDocumentList = new List<DistrictDto>();
                viewModel.CityOfBehzistiDocumentList = new List<CityDto>();

                viewModel.RequestTypes = _dbContext.RequestTypes.ToList();

                return View("MadadjooForm", viewModel);
            }

            var isEditMode = viewModel.ClientWaitingApplicant.Id != 0;
            var clientId = viewModel.ClientWaitingApplicant.Id;

            // تاریخ شمسی را به میلادی تبدیل می کند
            viewModel.ClientWaitingApplicant.Birthdate = viewModel.ClientWaitingApplicant.Birthdate;
            int newWaitingApplicantId = 0;
            if (isEditMode)
            {
                var waitingApplicantInDb =
                    _dbContext.ClientWaitingApplicants.Single(c => c.Id == clientId);
                _mapper.Map(viewModel.ClientWaitingApplicant, waitingApplicantInDb);
                waitingApplicantInDb.ClientTypeDescription = viewModel.ClientWaitingApplicant.ClientTypeDescription;
                viewModel.ClientWaitingApplicant.UpdatedDate = DateTime.Now;
                string[] birthDate = viewModel.ClientWaitingApplicant.Birthdate.ToString().Split(' ')[0].Split('/');
                var pc = new PersianCalendar();
                DateTime dt = new DateTime(int.Parse(birthDate[0]), int.Parse(birthDate[1]), int.Parse(birthDate[2]), pc);
                waitingApplicantInDb.Birthdate = dt.Date;
                newWaitingApplicantId = waitingApplicantInDb.Id;
            }
            else
            { viewModel.ClientWaitingApplicant.CreatedDate = DateTime.Now;
                var newApplicant = _mapper.Map<ClientWaitingApplicant>(viewModel.ClientWaitingApplicant);
                string[] birthDate = newApplicant.Birthdate.ToString().Split(' ')[0].Split('/');

             var pc = new  PersianCalendar();
             DateTime dt = new DateTime(int.Parse(birthDate[2]), int.Parse(birthDate[0]), int.Parse(birthDate[1]),pc);
             newApplicant.Birthdate = dt.Date;
             newApplicant.ClientTypeDescription = viewModel.ClientWaitingApplicant.ClientTypeDescription;
                _dbContext.ClientWaitingApplicants.Add(newApplicant);
                _dbContext.SaveChanges();
                newWaitingApplicantId = newApplicant.Id;
            }

            // ثبت متقاضی جدید در پایگاه داده
            _dbContext.SaveChanges();
            viewModel.ClientWaitingApplicant.Id = newWaitingApplicantId;





            //ثبت تاریخچه
            var log = new ClientWaitingApplicantLog
            {
                ClientWaitingApplicantId = viewModel.ClientWaitingApplicant.Id,
                CreatedAt = DateTime.Now,
            };
            log.Description = isEditMode ? "اطلاعات متقاضی توسط خود وی ویرایش و ذخیره گردید." : "اطلاعات متقاضی توسط خود وی برای اولین بار ثبت گردید.";
            _dbContext.ClientWaitingApplicantLogs.Add(log);

            _dbContext.SaveChanges();

            //ثبت اطلاعات مربوط به نوع تقاضا در پایگاه داده

            //نوع تقاضای مسکن - فقط یک مورد را می توانست انتخاب کند
            if (isEditMode)
            {
                var requestInDb =
                    _dbContext.ClientWaitingApplicantRequests.Single(r => r.ClientWaitingApplicantId == clientId && r.RequestType.IsExemption == false);
                if (requestInDb.RequestTypeId != viewModel.RequestTypeBuildingId)
                    requestInDb.RequestTypeId = viewModel.RequestTypeBuildingId;
            }
            else
            {
               
                var buildingRequest = new ClientWaitingApplicantRequest
                {
                    RequestTypeId = viewModel.RequestTypeBuildingId,
                    ClientWaitingApplicantId = viewModel.ClientWaitingApplicant.Id
                };

                _dbContext.ClientWaitingApplicantRequests.Add(buildingRequest);
            }

            _dbContext.SaveChanges();

            var allRequestTypes = _dbContext.RequestTypes.ToList();

            if (isEditMode)
            {
                //delete all exemption request for this client

                var allExemptionRequest =
                    _dbContext.ClientWaitingApplicantRequests.Where(r => r.RequestType.IsExemption);
                _dbContext.ClientWaitingApplicantRequests.RemoveRange(allExemptionRequest);
                _dbContext.SaveChanges();
            }


            // ذخیره تقاضاهای مربوط به معافقیت انشعابات
            if (viewModel.IsRequestWaterExemption)
            {
                var requestId = allRequestTypes.Single(r => r.Name == ClientRequestTypeStr.ExemptionWater).Id;
                var request = new ClientWaitingApplicantRequest { ClientWaitingApplicantId = viewModel.ClientWaitingApplicant.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(request);
            }

            if (viewModel.IsRequestGasExemption)
            {
                var requestId = allRequestTypes.Single(r => r.Name == ClientRequestTypeStr.ExemptionGas).Id;
                var request = new ClientWaitingApplicantRequest { ClientWaitingApplicantId = viewModel.ClientWaitingApplicant.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(request);
            }


            if (viewModel.IsRequestElectricalExemption)
            {
                var requestId = allRequestTypes.Single(r => r.Name == ClientRequestTypeStr.ExemptionElectrical).Id;
                var request = new ClientWaitingApplicantRequest { ClientWaitingApplicantId = viewModel.ClientWaitingApplicant.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(request);
            }

            if (viewModel.IsRequestProductionLicenseExemption)
            {
                var requestId = allRequestTypes.Single(r => r.Name == ClientRequestTypeStr.ExemptionProductionLicense).Id;
                var request = new ClientWaitingApplicantRequest { ClientWaitingApplicantId = viewModel.ClientWaitingApplicant.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(request);
            }

            _dbContext.SaveChanges();

            if (isEditMode)
            {
                return View("EditSuccessful");
            }

            var clientUser = new ClientUser
                {
                    NationalCode = viewModel.ClientWaitingApplicant.NationalCode, IsApproved = false,
                    ActivationCode = ActivationServer.GenerateActivationCode()
                };

                _dbContext.ClientUsers.Add(clientUser);

                _dbContext.SaveChanges();

                var successViewModel = new ClientWaitingListSuccessfulRegisterViewMode
                {
                    IsMale = viewModel.ClientWaitingApplicant.GenderTypeId == 1,
                    Name = viewModel.ClientWaitingApplicant.Name,
                    Family = viewModel.ClientWaitingApplicant.Family,
                    NationalCode = viewModel.ClientWaitingApplicant.NationalCode,
                    ActivationCode = clientUser.ActivationCode,
                };

                return View("RegisterSuccess", successViewModel);
            
        }

        public enum MessageE
        {
            ClientAlreadyApproved,
            ClientAlreadyRegistered,
            UsedAsClientFamily,
            ClientUserNotFound,
            InfoCannotFindInBehzistiPayment

        }
    }
}