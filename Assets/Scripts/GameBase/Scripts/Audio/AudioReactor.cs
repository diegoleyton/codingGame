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
        private readonly EventDispatcher eventDispatcher_;
        private readonly AudioPlayer<SoundId> audioPlayer_;

        private bool playingGameMusic_ = false;
        private float musicTransitionTime_ = 1f;

        /// <summary>
        /// Creates a new audio reactor.
        /// </summary>
        public AudioReactor(
            EventDispatcher eventDispatcher,
            AudioPlayer<SoundId> audioPlayer,
            float musicTransitionTime)
        {
            eventDispatcher_ = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            audioPlayer_ = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));
            musicTransitionTime_ = musicTransitionTime;
            Subscribe();
        }

        private void Subscribe()
        {
            eventDispatcher_.Subscribe<OnNextScene>(OnNextScene);
            eventDispatcher_.Subscribe<OnPreviousScene>(OnPreviousScene);
            eventDispatcher_.Subscribe<OnPopupOpen>(_ => Play(SoundId.PopupOpen));
            eventDispatcher_.Subscribe<OnPopupClose>(_ => Play(SoundId.PopupClosed));
            eventDispatcher_.Subscribe<OnFirstScene>(_ => PlayLoop(SoundId.Main));

            eventDispatcher_.Subscribe<OnLevelCompletedPopupEvent>(_ => Play(SoundId.Win));
            eventDispatcher_.Subscribe<LevelFailedEvent>(_ => Play(SoundId.Lose));

            eventDispatcher_.Subscribe<OnMovingGameAttack>(_ => Play(SoundId.MovingGameAttack));
            eventDispatcher_.Subscribe<OnMovingGameBreak>(_ => Play(SoundId.MovingGameBreak));
            eventDispatcher_.Subscribe<OnMovingGameMove>(_ => Play(SoundId.MovingGameMove));
            eventDispatcher_.Subscribe<OnMovingGameRotate>(_ => Play(SoundId.MovingGameRotate));
            eventDispatcher_.Subscribe<OnMovingGameGoalReached>(_ => Play(SoundId.MovingGameGoalReached));

            eventDispatcher_.Subscribe<OnInstructionAdded>(_ => Play(SoundId.ProgramActionAdded));
            eventDispatcher_.Subscribe<OnInstructionRemoved>(_ => Play(SoundId.ProgramActionDeleted));
            eventDispatcher_.Subscribe<OnAllInstructionsRemoved>(_ => Play(SoundId.ProgramActionsDeleted));
            eventDispatcher_.Subscribe<OnProgramStep>(_ => Play(SoundId.ProgramStep));
            eventDispatcher_.Subscribe<OnProgramStopped>(_ => Play(SoundId.ProgramStop));
        }

        private void OnNextScene(OnNextScene e)
        {
            Play(SoundId.NextScene);
            if (e.SceneType == SceneType.MovingGame)
            {
                PlayGameMusic();
            }
            else
            {
                PlayMenuMusic();
            }
        }

        private void OnPreviousScene(OnPreviousScene e)
        {
            Play(SoundId.PreviousScene);
            PlayMenuMusic();
        }

        private void PlayGameMusic()
        {
            if (playingGameMusic_)
            {
                return;
            }

            playingGameMusic_ = true;
            audioPlayer_.TryTransitionLoop(SoundId.Main, SoundId.Main2, musicTransitionTime_);
        }

        private void PlayMenuMusic()
        {
            if (!playingGameMusic_)
            {
                return;
            }

            playingGameMusic_ = false;
            audioPlayer_.TryTransitionLoop(SoundId.Main2, SoundId.Main, musicTransitionTime_);
        }

        private void Play(SoundId soundId)
        {
            audioPlayer_.TryPlayOneShot(soundId);
        }

        private void PlayLoop(SoundId soundId)
        {
            audioPlayer_.StopAllLoops();
            audioPlayer_.TryPlayLoop(soundId);
        }
    }
}