using Bogus;
using System;
using System.Collections.Generic;

namespace AutoBogus
{
  /// <summary>
  /// A class that provides context when overriding a generate request.
  /// </summary>
  public sealed class AutoGenerateOverrideContext
  {
    internal AutoGenerateOverrideContext(AutoGenerateContext generateContext)
    {
      GenerateContext = generateContext;

      GenerateType = GenerateContext.GenerateType;
      GenerateName = GenerateContext.GenerateName;
      Faker = GenerateContext.Faker;
      RuleSets = GenerateContext.RuleSets;
      Rules = GenerateContext.Rules;
    }

    /// <summary>
    /// The instance generated during the override.
    /// </summary>
    public object Instance { get; set; }

    /// <summary>
    /// The type associated with the current generate request.
    /// </summary>
    public Type GenerateType { get; }

    /// <summary>
    /// The name associated with the current generate request.
    /// </summary>
    public string GenerateName { get; }

    /// <summary>
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public Faker Faker { get; }

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public IEnumerable<string> RuleSets { get; }

    /// <summary>
    /// The AutoBogus rules for the generate request.
    /// </summary>
    public IReadOnlyDictionary<Type, RuleBuilder> Rules { get; }

    internal AutoGenerateContext GenerateContext { get; }
  }
}
