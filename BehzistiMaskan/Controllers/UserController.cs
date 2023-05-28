using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using BehzistiMaskan.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BehzistiMaskan.Controllers
{
    [Authorize(Roles = RoleName.SystemAdministrator)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        private const string DefaultUserSignFolder = "~/Important/UserSigns";
        private const string DefaultTempUploadFolder = "~/TempUploadFile";

        private readonly string _canManageClientRoleId;
        public UserController()
        {
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
            _dbContext = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            var role = _dbContext.Roles.SingleOrDefault(r => r.Name == RoleName.CanManageClient);
            if (role == null)
            {
                var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                roleManager.Create(new IdentityRole(RoleName.CanManageClient));
                _canManageClientRoleId = _dbContext.Roles.Single(r => r.Name == RoleName.CanManageClient).Id;
            }
            else
            {
                _canManageClientRoleId = role.Id;
            }
        }

        // GET: User
        public ActionResult Index(MessageUserManageId? message)
        {

            if (message != null)
            {
                var strMsg = "";
                switch (message)
                {
                    case MessageUserManageId.UserCreatedSuccessfully:
                        strMsg = "کاربر با موفقیت در سامانه ثبت شد";
                        ViewBag.IsErrorMessage = false;
                        break;

                    case MessageUserManageId.ErrorWhileCreateUser:
                        strMsg = "هنگام ساخت کاربر خطایی رخ داده است. در صورت امکان با مدیر سیستم تماس بگیرید.";
                        ViewBag.IsErrorMessage = true;
                        break;
                    case MessageUserManageId.UserDeletedSuccessfully:
                        strMsg = "کاربر انتخاب شده با موفقیت از سامانه حذف گردید.";
                        ViewBag.IsErrorMessage = false;
                        break;
                    case MessageUserManageId.ErrorOnUserDelete:
                        strMsg = "هنگام حذف کاربر خطایی رخ داده است.";
                        ViewBag.IsErrorMessage = true;
                        break;
                    case null:
                    default:
                        break;
                }

                ViewBag.StatusMessage = strMsg;
            }

            var usersInDb = _dbContext.Users
                .Include(u => u.UserInfo)
                .Include(u => u.Roles)
                .Include(u => u.UserInfo)
                .Include(u => u.UserInfo.Province)
                .Include(u => u.UserInfo.County)
                .Include(u => u.UserInfo.AssistanceType)
                .Include(u => u.UserInfo.CoOrganizationType).ToList();

            var rolesListInDb = _dbContext.Roles.ToList();

            var userDtoList = new List<UserDto>();

            foreach (var user in usersInDb)
            {
                var userDto = new UserDto { Id = user.Id, UserName = user.UserName, UserInfo = user.UserInfo, UserRoles = new List<UserRoleDto>() };

                foreach (var role in user.Roles)
                {
                    var userRoleDto = new UserRoleDto
                    { Id = role.RoleId, Name = rolesListInDb.Single(r => r.Id == role.RoleId).Name };
                    userDto.UserRoles.Add(userRoleDto);
                }

                userDtoList.Add(userDto);
            }

            return View(userDtoList);
        }

        public ActionResult New()
        {


            var viewModel = new UserFormViewModel
            {
                Counties = _dbContext.Counties.Select(_mapper.Map<CountyDto>).ToList(),
                Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>).ToList(),
                AssistanceTypes = _dbContext.AssistanceTypes.ToList(),
                CoOrganizationTypes = _dbContext.CoOrganizationTypes.ToList(),
                UserRoles = _dbContext.Roles.Where(r => r.Name != RoleName.CanManageClient)
                    .AsEnumerable()
                    .Select(_mapper.Map<UserRoleDto>),
            };

            var prInDb = _dbContext.Provinces.ToList();

            return View("UserForm", viewModel);
        }

        public ActionResult Edit(string id)
        {
            var userInDb = _dbContext.Users.SingleOrDefault(u => u.UserName == id || u.Id == id);
            if (userInDb == null)
                return HttpNotFound();

            var userInfoInDb = _dbContext.UserInfos.SingleOrDefault(u => u.Id == userInDb.Id);

            var canManageClient = userInDb.Roles.SingleOrDefault(r => r.RoleId == _canManageClientRoleId) != null;

            var userRoleIds = userInDb.Roles.Select(r => r.RoleId);
            var userRoleNames = _dbContext.Roles.Where(r => r.Name != RoleName.CanManageClient && userRoleIds.Contains(r.Id)).Select(u => u.Name).ToList();

            var viewModel = new UserFormViewModel
            {
                Counties = _dbContext.Counties.Select(_mapper.Map<CountyDto>).AsEnumerable(),
                Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>),
                AssistanceTypes = _dbContext.AssistanceTypes.AsEnumerable(),
                CoOrganizationTypes = _dbContext.CoOrganizationTypes.AsEnumerable(),
                UserRoles = _dbContext.Roles.Where(r => r.Name != RoleName.CanManageClient)
                    .AsEnumerable()
                    .Select(_mapper.Map<UserRoleDto>),
                UserName = userInDb.UserName,
                UserId = userInDb.Id,
                UserInfo = userInfoInDb,
                CanManageClient = canManageClient,
                UserRoleNames = userRoleNames,
                IsSignUploaded = userInfoInDb != null && userInfoInDb.IsSignUploaded,
                Password = "passwordAlaki",
                ConfirmPassword = "passwordAlaki"
            };

            return View("UserForm", viewModel);
        }


        public ActionResult EditPassword(string id)
        {
            var userInDb = _dbContext.Users.SingleOrDefault(u => u.UserName == id || u.Id == id);
            if (userInDb == null)
                return HttpNotFound();

            var userInfoInDb = _dbContext.UserInfos.SingleOrDefault(u => u.Id == userInDb.Id);

            //var canManageClient = userInDb.Roles.SingleOrDefault(r => r.RoleId == _canManageClientRoleId) != null;

            //var userRoleIds = userInDb.Roles.Select(r => r.RoleId);
            //var userRoleNames = _dbContext.Roles.Where(r => r.Name != RoleName.CanManageClient && userRoleIds.Contains(r.Id)).Select(u => u.Name).ToList();

            var viewModel = new UserFormViewModel
            {
                UserName = userInDb.UserName,
                UserId = userInDb.Id,
                UserInfo = userInfoInDb,
                Password = "",
                ConfirmPassword = ""
            };

            return View(viewModel);
        }


        public ActionResult GetUserSignPhoto(string id)
        {
            var userInfoInDb =
                _dbContext.UserInfos.SingleOrDefault(u => u.Id == id);
            if (userInfoInDb == null || !userInfoInDb.IsSignUploaded)
                return null;

            return File(userInfoInDb.SignFullAddress, "image/png");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetNewPassword(UserFormViewModel viewModel)
        {
            ModelState.Remove("UserName");
            ModelState.Remove("UserRoleNames");
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditPassword", new { id = viewModel.UserId });
            }

            var user = await _userManager.FindByIdAsync(viewModel.UserId);

            if (user == null)
                return HttpNotFound();

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(viewModel.Password);

            var result = await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", result == IdentityResult.Success ? new { message = MessageUserManageId.UserCreatedSuccessfully } : new { message = MessageUserManageId.ErrorWhileCreateUser });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UserFormViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.UserId))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            if (!ModelState.IsValid)
            {
                viewModel.Counties = _dbContext.Counties.Select(_mapper.Map<CountyDto>).AsEnumerable();
                viewModel.Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>);
                viewModel.AssistanceTypes = _dbContext.AssistanceTypes.AsEnumerable();
                viewModel.CoOrganizationTypes = _dbContext.CoOrganizationTypes.AsEnumerable();
                viewModel.UserRoles = _dbContext.Roles.Where(r => r.Name != RoleName.CanManageClient)
                    .AsEnumerable()
                    .Select(_mapper.Map<UserRoleDto>);

                return View("UserForm", viewModel);
            }


            try
            {
                ApplicationUser user;
                UserInfo userInfoInDb;
                if (string.IsNullOrEmpty(viewModel.UserId))
                {
                    user = new ApplicationUser { UserName = viewModel.UserName };
                    var result = await _userManager.CreateAsync(user, viewModel.Password);

                    if (result.Succeeded)
                    {

                        // اضافه کردن گروه کاربر
                        foreach (var roleName in viewModel.UserRoleNames)
                        {
                            await _userManager.AddToRoleAsync(user.Id, roleName);
                        }


                        // در صورتی که مجوز ویرایش مددجو را دارد کاربر را به آن گروه اضافه کند
                        if (viewModel.CanManageClient)
                            await _userManager.AddToRoleAsync(user.Id, RoleName.CanManageClient);

                        viewModel.UserInfo.User = user;
                        _dbContext.UserInfos.Add(viewModel.UserInfo);

                        _dbContext.SaveChanges();

                        userInfoInDb = viewModel.UserInfo;


                    }
                    else // if create user RESULT has error
                    {
                        viewModel.Counties = _dbContext.Counties.Select(_mapper.Map<CountyDto>).AsEnumerable();
                        viewModel.Provinces = _dbContext.Provinces.Select(_mapper.Map<ProvinceDto>);
                        viewModel.AssistanceTypes = _dbContext.AssistanceTypes.AsEnumerable();
                        viewModel.CoOrganizationTypes = _dbContext.CoOrganizationTypes.AsEnumerable();
                        viewModel.UserRoles = _dbContext.Roles.Where(r => r.Name != RoleName.CanManageClient)
                            .AsEnumerable()
                            .Select(_mapper.Map<UserRoleDto>);
                        return View("UserForm", viewModel);
                    }

                    return RedirectToAction("Index", new { Message = MessageUserManageId.UserCreatedSuccessfully });

                } //end new User Branch
                else //edit current user
                {

                    user = _dbContext.Users.Include(u => u.UserInfo).SingleOrDefault(u => u.Id == viewModel.UserId);
                    if (user == null)
                        return HttpNotFound();

                    userInfoInDb = _dbContext.UserInfos.SingleOrDefault(u => u.Id == viewModel.UserId) ?? new UserInfo { User = user };

                    user.UserName = viewModel.UserName;
                    userInfoInDb.Name = viewModel.UserInfo.Name;
                    userInfoInDb.Family = viewModel.UserInfo.Family;
                    userInfoInDb.Mobile = viewModel.UserInfo.Mobile;
                    userInfoInDb.AssistanceTypeId = viewModel.UserInfo.AssistanceTypeId;
                    userInfoInDb.CoOrganizationTypeId = viewModel.UserInfo.CoOrganizationTypeId;
                    userInfoInDb.ProvinceId = viewModel.UserInfo.ProvinceId;
                    userInfoInDb.CountyId = viewModel.UserInfo.CountyId;

                    if (user.UserInfo == null)
                        _dbContext.UserInfos.Add(userInfoInDb);
                    _dbContext.SaveChanges();

                    var userRoles = (await _userManager.GetRolesAsync(user.Id)).ToArray();
                    var result = await _userManager.RemoveFromRolesAsync(user.Id, userRoles);

                    result = await _userManager.AddToRolesAsync(user.Id, viewModel.UserRoleNames.ToArray());
                    if (viewModel.CanManageClient)
                        result = await _userManager.AddToRoleAsync(user.Id, RoleName.CanManageClient);

                }


                if (viewModel.TempImageId != null)
                {
                    var tempImage = _dbContext.TemporaryImages.SingleOrDefault(t => t.Id == viewModel.TempImageId);
                    if (tempImage != null)
                    {
                        // Save File
                        var newAddress = Server.MapPath(DefaultUserSignFolder);
                        if (!Directory.Exists(newAddress))
                            Directory.CreateDirectory(newAddress);

                        newAddress = Path.Combine(newAddress, $"Sign-{user.Id}{Path.GetExtension(tempImage.MasterFileName ?? ".png")}");

                        System.IO.File.Move(tempImage.TemporaryFileName, newAddress);

                        _dbContext.TemporaryImages.Remove(tempImage);
                        _dbContext.SaveChanges();

                        userInfoInDb.IsSignUploaded = true;
                        userInfoInDb.SignFullAddress = newAddress;
                        _dbContext.SaveChanges();

                    }
                }
                else if (userInfoInDb.IsSignUploaded)
                {
                    if (System.IO.File.Exists(userInfoInDb.SignFullAddress))
                        System.IO.File.Delete(userInfoInDb.SignFullAddress);

                    userInfoInDb.SignFullAddress = null;
                    userInfoInDb.IsSignUploaded = false;

                    _dbContext.SaveChanges();
                }

                return RedirectToAction("Index", new { Message = MessageUserManageId.UserCreatedSuccessfully });
            }
            catch
            {
                return RedirectToAction("Index", new { Message = MessageUserManageId.ErrorWhileCreateUser });
            }
        }


        public ActionResult DeleteUser(string id)
        {
            var userInDb = _dbContext.Users
                .Include(u => u.UserInfo)
                .SingleOrDefault(u => u.Id == id);
            if (userInDb == null)
                return HttpNotFound();

            var viewModel = new UserFormViewModel
            {
                UserId = userInDb.Id,
                UserName = userInDb.UserName,
                UserInfo = userInDb.UserInfo
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserPost(string userId)
        {
            var userInDb = _dbContext.Users
                .Include(u => u.UserInfo)
                .SingleOrDefault(u => u.Id == userId);
            if (userInDb == null)
                return HttpNotFound();

            var result = _userManager.Delete(userInDb);
            if (result.Succeeded)
                return RedirectToAction("Index", new { Message = MessageUserManageId.UserDeletedSuccessfully });

            return RedirectToAction("Index", new { Message = MessageUserManageId.ErrorOnUserDelete });
        }

        public ActionResult IsUserNameExist(string userName, string userId)
        {
            var usersInDb = _dbContext.Users.SingleOrDefault(u => u.UserName == userName && u.Id != userId);
            return Json(usersInDb == null, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public string UploadTempSignImage(TempImageDto tempImageDto)
        {
            //if file is not empty or null
            if (tempImageDto.image != null && tempImageDto.image.ContentLength > 0)
            {
                //check image size
                if (tempImageDto.image.ContentLength > 1150000)
                {
                    return "Error#FileIsBig";
                }

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
                if (!Directory.Exists(initialPath))
                {
                    Directory.CreateDirectory(initialPath);
                }

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
                    MasterFileName = masterFileName,
                    TemporaryFileName = tempFileNameFullPath
                };

                _dbContext.TemporaryImages.Add(tempImage);

                _dbContext.SaveChanges();

                return $"Created#{tempImage.Id}";
            }

            return "Error#InvalidFile";
        }


        public enum MessageUserManageId
        {
            UserCreatedSuccessfully,
            ErrorWhileCreateUser,
            UserDeletedSuccessfully,
            ErrorOnUserDelete
        }
    }
}