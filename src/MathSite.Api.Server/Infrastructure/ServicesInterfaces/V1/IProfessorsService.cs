﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IProfessorsService : ICrudService<ProfessorDto>, IPageableService<ProfessorDto>
    {
        Task<ApiResponse<IEnumerable<ProfessorDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProfessorDto>>> GetBySurnameAsync(string surname);
        Task<ApiResponse<int>> DeleteManyAsync(List<Guid> ids);
    }
}