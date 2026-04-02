using System;
using System.Collections.Generic;
using UnityEngine;
using Flowbit.Engine;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Displays the current program as a list of instruction items.
    /// </summary>
    public abstract class ProgramViewBase : MonoBehaviour
    {
        /// <summary>
        /// Rebuilds the visual list for the given program.
        /// </summary>
        public abstract void Rebuild(IReadOnlyList<IReadOnlyInstructionInstance> instructions);

        /// <summary>
        /// Highlights the item at the given index.
        /// </summary>
        public abstract void HighlightIndex(int index);

        /// <summary>
        /// Clears any current highlight.
        /// </summary>
        public abstract void ClearHighlight();

        /// <summary>
        /// Enables or disables the instruction editing controls.
        /// </summary>
        public abstract void EnableInstructions(bool enabled);

        /// <summary>
        /// Sets a callback invoked when the user clicks an instruction item.
        /// </summary>
        public abstract void SetInstructionSelectedCallback(Action<int> onInstructionSelected);
    }
}