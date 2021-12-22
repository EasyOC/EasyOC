using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace EasyOC.OrchardCore.WorkflowPlus
{
    public class ActivityPropertyDefaultValueResolver : IActivityPropertyDefaultValueResolver
    {
        private readonly IServiceProvider _serviceProvider;



        public ActivityPropertyDefaultValueResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? GetDefaultValue(PropertyInfo activityPropertyInfo)
        {
            var activityPropertyAttribute = activityPropertyInfo.GetCustomAttribute<ActivityInputAttribute>();

            if (activityPropertyAttribute == null)
                return null;

            if (activityPropertyAttribute.DefaultValueProvider == null)
                return activityPropertyAttribute.DefaultValue;

            var providerType = activityPropertyAttribute.DefaultValueProvider;

            using var scope = _serviceProvider.CreateScope();
            var provider = (IActivityPropertyDefaultValueProvider)ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, providerType);
            return provider.GetDefaultValue(activityPropertyInfo);
        }
    }
}



