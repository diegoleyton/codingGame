using System;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Audio;
using Flowbit.GameBase.Definitions;

namespace Flowbit.GameBase.Audio
{
    /// <summary>
    /// Translates domain events into audio playback actions.
    /// </summary>
    public sealed class AudioReactor
    {
        private readonly EventDispatcher _eventDispatcher;
        private readonly AudioPlayer<SoundId> _audioPlayer;

        /// <summary>
        /// Creates a new audio reactor.
        /// </summary>
        public AudioReactor(
            EventDispatcher eventDispatcher,
            AudioPlayer<SoundId> audioPlayer)
        {
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));

            Subscribe();
        }

        private void Subscribe()
        {
            _eventDispatcher.Subscribe<OnNextScene>(_ => Play(SoundId.NextScene));
            _eventDispatcher.Subscribe<OnPreviousScene>(_ => Play(SoundId.PreviousScene));
            _eventDispatcher.Subscribe<OnPopupOpen>(_ => Play(SoundId.PopupOpen));
            _eventDispatcher.Subscribe<OnPopupClose>(_ => Play(SoundId.PopupClosed));
        }

        private void Play(SoundId soundId)
        {
            _audioPlayer.TryPlayOneShot(soundId);
        }

        private void PlayLoop(SoundId soundId)
        {
            _audioPlayer.StopAllLoops();
            _audioPlayer.TryPlayLoop(soundId);
        }
    }
}