using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Users
{
    public class HasLoginSpecification : Specification<User>
    {
        private readonly string _login;

        public HasLoginSpecification(string login)
        {
            _login = login;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.Login == _login;
        }
    }
}