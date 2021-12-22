using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Panda.DynamicWebApi;
using Panda.DynamicWebApi.Attributes;
using System.Reflection;

namespace EasyOC.Core.Dynamic
{
    public class ServiceActionRouteFactory : IActionRouteFactory
    {
        public string CreateActionRouteModel(string areaName, string controllerName, ActionModel action)
        {
            var controllerType = action.ActionMethod.DeclaringType;
            var serviceAttribute = controllerType.GetCustomAttribute<DynamicWebApiAttribute>();

            var _controllerName = string.IsNullOrEmpty(serviceAttribute?.Module) ?
                controllerName.Replace("AppService", "") :
                serviceAttribute.Module.Replace("AppService", "");

            return $"api/{_controllerName}/{action.ActionName.Replace("Async", "")}";
        }
    }
}



