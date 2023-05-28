using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Controllers.Api
{
    [RoutePrefix("api/counties")]
    public class CountiesController : ApiController
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CountiesController()
        {
            _context = new ApplicationDbContext();
            _mapper = MapperUtils.MapperConfiguration.CreateMapper();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        //GET /api/counties
        public IEnumerable<CountyDto> GetCounties()
        {

            return _context.Counties.Include(c => c.Province).ToList().Select(_mapper.Map<County, CountyDto>);
        }

        // GET /api/counties/1
        [Route("{id:int}")]
        public IHttpActionResult GetCounty(int id)
        {
            var county = _context.Counties.Include(c => c.Province).SingleOrDefault(c => c.Id == id);
            if (county == null)
                return NotFound();

            return Ok(_mapper.Map<CountyDto>(county));
        }


        // GET /api/counties/fa90ec61-ba32-4c40-9eeb-b3f5e9c40cd9
        [Route("{id:Guid}")]
        public IHttpActionResult GetCounty(Guid id)
        {
            var county = _context.Counties.Include(c=>c.Province).SingleOrDefault(c => c.UniqueId == id);
            if (county == null)
                return NotFound();

            return Ok(_mapper.Map<CountyDto>(county));
        }

        // GET /api/counties/1/districts
        [Route("{id}/districts")]
        public IEnumerable<DistrictDto> GetDistrictsByCountyId(int id)
        {
            return _context.Districts.Where(d => d.CountyId == id).Include(d => d.County).AsEnumerable()
                .Select(_mapper.Map<District, DistrictDto>);
        }

        // GET /api/counties/1/districts
        [Route("{id}/cities")]
        public IEnumerable<CityDto> GetCitiesByCountyId(int id)
        {
            var districtIds = _context.Districts.Where(d => d.CountyId == id).Select(d => d.Id).ToList();

            return _context.Cities.Where(d => districtIds.Contains(d.DistrictId)).Include(d => d.District).AsEnumerable()
                .Select(_mapper.Map<City, CityDto>);
        }

        // POST /api/counties
        [HttpPost]
        public IHttpActionResult CreateCounty(CountyDto countyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var county = _mapper.Map<County>(countyDto);
            county.UniqueId = Guid.NewGuid();

            _context.Counties.Add(county);
            _context.SaveChanges();

            countyDto.Id = county.Id;
            countyDto.UniqueId = county.UniqueId;

            return Created(new Uri(Request.RequestUri + "/" + county.Id), countyDto);
        }

        // PUT /api/counties/1
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateCounty(int id, CountyDto countyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var countyInDb = _context.Counties.SingleOrDefault(c => c.Id == id);
            if (countyInDb == null)
                return NotFound();

            countyInDb.Name = countyDto.Name;
            countyInDb.ProvinceId = countyDto.ProvinceId;

            _context.SaveChanges();

            return Ok(_mapper.Map<CountyDto>(countyInDb));
        }

        // DELETE /api/counties/1
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteCounty(int id)
        {
            var countyInDb = _context.Counties.SingleOrDefault(c => c.Id == id);
            if(countyInDb == null)
                return StatusCode(HttpStatusCode.NotFound);

            _context.Counties.Remove(countyInDb);
            _context.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
