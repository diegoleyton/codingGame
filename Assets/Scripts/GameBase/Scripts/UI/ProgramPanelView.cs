using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Engine;
using Flowbit.EngineController;
using Flowbit.GameBase.Definitions;
using Flowbit.Utilities.Unity.UI;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Displays the current program as a vertical list of instruction items.
    /// </summary>
    public sealed class ProgramPanelView : ProgramViewBase
    {
        [SerializeField] private Transform contentRoot_;
        [SerializeField] private InstructionListItemView itemPrefab_;
        [SerializeField] private InstructionsPresentationSettings instructionsPresentationSettings_;
        [SerializeField] private GameObject instructionButtonContainer_;
        [SerializeField] private ScrollRect scrollRect_;
        [SerializeField] private ScrollRectInteractionTracker scrollRectTracker_;
        [SerializeField] private float autoScrollTime_ = 0.2f;
        [SerializeField] private GameObject uiBlocker_;

        private readonly List<InstructionListItemView> spawnedItems_ =
            new List<InstructionListItemView>();

        private Action<int> onInstructionSelected_;

        /// <summary>
        /// Rebuilds the visual list for the given program.
        /// </summary>
        public override void Rebuild(IReadOnlyList<IReadOnlyInstructionInstance> instructions)
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
                        (InstructionType)instructions[i].GetDefinition().GetInstructionId());

                item.SetInstructionView(instructionUi);
                item.SetInstructionIndex((i + 1).ToString());
                item.SetInstructionLabel(BuildInstructionLabel(instructions[i]));
                item.SetHighlighted(false);
                item.SetInteractable(true);
                item.SetClickAction(() => onInstructionSelected_?.Invoke(instructionIndex));

                spawnedItems_.Add(item);
            }
        }

        /// <summary>
        /// Highlights the item at the given index.
        /// </summary>
        public override void HighlightIndex(int index)
        {
            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                bool highlighted = i == index;

                if (highlighted && scrollRect_ != null && CanAutoScroll())
                {
                    StartCoroutine(
                        scrollRect_.ScrollItemToTopSlotAnimated(
                            spawnedItems_[i].GetRectTransform(),
                            autoScrollTime_));
                }

                spawnedItems_[i].SetHighlighted(highlighted);
            }
        }

        /// <summary>
        /// Clears any current highlight.
        /// </summary>
        public override void ClearHighlight()
        {
            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                spawnedItems_[i].SetHighlighted(false);
            }
        }

        public override void EnableInstructions(bool enabled)
        {
            instructionButtonContainer_?.SetActive(enabled);

            // The program items themselves must stay clickable even while running,
            // so we do not block the whole panel anymore.
            if (uiBlocker_ != null)
            {
                uiBlocker_.SetActive(false);
            }
        }

        public override void SetInstructionSelectedCallback(Action<int> onInstructionSelected)
        {
            onInstructionSelected_ = onInstructionSelected;
        }

        private void ClearItems()
        {
            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                if (spawnedItems_[i] != null)
                {
                    Destroy(spawnedItems_[i].gameObject);
                }
            }

            spawnedItems_.Clear();
        }

        private string BuildInstructionLabel(IReadOnlyInstructionInstance instruction)
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

        private bool CanAutoScroll()
        {
            if (scrollRectTracker_ == null)
            {
                return true;
            }

            return !scrollRectTracker_.IsDragging;
        }
    }
}