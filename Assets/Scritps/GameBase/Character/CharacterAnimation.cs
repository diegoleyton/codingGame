using System;
using UnityEngine;

namespace Flowbit.GameBase.Character
{
    /// <summary>
    /// Represents the supported character animation states.
    /// </summary>
    public enum CharacterAnimationStateType
    {
        Idle = 0,
        Think = 1,
        Celebrate = 2,
    }

    /// <summary>
    /// Controls a UI character Animator and enables or disables state-specific objects
    /// when the Animator changes state.
    /// </summary>
    public sealed class CharacterAnimation : MonoBehaviour
    {
        private const string THINKING_PARAMETER_NAME = "IsThinking";
        private const string IS_CELEBRATING_NAME = "IsCelebrating";

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private CharacterAnimationStateObjects[] stateObjects;

        private CharacterAnimationStateType? currentAppliedState;

        /// <summary>
        /// Gets the Animator used by this character.
        /// </summary>
        public Animator Animator => animator;

        private void Awake()
        {
            if (animator == null)
            {
                Debug.LogError($"{nameof(CharacterAnimation)} on {gameObject.name} is missing an Animator reference.", this);
            }
        }

        private void Start()
        {
            RefreshStateObjectsFromAnimator();
        }

        private void Update()
        {
            RefreshStateObjectsFromAnimator();
        }

        /// <summary>
        /// Requests a new animation state in the Animator.
        /// </summary>
        /// <param name="stateType">The state to activate.</param>
        public void SetState(CharacterAnimationStateType stateType)
        {
            if (animator == null)
            {
                Debug.LogError($"{nameof(CharacterAnimation)} on {gameObject.name} cannot set state because Animator is null.", this);
                return;
            }

            animator.SetBool(THINKING_PARAMETER_NAME, stateType == CharacterAnimationStateType.Think);
            animator.SetBool(IS_CELEBRATING_NAME, stateType == CharacterAnimationStateType.Celebrate);
        }

        private void RefreshStateObjectsFromAnimator()
        {
            if (animator == null)
            {
                return;
            }

            AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (!TryGetAnimatorStateType(currentStateInfo, out CharacterAnimationStateType stateType))
            {
                return;
            }

            if (currentAppliedState.HasValue && currentAppliedState.Value == stateType)
            {
                return;
            }

            ApplyObjectsForState(stateType);
            currentAppliedState = stateType;
        }

        private bool TryGetAnimatorStateType(AnimatorStateInfo stateInfo, out CharacterAnimationStateType stateType)
        {
            foreach (CharacterAnimationStateType candidate in Enum.GetValues(typeof(CharacterAnimationStateType)))
            {
                if (stateInfo.IsName(candidate.ToString()))
                {
                    stateType = candidate;
                    return true;
                }
            }

            stateType = default;
            return false;
        }

        private void ApplyObjectsForState(CharacterAnimationStateType stateType)
        {
            DisableAllConfiguredObjects();

            CharacterAnimationStateObjects matchingEntry = GetStateObjectsEntry(stateType);
            if (matchingEntry == null)
            {
                return;
            }

            GameObject[] objectsToEnable = matchingEntry.ObjectsToEnable;
            if (objectsToEnable == null)
            {
                return;
            }

            for (int i = 0; i < objectsToEnable.Length; i++)
            {
                GameObject targetObject = objectsToEnable[i];
                if (targetObject != null)
                {
                    targetObject.SetActive(true);
                }
            }
        }

        private void DisableAllConfiguredObjects()
        {
            if (stateObjects == null)
            {
                return;
            }

            for (int i = 0; i < stateObjects.Length; i++)
            {
                CharacterAnimationStateObjects entry = stateObjects[i];
                if (entry == null || entry.ObjectsToEnable == null)
                {
                    continue;
                }

                GameObject[] objects = entry.ObjectsToEnable;
                for (int j = 0; j < objects.Length; j++)
                {
                    GameObject targetObject = objects[j];
                    if (targetObject != null)
                    {
                        targetObject.SetActive(false);
                    }
                }
            }
        }

        private CharacterAnimationStateObjects GetStateObjectsEntry(CharacterAnimationStateType stateType)
        {
            if (stateObjects == null)
            {
                return null;
            }

            for (int i = 0; i < stateObjects.Length; i++)
            {
                CharacterAnimationStateObjects entry = stateObjects[i];
                if (entry != null && entry.StateType == stateType)
                {
                    return entry;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Maps a character animation state to the objects that should be active while that state is playing.
    /// </summary>
    [Serializable]
    public sealed class CharacterAnimationStateObjects
    {
        [SerializeField]
        private CharacterAnimationStateType stateType;

        [SerializeField]
        private GameObject[] objectsToEnable;

        /// <summary>
        /// Gets the animation state type associated with this entry.
        /// </summary>
        public CharacterAnimationStateType StateType => stateType;

        /// <summary>
        /// Gets the objects that should be enabled for this state.
        /// </summary>
        public GameObject[] ObjectsToEnable => objectsToEnable;
    }
}