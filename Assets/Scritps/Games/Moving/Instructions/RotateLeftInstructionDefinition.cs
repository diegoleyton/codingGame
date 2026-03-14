using System;

/// <summary>
/// Represents a primitive instruction that rotates the agent to the left.
/// </summary>
public sealed class RotateLeftInstructionDefinition : GameInstructionDefinitionBase<IMovingGame>
{
    /// <summary>
    /// Returns a unique identifier for this instruction definition.
    /// </summary>
    public override string GetId()
    {
        return "rotate_left";
    }

    /// <summary>
    /// Returns the display name of this instruction definition.
    /// </summary>
    public override string GetDisplayName()
    {
        return "Rotate Left";
    }

    /// <summary>
    /// Executes this instruction instance on the given game.
    /// </summary>
    protected override void ExecuteTyped(IMovingGame game, InstructionInstance instance)
    {
        game.RotateLeft();
    }
}