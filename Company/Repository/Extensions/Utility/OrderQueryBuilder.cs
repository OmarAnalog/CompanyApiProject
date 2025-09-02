using Entities.Models;
using System.Reflection;
using System.Text;

namespace Repository.Extensions.Utility
{
    public static class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderString)
        {
            var orderParams = orderString.Trim().Split(','); // removing front,back spaces && spliting terms with , into strings
            var objProperties = typeof(T).GetProperties((BindingFlags.Public | BindingFlags.Instance));
            StringBuilder orderQuery = new();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param)) continue;
                var propertyFromQueryName = param.Split(" ")[0]; // param is property ' ' desc/asc
                var property = objProperties.FirstOrDefault(e => e.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (property is null) continue;
                var type = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQuery.Append($"{property.Name.ToString()} {type},");
            }
            var orderQueryString = orderQuery.ToString().TrimEnd(',', ' ');// remove ',' and ' ' 
            return orderQueryString;
        }
    }
}
