using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AutoBogus;

public static class ObjectExtensions
{
  public static object GetDefault<T>() => default(T);

  public static bool IsDefaultValue(this PropertyInfo property, object instance)
  {
    if (instance == null)
    {
      throw new ArgumentNullException(nameof(instance));
    }

    var existingValue = property.GetValue(instance);

    var delegateMethod = (Func<object>)typeof(ObjectExtensions).GetMethod(nameof(GetDefault))
      .MakeGenericMethod(property.PropertyType)
      .CreateDelegate(typeof(Func<object>));

    var defaultValue = delegateMethod.Invoke();
    var propertyComparerType = typeof(EqualityComparer<>).MakeGenericType(property.PropertyType);
    var defaultMethod = propertyComparerType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
    var propertyComparer = defaultMethod.GetValue(null) as IEqualityComparer;
    return existingValue == defaultValue || propertyComparer.Equals(existingValue, defaultValue);
  }

}
