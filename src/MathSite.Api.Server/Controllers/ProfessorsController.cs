using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Dto;
using MathSite.Api.Entities;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.Attributes;
using MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes;
using MathSite.Api.Server.Infrastructure.CommonServiceMethods;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Professors)]
    [ApiController]
    public class ProfessorsController : EntityApiControllerBase<Professor>, IProfessorsService
    {
        private const string ServiceName = ServiceNames.Professors;
        private readonly CrudServiceMethods<Professor, ProfessorDto> _crudServiceMethods;
        private readonly PageableServiceMethods<Professor, ProfessorDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<Professor> _countableServiceMethods;

        public ProfessorsController(MathSiteDbContext context,
            MathServices services,
            IMapper mapper,
            CrudServiceMethods<Professor, ProfessorDto> crudServiceMethods,
            PageableServiceMethods<Professor, ProfessorDto> pageableServiceMethods,
            CountableServiceMethods<Professor> countableServiceMethods) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<ProfessorDto>> GetById([FromQuery] Guid id)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.GetById(id));
        }

        [HttpGet("get-by-id-with-person")]
        [AuthorizeMethod(ServiceName, "get-by-id-with-person")]
        public Task<ApiResponse<ProfessorDto>> GetByIdWithPersonAsync([FromQuery] Guid id)
        {
            return ExecuteSafely(async () =>
            {
                var professor = await Repository
                    .Include(p=>p.Person)
                    .FirstOrDefaultAsync(p => p.Id == id);
                var professorDto =  Mapper.Map<ProfessorDto>(professor);
                professorDto.Person.Professor = null;
                return professorDto;
            });
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync([FromBody] ProfessorDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync([FromBody] ProfessorDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.UpdateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpDelete(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        [HttpGet("get-all-by-surname")]
        [AuthorizeMethod(ServiceName, "get-all-by-surname")]
        public Task<ApiResponse<IEnumerable<ProfessorDto>>> GetBySurnameAsync(string surname)
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository
                    .Include(p=> p.Person)
                    .Where(p => p.Person.Surname.ToLower().Contains(surname.ToLower()))
                    .Select(p => Mapper.Map<ProfessorDto>(p))
                    .ToArrayAsync();
                return (IEnumerable<ProfessorDto>)persons;
            });
        }

        [HttpDelete("delete-many")]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse<int>> DeleteManyAsync([FromBody] List<Guid> ids)
        {
            return ExecuteSafely(() =>
            {
                return Repository.Where(p => ids.Contains(p.Id)).DeleteFromQueryAsync();
            });
        }

        [HttpGet(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<ProfessorDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet("get-all-by-page-nested")]
        [AuthorizeMethod(ServiceName, "get-all-by-page-nested")]
        public Task<ApiResponse<IEnumerable<ProfessorDto>>> GetAllByPageNested(int page, int perPage)
        {
            return ExecuteSafely(async () =>
            {
                page = page >= 1 ? page : 0;
                perPage = perPage > 0 ? perPage : 0;

                var professors = await Repository
                    .Include(p => p.Person)
                    .Skip(page * perPage)
                    .Take(perPage)
                    .Select(u => Mapper.Map<ProfessorDto>(u))
                    .ToArrayAsync();
                return (IEnumerable<ProfessorDto>)professors;
            });
        }

        [HttpGet(MethodNames.Global.GetCount)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }

        [HttpGet(MethodNames.Users.GetAll)]
        [AuthorizeMethod(ServiceName, MethodNames.Users.GetAll)]
        public Task<ApiResponse<IEnumerable<ProfessorDto>>> GetAllAsync()
        {
            return ExecuteSafely(async () =>
            {
                var data = await Repository.Select(professor => Mapper.Map<ProfessorDto>(professor)).ToArrayAsync();
                return (IEnumerable<ProfessorDto>)data;
            });
        }
        protected async Task<Professor> ViewModelToEntityAsync(ProfessorDto viewModel, ActionType actionType)
        {
            Professor professor;
            if (actionType == ActionType.Create)
            {
                professor = new Professor();
                Mapper.Map(viewModel, professor);
            }
            else
            {
                professor = await Repository.FirstOrDefaultAsync(p => p.Id == viewModel.Id);
                Mapper.Map(viewModel, professor);
            }

            return professor;
        }
    }
}