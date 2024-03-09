using System.Reflection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace com.hmdp.utils;

public static class RedisHelper
{
    public static HashEntry[] ToHashEntries(this object obj)
    {
        PropertyInfo[] properties = obj.GetType().GetProperties();
        return properties
            .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
            .Select
            (
                property =>
                {
                    object propertyValue = property.GetValue(obj);
                    string hashValue;

                    // This will detect if given property value is 
                    // enumerable, which is a good reason to serialize it
                    // as JSON!
                    if (propertyValue is IEnumerable<object>)
                    {
                        // So you use JSON.NET to serialize the property
                        // value as JSON
                        hashValue = JsonConvert.SerializeObject(propertyValue);
                    }
                    else
                    {
                        hashValue = propertyValue.ToString();
                    }

                    return new HashEntry(property.Name, hashValue);
                }
            )
            .ToArray();
    }

    public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();
        var obj = Activator.CreateInstance(typeof(T));
        foreach (var property in properties)
        {
            HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
            if (entry.Equals(new HashEntry())) continue;
            property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
        }
        return (T)obj;
    }
}