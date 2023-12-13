using System.Collections.Generic;

namespace GSharp;

public static class Utils
{
  internal static TValue TryGetObjectValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
  {
    return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
  }
}