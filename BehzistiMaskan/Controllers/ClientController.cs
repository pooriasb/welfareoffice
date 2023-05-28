using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using MD.PersianDateTime;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace BehzistiMaskan.Controllers
{
    //[Authorize(Roles = RoleName.KarshenasOstan + ", " + RoleName.KarshenasShahrestan + ", " + RoleName.ModirShahrestan + ", " + RoleName.ModirKolOstan + ", " + RoleName.MoavenOstan + ", " +)]
    [Authorize]
    [System.Runtime.InteropServices.Guid("B1F0C300-5780-4FED-A0D7-3E5011E0A411")]
    public class ClientController : Controller
    {
        public enum MessageClientManageId
        {
            YouCannotAccessThisArea,
            YouCannotEditClient,
            YouCannotEditClientInThisStep,
            Error,
            DuplicateNationalCode,
            PersonWasDeleted,
            ClientWasDeleted,
            ClientWaitingApplicantUpdateSuccessfully,
            DataSavedSuccessfully,
            ValidationError,
            RestrictionOnDownloadRequest
        }

        private const string DefaultClientDocumentAddress = "~/ClientDocuments";
        private const string DefaultTempUploadFolder = "~/TempUploadFile";
        private const string DefaultTempZipCacheFolder = "~/TempZipCache";
        private const string PersianDateFormatStr = "yyyy/MM/dd";

        // کد استان فارس به صورت هارد کد وارد شده است
        private const int ProvinceIdFix = 1;
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();

        private readonly IMapper _mapper;

        // Constructor
        public ClientController()
        {
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
        }


        public ActionResult GetClientDataAsZip(int id)
        {
            // temp Comment *********************************
            //var file = new FileInfo(@"C:\BehzistiMaskan\myWorkbook.xlsx");
            //using (var package = new ExcelPackage(file))
            //{
            //    var sheet = package.Workbook.Worksheets.Add("My Sheet");
            //    sheet.Cells["A1"].Value = "Bye World!";

            //    // Save to file
            //    package.Save();
            //}

            //return File(@"C:\BehzistiMaskan\myWorkbook.xlsx", "application/vnd.ms-excel");



            // کاربر سازمان همکار اجازه دانلود اطلاعات مددجو را ندارد
            if (User.IsInRole(RoleName.SazmanHamkar))
                return RedirectToAction("Index", new { message = MessageClientManageId.YouCannotAccessThisArea });

            ClearExpiredTemporaryImages();

            ClearExpiredDownloadRequests();

            var userId = User.Identity.GetUserId();


            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.ClientDocuments)
                .Include(c => c.ClientDocuments.Select(cd => cd.DocumentType))
                .Include(c => c.FormEmtiazBandi)
                .Include(c => c.FormEmtiazBandi.OfflineFormEmtiazBandi)
                .Include(c => c.Person.GenderType)
                .Include(c => c.Person.MarriageType)
                .Include(c => c.ClientForms)
                .Include(c => c.ClientForms.Select(cf => cf.Form))
                .Include(c => c.Person.CityOfBirth)
                .Include(c => c.AssistanceType)
                .Include(c => c.ClientType)
                .SingleOrDefault(c => c.Id == id);

            if (clientInDb == null)
                return HttpNotFound();


            // آدرس پوشه موقت برای ذخیره فایل فشرده را یافته و در صورت نبودن پوشه آن را می سازد
            var tempFolderAddressOfThisUser = Server.MapPath(DefaultTempZipCacheFolder);
            if (!Directory.Exists(tempFolderAddressOfThisUser))
                Directory.CreateDirectory(tempFolderAddressOfThisUser);

            // یک پوشه با کد این کاربر بساز
            // این پوشه برای این ساخته می شود که اگر همزمان دو کاربر برای همین مددجو درخواست دانلود فایل زیپ را ارسال کردند سرور با خطا مواجه نشود
            tempFolderAddressOfThisUser = Path.Combine(tempFolderAddressOfThisUser, userId);
            if (!Directory.Exists(tempFolderAddressOfThisUser))
                Directory.CreateDirectory(tempFolderAddressOfThisUser);






            // در این بخش کنترل می شود که این کاربر در مدت زمان غیر مجاز دو درخواست ارسال نکند
            var downloadRequestForThisClientInDb =
                _dbContext.DownloadRequests.SingleOrDefault(d => d.UserId == userId && d.ClientId == id);

            if (downloadRequestForThisClientInDb != null)
            {
                // یعنی این کاربر برای همین مددجو یک درخواست دانلود ثبت کرده است که هنوز زمان آن نگذشته است
                // بنابراین نمی تواند درخواست دیگری ثبت کند
                // در نتیجه باید به او پیام خطای درخواست غیر مجاز نمایش داده شود

                var alreadyZipFileAddress = $"{tempFolderAddressOfThisUser}\\{clientInDb.Person.NationalCode}.zip";
                // در صورتی که قبلا این فایل ساخته شده بود خود فایل را برگردان
                if (System.IO.File.Exists(alreadyZipFileAddress))
                    return File(alreadyZipFileAddress, "application/zip", Path.GetFileName(alreadyZipFileAddress));

                return RedirectToAction("Index", new { message = MessageClientManageId.RestrictionOnDownloadRequest });
            }


            if (!SystemSettingsManager.HasInitialized)
                SystemSettingsManager.Initialize();


            downloadRequestForThisClientInDb = new DownloadRequest
            {
                UserId = userId,
                ClientId = id,
                RequestTime = DateTime.Now,
                RequestExpireTime = DateTime.Now.AddMinutes(SystemSettingsManager.DownloadRequestRestrictTimeOutInMinuets),
            };

            _dbContext.DownloadRequests.Add(downloadRequestForThisClientInDb);
            _dbContext.SaveChanges();




            // یک پوشه به نام کد ملی مددجو ساخته و در صورتی که قبلا پوشه ای با این نام وجود داشته باشد
            // ابتدا آن را حذف می کند و یک پوشه خالی با همان نام می سازد
            var folderAddressNeedsToBeZipped = Path.Combine(tempFolderAddressOfThisUser, clientInDb.Person.NationalCode);
            if (Directory.Exists(folderAddressNeedsToBeZipped))
                Directory.Delete(folderAddressNeedsToBeZipped, true);
            Directory.CreateDirectory(folderAddressNeedsToBeZipped);


            var currentPersianDateTime = PersianDateTime.Now;
            var excelFileAddress =
                $"{folderAddressNeedsToBeZipped}\\{clientInDb.Person.NationalCode}_{currentPersianDateTime.Year:0000}-{currentPersianDateTime.Month:00}-{currentPersianDateTime.Day:00}_{currentPersianDateTime.Hour:00}-{currentPersianDateTime.Minute:00}-{currentPersianDateTime.Second:00}.xlsx";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(excelFileAddress)))
            {
                var sheet = package.Workbook.Worksheets.Add("Info");
                sheet.View.RightToLeft = true;

                var colIndex = 1;

                sheet.Cells[1, colIndex].Value = "#";
                sheet.Cells[2, colIndex++].Value = clientInDb.Id.ToString();


                sheet.Cells[1, colIndex].Value = "نام";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.Name;


                sheet.Cells[1, colIndex].Value = "نام خانوادگی";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.Family;


                sheet.Cells[1, colIndex].Value = "کد ملی";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.NationalCode;


                sheet.Cells[1, colIndex].Value = "نام پدر";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.FatherName;


                sheet.Cells[1, colIndex].Value = "نام مادر";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.MotherName;


                sheet.Cells[1, colIndex].Value = "تاریخ تولد";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.Birthdate == null
                    ? ""
                    : new PersianDateTime(clientInDb.Person.Birthdate).ToString(PersianDateFormatStr);


                sheet.Cells[1, colIndex].Value = "شماره شناسنامه";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.BirthCertificateNo;


                sheet.Cells[1, colIndex].Value = "شماره مسلسل شناسنامه";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.BirthCertificateMosalsal;


                sheet.Cells[1, colIndex].Value = "توضیحات شناسنامه";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.BirthCertificateDescription;


                sheet.Cells[1, colIndex].Value = "جنسیت";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.GenderType?.Name;


                sheet.Cells[1, colIndex].Value = "وضعیت تاهل";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.MarriageType?.Name;


                sheet.Cells[1, colIndex].Value = "تعداد فرزندان";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.NumberOfChildren;


                sheet.Cells[1, colIndex].Value = "شهر محل تولد";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.CityOfBirth?.Name;

                // سالم یا معلول
                sheet.Cells[1, colIndex].Value = "وضعیت جسمی";
                sheet.Cells[2, colIndex++].Value = clientInDb.Person.IsDisabled == null ? "" :
                    clientInDb.Person.IsDisabled == false ? "سالم" : "معلول";


                sheet.Cells[1, colIndex].Value = "نوع مددجو";
                sheet.Cells[2, colIndex++].Value = clientInDb.ClientType?.Name;


                sheet.Cells[1, colIndex].Value = "نوع پرونده (معاونت)";
                sheet.Cells[2, colIndex++].Value = clientInDb.AssistanceType?.Name;

                sheet.Cells[1, colIndex].Value = "درآمد ماهانه";
                sheet.Cells[2, colIndex++].Value = clientInDb.MonthlyIncome;


                sheet.Cells[1, colIndex].Value = "شماره پیمنت";
                sheet.Cells[2, colIndex++].Value = clientInDb.GlobalBehzistiUiCode;

                if (clientInDb.ClientForms.Any())
                {
                    foreach (var clientForm in clientInDb.ClientForms)
                    {
                        sheet.Cells[1, colIndex].Value = "نام طرح";
                        sheet.Cells[2, colIndex++].Value = clientForm.Form.Name;
                    }
                }
                else
                {
                    sheet.Cells[1, colIndex].Value = "نام طرح";
                    sheet.Cells[2, colIndex++].Value = "مددجو در هیچ طرحی ثبت نشده است";
                }

                // For merge cells in excel use this
                //sheet.Cells["A1:C1"].Merge = true;


                sheet.Cells.Style.Font.Name = "B Nazanin";

                sheet.Cells[1, 1, 1, colIndex].Style.Font.Bold = true;

                package.Save();

                // به خاطر اینکه خطا داد این رو نوشتم
                // ولی قاعدتا نباید مینوشتم
                package.Dispose();
            }

            var subFolderAddressDocument = Path.Combine(folderAddressNeedsToBeZipped, clientInDb.Person.NationalCode);
            if (!Directory.Exists(subFolderAddressDocument))
                Directory.CreateDirectory(subFolderAddressDocument);

            // همه مدارک مددجو از جمله شناسنامه و کارت ملی و ...
            // را به پوشه ای که قرار است زیپ شود منتقل می کند
            foreach (var clientDocument in clientInDb.ClientDocuments)
            {
                if (System.IO.File.Exists(clientDocument.DocURI))
                {
                    System.IO.File.Copy(clientDocument.DocURI, $"{subFolderAddressDocument}\\{Path.GetFileName(clientDocument.DocURI)}");
                }
            }

            // آدرس فایل فشرده خروجی در پوشه فایل های موقت اطلاعات مددجو می باشد
            var tempZipFileAddress = $"{tempFolderAddressOfThisUser}\\{clientInDb.Person.NationalCode}.zip";

            // اگر یک فایل با این نام وجود داشت ابتدا آن را حذف کن
            if (System.IO.File.Exists(tempZipFileAddress))
                System.IO.File.Delete(tempZipFileAddress);

            ZipFile.CreateFromDirectory(folderAddressNeedsToBeZipped, tempZipFileAddress, CompressionLevel.Optimal, false, Encoding.UTF8);

            // همه فایل های موقتی که ساخته شده را حذف کن
            // چون قبلا از همه آنها یک فایل زیپ تهیه شده است
            Directory.Delete(folderAddressNeedsToBeZipped, true);


            // برای اینکه بعدا فایل خود به خود از سرور حذف شود 
            // آن را به جدول عکس های موقت اضافه کردم
            // شبیه همان کاری که برای عکس های آپلودی انجام دادم
            // زمان انقضا هم روی 60 دقیقه تنظیم شده است
            var tempZipFile = new TemporaryImage
            {
                ClientId = clientInDb.Id,
                CreatedAt = DateTime.Now,
                ExpireAt = DateTime.Now.AddMinutes(60),
                MasterFileName = Path.GetFileName(tempZipFileAddress),
                TemporaryFileName = tempZipFileAddress,
            };


            _dbContext.TemporaryImages.Add(tempZipFile);
            _dbContext.SaveChanges();

            // این نوع محتوی برای فایل زیپ بود
            //"application/vnd.ms-excel"

            return File(tempZipFileAddress, "application/zip", Path.GetFileName(tempZipFileAddress));

        }

        // برای مشکل سیستم عامل هایی که زبان فارسی به عنوان پیش فرض آنها انتخاب شده است
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            const string culture = "en-US";
            var ci = CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        public ActionResult Index(MessageClientManageId? message, string searchWord = "")
        {
            //ViewBag.StatusMessage =
            //    message == MessageClientManageId.YouCannotAccessThisArea
            //        ? "شما مجوز دسترسی به این قسمت را ندارید"
            //        : message == MessageClientManageId.YouCannotEditClient
            //            ? "شما مجوز ویرایش اطلاعات مددجو را ندارید"
            //            : message == MessageClientManageId.DuplicateNationalCode
            //                ? "فردی با این کد ملی در سامانه ثبت شده است و امکان ثبت فرد جدید با این کد ملی وجود ندارد"
            //                : "";

            var status = new Status();
            switch (message)
            {
                case MessageClientManageId.YouCannotAccessThisArea:
                    status.StatusType = ModelEnums.StatusTypeE.YouCannotAccessThisArea;
                    break;
                case MessageClientManageId.YouCannotEditClient:
                    status.StatusType = ModelEnums.StatusTypeE.YouCannotEditClient;
                    break;
                case MessageClientManageId.DuplicateNationalCode:
                    status.StatusType = ModelEnums.StatusTypeE.DuplicateNationalCode;
                    break;
                case MessageClientManageId.RestrictionOnDownloadRequest:
                    status.StatusType = ModelEnums.StatusTypeE.RestrictionOnDownloadRequest;
                    break;
                default:
                    status = null;
                    break;
            }

            ViewBag.Status = status;
            ViewBag.SearchWord = searchWord;

            return View("ClientServerSideList");
        }

        [HttpPost]
        public ActionResult SearchClientSimple(string searchWord)
        {
            return RedirectToAction("Index", new { searchWord });
        }

        public ActionResult WaitingApplicantList(MessageClientManageId? message)
        {
            ViewBag.StatusMessage =
                message == MessageClientManageId.YouCannotAccessThisArea
                    ? "شما مجوز دسترسی به این قسمت را ندارید"
                    : message == MessageClientManageId.YouCannotEditClient
                        ? "شما مجوز ویرایش اطلاعات مددجو را ندارید"
                        : message == MessageClientManageId.DuplicateNationalCode
                            ? "فردی با این کد ملی در سامانه ثبت شده است و امکان ثبت فرد جدید با این کد ملی وجود ندارد"
                            : message == MessageClientManageId.ClientWaitingApplicantUpdateSuccessfully
                                ? "اطلاعات مددجو با موفقیت در پایگاه داده ذخیره گردید"
                                : "";

            return View("ClientWaitingApplicantServerSideList");
        }

        public ActionResult ExemptionList(MessageClientManageId? message)
        {
            ViewBag.StatusMessage =
                message == MessageClientManageId.YouCannotAccessThisArea
                    ? "شما مجوز دسترسی به این قسمت را ندارید"
                    : message == MessageClientManageId.YouCannotEditClient
                        ? "شما مجوز ویرایش اطلاعات مددجو را ندارید"
                        : message == MessageClientManageId.DuplicateNationalCode
                            ? "فردی با این کد ملی در سامانه ثبت شده است و امکان ثبت فرد جدید با این کد ملی وجود ندارد"
                            : message == MessageClientManageId.ClientWaitingApplicantUpdateSuccessfully
                                ? "اطلاعات مددجو با موفقیت در پایگاه داده ذخیره گردید"
                                : "";

            return View("ClientByExemptionServerSideList");
        }

        public ActionResult IncompleteRegisterList(MessageClientManageId? message)
        {
            ViewBag.StatusMessage =
                message == MessageClientManageId.YouCannotAccessThisArea
                    ? "شما مجوز دسترسی به این قسمت را ندارید"
                    : message == MessageClientManageId.YouCannotEditClient
                        ? "شما مجوز ویرایش اطلاعات فرد را ندارید"
                        : message == MessageClientManageId.DuplicateNationalCode
                            ? "فردی با این کد ملی در سامانه ثبت شده است و امکان ثبت فرد جدید با این کد ملی وجود ندارد"
                            : message == MessageClientManageId.DuplicateNationalCode
                                ? "فرد انتخاب شده با موفقیت از پایگاه داده حذف گردید."
                                : "";

            return View("IncompleteRegisterServerSideList");
        }

        public ActionResult PersonStoreAsFamilyRelationList(MessageClientManageId? message)
        {
            return View("PersonStoreAsFamilyRelationServerSideList");
        }

        public ActionResult DeletedClientList()
        {
            return View("DeletedClientServerSideList");
        }

        public ActionResult EditWaitingApplicant(int id)
        {
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
                .Include(w => w.ClientWaitingApplicantLogs)
                .SingleOrDefault(c => c.Id == id);
            if (waitingApplicantInDb == null) return HttpNotFound();

            //waitingApplicantInDb.Birthdate = new PersianDateTime(waitingApplicantInDb.Birthdate);
            var allCounty = _dbContext.Counties.Select(_mapper.Map<CountyDto>).ToList();

            var allRequests =
                _dbContext.ClientWaitingApplicantRequests
                    .Where(c => c.ClientWaitingApplicantId == waitingApplicantInDb.Id)
                    .Include(c => c.RequestType)
                    .ToList();


            var viewModel = new MadadjooRegisterViewModel
            {
                ClientWaitingApplicant = _mapper.Map<ClientWaitingApplicantDto>(waitingApplicantInDb),
                AllProvinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList(),
                MarriageTypes = _dbContext.MarriageTypes.ToList(),
                GenderTypes = _dbContext.GenderTypes.ToList(),
                AllCounties = allCounty,


                CountyOfBirthList = allCounty,
                DistrictOfBirthList = _dbContext.Districts
                    .Where(d => d.CountyId == waitingApplicantInDb.CityOfBirth.District.CountyId)
                    .AsEnumerable().Select(_mapper.Map<DistrictDto>).ToList(),
                CityOfBirthList = _dbContext.Cities
                    .Where(c => c.DistrictId == waitingApplicantInDb.CityOfBirth.DistrictId)
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
                DistrictOfBehzistiDocumentList = _dbContext.Districts
                    .Where(d => d.CountyId == waitingApplicantInDb.City.District.CountyId)
                    .AsEnumerable().Select(_mapper.Map<DistrictDto>),
                CityOfBehzistiDocumentList = _dbContext.Cities
                    .Where(c => c.DistrictId == waitingApplicantInDb.City.DistrictId)
                    .AsEnumerable().Select(_mapper.Map<CityDto>).ToList(),

                CountyOfBehzistiDocumentId = waitingApplicantInDb.City.District.CountyId,
                DistrictOfBehzistiDocumentId = waitingApplicantInDb.City.DistrictId,

                RequestTypes = _dbContext.RequestTypes.ToList(),

                RequestTypeBuildingId = allRequests.Single(r => r.RequestType.IsExemption == false).RequestTypeId,

                IsRequestWaterExemption =
                    allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionWater),
                IsRequestGasExemption = allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionGas),
                IsRequestElectricalExemption =
                    allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionElectrical),
                IsRequestProductionLicenseExemption = allRequests.Any(r =>
                    r.RequestType.Name == ClientRequestTypeStr.ExemptionProductionLicense),
                ClientWaitingApplicantLogs = waitingApplicantInDb.ClientWaitingApplicantLogs,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveWaitingApplicant(MadadjooRegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var allCounty = _dbContext.Counties.Select(_mapper.Map<CountyDto>).ToList();
                viewModel.AllProvinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList();
                viewModel.MarriageTypes = _dbContext.MarriageTypes.ToList();
                viewModel.GenderTypes = _dbContext.GenderTypes.ToList();
                viewModel.AllCounties = allCounty;


                viewModel.CountyOfBirthList = allCounty;
                viewModel.DistrictOfBirthList = _dbContext.Districts.Where(d =>
                        d.CountyId == viewModel.ClientWaitingApplicant.CityOfBirth.District.CountyId)
                    .AsEnumerable().Select(_mapper.Map<DistrictDto>).ToList();
                viewModel.CityOfBirthList = _dbContext.Cities
                    .Where(c => c.DistrictId == viewModel.ClientWaitingApplicant.CityOfBirth.DistrictId)
                    .AsEnumerable().Select(_mapper.Map<CityDto>).ToList();


                viewModel.RequestTypes = _dbContext.RequestTypes.ToList();

                viewModel.CountyOfBehzistiDocumentList = allCounty;
                viewModel.DistrictOfBehzistiDocumentList = _dbContext.Districts.Where(d =>
                        d.CountyId == viewModel.ClientWaitingApplicant.City.District.CountyId)
                    .AsEnumerable().Select(_mapper.Map<DistrictDto>);
                viewModel.CityOfBehzistiDocumentList = _dbContext.Cities.Where(c =>
                        c.DistrictId == viewModel.ClientWaitingApplicant.City.DistrictId)
                    .AsEnumerable().Select(_mapper.Map<CityDto>).ToList();

                viewModel.ClientTypes = _dbContext.ClientTypes.ToList();


                return View("EditWaitingApplicant", viewModel);
            }


            // fetch waiting applicant from db to set change tracker flag for update
            var waitingApplicantInDb =
                _dbContext.ClientWaitingApplicants.Single(w => w.Id == viewModel.ClientWaitingApplicant.Id);

            //map object -> update data
            _mapper.Map(viewModel.ClientWaitingApplicant, waitingApplicantInDb);

            //set update date and time
            waitingApplicantInDb.UpdatedDate = DateTime.Now;
            waitingApplicantInDb.Birthdate = Convert.ToDateTime(viewModel.ClientWaitingApplicant.Birthdate.ToString()).Date; 

            //save changes to db
            _dbContext.SaveChanges();

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos.Single(u => u.Id == userId);


            var applicantLog = new ClientWaitingApplicantLog
            {
                ClientWaitingApplicantId = waitingApplicantInDb.Id,
                CreatedAt = DateTime.Now,
                Description =
                    $"اطلاعات متقاضی توسط کارشناس با مشخصات زیر ویرایش گردید: کد کاربر {userInfoInDb.Id} و نام و نام خانوادگی کاربر {userInfoInDb.Name} {userInfoInDb.Family}",
            };
            _dbContext.ClientWaitingApplicantLogs.Add(applicantLog);
            _dbContext.SaveChanges();

            var buildingRequest = _dbContext.ClientWaitingApplicantRequests.Single(r =>
                r.ClientWaitingApplicantId == waitingApplicantInDb.Id && r.RequestType.IsExemption == false);
            if (buildingRequest.RequestTypeId != viewModel.RequestTypeBuildingId)
            {
                buildingRequest.RequestTypeId = viewModel.RequestTypeBuildingId;
                _dbContext.SaveChanges();
            }


            // همه تقاضاهای مربوط به معافیت را پاک کن
            var allExemptionRequestInDb =
                _dbContext.ClientWaitingApplicantRequests.Where(r =>
                    r.RequestType.IsExemption && r.ClientWaitingApplicantId == waitingApplicantInDb.Id);
            _dbContext.ClientWaitingApplicantRequests.RemoveRange(allExemptionRequestInDb);
            _dbContext.SaveChanges();


            var allExemptionRequestType = _dbContext.RequestTypes.Where(r => r.IsExemption).ToList();

            if (viewModel.IsRequestWaterExemption)
            {
                var requestId = allExemptionRequestType.Single(r => r.Name == ClientRequestTypeStr.ExemptionWater).Id;
                var newRequest = new ClientWaitingApplicantRequest
                { ClientWaitingApplicantId = waitingApplicantInDb.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(newRequest);
                _dbContext.SaveChanges();
            }


            if (viewModel.IsRequestGasExemption)
            {
                var requestId = allExemptionRequestType.Single(r => r.Name == ClientRequestTypeStr.ExemptionGas).Id;
                var newRequest = new ClientWaitingApplicantRequest
                { ClientWaitingApplicantId = waitingApplicantInDb.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(newRequest);
                _dbContext.SaveChanges();
            }


            if (viewModel.IsRequestElectricalExemption)
            {
                var requestId = allExemptionRequestType.Single(r => r.Name == ClientRequestTypeStr.ExemptionElectrical)
                    .Id;
                var newRequest = new ClientWaitingApplicantRequest
                { ClientWaitingApplicantId = waitingApplicantInDb.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(newRequest);
                _dbContext.SaveChanges();
            }


            if (viewModel.IsRequestProductionLicenseExemption)
            {
                var requestId = allExemptionRequestType
                    .Single(r => r.Name == ClientRequestTypeStr.ExemptionProductionLicense).Id;
                var newRequest = new ClientWaitingApplicantRequest
                { ClientWaitingApplicantId = waitingApplicantInDb.Id, RequestTypeId = requestId };
                _dbContext.ClientWaitingApplicantRequests.Add(newRequest);
                _dbContext.SaveChanges();
            }


            return RedirectToAction("WaitingApplicantList",
                new { Message = MessageClientManageId.ClientWaitingApplicantUpdateSuccessfully });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptOrRejectApplicant(MadadjooRegisterViewModel viewModel)
        {
            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos
                .Include(u => u.User)
                .Single(u => u.Id == userId);

            if (viewModel.ClientWaitingApplicant.Id == 0)
                return HttpNotFound();
            var applicantInDb =
                _dbContext.ClientWaitingApplicants
                    .Include(a => a.Requests)
                    .Single(a => a.Id == viewModel.ClientWaitingApplicant.Id);
            if (applicantInDb.NationalCode != viewModel.ClientWaitingApplicant.NationalCode)
                return HttpNotFound();


            if (viewModel.IsApproved != null && viewModel.IsApproved == true)
            {
                // approved by karshenas


                // اضافه کردن فرد به لیست افراد
                var person = new Person
                {
                    Name = applicantInDb.Name,
                    Family = applicantInDb.Family,
                    FatherName = applicantInDb.FatherName,
                    MotherName = applicantInDb.MotherName,
                    NationalCode = applicantInDb.NationalCode,
                    BirthCertificateNo = applicantInDb.BirthCertificateNo,
                    BirthCertificateMosalsal = applicantInDb.BirthCertificateMosalsal,
                    BirthCertificateDescription = applicantInDb.BirthCertificateDescription,
                    Birthdate = applicantInDb.Birthdate,
                    CityOfBirthId = applicantInDb.CityOfBirthId,
                    CreatedDate = DateTime.Now,
                    GenderTypeId = applicantInDb.GenderTypeId,
                    IsDeleted = false,
                    MarriageTypeId = applicantInDb.MarriageTypeId,
                    NumberOfChildren = applicantInDb.NumberOfChildren,
                    IsDisabled = applicantInDb.IsDisabled,
                    UpdatedDate = DateTime.Now
                };

                _dbContext.Persons.Add(person);

                _dbContext.SaveChanges();


                // ثبت در جدول مددجویان
                var client = new Client
                {
                    Person = person,
                    UpdatedDate = DateTime.Now,
                    CityId = applicantInDb.CityId,
                    ClientTypeId = applicantInDb.ClientTypeId,
                    NumberOfDisabledInFamily = applicantInDb.NumberOfDisabledInFamily,
                    IsHouseHold = applicantInDb.IsHouseHold,
                    ContactInfo = new ContactInfo
                    {
                        Mobile = applicantInDb.Mobile,
                        HomeTel = applicantInDb.HomeTel,
                        EmergencyTel = applicantInDb.EmergencyTel,
                        WorkTel = applicantInDb.WorkTel,
                        WorkAddress = applicantInDb.WorkAddress,
                    },
                    CurrentHousing = new CurrentHousing
                    {
                        CurrentHouseTypeId = applicantInDb.CurrentHouseTypeId,
                        Address = applicantInDb.Address,
                        AddressCurrentHouse = applicantInDb.Address,
                        CityId = applicantInDb.HouseCityId,
                        DepositAmount = applicantInDb.DepositAmount,
                        Latitude = applicantInDb.Latitude,
                        Longitude = applicantInDb.Longitude,
                        MonthlyRentalRate = applicantInDb.MonthlyRentalRate,
                        OtherDescription = applicantInDb.OtherDescription,
                        PostalCode = applicantInDb.PostalCode,
                        RelativeFamilyNameWhoClientLiveInHerHouse =
                            applicantInDb.RelativeFamilyNameWhoClientLiveInHerHouse,
                    },
                    ClientState = new ClientState
                    {
                        ClientStateTypeE = ModelEnums.ClientStateTypeE.InitialClientRegister,
                        CurrentStateDate = DateTime.Now,
                        PrevStateDate = DateTime.Now,
                    },
                    CreatedDate = DateTime.Now,
                    MonthlyIncome = applicantInDb.MonthlyIncome,
                    IsDeleted = false,
                    IsIncludedInComprehensiveReport = false
                };

                _dbContext.Clients.Add(client);
                _dbContext.SaveChanges();


                // ثبت همین تقاضا ها برای مددجو
                var requestsForThisApplicant =
                    _dbContext.ClientWaitingApplicantRequests.Where(r =>
                        r.ClientWaitingApplicantId == applicantInDb.Id);

                foreach (var request in requestsForThisApplicant)
                {
                    var newRequest = new ClientRequest
                    {
                        ClientId = client.Id,
                        RequestTypeId = request.RequestTypeId,
                    };
                    _dbContext.ClientRequests.Add(newRequest);
                }

                _dbContext.SaveChanges();


                // ثبت اولین گزارش برای این مددجو - در تاریخچه وضعیت ها
                var newLog = new ClientStateLog
                {
                    ClientId = client.Id,
                    ClientStateTypeE = ModelEnums.ClientStateTypeE.InitialClientRegister,
                    LogDate = DateTime.Now,
                    Description =
                        $"اطلاعات توسط کاربر با کد شناسایی{userInfoInDb.Id} به نام {userInfoInDb.Name} و نام خانوادگی {userInfoInDb.Family} و نام کاربری {userInfoInDb.User.UserName} تایید گردید."
                };
                _dbContext.ClientStateLogs.Add(newLog);
                _dbContext.SaveChanges();


                // پاک کردن اطلاعات متقاضی

                // چون قبلا به عنوان مددجو او را ثبت کردیم دیگر نیازی به اطلاعات موجود در جدول متقاضیان وجود ندارد
                var logsForThisApplicant =
                    _dbContext.ClientWaitingApplicantLogs.Where(l => l.ClientWaitingApplicantId == applicantInDb.Id);
                _dbContext.ClientWaitingApplicantLogs.RemoveRange(logsForThisApplicant);
                _dbContext.SaveChanges();

                // پاک کردن اطلاعات تقاضا ها
                _dbContext.ClientWaitingApplicantRequests.RemoveRange(requestsForThisApplicant);
                _dbContext.SaveChanges();

                // پاک کردن خود متقاضی
                _dbContext.ClientWaitingApplicants.Remove(applicantInDb);
                _dbContext.SaveChanges();
            }
            else if (viewModel.IsRejected != null && viewModel.IsRejected == true)
            {
                //rejected by karshenas

                var log = new ClientWaitingApplicantLog
                {
                    ClientWaitingApplicantId = applicantInDb.Id,
                    CreatedAt = DateTime.Now,
                    Description = "اطلاعات متقاضی توسط کارشناس رد شد و اطلاعات مددجو به مددجویان حذف شده منتقل گردید",
                };

                _dbContext.ClientWaitingApplicantLogs.Add(log);
                _dbContext.SaveChanges();

                applicantInDb.IsDeleted = true;

                _dbContext.SaveChanges();
            }

            return RedirectToAction("WaitingApplicantList");
        }

        public ActionResult NewPerson()
        {
            var viewModel = new PersonFormViewModel
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                ProvinceOfBirth = _dbContext.Provinces.AsEnumerable().Select(_mapper.Map<ProvinceDto>).ToList(),
                CountyOfBirth = new List<CountyDto>(),
                DistrictOfBirth = new List<DistrictDto>(),
                CityOfBirth = new List<CityDto>(),
                GenderTypes = _dbContext.GenderTypes.ToList(),
                MarriageTypes = _dbContext.MarriageTypes.ToList(),
                Person = new PersonDto()
            };
            return View("PersonForm", viewModel);
        }

        public ActionResult EditPerson(int id)
        {
            var personInDb = _dbContext.Persons
                .Include(p => p.CityOfBirth)
                .Include(p => p.CityOfBirth.District)
                .Include(p => p.CityOfBirth.District.County)
                .SingleOrDefault(p => p.Id == id);
            if (personInDb == null)
                return HttpNotFound();

            if (!User.IsInRole(RoleName.KarshenasOstan) && !User.IsInRole(RoleName.KarshenasShahrestan))
                return RedirectToAction("IncompleteRegisterList",
                    new { Message = MessageClientManageId.YouCannotEditClient });


            var viewModel = new PersonFormViewModel
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                ProvinceOfBirth = _dbContext.Provinces.AsEnumerable().Select(_mapper.Map<ProvinceDto>).ToList(),
                CountyOfBirth = personInDb.CityOfBirth != null
                    ? _dbContext.Counties
                        .Where(c => c.ProvinceId == personInDb.CityOfBirth.District.County.ProvinceId)
                        .AsEnumerable().Select(_mapper.Map<CountyDto>)
                    : new List<CountyDto>(),
                DistrictOfBirth = personInDb.CityOfBirth != null
                    ? _dbContext.Districts
                        .Where(d => d.CountyId == personInDb.CityOfBirth.District.CountyId)
                        .AsEnumerable().Select(_mapper.Map<DistrictDto>)
                    : new List<DistrictDto>(),
                CityOfBirth = personInDb.CityOfBirth != null
                    ? _dbContext.Cities
                        .Where(d => d.DistrictId == personInDb.CityOfBirth.DistrictId)
                        .AsEnumerable().Select(_mapper.Map<CityDto>)
                    : new List<CityDto>(),
                GenderTypes = _dbContext.GenderTypes.ToList(),
                MarriageTypes = _dbContext.MarriageTypes.ToList(),
                Person = _mapper.Map<PersonDto>(personInDb)
            };

            return View("PersonForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePerson(PersonDto person)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new PersonFormViewModel
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ProvinceOfBirth = _dbContext.Provinces.AsEnumerable().Select(_mapper.Map<ProvinceDto>).ToList(),
                    GenderTypes = _dbContext.GenderTypes.ToList(),
                    MarriageTypes = _dbContext.MarriageTypes.ToList(),
                    Person = person
                };
                return View("PersonForm", viewModel);
            }

            // در حالت افزودن فرد جدید 
            if (person.Id == 0)
            {
                // اگر کد ملی تکراری بود نباید فرد در سیستم اضافه شود
                var personInDb = _dbContext.Persons.SingleOrDefault(p => p.NationalCode == person.NationalCode);
                if (personInDb != null)
                    // اگر کد ملی تکراری بود کاربر را به صفحه اصلی منتقل نماید
                    return RedirectToAction("IncompleteRegisterList",
                        new { Message = MessageClientManageId.DuplicateNationalCode });

                // اضافه کردن فرد جدید به پایگاه داده
                var newPerson = _mapper.Map<Person>(person);
                _dbContext.Persons.Add(newPerson);
            }
            // در حالت ویرایش اطلاعات فرد
            else
            {
                // ثبت اطلاعات ویرایش شده فرد در پایگاه داده
                var personInDb = _dbContext.Persons.Single(p => p.NationalCode == person.NationalCode);
                if (personInDb.Id != person.Id)
                    // اگر در حال ثبت کد ملی تکراری در سامانه بودیم کاربر را به صفحه اول منتقل کند
                    return RedirectToAction("IncompleteRegisterList",
                        new { Message = MessageClientManageId.DuplicateNationalCode });
                personInDb = _dbContext.Persons.Single(p => p.Id == person.Id);

                _mapper.Map(person, personInDb);
                personInDb.UpdatedDate = DateTime.Now;
                if (personInDb.CreatedDate == null) personInDb.CreatedDate = DateTime.Now;
            }

            _dbContext.SaveChanges();

            return RedirectToAction("IncompleteRegisterList");
        }


        private ClientFormViewModel GenerateViewModelForNewClient(bool editMode = false, ClientDto client = null)
        {
            var currentUserId = User.Identity.GetUserId();
            var viewModel = new ClientFormViewModel
            {
                Client = client ?? new ClientDto(),

                GenderTypes = _dbContext.GenderTypes.AsEnumerable(),
                MarriageTypes = _dbContext.MarriageTypes.AsEnumerable(),
                ClientTypes = _dbContext.ClientTypes.AsEnumerable(),
                AssistanceTypes = _dbContext.AssistanceTypes.AsEnumerable(),

                ProvinceOfBirth = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).AsEnumerable(),
                CountyOfBirth = new List<CountyDto>().AsEnumerable(),
                DistrictOfBirth = new List<DistrictDto>().AsEnumerable(),
                CityOfBirth = new List<CityDto>().AsEnumerable()
            };
            if (viewModel.Client.Person != null)
                viewModel.Client.Person.Id = viewModel.Client.Id;
            else
                viewModel.Client.Person = new PersonDto();

            var userInfoInDb = _dbContext.UserInfos
                .Include(ui => ui.AssistanceType)
                .Include(ui => ui.CoOrganizationType)
                .Single(ui => ui.Id == currentUserId);

            var counties = new List<CountyDto>().AsEnumerable();
            var districts = new List<DistrictDto>().AsEnumerable();
            var cities = new List<CityDto>().AsEnumerable();

            // همانند بالا اگر در حالت ویرایش بودیم
            // باید لیست شهرستان ها، بخش ها و شهرها و روستاهای هم رده با محل تولد مددجو برای کاربر ارسال شود
            if (client != null && editMode && client.Person.ProvinceOfBirthId != 0)
            {
                viewModel.CountyOfBirth = _dbContext.Counties
                    .Where(c => c.ProvinceId == client.Person.ProvinceOfBirthId)
                    .AsEnumerable()
                    .Select(_mapper.Map<CountyDto>);

                var countyOfBirthId = client.Person.CountyOfBirthId;
                viewModel.DistrictOfBirth = _dbContext.Districts.Where(d => d.CountyId == countyOfBirthId)
                    .AsEnumerable()
                    .Select(_mapper.Map<DistrictDto>);

                var districtOfBirthId = client.Person.DistrictOfBirthId;
                viewModel.CityOfBirth = _dbContext.Cities.Where(c => c.DistrictId == districtOfBirthId)
                    .AsEnumerable()
                    .Select(_mapper.Map<CityDto>);
            }

            if (User.IsInRole(RoleName.KarshenasOstan) || User.IsInRole(RoleName.ModirKolOstan) ||
                User.IsInRole(RoleName.MoavenMosharekat) || User.IsInRole(RoleName.MoavenOstan))
            {
                // کاربر کارشناس استان می تواند در همه شهرستان ها مددجو اضافه کند
                counties = _dbContext.Counties.Where(c => c.ProvinceId == userInfoInDb.ProvinceId).AsEnumerable()
                    .Select(_mapper.Map<County, CountyDto>);
                if (editMode)
                {
                    // اگر در حالت ویرایش بودیم
                    // باید لیست تمام بخش های زیر مجموعه شهرستان انتخاب شده را برای کاربر ارسال کنیم
                    // و بعد از آن لیست تمام شهرهای موجود در بخش انتخاب شده مربوط به مددجو نیز باید ارسال شود
                    var countyId = client == null ? 0 : client.CountyId;
                    if (countyId != 0)
                    {
                        var districtsInDb = _dbContext.Districts.Where(d => d.CountyId == countyId);
                        var districtIds = districtsInDb.Select(d => d.Id).ToList();
                        districts = districtsInDb.AsEnumerable().Select(_mapper.Map<DistrictDto>);

                        cities = _dbContext.Cities.Where(c => districtIds.Contains(c.DistrictId)).AsEnumerable()
                            .Select(_mapper.Map<CityDto>);
                    }
                }
            }
            else if (User.IsInRole(RoleName.KarshenasShahrestan))
            {
                // در صورتی که کاربر کارشناس شهرستان بود
                // تنها لیست بخش ها و شهرهای زیر مجموعه شهرستان
                // خودش را نمایش می دهد
                var userInfo = _dbContext.UserInfos.Include(u => u.County).Single(u => u.Id == currentUserId);
                if (userInfo.County == null)
                    return null;

                counties = new List<CountyDto>
                {
                    _mapper.Map<CountyDto>(_dbContext.Counties.Single(c => c.Id == userInfo.County.Id))
                }.AsEnumerable();

                var districtsInDb = _dbContext.Districts.Where(d => d.CountyId == userInfo.County.Id);
                var districtIds = districtsInDb.Select(d => d.Id).ToList();
                districts = districtsInDb.AsEnumerable().Select(_mapper.Map<DistrictDto>);
                if (districtsInDb.Any())
                    cities = _dbContext.Cities.Where(c => districtIds.Contains(c.DistrictId)).AsEnumerable()
                        .Select(_mapper.Map<CityDto>);
            }

            viewModel.Counties = counties;
            viewModel.Districts = districts;
            viewModel.Cities = cities;
            return viewModel;
        }

        private bool CheckPrivilegeForEditClient(int id, out Client client, out ActionResult result)
        {
            client = null;
            result = null;


            // قرار بر این شد که بتوانند همه اطلاعات رو ببینند اما مجوز ویرایش نداشته باشند
            // پس کد این قسمت به بخش ذخیره منتقل شد
            //
            ////در صورتی که کاربر اجازه ویرایش اطلاعات را نداشته باشد
            ////ممکن است کاربر (حتی در نقش کاربر استان) هم باشد ولی اجازه ویرایش نداشته باشد
            //if (!User.IsInRole(RoleName.CanManageClient))
            //{
            //    result = RedirectToAction("Index", new
            //    {
            //        Message = MessageClientManageId.YouCannotAccessThisArea
            //    });
            //    return false;
            //}
            //


            //جستجوی مددجو در پایگاه داده
            var clientInDb = _dbContext.Clients.Include(c => c.Person)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.Person.CityOfBirth)
                .Include(c => c.Person.CityOfBirth.District)
                .Include(c => c.Person.CityOfBirth.District.County)
                .Include(c => c.ClientState)
                .Include(c => c.CurrentHousing)
                .Include(c => c.ContactInfo)
                .Include(c => c.BankInfo)
                .Include(c => c.FormEmtiazBandi)
                .Include(c => c.ClientForms)
                .Include(c => c.ClientRequests)
                .Include(c => c.ClientPhysicalProgresses)
                .Include(c => c.ClientRequests.Select(cr => cr.RequestType))
                .Include(c => c.ClientForms.Select(f => f.ClientFormFields))
                .Include(c => c.ClientDocuments)
                .Include(c => c.ClientDocuments.Select(cd => cd.DocumentType))
                .Include(c => c.ClientRequiredMaterials)
                .Include(c => c.ClientRequiredMaterials.Select(rm => rm.MaterialType))
                .SingleOrDefault(c => c.Id == id);
            //اگر مددجو در پایگاه داده یافت نشد
            if (clientInDb == null)
            {
                result = HttpNotFound();
                return false;
            }

            //بررسی شود که آیا کاربر اجازه دسترسی به این مددجو را دارد یا خیر

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos
                .Include(u => u.Province)
                .Include(u => u.County)
                .Include(u => u.CoOrganizationType)
                .Include(u => u.AssistanceType)
                .Single(u => u.Id == userId);

            // برای جلوگیری از نمایش خطا
            // مددجویانی که وضعیت مددجو برای آنها ثبت نشده است
            // به صورت پیش فرض وضعیت آنها بر روی ثبت نام اولیه قرار داده می شود
            if (clientInDb.ClientState == null)
            {
                clientInDb.ClientState = new ClientState
                {
                    ClientStateTypeE = ModelEnums.ClientStateTypeE.InitialClientRegister,
                    CurrentStateDate = DateTime.Now,
                    PrevStateDate = DateTime.Now
                };
                _dbContext.ClientStates.Add(clientInDb.ClientState);
                _dbContext.SaveChanges();
            }


            // کارشناس استان فقط به مددجویان استان خودش
            if (User.IsInRole(RoleName.KarshenasOstan) || User.IsInRole(RoleName.KarshenasMasoolOstan))
            {
                if (clientInDb.City.District.County.ProvinceId != userInfoInDb.ProvinceId)
                {
                    result = RedirectToAction("Index", new
                    {
                        Message = MessageClientManageId.YouCannotAccessThisArea
                    });
                    return false;
                }
            }
            else if (User.IsInRole(RoleName.KarshenasShahrestan))
            {
                // کارشناس شهرستان فقط به مددجویان شهرستان خودش دسترسی داشته باشد
                if (clientInDb.City.District.CountyId != userInfoInDb.CountyId)
                {
                    result = RedirectToAction("Index", new
                    {
                        Message = MessageClientManageId.YouCannotAccessThisArea
                    });
                    return false;
                }
            }
            // مدیر شهرستان اجازه ویرایش اطلاعات مددجو را ندارد
            // و باید به صفحه نمایش اطلاعات مددجو منتقل شود
            else if (User.IsInRole(RoleName.ModirShahrestan))
            {
                // باید چک شود که آیا مددجو در شهرستان وی می باشد یا خیر
                if (clientInDb.City.District.CountyId != userInfoInDb.CountyId)
                {
                    result = RedirectToAction("Index", new
                    {
                        Message = MessageClientManageId.YouCannotAccessThisArea
                    });
                    return false;
                }
            }
            else if (User.IsInRole(RoleName.ModirKolOstan))
            {
                // باید جک شود که آیا مددجو در استان وی می باشد یا خیر
                if (clientInDb.City.District.County.ProvinceId != userInfoInDb.ProvinceId)
                {
                    result = RedirectToAction("Index", new
                    {
                        Message = MessageClientManageId.YouCannotAccessThisArea
                    });
                    return false;
                }
            }
            else if (User.IsInRole(RoleName.MoavenMosharekat))
            {
                // باید جک شود که آیا مددجو در استان وی می باشد یا خیر
                if (clientInDb.City.District.County.ProvinceId != userInfoInDb.ProvinceId)
                {
                    result = RedirectToAction("Index", new
                    {
                        Message = MessageClientManageId.YouCannotAccessThisArea
                    });
                    return false;
                }
            }
            else if (User.IsInRole(RoleName.MoavenOstan))
            {
                // باید جک شود که آیا مددجو در استان وی می باشد یا خیر
                if (clientInDb.City.District.County.ProvinceId != userInfoInDb.ProvinceId ||
                    clientInDb.AssistanceTypeId != userInfoInDb.AssistanceTypeId)
                {
                    result = RedirectToAction("Index", new
                    {
                        Message = MessageClientManageId.YouCannotAccessThisArea
                    });
                    return false;
                }
            }
            else
            {
                if (userInfoInDb == null || userInfoInDb.CountyId != clientInDb.City.District.CountyId)
                {
                    result = RedirectToAction("Index", new
                    {
                        Message = MessageClientManageId.YouCannotAccessThisArea
                    });
                    return false;
                }
            }

            client = clientInDb;
            return true;
        }


        private bool CheckPrivilegeForSaveClient(int id, out Client client, out ActionResult result,
            ClientDto clientDto)
        {
            client = null;
            result = null;

            var isEditMode = id != 0;
            if (!User.IsInRole(RoleName.CanManageClient) ||
                !User.IsInRole(RoleName.KarshenasShahrestan) && !User.IsInRole(RoleName.KarshenasOstan))
            {
                result = RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotEditClient });
                return false;
            }

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos
                .Include(u => u.Province)
                .Include(u => u.County)
                .Include(u => u.AssistanceType)
                .Include(u => u.CoOrganizationType)
                .Single(ui => ui.Id == userId);


            if (isEditMode)
            {
                // مددجو را پیدا کن
                client = _dbContext.Clients
                    .Include(c => c.Person)
                    .Include(c => c.City)
                    .Include(c => c.City.District)
                    .Include(c => c.City.District.County)
                    .Include(c => c.City.District.County.Province)
                    .Include(c => c.ClientState)
                    .SingleOrDefault(c => c.Id == id);

                // اگر در حالت ویرایش بودیم و مددجو را پیدا نکردیم
                if (client == null)
                {
                    result = HttpNotFound();
                    return false;
                }
            }


            if (User.IsInRole(RoleName.KarshenasOstan))
            {
                if (isEditMode)
                {
                    // فقط اطلاعات مددجو در استان خودش را می تواند ویرایش کند
                    if (client.City.District.County.ProvinceId != userInfoInDb.ProvinceId)
                    {
                        result = RedirectToAction("Index", "Client",
                            new { Message = MessageClientManageId.YouCannotAccessThisArea });
                        return false;
                    }
                }
                else
                {
                    // فقط اطلاعات مددجو در استان خودش را می تواند ثبت کند
                    if (clientDto.ProvinceId != userInfoInDb.ProvinceId)
                    {
                        result = RedirectToAction("Index", "Client",
                            new { Message = MessageClientManageId.YouCannotAccessThisArea });
                        return false;
                    }
                }
            }

            // اگر کارشناس شهرستان بود
            else if (User.IsInRole(RoleName.KarshenasShahrestan))
            {
                if (isEditMode)
                {
                    // فقط شهرستان خودش را می تواند ویرایش کند
                    if (client.City.District.CountyId != userInfoInDb.CountyId)
                    {
                        result = RedirectToAction("Index", "Client",
                            new { Message = MessageClientManageId.YouCannotAccessThisArea });
                        return false;
                    }

                    // در حالت های زیر نمی تواند ویرایش انجام دهد
                    if (client.ClientState.ClientStateTypeE ==
                        ModelEnums.ClientStateTypeE.InWaitingListSazmanHamkarOstan ||
                        client.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.DenyBySazmanHamkarOstan ||
                        client.ClientState.ClientStateTypeE ==
                        ModelEnums.ClientStateTypeE.ApproveByAllOfSazmanHamkarOstan ||
                        client.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.ApproveAllFormEmtiazBandi ||
                        client.ClientState.ClientStateTypeE ==
                        ModelEnums.ClientStateTypeE.InWaitingListKarshenasKeshvar ||
                        client.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.DenyByKarshenasKeshvar)
                    {
                        result = RedirectToAction("Index", "Client",
                            new { Message = MessageClientManageId.YouCannotEditClientInThisStep });
                        return false;
                    }
                }
                else
                {
                    // فقط شهرستان خودش را می تواند ویرایش کند
                    if (clientDto.CountyId != userInfoInDb.CountyId)
                    {
                        result = RedirectToAction("Index", "Client",
                            new { Message = MessageClientManageId.YouCannotAccessThisArea });
                        return false;
                    }
                }
            }

            // معاون مسکن استان
            else if (User.IsInRole(RoleName.MoavenMosharekat))
            {
                if (isEditMode)
                {
                    if (client.City.District.County.ProvinceId != userInfoInDb.ProvinceId)
                    {
                        result = RedirectToAction("Index", "Client",
                            new { Message = MessageClientManageId.YouCannotAccessThisArea });
                        return false;
                    }
                }
                else
                {
                    // معاون مسکن استان نمی تواند مددجوی جدید اضافه کند
                    result = RedirectToAction("Index", "Client",
                        new { Message = MessageClientManageId.YouCannotAccessThisArea });
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public ActionResult ReadOnlyForm(int id)
        {
            return View();
        }


        // مددجوی جدید -- که صفحه اطلاعات اولیه را به او نمایش می دهد
        public ActionResult New(int id = 0)
        {
            //فقط کارشناس استان و کارشناس شهرستان می توانند
            //مددجوی جدید را به سامانه اضافه کنند
            // نکته: برای افزودن مددجو نیازی به
            //مجوز ویرایش مددجویان نیست و فقط به نقش کاربر مربوط می شود
            if (!User.IsInRole(RoleName.KarshenasOstan) &&
                !User.IsInRole(RoleName.KarshenasShahrestan))
                return RedirectToAction("Index", new
                {
                    Message = MessageClientManageId.YouCannotAccessThisArea
                });


            var viewModel = GenerateViewModelForNewClient();
            //اگر مقدار برگشتی نال باشد یعنی این کاربر اجازه ندارد از این استفاده کند
            if (viewModel == null)
                return HttpNotFound();

            // یعنی صفحه افزودن مددجو بر اساس اطلاعات فرد ثبت شده در سیستم
            if (id != 0)
            {
                var clientInDb = _dbContext.Clients.SingleOrDefault(c => c.Id == id);
                if (clientInDb != null)
                    return RedirectToAction("EditBasic", new { Id = id });
                var personInDb = _dbContext.Persons
                    .Include(p => p.CityOfBirth)
                    .Include(p => p.CityOfBirth.District)
                    .Include(p => p.CityOfBirth.District.County)
                    .Single(p => p.Id == id);
                viewModel.Client.Person = _mapper.Map<PersonDto>(personInDb);

                if (personInDb.CityOfBirth != null)
                {
                    viewModel.CountyOfBirth = _dbContext.Counties
                        .Where(c => c.ProvinceId == personInDb.CityOfBirth.District.County.ProvinceId).AsEnumerable()
                        .Select(_mapper.Map<CountyDto>);
                    viewModel.DistrictOfBirth = _dbContext.Districts
                        .Where(c => c.CountyId == personInDb.CityOfBirth.District.CountyId).AsEnumerable()
                        .Select(_mapper.Map<DistrictDto>);
                    viewModel.CityOfBirth = _dbContext.Cities
                        .Where(c => c.DistrictId == personInDb.CityOfBirth.DistrictId).AsEnumerable()
                        .Select(_mapper.Map<CityDto>);
                }
            }

            return View("ClientForm", viewModel);
        }

        private void PopulateMenuTabItemOfRegisteredClientForm(int clientId)
        {
            // for populate menu item of registered client form
            //-----------------------------------------------------------
            var registeredForms = _dbContext.ClientForms
                .Include(cf => cf.Form)
                .Where(cf => cf.ClientId == clientId);

            var selectedFormMenu = new Dictionary<int, string>();
            foreach (var clientForm in registeredForms) selectedFormMenu.Add(clientForm.Id, clientForm.Form.Name);

            ViewBag.ClientSelectedFormTabItems = selectedFormMenu;
            //-----------------------------------------------------------
        }

        // ویرایش اطلاعات اولیه و اصلی مددجو
        public ActionResult EditBasic(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult res;

            if (!CheckPrivilegeForEditClient(id, out client, out res)) return res;
            var clientDto = _mapper.Map<ClientDto>(client);

            var viewModel = GenerateViewModelForNewClient(true, clientDto);
            viewModel.Client = clientDto;

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);


            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully
                        ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error
                            ? ModelEnums.StatusTypeE.Error
                            : ModelEnums.StatusTypeE.Warning
                };
            return View("ClientForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBasic(ClientDto client)
        {
            Client clientOut;
            ActionResult result;

            if (!CheckPrivilegeForSaveClient(client.Id, out clientOut, out result, client)) return result;

            if (!ModelState.IsValid)
            {
                //var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                // return user to client edit view
                var viewModel = GenerateViewModelForNewClient();

                // for Edit Client
                if (client.Id != 0) viewModel.Client = client;

                viewModel.Status = new Status { StatusType = ModelEnums.StatusTypeE.ValidationError };
                return View("ClientForm", viewModel);
            }

            // در صورتی که در حال ثبت مددجو از روی افراد ثبت شده در سامانه بودیم این پرچم فعال می شود
            var isInStateSavePersonAsClient = client.Id == 0 && client.Person.Id != 0;

            // اگر در حال ثبت مددجوی جدید بودیم کد صفر می باشد
            // For New Client
            if (client.Id == 0)
            {
                var clientNew = _mapper.Map<Client>(client);

                // چک شود که اگر کد ملی تکراری بود به صفحه بعد نرود و اطلاعات ذخیره نشود
                // با این جستجو اگر فردی به عنوان مددجو انتخاب نشده بود در نتایج جستجو لحاظ نشده 
                // و می توان او را به عنوان مددجو در سامانه ثبت کرد - با زدن دکمه ثبت به عنوان مددجو در لیست افراد
                var isDuplicateClient =
                    _dbContext.Clients.Any(c => c.Person.NationalCode == client.Person.NationalCode);
                if (isDuplicateClient)
                    return RedirectToAction("Index", new { Message = MessageClientManageId.DuplicateNationalCode });

                if (isInStateSavePersonAsClient)
                {
                    // ذخیره کردن اطلاعات فردی مددجو در حالت ثبت فرد به عنوان مددجو
                    var personInDb = _dbContext.Persons.Single(p => p.Id == client.Person.Id);
                    _mapper.Map(client.Person, personInDb);
                    personInDb.UpdatedDate = DateTime.Now;
                    if (personInDb.CreatedDate == null) personInDb.CreatedDate = DateTime.Now;
                    _dbContext.SaveChanges();

                    clientNew.Id = personInDb.Id;
                }

                // تنظیم زمان ساخت و آخرین ویرایش مددجو
                clientNew.CreatedDate = clientNew.UpdatedDate = DateTime.Now;

                // اگر در حالت افزودن مددجوی جدید از صفحه خالی بودیم
                if (!isInStateSavePersonAsClient)
                {
                    var personNew = _mapper.Map<Person>(client.Person);
                    personNew.CreatedDate = personNew.UpdatedDate = DateTime.Now;
                    _dbContext.Persons.Add(personNew);
                }

                _dbContext.Clients.Add(clientNew);

                _dbContext.SaveChanges();

                client.Id = clientNew.Id;
            }
            else
            // ویرایش مددجوی قبلی
            {
                var isDuplicateNationalCode = _dbContext.Clients.Include(c => c.Person).Any(c =>
                    c.Person.NationalCode == client.Person.NationalCode && c.Id != client.Id);

                // اگر فردی با این کد ملی در سامانه بود اما کد آن با کد این فرد که در حال ویرایش آن هستیم برابر نبود 
                // حتما در حال وارد کردن کد ملی تکراری هستیم
                if (isDuplicateNationalCode)
                    // چون یک فرد با این کد ملی در سیستم ثبت شده پس باید مجدد صفحه ویرایش به کاربر نمایش داده شود
                    return RedirectToAction("Index", new { Message = MessageClientManageId.DuplicateNationalCode });

                var clientInDb = _dbContext.Clients.Single(c => c.Id == client.Id);

                var personInDb = _dbContext.Persons.Single(p => p.Id == client.Id);

                _mapper.Map(client, clientInDb);
                _mapper.Map(client.Person, personInDb);

                if (clientInDb.CreatedDate == null) clientInDb.CreatedDate = DateTime.Now;
                if (personInDb.CreatedDate == null) personInDb.CreatedDate = DateTime.Now;
                clientInDb.UpdatedDate = personInDb.UpdatedDate = DateTime.Now;
                _dbContext.SaveChanges();
            }


            // نمایش سربرگ فعلی و ادامه ویرایش مددجو
            return RedirectToAction("EditBasic",
                new { id = client.Id, message = MessageClientManageId.DataSavedSuccessfully });
        }


        // ویرایش اطلاعات نوع تقاضای مددجو
        public ActionResult EditExemptionRequest(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var clientExemptionRequestInDb =
                _dbContext.ClientRequests
                    .Include(c => c.RequestType)
                    .Include(c => c.GetLetters)
                    .Include(c => c.GetLetters.Select(gl => gl.LetterPhoto))
                    .Include(c => c.ExemptionBenefits)
                    .Include(c => c.ExemptionBenefits.Select(eb => eb.BenefitPhoto))
                    .Where(c => c.ClientId == client.Id)
                    .ToList();

            var allExemptionRequestTypesInDb = _dbContext.RequestTypes.Where(r => r.IsExemption).ToList();

            var clientExemptionDetails = (from requestType in allExemptionRequestTypesInDb
                                          let clientExemption =
                                              clientExemptionRequestInDb.SingleOrDefault(cr => cr.RequestTypeId == requestType.Id)
                                          select new ClientExemptionDetailViewModel
                                          {
                                              RequestType = requestType,
                                              GetLetters = clientExemption?.GetLetters,
                                              ExemptionBenefits = clientExemption?.ExemptionBenefits,
                                              HasRequestExemption = clientExemption != null,
                                              HasGetExemptionLetter = clientExemption?.GetLetters.Any() ?? false,
                                              HasBenefitedFromExemption = clientExemption?.ExemptionBenefits.Any() ?? false,
                                          }).ToList();


            //foreach (var requestType in allExemptionRequestTypesInDb)
            //{
            //    var clientExemption = clientExemptionRequestInDb.SingleOrDefault(cr => cr.RequestTypeId == requestType.Id);
            //    var exemptionDetail = new ClientExemptionDetailViewModel
            //    {
            //        RequestTypeId = requestType.Id,
            //        HasRequestExemption = clientExemption != null,
            //        HasGetExemptionLetter = false,
            //        HasBenefitedFromExemption = false,
            //    };
            //    clientExemptionDetails.Add(exemptionDetail);
            //}

            var viewModel = new ClientExemptionRequestViewModel
            {
                ClientExemptionDetails = clientExemptionDetails,

                ClientId = id,
                Client = _mapper.Map<ClientDto>(client),
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error ? ModelEnums.StatusTypeE.Error
                        : message == MessageClientManageId.ValidationError ? ModelEnums.StatusTypeE.ValidationError
                        : ModelEnums.StatusTypeE.Warning
                };

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveClientExemptionRequestFormPhoto(ClientExemptionRequestViewModel viewModel)
        {
            ModelState.Remove("BenefitFormRequestTypeId");
            ModelState.Remove("BenefitFormLetterDate");
            ModelState.Remove("BenefitAmount");
            ModelState.Remove("BenefitFormTemporaryImageId");


            PersianDateTime pDate;
            var isCorrectDate = PersianDateTime.TryParse(viewModel.GetFormLetterDate, out pDate);

            if (!ModelState.IsValid || !isCorrectDate)
                return RedirectToAction("EditExemptionRequest",
                    new { id = viewModel.ClientId, message = MessageClientManageId.ValidationError });

            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.City.District.County)
                .Include(c => c.ClientRequests)
                .Include(c => c.ClientRequests.Select(cr => cr.RequestType))
                .SingleOrDefault(c => c.Id == viewModel.ClientId);
            if (clientInDb == null)
                return HttpNotFound();

            var clientRequestInDb =
                clientInDb.ClientRequests.SingleOrDefault(cr => cr.RequestType.Id == viewModel.GetLetterRequestTypeId);
            if (clientRequestInDb == null)
                return HttpNotFound();

            var tempImage = _dbContext.TemporaryImages.SingleOrDefault(t => t.Id == viewModel.GetFormTemporaryImageId);
            if (tempImage == null)
                return HttpNotFound();


            var newAddress = GetOrCreateClientDocumentFolder(clientInDb.City.District.County.ProvinceId,
                clientInDb.Person.NationalCode);

            var filesStoredOnServerWithThisName =
                Directory.GetFiles(newAddress, $"GetLetter-{clientRequestInDb.RequestTypeId}#Photo-*");

            var lastPhotoIndex = 0;

            foreach (var fileName in filesStoredOnServerWithThisName)
            {
                var fileNameSections = fileName.Split('#');
                if (fileNameSections.Length < 2) continue;
                var photoIdStr = fileNameSections[1].Split('-');
                if (photoIdStr.Length < 2) continue;
                photoIdStr = photoIdStr[1].Split('.');
                if (photoIdStr.Length < 2) continue;
                int outInt;
                if (int.TryParse(photoIdStr[0], out outInt))
                    lastPhotoIndex = outInt > lastPhotoIndex ? outInt : lastPhotoIndex;
            }

            newAddress = Path.Combine(newAddress,
                $"GetLetter-{clientRequestInDb.RequestTypeId}#Photo-{lastPhotoIndex + 1}{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

            System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

            _dbContext.TemporaryImages.Remove(tempImage);

            var clientDoc = new ClientDocument
            {
                ClientId = clientInDb.Id,
                DocumentTypeId = (int)ModelEnums.DocumentTypesE.RequestGetLetter,
                IsOptimized = false,
                DocURI = newAddress
            };
            _dbContext.ClientDocuments.Add(clientDoc);
            _dbContext.SaveChanges();


            var clientRequestGetLetter = new ClientRequestGetLetter
            {
                ClientRequestId = clientRequestInDb.Id,
                LetterDate = pDate.ToDateTime(),
                LetterNumber = viewModel.GetFormLetterNumber,
                LetterPhotoId = clientDoc.Id
            };

            _dbContext.ClientRequestGetLetters.Add(clientRequestGetLetter);
            _dbContext.SaveChanges();

            return RedirectToAction("EditExemptionRequest",
                new { id = viewModel.ClientId, message = MessageClientManageId.DataSavedSuccessfully });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveClientExemptionBenefitFormPhoto(ClientExemptionRequestViewModel viewModel)
        {
            ModelState.Remove("GetLetterRequestTypeId");
            ModelState.Remove("GetFormLetterDate");
            ModelState.Remove("GetFormLetterNumber");
            ModelState.Remove("GetFormTemporaryImageId");

            long amount;
            var amountIsNumber = long.TryParse(viewModel.BenefitAmount.Replace(",", ""), out amount);
            PersianDateTime pDate;
            var isCorrectDate = PersianDateTime.TryParse(viewModel.BenefitFormLetterDate, out pDate);

            if (!ModelState.IsValid || !amountIsNumber || !isCorrectDate)
                return RedirectToAction("EditExemptionRequest",
                    new { id = viewModel.ClientId, message = MessageClientManageId.ValidationError });

            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.City.District.County)
                .Include(c => c.ClientRequests)
                .Include(c => c.ClientRequests.Select(cr => cr.RequestType))
                .SingleOrDefault(c => c.Id == viewModel.ClientId);
            if (clientInDb == null)
                return HttpNotFound();

            var clientRequestInDb =
                clientInDb.ClientRequests.SingleOrDefault(cr =>
                    cr.RequestType.Id == viewModel.BenefitFormRequestTypeId);
            if (clientRequestInDb == null)
                return HttpNotFound();

            var tempImage =
                _dbContext.TemporaryImages.SingleOrDefault(t => t.Id == viewModel.BenefitFormTemporaryImageId);
            if (tempImage == null)
                return HttpNotFound();


            var newAddress = GetOrCreateClientDocumentFolder(clientInDb.City.District.County.ProvinceId,
                clientInDb.Person.NationalCode);

            var filesStoredOnServerWithThisName =
                Directory.GetFiles(newAddress, $"BenefitLetter-{clientRequestInDb.RequestTypeId}#Photo-*");

            var lastPhotoIndex = 0;

            foreach (var fileName in filesStoredOnServerWithThisName)
            {
                var fileNameSections = fileName.Split('#');
                if (fileNameSections.Length < 2) continue;
                var photoIdStr = fileNameSections[1].Split('-');
                if (photoIdStr.Length < 2) continue;
                photoIdStr = photoIdStr[1].Split('.');
                if (photoIdStr.Length < 2) continue;
                int outInt;
                if (int.TryParse(photoIdStr[0], out outInt))
                    lastPhotoIndex = outInt > lastPhotoIndex ? outInt : lastPhotoIndex;
            }

            newAddress = Path.Combine(newAddress,
                $"BenefitLetter-{clientRequestInDb.RequestTypeId}#Photo-{lastPhotoIndex + 1}{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

            System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

            _dbContext.TemporaryImages.Remove(tempImage);

            var clientDoc = new ClientDocument
            {
                ClientId = clientInDb.Id,
                DocumentTypeId = (int)ModelEnums.DocumentTypesE.RequestExemptionBenefit,
                IsOptimized = false,
                DocURI = newAddress
            };
            _dbContext.ClientDocuments.Add(clientDoc);
            _dbContext.SaveChanges();

            var clientExemptionBenefit = new ClientExemptionBenefit
            {
                ClientRequestId = clientRequestInDb.Id,
                BenefitDate = pDate.ToDateTime(),
                BenefitAmount = amount,
                BenefitPhotoId = clientDoc.Id
            };

            _dbContext.ClientExemptionBenefits.Add(clientExemptionBenefit);
            _dbContext.SaveChanges();

            return RedirectToAction("EditExemptionRequest",
                new { id = viewModel.ClientId, message = MessageClientManageId.DataSavedSuccessfully });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteGetLetter(int id)
        {
            var getLetterInDb = _dbContext.ClientRequestGetLetters
                .Include(gl => gl.ClientRequest)
                .Include(gl => gl.LetterPhoto)
                .SingleOrDefault(gl => gl.Id == id);

            if (getLetterInDb == null)
                HttpNotFound();

            // ReSharper disable once PossibleNullReferenceException
            var clientDocPhotoId = getLetterInDb.LetterPhotoId;
            var clientId = getLetterInDb.ClientRequest.ClientId;

            _dbContext.ClientRequestGetLetters.Remove(getLetterInDb);
            _dbContext.SaveChanges();


            var clientDocumentInDb =
                _dbContext.ClientDocuments.SingleOrDefault(cd => cd.Id == clientDocPhotoId);
            if (clientDocumentInDb == null)
                return HttpNotFound();
            try
            {
                System.IO.File.Delete(clientDocumentInDb.DocURI);

            }
            catch (Exception)
            {
            }

            _dbContext.ClientDocuments.Remove(clientDocumentInDb);
            _dbContext.SaveChanges();


            return RedirectToAction("EditExemptionRequest", new { id = clientId });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteBenefitLetter(int id)
        {
            var exemptionBenefitInDb = _dbContext.ClientExemptionBenefits
                .Include(eb => eb.ClientRequest)
                .Include(eb => eb.BenefitPhoto)
                .SingleOrDefault(eb => eb.Id == id);

            if (exemptionBenefitInDb == null)
                HttpNotFound();

            // ReSharper disable once PossibleNullReferenceException
            var clientDocPhotoId = exemptionBenefitInDb.BenefitPhotoId;
            var clientId = exemptionBenefitInDb.ClientRequest.ClientId;

            _dbContext.ClientExemptionBenefits.Remove(exemptionBenefitInDb);
            _dbContext.SaveChanges();


            var clientDocumentInDb =
                _dbContext.ClientDocuments.SingleOrDefault(cd => cd.Id == clientDocPhotoId);
            if (clientDocumentInDb == null)
                return HttpNotFound();
            try
            {
                System.IO.File.Delete(clientDocumentInDb.DocURI);
            }
            catch (Exception)
            {
            }

            _dbContext.ClientDocuments.Remove(clientDocumentInDb);
            _dbContext.SaveChanges();


            return RedirectToAction("EditExemptionRequest",
                new { id = clientId, message = MessageClientManageId.DataSavedSuccessfully });
        }


        // ویرایش اطلاعات نوع تقاضای مددجو
        public ActionResult EditRequest(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            //int a = 90;
            var allRequests =
                _dbContext.ClientRequests
                    .Where(c => c.ClientId == client.Id)
                    .Include(c => c.RequestType)
                    .Include(r => r.ExemptionBenefits)
                    .ToList();

            var clientBuildingRequest = allRequests.SingleOrDefault(r => r.RequestType.IsExemption == false);
            int requestTypeBuildingId;
            if (clientBuildingRequest == null)
            {
                // در صورتی که مددجو به عنوان مددجوی تایید شده ثبت شده بود
                // اما در بخش درخواست ها هیچ تقاضایی برای او ثبت نشده بود
                // احتمالا از افرادی است که از فایل اکسل وارد نرم افزار شده اند
                // بنا براین باید درخواستی برای آن ها ثبت شود

                // به صورت پیش فرض درخواست ساختن خانه برای آنها ثبت می گردد.

                var newClientRequest = new ClientRequest
                {
                    RequestTypeId = 3, //Build a House Can Buy Land
                    ClientId = client.Id,
                };

                _dbContext.ClientRequests.Add(newClientRequest);
                _dbContext.SaveChanges();

                requestTypeBuildingId = 4;
            }
            else
            {
                requestTypeBuildingId = clientBuildingRequest.RequestTypeId;
            }

            var viewModel = new ClientRequestViewModel
            {
                RequestTypeBuildingId = requestTypeBuildingId,

                HasRequestWaterExemption =
                    allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionWater),
                HasBenefitWaterExemption = allRequests.Any(r =>
                    r.RequestType.Name == ClientRequestTypeStr.ExemptionWater && r.ExemptionBenefits.Any()),

                HasRequestGasExemption = allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionGas),
                HasBenefitGasExemption = allRequests.Any(r =>
                    r.RequestType.Name == ClientRequestTypeStr.ExemptionGas && r.ExemptionBenefits.Any()),

                HasRequestElectricalExemption =
                    allRequests.Any(r => r.RequestType.Name == ClientRequestTypeStr.ExemptionElectrical),
                HasBenefitElectricalExemption = allRequests.Any(r =>
                    r.RequestType.Name == ClientRequestTypeStr.ExemptionElectrical && r.ExemptionBenefits.Any()),

                HasRequestProductionLicenseExemption = allRequests.Any(r =>
                    r.RequestType.Name == ClientRequestTypeStr.ExemptionProductionLicense),
                HasBenefitProductionLicenseExemption = allRequests.Any(r =>
                    r.RequestType.Name == ClientRequestTypeStr.ExemptionProductionLicense && r.ExemptionBenefits.Any()),

                HasBenefitFromBuildingAid = client.ClientPhysicalProgresses.Any(ph =>
                    ph.PhysicalProgressId == (int)ModelEnums.PhysicalProgressTypeE.VagozarShodeh),

                RequestTypes = _dbContext.RequestTypes.ToList(),
                ClientId = id,
                Client = _mapper.Map<ClientDto>(client),
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully
                        ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error
                            ? ModelEnums.StatusTypeE.Error
                            : ModelEnums.StatusTypeE.Warning
                };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRequest(ClientRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Status = new Status { StatusType = ModelEnums.StatusTypeE.ValidationError };
                return RedirectToAction("EditRequest",
                new { id = viewModel.ClientId, message = MessageClientManageId.ValidationError });
                //return View("EditRequest", viewModel);
            }

            // در صورتی که فقط گزینه دریافت معافیت انشعابات را انتخاب کرده بود، اما هیچ کدام از تیک موارد معافیت را نزده بود
            // مانند ولیدیشن ارور هست
            // پس باید به صفحه مربوطه برگردانده شود
            if (viewModel.RequestTypeBuildingId == 0 &&
                !(viewModel.HasRequestWaterExemption || viewModel.HasRequestGasExemption ||
                  viewModel.HasRequestElectricalExemption || viewModel.HasRequestProductionLicenseExemption) &&
                  !(viewModel.HasBenefitElectricalExemption || viewModel.HasBenefitFromBuildingAid ||
                  viewModel.HasBenefitGasExemption || viewModel.HasBenefitWaterExemption))
            {
                viewModel.Status = new Status { StatusType = ModelEnums.StatusTypeE.ValidationError };
                return RedirectToAction("EditRequest",
                new { id = viewModel.ClientId, message = MessageClientManageId.ValidationError });
                //return View("EditRequest", viewModel);
            }

            //var test = 1;
            var allClientRequests = _dbContext.ClientRequests
                .Include(cr => cr.RequestType)
                .Include(cr => cr.Client)
                .Include(cr => cr.ExemptionBenefits)
                .Where(cr => cr.ClientId == viewModel.ClientId).ToList();


            // اگر در پایگاه داده هیچ کدام از موارد تقاضای مسکن برای مددجو انتخاب نشده بود 
            // حتما خطایی صورت گرفته است و باید کاربر به صفحه ویرایش تقاضا برگردانده شود.
            var clientBuildingRequestInDb = allClientRequests.SingleOrDefault(r => r.RequestType.IsExemption == false);
            if (clientBuildingRequestInDb == null)
                return RedirectToAction("EditRequest", new { id = viewModel.ClientId });


            var allRequestTypeInDb = _dbContext.RequestTypes.ToList();


            // **************************
            // در اینجا باید چک شود که آیا مددجو از خدمت انتخاب شده بهره مند شده است یا خیر
            // در صورتی که مددجو از یکی از تقاضاهای خود بهره مند شده است نباید اجازه ویرایش و برداشتن تیک آن تقاضا را داشته باشد.


            // هنوز کد بالا مربوط به بررسی بهره مندی تقاضا نوشته نشده است
            //  این کد به صورت موقت برای ذخیره اطلاعات تقاضای مددجو درمورد مسکن نوشته شده است و بعدا باید ویرایش شود

            clientBuildingRequestInDb.RequestTypeId = viewModel.RequestTypeBuildingId;
            _dbContext.SaveChanges();

            // *************************


            //------------------------------------------------------------------------------------------------------------------------------
            // تقاضای معافیت انشعاب آب
            //------------------------------------------------------------------------------------------------------------------------------
            var waterExemptionId = allRequestTypeInDb.Single(r => r.Name == ClientRequestTypeStr.ExemptionWater).Id;

            var clientWaterExemption =
                allClientRequests.SingleOrDefault(r => r.RequestTypeId == waterExemptionId);

            var hasBenefitWaterExemption =
                allClientRequests.Any(r => r.RequestTypeId == waterExemptionId && r.ExemptionBenefits.Any());

            // اگر کارشناس تیک معافیت انشعاب آب را زده بود
            if (viewModel.HasRequestWaterExemption)
            {
                // و تقاضای معافیت انشعاب آب در پایگاه داده ثبت نشده بود
                if (clientWaterExemption == null)
                {
                    // تقاضای معافیت آب رو به پایگاه داده اضافه کن
                    clientWaterExemption = new ClientRequest
                    {
                        RequestTypeId = waterExemptionId,
                        ClientId = viewModel.ClientId
                    };
                    _dbContext.ClientRequests.Add(clientWaterExemption);
                }

                // اگر هم که تقاضا ثبت شده بود نیازی به انجام هیچ کاری نیست
            }
            else //اگر تیک معافت انشعاب آب نخورده بود
            {
                // ولی در پایگاه داده معافیت انشعاب آب تیک خورده بود
                if (clientWaterExemption != null && !hasBenefitWaterExemption)
                    // در اینجا باید بررسی کنیم که آیا از معافیت انشعاب آب استفاده کرده است یا خیر

                    // در صورتی که بهره مند نشده بود باید این درخواست از پایگاه داده حذف شود.

                    // فعلا به صورت موقت کد حذف از پایگاه داده نوشته می شود تا بعدا بررسی بهره مند شدن انجام گیرد
                    _dbContext.ClientRequests.Remove(clientWaterExemption);
                // در صورتی هم که تیک آب زده نشده بود و در پایگاه داده هم چیزی نبود نیاز به انجام کاری نمی باشد.
            }


            //------------------------------------------------------------------------------------------------------------------------------
            // تقاضای معافیت انشعاب برق
            //------------------------------------------------------------------------------------------------------------------------------
            var electricalExemptionId =
                allRequestTypeInDb.Single(r => r.Name == ClientRequestTypeStr.ExemptionElectrical).Id;
            var clientElectricalExemption =
                allClientRequests.SingleOrDefault(r => r.RequestTypeId == electricalExemptionId);
            var hasBenefitElectricalExemption =
                allClientRequests.Any(r => r.RequestTypeId == electricalExemptionId && r.ExemptionBenefits.Any());

            // اگر کارشناس تیک معافیت انشعاب برق را زده بود
            if (viewModel.HasRequestElectricalExemption)
            {
                // و تقاضای معافیت انشعاب برق در پایگاه داده ثبت نشده بود
                if (clientElectricalExemption == null)
                {
                    // تقاضای معافیت برق رو به پایگاه داده اضافه کن
                    clientElectricalExemption = new ClientRequest
                    {
                        RequestTypeId = electricalExemptionId,
                        ClientId = viewModel.ClientId
                    };
                    _dbContext.ClientRequests.Add(clientElectricalExemption);
                }

                // اگر هم که تقاضا ثبت شده بود نیازی به انجام هیچ کاری نیست
            }
            else //اگر تیک معافت انشعاب برق نخورده بود
            {
                // ولی در پایگاه داده معافیت انشعاب برق تیک خورده بود
                if (clientElectricalExemption != null && !hasBenefitElectricalExemption)
                    // در اینجا باید بررسی کنیم که آیا از معافیت انشعاب برق استفاده کرده است یا خیر

                    // در صورتی که بهره مند نشده بود باید این درخواست از پایگاه داده حذف شود.

                    // فعلا به صورت موقت کد حذف از پایگاه داده نوشته می شود تا بعدا بررسی بهره مند شدن انجام گیرد
                    _dbContext.ClientRequests.Remove(clientElectricalExemption);
                // در صورتی هم که تیک برق زده نشده بود و در پایگاه داده هم چیزی نبود نیاز به انجام کاری نمی باشد.
            }


            //------------------------------------------------------------------------------------------------------------------------------
            // تقاضای معافیت انشعاب گاز
            //------------------------------------------------------------------------------------------------------------------------------
            var gasExemptionId = allRequestTypeInDb.Single(r => r.Name == ClientRequestTypeStr.ExemptionGas).Id;
            var clientGasExemption =
                allClientRequests.SingleOrDefault(r => r.RequestTypeId == gasExemptionId);
            var hasBenefitGasExemption =
                allClientRequests.Any(r => r.RequestTypeId == gasExemptionId && r.ExemptionBenefits.Any());

            // اگر کارشناس تیک معافیت انشعاب گاز را زده بود
            if (viewModel.HasRequestGasExemption)
            {
                // و تقاضای معافیت انشعاب گاز در پایگاه داده ثبت نشده بود
                if (clientGasExemption == null)
                {
                    // تقاضای معافیت گاز رو به پایگاه داده اضافه کن
                    clientGasExemption = new ClientRequest
                    {
                        RequestTypeId = gasExemptionId,
                        ClientId = viewModel.ClientId
                    };
                    _dbContext.ClientRequests.Add(clientGasExemption);
                }

                // اگر هم که تقاضا ثبت شده بود نیازی به انجام هیچ کاری نیست
            }
            else //اگر تیک معافت انشعاب گاز نخورده بود
            {
                // ولی در پایگاه داده معافیت انشعاب گاز تیک خورده بود
                if (clientGasExemption != null && !hasBenefitGasExemption)
                    // در اینجا باید بررسی کنیم که آیا از معافیت انشعاب گاز استفاده کرده است یا خیر

                    // در صورتی که بهره مند نشده بود باید این درخواست از پایگاه داده حذف شود.

                    // فعلا به صورت موقت کد حذف از پایگاه داده نوشته می شود تا بعدا بررسی بهره مند شدن انجام گیرد
                    _dbContext.ClientRequests.Remove(clientGasExemption);
                // در صورتی هم که تیک گاز زده نشده بود و در پایگاه داده هم چیزی نبود نیاز به انجام کاری نمی باشد.
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // تقاضای معافیت انشعاب پروانه ساخت
            //------------------------------------------------------------------------------------------------------------------------------
            var productionLicenseExemptionId = allRequestTypeInDb
                .Single(r => r.Name == ClientRequestTypeStr.ExemptionProductionLicense).Id;
            var clientProductionLicenseExemption =
                allClientRequests.SingleOrDefault(r => r.RequestTypeId == productionLicenseExemptionId);
            var hasBenefitProductionLicenseExemption =
                allClientRequests.Any(r =>
                    r.RequestTypeId == productionLicenseExemptionId && r.ExemptionBenefits.Any());

            // اگر کارشناس تیک معافیت انشعاب پروانه ساخت را زده بود
            if (viewModel.HasRequestProductionLicenseExemption)
            {
                // و تقاضای معافیت انشعاب پروانه ساخت در پایگاه داده ثبت نشده بود
                if (clientProductionLicenseExemption == null)
                {
                    // تقاضای معافیت پروانه ساخت رو به پایگاه داده اضافه کن
                    clientProductionLicenseExemption = new ClientRequest
                    {
                        RequestTypeId = productionLicenseExemptionId,
                        ClientId = viewModel.ClientId
                    };
                    _dbContext.ClientRequests.Add(clientProductionLicenseExemption);
                }

                // اگر هم که تقاضا ثبت شده بود نیازی به انجام هیچ کاری نیست
            }
            else //اگر تیک معافت انشعاب پروانه ساخت نخورده بود
            {
                // ولی در پایگاه داده معافیت انشعاب پروانه ساخت تیک خورده بود
                if (clientProductionLicenseExemption != null && !hasBenefitProductionLicenseExemption)
                    // در اینجا باید بررسی کنیم که آیا از معافیت انشعاب پروانه ساخت استفاده کرده است یا خیر

                    // در صورتی که بهره مند نشده بود باید این درخواست از پایگاه داده حذف شود.

                    // فعلا به صورت موقت کد حذف از پایگاه داده نوشته می شود تا بعدا بررسی بهره مند شدن انجام گیرد
                    _dbContext.ClientRequests.Remove(clientProductionLicenseExemption);
                // در صورتی هم که تیک پروانه ساخت زده نشده بود و در پایگاه داده هم چیزی نبود نیاز به انجام کاری نمی باشد.
            }

            _dbContext.SaveChanges();

            return RedirectToAction("EditRequest",
                new { id = viewModel.ClientId, message = MessageClientManageId.DataSavedSuccessfully });
        }

        public ActionResult GetClientExemptionForm(int id, int requestTypeId)
        {
            // باید فایل ورد دانلود شود
            return null;

            //var clientDocumentInDb = _dbContext.ClientDocuments.SingleOrDefault(cd => cd.Id == id);
            //return clientDocumentInDb == null ? null : File(clientDocumentInDb.DocURI, "image/jpeg");
        }

        // مسکن فعلی
        public ActionResult EditCurrentHousing(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var viewModel = new CurrentHousingViewModel
            {
                CurrentHouseTypes = _dbContext.CurrentHouseTypes.AsEnumerable(),
                CountyOfCurrentHousing = _dbContext.Counties.Where(c => c.ProvinceId == ProvinceIdFix).AsEnumerable()
                    .Select(_mapper.Map<CountyDto>),
                DistrictOfCurrentHousing = new List<DistrictDto>(),
                CityOfCurrentHousing = new List<CityDto>(),
                ClientId = id,
                Client = _mapper.Map<ClientDto>(client),
            };

            var currentHousingInDb = _dbContext.CurrentHousings
                .Include(c => c.City)
                .Include(c => c.City.District)
                .SingleOrDefault(c => c.Id == id);
            if (currentHousingInDb == null)
            {
                viewModel.CurrentHousing = new CurrentHousing
                {
                    Latitude = 29.61342,
                    Longitude = 52.50611
                };
            }
            else
            {
                // باید لیست تمام بخش ها و شهر و روستاهای هم رده با آدرس منزل مددجو را برای کاربر ارسال کنیم

                var countyIdOfCurrentHouse = currentHousingInDb.City.District.CountyId;
                viewModel.DistrictOfCurrentHousing = _dbContext.Districts
                    .Where(d => d.CountyId == countyIdOfCurrentHouse)
                    .AsEnumerable()
                    .Select(_mapper.Map<DistrictDto>);

                var districtIdOfCurrentHouse = client.CurrentHousing.City.DistrictId;
                viewModel.CityOfCurrentHousing = _dbContext.Cities
                    .Where(c => c.DistrictId == districtIdOfCurrentHouse)
                    .AsEnumerable()
                    .Select(_mapper.Map<CityDto>);

                viewModel.CurrentHousing = currentHousingInDb;
            }

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully
                        ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error
                            ? ModelEnums.StatusTypeE.Error
                            : ModelEnums.StatusTypeE.Warning
                };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCurrentHousing(CurrentHousing currentHousing)
        {
            //this is a temp solution i need details !!!
            //currentHousing.CurrentHouseTypeId = 3;
            ModelState.Remove("CurrentHouseTypeId");
            if (!ModelState.IsValid)
            {
                var viewModel = new CurrentHousingViewModel
                {
                    CurrentHouseTypes = _dbContext.CurrentHouseTypes.AsEnumerable(),
                    CountyOfCurrentHousing = _dbContext.Counties.Where(c => c.ProvinceId == ProvinceIdFix)
                        .AsEnumerable().Select(_mapper.Map<CountyDto>),
                    DistrictOfCurrentHousing = new List<DistrictDto>(),
                    CityOfCurrentHousing = new List<CityDto>(),
                    CurrentHousing = currentHousing
                };
                viewModel.Status = new Status { StatusType = ModelEnums.StatusTypeE.ValidationError };
                return View("EditCurrentHousing", viewModel);
            }

            if (currentHousing.Id == 0)
                return RedirectToAction("Index", new
                {
                    Message = MessageClientManageId.Error
                });

            var currentHousingInDb = _dbContext.CurrentHousings.SingleOrDefault(c => c.Id == currentHousing.Id);
            if (currentHousingInDb == null)
                _dbContext.CurrentHousings.Add(currentHousing);
            else
                _mapper.Map(currentHousing, currentHousingInDb);

            _dbContext.SaveChanges();

            return RedirectToAction("EditCurrentHousing",
                new { id = currentHousing.Id, message = MessageClientManageId.DataSavedSuccessfully });
        }


        public ActionResult EditFamily(int id)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            if (User.IsInRole(RoleName.SazmanHamkar))
                return RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var familyRelationsInDb = _dbContext.FamilyRelations.Where(c => c.PersonMajorId == id)
                .Include(c => c.PersonMinor)
                .Include(c => c.FamilyRelationType);

            var familyDtos = new List<FamilyRelationDto>();

            foreach (var familyRelation in familyRelationsInDb)
            {
                var familyPersonDto = new FamilyRelationDto
                {
                    Id = familyRelation.Id,
                    PersonId = familyRelation.PersonMinorId,
                    Name = familyRelation.PersonMinor.Name,
                    Family = familyRelation.PersonMinor.Family,
                    NationalCode = familyRelation.PersonMinor.NationalCode,
                    IsDisabled = familyRelation.PersonMinor.IsDisabled ?? false,
                    RelationTypeName = familyRelation.FamilyRelationType.Name,
                    RelationTypeId = familyRelation.FamilyRelationTypeId,
                    Description = familyRelation.Description
                };
                familyDtos.Add(familyPersonDto);
            }

            var viewModel = new FamilyViewModel
            {
                ClientId = id,
                FamilyRelationDtos = familyDtos,
                FamilyRelationTypes = _dbContext.FamilyRelationTypes.ToList().AsEnumerable(),
                Client = _mapper.Map<ClientDto>(client),
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);
            return View(viewModel);
        }

        // اطلاعات تماس
        public ActionResult EditContactInfo(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var viewModel = new ContactInfoViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                ClientId = id,
                ContactInfo = _dbContext.ContactInfos.SingleOrDefault(c => c.Id == id)
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully
                        ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error
                            ? ModelEnums.StatusTypeE.Error
                            : ModelEnums.StatusTypeE.Warning
                };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveContactInfo(ContactInfo contactInfo)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ContactInfoViewModel
                {
                    ContactInfo = contactInfo,
                    ClientId = contactInfo.Id
                };
                viewModel.Status = new Status { StatusType = ModelEnums.StatusTypeE.ValidationError };
                return View("EditContactInfo", viewModel);
            }

            // اگر کد مددجو صفر بود حتما خطایی صورت گرفته پس کاربر را به صفحه لیست برمیگرداند
            if (contactInfo.Id == 0)
                return RedirectToAction("Index", new
                {
                    Message = MessageClientManageId.Error
                });

            var contactInfoInDb = _dbContext.ContactInfos.SingleOrDefault(c => c.Id == contactInfo.Id);
            if (contactInfoInDb == null)
                _dbContext.ContactInfos.Add(contactInfo);
            else
                _mapper.Map(contactInfo, contactInfoInDb);

            _dbContext.SaveChanges();

            //return RedirectToAction("Index");
            return RedirectToAction("EditContactInfo",
                new { id = contactInfo.Id, message = MessageClientManageId.DataSavedSuccessfully });
        }


        // اطلاعات بانکی
        public ActionResult EditBankInfo(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;


            if (User.IsInRole(RoleName.SazmanHamkar))
                return RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var viewModel = new BankInfoViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                ClientId = id,
                BankTypes = _dbContext.BankTypes.AsEnumerable(),
                BankInfo = _dbContext.BankInfos
                    .Include(b => b.Client)
                    .Include(b => b.AccountApproveImage)
                    .SingleOrDefault(c => c.Id == id)
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);


            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully
                        ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error
                            ? ModelEnums.StatusTypeE.Error
                            : ModelEnums.StatusTypeE.Warning
                };


            return View(viewModel);
        }


        // ذخیره اطلاعات بانکی
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBankInfo(BankInfoViewModel viewModel)
        {
            Client clientOut;
            ActionResult result;

            // در صورتی که کاربر مجوز ویرایش مددجو را نداشته باشد او را به صفحه دیگری منقل می نماید
            if (!CheckPrivilegeForSaveClient(viewModel.BankInfo.Id, out clientOut, out result, null)) return result;

            // در صورتی که در اطلاعات فرم خطا وجود داشته باشد
            if (!ModelState.IsValid)
            {
                viewModel.BankTypes = _dbContext.BankTypes.AsEnumerable();

                PopulateMenuTabItemOfRegisteredClientForm(viewModel.BankInfo.Id);
                viewModel.Client = _mapper.Map<ClientDto>(clientOut);

                viewModel.Status = new Status { StatusType = ModelEnums.StatusTypeE.ValidationError };
                return View("EditBankInfo", viewModel);
            }

            // اگر کد مددجو صفر بود حتما خطایی صورت گرفته پس کاربر را به صفحه لیست برمیگرداند
            if (viewModel.BankInfo.Id == 0)
                return RedirectToAction("Index", new
                {
                    Message = MessageClientManageId.Error
                });

            // اطلاعات بانکی مددجو را در پایگاه داده پیدا کن
            var bankInfoInDb = _dbContext.BankInfos
                .Include(b => b.Client)
                .Include(b => b.AccountApproveImage)
                .SingleOrDefault(c => c.Id == viewModel.BankInfo.Id);
            if (bankInfoInDb == null)
            {
                // اگر ردیف در پایگاه داده قبلا ثبت نشده بود آن را اضافه کن
                _dbContext.BankInfos.Add(viewModel.BankInfo);
                bankInfoInDb = viewModel.BankInfo;
            }
            else
            {
                bankInfoInDb.CardNumber = viewModel.BankInfo.CardNumber;
                bankInfoInDb.AccountNumber = viewModel.BankInfo.AccountNumber;
                bankInfoInDb.BankTypeId = viewModel.BankInfo.BankTypeId;
                // اگر قبلا اطلاعات بانکی این مددجو ثبت شده بود آنها را به روزرسانی کن
                //_mapper.Map(viewModel.BankInfo, bankInfoInDb);
            }

            _dbContext.SaveChanges();

            // در صورتی که کد عکس موقت مخالف با صفر بود به این معنی است که عکس موقتی در پایگاه داده آپلود شده است
            // که این عکس باید در پوشه مدارک مددجو ذخیره گردد
            if (viewModel.TemporaryImageId != 0)
            {
                var tempImage = _dbContext.TemporaryImages.SingleOrDefault(t => t.Id == viewModel.TemporaryImageId);
                if (tempImage == null)
                    return HttpNotFound();

                var newAddress = GetOrCreateClientDocumentFolder(clientOut.City.District.County.ProvinceId,
                    clientOut.Person.NationalCode);

                newAddress = Path.Combine(newAddress,
                    $"BankApproveImage{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

                // اگر فایلی با این نام وجود داشت آن را حذف کن
                if (System.IO.File.Exists(newAddress)) System.IO.File.Delete(newAddress);

                // فایل را از پوشه عکس های موقت به پوشه مدارک مددجو منتقل کن
                System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

                // ردیف عکس موقت را از پایگاه داده حذف کن
                _dbContext.TemporaryImages.Remove(tempImage);

                // اگر برای این مددجو قبلا تاییدیه حساب بارگذاری شده بود  
                if (bankInfoInDb.AccountApproveImage != null)
                {
                    // اطلاعات مدارک مددجو را ویرایش کن
                    bankInfoInDb.AccountApproveImage.DocURI = newAddress;
                    bankInfoInDb.AccountApproveImage.IsOptimized = false;
                }
                else
                {
                    bankInfoInDb.AccountApproveImage = new ClientDocument
                    {
                        ClientId = clientOut.Id,
                        DocumentTypeId = (int)ModelEnums.DocumentTypesE.BankTaeidHesabOffline,
                        IsOptimized = false,
                        DocURI = newAddress
                    };
                    _dbContext.ClientDocuments.Add(bankInfoInDb.AccountApproveImage);
                }

                _dbContext.SaveChanges();
            }
            else
            {
                // اگر کد عکس موقت برابر صفر بود
                // در صورتی که کد عکس تاییدیه حساب بانکی صفر بود و قبلا در پایگاه داده یک عکس ذخیره شده بود
                // به این معنی می باشد که کارشناس قصد دارد عکس آپلود شده را از سیستم حذف نماید
                if (viewModel.BankInfo.AccountApproveImageId != 0 && bankInfoInDb.AccountApproveImage != null)
                {
                    // فایل عکس را حذف کن
                    System.IO.File.Delete(bankInfoInDb.AccountApproveImage.DocURI);
                    var clientDocumentInDb =
                        _dbContext.ClientDocuments.Single(c => c.Id == bankInfoInDb.AccountApproveImageId);
                    // ردیف اطلاعات را از پایگاه داده حذف کن
                    _dbContext.ClientDocuments.Remove(clientDocumentInDb);
                    _dbContext.SaveChanges();
                }
            }

            return RedirectToAction("EditBankInfo",
                new { id = bankInfoInDb.Id, message = MessageClientManageId.DataSavedSuccessfully });
        }

        // پیشرفت فیزیکی
        public ActionResult EditClientPhysicalProgress(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;
            var date = PersianDateTime.Now;
            date.EnglishNumber = true;

            var viewModel = new ClientPhysicalProgressViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                AllPhysicalProgress = _dbContext.PhysicalProgresses.OrderBy(p => p.Order).ToList(),
                ClientPhysicalProgresses = _dbContext.ClientPhysicalProgresses.Where(cp => cp.ClientId == id)
                    .Include(c => c.ClientPhysicalProgressPhotos).ToList(),
            };

            //var date = PersianDateTime.Now;
            //viewModel.PhotoTakenDate = date;
            //viewModel.PhotoTakenDate.Value.
            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error ? ModelEnums.StatusTypeE.Error
                        : message == MessageClientManageId.ValidationError ? ModelEnums.StatusTypeE.ValidationError
                        : ModelEnums.StatusTypeE.Warning
                };

            return View(viewModel);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveClientPhysicalProgressPhoto(ClientPhysicalProgressViewModel viewModel)
        {
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            if (!ModelState.IsValid)
                return RedirectToAction("EditClientPhysicalProgress", new { id = viewModel.Client.Id, message = MessageClientManageId.ValidationError });

            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.City.District.County)
                .SingleOrDefault(c => c.Id == viewModel.Client.Id);
            if (clientInDb == null)
                return HttpNotFound();

            var clientPhysicalProgressInDb = _dbContext.ClientPhysicalProgresses.SingleOrDefault(cph =>
                cph.ClientId == clientInDb.Id && cph.PhysicalProgressId == viewModel.SelectedPhysicalProgressId);
            if (clientPhysicalProgressInDb == null)
            {
                clientPhysicalProgressInDb = new ClientPhysicalProgress
                {
                    ClientId = viewModel.Client.Id,
                    IsDone = false,
                    PhysicalProgressId = viewModel.SelectedPhysicalProgressId
                };
                _dbContext.ClientPhysicalProgresses.Add(clientPhysicalProgressInDb);
                _dbContext.SaveChanges();
            }

            var tempImage = _dbContext.TemporaryImages.SingleOrDefault(t => t.Id == viewModel.TemporaryImageId);
            if (tempImage == null)
                return HttpNotFound();

            var physicalProgressInDb =
                _dbContext.PhysicalProgresses.SingleOrDefault(p => p.Id == viewModel.SelectedPhysicalProgressId);
            if (physicalProgressInDb == null)
                return HttpNotFound();


            var newAddress = GetOrCreateClientDocumentFolder(clientInDb.City.District.County.ProvinceId,
                clientInDb.Person.NationalCode);

            var currentFiles =
                Directory.GetFiles(newAddress, $"PhysicalProgress-{physicalProgressInDb.Id}#Photo-*");

            var lastPhotoIndex = 0;

            foreach (var currentFile in currentFiles)
            {
                var fStrs = currentFile.Split('#');
                if (fStrs.Length < 2) continue;
                var photoIdStr = fStrs[1].Split('-');
                if (photoIdStr.Length < 2) continue;
                photoIdStr = photoIdStr[1].Split('.');
                if (photoIdStr.Length < 2) continue;
                int outInt;
                if (int.TryParse(photoIdStr[0], out outInt))
                    lastPhotoIndex = outInt > lastPhotoIndex ? outInt : lastPhotoIndex;
            }

            newAddress = Path.Combine(newAddress,
                $"PhysicalProgress-{physicalProgressInDb.Id}#Photo-{lastPhotoIndex + 1}{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

            System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

            _dbContext.TemporaryImages.Remove(tempImage);

            var clientDoc = new ClientDocument
            {
                ClientId = clientInDb.Id,
                DocumentTypeId = (int)ModelEnums.DocumentTypesE.PhysicalProgressPhoto,
                IsOptimized = false,
                DocURI = newAddress
            };
            _dbContext.ClientDocuments.Add(clientDoc);
            _dbContext.SaveChanges();



            var clientPhysicalProgressPhoto = new ClientPhysicalProgressPhoto
            {
                ClientPhysicalProgressId = clientPhysicalProgressInDb.Id,
                ClientDocument = clientDoc,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                Description = viewModel.Description
            };

            if (!string.IsNullOrEmpty(viewModel.PhotoTakenDate))
            {
                PersianDateTime pDate;
                var isCorrectDate = PersianDateTime.TryParse(viewModel.PhotoTakenDate, out pDate);
                if (isCorrectDate)
                    clientPhysicalProgressPhoto.PhotoTakenDate = pDate.ToDateTime();
            }

            _dbContext.ClientPhysicalProgressPhotos.Add(clientPhysicalProgressPhoto);
            _dbContext.SaveChanges();

            return RedirectToAction("EditClientPhysicalProgress", new { id = viewModel.Client.Id });
        }

        public ActionResult SaveClientPhysicalProgress(ClientPhysicalProgressViewModel viewModel)
        {
            ModelState.Remove("TemporaryImageId");
            ModelState.Remove("SelectedPhysicalProgressId");
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            if (!ModelState.IsValid)
                return RedirectToAction("EditClientPhysicalProgress", new { id = viewModel.Client.Id });

            var clientInDb = _dbContext.Clients.SingleOrDefault(c => c.Id == viewModel.Client.Id);
            if (clientInDb == null)
                return HttpNotFound();

            var allClientPhysicalProgressInDb =
                _dbContext.ClientPhysicalProgresses.Where(cph => cph.ClientId == clientInDb.Id);

            var storedPhysicalProgressIds = allClientPhysicalProgressInDb.Select(c => c.PhysicalProgressId).ToList();
            var allPhysicalProgress = _dbContext.PhysicalProgresses.ToList();

            foreach (var physicalProgress in allPhysicalProgress)
            {
                var isDone = viewModel.EnablePhysicalProgressIds != null &&
                             viewModel.EnablePhysicalProgressIds.Contains(physicalProgress.Id);

                if (storedPhysicalProgressIds.Contains(physicalProgress.Id))
                {
                    // already stored in db
                    var clientPhysicalProgress =
                        allClientPhysicalProgressInDb.Single(c => c.PhysicalProgressId == physicalProgress.Id);
                    clientPhysicalProgress.IsDone = isDone;
                }
                else
                {
                    var newClientPhysicalProgress = new ClientPhysicalProgress
                    {
                        ClientId = clientInDb.Id,
                        IsDone = isDone,
                        PhysicalProgressId = physicalProgress.Id
                    };
                    _dbContext.ClientPhysicalProgresses.Add(newClientPhysicalProgress);
                }

                _dbContext.SaveChanges();
            }

            return RedirectToAction("EditClientPhysicalProgress",
                new { id = clientInDb.Id, message = MessageClientManageId.DataSavedSuccessfully });
        }

        public ActionResult GetClientPhysicalProgressPhoto(int id)
        {
            var clientPhysicalProgressPhotoInDb =
                _dbContext.ClientPhysicalProgressPhotos.SingleOrDefault(cph => cph.Id == id);
            if (clientPhysicalProgressPhotoInDb == null)
                return null;

            var clientDocumentInDb = _dbContext.ClientDocuments.SingleOrDefault(cd => cd.Id == id);
            return clientDocumentInDb == null ? null : File(clientDocumentInDb.DocURI, "image/jpeg");
        }

        /// <summary>
        /// تصویر بارگذاری شده فرم معرفی نامه معافیت انشعابات
        /// </summary>
        /// <param name="id">ClientRequestGetLetterId</param>
        /// <returns></returns>
        public ActionResult GetClientRequestGetLetterPhoto(int id)
        {
            var getLetterInDb =
                _dbContext.ClientRequestGetLetters.SingleOrDefault(gl => gl.Id == id);
            if (getLetterInDb == null)
                return null;

            var clientDocumentInDb =
                _dbContext.ClientDocuments.SingleOrDefault(cd => cd.Id == getLetterInDb.LetterPhotoId);
            return clientDocumentInDb == null ? null : File(clientDocumentInDb.DocURI, "image/jpeg");
        }

        /// <summary>
        /// تصویر بارگذاری شده بهره مندی از معافیت انشعابات
        /// </summary>
        /// <param name="id">ClientRequestBenefitLetterId</param>
        /// <returns></returns>
        public ActionResult GetClientRequestBenefitLetterPhoto(int id)
        {
            var exemptionBenefitInDb =
                _dbContext.ClientExemptionBenefits.SingleOrDefault(eb => eb.Id == id);
            if (exemptionBenefitInDb == null)
                return null;

            var clientDocumentInDb =
                _dbContext.ClientDocuments.SingleOrDefault(cd => cd.Id == exemptionBenefitInDb.BenefitPhotoId);
            return clientDocumentInDb == null ? null : File(clientDocumentInDb.DocURI, "image/jpeg");
        }

        public ActionResult GetClientBankApproveImage(int id)
        {
            var bankInfoInDb = _dbContext.BankInfos
                .Include(b => b.AccountApproveImage)
                .SingleOrDefault(b => b.Id == id);

            return bankInfoInDb?.AccountApproveImage == null
                ? null
                : File(bankInfoInDb.AccountApproveImage.DocURI, "image/jpeg");
        }

        // مدارک مددجو
        public ActionResult EditDocument(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var viewModel = new ClientDocumentViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                ClientId = id,
                AllDocumentTypes = _dbContext.DocumentTypes.Where(d => d.IsBasic).ToList(),
                ClientDocuments = client.ClientDocuments.Where(cd => cd.DocumentType.IsBasic),
                HasSelectRentalForCurrentHouse = client.CurrentHousing != null &&
                                                 client.CurrentHousing.CurrentHouseTypeId ==
                                                 (int)ModelEnums.CurrentHouseTypeE.Ejarei,
                HasSelectLiveInBenefactorHouse = client.CurrentHousing != null &&
                                                 client.CurrentHousing.CurrentHouseTypeId ==
                                                 (int)ModelEnums.CurrentHouseTypeE.Khayerin,
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully
                        ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error
                            ? ModelEnums.StatusTypeE.Error
                            : ModelEnums.StatusTypeE.Warning
                };

            return View(viewModel);
        }


        private string GetOrCreateClientDocumentFolder(int provinceId, string nationalCode)
        {
            var newAddress = Server.MapPath(DefaultClientDocumentAddress);
            if (!Directory.Exists(newAddress))
                Directory.CreateDirectory(newAddress);

            newAddress = Path.Combine(newAddress, provinceId.ToString());
            if (!Directory.Exists(newAddress))
                Directory.CreateDirectory(newAddress);
            newAddress = Path.Combine(newAddress, nationalCode);
            if (!Directory.Exists(newAddress))
                Directory.CreateDirectory(newAddress);

            return newAddress;
        }


        [HttpPost]
        public ActionResult SaveClientDocument(ClientDocumentViewModel viewModel)
        {
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            if (!ModelState.IsValid)
                return RedirectToAction("EditDocument",
                    new { id = viewModel.ClientId, message = MessageClientManageId.ValidationError });

            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.City.District.County)
                .Include(c => c.ClientDocuments)
                .Include(c => c.ClientDocuments.Select(cd => cd.DocumentType))
                .SingleOrDefault(c => c.Id == viewModel.ClientId);

            if (clientInDb == null)
                return HttpNotFound();

            //var clientDocumentWithThisTypeInDb =
            //    clientInDb.ClientDocuments.SingleOrDefault(cd => cd.DocumentTypeId == viewModel.DocumentTypeId);


            var tempImage =
                _dbContext.TemporaryImages.SingleOrDefault(ti => ti.Id == viewModel.TempClientDocumentId);
            if (tempImage == null)
                return HttpNotFound();

            var newAddress = GetOrCreateClientDocumentFolder(clientInDb.City.District.County.ProvinceId,
                clientInDb.Person.NationalCode);


            var documentType = _dbContext.DocumentTypes.Single(d => d.Id == viewModel.DocumentTypeId);

            newAddress = Path.Combine(newAddress,
                $"{documentType.Name}{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

            if (System.IO.File.Exists(newAddress))
                System.IO.File.Delete(newAddress);

            System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

            _dbContext.TemporaryImages.Remove(tempImage);

            //if (clientDocumentWithThisTypeInDb != null)
            //{
            //    // قبلا برای این مدرک یک عکس بارگذاری شده است
            //    // باید اطلاعات قبلی از پایگاه داده حذف شود
            //    clientDocumentWithThisTypeInDb.DocURI = newAddress;
            //}
            //else
            //{
            //}

            var clientDoc = new ClientDocument
            {
                ClientId = clientInDb.Id,
                DocumentTypeId = viewModel.DocumentTypeId,
                IsOptimized = false,
                DocURI = newAddress
            };
            _dbContext.ClientDocuments.Add(clientDoc);

            _dbContext.SaveChanges();

            return RedirectToAction("EditDocument", new { id = clientInDb.Id });
        }

        private FileContentResult ResizeImageAndReturnAsFile(string fileAddress)
        {
            var img = new Bitmap(fileAddress);
            img = img.ResizeImage(150, 150);
            var ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);

            return File(ms.ToArray(), "image/jpeg");
        }

        [Route("Client/GetClientDocumentImageThumbnail/{id}/{documentTypeId}")]
        public ActionResult GetClientDocumentImageThumbnail(int id, int documentTypeId)
        {
            var clientDocumentInDb =
                _dbContext.ClientDocuments.SingleOrDefault(cd =>
                    cd.ClientId == id && cd.DocumentTypeId == documentTypeId);
            return clientDocumentInDb == null ? null : ResizeImageAndReturnAsFile(clientDocumentInDb.DocURI);
        }

        [Route("Client/GetClientDocumentImage/{id}/{documentTypeId}")]
        public ActionResult GetClientDocumentImage(int id, int documentTypeId)
        {
            var clientDocumentInDb =
                _dbContext.ClientDocuments.SingleOrDefault(cd =>
                    cd.ClientId == id && cd.DocumentTypeId == documentTypeId);
            return clientDocumentInDb == null ? null : File(clientDocumentInDb.DocURI, "image/jpeg");
        }


        // کمک های مالی
        public ActionResult EditClientFinancialAid(int id)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var allClientForms = _dbContext.ClientForms
                .Include(cf => cf.Form)
                .Include(cf => cf.Form.FormPhysicalProgresses)
                .Include(cf => cf.Form.FormPhysicalProgresses.Select(fp => fp.FormPhysicalProgressHelps))
                .Where(cf => cf.ClientId == id);

            var allClientPhysicalProgressInDb =
                _dbContext.ClientPhysicalProgresses.Where(cph => cph.ClientId == id).ToList();


            var lastDoneClientPhysicalProgress = 0;
            if (allClientPhysicalProgressInDb.Count > 0)
            {
                var doneClientPhysicalProgressInDb = allClientPhysicalProgressInDb.Where(fp => fp.IsDone).ToList();
                if (doneClientPhysicalProgressInDb.Count > 0)
                    lastDoneClientPhysicalProgress = doneClientPhysicalProgressInDb.Max(cph => cph.PhysicalProgressId);
            }

            long financialAidAmountSum = 0;
            if (client.FormEmtiazBandi != null)
                financialAidAmountSum = client.FormEmtiazBandi.Amount;
            else
                foreach (var clientForm in allClientForms)
                {
                    var allClientFormPhysicalProgress =
                        clientForm.Form.FormPhysicalProgresses.Where(fp =>
                            fp.PhysicalProgressId < lastDoneClientPhysicalProgress);

                    foreach (var formPhysicalProgress in allClientFormPhysicalProgress)
                    {
                        var coOrgFinancialHelpAmount =
                            formPhysicalProgress.FormPhysicalProgressHelps.Sum(formPhysicalProgressHelp =>
                                formPhysicalProgressHelp.HelpAmount);
                        financialAidAmountSum += formPhysicalProgress.BehzistiHelpAmount + coOrgFinancialHelpAmount;
                    }
                }


            var allFinancialAidInDb = _dbContext.FinancialAids.Where(f => f.ClientId == client.Id).ToList();
            var viewModel = new ClientFinancialAidViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                ClientId = id,
                IsRegisteredInAnyForm = _dbContext.ClientForms.Any(cf => cf.ClientId == id),
                FinancialAidAmountThatClientMustGet = financialAidAmountSum,
                AllCoOrganizationTypes = _dbContext.CoOrganizationTypes.ToList(),
                ClientFinancialAids = allFinancialAidInDb,
                FinancialAidSummation = allFinancialAidInDb.Sum(f => f.Amount)
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveClientFinancialAid(ClientFinancialAidViewModel viewModel)
        {
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            long amount;
            PersianDateTime pDate;
            var isCorrectDate = PersianDateTime.TryParse(viewModel.DepositDate, out pDate);
            var amountIsNumber = long.TryParse(viewModel.Amount.Replace(",", ""), out amount);
            if (!ModelState.IsValid || !amountIsNumber || !isCorrectDate) return RedirectToAction("EditClientFinancialAid", new { id = viewModel.ClientId });

            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.City.District.County)
                .SingleOrDefault(c => c.Id == viewModel.Client.Id);
            if (clientInDb == null)
                return HttpNotFound();

            var tempImage =
                _dbContext.TemporaryImages.SingleOrDefault(ti => ti.Id == viewModel.FinancialAid.ClientDocumentId);
            if (tempImage == null)
                return HttpNotFound();

            var newAddress = GetOrCreateClientDocumentFolder(clientInDb.City.District.County.ProvinceId,
                clientInDb.Person.NationalCode);

            var currentFiles =
                Directory.GetFiles(newAddress, "FinancialAid#Photo-*");

            var lastPhotoIndex = 0;

            foreach (var currentFile in currentFiles)
            {
                var fStrs = currentFile.Split('#');
                if (fStrs.Length < 2) continue;
                var photoIdStr = fStrs[1].Split('-');
                if (photoIdStr.Length < 2) continue;
                photoIdStr = photoIdStr[1].Split('.');
                if (photoIdStr.Length < 2) continue;
                int outInt;
                if (int.TryParse(photoIdStr[0], out outInt))
                    lastPhotoIndex = outInt > lastPhotoIndex ? outInt : lastPhotoIndex;
            }

            newAddress = Path.Combine(newAddress,
                $"FinancialAid#Photo-{lastPhotoIndex + 1}{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

            System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

            _dbContext.TemporaryImages.Remove(tempImage);

            var clientDoc = new ClientDocument
            {
                ClientId = clientInDb.Id,
                DocumentTypeId = (int)ModelEnums.DocumentTypesE.FinancialAidBankFish,
                IsOptimized = false,
                DocURI = newAddress
            };
            _dbContext.ClientDocuments.Add(clientDoc);
            _dbContext.SaveChanges();


            var financialAid = new FinancialAid
            {
                ClientId = clientInDb.Id,
                Amount = amount,
                ClientDocumentId = clientDoc.Id,
                CoOrganizationTypeId = viewModel.FinancialAid.CoOrganizationTypeId,
                DepositDate = pDate.ToDateTime(),
            };
            _dbContext.FinancialAids.Add(financialAid);
            _dbContext.SaveChanges();

            return RedirectToAction("EditClientFinancialAid", new { id = clientInDb.Id });
        }

        public ActionResult GetClientFinancialAidPhoto(int id)
        {
            var clientFinancialAidInDb =
                _dbContext.FinancialAids.SingleOrDefault(cph => cph.Id == id);
            if (clientFinancialAidInDb == null)
                return null;

            var clientDocumentInDb =
                _dbContext.ClientDocuments.Single(cd => cd.Id == clientFinancialAidInDb.ClientDocumentId);
            return File(clientDocumentInDb.DocURI, "image/jpeg");
        }

        //طرح ها

        //مدیریت طرح های مددجو - ثبت مددجو در طرح و یا حذف مددجو از طرح از طریق این صفحه می باشد
        public ActionResult EditClientForm(int id)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            if (!User.IsInRole(RoleName.KarshenasOstan) && !User.IsInRole(RoleName.KarshenasShahrestan))
                return RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });


            var registeredForms = _dbContext.ClientForms
                .Include(cf => cf.Form)
                .Where(cf => cf.ClientId == client.Id);

            var registeredFormIds = registeredForms.Select(cf => cf.FormId).ToList();


            var availableForms = _dbContext.Forms.Include(f => f.FormAccessLevels);
            // بررسی شرایط طرح
            // فقط طرح هایی که در شهرستان انتخاب شده برای مددجو می باشند
            availableForms = availableForms.Where(f =>
                f.FormAccessLevels.Any(fc => fc.CountyId == client.City.District.CountyId));
            // حذف طرح هایی که قبلا برای مددجو ثبت شده اند
            availableForms = availableForms.Where(f => !registeredFormIds.Contains(f.Id));

            var availableFormIds = availableForms.Select(f => f.Id).ToList();

            // طرح هایی که در دسترس نیستند و شرایط آنها با مددجو مطابقت ندارد
            var nonAvailableForms =
                _dbContext.Forms.Where(f => !availableFormIds.Contains(f.Id) && !registeredFormIds.Contains(f.Id));

            var viewModel = new EditClientFormViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                AvailableForms = availableForms.ToList(),
                ClientRegisteredForms = registeredForms.ToList(),
                NotAvailableForms = nonAvailableForms.ToList()
            };

            var selectedFormMenu = new Dictionary<int, string>();
            foreach (var clientForm in registeredForms) selectedFormMenu.Add(clientForm.Id, clientForm.Form.Name);

            ViewBag.ClientSelectedFormTabItems = selectedFormMenu;
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClientToForm(EditClientFormViewModel viewModel)
        {
            //modified old :D
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            if (!ModelState.IsValid) return View("EditClientForm", viewModel);

            var clientId = viewModel.Client.Id;
            var selectedFormId = viewModel.SelectedFormId;

            if (clientId == 0 || selectedFormId == 0) return RedirectToAction("EditClientForm", new { id = clientId });

            var isClientExist = _dbContext.Clients.Any(c => c.Id == clientId);
            var isFormExist = _dbContext.Forms.Any(f => f.Id == selectedFormId);

            if (isClientExist && isFormExist)
            {
                var clientForm = new ClientForm
                { ClientId = clientId, FormId = selectedFormId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                _dbContext.ClientForms.Add(clientForm);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("EditClientForm", new { id = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveClientFromForm(EditClientFormViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Client.ClientTypeId");
                ModelState.Remove("Client.AssistanceTypeId");
                if (!ModelState.IsValid) return View("EditClientForm", viewModel);

                var clientFormId = viewModel.SelectedFormId;

                var clientFormInDb = _dbContext.ClientForms
                    .Include(cf => cf.ClientFormFields)
                    .Single(cf => cf.Id == clientFormId);

                var clientId = viewModel.Client.Id;


                // در صورتی که اطلاعات و مدارکی برای این طرح ثبت شده بود این طرح فقط توسط کارشناس استان قابل حذف می باشد

                if (clientFormInDb.ClientFormFields == null || clientFormInDb.ClientFormFields.Count < 1)
                {
                    // یعنی تا به حال اطلاعاتی برای این مددجو ثبت نشده است
                    _dbContext.ClientForms.Remove(clientFormInDb);
                    _dbContext.SaveChanges();
                }
                else if (User.IsInRole(RoleName.KarshenasOstan))
                {
                    // در صورتی که برای این مددجو اطلاعات ذخیره شده بود و کاربر کارشناس استان بود
                    // باید ابتدا اطلاعات او از سیستم حذف گردد
                    var formFieldsInDb = _dbContext.ClientFormFields.Where(f => f.ClientFormId == clientFormId);
                    _dbContext.ClientFormFields.RemoveRange(formFieldsInDb);

                    _dbContext.SaveChanges();

                    _dbContext.ClientForms.Remove(clientFormInDb);
                    _dbContext.SaveChanges();
                }


                return RedirectToAction("EditClientForm", new { id = clientId });
            }
            catch (Exception ex)
            {

                return View("EditClientForm", viewModel);
            }
     
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SaveClientForm(EditClientFormViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var clientForms = _dbContext.ClientForms.Where(cf => cf.ClientId == viewModel.Client.Id).ToList();
        //        var clientFormIds = clientForms.Select(cf => cf.Id).ToList();
        //        viewModel.AvailableForms = _dbContext.Forms.Include(f => f.FormAccessLevels)
        //            .Where(f => !clientFormIds.Contains(f.Id)).ToList();
        //        viewModel.ClientRegisteredForms = clientForms;

        //        return View("EditFormInfo", viewModel);
        //    }

        //    // با توجه به اینکه گزینه حذف مددجو از طرح هنوز برنامه ریزی نشده است
        //    // فعلا این گزینه غیر فعال است و نمی توان مددجو را از طرح حذف کرد
        //    var allClientFormIdsInDb = _dbContext.ClientForms
        //        .Where(cf => cf.ClientId == viewModel.Client.Id).Select(cf => cf.FormId)
        //        .ToList();

        //    foreach (var clientFormId in viewModel.ClientNewFormIds)
        //    {
        //        if (allClientFormIdsInDb.Contains(clientFormId)) continue;
        //        var clientForm = new ClientForm
        //        {
        //            ClientId = viewModel.Client.Id,
        //            FormId = clientFormId,
        //            CreatedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now,
        //        };
        //        _dbContext.ClientForms.Add(clientForm);
        //    }

        //    _dbContext.SaveChanges();


        //    // دوباره همین صفحه را نمایش بده
        //    return RedirectToAction("EditClientForm", new { id = viewModel.Client.Id });
        //}


        public ActionResult EditClientFormData(int id, int clientFormId)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var clientFormInDb = _dbContext.ClientForms
                .Include(cf => cf.ClientFormFields)
                .SingleOrDefault(cf => cf.Id == clientFormId);
            if (clientFormInDb == null)
                return HttpNotFound();

            var formInDb = _dbContext.Forms
                .Include(f => f.Fields)
                .Include(f => f.Fields.Select(fi => fi.FieldTemplate))
                .Single(f => f.Id == clientFormInDb.FormId);

            var viewModel = new ClientEditFormDataViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                Form = formInDb,
                ClientForm = clientFormInDb,
                FieldTemplates = _dbContext.FieldTemplates.ToList()
            };

            var clientForms = _dbContext.ClientForms
                .Include(cf => cf.Form)
                .Where(cf => cf.ClientId == client.Id).ToList();
            var clientSelectedFormTabItems = new Dictionary<int, string>();
            foreach (var clientForm in clientForms) clientSelectedFormTabItems.Add(clientForm.Id, clientForm.Form.Name);

            ViewBag.ClientSelectedFormTabItems = clientSelectedFormTabItems;

            ViewBag.ClientSelectedFormId = clientFormId;

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveClientForm(ClientEditFormDataViewModel viewModel)
        {
            // if client is null or id is zero return BadRequest Or NotFount
            if (viewModel.Client?.Id == 0)
                return HttpNotFound();

            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.City.District.County)
                .SingleOrDefault(c => c.Id == viewModel.Client.Id);
            // if client is null
            if (clientInDb == null)
                return HttpNotFound();

            // if client form is null or id is zero return BadRequest Or NotFount
            // when we add client to form we create a client form so this item must be non null
            if (viewModel.ClientForm?.Id == 0)
                return HttpNotFound();

            var formInDb = _dbContext.Forms
                .Include(f => f.Fields)
                .Include(f => f.Fields.Select(fi => fi.FieldTemplate))
                .SingleOrDefault(f => f.Id == viewModel.Form.Id);
            // if form is null or id is zero return BadRequest Or NotFount
            if (formInDb == null)
                return HttpNotFound();

            // store all fields
            var allFormFieldsInDb = formInDb.Fields;

            var tmpClientId = viewModel.ClientForm?.Id ?? 0;
            var clientFormInDb = _dbContext.ClientForms
                .Include(cf => cf.ClientFormFields)
                .Include(cf => cf.Form)
                .Include(cf => cf.Form.Fields)
                .SingleOrDefault(cf => cf.Id == tmpClientId);
            if (clientFormInDb == null)
                return HttpNotFound();

            var allClientFormFieldsInDb = clientFormInDb.ClientFormFields;

            var jsonFormFieldData =
                JsonConvert.DeserializeObject<IEnumerable<JsonFormFieldData>>(viewModel.JsonFormFieldData);

            foreach (var fieldData in jsonFormFieldData)
            {
                var clientFormFieldInDb = allClientFormFieldsInDb.SingleOrDefault(f => f.FieldId == fieldData.FieldId);

                //get formField from allFormFields :-D - good comment 
                var formFieldInDb = allFormFieldsInDb.Single(f => f.Id == fieldData.FieldId);

                //First time we add data for this client form fields
                if (clientFormFieldInDb == null)
                {
                    var newClientFormField = new ClientFormField
                    {
                        ClientFormId = clientFormInDb.Id,
                        CreatedAt = DateTime.Now,
                        FieldId = fieldData.FieldId,
                        UpdatedAt = DateTime.Now
                    };
                    switch (formFieldInDb.FieldTemplate.Type)
                    {
                        case ModelEnums.FieldTemplateE.ImageTemplate:
                            {
                                // به دلیل اینکه انتهای کد یکتای تصویر موقت یک هشتگ اضافه کردیم
                                // و برای مواردی که تازه بارگذاری شده اند عبارت جدید را به انتهای آن اضافه کردیم
                                // در اینجا آن را حذف کرده و فقط کد یکتا را جدا میکنیم
                                var tempImageId = int.Parse(fieldData.FieldValue.Split('#')[0]);
                                var tempImage = _dbContext.TemporaryImages.SingleOrDefault(ti =>
                                    ti.ClientId == clientInDb.Id && ti.FieldId == fieldData.FieldId &&
                                    ti.Id == tempImageId);
                                if (tempImage == null) continue;

                                var newAddress = GetOrCreateClientDocumentFolder(clientInDb.City.District.County.ProvinceId,
                                    clientInDb.Person.NationalCode);

                                newAddress = Path.Combine(newAddress,
                                    $"FormId-{formInDb.Id}#FieldId-{formFieldInDb.Id}{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

                                //move file from temporary folder to client Documents
                                System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

                                var clientDoc = new ClientDocument
                                {
                                    ClientId = clientInDb.Id,
                                    DocumentTypeId = (int)ModelEnums.DocumentTypesE.FormFieldDocument,
                                    IsOptimized = false,
                                    DocURI = newAddress
                                };
                                _dbContext.ClientDocuments.Add(clientDoc);
                                _dbContext.SaveChanges();

                                // store clientDocument id as value of filed
                                // later when we want to show image 
                                // find this id in clientDocument table and get the image
                                newClientFormField.Value = clientDoc.Id.ToString();
                                break;
                            }

                        case ModelEnums.FieldTemplateE.DateTemplate:
                            {
                                DateTime dt;
                                var isDateCorrect = DateTime.TryParse(fieldData.FieldValue, out dt);
                                // if this is a correct date set to field data value
                                if (isDateCorrect)
                                    newClientFormField.Value = fieldData.FieldValue;

                                break;
                            }

                        case ModelEnums.FieldTemplateE.NumberTemplate:
                            {
                                int num;
                                var isNumCorrect = int.TryParse(fieldData.FieldValue, out num);
                                // if this is a correct num -> set for field data value
                                if (isNumCorrect)
                                    newClientFormField.Value = fieldData.FieldValue;
                                break;
                            }

                        default:
                            {
                                newClientFormField.Value = fieldData.FieldValue;
                                break;
                            }
                    }

                    _dbContext.ClientFormFields.Add(newClientFormField);
                }
                else // we already submit data for this client form field
                {
                    // if client form field is already exist
                    // we can simply change variable
                    // but in case of ImageTemplate
                    // we have some situation

                    switch (formFieldInDb.FieldTemplate.Type)
                    {
                        case ModelEnums.FieldTemplateE.DateTemplate:
                            {
                                if (clientFormFieldInDb.Value != fieldData.FieldValue)
                                {
                                    DateTime dt;
                                    var isDateCorrect = DateTime.TryParse(fieldData.FieldValue, out dt);
                                    // if this is a correct date set to field data value
                                    if (isDateCorrect)
                                    {
                                        clientFormFieldInDb.Value = fieldData.FieldValue;
                                        clientFormFieldInDb.UpdatedAt = DateTime.Now;
                                    }
                                }

                                break;
                            }


                        case ModelEnums.FieldTemplateE.NumberTemplate:
                            {
                                if (clientFormFieldInDb.Value != fieldData.FieldValue)
                                {
                                    int num;
                                    var isNumCorrect = int.TryParse(fieldData.FieldValue, out num);
                                    // if this is a correct num -> set for field data value
                                    if (isNumCorrect)
                                        clientFormFieldInDb.Value = fieldData.FieldValue;
                                }

                                break;
                            }

                        case ModelEnums.FieldTemplateE.ImageTemplate:
                            {
                                // برای ساده تر کردن مدیریت تصاویر و تشخیص عکس های جدیدی از عکس های قبلی ذخیره شده در سرور
                                // از یک هشتگ در انتهای آن استفاده کردم که در صورتی که کلمه جدید بود
                                // متوجه می شویم باید در پوشه عکس های موقت دنبال تصویر بگردیم
                                // در غیر اینصورت عکس می بایست در پوشه عکس های ثبت نهایی شده قرار گرفته باشد
                                var tmpString = fieldData.FieldValue.Split('#');
                                var newId = tmpString[0]; // Id of New (temp Image) Or Old (Client Document)
                                var status =
                                    tmpString.Length > 1
                                        ? tmpString[1]
                                        : ""; // New -> newly uploaded image on temporary Image table -- Old -> saved image in constant storage

                                if (status == "New")
                                {
                                    var clientDocId = int.Parse(clientFormFieldInDb.Value);
                                    // fetch clientDoc from db
                                    var clientDocInDb = _dbContext.ClientDocuments.SingleOrDefault(cd =>
                                        cd.Id == clientDocId);

                                    // if constant image is stored
                                    if (clientDocInDb != null)
                                    {
                                        var tempImageId = int.Parse(newId);
                                        var tempImage = _dbContext.TemporaryImages.SingleOrDefault(ti =>
                                            ti.ClientId == clientInDb.Id && ti.FieldId == fieldData.FieldId &&
                                            ti.Id == tempImageId);
                                        // اگر فایل موقت پیدا نشود یا از طریق هکر اقدام شده و یا اینکه زمان آن منقضی شده است
                                        // پس در هر دوصورت نباید کار را ادامه دهیم
                                        if (tempImage == null) continue;


                                        // فایل جدیدی آپلود شده است
                                        // باید فایل قبلی پاک شود
                                        System.IO.File.Delete(clientDocInDb.DocURI);

                                        // move new image from temporary file name to constant folder
                                        System.IO.File.Move(tempImage.TemporaryFileName, clientDocInDb.DocURI);

                                        _dbContext.TemporaryImages.Remove(tempImage);

                                        clientFormFieldInDb.UpdatedAt = DateTime.Now;
                                    } // end if clientDocInDb != null
                                    else
                                    {
                                        // به دلیل اینکه انتهای کد یکتای تصویر موقت یک هشتگ اضافه کردیم
                                        // و برای مواردی که تازه بارگذاری شده اند عبارت جدید را به انتهای آن اضافه کردیم
                                        // در اینجا آن را حذف کرده و فقط کد یکتا را جدا میکنیم
                                        var tempImageId = int.Parse(newId);
                                        var tempImage = _dbContext.TemporaryImages.SingleOrDefault(ti =>
                                            ti.ClientId == clientInDb.Id && ti.FieldId == fieldData.FieldId &&
                                            ti.Id == tempImageId);
                                        if (tempImage == null) continue;

                                        var newAddress = GetOrCreateClientDocumentFolder(
                                            clientInDb.City.District.County.ProvinceId, clientInDb.Person.NationalCode);

                                        newAddress = Path.Combine(newAddress,
                                            $"FormId-{formInDb.Id}#FieldId-{formFieldInDb.Id}{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");

                                        //move file from temporary folder to client Documents
                                        System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

                                        var clientDoc = new ClientDocument
                                        {
                                            ClientId = clientInDb.Id,
                                            DocumentTypeId = (int)ModelEnums.DocumentTypesE.FormFieldDocument,
                                            IsOptimized = false,
                                            DocURI = newAddress
                                        };
                                        _dbContext.ClientDocuments.Add(clientDoc);
                                    }
                                } // End if status == "New"
                                else if (status == "Old") // "Old"
                                {
                                    // در صورتی که کد تصویر جدید خالی باشد ولی قبلا تصویر بارگذاری شده باشد
                                    // باید تصویر قبلی را پاک کنیم
                                    if (clientFormFieldInDb.Value != newId)
                                    {
                                        var clientDocumentId = int.Parse(clientFormFieldInDb.Value);
                                        var clientDocumentInDb = _dbContext.ClientDocuments.Single(cd =>
                                            cd.Id == clientDocumentId);

                                        if (newId.IsNullOrWhiteSpace())
                                        {
                                            System.IO.File.Delete(clientDocumentInDb.DocURI);
                                            _dbContext.ClientDocuments.Remove(clientDocumentInDb);

                                            _dbContext.ClientFormFields.Remove(clientFormFieldInDb);
                                        }
                                    }
                                } // End if status == "Old"
                                else //status == "" or remove field data
                                {
                                    var clientDocId = int.Parse(clientFormFieldInDb.Value);
                                    var clientDocumentInDb =
                                        _dbContext.ClientDocuments.Single(cd => cd.Id == clientDocId);

                                    if (System.IO.File.Exists(clientDocumentInDb.DocURI))
                                        System.IO.File.Delete(clientDocumentInDb.DocURI);
                                    _dbContext.ClientDocuments.Remove(clientDocumentInDb);

                                    _dbContext.ClientFormFields.Remove(clientFormFieldInDb);
                                }

                                break;
                            }

                        default:
                            {
                                if (clientFormFieldInDb.Value != fieldData.FieldValue)
                                    clientFormFieldInDb.Value = fieldData.FieldValue;
                                break;
                            }
                    }
                }

                _dbContext.SaveChanges();
            }

            return RedirectToAction("EditClientFormData", new { id = clientInDb.Id, clientFormId = clientFormInDb.Id });
        }


        public ActionResult GetFormFieldsImage(int id)
        {
            var clientDocumentInDb = _dbContext.ClientDocuments.SingleOrDefault(cd => cd.Id == id);
            return clientDocumentInDb == null ? null : File(clientDocumentInDb.DocURI, "image/jpeg");
        }


        //public ActionResult FormSubmitData(int id)
        //{
        //    Client client;
        //    ActionResult result;
        //    if (!CheckPrivilegeForEditClient(id, out client, out result))
        //        return result;

        //    var clientForms = _dbContext.ClientForms
        //        .Include(cf => cf.Form)
        //        .Where(cf => cf.ClientId == client.Id).ToList();
        //    var clientFormIds = clientForms.Select(cf => cf.FormId).ToList();

        //    return View();
        //}


        // پایان طرح ها


        public ActionResult DeletePerson(int id)
        {
            if (!User.IsInRole(RoleName.KarshenasOstan))
                return RedirectToAction("Index", new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var clientInDb = _dbContext.Clients.SingleOrDefault(c => c.Id == id);
            if (clientInDb != null)
                return RedirectToAction("Delete", new { id });

            var personInDb = _dbContext.Persons
                .Include(p => p.CityOfBirth.District.County)
                .Include(p => p.CityOfBirth.District)
                .Include(p => p.CityOfBirth)
                .SingleOrDefault(p => p.Id == id);
            if (personInDb == null) return HttpNotFound();

            return View(_mapper.Map<PersonDto>(personInDb));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePersonPost(int id)
        {
            if (!User.IsInRole(RoleName.KarshenasOstan))
                return RedirectToAction("IncompleteRegisterList",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var personInDb = _dbContext.Persons.Single(p => p.Id == id);

            _dbContext.Persons.Remove(personInDb);

            _dbContext.SaveChanges();
            return RedirectToAction("IncompleteRegisterList", new { Message = MessageClientManageId.PersonWasDeleted });
        }

        public ActionResult Delete(int id)
        {
            if (!User.IsInRole(RoleName.KarshenasOstan))
                return RedirectToAction("Index", new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var clientInDb = _dbContext.Clients
                .Include(c => c.City.District.County.Province)
                .SingleOrDefault(c => c.Id == id);
            if (clientInDb == null)
                return HttpNotFound();

            var userId = User.Identity.GetUserId();

            var userInfoInDb = _dbContext.UserInfos.Single(u => u.Id == userId);

            if (clientInDb.City.District.County.ProvinceId != userInfoInDb.ProvinceId)
                return RedirectToAction("Index", new { Message = MessageClientManageId.YouCannotAccessThisArea });

            // بعدا باید کد مناسب حذف کردن مددجو نوشته شود
            clientInDb.IsDeleted = true;
            _dbContext.SaveChanges();

            return RedirectToAction("Index", new { Message = MessageClientManageId.DataSavedSuccessfully });
            //return View();
        }

        public ActionResult SetClientUserActivationCode(int id)
        {
            if (!User.IsInRole(RoleName.KarshenasOstan) &&
                !User.IsInRole(RoleName.KarshenasShahrestan))
                return RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });


            var clientInDb = _dbContext.Clients
                .Include(c => c.Person)
                .SingleOrDefault(c => c.Id == id);
            if (clientInDb == null)
                return HttpNotFound();

            var clientUserInDb =
                _dbContext.ClientUsers.SingleOrDefault(c => c.NationalCode == clientInDb.Person.NationalCode);
            if (clientUserInDb == null)
            {
                clientUserInDb = new ClientUser
                {
                    NationalCode = clientInDb.Person.NationalCode,
                    ActivationCode = ActivationServer.GenerateActivationCode(),
                    IsApproved = true
                };
                _dbContext.ClientUsers.Add(clientUserInDb);
            }
            else
            {
                clientUserInDb.ActivationCode = ActivationServer.GenerateActivationCode();
            }

            _dbContext.SaveChanges();

            var viewModel = new ClientActivationCodeViewModel
            {
                ClientId = clientInDb.Id,
                Name = clientInDb.Person.Name,
                Family = clientInDb.Person.Family,
                NationalCode = clientUserInDb.NationalCode,
                ActivationCode = clientUserInDb.ActivationCode
            };

            return View(viewModel);
        }

        private void ClearExpiredTemporaryImages()
        {
            var expiredImages = _dbContext.TemporaryImages.Where(ti => ti.ExpireAt < DateTime.Now);
            foreach (var temporaryImage in expiredImages)
                if (System.IO.File.Exists(temporaryImage.TemporaryFileName))
                    System.IO.File.Delete(temporaryImage.TemporaryFileName);

            _dbContext.TemporaryImages.RemoveRange(expiredImages);
            _dbContext.SaveChanges();
        }

        private void ClearExpiredDownloadRequests()
        {
            var expiredDownRequests = _dbContext.DownloadRequests.Where(d => d.RequestExpireTime < DateTime.Now);
            _dbContext.DownloadRequests.RemoveRange(expiredDownRequests);

            _dbContext.SaveChanges();
        }

        [HttpPost]
        public string UploadClientFieldImage(TempImageDto tempImageDto)
        {
            ClearExpiredTemporaryImages();

            //if file is not empty or null
            if (tempImageDto.image != null && tempImageDto.image.ContentLength > 0)
            {
                //check image size
                if (tempImageDto.image.ContentLength > 1150000) return "Error#FileIsBig";

                var client = _dbContext.Clients.SingleOrDefault(c => c.Id == tempImageDto.clientId);
                if (client == null) return "Error#InvalidClient";

                var formField = _dbContext.Fields.SingleOrDefault(f => f.Id == tempImageDto.fieldId);
                if (formField == null) return "Error#InvalidField";

                var tempImageInDb = _dbContext.TemporaryImages.SingleOrDefault(t => t.Id == tempImageDto.fieldId);
                //اگر قبلا برای این فیلد اطلاعاتی در پایگاه داده موجود بود باید ابتدا آن مورد حذف شود
                // if we have existing image uploaded for this field
                if (tempImageInDb != null)
                {
                    //remove existing image
                    if (System.IO.File.Exists(tempImageInDb.TemporaryFileName))
                        System.IO.File.Delete(tempImageInDb.TemporaryFileName);

                    //remove temp image entry from database
                    _dbContext.TemporaryImages.Remove(tempImageInDb);
                    _dbContext.SaveChanges();
                }

                var initialPath = Server.MapPath(DefaultTempUploadFolder);
                if (!Directory.Exists(initialPath)) Directory.CreateDirectory(initialPath);

                var masterFileName = tempImageDto.image.FileName;

                var tempFileName =
                    $"{DateTime.Now:yyyy-M-d_hh-mm-ss}_{tempImageDto.clientId}_{tempImageDto.fieldId}{Path.GetExtension(tempImageDto.image.FileName)}";

                var tempFileNameFullPath = Path.Combine(initialPath, tempFileName);

                try
                {
                    tempImageDto.image.SaveAs(tempFileNameFullPath);
                }
                catch (Exception e)
                {
                    return "Error#" + e.Message;
                }

                var tempImage = new TemporaryImage
                {
                    CreatedAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddHours(1),
                    ClientId = client.Id,
                    Field = formField,
                    MasterFileName = masterFileName,
                    TemporaryFileName = tempFileNameFullPath
                };

                _dbContext.TemporaryImages.Add(tempImage);

                _dbContext.SaveChanges();

                return $"Created#{tempImage.Id}";
            }

            return "Error#InvalidFile";
        }

        [HttpPost]
        public string UploadClientDocumentImage(TempImageDto tempImageDto)
        {
            ClearExpiredTemporaryImages();

            //if file is not empty or null
            if (tempImageDto.image != null && tempImageDto.image.ContentLength > 0)
            {
                //check image size
                if (tempImageDto.image.ContentLength > 1150000) return "Error#FileIsBig";

                var client = _dbContext.Clients.SingleOrDefault(c => c.Id == tempImageDto.clientId);
                if (client == null) return "Error#InvalidClient";

                var initialPath = Server.MapPath(DefaultTempUploadFolder);
                if (!Directory.Exists(initialPath)) Directory.CreateDirectory(initialPath);

                var masterFileName = tempImageDto.image.FileName;

                var tempFileName =
                    $"{DateTime.Now:yyyy-M-d_hh-mm-ss}_{tempImageDto.clientId}_{tempImageDto.fieldId}{Path.GetExtension(tempImageDto.image.FileName)}";

                var tempFileNameFullPath = Path.Combine(initialPath, tempFileName);

                try
                {
                    tempImageDto.image.SaveAs(tempFileNameFullPath);
                }
                catch (Exception e)
                {
                    return "Error#" + e.Message;
                }

                var tempImage = new TemporaryImage
                {
                    CreatedAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddHours(1),
                    ClientId = client.Id,
                    MasterFileName = masterFileName,
                    TemporaryFileName = tempFileNameFullPath
                };

                _dbContext.TemporaryImages.Add(tempImage);

                _dbContext.SaveChanges();

                return $"Created#{tempImage.Id}";
            }

            return "Error#InvalidFile";
        }


        public ActionResult EditClientState(int id)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var hasCompleteData = client.CurrentHousing != null &&
                                  client.ContactInfo != null &&
                                  client.BankInfo != null &&
                                  client.ClientForms != null && client.ClientForms.Count > 0;

            var needCoOrganizationApprove = _dbContext.ClientForms.Where(cf => cf.ClientId == id)
                .Any(cf => cf.Form.FormCoOrganizationRoles.Count > 0);

            //var isNeedSazmanHamkarApprove = true;
            //_dbContext.Clients.Single(c=>c.Id == client.Id).ClientForms.Any(cf => cf.Form.FormCoOrganizationRoles.Any());

            var viewModel = new EditClientStateViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                ClientHasCompleteData = hasCompleteData,
                NeedCoOrganizationApprove = needCoOrganizationApprove,
                ClientStateLogs = _dbContext.ClientStateLogs.Where(cl => cl.ClientId == client.Id).AsEnumerable()
                    .Select(_mapper.Map<ClientStateLogDto>),
                //IsNeedSazmanHamkarApprove = isNeedSazmanHamkarApprove,
            };
            PopulateMenuTabItemOfRegisteredClientForm(client.Id);


            // در صورتی که کاربر شهرستان بود این پرچم برای او زده شود
            // به خاطر کاربر سازمان همکار این پرچم رو قرار دادم
            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos.Single(u => u.Id == userId);
            var isShahrestanUser = userInfoInDb.CountyId != null;
            viewModel.IsShahrestanUser = isShahrestanUser;


            if (User.IsInRole(RoleName.KarshenasShahrestan))
                return View("EditClientStateKarshenasShahrestan", viewModel);
            else if (User.IsInRole(RoleName.SazmanHamkar))
                return View("EditClientStateSazmanHamkar", viewModel);
            else if (User.IsInRole(RoleName.KarshenasOstan))
                return View("EditClientStateKarshenasOstan", viewModel);
            else return View("ReadOnlyClientState", viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveClientState(EditClientStateViewModel viewModel)
        {
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            ModelState.Remove("TempImageIdFormEmtiazBandi");

            if (!ModelState.IsValid) return RedirectToAction("EditClientState", new { id = viewModel.Client.Id });

            var clientInDb = _dbContext.Clients.Single(c => c.Id == viewModel.Client.Id);
            var clientStateInDb = _dbContext.ClientStates.SingleOrDefault(cs => cs.Id == clientInDb.Id);

            if (clientStateInDb == null)
            {
                clientStateInDb = new ClientState
                {
                    Client = clientInDb,
                    CurrentStateDate = DateTime.Now,
                    ClientStateTypeE = viewModel.NewClientStateTypeE,
                    PrevStateDate = DateTime.Now
                };
                _dbContext.ClientStates.Add(clientStateInDb);
            }
            else
            {
                clientStateInDb.PrevStateDate = clientStateInDb.CurrentStateDate;
                clientStateInDb.CurrentStateDate = DateTime.Now;
                clientStateInDb.ClientStateTypeE = viewModel.NewClientStateTypeE;
            }

            _dbContext.SaveChanges();

            // این بخش از کد از نظر امنیتی مشکل داره
            // یک نفر میتونه وارد سیستم بشه با نام کاربری و رمز عبور 
            // و کد جاوا اسکریپت رو تغییر بده و حالت مددجو رو به یه حالت دیگه برگردونه
            // بعدا باید همه تنظیماتی که توی بخش نمایش قرار دادم برای تغییر وضعیت مددجو
            // اینجا هم نوشته بشه

            var clientStateLog = new ClientStateLog
            {
                ClientId = clientInDb.Id,
                ClientStateTypeE = viewModel.NewClientStateTypeE,
                LogDate = DateTime.Now,
                Description = viewModel.Description
            };
            _dbContext.ClientStateLogs.Add(clientStateLog);
            _dbContext.SaveChanges();

            return RedirectToAction("EditClientState", new { id = viewModel.Client.Id });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ApproveBySazmanHamkar(EditClientStateViewModel viewModel)
        {
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            ModelState.Remove("TempImageIdFormEmtiazBandi");
            if (!ModelState.IsValid) return RedirectToAction("EditClientState", new { id = viewModel.Client.Id });

            var clientInDb = _dbContext.Clients.Single(c => c.Id == viewModel.Client.Id);
            var clientStateInDb = _dbContext.ClientStates.SingleOrDefault(cs => cs.Id == clientInDb.Id);

            // اگر توسط کاربر سازمان همکار تایید شده بود
            if (viewModel.NewClientStateTypeE == ModelEnums.ClientStateTypeE.ApproveBySazmanHamkarShahrestan ||
                viewModel.NewClientStateTypeE == ModelEnums.ClientStateTypeE.ApproveBySazmanHamkarOstan)
            {
                var userId = User.Identity.GetUserId();
                var userInfoInDb = _dbContext.UserInfos.Single(u => u.Id == userId);

                // در لیست تاییدیه های این سازمان همکار ذخیره شود
                var coOrgApprove = new CoOrganizationApproveList
                {
                    ClientId = viewModel.Client.Id,
                    ApproveDateTime = DateTime.Now,
                    CoOrganizationTypeId = userInfoInDb.CoOrganizationTypeId ?? 0,
                    Description = viewModel.Description ?? ""
                };
                _dbContext.CoOrganizationApproveLists.Add(coOrgApprove);
                _dbContext.SaveChanges();
            }

            if (clientStateInDb == null)
            {
                clientStateInDb = new ClientState
                {
                    Client = clientInDb,
                    CurrentStateDate = DateTime.Now,
                    ClientStateTypeE = viewModel.NewClientStateTypeE,
                    PrevStateDate = DateTime.Now
                };
                _dbContext.ClientStates.Add(clientStateInDb);
            }
            else
            {
                clientStateInDb.PrevStateDate = clientStateInDb.CurrentStateDate;
                clientStateInDb.CurrentStateDate = DateTime.Now;
                clientStateInDb.ClientStateTypeE = viewModel.NewClientStateTypeE;
            }

            _dbContext.SaveChanges();

            // این بخش از کد از نظر امنیتی مشکل داره
            // یک نفر میتونه وارد سیستم بشه با نام کاربری و رمز عبور 
            // و کد جاوا اسکریپت رو تغییر بده و حالت مددجو رو به یه حالت دیگه برگردونه
            // بعدا باید همه تنظیماتی که توی بخش نمایش قرار دادم برای تغییر وضعیت مددجو
            // اینجا هم نوشته بشه

            var clientStateLog = new ClientStateLog
            {
                ClientId = clientInDb.Id,
                ClientStateTypeE = viewModel.NewClientStateTypeE,
                LogDate = DateTime.Now,
                Description = viewModel.Description
            };
            _dbContext.ClientStateLogs.Add(clientStateLog);
            _dbContext.SaveChanges();

            return RedirectToAction("EditClientState", new { id = viewModel.Client.Id });
        }

        public ActionResult EditFormEmtiazBandi(int id)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            if (!User.IsInRole(RoleName.KarshenasOstan) &&
                !User.IsInRole(RoleName.KarshenasShahrestan) &&
                !User.IsInRole(RoleName.ModirKolOstan) &&
                !User.IsInRole(RoleName.MoavenMosharekat) &&
                !User.IsInRole(RoleName.ModirShahrestan) &&
                !User.IsInRole(RoleName.MoavenOstan))
                return RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var viewModel = new EditClientRateFormViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                FormEmtiazBandi = _mapper.Map<FormEmtiazBandiDto>(client.FormEmtiazBandi),
            };
            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (User.IsInRole(RoleName.ModirKolOstan))
                viewModel.IsApprovedByThisUser = client.FormEmtiazBandi.IsApproveByModirKol;
            else if (User.IsInRole(RoleName.ModirShahrestan))
                viewModel.IsApprovedByThisUser = client.FormEmtiazBandi.IsApproveByModirShahrestan;
            else if (User.IsInRole(RoleName.MoavenOstan))
                viewModel.IsApprovedByThisUser = client.FormEmtiazBandi.IsApproveByRelatedAssistance;
            else if (User.IsInRole(RoleName.MoavenMosharekat))
                viewModel.IsApprovedByThisUser = client.FormEmtiazBandi.IsApproveByMoavenMosharekatAssistance;

            if (User.IsInRole(RoleName.KarshenasShahrestan))
                return View("EditFormEmtiazBandiKarshenas", viewModel);
            if (User.IsInRole(RoleName.KarshenasOstan))
                return View("EditFormEmtiazBandiKarshenas", viewModel);
            //if (User.IsInRole(RoleName.ModirKolOstan))
            return View("EditFormEmtiazBandiModir", viewModel);
        }

        public ActionResult ShowFormEmtiazBandiOnline(int id)
        {
            var client = _dbContext.Clients
                .Include(c => c.FormEmtiazBandi)
                .Include(c => c.FormEmtiazBandi.OfflineFormEmtiazBandi)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.City.District.County)
                .Single(c => c.Id == id);


            var formEmtiazBandiOnlineInDb = client.FormEmtiazBandi;

            if (formEmtiazBandiOnlineInDb?.OfflineFormEmtiazBandi == null)
                return null;

            var currentCountyId = client.City.District.CountyId;
            var currentProvinceId = client.City.District.County.ProvinceId;
            //باید ابتدا سایز تصویر را چک کنیم که حتما A4 باشد


            //load images
            using(var formImage = new Bitmap(client.FormEmtiazBandi.OfflineFormEmtiazBandi.DocURI))
            {
                formImage.SetResolution(72, 72);


                var a4Width = 842;
                var a4Height = 596;
                var whiteBrush = new SolidBrush(Color.White);

                var signMaxWidthHeight = 142;

                var bmp = new Bitmap(a4Width, a4Height);
                var graphics = Graphics.FromImage(bmp);

                // به دست آوردن کوچکترین مقیاس
                float scale = Math.Min(a4Width / formImage.Width, a4Height / formImage.Height);
                var scaleWidth = (int)(formImage.Width * scale);
                var scaleHeight = (int)(formImage.Height * scale);

                graphics.FillRectangle(whiteBrush, new RectangleF(0, 0, a4Width, a4Height));
                graphics.DrawImage(formImage, (a4Width - scaleWidth) / 2, (a4Height - scaleHeight) / 2, scaleWidth,
                    scaleHeight);

                graphics.CompositingMode = CompositingMode.SourceOver;


                // اگر مدیر شهرستان امضا کرده بود باید در سیستم به دنبال کاربر مدیر این شهرستان بگردیم و اگر امضا داشت آن را قرار دهیم
                if (formEmtiazBandiOnlineInDb.IsApproveByModirShahrestan)
                {
                    var modirShahrestanRoleId = _dbContext.Roles.Single(r => r.Name == RoleName.ModirShahrestan).Id;
                    var modirshahrestanUserInfo = _dbContext.UserInfos.SingleOrDefault(ui =>
                        ui.User.Roles.Any(r => r.RoleId == modirShahrestanRoleId) &&
                        ui.ProvinceId == currentProvinceId &&
                        ui.CountyId == currentCountyId);

                    if (modirshahrestanUserInfo != null && !string.IsNullOrEmpty(modirshahrestanUserInfo.SignFullAddress))
                    {
                        var signImage = new Bitmap(modirshahrestanUserInfo.SignFullAddress);
                        signImage.SetResolution(72, 72);

                        //تغییر مقیاس امضا
                        scale = Math.Min(signMaxWidthHeight / signImage.Width, signMaxWidthHeight / signImage.Height);
                        var signResize = new Bitmap(signMaxWidthHeight, signMaxWidthHeight);
                        var signGraph = Graphics.FromImage(signResize);
                        var signScaleW = (int)(signImage.Width * scale);
                        var signScaleH = (int)(signImage.Height * scale);

                        signGraph.DrawImage(signImage, (signMaxWidthHeight - signScaleW) / 2,
                            (signMaxWidthHeight - signScaleH) / 2, signScaleW, signScaleH);

                        // این عدد ها از فتوشاپ در اومده
                        graphics.DrawImage(signResize, 513, 421);
                    }
                }

                // اگر معاون مشارکت ها امضا کرده بود باید در سیستم به دنبال کاربر معاون مشارکت ها بگردیم و اگر امضا داشت آن را قرار دهیم
                if (formEmtiazBandiOnlineInDb.IsApproveByMoavenMosharekatAssistance)
                {
                    var moavenMosharekatRoleId = _dbContext.Roles.Single(r => r.Name == RoleName.MoavenMosharekat).Id;
                    var moavenMosharekatUserInfo = _dbContext.UserInfos.SingleOrDefault(ui =>
                        ui.User.Roles.Any(r => r.RoleId == moavenMosharekatRoleId) &&
                        ui.ProvinceId == currentProvinceId);

                    if (moavenMosharekatUserInfo != null && !string.IsNullOrEmpty(moavenMosharekatUserInfo.SignFullAddress))
                    {
                        var signImage = new Bitmap(moavenMosharekatUserInfo.SignFullAddress);
                        signImage.SetResolution(72, 72);

                        //تغییر مقیاس امضا
                        scale = Math.Min(signMaxWidthHeight / signImage.Width, signMaxWidthHeight / signImage.Height);
                        var signResize = new Bitmap(signMaxWidthHeight, signMaxWidthHeight);
                        var signGraph = Graphics.FromImage(signResize);
                        var signScaleW = (int)(signImage.Width * scale);
                        var signScaleH = (int)(signImage.Height * scale);

                        signGraph.DrawImage(signResize, (signMaxWidthHeight - signScaleW) / 2,
                            (signMaxWidthHeight - signScaleH) / 2, signScaleW, signScaleH);

                        // این عدد ها از فتوشاپ در اومده
                        graphics.DrawImage(signImage, 352, 421);
                    }
                }

                // اگر معاون تخصصی امضا کرده بود باید در سیستم به دنبال کاربر معاون تخصصی بگردیم و اگر امضا داشت آن را قرار دهیم
                if (formEmtiazBandiOnlineInDb.IsApproveByRelatedAssistance)
                {
                    var moavenTakhasosiRoleId = _dbContext.Roles.Single(r => r.Name == RoleName.MoavenOstan).Id;
                    var moavenTakhasosiUserInfo = _dbContext.UserInfos.SingleOrDefault(ui =>
                        ui.User.Roles.Any(r => r.RoleId == moavenTakhasosiRoleId) &&
                        ui.ProvinceId == currentProvinceId &&
                        ui.AssistanceTypeId == client.AssistanceTypeId);

                    if (moavenTakhasosiUserInfo != null && !string.IsNullOrEmpty(moavenTakhasosiUserInfo.SignFullAddress))
                    {
                        var signImage = new Bitmap(moavenTakhasosiUserInfo.SignFullAddress);
                        signImage.SetResolution(72, 72);

                        //تغییر مقیاس امضا
                        scale = Math.Min(signMaxWidthHeight / signImage.Width, signMaxWidthHeight / signImage.Height);
                        var signResize = new Bitmap(signMaxWidthHeight, signMaxWidthHeight);
                        var signGraph = Graphics.FromImage(signResize);
                        var signScaleW = (int)(signImage.Width * scale);
                        var signScaleH = (int)(signImage.Height * scale);

                        signGraph.DrawImage(signResize, (signMaxWidthHeight - signScaleW) / 2,
                            (signMaxWidthHeight - signScaleH) / 2, signScaleW, signScaleH);

                        // این عدد ها از فتوشاپ در اومده
                        graphics.DrawImage(signResize, 190, 421);
                    }
                }


                // اگر مدیر کل امضا کرده بود باید در سیستم به دنبال کاربر مدیر کل بگردیم و اگر امضا داشت آن را قرار دهیم
                if (formEmtiazBandiOnlineInDb.IsApproveByModirKol)
                {
                    var modirKolRoleId = _dbContext.Roles.Single(r => r.Name == RoleName.ModirKolOstan).Id;
                    var modirKolUserInfo = _dbContext.UserInfos.SingleOrDefault(ui =>
                        ui.User.Roles.Any(r => r.RoleId == modirKolRoleId) &&
                        ui.ProvinceId == currentProvinceId);

                    if (modirKolUserInfo != null && !string.IsNullOrEmpty(modirKolUserInfo.SignFullAddress))
                    {
                        var signImage = new Bitmap(modirKolUserInfo.SignFullAddress);
                        signImage.SetResolution(72, 72);


                        //تغییر مقیاس امضا
                        scale = Math.Min(signMaxWidthHeight / signImage.Width, signMaxWidthHeight / signImage.Height);
                        var signResize = new Bitmap(signMaxWidthHeight, signMaxWidthHeight);
                        var signGraph = Graphics.FromImage(signResize);
                        var signScaleW = (int)(signImage.Width * scale);
                        var signScaleH = (int)(signImage.Height * scale);

                        signGraph.DrawImage(signResize, (signMaxWidthHeight - signScaleW) / 2,
                            (signMaxWidthHeight - signScaleH) / 2, signScaleW, signScaleH);

                        // این عدد ها از فتوشاپ در اومده
                        graphics.DrawImage(signResize, 32, 421);
                    }
                }


                var memStr = new MemoryStream();
                formImage.Save(memStr, ImageFormat.Png);

                return formEmtiazBandiOnlineInDb?.OfflineFormEmtiazBandi == null
                    ? null
                    : File(memStr.ToArray(), "image/png");
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.ModirKolOstan + "," + RoleName.MoavenMosharekat + "," + RoleName.MoavenOstan + "," +
                           RoleName.ModirShahrestan)]
        public ActionResult ApproveOrDenyFormEmtiazBandi(EditClientRateFormViewModel viewModel)
        {
            //var userId = User.Identity.GetUserId();
            //var userInfoInDb = _dbContext.UserInfos.Single(ui => ui.Id == userId);

            var clientInDb = _dbContext.Clients
                .Include(c => c.ClientState)
                .Include(c => c.FormEmtiazBandi)
                .Single(c => c.Id == viewModel.Client.Id);

            if (clientInDb.FormEmtiazBandi == null)
                return null;


            if (User.IsInRole(RoleName.ModirKolOstan))
            {
                clientInDb.FormEmtiazBandi.IsApproveByModirKol = viewModel.IsApprovedByThisUser;
                clientInDb.FormEmtiazBandi.DateOfModirKolApprove = PersianDateTime.Today;
            }
            else if (User.IsInRole(RoleName.MoavenOstan))
            {
                clientInDb.FormEmtiazBandi.IsApproveByRelatedAssistance = viewModel.IsApprovedByThisUser;
                clientInDb.FormEmtiazBandi.DateOfRelatedAssistanceApprove = PersianDateTime.Today;
            }
            else if (User.IsInRole(RoleName.ModirShahrestan))
            {
                clientInDb.FormEmtiazBandi.IsApproveByModirShahrestan = viewModel.IsApprovedByThisUser;
                clientInDb.FormEmtiazBandi.DateOfModirShahrestanApprove = PersianDateTime.Today;
            }
            else
            {
                clientInDb.FormEmtiazBandi.IsApproveByMoavenMosharekatAssistance = viewModel.IsApprovedByThisUser;
                clientInDb.FormEmtiazBandi.DateOfMoavenMosharekatAssistanceApprove = PersianDateTime.Today;
            }


            if (clientInDb.FormEmtiazBandi.IsApproveByModirKol &&
                clientInDb.FormEmtiazBandi.IsApproveByKarshenasShahrestan &&
                clientInDb.FormEmtiazBandi.IsApproveByMoavenMosharekatAssistance &&
                clientInDb.FormEmtiazBandi.IsApproveByModirShahrestan &&
                clientInDb.FormEmtiazBandi.IsApproveByRelatedAssistance)
            {
                clientInDb.ClientState.ClientStateTypeE = ModelEnums.ClientStateTypeE.ApproveAllFormEmtiazBandi;

                var clientStateLog = new ClientStateLog
                {
                    ClientId = clientInDb.Id,
                    ClientStateTypeE = ModelEnums.ClientStateTypeE.ApproveAllFormEmtiazBandi,
                    LogDate = DateTime.Now,
                    Description = "توسط همه مدیران امضا و تایید شد.",
                };
                _dbContext.ClientStateLogs.Add(clientStateLog);
            }

            _dbContext.SaveChanges();

            return RedirectToAction("EditFormEmtiazBandi", "Client", new { id = viewModel.Client.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFormEmtiazBandiKarshenas(EditClientRateFormViewModel viewModel)
        {
            ModelState.Remove("Client.ClientTypeId");
            ModelState.Remove("Client.AssistanceTypeId");
            ModelState.Remove("TempImageIdFormEmtiazBandi");

            long amount;
            var amountIsNumber = long.TryParse(viewModel.Amount.Replace(",", ""), out amount);

            if (!ModelState.IsValid || !amountIsNumber) return RedirectToAction("EditFormEmtiazBandi", new { id = viewModel.Client.Id });

            if (!User.IsInRole(RoleName.KarshenasShahrestan) && !User.IsInRole(RoleName.KarshenasOstan))
                return RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var clientInDb = _dbContext.Clients
                .Include(c => c.FormEmtiazBandi)
                .Include(c => c.City.District.County.Province)
                .Include(c => c.Person)
                .Single(c => c.Id == viewModel.Client.Id);

            // فقط در صورتی که هیچ فرم امتیاز بندی برای این مددجو ثبت نشده باشد کارشناس شهرستان می تواند اطلاعات آن را ثبت کند
            // چون این مددجو فرم امتیاز بندی داشت پس به کاربر خطا نشان می دهد
            if (clientInDb.FormEmtiazBandi != null)
                return RedirectToAction("Index", "Client",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var tempImage =
                _dbContext.TemporaryImages.SingleOrDefault(ti => ti.Id == viewModel.TempImageIdFormEmtiazBandi);
            if (tempImage == null)
                return HttpNotFound();

            var newAddress = GetOrCreateClientDocumentFolder(clientInDb.City.District.County.ProvinceId,
                clientInDb.Person.NationalCode);

            newAddress = Path.Combine(newAddress,
                $"FormEmtiazBandi{Path.GetExtension(tempImage.MasterFileName ?? ".jpg")}");
            if (System.IO.File.Exists(newAddress))
                System.IO.File.Delete(newAddress);

            System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

            _dbContext.TemporaryImages.Remove(tempImage);

            var clientDoc = new ClientDocument
            {
                ClientId = clientInDb.Id,
                DocumentTypeId = (int)ModelEnums.DocumentTypesE.FormEmtiazBandi,
                IsOptimized = false,
                DocURI = newAddress,
            };
            _dbContext.ClientDocuments.Add(clientDoc);
            _dbContext.SaveChanges();


            clientInDb.FormEmtiazBandi = new FormEmtiazBandi
            {
                Amount = amount,
                Rate = viewModel.FormEmtiazBandi.Rate,
                IsApproveByKarshenasShahrestan = true,
                DateOfKarshenasShahrestanApprove = PersianDateTime.Today,
                IsApprovedOffline = viewModel.FormEmtiazBandi.IsApprovedOffline,
                OfflineFormEmtiazBandi = clientDoc,

            };
            _dbContext.FormEmtiazBandi.Add(clientInDb.FormEmtiazBandi);

            _dbContext.SaveChanges();


            //کارشناس شهرستان رو به همون صفحه فرم امتیاز بندی برگردان
            return RedirectToAction("EditFormEmtiazBandi", new { id = viewModel.Client.Id });
        }


        //[HttpPost]
        //public ActionResult SaveClientFormEmtiazBandiOffline(EditClientRateFormViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return RedirectToAction("EditClientRateForm", new { id = viewModel.Client.Id });
        //    }

        //    var clientInDb = _dbContext.Clients
        //        .Include(c => c.Person)
        //        .Include(c => c.City.District.County)
        //        .SingleOrDefault(c => c.Id == viewModel.Client.Id);
        //    if (clientInDb == null)
        //        return HttpNotFound();

        //    var financialAid = new FinancialAid
        //    {
        //        ClientId = clientInDb.Id,
        //        Amount = viewModel.FinancialAid.Amount,
        //        ClientDocumentId = clientDoc.Id,
        //        CoOrganizationTypeId = viewModel.FinancialAid.CoOrganizationTypeId,
        //        DepositDate = ConvertDate.ToGregorian(viewModel.FinancialAid.DepositDate),
        //    };
        //    _dbContext.FinancialAids.Add(financialAid);
        //    _dbContext.SaveChanges();

        //    return RedirectToAction("EditClientFinancialAid", new { id = clientInDb.Id });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFormEmtiazBandi(EditClientRateFormViewModel viewModel)
        {
            if (viewModel?.Client == null || viewModel.Client.Id == 0) return HttpNotFound();

            if (!User.IsInRole(RoleName.KarshenasOstan))
                return RedirectToAction("Index",
                    new { Message = MessageClientManageId.YouCannotAccessThisArea });

            var formEmtiazBandiInDb = _dbContext.FormEmtiazBandi
                .Include(s => s.OfflineFormEmtiazBandi)
                .Single(f => f.Id == viewModel.Client.Id);
            if (formEmtiazBandiInDb.OfflineFormEmtiazBandi != null &&
                System.IO.File.Exists(formEmtiazBandiInDb.OfflineFormEmtiazBandi.DocURI))
                System.IO.File.Delete(formEmtiazBandiInDb.OfflineFormEmtiazBandi.DocURI);
            _dbContext.FormEmtiazBandi.Remove(formEmtiazBandiInDb);
            _dbContext.SaveChanges();


            return RedirectToAction("EditFormEmtiazBandi", new { id = viewModel.Client.Id });
        }

        public ActionResult EditRequiredMaterial(int id, MessageClientManageId? message)
        {
            Client client;
            ActionResult result;
            if (!CheckPrivilegeForEditClient(id, out client, out result))
                return result;

            var viewModel = new ClientRequiredMaterialViewModel
            {
                Client = _mapper.Map<ClientDto>(client),
                ClientId = id,
                AllMaterialTypes = _dbContext.MaterialTypes.ToList(),
                ClientRequiredMaterials = client.ClientRequiredMaterials.ToList(),
                ClientRequiredMaterial = new ClientRequiredMaterial { Date = DateTime.Now, ClientId = client.Id }
            };

            PopulateMenuTabItemOfRegisteredClientForm(client.Id);

            if (message != null)
                viewModel.Status = new Status
                {
                    StatusType = message == MessageClientManageId.DataSavedSuccessfully
                        ? ModelEnums.StatusTypeE.Success
                        : message == MessageClientManageId.Error
                            ? ModelEnums.StatusTypeE.Error
                            : ModelEnums.StatusTypeE.Warning
                };

            return View(viewModel);
        }

        public ActionResult SaveRequiredMaterial(ClientRequiredMaterial clientRequiredMaterial)
        {
            ModelState.Remove("Date");
            if (!ModelState.IsValid)
                return RedirectToAction("EditRequiredMaterial",
                    new { id = clientRequiredMaterial.ClientId, message = MessageClientManageId.ValidationError });

            var clientInDb = _dbContext.Clients.SingleOrDefault(c => c.Id == clientRequiredMaterial.ClientId);
            if (clientInDb == null)
                return HttpNotFound();


            var materialTypeInDb =
                _dbContext.MaterialTypes.SingleOrDefault(mt => mt.Id == clientRequiredMaterial.MaterialTypeId);
            if (materialTypeInDb == null)
                return HttpNotFound();

            clientRequiredMaterial.Date = DateTime.Now;

            _dbContext.ClientRequiredMaterials.Add(clientRequiredMaterial);
            _dbContext.SaveChanges();

            return RedirectToAction("EditRequiredMaterial",
                new { id = clientRequiredMaterial.ClientId, message = MessageClientManageId.DataSavedSuccessfully });
        }
    }
}