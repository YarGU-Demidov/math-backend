using System;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    /// <summary>
    ///     Настройки пользователя.
    /// </summary>
    public class UserSetting : Entity
    {
        /// <summary>
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// </summary>
        public User User { get; set; }
    }
}