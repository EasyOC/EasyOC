using EasyOC.Core.Application;
using EasyOC.OrchardCore.WorkflowPlus.Models;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.OrchardCore.WorkflowPlus.Servcie
{
    public class WorkflowApiAppService : AppServiceBase, IWorkflowApiAppService
    {


        /// <summary>
        /// 尝试使用反射获取所有 工作流的JS扩展方法（IGlobalMethodProvider）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GlobalMethodDto> ListAllGlobalMethods()
        {
            var tps = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(t => t.IsAssignableFrom(typeof(IGlobalMethodProvider))));
            var methods = tps;
            //methods
            return ScriptingManager.GlobalMethodProviders.SelectMany(x =>
            {
                var globalMethods = x.GetMethods();
                var dtos = new List<GlobalMethodDto>();
                foreach (var method in globalMethods)
                {
                    var dto = new GlobalMethodDto();
                    dto.Name = method.Name;
                    dtos.Add(dto);
                }
                return dtos;
            });
        }
    }
}



