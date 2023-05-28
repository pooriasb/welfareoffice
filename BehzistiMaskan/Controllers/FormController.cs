using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace BehzistiMaskan.Controllers
{
    [Authorize(Roles = RoleName.KarshenasOstan)]
    public class FormController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public FormController()
        {
            _dbContext = new ApplicationDbContext();
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
        }

        // GET: Form
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New()
        {
            var viewModel = new FormViewModel
            {
                Form = new FormDto { IsEnabled = true },
                FieldTemplates = _dbContext.FieldTemplates.ToList(),

                AllCoOrganizationTypes = _dbContext.CoOrganizationTypes.ToList(),
                CoOrganizationTypes = new List<CoOrganizationType>(),

                // ReSharper disable once AssignNullToNotNullAttribute
                Counties = _dbContext.Counties.AsEnumerable().Select(_mapper.Map<CountyDto>).ToList(),
                FormCountyAccessLevels = new List<FormAccessLevel>(),
                FormCountyAccessLevelIds = new List<int>(),

                AllPhysicalProgress = _dbContext.PhysicalProgresses.OrderBy(p => p.Order).ToList(),
            };

            return View("FormForm", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var formInDb = _dbContext.Forms
                .Include(f => f.Fields)
                .Include(f => f.Fields.Select(fi => fi.FieldTemplate))
                .Include(f => f.FormAccessLevels)
                .Include(f => f.FormCoOrganizationRoles)
                .SingleOrDefault(f => f.Id == id);
            if (formInDb == null)
            {
                return HttpNotFound();
            }

            var formCountyAccessLevels = _dbContext.FormCountyAccessLevels.Include(a => a.County).Where(a => a.FormId == id).ToList();
            var formCountyAccessLevelIds = formCountyAccessLevels.Select(c => c.CountyId).ToList();
            //var formCountyQuotas = formCountyAccessLevels.Select(c => c.Quota).ToList();

            var coOrganizationTypeIds = formInDb.FormCoOrganizationRoles.Select(c => c.CoOrganizationTypeId).ToList();
            var allCoOrganizationInDb = _dbContext.CoOrganizationTypes.OrderBy(c => c.Name).ToList();

            var viewModel = new FormViewModel
            {
                Form = _mapper.Map<FormDto>(formInDb),
                FieldTemplates = _dbContext.FieldTemplates.ToList(),

                AllCoOrganizationTypes = allCoOrganizationInDb.Where(c => !coOrganizationTypeIds.Contains(c.Id)).ToList(),
                CoOrganizationTypes = allCoOrganizationInDb.Where(c => coOrganizationTypeIds.Contains(c.Id)).ToList(),
                CoOrganizationTypeIds = coOrganizationTypeIds,


                Counties = _dbContext.Counties.Where(c => !formCountyAccessLevelIds.Contains(c.Id)).AsEnumerable().Select(_mapper.Map<CountyDto>).ToList(),
                FormCountyAccessLevels = formCountyAccessLevels.AsEnumerable(),
                FormCountyAccessLevelIds = formCountyAccessLevelIds,

                AllPhysicalProgress = _dbContext.PhysicalProgresses.OrderBy(p => p.Order).ToList(),
                FormPhysicalProgresses = _dbContext.FormPhysicalProgresses.Where(fp => fp.FormId == id).Include(fp => fp.FormPhysicalProgressHelps).ToList(),
            };

            return View("FormForm", viewModel);
        }


        public ActionResult Delete(int id)
        {
            var formInDb = _dbContext.Forms.SingleOrDefault(f => f.Id == id);
            if (formInDb == null)
            {
                return HttpNotFound();
            }

            var hasClientForm = _dbContext.ClientForms.Any(cf => cf.FormId == formInDb.Id);

            ViewBag.HasClientForm = hasClientForm;

            return View(formInDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFormPost(int id)
        {
            var formInDb = _dbContext.Forms.SingleOrDefault(f => f.Id == id);
            if (formInDb == null)
            {
                return HttpNotFound();
            }

            var hasClientForm = _dbContext.ClientForms.Any(cf => cf.FormId == formInDb.Id);
            if (hasClientForm)
            {
                return RedirectToAction("Index");
            }

            _dbContext.Forms.Remove(formInDb);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.KarshenasOstan)]
        public ActionResult Save(FormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
                viewModel.AllCoOrganizationTypes = _dbContext.CoOrganizationTypes.ToList();
                // ReSharper disable once AssignNullToNotNullAttribute
                viewModel.Counties = _dbContext.Counties.AsEnumerable().Select(_mapper.Map<CountyDto>).ToList();
                viewModel.FieldTemplates = _dbContext.FieldTemplates.ToList();

                return View("FormForm", viewModel);
            }

            // ----- افزودن یا بروزرسانی کردن طرح -----
            #region MyRegion
            Form form;
            var isInEditForm = viewModel.Form.Id != 0;

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos
                .Include(ui => ui.Province)
                .Single(ui => ui.Id == userId);

            if (userInfoInDb.ProvinceId == null)
                return RedirectToAction("Index", "Form");
            if (!isInEditForm)
            {
                // Add new form to db
                form = _mapper.Map<Form>(viewModel.Form);
                form.CreatedAt = DateTime.Now;
                form.ProvinceId = (int)userInfoInDb.ProvinceId;
                _dbContext.Forms.Add(form);
            }
            else
            {
                form = _dbContext.Forms.Single(f => f.Id == viewModel.Form.Id);
                _mapper.Map(viewModel.Form, form);
                form.UpdatedAt = DateTime.Now;

            }
            _dbContext.SaveChanges();


            #endregion

            //TODO Please Use This Mother Fucker
            //try
            //{

            //}
            //catch (Exception e)
            //{
            //    throw;
            //}


            // *********************************************************************************
            // ----- افزودن یا بروزرسانی کردن شهرستان هایی که طرح در آنها اجرا می شود -----
            #region MyRegion
            if (isInEditForm)
            {
                // اگر در حالت ویرایش بودیم باید تمام شهرستان هایی که در این طرح ثبت شده اند حذف شوند

                // بعدا باید چک شود که آیا مددجو ساکن این شهرستان در این طرح ثبت شده است یا خیر؟
                // در صورتی که مددجو ساکن این شهرستان در این طرح ثبت شده باشد نباید آن شهرستان را حذف کنیم

                var formAccessLevelInDb = _dbContext.FormCountyAccessLevels.Where(f => f.FormId == form.Id);
                _dbContext.FormCountyAccessLevels.RemoveRange(formAccessLevelInDb);
            }

            var quotaIndex = 0;
            foreach (var countyId in viewModel.FormCountyAccessLevelIds)
            {
                var formAccessLevel = new FormAccessLevel { CountyId = countyId, FormId = form.Id, Quota = viewModel.FormCountyQuotas[quotaIndex++] };
                _dbContext.FormCountyAccessLevels.Add(formAccessLevel);
            }

            _dbContext.SaveChanges();




            #endregion


            // ****************************************************************
            // ----- افزودن یا بروزرسانی کردن سازمان های همکار در طرح -----

            #region MyRegion

            if (isInEditForm)
            {
                // اگر در حالت ویرایش بودیم باید تمام سازمان های همکار که در این طرح ثبت شده اند حذف شوند

                // بعدا باید چک شود که آیا حذف این سازمان همکار در اطلاعات وارد شده مشکلی ایجاد می کند یا خیر؟
                // ممکن است با حذف سازمان همکار از این طرح اطلاعاتی که او ذخیره کرده است دیگر نمایش داده نشود
                // و یا حذف سازمان همکار باعث خراب شدن اطلاعات قبلی ثبت شده گردد
                var coOrgRole = _dbContext.FormCoOrganizationRoles.Where(c => c.FormId == form.Id);
                _dbContext.FormCoOrganizationRoles.RemoveRange(coOrgRole);
            }

            if (viewModel.CoOrganizationTypeIds != null)
            {
                foreach (var organizationTypeId in viewModel.CoOrganizationTypeIds)
                {
                    var coOrganizationRole = new FormCoOrganizationRole
                    { CoOrganizationTypeId = organizationTypeId, FormId = form.Id };
                    _dbContext.FormCoOrganizationRoles.Add(coOrganizationRole);
                }
            }
            else
            {
                var allFormCoOrgInDb = _dbContext.FormCoOrganizationRoles.Where(fc => fc.FormId == form.Id);
                _dbContext.FormCoOrganizationRoles.RemoveRange(allFormCoOrgInDb);
            }
            _dbContext.SaveChanges();

            #endregion


            // ******************************************************
            // ----- افزودن یا بروزرسانی کردن فیلد های طرح -----

            #region MyRegion

            var jsonFormFields = JsonConvert.DeserializeObject<JsonFormPostObject>(viewModel.JsonFormFieldStr).Items;
            try
            {
                foreach (var formField in jsonFormFields)
                    if (isInEditForm)
                    {
                        var fieldInDb = _dbContext.Fields.SingleOrDefault(f => f.Id == formField.Id);
                        if (fieldInDb == null)
                        {
                            // new Field
                            var field = _mapper.Map<Field>(formField);
                            field.CreatedAt = DateTime.Now;
                            field.FormId = form.Id;
                            _dbContext.Fields.Add(field);
                        }
                        else
                        {
                            // Edit Existing field
                            _mapper.Map(formField, fieldInDb);
                            fieldInDb.UpdatedAt = DateTime.Now;
                        }
                    }
                    else
                    {
                        var field = _mapper.Map<Field>(formField);
                        field.CreatedAt = DateTime.Now;
                        field.FormId = form.Id;

                        _dbContext.Fields.Add(field);
                    }

                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            #endregion



            // ************************************************************************************************************
            // -----  افزودن یا بروزرسانی کردن اطلاعات پیشرفت فیزیکی و میزان کمک هر سازمان -----

            #region MyRegion
            var jsonPhysicalProgressList = JsonConvert.DeserializeObject<List<JsonPhysicalProgress>>(viewModel.JsonFormPhysicalProgressStr);


            if (isInEditForm)
            {
                // اول از همه تمام موارد مربوط به پیشرفت فیزیکی از پایگاه داده حذف شده و سپس آن ها را از روی اطلاعات جدید ثبت می کنیم

                //select all FormPhysicalProgress related to current form
                var formPhysicalProgressesInDb = _dbContext.FormPhysicalProgresses.Where(fp => fp.FormId == form.Id);
                // select ids
                var formPhysicalProgressesIds = formPhysicalProgressesInDb.Select(fp => fp.Id).ToList();

                // select all FormPhysicalProgressHelp related to current form
                var formPhysicalProgressHelpInDb = _dbContext.FormPhysicalProgressHelps.Where(fph =>
                    formPhysicalProgressesIds.Contains(fph.FormPhysicalProgressId));

                //remove all formPhysicalProgressHelp from db
                _dbContext.FormPhysicalProgressHelps.RemoveRange(formPhysicalProgressHelpInDb);
                _dbContext.SaveChanges();

                // Remove All FormPhysicalProgress form db
                _dbContext.FormPhysicalProgresses.RemoveRange(formPhysicalProgressesInDb);
                _dbContext.SaveChanges();
            }


            foreach (var jsonPhysicalProgress in jsonPhysicalProgressList)
            {
                //  برای ذخیره کمک بهزیستی از کد شماره صفر استفاده شده است
                // توی جاوا اسکریپت این کد رو برای راحتی در نظر گرفتم
                var behzistiHelpAmount = (jsonPhysicalProgress.CoOrganizationQuotaList.SingleOrDefault(c => c.CoOrganizationTypeId == 0)?.HelpAmount) ?? 0;

                var formPhysicalProgress = new FormPhysicalProgress
                {
                    FormId = form.Id,
                    PhysicalProgressId = jsonPhysicalProgress.PhysicalProgressId,
                    BehzistiHelpAmount = behzistiHelpAmount,
                };

                _dbContext.FormPhysicalProgresses.Add(formPhysicalProgress);
                _dbContext.SaveChanges();
                // اولین مورد مربوط به کمک بلاعوض بهزیستی است که باید از لیست پاک شود
                jsonPhysicalProgress.CoOrganizationQuotaList.RemoveAt(0);
                foreach (var jsonPhysicalProgressHelp in jsonPhysicalProgress.CoOrganizationQuotaList)
                {

                    var formPhysicalProgressHelp = new FormPhysicalProgressHelp
                    {
                        CoOrganizationTypeId = jsonPhysicalProgressHelp.CoOrganizationTypeId,
                        HelpAmount = jsonPhysicalProgressHelp.HelpAmount,
                        FormPhysicalProgress = formPhysicalProgress
                    };
                    formPhysicalProgress.FormPhysicalProgressHelps.Add(formPhysicalProgressHelp);
                }
            }

            _dbContext.SaveChanges();


            #endregion



            return RedirectToAction("Index");
        }
    }
}