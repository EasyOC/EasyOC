using System.Collections.Concurrent;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        //
        // 摘要:
        //     This method is used to try to get a value in a dictionary if it does exists.
        //
        // 参数:
        //   dictionary:
        //     The collection object
        //
        //   key:
        //     Key
        //
        //   value:
        //     Value of the key (or default value if key not exists)
        //
        // 类型参数:
        //   T:
        //     Type of the value
        //
        // 返回结果:
        //     True if key does exists in the dictionary
        internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            if (dictionary.TryGetValue(key, out object value2) && value2 is T)
            {
                value = (T)value2;
                return true;
            }

            value = default(T);
            return false;
        }

        //
        // 摘要:
        //     Gets a value from the dictionary with given key. Returns default value if can
        //     not find.
        //
        // 参数:
        //   dictionary:
        //     Dictionary to check and get
        //
        //   key:
        //     Key to find the value
        //
        // 类型参数:
        //   TKey:
        //     Type of the key
        //
        //   TValue:
        //     Type of the value
        //
        // 返回结果:
        //     Value if found, default if can not found.
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                return default(TValue);
            }

            return value;
        }

        //
        // 摘要:
        //     Gets a value from the dictionary with given key. Returns default value if can
        //     not find.
        //
        // 参数:
        //   dictionary:
        //     Dictionary to check and get
        //
        //   key:
        //     Key to find the value
        //
        // 类型参数:
        //   TKey:
        //     Type of the key
        //
        //   TValue:
        //     Type of the value
        //
        // 返回结果:
        //     Value if found, default if can not found.
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                return default(TValue);
            }

            return value;
        }

        //
        // 摘要:
        //     Gets a value from the dictionary with given key. Returns default value if can
        //     not find.
        //
        // 参数:
        //   dictionary:
        //     Dictionary to check and get
        //
        //   key:
        //     Key to find the value
        //
        // 类型参数:
        //   TKey:
        //     Type of the key
        //
        //   TValue:
        //     Type of the value
        //
        // 返回结果:
        //     Value if found, default if can not found.
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                return default(TValue);
            }

            return value;
        }

        //
        // 摘要:
        //     Gets a value from the dictionary with given key. Returns default value if can
        //     not find.
        //
        // 参数:
        //   dictionary:
        //     Dictionary to check and get
        //
        //   key:
        //     Key to find the value
        //
        // 类型参数:
        //   TKey:
        //     Type of the key
        //
        //   TValue:
        //     Type of the value
        //
        // 返回结果:
        //     Value if found, default if can not found.
        public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                return default(TValue);
            }

            return value;
        }

        //
        // 摘要:
        //     Gets a value from the dictionary with given key. Returns default value if can
        //     not find.
        //
        // 参数:
        //   dictionary:
        //     Dictionary to check and get
        //
        //   key:
        //     Key to find the value
        //
        //   factory:
        //     A factory method used to create the value if not found in the dictionary
        //
        // 类型参数:
        //   TKey:
        //     Type of the key
        //
        //   TValue:
        //     Type of the value
        //
        // 返回结果:
        //     Value if found, default if can not found.
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }

            return dictionary[key] = factory(key);
        }

        //
        // 摘要:
        //     Gets a value from the dictionary with given key. Returns default value if can
        //     not find.
        //
        // 参数:
        //   dictionary:
        //     Dictionary to check and get
        //
        //   key:
        //     Key to find the value
        //
        //   factory:
        //     A factory method used to create the value if not found in the dictionary
        //
        // 类型参数:
        //   TKey:
        //     Type of the key
        //
        //   TValue:
        //     Type of the value
        //
        // 返回结果:
        //     Value if found, default if can not found.
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            return dictionary.GetOrAdd(key, (TKey k) => factory());
        }

        //
        // 摘要:
        //     Gets a value from the concurrent dictionary with given key. Returns default value
        //     if can not find.
        //
        // 参数:
        //   dictionary:
        //     Concurrent dictionary to check and get
        //
        //   key:
        //     Key to find the value
        //
        //   factory:
        //     A factory method used to create the value if not found in the dictionary
        //
        // 类型参数:
        //   TKey:
        //     Type of the key
        //
        //   TValue:
        //     Type of the value
        //
        // 返回结果:
        //     Value if found, default if can not found.
        public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            return dictionary.GetOrAdd(key, (TKey k) => factory());
        }
    }
}
