using System;
using UnityEngine;
using UnityEngine.UI;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Represents one visual item inside the program panel.
    /// </summary>
    public sealed class InstructionListItemView : MonoBehaviour
    {
        [SerializeField] private Transform uiContainer_;
        [SerializeField] private Text text_;
        [SerializeField] private Text indexText_;
        [SerializeField] private Image background_;
        [SerializeField] private Button button_;

        [Header("Colors")]
        [SerializeField] private Color normalColor_ = Color.white;
        [SerializeField] private Color highlightedColor_ = Color.yellow;

        private Action onClick_;

        private void Awake()
        {
            if (button_ != null)
            {
                button_.onClick.AddListener(HandleClick);
            }
        }

        private void OnDestroy()
        {
            if (button_ != null)
            {
                button_.onClick.RemoveListener(HandleClick);
            }
        }

        /// <summary>
        /// Sets the UI visual of an instruction.
        /// </summary>
        public void SetInstructionView(GameObject instructionView)
        {
            if (instructionView == null || uiContainer_ == null)
            {
                return;
            }

            instructionView.transform.SetParent(uiContainer_, false);
            instructionView.transform.localScale = Vector3.one;
            instructionView.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Sets the instruction click action.
        /// </summary>
        public void SetClickAction(Action onClick)
        {
            onClick_ = onClick;
        }

        /// <summary>
        /// Sets the instruction item interactable state.
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            if (button_ != null)
            {
                button_.interactable = interactable;
            }
        }

        public void SetInstructionIndex(string index)
        {
            if (indexText_ != null)
            {
                indexText_.text = index;
            }
        }

        /// <summary>
        /// Sets the display text of the instruction item.
        /// </summary>
        public void SetInstructionLabel(string text)
        {
            if (text_ != null)
            {
                text_.text = text;
            }
        }

        /// <summary>
        /// Sets whether this instruction item is highlighted.
        /// </summary>
        public void SetHighlighted(bool highlighted)
        {
            if (background_ != null)
            {
                background_.color = highlighted ? highlightedColor_ : normalColor_;
            }
        }

        public RectTransform GetRectTransform()
        {
            return GetComponent<RectTransform>();
        }

        private void HandleClick()
        {
            onClick_?.Invoke();
        }
    }
}