using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Definitions;
using System;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// ScriptableObject that map scene names with scenes NavigationTargets, so they can be used
    /// by the Navigation Service.
    /// </summary>
    [CreateAssetMenu(fileName = "SceneSettings", menuName = "Flowbit/Scene Settings", order = 0)]
    public class SceneSettings : ScriptableObject
    {
        /// <summary>
        /// List of the navigationTagerts to define all the scenes and screens in the game.
        /// </summary>
        [SerializeField]
        private SceneDefinition[] scenes_;

        [field: SerializeField]
        public GameSceneTransitionBase TransitionNext { get; private set; }

        [field: SerializeField]
        public GameSceneTransitionBase TransitionPrev { get; private set; }

        [field: SerializeField]
        public GameSceneTransitionBase PopupTransitionOpen { get; private set; }

        [field: SerializeField]
        public GameSceneTransitionBase PopupTransitionClose { get; private set; }

        Dictionary<SceneType, NavigationTarget> navigationTargetMap_;
        Dictionary<string, NavigationTarget> navigationTargetMapPerName_;

        private Dictionary<string, GameObject> prefabs_;

        /// <summary>
        /// Gets a navigation target for the scene type.
        /// </summary>
        public NavigationTarget GetTarget(SceneType id)
        {
            CreateNavigationTargets();
            return navigationTargetMap_[id];
        }

        /// <summary>
        /// Gets a navigation target for the scene name.
        /// </summary>
        public NavigationTarget GetTarget(string id)
        {
            CreateNavigationTargets();
            return navigationTargetMapPerName_[id];
        }

        /// <summary>
        /// Gets a map of scene ids and prefabs.
        /// </summary>
        /// <returns></returns>

        public Dictionary<string, GameObject> GetPrefabs()
        {
            CreateNavigationTargets();
            return prefabs_;
        }

        private void CreateNavigationTargets()
        {
            if (navigationTargetMap_ != null)
            {
                return;
            }
            navigationTargetMap_ = new Dictionary<SceneType, NavigationTarget>();
            navigationTargetMapPerName_ = new Dictionary<string, NavigationTarget>();
            prefabs_ = new Dictionary<string, GameObject>();

            for (int i = 0; i < scenes_.Length; i++)
            {
                SceneDefinition scene = scenes_[i];
                var target = new NavigationTarget(scene.GetName(), scene.TargetType);
                navigationTargetMap_[scene.SceneType] = target;
                navigationTargetMapPerName_[scene.GetName()] = target;
                if (scene.Prefab != null)
                {
                    prefabs_[scene.GetName()] = scene.Prefab;
                }
            }
        }

        [Serializable]
        public class SceneDefinition
        {
            /// <summary>
            /// Gets the destination scene type.
            /// </summary>
            [field: SerializeField]
            public SceneType SceneType { get; private set; }

            /// <summary>
            /// Gets the destination target type.
            /// </summary>
            [field: SerializeField]
            public NavigationTargetType TargetType { get; private set; }

            /// <summary>
            /// Gets the destination prefab.
            /// </summary>
            [field: SerializeField]
            public GameObject Prefab { get; private set; }

            public string GetName()
            {
                return SceneType.GetId();
            }
        }
    }
}