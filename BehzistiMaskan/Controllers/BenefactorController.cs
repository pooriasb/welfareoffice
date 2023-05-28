using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using Microsoft.AspNet.Identity;
using MD.PersianDateTime;
namespace BehzistiMaskan.Controllers
{
    public class BenefactorController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public enum MessageBenefactorManageId
        {
            LoginError,
            Error,
            DataSavedSuccessfully,
            ValidationError,
            DuplicateNationalCode,
            DeleteSuccessfully
        }

        private readonly IMapper _mapper;

        public BenefactorController()
        {
            _dbContext = new ApplicationDbContext();
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
        }

        // GET: Benefactor
        public ActionResult Index(MessageBenefactorManageId? message)
        {
            var status = new Status();
            switch (message)
            {
                case MessageBenefactorManageId.LoginError:
                    status.StatusType = ModelEnums.StatusTypeE.Error;
                    break;
                case MessageBenefactorManageId.DataSavedSuccessfully:
                    status.StatusType = ModelEnums.StatusTypeE.Success;
                    break;
                case MessageBenefactorManageId.ValidationError:
                    status.StatusType = ModelEnums.StatusTypeE.ValidationError;
                    break;
                case MessageBenefactorManageId.DuplicateNationalCode:
                    status.StatusType = ModelEnums.StatusTypeE.DuplicateNationalCode;
                    break;
                default:
                    status = null;
                    break;
            }

            ViewBag.Status = status;

            using (var context = new ApplicationDbContext())
            {

                ViewBag.RequiredMessage = context.RequiredMessage2s.Include(x => x.County).Include(x=>x.County.Province).OrderBy(x=>x.CountyId).ToList();

            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string nationalCode, string mobile)
        {
            if (string.IsNullOrEmpty(nationalCode) || string.IsNullOrEmpty(mobile))
                return RedirectToAction("Index");

            var benefactorInDb =
                _dbContext.Benefactors.SingleOrDefault(b => b.NationalCode == nationalCode && b.Mobile == mobile);
            if (benefactorInDb == null)
            {
                return RedirectToAction("Index", new { message = MessageBenefactorManageId.LoginError });
            }


            var viewModel = new BenefactorViewModel
            {
                GenderTypes = _dbContext.GenderTypes.Take(2).AsEnumerable(),
                MarriageTypes = _dbContext.MarriageTypes.Take(2).AsEnumerable(),
                Benefactor = _mapper.Map<BenefactorDto>(benefactorInDb),
                Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList(),
                Counties = _dbContext.Counties.Where(c => c.ProvinceId == benefactorInDb.ProvinceId).AsEnumerable().Select(_mapper.Map<CountyDto>),
            };

            return View("BenefactorForm", viewModel);
        }

        public ActionResult New()
        {
            var viewModel = new BenefactorViewModel
            {
                GenderTypes = _dbContext.GenderTypes.Take(2).AsEnumerable(),
                MarriageTypes = _dbContext.MarriageTypes.Take(2).AsEnumerable(),
                Benefactor = new BenefactorDto(),
                Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList(),
                Counties = new List<CountyDto>(),

            };

            return View("BenefactorForm", viewModel);

        }


        public ActionResult ThankYou()
        {
            return View();
        }

        [Authorize]
        // لیست خیرین ثبت نام کرده
        public ActionResult List(MessageBenefactorManageId? message)
        {
            if (message != null)
            {
                var status = new Status();
                switch (message)
                {
                    case MessageBenefactorManageId.LoginError:
                        status.StatusType = ModelEnums.StatusTypeE.Error;
                        break;
                    case MessageBenefactorManageId.Error:
                        status.StatusType = ModelEnums.StatusTypeE.Error;
                        break;
                    case MessageBenefactorManageId.DataSavedSuccessfully:
                        status.StatusType = ModelEnums.StatusTypeE.Success;
                        break;
                    case MessageBenefactorManageId.ValidationError:
                        status.StatusType = ModelEnums.StatusTypeE.ValidationError;
                        break;
                    case MessageBenefactorManageId.DuplicateNationalCode:
                        status.StatusType = ModelEnums.StatusTypeE.DuplicateNationalCode;
                        break;
                    case MessageBenefactorManageId.DeleteSuccessfully:
                        status.StatusType = ModelEnums.StatusTypeE.DeletedSuccessful;
                        break;
                    default:
                        status = null;
                        break;
                }

                ViewBag.Status = status;
            }
            return View("BenefactorServerSideList");
        }

        [Authorize(Roles = RoleName.KarshenasKeshvar + "," + RoleName.KarshenasOstan + "," + RoleName.KarshenasShahrestan)]
        public ActionResult Edit(int id)
        {
            var benefactorInDb = _dbContext.Benefactors.SingleOrDefault(b => b.Id == id);
            if (benefactorInDb == null)
                return HttpNotFound();

            var viewModel = new BenefactorViewModel
            {
                Benefactor = _mapper.Map<BenefactorDto>(benefactorInDb),
                GenderTypes = _dbContext.GenderTypes.Take(2).AsEnumerable(),
                MarriageTypes = _dbContext.MarriageTypes.Take(2).AsEnumerable(),
                Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList(),
                Counties = _dbContext.Counties.Where(c => c.ProvinceId == benefactorInDb.ProvinceId).AsEnumerable().Select(_mapper.Map<CountyDto>),

            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BenefactorViewModel benefactorDto)
        {
            if (benefactorDto.Birthdate != null)
            {
                PersianDateTime pDate;
                var isCorrectDate = PersianDateTime.TryParse(benefactorDto.Birthdate, out pDate);
                benefactorDto.Benefactor.Birthdate = pDate;
            }
            
            if (!ModelState.IsValid)
            {
                var viewModel = new BenefactorViewModel
                {
                    GenderTypes = _dbContext.GenderTypes.Take(2).AsEnumerable(),
                    MarriageTypes = _dbContext.MarriageTypes.Take(2).AsEnumerable(),
                    Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList(),
                    Counties = _dbContext.Counties.Where(c => c.ProvinceId == benefactorDto.Benefactor.ProvinceId).AsEnumerable().Select(_mapper.Map<CountyDto>),
                };

                return View("BenefactorForm", viewModel);
            }

            var isEdit = benefactorDto.Benefactor.Id != 0;


            if (!isEdit)
            {
                var benefactorInDb =
                    _dbContext.Benefactors.SingleOrDefault(b => b.NationalCode == benefactorDto.Benefactor.NationalCode);
                if (benefactorInDb != null)
                {
                    // فردی قبلا با این کد ملی ثبت نام کرده است
                    return RedirectToAction("Index", new { message = MessageBenefactorManageId.DuplicateNationalCode });
                }

                benefactorInDb = _mapper.Map<Benefactor>(benefactorDto.Benefactor);
                _dbContext.Benefactors.Add(benefactorInDb);
            }
            else
            {
                var benefactorInDb = _dbContext.Benefactors.SingleOrDefault(b => b.Id == benefactorDto.Benefactor.Id);
                if (benefactorInDb == null)
                    return HttpNotFound();

                if (benefactorInDb.NationalCode != benefactorDto.Benefactor.NationalCode)
                {
                    // اقدام به تعویض کد ملی خود نموده است
                    // بنابراین باید کنترل شود که کد ملی فرد تکراری نباشد
                    var benefactorWithThisNationalCodeAndDifferentIdInDb = _dbContext.Benefactors.SingleOrDefault(b =>
                        b.Id != benefactorInDb.Id && b.NationalCode == benefactorDto.Benefactor.NationalCode);

                    if (benefactorWithThisNationalCodeAndDifferentIdInDb != null)
                        return RedirectToAction("Index",
                            new
                            {
                                messsage = MessageBenefactorManageId.DuplicateNationalCode
                            });
                }
                _mapper.Map(benefactorDto.Benefactor, benefactorInDb);
            }

            _dbContext.SaveChanges();

            if (IsAuthenticated)
            {
                // باید کد مربوط به فعالیت کاربر در اینجا نوشته شود
                return RedirectToAction("List", new { message = MessageBenefactorManageId.DataSavedSuccessfully });
            }


            return isEdit
                ? RedirectToAction("ThankYou",
                    new
                    {
                        message = MessageBenefactorManageId.DataSavedSuccessfully
                    })
                : RedirectToAction("ThankYou", new
                {
                    message = MessageBenefactorManageId.DataSavedSuccessfully
                });
        }

        [Authorize(Roles = RoleName.KarshenasOstan)]
        public ActionResult Delete(int id)
        {
            var benefactorInDb = _dbContext.Benefactors
                .Include(b => b.Province)
                .Include(b => b.County)
                .SingleOrDefault(b => b.Id == id);

            if (benefactorInDb == null)
                return HttpNotFound();

            var userId = User.Identity.GetUserId();
            var userInfoInDb = _dbContext.UserInfos.Single(u => u.Id == userId);

            if (benefactorInDb.ProvinceId != userInfoInDb.ProvinceId)
            {
                return RedirectToAction("List", new { message = MessageBenefactorManageId.Error });
            }

            return View(benefactorInDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var benefactorInDb = _dbContext.Benefactors.SingleOrDefault(b => b.Id == id);

            if (benefactorInDb == null)
                return HttpNotFound();

            _dbContext.Benefactors.Remove(benefactorInDb);
            _dbContext.SaveChanges();

            return RedirectToAction("List", new { message = MessageBenefactorManageId.DeleteSuccessfully });
        }






        [Authorize(Roles = RoleName.KarshenasKeshvar + "," + RoleName.KarshenasOstan)]
        public ActionResult EditRequiredMessage(MessageBenefactorManageId? resMessage)
        {


            var status = new Status();
            switch (resMessage)
            {
                case MessageBenefactorManageId.LoginError:
                    status.StatusType = ModelEnums.StatusTypeE.Error;
                    break;
                case MessageBenefactorManageId.DataSavedSuccessfully:
                    status.StatusType = ModelEnums.StatusTypeE.Success;
                    break;
                case MessageBenefactorManageId.ValidationError:
                    status.StatusType = ModelEnums.StatusTypeE.ValidationError;
                    break;
                case MessageBenefactorManageId.DuplicateNationalCode:
                    status.StatusType = ModelEnums.StatusTypeE.DuplicateNationalCode;
                    break;
                default:
                    status = null;
                    break;
            }

            ViewBag.Status = status;

           // RequiredMessage2 requiredMessage;
            //using (var context = new ApplicationDbContext())
            //{
            //    requiredMessage = context.RequiredMessage2s.SingleOrDefault() ?? new RequiredMessage2();

            //}

            ViewBag.Provinces = _dbContext.Provinces.AsEnumerable();
            ViewBag.RecentMessages = _dbContext.RequiredMessage2s.Include(x=>x.County).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRequiredMessage(RequiredMessage2 requiredMessage, int countyId)
        {
          //  ModelState.Remove("UpdateDate");
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                ViewBag.Status = ModelEnums.StatusTypeE.Error;
                return View("EditRequiredMessage", requiredMessage);
            }

            using (var context = new ApplicationDbContext())
            {
             

              
                 var   requiredMessageInDb = new RequiredMessage2
                    {
                        Message = requiredMessage.Message,
                        CreateDateTime = DateTime.Now,
                        CountyId = countyId
                    };
                    context.RequiredMessage2s.Add(requiredMessageInDb);
              
                context.SaveChanges();
            }
            ViewBag.Provinces = _dbContext.Provinces.AsEnumerable();

            return RedirectToAction("EditRequiredMessage", new { ResMessage = MessageBenefactorManageId.DataSavedSuccessfully });
        }


        public ActionResult DeleteRequiredMessage(int id)
        {
            _dbContext.RequiredMessage2s.Remove(_dbContext.RequiredMessage2s.Find(id));
            _dbContext.SaveChanges();

            return RedirectToAction("EditRequiredMessage",
                new {ResMessage = MessageBenefactorManageId.DeleteSuccessfully});
        }




        public bool IsAuthenticated => User?.Identity != null && User.Identity.IsAuthenticated;
    }
}