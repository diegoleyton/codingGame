using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Engine;
using Flowbit.EngineController;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Services;
using Flowbit.Utilities.Unity.UI;
using Flowbit.Utilities.Core.Events;


namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Displays the current program as a vertical list of instruction items.
    /// </summary>
    public sealed class ProgramPanelView : ProgramViewBase<InstructionType>
    {
        [SerializeField]
        private Transform contentRoot_;

        [SerializeField]
        private InstructionListItemView itemPrefab_;

        [SerializeField]
        private InstructionsPresentationSettings instructionsPresentationSettings_;

        [SerializeField]
        private GameObject instructionButtonContainer_;

        [SerializeField]
        private ScrollRect scrollRect_;

        [SerializeField]
        private float autoScrollTime_ = 0.2f;

        [SerializeField]
        private ScrollRectInteractionTracker scrollRectInteractionTracker_;

        [SerializeField]
        private float autoScrollInteractionCooldown_ = 0.3f;

        private readonly List<InstructionListItemView> spawnedItems_ =
            new List<InstructionListItemView>();

        private Action<int> onInstructionSelected_;
        private Coroutine autoScrollCoroutine_;
        private bool suppressNextHighlightAutoScroll_;
        private EventDispatcher eventDispatcher_;

        private void Start()
        {
            eventDispatcher_ = GlobalServiceContainer.ServiceContainer.Get<EventDispatcher>();
        }

        /// <summary>
        /// Rebuilds the visual list for the given program.
        /// </summary>
        public override void Rebuild(IReadOnlyList<IReadOnlyInstructionInstance<InstructionType>> instructions)
        {
            ClearItems();

            if (instructions == null || contentRoot_ == null || itemPrefab_ == null)
            {
                return;
            }

            for (int i = 0; i < instructions.Count; i++)
            {
                int instructionIndex = i;

                InstructionListItemView item =
                    Instantiate(itemPrefab_, contentRoot_);

                GameObject instructionUi =
                    instructionsPresentationSettings_.CreateInstructionUi(
                        instructions[i].GetDefinition().GetInstructionId());

                item.SetInstructionView(instructionUi);
                item.SetInstructionIndex((i + 1).ToString());
                item.SetInstructionLabel(BuildInstructionLabel(instructions[i]));
                item.SetHighlighted(false);
                item.SetInteractable(true);
                item.SetClickAction(() => OnInstructionSelected(instructionIndex));

                spawnedItems_.Add(item);
            }

            // Prevent the next highlight after a rebuild from triggering a sudden auto-scroll.
            suppressNextHighlightAutoScroll_ = true;
        }

        /// <summary>
        /// Highlights the item at the given index.
        /// </summary>
        public override void HighlightIndex(int index)
        {
            HighlightIndexInternal(index, allowAutoScroll: true);
        }

        /// <summary>
        /// Clears any current highlight.
        /// </summary>
        public override void ClearHighlight()
        {
            StopAutoScrollCoroutine();

            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                spawnedItems_[i].SetHighlighted(false);
            }
        }

        /// <summary>
        /// Enables or disables the instruction editing controls.
        /// </summary>
        public override void EnableInstructions(bool enabled)
        {
            instructionButtonContainer_?.SetActive(enabled);
        }

        /// <summary>
        /// Sets a callback invoked when the user clicks an instruction item.
        /// </summary>
        public override void SetInstructionSelectedCallback(Action<int> onInstructionSelected)
        {
            onInstructionSelected_ = onInstructionSelected;
        }

        /// <summary>
        /// Highlights the item at the given index without triggering auto-scroll.
        /// Useful after rebuilding the list or preserving state while removing instructions.
        /// </summary>
        public void HighlightIndexWithoutAutoScroll(int index)
        {
            HighlightIndexInternal(index, allowAutoScroll: false);
        }

        private void OnInstructionSelected(int index)
        {
            eventDispatcher_.Send(new OnProgramStep());
            onInstructionSelected_?.Invoke(index);
        }

        private void HighlightIndexInternal(int index, bool allowAutoScroll)
        {
            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                spawnedItems_[i].SetHighlighted(i == index);
            }

            if (index < 0 || index >= spawnedItems_.Count || scrollRect_ == null)
            {
                suppressNextHighlightAutoScroll_ = false;
                return;
            }

            if (!allowAutoScroll || suppressNextHighlightAutoScroll_ || !CanAutoScroll())
            {
                suppressNextHighlightAutoScroll_ = false;
                return;
            }

            StopAutoScrollCoroutine();
            autoScrollCoroutine_ = StartCoroutine(ScrollToHighlightedItemNextFrame(index));
            suppressNextHighlightAutoScroll_ = false;
        }

        private IEnumerator ScrollToHighlightedItemNextFrame(int index)
        {
            // Wait one frame so layout groups / content size fitters can settle after rebuilds.
            yield return null;

            if (index < 0 || index >= spawnedItems_.Count || scrollRect_ == null)
            {
                autoScrollCoroutine_ = null;
                yield break;
            }

            if (!CanAutoScroll())
            {
                autoScrollCoroutine_ = null;
                yield break;
            }

            yield return scrollRect_.ScrollItemToTopSlotAnimated(
                spawnedItems_[index].GetRectTransform(),
                autoScrollTime_);

            autoScrollCoroutine_ = null;
        }

        private bool CanAutoScroll()
        {
            if (scrollRect_ == null)
            {
                return false;
            }

            if (scrollRectInteractionTracker_ == null)
            {
                return true;
            }

            if (scrollRectInteractionTracker_.IsDragging)
            {
                return false;
            }

            if (Time.time - scrollRectInteractionTracker_.LastInteractionTime < autoScrollInteractionCooldown_)
            {
                return false;
            }

            return true;
        }

        private void StopAutoScrollCoroutine()
        {
            if (autoScrollCoroutine_ != null)
            {
                StopCoroutine(autoScrollCoroutine_);
                autoScrollCoroutine_ = null;
            }
        }

        private void ClearItems()
        {
            StopAutoScrollCoroutine();

            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                if (spawnedItems_[i] != null)
                {
                    Destroy(spawnedItems_[i].gameObject);
                }
            }

            spawnedItems_.Clear();
        }

        private string BuildInstructionLabel(IReadOnlyInstructionInstance<InstructionType> instruction)
        {
            if (instruction == null)
            {
                return "Null";
            }

            string displayName = instruction.GetDefinition().GetDisplayName();
            List<string> parts = new List<string>();

            foreach (var parameter in instruction.GetParameterValues())
            {
                parts.Add($"{parameter.Key}: {parameter.Value}");
            }

            if (parts.Count == 0)
            {
                return displayName;
            }

            return $"{displayName} ({string.Join(", ", parts)})";
        }
    }
}