using System.Collections.Generic;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    /// <summary>
    ///     Тип группы.
    ///     Например: пользовательская, студенчаская, сотрудников вуза.
    /// </summary>
    public class GroupType : EntityWithNameAndAlias
    {
        /// <summary>
        ///     Описание типа группы.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        ///     Список групп этого типа.
        /// </summary>
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}