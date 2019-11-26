using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Cipa.WebApi.ViewModels;
using System.Collections;

namespace Cipa.WebApi.Filters
{
    public class PaginationAttribute : ResultFilterAttribute
    {
        public PagedResult GetPagedResult(IDictionary<string, string> urlQuery, IEnumerable<object> response)
        {
            // Converte key para minúsculo.
            urlQuery = urlQuery.ToDictionary(q => q.Key.ToLower(), q => q.Value);
            if (!urlQuery.ContainsKey("pagesize")) throw new Exception("A query deve conter o parâmetro 'pagesize'!");

            int pageSize = int.Parse(urlQuery["pagesize"]);
            int pageNumber = 1;
            if (urlQuery.ContainsKey("pagenumber"))
                pageNumber = int.Parse(urlQuery["pagenumber"]);
            
            if (pageNumber == 0) pageNumber = 1;
            var rowCount = response.Count();
            var pageCount = (int)Math.Ceiling((double)rowCount / pageSize);
            var skip = (pageNumber - 1) * pageSize;
            return new PagedResult
            {
                CurrentPage = pageNumber,
                PageCount = pageCount,
                PageSize = pageSize,
                TotalRecords = rowCount,
                Result = response.Skip(skip).Take(pageSize)
            };
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            try
            {
                var urlQuery = context.HttpContext.Request.Query.ToDictionary(q => q.Key.ToLower(), q => q.Value.FirstOrDefault()?.ToString());
                if (urlQuery.ContainsKey("pagesize") && context.Result is ObjectResult && ((ObjectResult)context.Result).Value is IEnumerable)
                {
                    IEnumerable<object> response = ((ObjectResult)context.Result).Value as IEnumerable<object>;
                    ((ObjectResult)context.Result).Value = GetPagedResult(urlQuery, response);
                }
            }
            catch { }
            base.OnResultExecuting(context);
        }
    }
}