using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Bogus;

namespace AutoBogus;

public class AutoGenerationRule
{
  public PropertyInfo Property { get; set; }
  public Func<Faker, object, object> SetExpression { get; set; }
}

public class RuleBuilder
{
  private readonly Type _type;
  private readonly Dictionary<long, AutoGenerationRule> _rules = new();

  protected RuleBuilder(Type type)
  {
    _type = type;
  }

  public void Property(string propertyName, Func<Faker, object, object> setter)
  {
    var property = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .First(x => x.Name.Equals(propertyName, StringComparison.Ordinal) && x.CanRead && x.CanRead);

    Property(property, setter);
  }

  public void Property(PropertyInfo property, Func<Faker, object, object> setter)
  {
    _rules[property.MetadataToken] = new AutoGenerationRule
    {
      Property = property,
      SetExpression = setter
    };
  }

  public IReadOnlyCollection<AutoGenerationRule> GetRules()
  {
    return new ReadOnlyCollection<AutoGenerationRule>(_rules.Values.ToList());
  }
}

public class RuleBuilder<T> : RuleBuilder
{
  public RuleBuilder()
    : base(typeof(T))
  {
  }

  public void Property(string propertyName, Func<Faker, T, object> setter)
  {
    base.Property(propertyName, (faker, instance) => setter(faker, (T) instance));
  }

  public void Property(PropertyInfo property, Func<Faker, T, object> setter)
  {
    base.Property(property, (faker, instance) => setter(faker, (T)instance));
  }

  public void Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression, Func<Faker, T, TProperty> setter)
  {
    var property = PropertyExtensions.GetProperty(propertyExpression);
    Property(property, (faker, instance) => setter(faker, instance));
  }
}
