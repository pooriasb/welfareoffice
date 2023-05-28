using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.DataTable;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Controllers.Api
{
    [RoutePrefix("api/persons")]
    public class PersonsController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public PersonsController()
        {
            _dbContext = new ApplicationDbContext();
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }

        // GET /api/persons
        public IEnumerable<PersonDto> GetPersons()
        {
            return _dbContext.Persons
                .Include(p => p.Client)
                .Where(p => p.Client == null)
                .AsEnumerable()
                .Select(_mapper.Map<PersonDto>).ToList();
        }

        [Route("count")]
        public int GetPersonCount()
        {
            return _dbContext.Persons.Include(p => p.Client).Count(p => p.Client == null);
        }

        [Route("datatable")]
        [HttpPost]
        public IHttpActionResult GetPersonsDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data;

                if (sortBy.Contains("CountyOfBirthId"))
                    sortBy = sortBy.Replace("CountyOfBirthId", "CityOfBirth.District.County.");
                else if (sortBy.Contains("DistrictOfBirthId"))
                    sortBy = sortBy.Replace("DistrictOfBirthId", "CityOfBirth.District.");
                else if (sortBy.Contains("CityOfBirth"))
                    sortBy = sortBy.Replace("CityOfBirth", "CityOfBirth.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            // همه کسایی که مددجو نیستند و به عنوان عضو خانواده مددجوی دیگر هم انتخاب نشده اند

            var personsInDb = _dbContext.Persons
                .Include(c => c.CityOfBirth)
                .Include(c => c.CityOfBirth.District)
                .Include(c => c.CityOfBirth.District.County)
                .Where(p => p.Client == null && p.FamilyRelations.Count == 0);

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {
                    personsInDb = personsInDb.Where(c => c.Name.Contains(searchWord) ||
                                                         c.Family.Contains(searchWord) ||
                                                         c.NationalCode.Contains(searchWord) ||
                                                         c.FatherName.Contains(searchWord) ||
                                                         c.CityOfBirth.Name.Contains(searchWord) ||
                                                         c.CityOfBirth.District.Name.Contains(searchWord) ||
                                                         c.CityOfBirth.District.County.Name.Contains(searchWord));
                }

            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = personsInDb.Count();
            var totalResultsCount = _dbContext.Persons.Count(p => p.Client == null && p.FamilyRelations.Count == 0);
            var searchResult = personsInDb
                .OrderBy(sortBy)
                .Skip(skip)
                .Take(take)
                .AsEnumerable()
                .Select(_mapper.Map<PersonDto>)
                .ToList();

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

        [Route("familyrelationdatatable")]
        [HttpPost]
        public IHttpActionResult GetPersonsStoreAsFamilyRelationDataTableSearch([FromBody] DataTableAjaxPost model)
        {
            var searchBy = model.Search?.value;
            var take = model.Length;
            var skip = model.Start;

            var sortBy = "";

            if (model.Order != null)
            {
                sortBy = model.Columns[model.Order[0].column].data;

                if (sortBy.Contains("CountyOfBirthId"))
                    sortBy = sortBy.Replace("CountyOfBirthId", "CityOfBirth.District.County.");
                else if (sortBy.Contains("DistrictOfBirthId"))
                    sortBy = sortBy.Replace("DistrictOfBirthId", "CityOfBirth.District.");
                else if (sortBy.Contains("CityOfBirth"))
                    sortBy = sortBy.Replace("CityOfBirth", "CityOfBirth.");
                sortBy += " " + model.Order[0].dir.ToLower();
            }

            // فقط افرادی که به عنوان اعضای خانواده سایر مددجویان ثبت شده اند و خود مددجوی تایید شده نمی باشند

            var personsInDb = _dbContext.Persons
                .Include(c => c.CityOfBirth)
                .Include(c => c.CityOfBirth.District)
                .Include(c => c.CityOfBirth.District.County)
                .Where(p => p.Client == null && p.FamilyRelations.Count > 0);

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {
                    personsInDb = personsInDb.Where(c => c.Name.Contains(searchWord) ||
                                                         c.Family.Contains(searchWord) ||
                                                         c.NationalCode.Contains(searchWord) ||
                                                         c.FatherName.Contains(searchWord) ||
                                                         c.CityOfBirth.Name.Contains(searchWord) ||
                                                         c.CityOfBirth.District.Name.Contains(searchWord) ||
                                                         c.CityOfBirth.District.County.Name.Contains(searchWord));
                }

            }

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = personsInDb.Count();
            var totalResultsCount = _dbContext.Persons.Count(p => p.Client == null);
            var searchResult = personsInDb
                .OrderBy(sortBy)
                .Skip(skip)
                .Take(take)
                .AsEnumerable()
                .Select(_mapper.Map<PersonDto>)
                .ToList();

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
    }
}