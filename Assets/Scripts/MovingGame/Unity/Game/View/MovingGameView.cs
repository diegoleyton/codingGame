using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Flowbit.GameBase.Character;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.UI;
using Flowbit.GameBase.Services;
using Flowbit.MovingGame.Core;
using Flowbit.Utilities.Core.Events;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Updates the visual representation of the moving game in the scene.
    /// Also rebuilds the list of currently available instruction items.
    /// </summary>
    public sealed class MovingGameView : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform character_;
        [SerializeField] private PetAnimation petAnimation_;

        [Header("Available Instructions UI")]
        [SerializeField] private Transform availableInstructionsRoot_;
        [SerializeField] private InstructionListItemView availableInstructionItemPrefab_;

        [Header("Food")]
        [SerializeField] private GameObject foodPrefab_;
        [SerializeField] private Transform foodRoot_;
        [SerializeField] private GameObject consumedFoodEffectPrefab_;
        [SerializeField] private Transform consumedFoodEffectRoot_;

        [Header("Break")]
        [SerializeField] private GameObject breakPrefab_;
        [SerializeField] private Transform breakRoot_;
        [SerializeField] private float breakDelaySeconds_ = 0.25f;

        [Header("Jump")]
        [SerializeField] private float jumpDelaySeconds_ = 0.2f;
        [SerializeField] private float jumpDurationSeconds_ = 0.35f;
        [SerializeField] private float jumpArcHeight_ = 0.45f;

        [Header("Animation")]
        [SerializeField] private float moveDurationSeconds_ = 0.2f;
        [SerializeField] private float moveAnimationSeconds_ = 0.2f;
        [SerializeField] private float rotateDurationSeconds_ = 0.15f;
        [SerializeField] private float attackDurationSeconds_ = 0.25f;
        [SerializeField] private bool animateMovement_ = true;
        [SerializeField] private bool animateRotation_ = true;

        private readonly List<GameObject> spawnedFood_ = new List<GameObject>();
        private readonly HashSet<GridPosition> lastFoodPositions_ = new HashSet<GridPosition>();

        private GridRenderer gridRenderer_;
        private Core.MovingGame game_;
        private EventDispatcher eventDispatcher_;
        private GridPosition lastCharacterGridPosition_;

        /// <summary>
        /// Initializes the view with the given grid renderer and game.
        /// </summary>
        public void Initialize(GridRenderer gridRenderer, Core.MovingGame game)
        {
            gridRenderer_ = gridRenderer;
            game_ = game;
            eventDispatcher_ = GlobalServiceContainer.ServiceContainer.Get<EventDispatcher>();

            if (game_ != null)
            {
                lastCharacterGridPosition_ = game_.GetCharacterPosition();
            }
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

            if (game_ != null)
            {
                lastCharacterGridPosition_ = game_.GetCharacterPosition();
            }
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
                lastCharacterGridPosition_ = game_.GetCharacterPosition();
                yield break;
            }

            GridPosition startGridPosition = lastCharacterGridPosition_;
            GridPosition endGridPosition = game_.GetCharacterPosition();

            Vector3 startPosition = character_.position;
            Vector3 targetPosition = GridToWorld(endGridPosition);

            Quaternion startRotation = character_.rotation;
            Quaternion targetRotation = DirectionToRotation(game_.GetCharacterDirection());

            bool changedGridPosition = !startGridPosition.Equals(endGridPosition);

            bool shouldAnimateMovement = animateMovement_ &&
                                         changedGridPosition &&
                                         (startPosition - targetPosition).sqrMagnitude > 0.0001f;

            bool shouldAnimateRotation = animateRotation_ &&
                                         Quaternion.Angle(startRotation, targetRotation) > 0.1f;

            float duration = 0f;

            if (shouldAnimateMovement && shouldAnimateRotation)
            {
                eventDispatcher_?.Send(new OnMovingGameMove());
                duration = Mathf.Max(moveDurationSeconds_, rotateDurationSeconds_);
            }
            else if (shouldAnimateMovement)
            {
                eventDispatcher_?.Send(new OnMovingGameMove());
                duration = moveDurationSeconds_;
            }
            else if (shouldAnimateRotation)
            {
                eventDispatcher_?.Send(new OnMovingGameRotate());
                duration = rotateDurationSeconds_;
            }

            if (duration <= 0f)
            {
                character_.position = targetPosition;
                character_.rotation = targetRotation;
                RebuildFoodVisuals(false);
                SendSwitchEventIfNeeded(changedGridPosition);
                SetPetAnimationState(PetAnimationStateType.Idle);
                lastCharacterGridPosition_ = endGridPosition;
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
            SendSwitchEventIfNeeded(changedGridPosition);
            SetPetAnimationState(PetAnimationStateType.Idle);
            lastCharacterGridPosition_ = endGridPosition;
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

                case StepAfterProcessType.Jump:
                    yield return ExecuteJumpAfterProcess();
                    yield break;
            }
        }

        private IEnumerator ExecuteBreakAfterProcess(StepAfterProcess stepAfterProcess)
        {
            UpdateCharacterImmediate();
            RebuildFoodVisuals(false);

            SetPetAnimationState(PetAnimationStateType.Attack);

            if (attackDurationSeconds_ > 0f)
            {
                yield return new WaitForSeconds(attackDurationSeconds_);
            }

            if (stepAfterProcess.HasPosition() && stepAfterProcess.IsBreakable())
            {
                SpawnBreakPrefab(stepAfterProcess.GetPosition());
                eventDispatcher_?.Send(new OnMovingGameBreak());

                if (breakDelaySeconds_ > 0f)
                {
                    yield return new WaitForSeconds(breakDelaySeconds_);
                }
            }
            else
            {
                eventDispatcher_?.Send(new OnMovingGameAttack());
            }

            SetPetAnimationState(PetAnimationStateType.Idle);
        }

        private IEnumerator ExecuteJumpAfterProcess()
        {
            if (game_ == null || character_ == null)
            {
                RefreshImmediate(game_);
                yield break;
            }

            GridPosition endGridPosition = game_.GetCharacterPosition();
            Vector3 startPosition = GridToWorld(lastCharacterGridPosition_);
            Vector3 endPosition = GridToWorld(endGridPosition);

            eventDispatcher_?.Send(new OnMovingGameMove());
            SetPetAnimationState(PetAnimationStateType.Walk);

            float duration = jumpDurationSeconds_ > 0f ? jumpDurationSeconds_ : moveDurationSeconds_;

            if (duration <= 0f)
            {
                character_.position = endPosition;
            }
            else
            {
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    float easedT = EaseOutQuad(t);
                    Vector3 position = Vector3.Lerp(startPosition, endPosition, easedT);
                    float arcOffset = 4f * jumpArcHeight_ * t * (1f - t);
                    position += Vector3.up * arcOffset;
                    character_.position = position;
                    yield return null;
                }
            }

            character_.position = endPosition;
            character_.rotation = DirectionToRotation(game_.GetCharacterDirection());
            RebuildFoodVisuals(false);

            if (jumpDelaySeconds_ > 0f)
            {
                yield return new WaitForSeconds(jumpDelaySeconds_);
            }

            SendSwitchEventIfNeeded(!lastCharacterGridPosition_.Equals(endGridPosition));
            SetPetAnimationState(PetAnimationStateType.Idle);
            lastCharacterGridPosition_ = endGridPosition;
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
            HashSet<GridPosition> currentFoodPositions = new HashSet<GridPosition>();

            if (game_ != null)
            {
                IReadOnlyCollection<GridPosition> foodPositions = game_.GetFoodPositions();

                foreach (GridPosition foodPosition in foodPositions)
                {
                    currentFoodPositions.Add(foodPosition);
                }
            }

            if (!skipConsumedFoodEffect)
            {
                foreach (GridPosition previousFoodPosition in lastFoodPositions_)
                {
                    if (!currentFoodPositions.Contains(previousFoodPosition))
                    {
                        SpawnConsumedFoodEffect(previousFoodPosition);
                    }
                }
            }

            ClearFoodVisuals();

            if (foodPrefab_ != null && game_ != null)
            {
                Transform parent = foodRoot_ != null ? foodRoot_ : transform;

                foreach (GridPosition foodPosition in currentFoodPositions)
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

            lastFoodPositions_.Clear();

            foreach (GridPosition foodPosition in currentFoodPositions)
            {
                lastFoodPositions_.Add(foodPosition);
            }
        }

        private void SpawnConsumedFoodEffect(GridPosition position)
        {
            if (consumedFoodEffectPrefab_ == null)
            {
                return;
            }

            Transform parent = consumedFoodEffectRoot_ != null
                ? consumedFoodEffectRoot_
                : transform;

            eventDispatcher_?.Send(new OnMovingGameGoalReached());

            GameObject effectObject = Instantiate(
                consumedFoodEffectPrefab_,
                GridToWorld(position),
                Quaternion.identity,
                parent);

            effectObject.name = $"ConsumedFood_{position.GetX()}_{position.GetY()}";
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

        private void SendSwitchEventIfNeeded(bool changedGridPosition)
        {
            if (!changedGridPosition || game_ == null || eventDispatcher_ == null)
            {
                return;
            }

            if (game_.TryGetToggleSwitchTile(game_.GetCharacterPosition(), out ToggleSwitchTileState toggleSwitch))
            {
                eventDispatcher_.Send(new OnMovingGameSwitch(toggleSwitch.GroupId));
            }
        }
    }
}
