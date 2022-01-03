using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EasyOC
{
    public static class EnumExtensions
    {
        public static string ToDescriptionOrString(this Enum item)
        {
            var name = item.ToString();
            var desc = item.GetType().GetField(name)?.GetCustomAttributes(typeof(DescriptionAttribute), false)?.FirstOrDefault() as DescriptionAttribute;
            return desc?.Description ?? name;
        }

    }
}
