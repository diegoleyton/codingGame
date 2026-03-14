using System;

/// <summary>
/// Represents a primitive instruction that rotates the agent to the right.
/// </summary>
public sealed class RotateRightInstructionDefinition : GameInstructionDefinitionBase<IMovingGame>
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
    /// Executes this instruction instance on the given game.
    /// </summary>
    protected override void ExecuteTyped(IMovingGame game, InstructionInstance instance)
    {
        game.RotateRight();
    }
}