using System;

/// <summary>
/// Represents a primitive instruction that rotates the agent to the right.
/// </summary>
public sealed class RotateRightInstructionDefinition : InstructionDefinitionBase
{
    /// <summary>
    /// Returns a unique identifier for this instruction definition.
    /// </summary>
    public override string GetId()
    {
        return "rotate_right";
    }

    /// <summary>
    /// Returns the display name of this instruction definition.
    /// </summary>
    public override string GetDisplayName()
    {
        return "Rotate Right";
    }

    /// <summary>
    /// Returns whether this instruction executes directly without expanding child instructions.
    /// </summary>
    public override bool IsPrimitive()
    {
        return true;
    }

    /// <summary>
    /// Executes this instruction instance on the given game.
    /// </summary>
    public override void Execute(IGame game, InstructionInstance instance)
    {
        IMovingGame movingGame = game as IMovingGame
            ?? throw new ArgumentException("Game must implement IMovingGame.", nameof(game));

        movingGame.RotateRight();
    }
}