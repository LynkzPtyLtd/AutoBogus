using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoBogus;

public static class PropertyExtensions
{
  public static void Generate(this PropertyInfo property, object parent, AutoGenerateOverrideContext context)
  {
    Generate(property, parent, context.GenerateContext);
  }

  public static void Generate(this PropertyInfo property, object parent, AutoGenerateContext context)
  {
    context.Binder.PopulateInstance(parent, context, new[] { property });
  }

  public static void Set<TSource, TProperty>(this TSource obj,
    Expression<Func<TSource, TProperty>> property,
    AutoGenerateOverrideContext context)
    => Set(obj, property, context.GenerateContext);

  public static void Set<TSource, TProperty>(this TSource obj,
    Expression<Func<TSource, TProperty>> property,
    AutoGenerateContext context)
  {
    var propertyInfo = GetProperty(property);
    var runtimeProperties = obj.GetType()
      .GetProperties();
    var match = runtimeProperties
      .FirstOrDefault(info => info.Name == propertyInfo.Name && info.CanWrite);

    if (match == null)
    {
      return;
    }

    var propertyContext = new AutoGenerateContext(context.Faker, context.Config);
    Generate(match, obj, propertyContext);
  }

  public static void Set<TSource, TProperty>(this TSource obj,
    Expression<Func<TSource, TProperty>> property,
    TProperty value)
  {
    var propertyInfo = GetProperty(property);
    var runtimeProperties = obj.GetType()
      .GetProperties();
    var match = runtimeProperties
      .FirstOrDefault(info => info.Name == propertyInfo.Name && info.CanWrite);

    match?.SetValue(obj, value);
  }

  internal static PropertyInfo GetProperty<TSource, TProperty>(Expression<Func<TSource, TProperty>> member)
  {
    if (member == null)
    {
      return null;
    }

    MemberExpression expression;

    if (member.Body is UnaryExpression unary)
    {
      expression = unary.Operand as MemberExpression;
    }
    else
    {
      expression = member.Body as MemberExpression;
    }

    var memberInfo = expression?.Member;

    return memberInfo as PropertyInfo;
  }
}
