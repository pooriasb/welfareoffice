using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Http;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.DataTable;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using Microsoft.AspNet.Identity;

namespace BehzistiMaskan.Controllers.api
{
    [RoutePrefix("api/clients")]
    //[Authorize(Roles = RoleName.ModirKolOstan + "," + RoleName.KarshenasOstan + "," + RoleName.KarshenasMasoolOstan + "," + RoleName.KarshenasShahrestan)]
    [Authorize]
    public class ClientsController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ClientsController()
        {
            _dbContext = new ApplicationDbContext();
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _dbContext.Dispose();
            base.Dispose(disposing);
        }

        // GET /api/clients
        public IEnumerable<ClientDto> GetClients()
        {
            return _dbContext.Clients.Include(c => c.Person)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Where(c => c.IsDeleted == null || c.IsDeleted == false)
                .AsEnumerable()
                .Select(_mapper.Map<Client, ClientDto>);
        }

        [Route("clientdatatable")]
        [HttpPost]
        public IHttpActionResult GetClientsDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data ?? "Id";

                //if (sortBy.Contains("Person"))
                //    sortBy = sortBy.Replace("Person", "Person.");
                //else

                if (sortBy.Contains("County"))
                    sortBy = sortBy.Replace("County", "City.District.County.");
                else if (sortBy.Contains("District"))
                    sortBy = sortBy.Replace("District", "City.District.");
                else if (sortBy.Contains("City"))
                    sortBy = sortBy.Replace("City", "City.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            var clientsInDb = _dbContext.Clients.Include(c => c.Person)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Where(c => c.IsDeleted != true);

            var userId = User.Identity.GetUserId();

            var userInfoInDb = _dbContext.UserInfos.Include(uInf => uInf.CoOrganizationType)
                .Include(u => u.Province)
                .Include(u => u.County)
                .Single(uInf => uInf.User.Id == userId);
            var provinceId = userInfoInDb.ProvinceId;
            var countyId = userInfoInDb.CountyId;

            if (User.IsInRole(RoleName.KarshenasOstan))
            {
                clientsInDb = clientsInDb.Where(c => c.City.District.County.ProvinceId == provinceId);
            }

            if (User.IsInRole(RoleName.KarshenasShahrestan))
            {

                //فقط کسانی که در شهرستان خودش هستند را می تواند مشاهده کند
                clientsInDb = clientsInDb.Where(c => c.City.District.CountyId == countyId);
            }


            if (User.IsInRole(RoleName.SazmanHamkar))
            {

                var coOrganizationTypeId = userInfoInDb.CoOrganizationTypeId;


                // فقط استان خودش
                clientsInDb = clientsInDb.Where(c => c.City.District.County.ProvinceId == userInfoInDb.ProvinceId);

                // اگر کاربر شهرستان بود فقط مربوط به شهرستان خودش
                if (userInfoInDb.CountyId != null)
                {
                    clientsInDb = clientsInDb.Where(c => c.City.District.CountyId == userInfoInDb.CountyId);
                }

                // مددجویانی که در طرحی ثبت شده باشند و سازمان این کاربر در آن طرح همکار بهزیستی باشد
                clientsInDb = clientsInDb.Where(c => c.ClientForms.Any(cf => cf.Form.FormCoOrganizationRoles.Any(fco => fco.CoOrganizationTypeId == coOrganizationTypeId)));

                //var formIdsByCoOrganization = _dbContext.FormCoOrganizationRoles
                //    .Where(fc => fc.CoOrganizationTypeId == coOrganizationTypeId).Select(fc => fc.FormId);
                //var clientIdsInScopeOfSazmanHamkarUser = _dbContext.ClientForms.Where(cf => formIdsByCoOrganization.Contains(cf.FormId)).Select(cf => cf.ClientId);
                //clientsInDb = clientsInDb.Where(c => clientIdsInScopeOfSazmanHamkarUser.Contains(c.Id));

            }


            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {
                    clientsInDb = clientsInDb.Where(c => c.Person.Name.ToLower().Contains(searchWord) ||
                                                         c.Person.Family.ToLower().Contains(searchWord) ||
                                                         c.Person.NationalCode.ToLower().Contains(searchWord) ||
                                                         c.City.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.County.Name.ToLower().Contains(searchWord) ||
                                                         c.Id.ToString().Contains(searchWord));
                }

            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = clientsInDb.Count();
            var totalResultsCount = _dbContext.Clients.Count(c => c.IsDeleted != true);

            var searchResult = string.IsNullOrEmpty(sortBy) ?
                (take != -1 ? clientsInDb.OrderBy("Id asc").Skip(skip).Take(take).AsEnumerable().Select(_mapper.Map<ClientDto>).ToList() :
                    clientsInDb.OrderBy("Id asc").AsEnumerable().Select(_mapper.Map<ClientDto>).ToList())
                : take != -1 ?
                clientsInDb.OrderBy(sortBy).Skip(skip).Take(take).AsEnumerable().Select(_mapper.Map<ClientDto>).ToList() :
                clientsInDb.OrderBy(sortBy).AsEnumerable().Select(_mapper.Map<ClientDto>).ToList()
                ;

            var userNationalCodeList = _dbContext.ClientUsers.Select(u => u.NationalCode).ToList();

            foreach (var clientDto in searchResult)
            {
                if (userNationalCodeList.Contains(clientDto.Person.NationalCode))
                {
                    clientDto.HasClientUser = true;
                }
            }

            var output = Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = searchResult
            });

            return output;
        }

        [Route("deletedclientdatatable")]
        [HttpPost]
        public IHttpActionResult GetDeletedClientsDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data;

                if (sortBy.Contains("Person"))
                    sortBy = sortBy.Replace("Person", "Person.");
                else if (sortBy.Contains("County"))
                    sortBy = sortBy.Replace("County", "City.District.County.");
                else if (sortBy.Contains("District"))
                    sortBy = sortBy.Replace("District", "City.District.");
                else if (sortBy.Contains("City"))
                    sortBy = sortBy.Replace("City", "City.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            var clientsInDb = _dbContext.Clients
                .Include(c => c.Person)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.ClientState)
                .Where(c => c.IsDeleted == true || c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.IsIllegalPersonAndNotABehzistiClient);

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {
                    clientsInDb = clientsInDb.Where(c => c.Person.Name.ToLower().Contains(searchWord) ||
                                                         c.Person.Family.ToLower().Contains(searchWord) ||
                                                         c.Person.NationalCode.ToLower().Contains(searchWord) ||
                                                         c.City.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.County.Name.ToLower().Contains(searchWord) ||
                                                         c.Id.ToString().Contains(searchWord));
                }

            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = clientsInDb.Count();
            var totalResultsCount = _dbContext.Clients.Count(c => c.IsDeleted == true);

            var searchResult = clientsInDb
                .OrderBy(sortBy)
                .Skip(skip)
                .Take(take)
                .AsEnumerable()
                .Select(_mapper.Map<ClientDto>)
                .ToList();

            var userNationalCodeList = _dbContext.ClientUsers.Select(u => u.NationalCode).ToList();

            foreach (var clientDto in searchResult)
            {
                if (userNationalCodeList.Contains(clientDto.Person.NationalCode))
                {
                    clientDto.HasClientUser = true;
                }
            }

            var output = Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = searchResult
            });

            return output;
        }

        [Route("deletedwaitingdatatable")]
        [HttpPost]
        public IHttpActionResult GetDeletedWaitingApplicantDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data;

                if (sortBy.Contains("County"))
                    sortBy = sortBy.Replace("County", "City.District.County.");
                else if (sortBy.Contains("District"))
                    sortBy = sortBy.Replace("District", "City.District.");
                else if (sortBy.Contains("City"))
                    sortBy = sortBy.Replace("City", "City.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            var deletedApplicantsInDb = _dbContext.ClientWaitingApplicants
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Where(c => c.IsDeleted);

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {
                    deletedApplicantsInDb = deletedApplicantsInDb.Where(c => c.Name.ToLower().Contains(searchWord) ||
                                                         c.Family.ToLower().Contains(searchWord) ||
                                                         c.NationalCode.ToLower().Contains(searchWord) ||
                                                         c.City.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.County.Name.ToLower().Contains(searchWord) ||
                                                         c.Id.ToString().Contains(searchWord));
                }

            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = deletedApplicantsInDb.Count();
            var totalResultsCount = _dbContext.Clients.Count(c => c.IsDeleted == true);

            var searchResult = deletedApplicantsInDb
                .OrderBy(sortBy)
                .Skip(skip)
                .Take(take)
                .AsEnumerable()
                .Select(_mapper.Map<ClientDto>)
                .ToList();

            var userNationalCodeList = _dbContext.ClientUsers.Select(u => u.NationalCode).ToList();

            foreach (var clientDto in searchResult)
            {
                if (userNationalCodeList.Contains(clientDto.Person.NationalCode))
                {
                    clientDto.HasClientUser = true;
                }
            }

            var output = Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = searchResult
            });

            return output;
        }

        [Route("clientwaitinglistdatatable")]
        [HttpPost]
        public IHttpActionResult GetClientsWaitingListDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data;

                sortBy += " " + model.Order[0].dir.ToLower();
            }

            var clientWaitingListInDb = _dbContext.ClientWaitingApplicants
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.Requests)
                .Include(c => c.Requests.Select(r => r.RequestType))
                .Where(c => c.IsDeleted == false);

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchStr in searchWords)
                {
                    clientWaitingListInDb = clientWaitingListInDb.Where(c => c.Name.Contains(searchStr) ||
                                                                             c.Family.Contains(searchStr) ||
                                                                             c.FatherName.Contains(searchStr) ||
                                                                             c.NationalCode.Contains(searchStr));
                }

            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = clientWaitingListInDb.Count();
            var totalResultsCount = _dbContext.ClientWaitingApplicants.Count();

            //var searchResult = clientWaitingListInDb
            //    .OrderBy(sortBy)
            //    .Skip(skip)
            //    .Take(take)
            //    .AsEnumerable()
            //    .Select(_mapper.Map<ClientWaitingApplicantSimpleDto>)
            //    .ToList();

            var searchResult = string.IsNullOrEmpty(sortBy) ?
                    (take != -1 ? clientWaitingListInDb.OrderBy("Id asc").Skip(skip).Take(take).AsEnumerable().Select(_mapper.Map<ClientWaitingApplicantSimpleDto>).ToList() :
                        clientWaitingListInDb.OrderBy("Id asc").AsEnumerable().Select(_mapper.Map<ClientWaitingApplicantSimpleDto>).ToList())
                    : take != -1 ?
                        clientWaitingListInDb.OrderBy(sortBy).Skip(skip).Take(take).AsEnumerable().Select(_mapper.Map<ClientWaitingApplicantSimpleDto>).ToList() :
                        clientWaitingListInDb.OrderBy(sortBy).AsEnumerable().Select(_mapper.Map<ClientWaitingApplicantSimpleDto>).ToList()
                ;
            var userNationalCodeList = _dbContext.ClientUsers.Select(u => u.NationalCode).ToList();

            foreach (var clientWaitingApplicant in searchResult)
            {
                if (userNationalCodeList.Contains(clientWaitingApplicant.NationalCode))
                {
                    clientWaitingApplicant.HasClientUser = true;
                }
            }

            var output = Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = searchResult
            });

            return output;
        }


        [Route("clientexemptionlistdatatable")]
        [HttpPost]
        public IHttpActionResult GetClientsExemptionListDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data ?? "Id";

                //if (sortBy.Contains("Person"))
                //    sortBy = sortBy.Replace("Person", "Person.");
                //else

                if (sortBy.Contains("County"))
                    sortBy = sortBy.Replace("County", "City.District.County.");
                else if (sortBy.Contains("District"))
                    sortBy = sortBy.Replace("District", "City.District.");
                else if (sortBy.Contains("City"))
                    sortBy = sortBy.Replace("City", "City.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            // فقط لیست مددجویانی که تیک یکی از گزینه های معافیت انشعابات را زده باشند
            var clientsInDbByExemption = _dbContext.Clients.Include(c => c.Person)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.ClientRequests)
                .Include(c => c.ClientRequests.Select(cr => cr.RequestType))
                .Where(c => c.IsDeleted != true && c.ClientRequests.Any(cr => cr.RequestType.IsExemption));

            var userId = User.Identity.GetUserId();

            var userInfoInDb = _dbContext.UserInfos.Include(uInf => uInf.CoOrganizationType)
                .Include(u => u.Province)
                .Include(u => u.County)
                .Single(uInf => uInf.User.Id == userId);
            var provinceId = userInfoInDb.ProvinceId;
            var countyId = userInfoInDb.CountyId;

            if (User.IsInRole(RoleName.KarshenasOstan))
            {
                clientsInDbByExemption = clientsInDbByExemption.Where(c => c.City.District.County.ProvinceId == provinceId);
            }

            if (User.IsInRole(RoleName.KarshenasShahrestan))
            {
                //فقط کسانی که در شهرستان خودش هستند را می تواند مشاهده کند
                clientsInDbByExemption = clientsInDbByExemption.Where(c => c.City.District.CountyId == countyId);
            }


            if (User.IsInRole(RoleName.SazmanHamkar))
            {

                // فقط استان خودش
                clientsInDbByExemption = clientsInDbByExemption.Where(c => c.City.District.County.ProvinceId == userInfoInDb.ProvinceId);

                // اگر کاربر شهرستان بود فقط مربوط به شهرستان خودش
                if (userInfoInDb.CountyId != null)
                {
                    clientsInDbByExemption = clientsInDbByExemption.Where(c => c.City.District.CountyId == userInfoInDb.CountyId);
                }

                // در این قسمت از کد چک میکنیم که هر سازمان همکار فقط مددجویانی که درخواست معافیت آن سازمان را دارند 
                // بتوانند نام او را مشاهده کنند
                var allRequestTypeInDb = _dbContext.RequestTypes.Where(r => r.IsExemption).ToList();
                int requestTypeId;
                switch (userInfoInDb.CoOrganizationTypeId)
                {
                    case (int)ModelEnums.CoOrganizationType.EdareyeAb:
                        requestTypeId = allRequestTypeInDb.Single(r => r.Name == ClientRequestTypeStr.ExemptionWater).Id;
                        break;
                    case (int)ModelEnums.CoOrganizationType.EdareyeBargh:
                        requestTypeId = allRequestTypeInDb.Single(r => r.Name == ClientRequestTypeStr.ExemptionElectrical).Id;
                        break;
                    case (int)ModelEnums.CoOrganizationType.EdareyeGas:
                        requestTypeId = allRequestTypeInDb.Single(r => r.Name == ClientRequestTypeStr.ExemptionGas).Id;
                        break;
                    case (int)ModelEnums.CoOrganizationType.ShahrdariVaDehdari:
                    default:
                        requestTypeId = allRequestTypeInDb.Single(r => r.Name == ClientRequestTypeStr.ExemptionProductionLicense).Id;
                        break;
                }
                // هر سازمان همکار فقط می تواند لیست مددجویانی که تیک متقاضی معافیت انشعابات آن سازمان را زده باشند مشاهده کند
                // مثلا کارمند اداره آب فقط مددجویان متقاضی معافیت آب را مشاهده می نمایند
                clientsInDbByExemption = clientsInDbByExemption.Where(c => c.ClientRequests.Any(cr => cr.RequestTypeId == requestTypeId));

            }

            // هنگام جستجوی مددجو، اولین کلمه بالاترین اولویت را دارد
            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {
                    clientsInDbByExemption = clientsInDbByExemption.Where(c => c.Person.Name.ToLower().Contains(searchWord) ||
                                                         c.Person.Family.ToLower().Contains(searchWord) ||
                                                         c.Person.NationalCode.ToLower().Contains(searchWord) ||
                                                         c.City.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.County.Name.ToLower().Contains(searchWord) ||
                                                         c.Id.ToString().Contains(searchWord));
                }
            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = clientsInDbByExemption.Count();
            var totalResultsCount = _dbContext.Clients.Count(c => c.IsDeleted != true && c.ClientRequests.Any(cr => cr.RequestType.IsExemption));

            //var searchResult = string.IsNullOrEmpty(sortBy) ?
            //    clientsInDbByExemption
            //        .OrderBy("Id asc")
            //        .Skip(skip)
            //        .Take(take)
            //        .AsEnumerable()
            //        .Select(_mapper.Map<ClientDto>)
            //        .ToList()
            //    :
            //clientsInDbByExemption
            //    .OrderBy(sortBy)
            //    .Skip(skip)
            //    .Take(take)
            //    .AsEnumerable()
            //    .Select(_mapper.Map<ClientDto>)
            //    .ToList();


            var searchResult = string.IsNullOrEmpty(sortBy) ?
                    (take != -1 ? clientsInDbByExemption.OrderBy("Id asc").Skip(skip).Take(take).AsEnumerable().Select(_mapper.Map<ClientDto>).ToList() :
                        clientsInDbByExemption.OrderBy("Id asc").AsEnumerable().Select(_mapper.Map<ClientDto>).ToList())
                    : take != -1 ?
                        clientsInDbByExemption.OrderBy(sortBy).Skip(skip).Take(take).AsEnumerable().Select(_mapper.Map<ClientDto>).ToList() :
                        clientsInDbByExemption.OrderBy(sortBy).AsEnumerable().Select(_mapper.Map<ClientDto>).ToList()
                ;

            var userNationalCodeList = _dbContext.ClientUsers.Select(u => u.NationalCode).ToList();

            foreach (var clientDto in searchResult.Where(clientDto => userNationalCodeList.Contains(clientDto.Person.NationalCode)))
            {
                clientDto.HasClientUser = true;
            }
            
            //foreach (var clientDto in searchResult)
            //{
            //    if (userNationalCodeList.Contains(clientDto.Person.NationalCode))
            //    {
            //        clientDto.HasClientUser = true;
            //    }
            //}

            //var userNationalCodeList = _dbContext.ClientUsers.Select(u => u.NationalCode).ToList();

            //foreach (var clientDto in searchResult)
            //{
            //    if (userNationalCodeList.Contains(clientDto.Person.NationalCode))
            //    {
            //        clientDto.HasClientUser = true;
            //    }
            //}

            var output = Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = searchResult
            });

            return output;
        }


        [Route("cartabledatatable")]
        [HttpPost]
        public IHttpActionResult GetCartableDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data;

                if (sortBy.Contains("Person"))
                    sortBy = sortBy.Replace("Person", "Person.");
                else if (sortBy.Contains("County"))
                    sortBy = sortBy.Replace("County", "City.District.County.");
                else if (sortBy.Contains("District"))
                    sortBy = sortBy.Replace("District", "City.District.");
                else if (sortBy.Contains("City"))
                    sortBy = sortBy.Replace("City", "City.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            var clientsInDb = _dbContext.Clients.Include(c => c.Person)
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.ClientState)
                .Include(c => c.ClientForms)
                .Include(c => c.ClientForms.Select(cf => cf.Form))
                .Include(c => c.ClientForms.Select(cf => cf.Form.FormCoOrganizationRoles))
                .Where(c => c.IsDeleted != true);

            var userId = User.Identity.GetUserId();

            var userInfoInDb = _dbContext.UserInfos.Include(uInf => uInf.CoOrganizationType)
                .Include(u => u.Province)
                .Include(u => u.County)
                .Single(uInf => uInf.User.Id == userId);
            var provinceId = userInfoInDb.ProvinceId;
            var countyId = userInfoInDb.CountyId;



            // کارتابل کارشناس استان
            if (User.IsInRole(RoleName.KarshenasOstan))
            {
                // فقط مددجویان موجود در استان کاربر به او نمایش داده خواهد شد
                clientsInDb = clientsInDb.Where(c => c.City.District.County.ProvinceId == provinceId);


                // افرادی که باید در کارتابل کارشناس استان مشاهده شوند در فایل 
                // ModelEnums.cs
                // در بخش کارشناس استان توضیح داده شده اند

                clientsInDb = clientsInDb.Where(c => c.ClientState != null &&
                                                     c.ClientState.ClientStateTypeE ==
                                                     ModelEnums.ClientStateTypeE.InWaitingListKarshenasOstan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .InWaitingListSazmanHamkarOstan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .ApproveBySazmanHamkarOstan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .DenyBySazmanHamkarOstan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .ApproveByAllOfSazmanHamkarOstan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .InWaitingListFormEmtiazBandi ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .ApproveAllFormEmtiazBandi);

            }

            // کارتابل کارشناس شهرستان
            if (User.IsInRole(RoleName.KarshenasShahrestan))
            {

                //فقط کسانی که در شهرستان خودش هستند را می تواند مشاهده کند
                clientsInDb = clientsInDb.Where(c => c.City.District.CountyId == countyId);

                // افرادی که باید در کارتابل کارشناس شهرستان مشاهده شوند در فایل 
                // ModelEnums.cs
                // در بخش کارشناس شهرستان توضیح داده شده اند

                clientsInDb = clientsInDb.Where(c => c.ClientState != null &&
                                                     c.ClientState.ClientStateTypeE ==
                                                     ModelEnums.ClientStateTypeE.InitialClientRegister ||
                                                     c.ClientState.ClientStateTypeE ==
                                                     ModelEnums.ClientStateTypeE.SubmitNewDataByClient ||
                                                     c.ClientState.ClientStateTypeE ==
                                                     ModelEnums.ClientStateTypeE.InWaitingListKarshenasShahrestan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .ApproveBySazmanHamkarShahrestan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .InWaitingListSazmanHamkarShahrestan ||
                                                     c.ClientState.ClientStateTypeE ==
                                                     ModelEnums.ClientStateTypeE.DenyBySazmanHamkarShahrestan ||
                                                     c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE
                                                         .ApproveByAllOfSazmanHamkarShahrestan ||
                                                     c.ClientState.ClientStateTypeE ==
                                                     ModelEnums.ClientStateTypeE.DenyByKarshenasOstan ||
                                                     c.ClientState.ClientStateTypeE ==
                                                     ModelEnums.ClientStateTypeE.InWaitingListFormEmtiazBandi);
            }

            // کارتابل کارشناس سازمان همکار
            else if (User.IsInRole(RoleName.SazmanHamkar))
            {
                // فقط مربوط به استان خود را مشاهده کند
                clientsInDb = clientsInDb.Where(c => c.City.District.County.ProvinceId == userInfoInDb.ProvinceId);

                // فقط مددجویانی که در طرح هایی ثبت شده اند که این سازمان در آن طرح همکار باشد
                clientsInDb = clientsInDb.Where(c => c.ClientForms.Any(cf =>
                    cf.Form.FormCoOrganizationRoles.Any(fco =>
                        fco.CoOrganizationTypeId == userInfoInDb.CoOrganizationTypeId)));

                // از کجا بفهمیم کارمند سازمان همکار در شهرستان هست یا استان؟
                // باید مقدار CountyID را بررسی کنیم

                if (userInfoInDb.CountyId == null)
                {
                    // کاربر سازمان همکار استان می باشد
                    clientsInDb = clientsInDb.Where(c =>
                        c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.InWaitingListSazmanHamkarOstan ||
                        c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.ApproveBySazmanHamkarOstan);
                }
                else
                {
                    // کاربر سازمان همکار شهرستان
                    clientsInDb = clientsInDb.Where(c => c.City.District.CountyId == userInfoInDb.CountyId);

                    clientsInDb = clientsInDb.Where(c =>
                        c.ClientState.ClientStateTypeE ==
                        ModelEnums.ClientStateTypeE.InWaitingListSazmanHamkarShahrestan ||
                        c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.ApproveBySazmanHamkarOstan);
                }

                // اگر قبلا برای این مددجو تاییدیه را زده باشیم نباید در کارتابل نمایش داده شود
                var clientIdsWhoAlreadyApproveByThisCoOrg = _dbContext.CoOrganizationApproveLists
                    .Where(coOrg => coOrg.CoOrganizationTypeId == userInfoInDb.CoOrganizationTypeId)
                    .Select(c => c.ClientId);

                clientsInDb = clientsInDb.Where(c => !clientIdsWhoAlreadyApproveByThisCoOrg.Contains(c.Id));
            }
            else if (User.IsInRole(RoleName.ModirKolOstan))
            {
                // فقط مددجویان موجود در استان کاربر به او نمایش داده خواهد شد
                clientsInDb = clientsInDb.Where(c => c.City.District.County.ProvinceId == provinceId);

                // در کارتابل مدیر کل فقط مواردی که در مرحله دریافت فرم امتیاز بندی هستند ولی هنوز فرم آنها توسط مدیر کل تایید نشده است نمایش داده شوند
                clientsInDb = clientsInDb.Where(c =>
                    c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.InWaitingListFormEmtiazBandi && !c.FormEmtiazBandi.IsApproveByModirKol);

            }
            else if (User.IsInRole(RoleName.MoavenOstan))
            {
                // فقط مددجویان موجود در استان کاربر به او نمایش داده خواهد شد
                clientsInDb = clientsInDb.Where(c => c.City.District.County.ProvinceId == provinceId);

                // در کارتابل معاون تخصصی
                // فقط مواردی که در مرحله دریافت فرم امتیاز بندی هستند اما هنوز تایید فرم امتیاز بندی توسط معاون تخصصی برای آنها خورده نشده است نمایش داده شوند.
                clientsInDb = clientsInDb.Where(c =>
                    c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.InWaitingListFormEmtiazBandi && !c.FormEmtiazBandi.IsApproveByRelatedAssistance &&
                    c.AssistanceTypeId == userInfoInDb.AssistanceTypeId);

            }
            else if (User.IsInRole(RoleName.MoavenMosharekat))
            {
                // فقط مددجویان موجود در استان کاربر به او نمایش داده خواهد شد
                clientsInDb = clientsInDb.Where(c => c.City.District.County.ProvinceId == provinceId);

                // در کارتابل معاون تخصصی
                // فقط مواردی که در مرحله دریافت فرم امتیاز بندی هستند اما هنوز تایید فرم امتیاز بندی توسط معاون تخصصی برای آنها خورده نشده است نمایش داده شوند.
                clientsInDb = clientsInDb.Where(c =>
                    c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.InWaitingListFormEmtiazBandi && !c.FormEmtiazBandi.IsApproveByMoavenMosharekatAssistance);


            }
            else if (User.IsInRole(RoleName.ModirShahrestan))
            {
                // فقط مددجویان موجود در استان کاربر به او نمایش داده خواهد شد
                clientsInDb = clientsInDb.Where(c => c.City.District.CountyId == countyId);

                // در کارتابل معاون تخصصی
                // فقط مواردی که در مرحله دریافت فرم امتیاز بندی هستند اما هنوز تایید فرم امتیاز بندی توسط معاون تخصصی برای آنها خورده نشده است نمایش داده شوند.
                clientsInDb = clientsInDb.Where(c =>
                    c.ClientState.ClientStateTypeE == ModelEnums.ClientStateTypeE.InWaitingListFormEmtiazBandi && !c.FormEmtiazBandi.IsApproveByModirShahrestan);


            }

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {
                    clientsInDb = clientsInDb.Where(c => c.Person.Name.ToLower().Contains(searchWord) ||
                                                         c.Person.Family.ToLower().Contains(searchWord) ||
                                                         c.Person.NationalCode.ToLower().Contains(searchWord) ||
                                                         c.City.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.Name.ToLower().Contains(searchWord) ||
                                                         c.City.District.County.Name.ToLower().Contains(searchWord) ||
                                                         c.Id.ToString().Contains(searchWord));
                }

            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = clientsInDb.Count();
            var totalResultsCount = _dbContext.Clients.Count(c => c.IsDeleted != true);

            var searchResult = clientsInDb
                .OrderBy(sortBy)
                .Skip(skip)
                .Take(take)
                .AsEnumerable()
                .Select(_mapper.Map<ClientDto>)
                .ToList();

            var userNationalCodeList = _dbContext.ClientUsers.Select(u => u.NationalCode).ToList();

            foreach (var clientDto in searchResult)
            {
                if (userNationalCodeList.Contains(clientDto.Person.NationalCode))
                {
                    clientDto.HasClientUser = true;
                }
            }

            var output = Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = searchResult
            });

            return output;
        }


        // GET /api/clients/1
        public IHttpActionResult GetClient(int id)
        {
            var clientInDb = _dbContext.Clients.Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.Person)
                .SingleOrDefault(c => c.Id == id && (c.IsDeleted == null || c.IsDeleted == false));
            if (clientInDb == null)
                return NotFound();

            return Ok(_mapper.Map<ClientDto>(clientInDb));
        }

        // POST /api/clients
        [HttpPost]
        public IHttpActionResult CreateClient(ClientDto clientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            clientDto.IsDeleted = false;
            clientDto.Person.IsDeleted = false;

            var client = _mapper.Map<Client>(clientDto);
            var person = _mapper.Map<Person>(clientDto);

            _dbContext.Persons.Add(person);
            _dbContext.SaveChanges();

            client.Person = person;

            _dbContext.Clients.Add(client);
            _dbContext.SaveChanges();

            clientDto.Id = client.Id;

            return Created(Request.RequestUri + "/" + client.Id, clientDto);
        }

        // PUT /api/clients/1
        [HttpPut]
        public IHttpActionResult UpdateClient(int id, ClientDto clientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var clientInDb = _dbContext.Clients.SingleOrDefault(c => c.Id == id);
            var personInDb = _dbContext.Persons.SingleOrDefault(p => p.Id == id);

            if (clientInDb == null || personInDb == null)
                return NotFound();

            _mapper.Map(clientDto, clientInDb);

            _mapper.Map(clientDto, personInDb);

            _dbContext.SaveChanges();

            return Ok(clientDto);
        }

        // DELETE /api/clients/1
        [HttpDelete]
        public IHttpActionResult DeleteClient(int id)
        {
            var clientInDb = _dbContext.Clients.SingleOrDefault(c => c.Id == id);
            if (clientInDb == null)
                return NotFound();

            _dbContext.Clients.Remove(clientInDb);
            _dbContext.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
        }


        [HttpPost]
        [Route("savefamilyrelation")]
        public IHttpActionResult SaveFamilyRelation(FamilyRelation familyRelation)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (familyRelation.PersonMinorId == 0 ||
                familyRelation.PersonMajorId == 0 ||
                familyRelation.FamilyRelationTypeId == 0)
                return BadRequest();

            if (familyRelation.Id == 0)
            {
                _dbContext.FamilyRelations.Add(familyRelation);
            }
            else
            {
                var familyRelationInDb = _dbContext.FamilyRelations.SingleOrDefault(f => f.Id == familyRelation.Id);
                if (familyRelationInDb == null)
                    return NotFound();

                _mapper.Map(familyRelation, familyRelationInDb);
            }
            _dbContext.SaveChanges();

            return Ok(familyRelation);
        }


        [HttpDelete]
        [Route("deletefamilyrelation/{id}")]
        public IHttpActionResult DeleteFamilyRelation(int id)
        {
            if (id == 0)
                return BadRequest();

            var familyRelationInDb = _dbContext.FamilyRelations.SingleOrDefault(f => f.Id == id);
            if (familyRelationInDb == null)
                return StatusCode(HttpStatusCode.NotFound);

            _dbContext.FamilyRelations.Remove(familyRelationInDb);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            return Ok(id);
        }


        [HttpPost]
        [Route("activationcode/{nationalCode}")]
        public IHttpActionResult GetActivationCode(string nationalCode)
        {
            if (!User.IsInRole(RoleName.KarshenasOstan) &&
                !User.IsInRole(RoleName.KarshenasShahrestan))
            {
                return BadRequest();
            }

            var clientUserInDb = _dbContext.ClientUsers.SingleOrDefault(u => u.NationalCode == nationalCode);
            if (clientUserInDb == null)
                return NotFound();

            return Ok(clientUserInDb.ActivationCode);
        }

        [HttpPost]
        [Route("clientsummary/{id}")]
        public IHttpActionResult ClientSummary(int id)
        {

            var clientInDb = _dbContext.Clients
                .Include(c => c.City)
                .Include(c => c.City.District)
                .Include(c => c.City.District.County)
                .Include(c => c.Person)
                .Include(c => c.Person.CityOfBirth)
                .Include(c => c.Person.CityOfBirth.District)
                .Include(c => c.Person.CityOfBirth.District.County)
                .Include(c => c.Person.CityOfBirth.District.County.Province)
                .Include(c => c.Person.GenderType)
                .Include(c => c.Person.MarriageType)
                .Include(c => c.ClientType)
                .Include(c => c.CurrentHousing)
                .Include(c => c.CurrentHousing.CurrentHouseType)
                .Include(c=>c.ClientForms)
                .Include(c=>c.ClientForms.Select(cf=>cf.Form))
                .Include(c => c.AssistanceType)
                .SingleOrDefault(c => c.Id == id);
            if (clientInDb == null)
                return NotFound();

            var clientSummaryDto = new ClientSummaryDto
            {
                Client = _mapper.Map<ClientDto>(clientInDb),
            };

            return Ok(clientSummaryDto);
        }
    }
}