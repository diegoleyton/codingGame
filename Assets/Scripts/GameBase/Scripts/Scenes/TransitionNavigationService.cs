using Flowbit.Utilities.Core.Navigation;
using Flowbit.GameBase.Definitions;
using System.Collections;
using System;
using UnityEngine;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Scene service for this game using transition
    /// </summary>
    public class TransitionNavigationService : IGameNavigationService
    {
        private NavigationService navigationService_;
        private SceneSettings sceneSettings_;
        private ISceneTransitionStrategy sceneTransitionStrategy_;

        public TransitionNavigationService(
            NavigationService navigationService,
            SceneSettings sceneSettings,
            ISceneTransitionStrategy sceneTransitionStrategy)
        {
            navigationService_ = navigationService;
            sceneSettings_ = sceneSettings;
            sceneTransitionStrategy_ = sceneTransitionStrategy;
        }

        /// <summary>
        /// Navigates to the given type
        /// </summary>
        public void Navigate(
            SceneType sceneType,
            NavigationParams navigationParams = null)
        {
            var sceneTarget = sceneSettings_.GetTarget(sceneType);

            if (sceneTarget.TargetType == NavigationTargetType.Scene)
            {
                sceneTransitionStrategy_.RunTransition(
                    () => navigationService_.Navigate(sceneTarget, navigationParams)
                );
                return;
            }

            navigationService_.Navigate(sceneTarget, navigationParams);
        }
    }
}