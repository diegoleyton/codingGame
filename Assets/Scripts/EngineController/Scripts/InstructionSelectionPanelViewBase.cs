using System;
using System.Collections.Generic;

using UnityEngine;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Handles the view of the available instructions for a game
    /// </summary>
    /// <typeparam name="TInstruction"></typeparam>
    public abstract class InstructionSelectionPanelViewBase<TInstruction> : MonoBehaviour
    {
        /// <summary>
        /// Sets the list of available instructions
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="onInstructionClicked"></param>
        public abstract void SetAvailableInstructions(IReadOnlyList<TInstruction> instructions, Action<TInstruction> onInstructionClicked);
    }
}