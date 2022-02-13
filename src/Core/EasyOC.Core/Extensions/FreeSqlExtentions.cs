using Microsoft.Extensions.DependencyInjection;

namespace EasyOC
{
    public static class FreeSqlExtentions
    {
        public static IServiceCollection AddFreeSql(this IServiceCollection services)
        {

            return services.AddSingleton(serviceProvider => serviceProvider.GetFreeSql());
        }
    }
}



