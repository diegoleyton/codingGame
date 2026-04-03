namespace Flowbit.Engine.Instructions
{
    /// <summary>
    /// Creates instruction definition objects based on the instruction type.
    /// </summary>
    public interface IInstructionFactory<TGame, TInstruction> where TGame : class, IGame
    {
        /// <summary>
        /// Creates a instruction definition based on the instructionType type.
        /// </summary>
        /// <param name="instructionType"></param>
        /// <returns></returns>
        GameInstructionDefinitionBase<TGame, TInstruction> CreateInstruction(TInstruction instructionType);
    }
}