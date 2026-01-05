using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtensions 
{
    
    /// <summary>
    /// 是否是默认值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsDefaultValue(this object value)
    {
        if (value == default)
        {
            return true;
        }

        return value switch
        {
            byte s => s == 0,
            sbyte s => s == 0,
            short s => s == 0,
            char s => s == 0,
            bool s => s == false,
            ushort s => s == 0,
            int s => s == 0,
            uint s => s == 0,
            long s => s == 0,
            ulong s => s == 0,
            decimal s => s == 0,
            float s => s == 0,
            double s => s == 0,
            Enum s => Equals(s, Enum.GetValues(value.GetType()).GetValue(0)),
            DateTime s => s == DateTime.MinValue,
            DateTimeOffset s => s == DateTimeOffset.MinValue,
            Guid g => g == Guid.Empty,
            ValueType => Activator.CreateInstance(value.GetType()).Equals(value),
            _ => false
        };
    }
    
    /// <summary>
    /// 判断是否为null，null或0长度都返回true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this T value)
        where T : class
    {
        return value switch
        {
            null => true,
            string s => string.IsNullOrWhiteSpace(s),
            IEnumerable list => !list.GetEnumerator().MoveNext(),
            _ => false
        };
    }
    
    /// <summary>
    /// 转成非null
    /// </summary>
    /// <param name="s"></param>
    /// <param name="value">为空时的替换值</param>
    /// <returns></returns>
    public static T IfNull<T>(this T s, in T value)
    {
        return s ?? value;
    }
    
    /// <summary>
    /// 链式操作
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    public static T2 Next<T1, T2>(this T1 source, Func<T1, T2> action)
    {
        return action(source);
    }
    
    
    
    
    
    
    
    
    
    
}
