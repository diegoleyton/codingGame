using Flowbit.Engine;
using Flowbit.EngineController;
using Flowbit.Utilities.Core.Events;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Services;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Coordinates a general game runtime, scene view, and execution controls.
    /// </summary>
    public abstract class GeneralGameController<TGame> : GameControllerBase<TGame, InstructionType> where TGame : class, IGame
    {
        protected EventDispatcher eventDispatcher_;

        private void Start()
        {
            var serviceContainer = GlobalServiceContainer.ServiceContainer;
            eventDispatcher_ = serviceContainer.Get<EventDispatcher>();
        }

        /// <summary>
        /// Returns whether a game should be created automatically on start.
        /// </summary>
        protected override bool ShouldCreateGameOnStart()
        {
            return false;
        }

        protected override void OnInstructionDeleted()
        {
            eventDispatcher_.Send(new OnInstructionRemoved());
        }

        protected override void OnAllInstructionsDeleted()
        {
            eventDispatcher_.Send(new OnAllInstructionsRemoved());
        }

        protected override void OnGameReset()
        {
            eventDispatcher_.Send(new OnProgramStopped());
        }
    }
}