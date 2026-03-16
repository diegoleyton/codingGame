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

        /// <summary>
        /// Shows the result.
        /// </summary>
        public void SetState(GameState state)
        {
            for (int i = 0; i < stateVisuals_.Length; i++)
            {
                stateVisuals_[i].visuals.SetActive(stateVisuals_[i].gameState == state);
            }
        }
    }

    [Serializable]
    public class GameStateVisualObject
    {
        public GameState gameState;
        public GameObject visuals;
    }
}