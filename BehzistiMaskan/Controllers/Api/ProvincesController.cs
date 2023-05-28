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
    //[Authorize(Roles = RoleName.SystemAdministrator)]
    [RoutePrefix("api/provinces")]
    public class ProvincesController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        //private static readonly Expression<Func<Province, ProvinceDto>> AsProvinceDto =
        //    x => new ProvinceDto
        //    {
        //        Name = x.Name,
        //        Id = x.Id,
        //        UniqueId = x.UniqueId
        //    };

        //public static Province AsProvince(ProvinceDto provinceDto)
        //{
        //    return new Province
        //    {
        //        Name = provinceDto.Name,
        //        Id = provinceDto.Id,
        //        UniqueId = provinceDto.UniqueId
        //    };
        //}

        public ProvincesController()
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

        // GET /api/provinces
        public IEnumerable<ProvinceDto> GetProvinces()
        {
            return _context.Provinces.ToList().Select(_mapper.Map<Province, ProvinceDto>);
        }

        // GET /api/provinces/1
        [Route("{id:int}")]
        public IHttpActionResult GetProvince(int id)
        {

            var province = _context.Provinces.SingleOrDefault(p => p.Id == id);
            if (province == null)
                return NotFound();

            return Ok(_mapper.Map<ProvinceDto>(province));
        }


        // GET /api/provinces/A298BE5A-AB34-4150-956A-A7276EA8D4A3
        [Route("{id:Guid}")]
        public IHttpActionResult GetProvince(Guid id)
        {
            var province = _context.Provinces.SingleOrDefault(p => p.UniqueId == id);
            if (province == null)
                return NotFound();

            return Ok(_mapper.Map<ProvinceDto>(province));
        }


        // GET /api/provinces/1/counties
        [Route("{id:int}/counties")]
        public IEnumerable<CountyDto> GetCountiesByProvinceId(int id)
        {
            return _context.Counties.Where(c => c.ProvinceId == id).Include(c => c.Province).AsEnumerable()
                .Select(_mapper.Map<County, CountyDto>);
        }


        // POST /api/provinces
        [HttpPost]
        public IHttpActionResult CreateProvince(ProvinceDto provinceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var province = new Province
            {
                Name = provinceDto.Name,
                UniqueId = Guid.NewGuid()
            };

            _context.Provinces.Add(province);
            _context.SaveChanges();

            provinceDto.Id = province.Id;
            provinceDto.UniqueId = province.UniqueId;

            return Created(new Uri(Request.RequestUri + "/" + province.Id), provinceDto);
        }

        // PUT /api/provinces/1
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateProvince(int id, ProvinceDto provinceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var provinceInDb = _context.Provinces.SingleOrDefault(p => p.Id == id);

            if (provinceInDb == null)
                return NotFound();

            provinceInDb.Name = provinceDto.Name;

            _context.SaveChanges();

            return Ok(_mapper.Map<ProvinceDto>(provinceInDb));
        }


        // DELETE /api/provinces/1
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteProvince(int id)
        {
            var provinceInDb = _context.Provinces.SingleOrDefault(p => p.Id == id);
            if (provinceInDb == null)
                return StatusCode(HttpStatusCode.NotFound);

            _context.Provinces.Remove(provinceInDb);
            _context.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
