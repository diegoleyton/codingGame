using UnityEngine;
using Flowbit.GameBase.Services;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Automatically registers this GameObject's Transform into a ComponentsLoopAnimator
    /// found in the scene. It adds itself on Awake and removes itself on Disable.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIAnimatedComponent : MonoBehaviour
    {
        private ComponentsLoopAnimator animator_;

        private bool registered_;

        private void Awake()
        {
            ResolveAnimator();

            if (animator_ == null)
            {
                return;
            }

            Register();
        }

        private void OnDisable()
        {
            if (animator_ == null || !registered_)
            {
                return;
            }

            Unregister();
        }

        /// <summary>
        /// Manually forces registration into the animator.
        /// </summary>
        public void Register()
        {
            if (animator_ == null || registered_)
            {
                return;
            }
            else
            {
                animator_.AddTarget(transform);
            }

            registered_ = true;
        }

        /// <summary>
        /// Manually forces removal from the animator.
        /// </summary>
        public void Unregister()
        {
            if (animator_ == null || !registered_)
            {
                return;
            }
            else
            {
                animator_.RemoveTarget(transform);
            }

            registered_ = false;
        }

        private void ResolveAnimator()
        {
            if (animator_ != null)
            {
                return;
            }

            animator_ = GlobalServiceContainer.ServiceContainer.Get<ComponentsLoopAnimator>();
        }
    }
}