using System;
using UnityEngine;
using UnityEngine.UI;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Represents one button inside the level selection UI.
    /// </summary>
    public sealed class LevelSelectionButtonView : MonoBehaviour
    {
        [SerializeField] private Button button_;
        [SerializeField] private Text titleText_;
        [SerializeField] private Text subtitleText_;

        /// <summary>
        /// Sets the main title text.
        /// </summary>
        public void SetTitle(string title)
        {
            if (titleText_ != null)
            {
                titleText_.text = title;
            }
        }

        /// <summary>
        /// Sets the subtitle text.
        /// </summary>
        public void SetSubtitle(string subtitle)
        {
            if (subtitleText_ != null)
            {
                subtitleText_.text = subtitle;
            }
        }

        /// <summary>
        /// Sets the click callback for this button.
        /// </summary>
        public void SetOnClick(Action onClick)
        {
            if (button_ == null)
            {
                throw new InvalidOperationException("Button reference is not assigned.");
            }

            button_.onClick.RemoveAllListeners();

            if (onClick != null)
            {
                button_.onClick.AddListener(() => onClick.Invoke());
            }
        }
    }
}