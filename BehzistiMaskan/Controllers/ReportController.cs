using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.Remoting.Channels;
using System.Web.Mvc;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.ReportBuilder;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using MD.PersianDateTime;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace BehzistiMaskan.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();
        private readonly IMapper _mapper;

        public enum MessageReportManageId
        {
            DataSavedSuccessfully,
            Error,
            ValidationError,
        }

        private const string TempReportCacheFolder = "~/TempReportCache";
        private const string PersianDateFormatStr = "yyyy/MM/dd";


        public ReportController()
        {
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
        }

        // GET: Report
        public ActionResult Index(MessageReportManageId? Message)
        {
            // در صورتی که کد استان انتخاب شده بود، پس گزارش مربوط به یک استان خاص می باشد
            // در صورتی که فقط کد شهرستان انتخاب شده بود، گزارش مربوط به یک شهرستان خاص می باشد
            // در صورتی که هیچ کدام از کد های استان و شهرستان انتخاب نشده بود، گزارش مربوط به کل کشور می باشد
            // و فقط توسط کارشناس کشور قابل ویرایش می باشد

            var status = new Status();
            switch (Message)
            {
                case MessageReportManageId.DataSavedSuccessfully:
                    status.StatusType = ModelEnums.StatusTypeE.Success;
                    break;
                case MessageReportManageId.Error:
                    status.StatusType = ModelEnums.StatusTypeE.Error;
                    break;
                case MessageReportManageId.ValidationError:
                    status.StatusType = ModelEnums.StatusTypeE.ValidationError;
                    break;
                case null:
                    status = null;
                    break;
                default:
                    break;
            }

            ViewBag.Status = status;

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos.Single(u => u.Id == userId);

            var allReportsInDb = _dbContext.Reports.AsQueryable();

            if (User.IsInRole(RoleName.KarshenasOstan) || User.IsInRole(RoleName.KarshenasMasoolOstan) ||
                User.IsInRole(RoleName.ModirKolOstan) ||
                User.IsInRole(RoleName.MoavenMosharekat) || User.IsInRole(RoleName.MoavenOstan))
            {
                allReportsInDb = allReportsInDb.Where(r => r.ProvinceId == userInfoInDb.ProvinceId);
            }
            else if (User.IsInRole(RoleName.KarshenasShahrestan) || User.IsInRole(RoleName.ModirShahrestan))
            {
                allReportsInDb = allReportsInDb.Where(r => r.CountyId == userInfoInDb.CountyId);
            }
            else if (User.IsInRole(RoleName.SazmanHamkar))
            {
                if (userInfoInDb.CountyId != null)
                {// کاربر سازمان همکار شهرستان
                    allReportsInDb = allReportsInDb.Where(r => r.CountyId == userInfoInDb.CountyId);
                }
                else
                {//کاربر سازمان همکار استان
                    allReportsInDb = allReportsInDb.Where(r => r.ProvinceId == userInfoInDb.ProvinceId);
                }
            }
            else
            {
                allReportsInDb = allReportsInDb.Where(r => r.ProvinceId == null && r.CountyId == null);
            }

            var outRes = allReportsInDb.AsEnumerable().Select(_mapper.Map<ReportSimpleDto>).ToList();
            return View(outRes);
        }

        public ActionResult New()
        {
            var userId = User.Identity.GetUserId();
            var provinceId = _dbContext.UserInfos.Single(u => u.Id == userId).ProvinceId;

            var allForms = _dbContext.Forms
                .Include(f => f.Fields)
                .Include(f => f.Fields.Select(fi => fi.FieldTemplate))
                .Where(f => f.ProvinceId == provinceId)
                .AsEnumerable()
                .Select(_mapper.Map<FormDto>).ToList();

            var viewModel = new ReportDesignerViewModel
            {
                AllCounties = _dbContext.Counties.Where(c => c.ProvinceId == provinceId).AsEnumerable().Select(_mapper.Map<CountyDto>).ToList(),
                SelectedBehzistiDocumentCounties = new List<int>(),
                AllForms = allForms,
                Report = new Report(),

            };

            return View("ReportForm", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var reportInDb = _dbContext.Reports
                .Include(r => r.ReportForms)
                .Include(r => r.ReportForms.Select(rf => rf.ReportFormFields))
                .Include(r => r.ReportCounties)
                .SingleOrDefault(r => r.Id == id);

            if (reportInDb == null)
                return HttpNotFound();

            var userId = User.Identity.GetUserId();
            var provinceId = _dbContext.UserInfos.Single(u => u.Id == userId).ProvinceId;

            var allForms = _dbContext.Forms
                .Include(f => f.Fields)
                .Include(f => f.Fields.Select(fi => fi.FieldTemplate))
                .Where(f => f.ProvinceId == provinceId)
                .AsEnumerable()
                .Select(_mapper.Map<FormDto>).ToList();

            var selectedCountyIds = reportInDb.ReportCounties.Select(rc => rc.CountyId).ToList();
            var allCountiesInThisProvince = _dbContext.Counties.Where(c => c.ProvinceId == provinceId).ToList();
            var viewModel = new ReportDesignerViewModel
            {
                AllCounties = allCountiesInThisProvince.AsEnumerable().Select(_mapper.Map<CountyDto>).ToList(),
                SelectedBehzistiDocumentCounties = selectedCountyIds,
                AllForms = allForms,
                Report = reportInDb,
            };

            return View("ReportForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ReportDesignerViewModel viewModel)
        {
            ModelState.Remove("AllCounties");
            if (!ModelState.IsValid)
            {
                return View("ReportForm", viewModel);
            }

            var isEdit = viewModel.Report.Id != 0;
            var userId = User.Identity.GetUserId();
            var userInfo = _dbContext.UserInfos.Single(u => u.Id == userId);

            if (!isEdit)
            {


                viewModel.Report.CreatedAt = DateTime.Now;
                viewModel.Report.UpdateAt = DateTime.Now;
                viewModel.Report.CreatorName = userInfo.Name + " " + userInfo.Family;

                if (User.IsInRole(RoleName.KarshenasOstan))
                {
                    viewModel.Report.ProvinceId = userInfo.ProvinceId;
                }
                else if (User.IsInRole(RoleName.KarshenasShahrestan))
                {
                    viewModel.Report.ProvinceId = userInfo.ProvinceId;
                    viewModel.Report.CountyId = userInfo.CountyId;
                }
                // New Report
                _dbContext.Reports.Add(viewModel.Report);

                _dbContext.SaveChanges();

            }
            else
            {
                // Edit Report
                var reportInDb = _dbContext.Reports.SingleOrDefault(r => r.Id == viewModel.Report.Id);
                if (reportInDb == null)
                    return HttpNotFound();
                viewModel.Report.CreatorName = reportInDb.CreatorName;
                viewModel.Report.CreatedAt = reportInDb.CreatedAt;
                viewModel.Report.ProvinceId = reportInDb.ProvinceId;
                viewModel.Report.CountyId = reportInDb.CountyId;

                _mapper.Map(viewModel.Report, reportInDb);


                reportInDb.UpdateAt = DateTime.Now;
                _dbContext.SaveChanges();

                viewModel.Report = reportInDb;
                var reportFormIds = _dbContext.ReportForms.Where(rf => rf.ReportId == reportInDb.Id).Select(rf => rf.Id)
                    .ToList();

                // remove all report form fields
                var allReportFormFields = _dbContext.ReportFormFields.Where(ff => reportFormIds.Contains(ff.ReportFormId));
                _dbContext.ReportFormFields.RemoveRange(allReportFormFields);
                _dbContext.SaveChanges();

                // remove all report form
                var allReportForms = _dbContext.ReportForms.Where(rf => rf.ReportId == reportInDb.Id);
                _dbContext.ReportForms.RemoveRange(allReportForms);
                _dbContext.SaveChanges();

                //remove all report counties
                var allReportCounty = _dbContext.ReportCounties.Where(rc => rc.ReportId == reportInDb.Id);
                _dbContext.ReportCounties.RemoveRange(allReportCounty);
                _dbContext.SaveChanges();
            }

            if (viewModel.SelectedBehzistiDocumentCounties != null)
            {
                var allCounties = _dbContext.Counties.ToList();
                // add all report counties
                foreach (var countyId in viewModel.SelectedBehzistiDocumentCounties)
                {
                    var countyInDb = allCounties.SingleOrDefault(c => c.Id == countyId);
                    if (countyInDb == null) continue;
                    var reportCounty = new ReportCounty
                    {
                        CountyId = countyId,
                        ReportId = viewModel.Report.Id
                    };
                    _dbContext.ReportCounties.Add(reportCounty);
                }

                _dbContext.SaveChanges();
            }


            var jsonReportForm = JsonConvert.DeserializeObject<IEnumerable<JsonReportForm>>(viewModel.JsonSelectedFormFields);

            //Add All report forms
            foreach (var reportForm in jsonReportForm)
            {
                var newReportForm = new ReportForm
                {
                    FormId = reportForm.FormId,
                    ReportId = viewModel.Report.Id,
                };
                _dbContext.ReportForms.Add(newReportForm);
                _dbContext.SaveChanges();


                foreach (var reportFormField in reportForm.JsonReportFormFields)
                {

                    var newReportFormField = new ReportFormField
                    {
                        FieldId = reportFormField.FieldId,
                        ReportFormId = newReportForm.Id,
                    };
                    _dbContext.ReportFormFields.Add(newReportFormField);
                    _dbContext.SaveChanges();
                }

            }

            return RedirectToAction("Index", new { Message = MessageReportManageId.DataSavedSuccessfully });
        }


        public ActionResult Generate(int id)
        {
            var reportInDb = _dbContext.Reports
                .Include(r => r.ReportCounties)
                .Include(r => r.ReportForms)
                .Include(r => r.ReportForms.Select(rf => rf.Form))
                .Include(r => r.ReportForms.Select(rf => rf.ReportFormFields))
                .Include(r => r.ReportForms.Select(rf => rf.ReportFormFields.Select(rff => rff.Field)))
                .SingleOrDefault(r => r.Id == id);
            if (reportInDb == null)
                return HttpNotFound();

            var reportResult = _dbContext.Clients
                    .Include(c => c.Person)
                    .Include(c => c.ClientForms)
                    .Include(c => c.CurrentHousing)
                    .Include(c => c.ContactInfo)
                    .Include(c => c.BankInfo)
                    .Include(c => c.BankInfo.BankType)
                    .Include(c => c.ClientType)
                    .Include(c => c.AssistanceType)

                    .Include(c => c.CurrentHousing)
                    .Include(c => c.CurrentHousing.CurrentHouseType)

                    .Include(c => c.City)
                    .Include(c => c.City.District)
                    .Include(c => c.City.District.County)
                    .Include(c => c.City.District.County.Province)

                    .Include(c => c.Person.CityOfBirth)
                    .Include(c => c.Person.CityOfBirth.District)
                    .Include(c => c.Person.CityOfBirth.District.County)
                    .Include(c => c.Person.CityOfBirth.District.County.Province)


                    .Include(c => c.Person.MarriageType)
                    .Include(c => c.Person.GenderType)

                    .Include(c => c.ClientPhysicalProgresses)
                    .Include(c => c.ClientPhysicalProgresses.Select(ph => ph.PhysicalProgress))
                    .Include(c => c.ClientForms)
                    .Include(c => c.ClientForms.Select(cf => cf.Form))
                    .Include(c => c.ClientForms.Select(cf => cf.Form.Fields))
                    .Include(c => c.FinancialAids)
                    .Include(c => c.FinancialAids.Select(f => f.CoOrganizationType))

                    .Include(c => c.ClientForms)
                    .Include(c => c.ClientForms.Select(cf => cf.ClientFormFields))
                    .Where(c => c.IsDeleted != true)
                ;


            if (reportInDb.ReportCounties.Any())
            {
                var countyIds = reportInDb.ReportCounties.Select(rc => rc.CountyId).ToList();
                reportResult = reportResult.Where(c => countyIds.Contains(c.City.District.CountyId));
            }

            if (reportInDb.ReportForms.Any())
            {
                var formIds = reportInDb.ReportForms.Select(rf => rf.FormId).ToList();
                reportResult = reportResult.Where(c => c.ClientForms.Any(cf => formIds.Contains(cf.FormId)));
            }

            var allPhysicalProgress = _dbContext.PhysicalProgresses.ToList();

            //var viewModel = new GenerateReportViewModel
            //{
            //    Report = reportInDb,
            //    ReportResult = reportResult.ToList(),
            //    PhysicalProgresses = allPhysicalProgress,

            //};

            var clientReportList = reportResult.ToList();

            var userId = User.Identity.GetUserId();

            // آدرس پوشه موقت برای ذخیره فایل های گزارش را یافته و در صورت نبودن پوشه آن را می سازد
            var tempReportFolder = Server.MapPath(TempReportCacheFolder);
            if (!Directory.Exists(tempReportFolder))
                Directory.CreateDirectory(tempReportFolder);

            var currentPersianDateTime = PersianDateTime.Now;

            var reportName = $"{reportInDb.ReportName}_{currentPersianDateTime.Year:0000}-{currentPersianDateTime.Month:00}-{currentPersianDateTime.Day:00}_{currentPersianDateTime.Hour:00}-{currentPersianDateTime.Minute:00}-{currentPersianDateTime.Second:00}_{userId}.xlsx";
            var excelFileAddress = Path.Combine(tempReportFolder, reportName);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(excelFileAddress)))
            {
                var sheet = package.Workbook.Worksheets.Add("Info");
                sheet.View.RightToLeft = true;

                var colIndex = 1;

                #region Generate Table Header
                // Begin Generate Table Header

                sheet.Cells[2, colIndex++].Value = "#";
                sheet.Cells[2, colIndex++].Value = "کد ملی";

                if (reportInDb.ClientIsDisabledShow)
                {
                    sheet.Cells[2, colIndex++].Value = "وضعیت جسمی";
                }

                if (reportInDb.ClientName)
                {
                    sheet.Cells[2, colIndex++].Value = "نام";
                }

                if (reportInDb.ClientFamily)
                {
                    sheet.Cells[2, colIndex++].Value = "نام خانوادگی";
                }

                if (reportInDb.ClientFatherName)
                {
                    sheet.Cells[2, colIndex++].Value = "نام پدر";
                }

                if (reportInDb.ClientMotherName)
                {
                    sheet.Cells[2, colIndex++].Value = "نام مادر";
                }

                if (reportInDb.ClientBirthDate)
                {
                    sheet.Cells[2, colIndex++].Value = "تاریخ تولد";
                }

                if (reportInDb.ClientGender)
                {
                    sheet.Cells[2, colIndex++].Value = "جنسیت";
                }

                if (reportInDb.ClientMarriageStatus)
                {
                    sheet.Cells[2, colIndex++].Value = "وضعیت تاهل";
                }

                if (reportInDb.ClientChildrenCount)
                {
                    sheet.Cells[2, colIndex++].Value = "تعداد فرزندان";
                }

                if (reportInDb.ClientCertificateNo)
                {
                    sheet.Cells[2, colIndex++].Value = "شماره شناسنامه";
                }

                if (reportInDb.ClientCertificateMosalsal)
                {
                    sheet.Cells[2, colIndex++].Value = "مسلسل شناسنامه";
                }

                if (reportInDb.ClientCertificateDescription)
                {
                    sheet.Cells[2, colIndex++].Value = "توضیحات شناسنامه";
                }

                if (reportInDb.ClientProvinceOfBirth)
                {
                    sheet.Cells[2, colIndex++].Value = "استان محل تولد";
                }

                if (reportInDb.ClientCountyOfBirth)
                {
                    sheet.Cells[2, colIndex++].Value = "شهرستان محل تولد";
                }

                if (reportInDb.ClientDistrictOfBirth)
                {
                    sheet.Cells[2, colIndex++].Value = "بخش محل تولد";
                }

                if (reportInDb.ClientCityOfBirth)
                {
                    sheet.Cells[2, colIndex++].Value = "شهر محل تولد";
                }

                if (reportInDb.ClientBehzistiDocumentCounty)
                {
                    sheet.Cells[2, colIndex++].Value = "شهرستان";
                }

                if (reportInDb.ClientBehzistiDocumentDistrict)
                {
                    sheet.Cells[2, colIndex++].Value = "بخش";
                }

                if (reportInDb.ClientBehzistiDocumentCity)
                {
                    sheet.Cells[2, colIndex++].Value = "شهر یا روستا";
                    sheet.Cells[2, colIndex++].Value = "شهری یا روستایی";
                }

                if (reportInDb.ClientAssistance)
                {
                    sheet.Cells[2, colIndex++].Value = "نوع حمایت (معاونت)";
                }

                if (reportInDb.ClientType)
                {
                    sheet.Cells[2, colIndex++].Value = "نوع مددجو";
                }

                if (reportInDb.ClientDescription)
                {
                    sheet.Cells[2, colIndex++].Value = "توضیحات نوع مددجو";
                }

                if (reportInDb.BehzistiCode)
                {
                    sheet.Cells[2, colIndex++].Value = "شماره پرونده مددجو";
                }

                if (reportInDb.GlobalBehzistiUiCode)
                {
                    sheet.Cells[2, colIndex++].Value = "کد مددجو در سامانه کشوری";
                }

                if (reportInDb.NumberOfDisabledInFamily)
                {
                    sheet.Cells[2, colIndex++].Value = "تعداد معلولین در خانواده";
                }

                if (reportInDb.IsHouseHold)
                {
                    sheet.Cells[2, colIndex++].Value = "سرپرست خانوار";
                }

                if (reportInDb.CurrentHousing_CurrentHouseTypeId)
                {
                    sheet.Cells[2, colIndex++].Value = "وضعیت مسکن فعلی";
                }

                if (reportInDb.CurrentHousing_DepositAmount)
                {
                    sheet.Cells[2, colIndex++].Value = "میزان ودیعه";
                }

                if (reportInDb.CurrentHousing_MonthlyRentalRate)
                {
                    sheet.Cells[2, colIndex++].Value = "میزان اجاره ماهانه";
                }

                if (reportInDb.CurrentHousing_Address)
                {
                    sheet.Cells[2, colIndex++].Value = "آدرس کامل مسکن فعلی";
                }

                if (reportInDb.CurrentHousing_PostalCode)
                {
                    sheet.Cells[2, colIndex++].Value = "کد پستی مسکن فعلی";
                }

                if (reportInDb.ContactInfo_HomeTel)
                {
                    sheet.Cells[2, colIndex++].Value = "تلفن منزل";
                }

                if (reportInDb.ContactInfo_Mobile)
                {
                    sheet.Cells[2, colIndex++].Value = "موبایل";
                }

                if (reportInDb.ContactInfo_EmergencyTel)
                {
                    sheet.Cells[2, colIndex++].Value = "موبایل ضروری";
                }

                if (reportInDb.BankInfo_BankTypeId)
                {
                    sheet.Cells[2, colIndex++].Value = "نام بانک";
                }

                if (reportInDb.BankInfo_AccountNumber)
                {
                    sheet.Cells[2, colIndex++].Value = "شماره حساب";
                }

                if (reportInDb.BankInfo_CardNumber)
                {
                    sheet.Cells[2, colIndex++].Value = "شماره کارت";
                }

                if (reportInDb.PhysicalProgress)
                {
                    sheet.Cells[1, colIndex, 1, (colIndex + allPhysicalProgress.Count() - 1)].Merge = true;
                    sheet.Cells[1, colIndex].Value = "مراحل پیشرفت فیزیکی";
                    if (!allPhysicalProgress.Any())
                        colIndex++;

                    foreach (var physicalProgress in allPhysicalProgress)
                    {
                        sheet.Cells[2, colIndex++].Value = physicalProgress.Name;
                    }
                }

                foreach (var reportForm in reportInDb.ReportForms)
                {
                    var formFieldCount = reportForm.ReportFormFields.Count();
                    if (formFieldCount != 0)
                        sheet.Cells[1, colIndex, 1, colIndex + formFieldCount].Merge = true;

                    sheet.Cells[1, colIndex].Value = reportForm.Form.Name;

                    sheet.Cells[2, colIndex++].Value = $"در طرح '{reportForm.Form.Name}' ثبت شده است؟";
                    foreach (var reportFormField in reportForm.ReportFormFields)
                    {
                        sheet.Cells[2, colIndex++].Value = reportFormField.Field.Title;
                    }
                }

                // End Generate Table Header
                #endregion

                #region Generate Table Data
                //Begin Generate Table Data

                var rowIndex = 3;

                foreach (var client in clientReportList)
                {
                    colIndex = 1;
                    sheet.Cells[rowIndex, colIndex++].Value = rowIndex - 2;
                    sheet.Cells[rowIndex, colIndex++].Value = client.Person.NationalCode;

                    if (reportInDb.ClientIsDisabledShow)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.IsDisabled != null ? "معلول" : "سالم";
                    }

                    if (reportInDb.ClientName)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.Name;
                    }

                    if (reportInDb.ClientFamily)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.Family;
                    }

                    if (reportInDb.ClientFatherName)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.FatherName;
                    }

                    if (reportInDb.ClientMotherName)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.MotherName;
                    }

                    if (reportInDb.ClientBirthDate)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.Birthdate?.ToJalali().ToString(PersianDateFormatStr) ?? "";
                    }

                    if (reportInDb.ClientGender)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.GenderType?.Name ?? "";
                    }

                    if (reportInDb.ClientMarriageStatus)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.MarriageType?.Name ?? "";
                    }

                    if (reportInDb.ClientChildrenCount)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.NumberOfChildren;
                    }

                    if (reportInDb.ClientCertificateNo)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.BirthCertificateNo;
                    }

                    if (reportInDb.ClientCertificateMosalsal)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.BirthCertificateMosalsal;
                    }

                    if (reportInDb.ClientCertificateDescription)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.BirthCertificateDescription;
                    }

                    if (reportInDb.ClientProvinceOfBirth)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.CityOfBirth?.District.County.Province.Name;
                    }

                    if (reportInDb.ClientCountyOfBirth)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.CityOfBirth?.District.County.Name;
                    }

                    if (reportInDb.ClientDistrictOfBirth)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.CityOfBirth?.District.Name;
                    }

                    if (reportInDb.ClientCityOfBirth)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.Person.CityOfBirth?.Name;
                    }

                    if (reportInDb.ClientBehzistiDocumentCounty)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.City.District.County.Name;
                    }

                    if (reportInDb.ClientBehzistiDocumentDistrict)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.City.District.Name;
                    }

                    if (reportInDb.ClientBehzistiDocumentCity)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.City.Name;
                        sheet.Cells[rowIndex, colIndex++].Value = client.City.IsVillage ? "روستایی" : "شهری";
                    }

                    if (reportInDb.ClientAssistance)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.AssistanceType?.Name;
                    }

                    if (reportInDb.ClientType)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.ClientType?.Name;
                    }

                    if (reportInDb.ClientDescription)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.ClientTypeDescription;
                    }

                    if (reportInDb.BehzistiCode)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.BehzistiCode;
                    }

                    if (reportInDb.GlobalBehzistiUiCode)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.GlobalBehzistiUiCode;
                    }

                    if (reportInDb.NumberOfDisabledInFamily)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.NumberOfDisabledInFamily?.ToString();
                    }

                    if (reportInDb.IsHouseHold)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.IsHouseHold == true ? "سرپرست خانوار" : "";
                    }

                    if (reportInDb.CurrentHousing_CurrentHouseTypeId)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.CurrentHousing?.CurrentHouseType.Name;
                    }

                    if (reportInDb.CurrentHousing_DepositAmount)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.CurrentHousing?.DepositAmount?.ToString("N0");
                    }

                    if (reportInDb.CurrentHousing_MonthlyRentalRate)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.CurrentHousing?.MonthlyRentalRate?.ToString("N0");
                    }

                    if (reportInDb.CurrentHousing_Address)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.CurrentHousing?.AddressCurrentHouse;
                    }

                    if (reportInDb.CurrentHousing_PostalCode)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.CurrentHousing?.PostalCode;
                    }

                    if (reportInDb.ContactInfo_HomeTel)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.ContactInfo?.HomeTel;
                    }

                    if (reportInDb.ContactInfo_Mobile)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.ContactInfo?.Mobile;
                    }

                    if (reportInDb.ContactInfo_EmergencyTel)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.ContactInfo?.EmergencyTel;
                    }

                    if (reportInDb.BankInfo_BankTypeId)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.BankInfo?.BankType.Name;
                    }

                    if (reportInDb.BankInfo_AccountNumber)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.BankInfo?.AccountNumber;
                    }

                    if (reportInDb.BankInfo_CardNumber)
                    {
                        sheet.Cells[rowIndex, colIndex++].Value = client.BankInfo?.CardNumber;
                    }

                    if (reportInDb.PhysicalProgress)
                    {

                        foreach (var physicalProgress in allPhysicalProgress)
                        {
                            sheet.Cells[rowIndex, colIndex++].Value = client.ClientPhysicalProgresses.Any(cph => cph.Id == physicalProgress.Id) ? "*" : "";
                        }
                    }

                    foreach (var reportForm in reportInDb.ReportForms)
                    {
                        var thisForm = client.ClientForms.SingleOrDefault(cf => cf.FormId == reportForm.FormId);

                        sheet.Cells[rowIndex, colIndex++].Value = thisForm != null ? "بلی" : "";

                        foreach (var reportFormField in reportForm.ReportFormFields)
                        {
                            if (thisForm == null)
                            {
                                colIndex++;
                                continue;
                            }

                            var thisField = thisForm.ClientFormFields.SingleOrDefault(f => f.FieldId == reportFormField.FieldId);
                            sheet.Cells[rowIndex, colIndex++].Value = thisField?.Value;
                        }
                    }


                    rowIndex++;
                }

                var tableRange = sheet.Cells[2, 1, rowIndex - 1, colIndex - 1];
                var tbl = sheet.Tables.Add(tableRange, "TableAliAmanzadegan");
                tbl.TableStyle = TableStyles.Medium2;

                sheet.Cells[rowIndex + 1, 1, rowIndex + 1, colIndex - 1].Merge = true;
                sheet.Cells[rowIndex + 1, 1].Value = "خروجی گرفته شده از سامانه جامع مسکن مددجویان بهزیستی استان فارس - طراح و برنامه نویس: علی امان زادگان";

                var colFromHex = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
                sheet.Cells[rowIndex + 1, 1, rowIndex + 1, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[rowIndex + 1, 1, rowIndex + 1, colIndex].Style.Fill.BackgroundColor.SetColor(colFromHex);

                package.Save();

                var tempExcelFile = new TemporaryImage
                {
                    CreatedAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddMinutes(60),
                    MasterFileName = excelFileAddress,
                    TemporaryFileName = excelFileAddress
                };

                _dbContext.TemporaryImages.Add(tempExcelFile);
                _dbContext.SaveChanges();

                return File(excelFileAddress, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(excelFileAddress));

                //End Generate Table Data
                #endregion
            }
        }

        /// <summary>
        /// گزارش گیری مربوط به معافیت انشعابات
        /// </summary>
        /// <returns></returns>
        public ActionResult Exemption(ExemptionReportViewModel viewModel)
        {
            var userId = User.Identity.GetUserId();
            var userInfoInDB = _dbContext.UserInfos.Single(u => u.Id == userId);
            int? CoOrganizationTypeId = userInfoInDB.CoOrganizationTypeId;

            viewModel.AllExemptionRequests = _dbContext.RequestTypes.Where(rt => rt.IsExemption).ToList();
            viewModel.ExemptionReportResult = new List<ExemptionReportSimpleDto>();
            var allClientInDbWhoGetRequestLetter = _dbContext.Clients
                .Include(c => c.ClientRequests)
                .Include(c => c.ClientRequests.Select(cr => cr.GetLetters))
                .Include(c => c.ClientRequests.Select(cr => cr.ExemptionBenefits))
                .Include(c => c.ClientRequests.Select(cr => cr.RequestType))
                .Include(c => c.Person)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.CurrentHousing)
                .Where(c => c.ClientRequests.Any(cr => cr.GetLetters.Any()) && c.City.District.CountyId == userInfoInDB.CountyId);


            if (CoOrganizationTypeId !=null)
            {
                allClientInDbWhoGetRequestLetter.Where(c => c.CoOrganizationApproveLists
                    .FirstOrDefault(co => co.CoOrganizationTypeId == CoOrganizationTypeId) != null);
            }


            if (viewModel.HasBenefited)
            {
                allClientInDbWhoGetRequestLetter =
                    allClientInDbWhoGetRequestLetter.Where(c => c.ClientRequests.Any(cr => cr.ExemptionBenefits.Any()));
            }

            if (viewModel.SelectedExemptionTypeId != null)
            {
                allClientInDbWhoGetRequestLetter = allClientInDbWhoGetRequestLetter.Where(c =>
                    c.ClientRequests.Any(cr => cr.RequestTypeId == viewModel.SelectedExemptionTypeId));
            }

            if (viewModel.GetLetterStartDateRange != null)
            {
                var getLetterStartDateRangeMiladi = ((PersianDateTime)viewModel.GetLetterStartDateRange).ToDateTime();
                allClientInDbWhoGetRequestLetter = allClientInDbWhoGetRequestLetter.Where(c =>
                    c.ClientRequests.Any(cr =>
                        cr.GetLetters.Any(gl => gl.LetterDate >= getLetterStartDateRangeMiladi)));
            }

            if (viewModel.GetLetterEndDateRange != null)
            {
                // چون وقتی تاریخ ثبت می شود ساعت 0 تنظیم می گردد
                //بنابراین من یک روز به تاریخ اضافه کردم که ساعت 0 فردا را به عنوان تاریخ اتمام در نظر بگیرد
                var getLetterEndDateRangeMiladi = ((PersianDateTime)viewModel.GetLetterEndDateRange).ToDateTime().AddDays(1);
                allClientInDbWhoGetRequestLetter = allClientInDbWhoGetRequestLetter.Where(c =>
                    c.ClientRequests.Any(cr =>
                        cr.GetLetters.Any(gl => gl.LetterDate < getLetterEndDateRangeMiladi)));
            }

            if (viewModel.BenefitStartDateRange != null)
            {
                var benefitStartDateRangeMiladi = ((PersianDateTime)viewModel.BenefitStartDateRange).ToDateTime();
                allClientInDbWhoGetRequestLetter = allClientInDbWhoGetRequestLetter.Where(c =>
                    c.ClientRequests.Any(cr =>
                        cr.GetLetters.Any(gl => gl.LetterDate >= benefitStartDateRangeMiladi)));
            }

            if (viewModel.BenefitEndDateRange != null)
            {
                // چون وقتی تاریخ ثبت می شود ساعت 0 تنظیم می گردد
                //بنابراین من یک روز به تاریخ اضافه کردم که ساعت 0 فردا را به عنوان تاریخ اتمام در نظر بگیرد
                var benefitEndDateRangeMiladi = ((PersianDateTime)viewModel.BenefitEndDateRange).ToDateTime().AddDays(1);
                allClientInDbWhoGetRequestLetter = allClientInDbWhoGetRequestLetter.Where(c =>
                    c.ClientRequests.Any(cr =>
                        cr.GetLetters.Any(gl => gl.LetterDate >= benefitEndDateRangeMiladi)));
            }

            var delimiter = "،";
            var persianDateFormat = "yyyy/MM/dd";
            var resultList = new List<ExemptionReportSimpleDto>();
            foreach (var client in allClientInDbWhoGetRequestLetter)
            {
                var reportSimpleDto = new ExemptionReportSimpleDto
                {
                    ClientId = client.Id,
                    Name = client.Person.Name,
                    Family = client.Person.Family,
                    NationalCode = client.Person.NationalCode,
                    CountyName = client.City.District.County.Name,
                    Address = client.CurrentHousing.Address
                };


                var clientWaterExemptionRequest = client.ClientRequests.SingleOrDefault(cr => cr.RequestTypeId == (int)ModelEnums.ClientRequestTypeE.ExemptionWater);
                if (clientWaterExemptionRequest != null)
                {
                    reportSimpleDto.HasGetLetterWater = clientWaterExemptionRequest.GetLetters.Any();
                    reportSimpleDto.GetLetterDateWater = string.Join(delimiter, clientWaterExemptionRequest.GetLetters.Select(gl => new PersianDateTime(gl.LetterDate).ToString(persianDateFormat)));
                    reportSimpleDto.GetLetterNumberWater = string.Join(delimiter, clientWaterExemptionRequest.GetLetters.Select(gl => gl.LetterNumber).ToArray());

                    reportSimpleDto.HasBenefitedWater = clientWaterExemptionRequest.ExemptionBenefits.Any();
                    reportSimpleDto.BenefitDateWater = string.Join(delimiter, clientWaterExemptionRequest.ExemptionBenefits.Select(eb => new PersianDateTime(eb.BenefitDate).ToString(persianDateFormat)));
                    reportSimpleDto.BenefitAmountWater = clientWaterExemptionRequest.ExemptionBenefits.Sum(eb => eb.BenefitAmount);
                }


                var clientElectricalExemptionRequest = client.ClientRequests.SingleOrDefault(cr => cr.RequestTypeId == (int)ModelEnums.ClientRequestTypeE.ExemptionElectrical);
                if (clientElectricalExemptionRequest != null)
                {
                    reportSimpleDto.HasGetLetterElectrical = clientElectricalExemptionRequest.GetLetters.Any();
                    reportSimpleDto.GetLetterDateElectrical = string.Join(delimiter, clientElectricalExemptionRequest.GetLetters.Select(gl => new PersianDateTime(gl.LetterDate).ToString(persianDateFormat)));
                    reportSimpleDto.GetLetterNumberElectrical = string.Join(delimiter, clientElectricalExemptionRequest.GetLetters.Select(gl => gl.LetterNumber).ToArray());

                    reportSimpleDto.HasBenefitedElectrical = clientElectricalExemptionRequest.ExemptionBenefits.Any();
                    reportSimpleDto.BenefitDateElectrical = string.Join(delimiter, clientElectricalExemptionRequest.ExemptionBenefits.Select(eb => new PersianDateTime(eb.BenefitDate).ToString(persianDateFormat)));
                    reportSimpleDto.BenefitAmountElectrical = clientElectricalExemptionRequest.ExemptionBenefits.Sum(eb => eb.BenefitAmount);
                }


                var clientGasExemptionRequest = client.ClientRequests.SingleOrDefault(cr => cr.RequestTypeId == (int)ModelEnums.ClientRequestTypeE.ExemptionGas);
                if (clientGasExemptionRequest != null)
                {
                    reportSimpleDto.HasGetLetterGas = clientGasExemptionRequest.GetLetters.Any();
                    reportSimpleDto.GetLetterDateGas = string.Join(delimiter, clientGasExemptionRequest.GetLetters.Select(gl => new PersianDateTime(gl.LetterDate).ToString(persianDateFormat)));
                    reportSimpleDto.GetLetterNumberGas = string.Join(delimiter, clientGasExemptionRequest.GetLetters.Select(gl => gl.LetterNumber).ToArray());

                    reportSimpleDto.HasBenefitedGas = clientGasExemptionRequest.ExemptionBenefits.Any();
                    reportSimpleDto.BenefitDateGas = string.Join(delimiter, clientGasExemptionRequest.ExemptionBenefits.Select(eb => new PersianDateTime(eb.BenefitDate).ToString(persianDateFormat)));
                    reportSimpleDto.BenefitAmountGas = clientGasExemptionRequest.ExemptionBenefits.Sum(eb => eb.BenefitAmount);
                }


                var clientProductionLicenseExemptionRequest = client.ClientRequests.SingleOrDefault(cr => cr.RequestTypeId == (int)ModelEnums.ClientRequestTypeE.ExemptionProductionLicense);
                if (clientProductionLicenseExemptionRequest != null)
                {
                    reportSimpleDto.HasGetLetterProductionLicense = clientProductionLicenseExemptionRequest.GetLetters.Any();
                    reportSimpleDto.GetLetterDateProductionLicense = string.Join(delimiter, clientProductionLicenseExemptionRequest.GetLetters.Select(gl => new PersianDateTime(gl.LetterDate).ToString(persianDateFormat)));
                    reportSimpleDto.GetLetterNumberProductionLicense = string.Join(delimiter, clientProductionLicenseExemptionRequest.GetLetters.Select(gl => gl.LetterNumber).ToArray());

                    reportSimpleDto.HasBenefitedProductionLicense = clientProductionLicenseExemptionRequest.ExemptionBenefits.Any();
                    reportSimpleDto.BenefitDateProductionLicense = string.Join(delimiter, clientProductionLicenseExemptionRequest.ExemptionBenefits.Select(eb => new PersianDateTime(eb.BenefitDate).ToString(persianDateFormat)));
                    reportSimpleDto.BenefitAmountProductionLicense = clientProductionLicenseExemptionRequest.ExemptionBenefits.Sum(eb => eb.BenefitAmount);
                }


                resultList.Add(reportSimpleDto);
            }
            viewModel.ExemptionReportResult = resultList;

            return View(viewModel);
        }


        [Authorize(Roles = RoleName.KarshenasOstan)]
        public ActionResult Delete(int id)
        {
            var reportInDb = _dbContext.Reports.SingleOrDefault(r => r.Id == id);
            if (reportInDb == null)
                return HttpNotFound();


            var reportDto = _mapper.Map<ReportSimpleDto>(reportInDb);

            return View(reportDto);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeletePost(int id)
        {
            var reportInDb = _dbContext.Reports.SingleOrDefault(r => r.Id == id);
            if (reportInDb == null)
                return HttpNotFound();

            var allReportForms = _dbContext.ReportForms.Where(rf => rf.ReportId == id);

            var reportFormIds = allReportForms.Select(rf => rf.Id).ToList();
            var allReportFormFields = _dbContext.ReportFormFields.Where(rff => reportFormIds.Contains(rff.ReportFormId));
            _dbContext.ReportFormFields.RemoveRange(allReportFormFields);
            _dbContext.SaveChanges();

            _dbContext.ReportForms.RemoveRange(allReportForms);
            _dbContext.SaveChanges();


            var allReportCounties = _dbContext.ReportCounties.Where(rfc => rfc.ReportId == id);
            _dbContext.ReportCounties.RemoveRange(allReportCounties);
            _dbContext.SaveChanges();

            _dbContext.Reports.Remove(reportInDb);
            _dbContext.SaveChanges();

            return RedirectToAction("Index", new { Message = MessageReportManageId.DataSavedSuccessfully });
        }
    }
}
