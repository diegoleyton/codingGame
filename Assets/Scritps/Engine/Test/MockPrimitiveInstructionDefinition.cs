using Flowbit.Engine;
using Flowbit.Engine.Instructions;
using System;
using System.Collections.Generic;

/// <summary>
/// Simple primitive instruction definition used by tests.
/// </summary>
internal sealed class MockPrimitiveInstructionDefinition
    : GameInstructionDefinitionBase<GameMock>
{
    private readonly string id_;
    private readonly string displayName_;
    private readonly string logEntry_;
    private readonly bool supportsAmountParameter_;
    private readonly IReadOnlyList<InstructionParameterDefinition> parameterDefinitions_;

    /// <summary>
    /// Creates a new mock primitive instruction definition.
    /// </summary>
    public MockPrimitiveInstructionDefinition(
        string id,
        string displayName,
        string logEntry,
        bool supportsAmountParameter = false)
    {
        id_ = id;
        displayName_ = displayName;
        logEntry_ = logEntry;
        supportsAmountParameter_ = supportsAmountParameter;

        if (supportsAmountParameter_)
        {
            parameterDefinitions_ = new[]
            {
                    new InstructionParameterDefinition("amount", typeof(int), 1)
                };
        }
        else
        {
            parameterDefinitions_ = Array.Empty<InstructionParameterDefinition>();
        }
    }

    /// <summary>
    /// Returns the display name.
    /// </summary>
    public override string GetDisplayName()
    {
        return displayName_;
    }

    /// <summary>
    /// Returns the instruction ID.
    /// </summary>
    public override int GetInstructionId()
    {
        return 1;
    }

    /// <summary>
    /// Returns whether this instruction is primitive.
    /// </summary>
    public override bool IsPrimitive()
    {
        return true;
    }

    /// <summary>
    /// Returns the parameter definitions.
    /// </summary>
    public override IReadOnlyList<InstructionParameterDefinition> GetParameterDefinitions()
    {
        return parameterDefinitions_;
    }

    /// <summary>
    /// Executes this instruction on the test game.
    /// </summary>
    protected override void ExecuteTyped(GameMock game, InstructionInstance instance)
    {
        if (supportsAmountParameter_)
        {
            int amount = (int)instance.GetParameterValue("amount");
            game.Log($"{logEntry_}({amount})");
            return;
        }

        game.Log(logEntry_);
    }
}