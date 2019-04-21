using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    interface ICategoriesService: ICrudService<CategoryDto>, IAliasedService<CategoryDto>, IPageableService<CategoryDto>, ICountableService
    {
    }
}
