using System;
using System.Collections.Generic;

namespace CodingGame.Runtime.Core
{
    /// <summary>
    /// Represents one concrete use of an instruction definition inside a program.
    /// </summary>
    public sealed class InstructionInstance
    {
        private readonly IInstructionDefinition definition_;
        private readonly Dictionary<string, object> parameterValues_;
        private readonly List<InstructionInstance> children_;

        /// <summary>
        /// Creates a new instruction instance using the definition's default parameter values.
        /// </summary>
        public InstructionInstance(IInstructionDefinition definition)
        {
            definition_ = definition ?? throw new ArgumentNullException(nameof(definition));
            parameterValues_ = new Dictionary<string, object>();
            children_ = new List<InstructionInstance>();

            IReadOnlyList<InstructionParameterDefinition> parameterDefinitions = definition_.GetParameterDefinitions();

            foreach (InstructionParameterDefinition parameterDefinition in parameterDefinitions)
            {
                parameterValues_[parameterDefinition.GetName()] = parameterDefinition.GetDefaultValue();
            }
        }

        /// <summary>
        /// Returns the instruction definition used by this instance.
        /// </summary>
        public IInstructionDefinition GetDefinition()
        {
            return definition_;
        }

        /// <summary>
        /// Returns the parameter value for the given parameter name.
        /// </summary>
        public object GetParameterValue(string parameterName)
        {
            if (!parameterValues_.TryGetValue(parameterName, out object value))
            {
                throw new ArgumentException($"Unknown parameter '{parameterName}'.", nameof(parameterName));
            }

            return value;
        }

        /// <summary>
        /// Sets the parameter value for the given parameter name.
        /// </summary>
        public void SetParameterValue(string parameterName, object value)
        {
            bool exists = false;
            Type expectedType = null;

            foreach (InstructionParameterDefinition parameterDefinition in definition_.GetParameterDefinitions())
            {
                if (parameterDefinition.GetName() == parameterName)
                {
                    exists = true;
                    expectedType = parameterDefinition.GetParameterType();
                    break;
                }
            }

            if (!exists)
            {
                throw new ArgumentException($"Unknown parameter '{parameterName}'.", nameof(parameterName));
            }

            if (value != null && !expectedType.IsInstanceOfType(value))
            {
                throw new ArgumentException(
                    $"Parameter '{parameterName}' expects type '{expectedType.Name}' but got '{value.GetType().Name}'.",
                    nameof(value));
            }

            parameterValues_[parameterName] = value;
        }

        /// <summary>
        /// Returns the child instruction instances of this instance.
        /// </summary>
        public IReadOnlyList<InstructionInstance> GetChildren()
        {
            return children_;
        }

        /// <summary>
        /// Adds a child instruction instance.
        /// </summary>
        public void AddChild(InstructionInstance child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (!definition_.SupportsChildren())
            {
                throw new InvalidOperationException($"{definition_.GetDisplayName()} does not support child instructions.");
            }

            children_.Add(child);
        }
    }
}