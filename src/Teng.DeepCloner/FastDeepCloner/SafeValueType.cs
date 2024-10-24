using System.Collections.Concurrent;

namespace FastDeepCloner
{
    /// <summary>
    /// CustomValueType Dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="P"></typeparam>
    public class SafeValueType<T, P> : ConcurrentDictionary<T, P> where T : notnull
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dictionary"></param>
        public SafeValueType(ConcurrentDictionary<T, P>? dictionary = null)
        {
            if (dictionary == null)
            {
                return;
            }

            foreach (var item in dictionary)
            {
                TryAdd(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 尝试增加值
        /// </summary>
        public bool TryAdd(T key, P item, bool overwrite = false)
        {
            if (overwrite)
            {
                TryRemove(key, out _);
            }
            else
            {
                if (ContainsKey(key))
                {
                    return true;
                }
            }
            base.TryAdd(key, item);

            return true;
        }

        /// <summary>
        /// 获取或添加值
        /// </summary>
        public P GetOrAdd(T key, P item, bool overwrite = false)
        {
            if (overwrite)
            {
                TryRemove(key, out _);
            }
            else
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }
            }

            base.TryAdd(key, item);
            return base[key];
        }

        /// <summary>
        /// 获取值
        /// </summary>
        public P Get(T key)
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8603 // 可能返回 null 引用。
            object o = null;
            return (P)o;
#pragma warning restore CS8603 // 可能返回 null 引用。
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
        }
    }
}