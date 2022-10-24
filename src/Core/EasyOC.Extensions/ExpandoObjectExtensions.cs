using System.Dynamic;

namespace EasyOC
{
    public static class ExpandoObjectExtensions
    {
        public static Dictionary<string, object> ToDictionary(this ExpandoObject input)
        {
            if(input == null)
            {
                return null;
            }
            var inputDict = input as IDictionary<string, object>;
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var item in inputDict.Keys)
            {
                result.Add(item, inputDict[item]);
            }
            return result;
        }
    }
}
