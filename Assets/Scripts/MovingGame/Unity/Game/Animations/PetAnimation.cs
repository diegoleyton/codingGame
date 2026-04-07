using UnityEngine;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Represents the supported pet animation states.
    /// </summary>
    public enum PetAnimationStateType
    {
        Idle = 0,
        Walk = 1,
        Attack = 2,
        Jump = 3
    }

    /// <summary>
    /// Controls a pet Animator.
    /// </summary>
    public sealed class PetAnimation : MonoBehaviour
    {
        private const string ATTACK_NAME = "attack";
        private const string WALK_NAME = "walk";

        private const string JUMP_NAME = "jump";

        [SerializeField]
        private Animator animator;

        /// <summary>
        /// Gets the Animator used by this character.
        /// </summary>
        public Animator Animator => animator;

        private void Awake()
        {
            if (animator == null)
            {
                Debug.LogError(
                    $"{nameof(PetAnimation)} on {gameObject.name} is missing an Animator reference.",
                    this);
            }
        }

        /// <summary>
        /// Sets a new animation state.
        /// </summary>
        public void SetState(PetAnimationStateType stateType)
        {
            if (animator == null)
            {
                Debug.LogError(
                    $"{nameof(PetAnimation)} on {gameObject.name} cannot set state because Animator is null.",
                    this);
                return;
            }

            animator.SetBool(WALK_NAME, stateType == PetAnimationStateType.Walk);
            animator.SetBool(JUMP_NAME, stateType == PetAnimationStateType.Jump);

            if (stateType == PetAnimationStateType.Attack)
            {
                animator.SetTrigger(ATTACK_NAME);
            }
        }
    }
}