using System;

/// <summary>
/// Represents the definition of a single instruction parameter.
/// </summary>
public sealed class InstructionParameterDefinition
{
    private readonly string name_;
    private readonly Type parameterType_;
    private readonly object defaultValue_;

    /// <summary>
    /// Creates a new instruction parameter definition.
    /// </summary>
    public InstructionParameterDefinition(string name, Type parameterType, object defaultValue)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be null or whitespace.", nameof(name));
        }

        name_ = name;
        parameterType_ = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
        defaultValue_ = defaultValue;
    }

    /// <summary>
    /// Returns the parameter name.
    /// </summary>
    public string GetName()
    {
        return name_;
    }

    /// <summary>
    /// Returns the parameter type.
    /// </summary>
    public Type GetParameterType()
    {
        return parameterType_;
    }

    /// <summary>
    /// Returns the default parameter value.
    /// </summary>
    public object GetDefaultValue()
    {
        return defaultValue_;
    }
}