using System;
using UnityEngine;

namespace Flowbit.Utilities.Audio
{
    /// <summary>
    /// Plays audio from a typed sound library using one-shot and looping source pools.
    /// </summary>
    /// <typeparam name="TKey">The enum type used by the sound library.</typeparam>
    public sealed class AudioPlayer<TKey> where TKey : Enum
    {
        private readonly SoundLibrary<TKey> _soundLibrary;
        private readonly IAudioSourcePool _oneShotPool;
        private readonly ILoopingAudioSourcePool _loopingPool;

        /// <summary>
        /// Creates a new audio player.
        /// </summary>
        /// <param name="soundLibrary">The sound library used to resolve audio keys.</param>
        /// <param name="oneShotPool">The pool used for one-shot playback.</param>
        /// <param name="loopingPool">The pool used for looping playback.</param>
        public AudioPlayer(
            SoundLibrary<TKey> soundLibrary,
            IAudioSourcePool oneShotPool,
            ILoopingAudioSourcePool loopingPool)
        {
            _soundLibrary = soundLibrary ?? throw new ArgumentNullException(nameof(soundLibrary));
            _oneShotPool = oneShotPool ?? throw new ArgumentNullException(nameof(oneShotPool));
            _loopingPool = loopingPool ?? throw new ArgumentNullException(nameof(loopingPool));
        }

        /// <summary>
        /// Tries to play the sound associated with the given key as a one-shot.
        /// </summary>
        /// <param name="key">The sound key to play.</param>
        /// <returns>True if playback was triggered; otherwise false.</returns>
        public bool TryPlayOneShot(TKey key)
        {
            if (!TryGetPlaybackData(
                key,
                out SoundAsset soundAsset,
                out AudioClip clip,
                out float volume,
                out float pitch))
            {
                return false;
            }

            AudioSource source = _oneShotPool.GetAvailableSource();
            if (source == null)
            {
                return false;
            }

            source.pitch = pitch;
            source.PlayOneShot(clip, volume);
            return true;
        }

        /// <summary>
        /// Tries to play the sound associated with the given key as a loop.
        /// If the same loop is already active, its playback settings are refreshed.
        /// </summary>
        /// <param name="key">The sound key to play in loop.</param>
        /// <returns>True if playback was triggered; otherwise false.</returns>
        public bool TryPlayLoop(TKey key)
        {
            if (!TryGetPlaybackData(
                key,
                out SoundAsset _,
                out AudioClip clip,
                out float volume,
                out float pitch))
            {
                return false;
            }

            int audioId = Convert.ToInt32(key);
            AudioSource source = _loopingPool.GetOrCreateSource(audioId);
            if (source == null)
            {
                return false;
            }

            source.Stop();
            source.clip = clip;
            source.loop = true;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();

            return true;
        }

        /// <summary>
        /// Stops the active loop associated with the given key.
        /// </summary>
        /// <param name="key">The sound key to stop.</param>
        /// <returns>True if an active loop was found and stopped; otherwise false.</returns>
        public bool StopLoop(TKey key)
        {
            int audioId = Convert.ToInt32(key);

            if (!_loopingPool.TryGetSource(audioId, out AudioSource _))
            {
                return false;
            }

            _loopingPool.ReleaseSource(audioId);
            return true;
        }

        /// <summary>
        /// Stops and releases all active loops.
        /// </summary>
        public void StopAllLoops()
        {
            _loopingPool.StopAndReleaseAll();
        }

        private bool TryGetPlaybackData(
            TKey key,
            out SoundAsset soundAsset,
            out AudioClip clip,
            out float volume,
            out float pitch)
        {
            clip = null;
            volume = 1f;
            pitch = 1f;

            if (!_soundLibrary.TryGetSoundAsset(key, out soundAsset))
            {
                return false;
            }

            if (soundAsset == null || !soundAsset.HasClips)
            {
                return false;
            }

            clip = soundAsset.GetRandomClip();
            if (clip == null)
            {
                return false;
            }

            volume = soundAsset.GetRandomVolume();
            pitch = soundAsset.GetRandomPitch();
            return true;
        }
    }
}