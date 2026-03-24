using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Completes a scene enter transition after Unity loads a new scene.
    /// </summary>
    public sealed class SceneTransitionBootstrap : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour transitionStrategyBehaviour_;

        private INavigationTransitionStrategy transitionStrategy_;

        private void Awake()
        {
            transitionStrategy_ = transitionStrategyBehaviour_ as INavigationTransitionStrategy;
        }

        private IEnumerator Start()
        {
            if (SceneNavigationState.ToTarget == null)
            {
                yield break;
            }

            if (transitionStrategy_ == null)
            {
                SceneNavigationState.Clear();
                yield break;
            }

            INavigationTransitionTargetProvider provider =
                SceneTransitionTargetResolver.ResolveActiveSceneProvider();

            IReadOnlyList<GameObject> enterTargets =
                provider?.GetTransitionTargets() ?? Array.Empty<GameObject>();

            NavigationTransitionContext context = new NavigationTransitionContext(
                SceneNavigationState.FromTarget,
                SceneNavigationState.ToTarget,
                SceneNavigationState.NavigationParams,
                Array.Empty<GameObject>(),
                enterTargets);

            yield return transitionStrategy_.RunTransition(context, () => { });

            SceneNavigationState.Clear();
        }
    }
}