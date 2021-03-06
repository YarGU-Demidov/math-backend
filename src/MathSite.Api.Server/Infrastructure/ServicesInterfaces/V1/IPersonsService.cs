﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IPersonsService: ICrudService<PersonDto>, IPageableService<PersonDto>, ICountableService
    {
        Task<ApiResponse<IEnumerable<PersonDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<PersonDto>>> GetBySurnameAsync(string surname);
        Task<ApiResponse<IEnumerable<PersonDto>>> GetBySurnameWithoutUsersAsync(string surname);
        Task<ApiResponse<IEnumerable<PersonDto>>> GetBySurnameWithoutProfessorsAsync(string surname);
        Task<ApiResponse<IEnumerable<PersonDto>>> GetAllWithoutProfessorsAsync();
        Task<ApiResponse<IEnumerable<PersonDto>>> GetAllWithoutUsersAsync();
    }
}