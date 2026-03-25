using UnityEngine;
using UnityEngine.UI;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Image that blocks the screen and user inputs
    /// </summary>
    public class ScreenBlockerImage : MonoBehaviour
    {
        [field: SerializeField]
        public Image Image { get; private set; }
    }
}