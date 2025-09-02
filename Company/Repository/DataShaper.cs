using Contracts;
using System.Dynamic;
using System.Reflection;

namespace Repository
{
    public class DataShaper<T> : IDataShaper<T> where T:class
    {
        public PropertyInfo[] Properties { get; set; }
        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        public IEnumerable<ExpandoObject> ShapedData(IEnumerable<T> entities, string fieldString)
        {
            var getRequriredProperties = getProperities(fieldString);
            return FetchData(entities, getRequriredProperties);
        }

        public ExpandoObject ShapedData(T entity, string fieldString)
        {
            var getRequriredProperties = getProperities(fieldString);
            return FetchDataForEntitiy(entity, getRequriredProperties);
        }
        private IEnumerable<PropertyInfo> getProperities(string fieldString)
        {
            var requiredProperties=new List<PropertyInfo>();
            if (string.IsNullOrWhiteSpace(fieldString))
            {
                return Properties.ToList();
            }
            var fields = fieldString.Split(',',StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in fields)
            {
                var prop = Properties.FirstOrDefault(p => p.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (prop is null) continue;
                requiredProperties.Add(prop);
            }
            return requiredProperties;
        }
        private IEnumerable<ExpandoObject>FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> propertyInfos)
        {
            var sharedObject = new List<ExpandoObject>();
            foreach (var entity in entities)
            {
                var data = FetchDataForEntitiy(entity, propertyInfos);
                sharedObject.Add(data);
            }
            return sharedObject;
        }
        private ExpandoObject FetchDataForEntitiy(T entity,IEnumerable<PropertyInfo> propertyInfos)
        {
            var sharedObject = new ExpandoObject();
            foreach (var prop in propertyInfos)
            {
                var objectProperityValue = prop.GetValue(entity);
                sharedObject.TryAdd(prop.Name, objectProperityValue);
            }
            return sharedObject;
        }
    }
}
