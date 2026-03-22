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

        [SerializeField] private float autoScrollTime_ = 0.2f;

        private readonly List<InstructionListItemView> spawnedItems_ =
            new List<InstructionListItemView>();

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
                InstructionListItemView item =
                    Instantiate(itemPrefab_, contentRoot_);

                GameObject instructionUi = instructionsPresentationSettings_.CreateInstructionUi((InstructionType)instructions[i].GetDefinition().GetInstructionId());
                item.SetInstructionView(instructionUi);
                item.SetInstructionIndex("" + (i + 1));
                item.SetInstructionLabel(BuildInstructionLabel(instructions[i]));
                item.SetHighlighted(false);

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
                if (highlighted)
                {
                    StartCoroutine(scrollRect_.ScrollItemToTopSlotAnimated(spawnedItems_[i].GetRectTransform(), autoScrollTime_));
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
            instructionButtonContainer_.SetActive(enabled);
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
    }
}