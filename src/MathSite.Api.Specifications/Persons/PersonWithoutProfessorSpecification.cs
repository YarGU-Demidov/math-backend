using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Persons
{
    public class PersonWithoutProfessorSpecification : Specification<Person>
    {
        public override Expression<Func<Person, bool>> ToExpression()
        {
            return person => person.Professor == null;
        }
    }
}