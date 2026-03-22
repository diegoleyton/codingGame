using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.MovingGame.Core.Levels;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Represents one button inside the level selection UI.
    /// </summary>
    public sealed class LevelSelectionButtonView : MonoBehaviour
    {
        [SerializeField] private Button button_;
        [SerializeField] private Text titleText_;
        [SerializeField] private Image dificultyImage_;

        [SerializeField] private DificultyVisuals[] dificultyVisuals_;

        private Dictionary<Dificulty, DificultyVisuals> dificultyVisualsMap_;

        private void Awake()
        {
            if (dificultyVisualsMap_ != null)
                return;

            dificultyVisualsMap_ = new Dictionary<Dificulty, DificultyVisuals>();

            foreach (var entry in dificultyVisuals_)
            {
                if (dificultyVisualsMap_.ContainsKey(entry.Dificulty))
                {
                    throw new Exception(
                        $"Duplicate UI mapping found for dificulty type {entry.Dificulty}");
                }
                dificultyVisualsMap_[entry.Dificulty] = entry;
            }
        }

        /// <summary>
        /// Sets the main title text.
        /// </summary>
        public void SetTitle(string title, int index)
        {
            if (titleText_ != null)
            {
                titleText_.text = "" + (index + 1);
            }
        }

        /// <summary>
        /// Sets the dificulty.
        /// </summary>
        public void SetDificulty(Dificulty dificulty)
        {
            if (dificultyImage_ == null)
            {
                return;
            }

            if (!dificultyVisualsMap_.TryGetValue(dificulty, out var dificultyVisual))
            {
                throw new Exception($"No UI visual registered for dificulty type {dificulty}");
            }

            dificultyImage_.color = dificultyVisual.Color;
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

    [Serializable]
    public class DificultyVisuals
    {
        public Dificulty Dificulty;
        public Color Color;
    }
}