using UnityEngine;
using UnityEngine.UI;

namespace CodingGame.Presentation.UI
{
    /// <summary>
    /// Represents one visual item inside the program panel.
    /// </summary>
    public sealed class InstructionListItemView : MonoBehaviour
    {
        [SerializeField] private Transform uiContainer_;

        [SerializeField] private Text text_;
        [SerializeField] private Image background_;

        [Header("Colors")]
        [SerializeField] private Color normalColor_ = Color.white;
        [SerializeField] private Color highlightedColor_ = Color.yellow;

        /// <summary>
        /// Sets the UI visual of an instructions.
        /// </summary>
        public void SetInstructionView(GameObject instructionView)
        {
            if (gameObject != null)
            {
                instructionView.transform.SetParent(uiContainer_, false);
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
    }
}