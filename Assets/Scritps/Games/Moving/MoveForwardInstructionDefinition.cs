using System;
using System.Collections.Generic;

/// <summary>
/// Represents a primitive instruction that moves the agent forward.
/// </summary>
public sealed class MoveForwardInstructionDefinition : InstructionDefinitionBase
{
    private readonly InstructionParameterDefinition[] parameterDefinitions_;

    /// <summary>
    /// Creates a new move-forward instruction definition.
    /// </summary>
    public MoveForwardInstructionDefinition()
    {
        parameterDefinitions_ = new[]
        {
            new InstructionParameterDefinition("steps", typeof(int), 1)
        };
    }

    /// <summary>
    /// Returns a unique identifier for this instruction definition.
    /// </summary>
    public override string GetId()
    {
        return "move_forward";
    }

    /// <summary>
    /// Returns the display name of this instruction definition.
    /// </summary>
    public override string GetDisplayName()
    {
        return "Move Forward";
    }

    /// <summary>
    /// Returns whether this instruction executes directly without expanding child instructions.
    /// </summary>
    public override bool IsPrimitive()
    {
        return true;
    }

    /// <summary>
    /// Returns the parameter definitions for this instruction.
    /// </summary>
    public override IReadOnlyList<InstructionParameterDefinition> GetParameterDefinitions()
    {
        return parameterDefinitions_;
    }

    /// <summary>
    /// Executes this instruction instance on the given game.
    /// </summary>
    public override void Execute(IGame game, InstructionInstance instance)
    {
        IMovingGame movingGame = game as IMovingGame
            ?? throw new ArgumentException("Game must implement IMovingGame.", nameof(game));

        int steps = (int)instance.GetParameterValue("steps");
        if (steps <= 0)
        {
            throw new InvalidOperationException("Steps must be greater than zero.");
        }

        movingGame.MoveForward(steps);
    }
}