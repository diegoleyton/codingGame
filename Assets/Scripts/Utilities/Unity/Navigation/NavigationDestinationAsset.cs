using UnityEngine;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Defines a navigation destination that can be used from Unity assets and inspector references.
    /// </summary>
    [CreateAssetMenu(fileName = "NavigationDestination", menuName = "Flowbit/Navigation/Destination")]
    public sealed class NavigationDestinationAsset : ScriptableObject
    {
        [SerializeField] private string id_;
        [SerializeField] private NavigationTargetType targetType_;

        /// <summary>
        /// Creates a runtime navigation target from this asset.
        /// </summary>
        public NavigationTarget CreateTarget()
        {
            return new NavigationTarget(id_, targetType_);
        }

        /// <summary>
        /// Returns the destination id.
        /// </summary>
        public string GetId()
        {
            return id_;
        }

        /// <summary>
        /// Returns the target type.
        /// </summary>
        public NavigationTargetType GetTargetType()
        {
            return targetType_;
        }
    }
}