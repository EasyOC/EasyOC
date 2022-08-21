using Fluid;
using Fluid.Values;
using OrchardCore.Liquid;
using OrchardCore.Users.Indexes;
using OrchardCore.Users.Models;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Services;

namespace EasyOC.OrchardCore.Scripting.Liquid
{

    public class UsersByUserNameFilter : ILiquidFilter
    {
        private readonly ISession _session;

        public UsersByUserNameFilter(ISession session)
        {
            _session = session;
        }

        public async ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext ctx)
        {
            if (input.Type == FluidValues.Array)
            {
                // List of user ids
                var userNames = input.Enumerate(ctx).Select(x => x.ToStringValue().ToUpper()).ToArray();

                return FluidValue.Create(await _session.Query<User, UserIndex>(x => x.NormalizedUserName.IsIn(userNames)).ListAsync(), ctx.Options);
            }

            var userName = input.ToStringValue().ToUpper();

            return FluidValue.Create(await _session.Query<User, UserIndex>(x => x.NormalizedUserName == userName).FirstOrDefaultAsync(), ctx.Options);
        }
    }

}
