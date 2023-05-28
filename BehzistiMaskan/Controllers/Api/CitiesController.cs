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
    [RoutePrefix("api/cities")]
    public class CitiesController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CitiesController()
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

        //GET /api/cities
        public IEnumerable<CityDto> GetCities()
        {
            return _context.Cities.Include(c => c.District).AsEnumerable()
                .Select(_mapper.Map<City, CityDto>);
        }

        // GET /api/cities/1
        [Route("{id:int}")]
        public IHttpActionResult GetCity(int id)
        {
            var city = _context.Cities.Include(c => c.District).SingleOrDefault(c => c.Id == id);
            if (city == null)
                return NotFound();

            return Ok(_mapper.Map<CityDto>(city));
        }


        // GET /api/cities/fa90ec61-ba32-4c40-9eeb-b3f5e9c40cd9
        [Route("{id:Guid}")]
        public IHttpActionResult GetCity(Guid id)
        {
            var city = _context.Cities.Include(c => c.District).SingleOrDefault(c => c.UniqueId == id);
            if (city == null)
                return NotFound();

            return Ok(_mapper.Map<CityDto>(city));
        }

        // POST /api/cities
        [HttpPost]
        public IHttpActionResult CreateCity(CityDto cityDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var city = _mapper.Map<City>(cityDto);
            city.UniqueId = Guid.NewGuid();

            _context.Cities.Add(city);
            _context.SaveChanges();

            cityDto.Id = city.Id;
            cityDto.UniqueId = city.UniqueId;

            return Created(new Uri(Request.RequestUri + "/" + city.Id), cityDto);
        }

        // PUT /api/cities/1
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateCity(int id, CityDto cityDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var cityInDb = _context.Cities.Include(c => c.District)
                .SingleOrDefault(c => c.Id == id);
            if (cityInDb == null)
                return NotFound();

            cityInDb.Name = cityDto.Name;
            cityInDb.IsVillage = cityDto.IsVillage;
            cityInDb.Dehestan = cityDto.Dehestan;
            cityInDb.DistrictId = cityDto.DistrictId;

            _context.SaveChanges();

            return Ok(_mapper.Map<CityDto>(cityInDb));
        }

        // DELETE /api/cities/1
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteCity(int id)
        {
            var cityInDb = _context.Cities.SingleOrDefault(c => c.Id == id);
            if (cityInDb == null)
                return StatusCode(HttpStatusCode.NotFound);

            _context.Cities.Remove(cityInDb);
            _context.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
