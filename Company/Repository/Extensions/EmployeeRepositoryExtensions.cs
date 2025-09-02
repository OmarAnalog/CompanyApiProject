using Entities.Models;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Repository.Extensions.Utility;


namespace Repository.Extensions
{
    public static class EmployeeRepositoryExtensions
    {
        public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> emp, uint minAge, uint maxAge)
        => emp.Where(e => e.Age <= maxAge && e.Age >= minAge);

        public static IQueryable<Employee>Search(this IQueryable<Employee>emp,string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))return emp;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return emp.Where(e=>e.Name.ToLower().Contains(lowerCaseTerm));
        }
        public static IQueryable<Employee>Order(this IQueryable<Employee> emp,string orderString)
        {
            if (string.IsNullOrEmpty(orderString)) return emp.OrderBy(e=>e.Name);
            var orderQueryString = OrderQueryBuilder.CreateOrderQuery<Employee>(orderString); 
            if (string.IsNullOrWhiteSpace(orderQueryString)) return emp.OrderBy(e => e.Name);
            return emp.OrderBy(orderQueryString);
        }
    }
}
