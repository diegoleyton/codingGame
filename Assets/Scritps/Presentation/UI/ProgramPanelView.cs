using System.Collections.Generic;
using UnityEngine;
using CodingGame.Runtime.Core;

namespace CodingGame.Presentation.UI
{
    /// <summary>
    /// Displays the current program as a vertical list of instruction items.
    /// </summary>
    public sealed class ProgramPanelView : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot_;
        [SerializeField] private InstructionListItemView itemPrefab_;

        [SerializeField] private InstructionsPresentationSettings instructionsPresentationSettings_;

        private readonly List<InstructionListItemView> spawnedItems_ =
            new List<InstructionListItemView>();

        /// <summary>
        /// Rebuilds the visual list for the given program.
        /// </summary>
        public void Rebuild(IReadOnlyList<IReadOnlyInstructionInstance> instructions)
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

                GameObject instructionUi = instructionsPresentationSettings_.CreateInstructionUi(instructions[i].GetDefinition().GetInstructionType());
                item.SetInstructionView(instructionUi);
                item.SetInstructionLabel(BuildInstructionLabel(instructions[i]));
                item.SetHighlighted(false);

                spawnedItems_.Add(item);
            }
        }

        /// <summary>
        /// Highlights the item at the given index.
        /// </summary>
        public void HighlightIndex(int index)
        {
            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                spawnedItems_[i].SetHighlighted(i == index);
            }
        }

        /// <summary>
        /// Clears any current highlight.
        /// </summary>
        public void ClearHighlight()
        {
            for (int i = 0; i < spawnedItems_.Count; i++)
            {
                spawnedItems_[i].SetHighlighted(false);
            }
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