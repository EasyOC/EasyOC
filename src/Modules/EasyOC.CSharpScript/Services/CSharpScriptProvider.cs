using System;
using System.Collections.Generic;
using System.Reflection;
using EasyOC;
using Natasha.CSharp;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EasyOC.CSharpScript.Services
{
    public class CSharpScriptProvider : ICSharpScriptProvider
    {
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private AssemblyCSharpBuilder _builder;
        private readonly ILogger _logger;

        public CSharpScriptProvider(ILogger<CSharpScriptProvider> logger)
        {
            _logger = logger;
        }

        public virtual Task<AssemblyCSharpBuilder> GetAssemblyCSharpBuilderAsync(
            bool useGlobalSharedBuilder = true)
        {
            NatashaInitializer.Preheating();
            return Task.FromResult(new AssemblyCSharpBuilder());
        }

        public virtual async Task<Type> GetOrCreateAsync(string fullName, string cSharpScripts,
            IEnumerable<string> usings = default)
        {
            if (_types.ContainsKey(fullName))
            {
                return _types[fullName];
            }
            else
            {
                var builder = await GetAssemblyCSharpBuilderAsync();
                builder.Domain = DomainManagement.CurrentDomain;
                if (usings != null)
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
            IEnumerable<string> usings = default)
        {
            try
            {
                var builder = await GetAssemblyCSharpBuilderAsync(false);
                //只包含命名空间，不含 using  xxx.xxx.xxx ；
                //如： System.Text
                if (usings != null)
                {
                    builder.Domain.UsingRecorder.Using(usings);// 全局引用
                }
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
                _logger.LogError(e, "脚本编译错误,{error}", e.Message);
                Console.WriteLine(e);
                throw new Exception("模型映射解析错误");
            }
        }
    }
}
