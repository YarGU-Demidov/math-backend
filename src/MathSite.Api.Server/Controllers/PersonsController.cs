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
    [DefaultApiRoute(ServiceNames.Persons)]
    [ApiController]
    public class PersonsController : EntityApiControllerBase<Person>, IPersonService
    {
        private const string ServiceName = ServiceNames.Persons;
        private readonly CrudServiceMethods<Person, PersonDto> _crudServiceMethods;
        private readonly PageableServiceMethods<Person, PersonDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<Person> _countableServiceMethods;
        public PersonsController(MathSiteDbContext context, 
            MathServices services, 
            IMapper mapper,
            CrudServiceMethods<Person, PersonDto> crudServiceMethods,
            PageableServiceMethods<Person, PersonDto> pageableServiceMethods,
            CountableServiceMethods<Person> countableServiceMethods) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<PersonDto>> GetById(Guid id)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.GetById(id));
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync(PersonDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync(PersonDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.UpdateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpDelete(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        [HttpGet(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet(MethodNames.Global.GetCount)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }

        [HttpGet(MethodNames.Users.GetAll)]
        [AuthorizeMethod(ServiceName, MethodNames.Users.GetAll)]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetAllAsync()
        {
            return ExecuteSafely(async () =>
            {
                var data = await Repository.Select(person => Mapper.Map<PersonDto>(person)).ToArrayAsync();
                return (IEnumerable<PersonDto>)data;
            });
        }

        [HttpGet(MethodNames.Persons.GetAllWithoutUsers)]
        [AuthorizeMethod(ServiceName, MethodNames.Persons.GetAllWithoutUsers)]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetAllWithoutUsersAsync()
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository.Include(p=>p.User).Where(p => p.User == null).Select(p => Mapper.Map<PersonDto>(p)).ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
            });
        }

        [HttpGet("get-by-surname")]
        [AuthorizeMethod(ServiceName, "get-by-surname")]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetBySurnameAsync(string surname)
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository.Where(p => p.Surname.Contains(surname)).Select(p=>Mapper.Map<PersonDto>(p)).ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
            });
        }

        [HttpGet(MethodNames.Persons.GetAllWithoutProfessors)]
        [AuthorizeMethod(ServiceName, MethodNames.Persons.GetAllWithoutUsers)]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetAllWithoutProfessorsAsync()
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository.Include(p => p.Professor).Where(p => p.Professor == null).Select(p => Mapper.Map<PersonDto>(p)).ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
            });
        }

        protected async Task<Person> ViewModelToEntityAsync(PersonDto viewModel, ActionType actionType)
        {
            Person person;
            if (actionType == ActionType.Create)
            {
                person = new Person();
                Mapper.Map(viewModel, person);
            }
            else
            {
                person = await Repository.FirstOrDefaultAsync(u => u.Id == viewModel.Id);
                Mapper.Map(viewModel, person);
            }

            return person;
        }
    }
}
