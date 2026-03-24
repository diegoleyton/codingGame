using System;

namespace Flowbit.Utilities.Core.Navigation
{
    /// <summary>
    /// Handles the transition between one scene and another.
    /// </summary>
    public interface ISceneTransitionStrategy
    {
        /// <summary>
        /// Play the transition animation and call the next scene action when it is required.
        /// </summary>
        public void RunTransition(Action goToNextSceneAction);
    }
}