using Fluid;
using Fluid.Values;
using Jint;
using OrchardCore.Liquid;
using System.Threading.Tasks;

namespace EasyOC.Core.LiquidFilters
{
    public class JSFilter : ILiquidFilter
    {
        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext ctx)
        {
            //var workflowContextValue = ctx.GetValue("Workflow");

            //if (workflowContextValue.IsNil())
            //{
            //    throw new ArgumentException("WorkflowExecutionContext missing while invoking 'signal_url'");
            //}

            //var workflowContext = (WorkflowExecutionContext)workflowContextValue.ToObjectValue();
            var engine = new Engine();
            var objValue = input.ToObjectValue();

            var dictResult = engine.SetValue("input", objValue)
                .Evaluate(@"
                                var obj={};
                                for(k in input){
                                    obj[k]=input[k];                              
                                }
                                return JSON.stringify(obj)");
            return new ValueTask<FluidValue>(new StringValue(dictResult.ToString()));

        }


    }
}



