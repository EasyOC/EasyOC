﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Panda.DynamicWebApi
{
    public class DynamicWebApiOptions
    {
        public DynamicWebApiOptions()
        {
            RemoveControllerPostfixes = new List<string>() { "AppService", "ApplicationService" };
            RemoveActionPostfixes = new List<string>() { "Async" };
            FormBodyBindingIgnoredTypes = new List<Type>() { typeof(IFormFile) };
            DefaultHttpVerb = "POST";
            DefaultApiPrefix = "api";
            AssemblyDynamicWebApiOptions = new Dictionary<Assembly, AssemblyDynamicWebApiOptions>();
        }


        /// <summary>
        /// API HTTP Verb.
        /// <para></para>
        /// Default value is "POST".
        /// </summary>
        public string DefaultHttpVerb { get; set; }

        public string DefaultAreaName { get; set; }

        /// <summary>
        /// Routing prefix for all APIs
        /// <para></para>
        /// Default value is "api".
        /// </summary>
        public string DefaultApiPrefix { get; set; }

        /// <summary>
        /// Remove the dynamic API class(Controller) name postfix.
        /// <para></para>
        /// Default value is {"AppService", "ApplicationService"}.
        /// </summary>
        public List<string> RemoveControllerPostfixes { get; set; }

        /// <summary>
        /// Remove the dynamic API class's method(Action) postfix.
        /// <para></para>
        /// Default value is {"Async"}.
        /// </summary>
        public List<string> RemoveActionPostfixes { get; set; }

        /// <summary>
        /// Ignore MVC Form Binding types.
        /// </summary>
        public List<Type> FormBodyBindingIgnoredTypes { get; set; }

        /// <summary>
        /// The method that processing the name of the action.
        /// </summary>
        public Func<string, string> GetRestFulActionName { get; set; }

        /// <summary>
        /// Specifies the dynamic webapi options for the assembly.
        /// </summary>
        public Dictionary<Assembly, AssemblyDynamicWebApiOptions> AssemblyDynamicWebApiOptions { get; }

        public ISelectController SelectController { get; set; } = new DefaultSelectController();
        public IActionRouteFactory ActionRouteFactory { get; set; } = new DefaultActionRouteFactory();

        /// <summary>
        /// Verify that all configurations are valid
        /// </summary>
        public void Valid()
        {
            if (string.IsNullOrEmpty(DefaultHttpVerb))
            {
                throw new ArgumentException($"{nameof(DefaultHttpVerb)} can not be empty.");
            }

            if (string.IsNullOrEmpty(DefaultAreaName))
            {
                DefaultAreaName = string.Empty;
            }

            if (string.IsNullOrEmpty(DefaultApiPrefix))
            {
                DefaultApiPrefix = string.Empty;
            }

            if (FormBodyBindingIgnoredTypes == null)
            {
                throw new ArgumentException($"{nameof(FormBodyBindingIgnoredTypes)} can not be null.");
            }

            if (RemoveControllerPostfixes == null)
            {
                throw new ArgumentException($"{nameof(RemoveControllerPostfixes)} can not be null.");
            }
        }

        /// <summary>
        /// Add the dynamic webapi options for the assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="apiPreFix"></param>
        /// <param name="httpVerb"></param>
        public void AddAssemblyOptions(Assembly assembly, string apiPreFix = null, string httpVerb = null)
        {
            if (assembly == null)
            {
                throw new ArgumentException($"{nameof(assembly)} can not be null.");
            }

            this.AssemblyDynamicWebApiOptions[assembly] = new AssemblyDynamicWebApiOptions(apiPreFix, httpVerb);
        }
    }
}