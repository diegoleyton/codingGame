using UnityEngine;
using UnityEngine.UI;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Small runtime HUD that shows the current editable time.
    /// </summary>
    public sealed class MovingGameRankingHudView : MonoBehaviour
    {
        [SerializeField] private Text elapsedTimeText_;

        public void SetElapsedTime(string elapsedTime)
        {
            if (elapsedTimeText_ != null)
            {
                elapsedTimeText_.text = elapsedTime ?? string.Empty;
            }
        }

        public void Clear()
        {
            if (elapsedTimeText_ != null)
            {
                elapsedTimeText_.text = string.Empty;
            }
        }
    }
}
