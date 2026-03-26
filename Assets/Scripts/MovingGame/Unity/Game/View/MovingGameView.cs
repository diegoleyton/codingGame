using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flowbit.MovingGame.Core;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Updates the visual representation of the moving game in the scene.
    /// </summary>
    public sealed class MovingGameView : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform character_;

        [Header("Food")]
        [SerializeField] private GameObject foodPrefab_;
        [SerializeField] private Transform foodRoot_;

        [Header("Break")]
        [SerializeField] private GameObject breakPrefab_;
        [SerializeField] private Transform breakRoot_;
        [SerializeField] private float breakDelaySeconds_ = 0.25f;

        [Header("Animation")]
        [SerializeField] private float moveDurationSeconds_ = 0.2f;
        [SerializeField] private float rotateDurationSeconds_ = 0.15f;
        [SerializeField] private bool animateMovement_ = true;
        [SerializeField] private bool animateRotation_ = true;

        private readonly List<GameObject> spawnedFood_ = new List<GameObject>();
        private GridRenderer gridRenderer_;
        private Core.MovingGame game_;

        /// <summary>
        /// Initializes the view with the given grid renderer and game.
        /// </summary>
        public void Initialize(GridRenderer gridRenderer, Core.MovingGame game)
        {
            gridRenderer_ = gridRenderer;
            game_ = game;
        }

        /// <summary>
        /// Refreshes the scene objects immediately using the given game state.
        /// </summary>
        public void RefreshImmediate(Core.MovingGame game)
        {
            game_ = game;
            RebuildFoodVisuals();
            UpdateCharacterImmediate();
        }

        /// <summary>
        /// Refreshes the scene objects with simple animation.
        /// </summary>
        public IEnumerator RefreshAnimated(Core.MovingGame game)
        {
            game_ = game;

            if (game_ == null)
            {
                yield break;
            }

            if (character_ == null)
            {
                RebuildFoodVisuals();
                yield break;
            }

            Vector3 targetPosition = GridToWorld(game_.GetCharacterPosition());
            Quaternion targetRotation = DirectionToRotation(game_.GetCharacterDirection());

            Vector3 startPosition = character_.position;
            Quaternion startRotation = character_.rotation;

            bool shouldAnimateMovement = animateMovement_ &&
                                         (startPosition - targetPosition).sqrMagnitude > 0.0001f;
            bool shouldAnimateRotation = animateRotation_ &&
                                         Quaternion.Angle(startRotation, targetRotation) > 0.1f;

            float duration = 0f;

            if (shouldAnimateMovement && shouldAnimateRotation)
            {
                duration = Mathf.Max(moveDurationSeconds_, rotateDurationSeconds_);
            }
            else if (shouldAnimateMovement)
            {
                duration = moveDurationSeconds_;
            }
            else if (shouldAnimateRotation)
            {
                duration = rotateDurationSeconds_;
            }

            if (duration <= 0f)
            {
                character_.position = targetPosition;
                character_.rotation = targetRotation;
                RebuildFoodVisuals();
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = EaseOutQuad(t);

                if (shouldAnimateMovement)
                {
                    character_.position = Vector3.Lerp(startPosition, targetPosition, easedT);
                }

                if (shouldAnimateRotation)
                {
                    character_.rotation = Quaternion.Lerp(startRotation, targetRotation, easedT);
                }

                yield return null;
            }

            character_.position = targetPosition;
            character_.rotation = targetRotation;
            RebuildFoodVisuals();
        }

        /// <summary>
        /// Executes the given step after-process.
        /// </summary>
        public IEnumerator ExecuteStepAfterProcess(StepAfterProcess stepAfterProcess, Core.MovingGame game)
        {
            game_ = game;

            switch (stepAfterProcess.GetProcessType())
            {
                case StepAfterProcessType.None:
                    yield break;

                case StepAfterProcessType.Move:
                case StepAfterProcessType.Rotate:
                    yield return RefreshAnimated(game_);
                    yield break;

                case StepAfterProcessType.Break:
                    yield return ExecuteBreakAfterProcess(stepAfterProcess);
                    yield break;
            }
        }

        private IEnumerator ExecuteBreakAfterProcess(StepAfterProcess stepAfterProcess)
        {
            UpdateCharacterImmediate();
            RebuildFoodVisuals();

            if (!stepAfterProcess.HasPosition())
            {
                yield break;
            }

            SpawnBreakPrefab(stepAfterProcess.GetPosition());

            if (!stepAfterProcess.IsBreakable())
            {
                yield break;
            }

            if (breakDelaySeconds_ > 0f)
            {
                yield return new WaitForSeconds(breakDelaySeconds_);
            }
        }

        private void SpawnBreakPrefab(GridPosition position)
        {
            if (breakPrefab_ == null)
            {
                return;
            }

            Transform parent = breakRoot_ != null ? breakRoot_ : transform;

            GameObject breakObject = Instantiate(
                breakPrefab_,
                GridToWorld(position),
                Quaternion.identity,
                parent);

            breakObject.name = $"Break_{position.GetX()}_{position.GetY()}";
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

        private void RebuildFoodVisuals()
        {
            ClearFoodVisuals();

            if (foodPrefab_ == null || game_ == null)
            {
                return;
            }

            Transform parent = foodRoot_ != null ? foodRoot_ : transform;
            IReadOnlyCollection<GridPosition> foodPositions = game_.GetFoodPositions();

            foreach (GridPosition foodPosition in foodPositions)
            {
                GameObject foodObject = Instantiate(
                    foodPrefab_,
                    GridToWorld(foodPosition),
                    Quaternion.identity,
                    parent);

                foodObject.name = $"Food_{foodPosition.GetX()}_{foodPosition.GetY()}";
                spawnedFood_.Add(foodObject);
            }
        }

        private void ClearFoodVisuals()
        {
            for (int i = 0; i < spawnedFood_.Count; i++)
            {
                if (spawnedFood_[i] != null)
                {
                    Destroy(spawnedFood_[i]);
                }
            }

            spawnedFood_.Clear();
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

        private float EaseOutQuad(float t)
        {
            return 1f - ((1f - t) * (1f - t));
        }
    }
}