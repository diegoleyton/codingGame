using Flowbit.Utilities.Core.Events;

namespace Flowbit.GameBase.Definitions
{
    /// <summary>
    /// Event called when an instruction is added to the program
    /// </summary>
    public sealed class OnInstructionAdded : IEvent
    {
        public OnInstructionAdded(InstructionType instructionType)
        {
            InstructionType = instructionType;
        }

        public InstructionType InstructionType { get; }
    }

    /// <summary>
    /// Event called when an instruction is removed from the program
    /// </summary>
    public sealed class OnInstructionRemoved : IEvent { }

    /// <summary>
    /// Event called when all instructions are removed from the program
    /// </summary>
    public sealed class OnAllInstructionsRemoved : IEvent { }

    /// <summary>
    /// Event called when we go to a program step
    /// </summary>
    public sealed class OnProgramStep : IEvent { }

    /// <summary>
    /// Event called when we go to a program step
    /// </summary>
    public sealed class OnProgramStopped : IEvent { }
}