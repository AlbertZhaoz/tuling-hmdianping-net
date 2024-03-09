using System.Reflection;

namespace com.hmdp.attribute;

[AttributeUsage(AttributeTargets.Property)]
public class AutoWireAttribute:Attribute
{
    
}

public class PropertySelector:Autofac.Core.IPropertySelector
{
    public bool InjectProperty(PropertyInfo propertyInfo, object instance)
    {
        return propertyInfo.GetCustomAttribute<AutoWireAttribute>() != null;
    }
}