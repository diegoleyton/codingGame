using UnityEngine;
using CodingGame.Presentation.Character;
using System;

namespace CodingGame.Presentation.UI
{
    /// <summary>
    /// Displays the current status of the game.
    /// </summary>
    public sealed class GameStatusView : MonoBehaviour
    {

        [SerializeField] private CharacterAnimation characterAnimation_;

        /// <summary>
        /// Sets the win state
        /// </summary>
        public void Win()
        {
            characterAnimation_.SetState(CharacterAnimationStateType.Celebrate);
        }

        /// <summary>
        /// Sets the lose state
        /// </summary>
        public void Lose()
        {
            characterAnimation_.SetState(CharacterAnimationStateType.Think);
        }

        /// <summary>
        /// Sets the idle state
        /// </summary>
        public void Idle()
        {
            characterAnimation_.SetState(CharacterAnimationStateType.Idle);
        }
    }
}