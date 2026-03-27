using System;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Scenes;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Popup shown for selecting a level.
    /// </summary>
    public sealed class MovingGameLevelSelectionPopupView : PopupBase
    {
        [Header("UI")]
        [SerializeField] private Text titleText_;
        [SerializeField] private Text indexText_;
        [SerializeField] private Text hintText_;
        [SerializeField] private Button continueButton_;
        [SerializeField] private Button closeButton_;

        private Action continueAction_;

        protected override void Initialize()
        {
            var popupParams = GetSceneParameters<MovingGameLevelSelectionPopupParams>();
            continueAction_ = popupParams.OnContinue;

            if (titleText_ != null)
            {
                titleText_.text = popupParams.MovingGameLevelData.name;
            }

            if (hintText_ != null)
            {
                hintText_.text = popupParams.MovingGameLevelData.hint;
            }

            if (indexText_ != null)
            {
                indexText_.text = (popupParams.Index + 1).ToString();
            }

            if (continueButton_ != null)
            {
                continueButton_.onClick.RemoveAllListeners();
                continueButton_.onClick.AddListener(OnContinuePressed);
            }

            if (closeButton_ != null)
            {
                closeButton_.onClick.RemoveAllListeners();
                closeButton_.onClick.AddListener(Close);
            }
        }

        private void OnContinuePressed()
        {
            continueAction_?.Invoke();
        }
    }
}