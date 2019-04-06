using AutoMapper;
using MathSite.Api.Dto;
using MathSite.Api.Entities;

namespace MathSite.Api.Server.Infrastructure
{
    public class AutoMapperMathSiteProfile : Profile
    {
        public AutoMapperMathSiteProfile()
        {
            BuildMaps();
        }

        private void BuildMaps()
        {
            CreateMap<User, ProfessorDto>();
            CreateMap<ProfessorDto, User>();
        }
    }
}