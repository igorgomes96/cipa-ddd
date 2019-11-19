using System.Collections.Generic;

namespace Cipa.WebApi.ViewModels
{
    public class PagedResult
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<object> Result { get; set; }
    }
}