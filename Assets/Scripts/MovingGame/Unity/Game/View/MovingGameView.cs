using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flowbit.GameBase.Character;
using Flowbit.MovingGame.Core;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Updates the visual representation of the moving game in the scene.
    /// </summary>
    public sealed class MovingGameView : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private Transform character_;

        [SerializeField]
        private PetAnimation petAnimation_;

        [Header("Food")]
        [SerializeField]
        private GameObject foodPrefab_;

        [SerializeField]
        private Transform foodRoot_;

        [SerializeField]
        private GameObject consumedFoodEffectPrefab_;

        [SerializeField]
        private Transform consumedFoodEffectRoot_;

        [Header("Break")]
        [SerializeField]
        private GameObject breakPrefab_;

        [SerializeField]
        private Transform breakRoot_;

        [SerializeField]
        private float breakDelaySeconds_ = 0.25f;

        [Header("Animation")]
        [SerializeField]
        private float moveDurationSeconds_ = 0.2f;

        [SerializeField]
        private float moveAnimationSeconds_ = 0.2f;

        [SerializeField]
        private float rotateDurationSeconds_ = 0.15f;

        [SerializeField]
        private float attackDurationSeconds_ = 0.25f;

        [SerializeField]
        private bool animateMovement_ = true;

        [SerializeField]
        private bool animateRotation_ = true;

        private readonly List<GameObject> spawnedFood_ = new List<GameObject>();
        private readonly HashSet<GridPosition> lastFoodPositions_ = new HashSet<GridPosition>();

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

            RebuildFoodVisuals(true);
            UpdateCharacterImmediate();
            SetPetAnimationState(PetAnimationStateType.Idle);
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
                RebuildFoodVisuals(false);
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
                RebuildFoodVisuals(false);
                SetPetAnimationState(PetAnimationStateType.Idle);
                yield break;
            }

            if (shouldAnimateMovement)
            {
                SetPetAnimationState(PetAnimationStateType.Walk);
            }

            float elapsed = 0f;
            bool returnedToIdleAfterMoveAnimation = false;

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

                if (!returnedToIdleAfterMoveAnimation && elapsed >= moveAnimationSeconds_)
                {
                    SetPetAnimationState(PetAnimationStateType.Idle);
                    returnedToIdleAfterMoveAnimation = true;
                }

                yield return null;
            }

            character_.position = targetPosition;
            character_.rotation = targetRotation;
            RebuildFoodVisuals(false);
            SetPetAnimationState(PetAnimationStateType.Idle);
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
            RebuildFoodVisuals(false);

            // Siempre hace la animación de ataque,
            // aunque no haya nada al frente o no sea quebrable.
            SetPetAnimationState(PetAnimationStateType.Attack);

            if (attackDurationSeconds_ > 0f)
            {
                yield return new WaitForSeconds(attackDurationSeconds_);
            }

            // Solo muestra el efecto visual de quiebre si realmente se rompió algo quebrable.
            if (stepAfterProcess.HasPosition() && stepAfterProcess.IsBreakable())
            {
                SpawnBreakPrefab(stepAfterProcess.GetPosition());

                if (breakDelaySeconds_ > 0f)
                {
                    yield return new WaitForSeconds(breakDelaySeconds_);
                }
            }

            SetPetAnimationState(PetAnimationStateType.Idle);
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

        private void RebuildFoodVisuals(bool skipConsumedFoodEffect)
        {
            HashSet<GridPosition> currentFoodPositions_ = new HashSet<GridPosition>();

            if (game_ != null)
            {
                IReadOnlyCollection<GridPosition> foodPositions_ = game_.GetFoodPositions();

                foreach (GridPosition foodPosition in foodPositions_)
                {
                    currentFoodPositions_.Add(foodPosition);
                }
            }

            if (!skipConsumedFoodEffect)
            {
                foreach (GridPosition previousFoodPosition_ in lastFoodPositions_)
                {
                    if (!currentFoodPositions_.Contains(previousFoodPosition_))
                    {
                        SpawnConsumedFoodEffect(previousFoodPosition_);
                    }
                }
            }

            ClearFoodVisuals();

            if (foodPrefab_ != null && game_ != null)
            {
                Transform parent_ = foodRoot_ != null ? foodRoot_ : transform;

                foreach (GridPosition foodPosition_ in currentFoodPositions_)
                {
                    GameObject foodObject_ = Instantiate(
                        foodPrefab_,
                        GridToWorld(foodPosition_),
                        Quaternion.identity,
                        parent_);

                    foodObject_.name = $"Food_{foodPosition_.GetX()}_{foodPosition_.GetY()}";
                    spawnedFood_.Add(foodObject_);
                }
            }

            lastFoodPositions_.Clear();

            foreach (GridPosition foodPosition_ in currentFoodPositions_)
            {
                lastFoodPositions_.Add(foodPosition_);
            }
        }

        private void SpawnConsumedFoodEffect(GridPosition position)
        {
            if (consumedFoodEffectPrefab_ == null)
            {
                return;
            }

            Transform parent_ = consumedFoodEffectRoot_ != null
                ? consumedFoodEffectRoot_
                : transform;

            GameObject effectObject_ = Instantiate(
                consumedFoodEffectPrefab_,
                GridToWorld(position),
                Quaternion.identity,
                parent_);

            effectObject_.name = $"ConsumedFood_{position.GetX()}_{position.GetY()}";
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

        private void SetPetAnimationState(PetAnimationStateType stateType)
        {
            if (petAnimation_ == null)
            {
                return;
            }

            petAnimation_.SetState(stateType);
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