#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using TransformalizeModule.Services.Contracts;
using TransformalizeModule.Services.Modifiers;
using TransformalizeModule.Services.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Containers.Autofac.Modules;
using Process = Transformalize.Configuration.Process;
using ParameterModifier = TransformalizeModule.Services.Modifiers.ParameterModifier;
using OrchardCore.ContentManagement;
using Microsoft.AspNetCore.Http;

namespace TransformalizeModule.Services {

   /// <summary>
   /// This container deals with the arrangement and how it becomes a process.
   /// Transforms and Validators are registered here as well because their 
   /// short-hand is expanded in the arrangement by customizers before it becomes a process.
   /// </summary>
   public class OrchardConfigurationContainer : IConfigurationContainer {

      private readonly CombinedLogger<OrchardConfigurationContainer> _logger;
      private readonly ITransformalizeParametersModifier _transformalizeParameters;
      private readonly ILoadFormModifier _loadFormModifier;
      private readonly IHttpContextAccessor _httpContext;

      public ISerializer Serializer { get; set; }

      public OrchardConfigurationContainer(
         CombinedLogger<OrchardConfigurationContainer> logger,
         ITransformalizeParametersModifier transformalizeParameters,
         ILoadFormModifier loadFormModifier,
         IHttpContextAccessor httpContext
      ) {
         _logger = logger;
         _transformalizeParameters = transformalizeParameters;
         _loadFormModifier = loadFormModifier;
         _httpContext = httpContext;
      }

      public ILifetimeScope CreateScope(string arrangement, ContentItem item, IDictionary<string, string> parameters, bool validateParameters = true) {

         var combinedParameters = parameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

         var builder = new ContainerBuilder();
         builder.RegisterModule(new ShorthandModule(_logger));

         builder.Register(ctx => {

            var dependancies = new List<IDependency>();

            if (item.ContentItem.Has("TransformalizeReportPart")) {
               dependancies.Add(new ReportParameterModifier());
            }

            if (Serializer != null) {
               dependancies.Add(Serializer);
            }

            dependancies.Add(new ParameterModifier(new PlaceHolderReplacer('@', '[', ']')));

            // these were registered by the ShorthandModule are are used to expand shorthand transforms and validators into "longhand".
            dependancies.Add(ctx.ResolveNamed<IDependency>(TransformModule.FieldsName));
            dependancies.Add(ctx.ResolveNamed<IDependency>(TransformModule.ParametersName));
            dependancies.Add(ctx.ResolveNamed<IDependency>(ValidateModule.FieldsName));
            dependancies.Add(ctx.ResolveNamed<IDependency>(ValidateModule.ParametersName));

            string modified = arrangement;
            if (_httpContext.HttpContext.Request.Method == "GET" && item.ContentItem.Has("TransformalizeFormPart")) {
               modified = _loadFormModifier.Modify(arrangement, item.Id, combinedParameters);               
            }

            if (validateParameters) {
               modified = _transformalizeParameters.Modify(arrangement, item.Id, combinedParameters);
            }

            var process = new Process(modified, combinedParameters, dependancies.ToArray());

            if (process.Errors().Any()) {
               _logger.Error(() => "The configuration has errors.");
               foreach (var error in process.Errors()) {
                  _logger.Error(() => error);
               }
            }

            process.Id = item.Id;

            return process;
         }).As<Process>().InstancePerDependency();
         return builder.Build().BeginLifetimeScope();
      }

   }

}
