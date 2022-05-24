using System;
using System.Collections.Generic;
using System.Reflection;
using EasyOC;
using Natasha.CSharp;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.CSharpScript.Services
{
    public class CSharpScriptProvider : ICSharpScriptProvider
    {
        private static bool _initState = false;
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private AssemblyCSharpBuilder _builder;

        public virtual Task<AssemblyCSharpBuilder> GetAssemblyCSharpBuilderAsync(
            bool useGlobalSharedBuilder = true)
        {
            // if (!_initState)
            // {
            //     _initState = true;
            //     await NatashaInitializer.InitializeAndPreheating();
            // }

            if (useGlobalSharedBuilder)
            {
                return Task.FromResult(_builder ??= new AssemblyCSharpBuilder());
            }
            else
            {
                return Task.FromResult(new AssemblyCSharpBuilder());
            }
        }

        public virtual Type GetIndexType(string fullTypeName)
        {
            return _types.ContainsKey(fullTypeName) ? _types[fullTypeName] : null;
        }

        public virtual async Task<Type> GetOrCreateAsync(string fullName, string cSharpScripts,
            HashSet<string>? usings = default)
        {
            if (_types.ContainsKey(fullName))
            {
                return _types[fullName];
            }
            else
            {
                var builder = await GetAssemblyCSharpBuilderAsync();
                builder.Domain = DomainManagement.CurrentDomain;
                builder.Domain.UsingRecorder.Using(usings);
                builder.Add(cSharpScripts);
                var asm = builder.GetAssembly();
                var type = asm.GetType(fullName);
                _types.Add(fullName, type);
                return type;
            }
        }

        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="cSharpScripts"></param>
        /// <param name="usings"> 只包含命名空间，不含 using  xxx.xxx.xxx ；
        ///如： System.Text</param>
        /// <returns></returns>
        public virtual async Task<Type> CreateTypeAsync(string fullName, string cSharpScripts,
            HashSet<string>? usings = default)
        {
            try
            {
                var builder = await GetAssemblyCSharpBuilderAsync();
                //只包含命名空间，不含 using  xxx.xxx.xxx ；
                //如： System.Text
                builder.Domain.UsingRecorder.Using(usings); // 全局引用
                builder.Add(cSharpScripts);
                var asm = builder.GetAssembly();
                var type = asm.GetType(fullName);

                if (_types.ContainsKey(fullName))
                {
                    _types[fullName] = type;
                }
                else
                {
                    _types.Add(fullName, type);
                }

                return type;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
