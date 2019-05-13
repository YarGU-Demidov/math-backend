using System;
using System.Collections.Generic;
using System.Text;

namespace MathSite.Api.Entities
{
    public class SiteSettings
    {
        /// <summary>
        ///      Имя сайта.
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        ///      Заголовок новостей поумолчанию.
        /// </summary>
        public string DefaultTitleForNewsPage { get; set; }
        /// <summary>
        ///      Заголовок домашней страницы поумолчанию.
        /// </summary>
        public string DefaultTitleForHomePage { get; set; }
        /// <summary>
        ///      Количество постов на страницу.
        /// </summary>
        public int PerPageCount { get; set; }
        /// <summary>
        ///      Разделитель заголовка страницы и имени сайта.
        /// </summary>
        public string TitleDelimiter { get; set; }
    }
}
