using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtensions
{
    /// <summary>
    /// 将对象转换成字典
    /// </summary>
    /// <param name="value"></param>
    public static Dictionary<string, object> ToDictionary(this object value)
    {
        var dictionary = new Dictionary<string, object>();
        if (value != null)
        {
            if (value is IDictionary dic)
            {
                foreach (DictionaryEntry e in dic)
                {
                    dictionary.Add(e.Key.ToString(), e.Value);
                }

                return dictionary;
            }

            foreach (var property in value.GetType().GetProperties())
            {
                var obj = property.GetValue(value, null);
                dictionary.Add(property.Name, obj);
            }
        }

        return dictionary;
    }
}