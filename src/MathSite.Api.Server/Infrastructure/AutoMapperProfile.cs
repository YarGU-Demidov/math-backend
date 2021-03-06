﻿using AutoMapper;
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
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Professor, ProfessorDto>();
            CreateMap<ProfessorDto, Professor>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<Group, GroupDto>();
            CreateMap<GroupDto, Group>();

            CreateMap<Directory, DirectoryDto>();
            CreateMap<DirectoryDto, Directory>();

            CreateMap<File, FileDto>();
            CreateMap<FileDto, File>();
        }
    }
}