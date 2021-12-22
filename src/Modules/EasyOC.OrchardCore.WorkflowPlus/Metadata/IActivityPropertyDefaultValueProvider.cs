using System.Reflection;

namespace EasyOC.OrchardCore.WorkflowPlus
{
    public interface IActivityPropertyDefaultValueProvider
    {
        object GetDefaultValue(PropertyInfo property);
    }
    public class ActivityPropertyDefaultValueProvider : IActivityPropertyDefaultValueProvider
    {
        public object? GetDefaultValue(PropertyInfo activityPropertyInfo)
        {
            return null;
        }
    }
}



