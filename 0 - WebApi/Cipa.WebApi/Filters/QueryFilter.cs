using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Cipa.WebApi.Filters
{
    public class QueryAttribute : ResultFilterAttribute
    {
        private readonly string[] _values;
        public QueryAttribute(params string[] values)
        {
            _values = values;
        }

        private string GetPropertyValue(object src, string propName)
        {
            object value = src;
            string[] tree = propName.Split('.');
            foreach (var prop in tree) {
                value = value.GetType().GetProperty(prop).GetValue(value, null);
            }
            return value.ToString();
        }

        private IEnumerable<object> FilterResponse(IEnumerable<object> response, IDictionary<string, string> atributos)
        {
            if (atributos.Count() > 0)
            {
                return response.Where(item => atributos.All(attr => GetPropertyValue(item, attr.Key)
                    .ToLower().Contains(attr.Value)));
            }
            return response;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            try
            {
                var urlQuery = context.HttpContext.Request.Query;
                var atributos = _values.Where(v => urlQuery.ContainsKey(v.Replace(".", "")))
                                .ToDictionary(k => k, v => urlQuery[v.Replace(".", "")].ToString().ToLower());
                if (context.Result is ObjectResult && ((ObjectResult)context.Result).Value is IQueryable)
                {
                    IQueryable<object> response = ((ObjectResult)context.Result).Value as IQueryable<object>;
                    ((ObjectResult)context.Result).Value = FilterResponse(response, atributos).AsQueryable();
                }
            }
            catch
            { 
                // Qualquer erro deve ser ignorado, e o Pipeline Request deve prosseguir.
            }
            base.OnResultExecuting(context);
        }
    }
}