using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.DataTable;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;
using Microsoft.AspNet.Identity;

namespace BehzistiMaskan.Controllers.Api
{
    [RoutePrefix("api/benefactors")]
    [Authorize]
    public class BenefactorsController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public BenefactorsController()
        {
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
            _dbContext = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _dbContext.Dispose();
            base.Dispose(disposing);
        }


        [Route("benefactordatatable")]
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

                if (sortBy.Contains("Province"))
                    sortBy = sortBy.Replace("Province", "Province.");
                else if (sortBy.Contains("County"))
                    sortBy = sortBy.Replace("County", "County.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            var benefactorsInDb = _dbContext.Benefactors
                .Include(b => b.County)
                .Include(b => b.Province)
                .AsQueryable();

            var userId = User.Identity.GetUserId();

            var userInfoInDb = _dbContext.UserInfos
                .Include(uInf => uInf.CoOrganizationType)
                .Include(u => u.Province)
                .Include(u => u.County)
                .Single(uInf => uInf.User.Id == userId);
            var provinceId = userInfoInDb.ProvinceId;
            var countyId = userInfoInDb.CountyId;

            if (User.IsInRole(RoleName.KarshenasOstan))
            {
                benefactorsInDb = benefactorsInDb.Where(b => b.ProvinceId == provinceId);
            }

            if (User.IsInRole(RoleName.KarshenasShahrestan))
            {
                //فقط کسانی که در شهرستان خودش هستند را می تواند مشاهده کند
                benefactorsInDb = benefactorsInDb.Where(b => b.CountyId == countyId);
            }

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                //foreach (var searchWord in searchWords)
                //{
                //    benefactorsInDb = benefactorsInDb.Where(c => c.Name.ToLower().Contains(searchWord) ||
                //                                         c.Family.ToLower().Contains(searchWord) ||
                //                                         c.NationalCode.ToLower().Contains(searchWord) ||
                //                                         c.County.Name.ToLower().Contains(searchWord) ||
                //                                         c.Province.Name.ToLower().Contains(searchWord) ||
                //                                         c.Id.ToString().Contains(searchWord));
                //}
                benefactorsInDb = searchWords.Aggregate(benefactorsInDb,
                    (current,
                        searchWord) => current.Where(c => c.Name.ToLower()
                                                              .Contains(searchWord) ||
                                                          c.Family.ToLower()
                                                              .Contains(searchWord) ||
                                                          c.NationalCode.ToLower()
                                                              .Contains(searchWord) ||
                                                          c.County.Name.ToLower()
                                                              .Contains(searchWord) ||
                                                          c.Province.Name.ToLower()
                                                              .Contains(searchWord) ||
                                                          c.Id.ToString()
                                                              .Contains(searchWord)));
            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = benefactorsInDb.Count();
            var totalResultsCount = _dbContext.Benefactors.Count();

            if (string.IsNullOrEmpty(sortBy))
                sortBy = "Id asc";

            var searchResult = take != -1
                ? benefactorsInDb.OrderBy(sortBy)
                    .Skip(skip)
                    .Take(take)
                    .AsEnumerable()
                    .Select(_mapper.Map<BenefactorSimpleDto>)
                    .ToList()
                // ReSharper disable once AssignNullToNotNullAttribute
                : benefactorsInDb.OrderBy(sortBy)
                    .AsEnumerable()
                    .Select(_mapper.Map<BenefactorSimpleDto>)
                    .ToList();

            var output = Json(new
            {
                // this is what Datatable wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = searchResult
            });

            return output;
        }

    }
}
