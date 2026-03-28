using UnityEngine;
using Flowbit.Utilities.Audio;
using System;

namespace Flowbit.GameBase.Audio
{
    [CreateAssetMenu(fileName = "GameSoundLibrary", menuName = "Flowbit/Audio/Game Sound Library")]
    public sealed class GameSoundLibrary : SoundLibrary<SoundId>
    {
        [field: SerializeField] public GameObject AudioPool { get; private set; }
    }
}