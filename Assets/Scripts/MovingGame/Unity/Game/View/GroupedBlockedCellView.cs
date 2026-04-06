using Flowbit.GameBase.Services;
using Flowbit.GameBase.GamesSettings;
using UnityEngine;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Controls the visual state of a grouped blocked cell.
    /// </summary>
    public sealed class GroupedBlockedCellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] colorTargets_;
        [SerializeField] private GameObject onObject_;
        [SerializeField] private GameObject offObject_;

        MovingGameSettings gameSettings_;

        private int groupId_;

        /// <summary>
        /// Initializes the cell with the given group id and power state.
        /// </summary>
        public void Initialize(int groupId, bool isOn)
        {
            groupId_ = groupId;
            gameSettings_ = GlobalServiceContainer.ServiceContainer.Get<GameResources>().MovingGameSettings;
            ApplyVisualState(isOn);
        }

        /// <summary>
        /// Refreshes the on/off visual state.
        /// </summary>
        public void RefreshState(bool isOn)
        {
            ApplyVisualState(isOn);
        }

        private void ApplyVisualState(bool isOn)
        {
            Color color = Color.white;

            if (gameSettings_ != null)
            {
                color = gameSettings_.GetColorForGroupId(groupId_);
            }

            onObject_.SetActive(isOn);
            offObject_.SetActive(!isOn);

            for (int i = 0; i < colorTargets_.Length; i++)
            {
                if (colorTargets_[i] != null)
                {
                    colorTargets_[i].color = color;
                }
            }
        }
    }
}