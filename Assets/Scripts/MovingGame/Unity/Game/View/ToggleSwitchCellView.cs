using Flowbit.GameBase.Services;
using Flowbit.GameBase.GamesSettings;
using UnityEngine;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Controls the visual state of a grouped toggle switch cell.
    /// </summary>
    public sealed class ToggleSwitchCellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] colorTargets_;

        /// <summary>
        /// Initializes the cell with the given group id.
        /// </summary>
        public void Initialize(int groupId)
        {
            GameResources gameResources = GlobalServiceContainer.ServiceContainer.Get<GameResources>();

            Color color = Color.white;

            if (gameResources != null && gameResources.MovingGameSettings != null)
            {
                color = gameResources.MovingGameSettings.GetColorForGroupId(groupId);
            }

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