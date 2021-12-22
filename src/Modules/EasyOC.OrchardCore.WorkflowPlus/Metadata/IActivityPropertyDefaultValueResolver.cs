using System.Reflection;

namespace EasyOC.OrchardCore.WorkflowPlus
{
    public interface IActivityPropertyDefaultValueResolver
    {
        object? GetDefaultValue(PropertyInfo activityPropertyInfo);
    }


}



