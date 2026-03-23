using UnityEngine;
using Flowbit.Utilities.Unity.Navigation;
using Flowbit.GameBase.UI;
using Flowbit.GameBase.Scenes;

namespace Flowbit.GameBase.Services
{
    /// <summary>
    /// ScriptableObject that contains all the resources for the game.
    /// </summary>
    [CreateAssetMenu(fileName = "GameResources", menuName = "Flowbit/Game Resources", order = 0)]
    public class GameResources : ScriptableObject
    {
        /// <summary>
        /// Gets the settings for all the screens in the game.
        /// </summary>
        [field: SerializeField]
        public SceneSettings SceneSettings { get; private set; }

        /// <summary>
        /// Gets the game navigation installer prefab.
        /// </summary>
        [field: SerializeField]
        public UnityNavigationInstaller NavigationInstallerPrefab { get; private set; }

        /// <summary>
        /// Gets the resources that define the presentation of instructions.
        /// </summary>
        [field: SerializeField]
        public InstructionsPresentationSettings InstructionsPresentationSettings { get; private set; }

        /// <summary>
        /// Gets the resources that define the presentation of instructions.
        /// </summary>
        [field: SerializeField]
        public ComponentsLoopAnimator ComponentLoopAnimator { get; private set; }

        /// <summary>
        /// Gets the scene transition overlay for scene navigation
        /// </summary>
        [field: SerializeField]
        public SceneTransitionOverlay SceneTransitionOverlay { get; private set; }
    }
}