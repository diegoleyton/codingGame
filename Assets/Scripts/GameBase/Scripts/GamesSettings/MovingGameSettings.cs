using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.GameBase.GamesSettings
{
    /// <summary>
    /// Stores presentation settings for moving game specific visuals.
    /// </summary>
    [CreateAssetMenu(
        fileName = "MovingGameSettings",
        menuName = "Flowbit/Moving Game/Moving Game Settings",
        order = 0)]
    public sealed class MovingGameSettings : ScriptableObject
    {
        [SerializeField] private Color defaultGroupColor_ = Color.white;
        [SerializeField] private List<GroupColorEntry> groupColors_ = new();

        /// <summary>
        /// Gets the configured color for a group id, or the default color when not found.
        /// </summary>
        public Color GetColorForGroupId(int groupId)
        {
            for (int i = 0; i < groupColors_.Count; i++)
            {
                if (groupColors_[i].GroupId == groupId)
                {
                    return groupColors_[i].Color;
                }
            }

            return defaultGroupColor_;
        }
    }

    /// <summary>
    /// Maps a group id to a color.
    /// </summary>
    [Serializable]
    public struct GroupColorEntry
    {
        [field: SerializeField]
        public int GroupId { get; private set; }

        [field: SerializeField]
        public Color Color { get; private set; }
    }
}