using System.Reflection;

namespace EasyOC.Workflows.Metadata
{
    public interface IActivityPropertyDefaultValueResolver
    {
        object? GetDefaultValue(PropertyInfo activityPropertyInfo);
    }


}



