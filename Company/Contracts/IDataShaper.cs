using System.Dynamic;

namespace Contracts
{
    public interface IDataShaper<T> 
    {
        IEnumerable<ExpandoObject>ShapedData(IEnumerable<T> entities , string fieldString);
        ExpandoObject ShapedData(T entity, string fieldString);

    }
}
