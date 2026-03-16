using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace CodingGame.Presentation.UI
{
    /// <summary>
    /// Represents the current state of the game
    /// </summary>
    public enum GameState
    {
        None,
        Idle,
        Win,
        Lose
    }

    /// <summary>
    /// Displays the current status of the game.
    /// </summary>
    public sealed class GameStatusView : MonoBehaviour
    {

        [SerializeField] private GameStateVisualObject[] stateVisuals_;

        private GameState currentState_ = GameState.None;

        /// <summary>
        /// Shows the result.
        /// </summary>
        public void SetState(GameState state)
        {
            if (state == currentState_)
            {
                return;
            }

            currentState_ = state;

            for (int i = 0; i < stateVisuals_.Length; i++)
            {
                if (stateVisuals_[i].visuals == null)
                {
                    continue;
                }

                stateVisuals_[i].visuals.SetActive(
                    stateVisuals_[i].gameState == state);
            }
        }
    }

    /// <summary>
    /// Defines the visuals for a given game state
    /// </summary>
    [Serializable]
    public class GameStateVisualObject
    {
        public GameState gameState;
        public GameObject visuals;
    }
}