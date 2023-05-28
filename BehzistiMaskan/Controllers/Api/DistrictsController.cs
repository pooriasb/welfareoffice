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
    [RoutePrefix("api/districts")]
    public class DistrictsController : ApiController
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DistrictsController()
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

        //GET /api/districts
        public IEnumerable<DistrictDto> GetDistricts()
        {
            return _context.Districts.Include(c => c.County).AsEnumerable()
                .Select(_mapper.Map<District, DistrictDto>);
        }

        // GET /api/districts/1
        [Route("{id:int}")]
        public IHttpActionResult GetDistrict(int id)
        {
            var district = _context.Districts.Include(c => c.County).SingleOrDefault(c => c.Id == id);
            if (district == null)
                return NotFound();

            return Ok(_mapper.Map<DistrictDto>(district));
        }


        // GET /api/districts/fa90ec61-ba32-4c40-9eeb-b3f5e9c40cd9
        [Route("{id:Guid}")]
        public IHttpActionResult GetDistrict(Guid id)
        {
            var district = _context.Districts.Include(c => c.County).SingleOrDefault(c => c.UniqueId == id);
            if (district == null)
                return NotFound();

            return Ok(_mapper.Map<DistrictDto>(district));
        }

        // GET /api/districts/1/districts
        [Route("{id}/cities")]
        public IEnumerable<CityDto> GetCitiesByDistrictId(int id)
        {
            return _context.Cities.Where(d => d.DistrictId == id).Include(d => d.District).AsEnumerable()
                .Select(_mapper.Map<City, CityDto>);
        }

        // POST /api/districts
        [HttpPost]
        public IHttpActionResult CreateDistrict(DistrictDto districtDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var district = _mapper.Map<District>(districtDto);
            district.UniqueId = Guid.NewGuid();

            _context.Districts.Add(district);
            _context.SaveChanges();

            districtDto.Id = district.Id;
            districtDto.UniqueId = district.UniqueId;

            return Created(new Uri(Request.RequestUri + "/" + district.Id), districtDto);
        }

        // PUT /api/districts/1
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateDistrict(int id, DistrictDto districtDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var districtInDb = _context.Districts.SingleOrDefault(c => c.Id == id);
            if (districtInDb == null)
                return NotFound();

            districtInDb.Name = districtDto.Name;
            districtInDb.CountyId = districtDto.CountyId;

            _context.SaveChanges();

            return Ok(_mapper.Map<DistrictDto>(districtInDb));
        }

        // DELETE /api/districts/1
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteDistrict(int id)
        {
            var districtInDb = _context.Districts.SingleOrDefault(c => c.Id == id);
            if (districtInDb == null)
                return StatusCode(HttpStatusCode.NotFound);

            _context.Districts.Remove(districtInDb);
            _context.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
