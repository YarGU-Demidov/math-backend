using System;
using MathSite.Api.Dto.Groups;
using MathSite.Api.Dto.Persons;

namespace MathSite.Api.Dto.Users
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid PersonId { get; set; }
        public PersonDto Person { get; set; }
        public Guid GroupId { get; set; }
        public GroupDto Group { get; set; }
    }
}