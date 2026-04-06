using UnityEngine;
using Flowbit.GameBase.UI;
using Flowbit.GameBase.Scenes;
using Flowbit.GameBase.Audio;
using Flowbit.GameBase.GamesSettings;

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
        /// Gets the resources that define the presentation of instructions.
        /// </summary>
        [field: SerializeField]
        public InstructionsPresentationSettings InstructionsPresentationSettings { get; private set; }

        /// <summary>
        /// Gets the back button controller.
        /// </summary>
        [field: SerializeField]
        public BackButton BackButton { get; private set; }

        /// <summary>
        /// Gets the resources that define the presentation of instructions.
        /// </summary>
        [field: SerializeField]
        public ComponentsLoopAnimator ComponentLoopAnimator { get; private set; }

        /// <summary>
        /// Gets the sound library for the game.
        /// </summary>
        [field: SerializeField]
        public GameSoundLibrary GameSoundLibrary { get; private set; }

        /// <summary>
        /// Gets the settings for moving game.
        /// </summary>
        [field: SerializeField]
        public MovingGameSettings MovingGameSettings { get; private set; }
    }
}