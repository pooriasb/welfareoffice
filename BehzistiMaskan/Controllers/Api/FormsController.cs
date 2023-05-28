using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.DataTable;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Controllers.Api
{
    [RoutePrefix("api/forms")]
    public class FormsController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public FormsController()
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

        // GET /api/clients
        public IEnumerable<Form> GetForms()
        {

            return _context.Forms
                .Include(f => f.FormAccessLevels)
                .Include(f => f.FormCoOrganizationRoles)
                .Where(c => c.IsDeleted == null || c.IsDeleted == false)
                .AsEnumerable();
        }

        [Route("datatable")]
        [HttpPost]
        public IHttpActionResult GetFormsDataTableSearch([FromBody] DataTableAjaxPost model)
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

            var formsInDb = _context.Forms
                .Include(f => f.FormAccessLevels)
                .Include(f => f.FormAccessLevels.Select(x => x.County))
                .Include(f => f.FormCoOrganizationRoles)
                .Include(f => f.FormCoOrganizationRoles.Select(x => x.CoOrganizationType))
                .Where(c => c.IsDeleted == null || c.IsDeleted == false)
                ;

            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchWords = searchBy.Split(' ');
                foreach (var searchWord in searchWords)
                {

                    formsInDb = formsInDb.Where(f => f.Name.Contains(searchWord) ||
                                                     f.FormCoOrganizationRoles.Any(o => o.CoOrganizationType.Name.Contains(searchWord)) ||
                                                     f.FormAccessLevels.Any(acl => acl.County.Name.Contains(searchWord))
                                                     );
                }
            }
            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = formsInDb.Count();
            var totalResultsCount = _context.Forms.Count();

            var searchResult = formsInDb
                .OrderBy(sortBy)
                .Skip(skip)
                .Take(take)
                .AsEnumerable()
                .Select(_mapper.Map<FormDto>)
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
