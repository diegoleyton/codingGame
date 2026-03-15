using UnityEngine;
using CodingGame.Runtime.Games.Moving;

namespace CodingGame.Presentation.Games.Moving
{
    /// <summary>
    /// Updates the visual representation of the moving game in the scene.
    /// </summary>
    public sealed class MovingGameView : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform character_;
        [SerializeField] private Transform food_;

        [Header("Animation")]
        [SerializeField] private bool animateTransitions_ = false;

        private GridRenderer gridRenderer_;
        private Runtime.Games.Moving.MovingGame game_;

        /// <summary>
        /// Initializes the view with the given grid renderer and game.
        /// </summary>
        public void Initialize(
            GridRenderer gridRenderer,
            Runtime.Games.Moving.MovingGame game)
        {
            gridRenderer_ = gridRenderer;
            game_ = game;
        }

        /// <summary>
        /// Refreshes the scene objects immediately using the given game state.
        /// </summary>
        public void RefreshImmediate(Runtime.Games.Moving.MovingGame game)
        {
            game_ = game;
            UpdateFood();
            UpdateCharacterImmediate();
        }

        /// <summary>
        /// Refreshes the scene objects after a game step.
        /// </summary>
        public void RefreshAnimated(Runtime.Games.Moving.MovingGame game)
        {
            game_ = game;

            if (!animateTransitions_)
            {
                RefreshImmediate(game_);
                return;
            }

            UpdateFood();
            UpdateCharacterImmediate();
        }

        private void UpdateCharacterImmediate()
        {
            if (character_ == null || game_ == null)
            {
                return;
            }

            character_.position = GridToWorld(game_.GetCharacterPosition());
            character_.rotation = DirectionToRotation(game_.GetCharacterDirection());
        }

        private void UpdateFood()
        {
            if (food_ == null || game_ == null)
            {
                return;
            }

            food_.position = GridToWorld(game_.GetFoodPosition());
        }

        private Vector3 GridToWorld(GridPosition position)
        {
            if (gridRenderer_ == null)
            {
                return new Vector3(position.GetX(), position.GetY(), 0f);
            }

            return gridRenderer_.GridToWorld(position);
        }

        private Quaternion DirectionToRotation(Direction direction)
        {
            float zRotation = direction switch
            {
                Direction.Up => 0f,
                Direction.Right => -90f,
                Direction.Down => 180f,
                Direction.Left => 90f,
                _ => 0f
            };

            return Quaternion.Euler(0f, 0f, zRotation);
        }
    }
}