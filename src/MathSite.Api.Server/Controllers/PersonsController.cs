﻿using System;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Persons)]
    [ApiController]
    public class PersonsController : EntityApiControllerBase<Person>, IPersonsService
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
        public Task<ApiResponse<Guid>> CreateAsync([FromBody]PersonDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync([FromBody]PersonDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.UpdateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpDelete(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        [HttpDelete("delete-many")]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse<int>> DeleteManyAsync([FromBody] List<Guid> ids)
        {
            return ExecuteSafely(() =>
            {
                return Repository.Where(x => ids.Contains(x.Id)).DeleteFromQueryAsync();
            });
        }

        [HttpGet(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet("get-all-by-page-nested")]
        [AuthorizeMethod(ServiceName, "get-all-by-page-nested")]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetAllByPageNested(int page, int perPage)
        {
            return ExecuteSafely(async () =>
            {
                page = page >= 1 ? page : 0;
                perPage = perPage > 0 ? perPage : 0;

                var persons = await Repository
                    .Include(u => u.User)
                    .Include(u=>u.Professor)
                    .Skip(page * perPage)
                    .Take(perPage)
                    .Select(u => Mapper.Map<PersonDto>(u))
                    .ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
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
                var persons = await Repository
                    .Include(p=>p.User)
                    .Where(p => p.User == null)
                    .Select(p => Mapper.Map<PersonDto>(p))
                    .ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
            });
        }

        [HttpGet("get-all-by-surname")]
        [AuthorizeMethod(ServiceName, "get-all-by-surname")]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetBySurnameAsync(string surname)
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository
                    .Where(p => p.Surname.ToLower().Contains(surname.ToLower()))
                    .Select(p=>Mapper.Map<PersonDto>(p))
                    .ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
            });
        }

        [HttpGet("get-all-by-surname-without-users")]
        [AuthorizeMethod(ServiceName, "get-all-by-surname-without-users")]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetBySurnameWithoutUsersAsync(string surname)
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository
                    .Include(p=>p.User)
                    .Where(p => p.Surname.ToLower().Contains(surname.ToLower()) && p.User==null)
                    .Select(p => Mapper.Map<PersonDto>(p))
                    .ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
            });
        }

        [HttpGet(MethodNames.Persons.GetAllWithoutProfessors)]
        [AuthorizeMethod(ServiceName, MethodNames.Persons.GetAllWithoutUsers)]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetAllWithoutProfessorsAsync()
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository
                    .Include(p => p.Professor)
                    .Where(p => p.Professor == null)
                    .Select(p => Mapper.Map<PersonDto>(p))
                    .ToArrayAsync();
                return (IEnumerable<PersonDto>)persons;
            });
        }

        [HttpGet("get-all-by-surname-without-professors")]
        [AuthorizeMethod(ServiceName, "get-all-by-surname-without-professors")]
        public Task<ApiResponse<IEnumerable<PersonDto>>> GetBySurnameWithoutProfessorsAsync(string surname)
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository
                    .Include(p => p.Professor)
                    .Where(p => p.Surname.ToLower().Contains(surname.ToLower()) && p.Professor == null)
                    .Select(p => Mapper.Map<PersonDto>(p))
                    .ToArrayAsync();
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
