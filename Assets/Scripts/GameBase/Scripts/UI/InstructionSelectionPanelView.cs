using System;
using System.Collections.Generic;

using UnityEngine;

using Flowbit.EngineController;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Services;
using Flowbit.Utilities.Core.Events;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Handles the view of the available instructions for a game
    /// </summary>
    public sealed class InstructionSelectionPanelView : InstructionSelectionPanelViewBase<InstructionType>
    {
        [Header("Available Instructions UI")]
        [SerializeField] private Transform availableInstructionsRoot_;
        [SerializeField] private InstructionListItemView availableInstructionItemPrefab_;

        private readonly List<InstructionListItemView> spawnedInstructionItems_ =
                    new List<InstructionListItemView>();
        private EventDispatcher eventDispatcher_;
        private Action<InstructionType> onInstructionClicked_;

        private InstructionsPresentationSettings instructionsPresentationSettings_;

        private void Start()
        {
            instructionsPresentationSettings_ = GlobalServiceContainer.ServiceContainer.Get<GameResources>().InstructionsPresentationSettings;
            eventDispatcher_ = GlobalServiceContainer.ServiceContainer.Get<EventDispatcher>();
        }

        /// <summary>
        /// Sets the list of available instructions
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="onInstructionClicked"></param>
        public override void SetAvailableInstructions(
            IReadOnlyList<InstructionType> instructions,
            Action<InstructionType> onInstructionClicked)
        {
            ClearAvailableInstructionItems();

            if (availableInstructionsRoot_ == null ||
                availableInstructionItemPrefab_ == null ||
                instructionsPresentationSettings_ == null ||
                instructions == null)
            {
                return;
            }

            onInstructionClicked_ = onInstructionClicked;
            for (int i = 0; i < instructions.Count; i++)
            {
                InstructionType instructionType = instructions[i];

                InstructionListItemView itemView = Instantiate(
                    availableInstructionItemPrefab_,
                    availableInstructionsRoot_);

                itemView.name = $"{instructionType}Item";

                GameObject instructionUi =
                    instructionsPresentationSettings_.CreateInstructionUi(instructionType);

                itemView.SetInstructionView(instructionUi);
                itemView.SetClickAction(() => OnInstructionClicked(instructionType));
                itemView.SetInteractable(true);

                spawnedInstructionItems_.Add(itemView);
            }
        }

        private void OnInstructionClicked(InstructionType instructionType)
        {
            eventDispatcher_.Send(new OnInstructionAdded(instructionType));
            onInstructionClicked_?.Invoke(instructionType);
        }

        private void ClearAvailableInstructionItems()
        {
            for (int i = 0; i < spawnedInstructionItems_.Count; i++)
            {
                if (spawnedInstructionItems_[i] != null)
                {
                    Destroy(spawnedInstructionItems_[i].gameObject);
                }
            }

            spawnedInstructionItems_.Clear();
        }
    }
}