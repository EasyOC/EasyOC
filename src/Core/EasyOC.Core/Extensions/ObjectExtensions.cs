using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace System
{
    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        ///// <summary>
        ///// Used to simplify and beautify casting an object to a type.
        ///// </summary>
        ///// <typeparam name="T">Type to be casted</typeparam>
        ///// <param name="obj">Object to cast</param>
        ///// <returns>Casted object</returns>
        //public static T As<T>(this object obj)
        //    where T : class
        //{
        //    return (T)obj;
        //}

        /// <summary>
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.Type)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }
 
        public static T If<T>(this T obj, bool condition, Func<T, T> func)
        {
            if (condition)
            {
                return func(obj);
            }

            return obj;
        }

        /// <summary>
        /// Can be used to conditionally perform an action
        /// on an object and return the original object.
        /// It is useful for chained calls on the object.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <param name="condition">A condition</param>
        /// <param name="action">An action that is executed only if the condition is <code>true</code></param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>
        /// Returns the original object.
        /// </returns>
        public static T If<T>(this T obj, bool condition, Action<T> action)
        {
            if (condition)
            {
                action(obj);
            }

            return obj;
        }
    }
}
