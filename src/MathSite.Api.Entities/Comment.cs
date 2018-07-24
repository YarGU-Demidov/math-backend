﻿using System;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    /// <inheritdoc />
    /// <summary>
    ///     Комментарий
    /// </summary>
    public class Comment : Entity
    {
        /// <summary>
        ///     Текст комментария
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Дата комментария
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Был ли комментарий изменён
        /// </summary>
        public bool Edited { get; set; }

        /// <summary>
        ///     Идентификатор поста, к которому написан комментарий
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        ///     Пост, к которому написан комментарий
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        ///     Идентификатор пользователя, написавшего комментарий
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     Пользователь, написавший комментарий
        /// </summary>
        public User User { get; set; }
    }
}