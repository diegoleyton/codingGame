using UnityEngine;
using Flowbit.GameBase.Character;
using Flowbit.EngineController;
using System;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Displays the current status of the game.
    /// </summary>
    public sealed class GameStatusView : GameStatusViewBase
    {

        [SerializeField] private CharacterAnimation characterAnimation_;

        /// <summary>
        /// Sets the win state
        /// </summary>
        public override void Win()
        {
            characterAnimation_.SetState(CharacterAnimationStateType.Celebrate);
        }

        /// <summary>
        /// Sets the lose state
        /// </summary>
        public override void Lose()
        {
            characterAnimation_.SetState(CharacterAnimationStateType.Think);
        }

        /// <summary>
        /// Sets the idle state
        /// </summary>
        public override void Idle()
        {
            characterAnimation_.SetState(CharacterAnimationStateType.Idle);
        }
    }
}