using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Engine;
using Flowbit.Engine.Instructions;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Coordinates the game runtime, scene view, and execution controls.
    /// </summary>
    public abstract class GameControllerBase<TGame> : MonoBehaviour
        where TGame : class, IGame
    {
        [Header("UI")]
        [SerializeField] private ProgramViewBase programPanelView_;
        [SerializeField] private GameStatusViewBase gameStatusView_;

        [SerializeField] private ExecutionControlsViewBase executionControlsView_;

        [Header("Execution")]
        [SerializeField] private float stepDelaySeconds_ = 0.5f;

        private TGame game_;
        private ProgramRunner runner_;
        private ProgramDefinition currentProgram_;
        private Coroutine runCoroutine_;
        private Coroutine stepCoroutine_;
        private bool programDirty_;
        private bool pauseRequested_;

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Start()
        {
            runner_ = new ProgramRunner();
            programPanelView_?.SetInstructionSelectedCallback(JumpToInstructionState);

            if (ShouldCreateGameOnStart())
            {
                LoadGame(CreateGame());
                return;
            }

            CreateNewProgram();
            ClearInstructionHighlight();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Executes one program step.
        /// </summary>
        public void Step()
        {
            if (stepCoroutine_ != null || runCoroutine_ != null)
            {
                return;
            }

            if (!CanExecuteStep())
            {
                return;
            }

            pauseRequested_ = false;
            programPanelView_?.EnableInstructions(false);
            RefreshExecutionControlsView();

            stepCoroutine_ = StartCoroutine(StepRoutine());
        }

        /// <summary>
        /// Toggles automatic execution.
        /// If already running, it pauses after the current step finishes.
        /// </summary>
        public void Run()
        {
            if (runCoroutine_ != null)
            {
                pauseRequested_ = true;
                RefreshExecutionControlsView();
                return;
            }

            if (stepCoroutine_ != null)
            {
                return;
            }

            if (!CanExecuteStep())
            {
                return;
            }

            pauseRequested_ = false;
            programPanelView_?.EnableInstructions(false);
            EnsureRunnerReady();

            runCoroutine_ = StartCoroutine(RunRoutine());
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Requests a pause after the current step finishes.
        /// </summary>
        public void Stop()
        {
            pauseRequested_ = true;
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Resets the game and execution state.
        /// </summary>
        public void ResetGame()
        {
            StopRunningImmediately();

            if (game_ == null || runner_ == null)
            {
                return;
            }

            game_.ResetGame();
            runner_.ResetExecution();
            programDirty_ = false;

            RefreshViewImmediate();
            ClearInstructionHighlight();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Deletes all instructions inside the program.
        /// </summary>
        public void CleanProgram()
        {
            StopRunningImmediately();

            if (game_ == null)
            {
                return;
            }

            game_.ResetGame();
            CreateNewProgram();

            RefreshViewImmediate();
            ClearInstructionHighlight();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Removes the last instruction from the current program.
        /// Always returns the game to the initial state.
        /// </summary>
        public void RemoveLastInstruction()
        {
            var instructionCount = currentProgram_.GetInstructionCount();
            if (instructionCount <= 0)
            {
                return;
            }

            RemoveInstructionAt(instructionCount - 1);
        }

        /// <summary>
        /// Removes the instruction at the given index and adjusts execution state accordingly.
        /// </summary>
        public void RemoveInstructionAt(int removedIndex)
        {
            if (currentProgram_ == null)
            {
                return;
            }

            int instructionCount = currentProgram_.GetInstructionCount();

            if (removedIndex < 0 || removedIndex >= instructionCount)
            {
                return;
            }

            // Gets the current index before stopping the program
            int currentInstructionIndex = runner_ != null
                ? runner_.GetCurrentInstructionIndex()
                : -1;

            StopRunningImmediately();

            currentProgram_.RemoveInstructionAt(removedIndex);
            OnProgramChanged();

            if (runner_ == null)
            {
                return;
            }

            // Case 1: If we current instruction is before the removed one, do nothing
            if (currentInstructionIndex <= removedIndex)
            {
                return;
            }

            // Case 2: If we deleted the current one or one after, go to the previous one
            int targetIndex = currentInstructionIndex - 1;

            if (targetIndex < 0)
            {
                // go to the starting position
                JumpToInstructionState(-1);
                return;
            }

            JumpToInstructionState(targetIndex);
        }

        /// <summary>
        /// Returns the current game instance.
        /// </summary>
        protected TGame GetGame()
        {
            return game_;
        }

        /// <summary>
        /// Returns the current program definition.
        /// </summary>
        protected ProgramDefinition GetCurrentProgram()
        {
            return currentProgram_;
        }

        /// <summary>
        /// Returns whether a game should be created automatically on start.
        /// </summary>
        protected virtual bool ShouldCreateGameOnStart()
        {
            return true;
        }

        /// <summary>
        /// Loads a new game instance into the controller.
        /// </summary>
        protected void LoadGame(TGame game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            StopRunningImmediately();

            if (runner_ == null)
            {
                runner_ = new ProgramRunner();
            }

            game_ = game;
            CreateNewProgram();
            programPanelView_?.SetInstructionSelectedCallback(JumpToInstructionState);

            InitializeView();
            RefreshViewImmediate();
            ClearInstructionHighlight();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Adds an instruction of the given instruction definition to the current program.
        /// </summary>
        protected void AddInstructionToCurrentProgram(
            GameInstructionDefinitionBase<TGame> instructionDefinition)
        {
            if (currentProgram_ == null)
            {
                throw new InvalidOperationException(
                    "A program must be created before adding instructions.");
            }

            InstructionInstance instructionInstance =
                new InstructionInstance(instructionDefinition);

            currentProgram_.AddInstruction(instructionInstance);
            OnProgramChanged();
        }

        /// <summary>
        /// Called when the current program changes.
        /// </summary>
        protected virtual void OnProgramChanged()
        {
            programDirty_ = true;

            if (programPanelView_ != null)
            {
                programPanelView_.Rebuild(
                    currentProgram_.GenerateReadOnlyInstructions());

                programPanelView_.SetInstructionSelectedCallback(JumpToInstructionState);
            }

            ClearInstructionHighlight();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Highlights the instruction at the given index.
        /// </summary>
        protected virtual void HighlightInstruction(int instructionIndex)
        {
            if (programPanelView_ == null)
            {
                return;
            }

            if (instructionIndex < 0)
            {
                programPanelView_.ClearHighlight();
                return;
            }

            programPanelView_.HighlightIndex(instructionIndex);
        }

        /// <summary>
        /// Clears the current instruction highlight.
        /// </summary>
        protected virtual void ClearInstructionHighlight()
        {
            if (programPanelView_ == null)
            {
                return;
            }

            programPanelView_.ClearHighlight();
        }

        /// <summary>
        /// Updates the result UI based on the current game state.
        /// </summary>
        protected virtual void RefreshResultView()
        {
            if (gameStatusView_ == null || game_ == null)
            {
                return;
            }

            if (game_.HasWon())
            {
                gameStatusView_.Win();
                return;
            }

            if (game_.HasFailed())
            {
                gameStatusView_.Lose();
                return;
            }

            gameStatusView_.Idle();
        }

        /// <summary>
        /// Initializes the scene view.
        /// </summary>
        protected abstract void InitializeView();

        /// <summary>
        /// Creates the initial game instance.
        /// </summary>
        protected abstract TGame CreateGame();

        /// <summary>
        /// Refreshes the view immediately.
        /// </summary>
        protected abstract void RefreshViewImmediate();

        /// <summary>
        /// Refreshes the view with animation.
        /// </summary>
        protected abstract IEnumerator RefreshViewAnimated();

        /// <summary>
        /// Applies the current executed step immediately, without animation.
        /// This is used when jumping directly to a selected instruction state.
        /// </summary>
        protected abstract void ApplyExecutedStepImmediate();

        private void CreateNewProgram()
        {
            currentProgram_ = new ProgramDefinition();
            runner_.LoadProgram(currentProgram_);
            programDirty_ = false;

            if (programPanelView_ != null)
            {
                programPanelView_.Rebuild(
                    currentProgram_.GenerateReadOnlyInstructions());

                programPanelView_.SetInstructionSelectedCallback(JumpToInstructionState);
            }
        }

        private void EnsureRunnerReady()
        {
            if (!programDirty_)
            {
                return;
            }

            runner_.ResetExecution();
            programDirty_ = false;
        }

        private bool CanExecuteStep()
        {
            if (game_ == null || runner_ == null)
            {
                return false;
            }

            if (game_.HasWon() || game_.HasFailed())
            {
                return false;
            }

            if (runner_.IsFinished())
            {
                return false;
            }

            return true;
        }

        private IEnumerator StepRoutine()
        {
            EnsureRunnerReady();

            int currentInstructionIndex = runner_.GetCurrentInstructionIndex();
            HighlightInstruction(currentInstructionIndex);

            runner_.ExecuteNextStep(game_);
            yield return RefreshViewAnimated();
            RefreshResultView();

            stepCoroutine_ = null;
            programPanelView_?.EnableInstructions(true);
            RefreshExecutionControlsView();
        }

        private IEnumerator RunRoutine()
        {
            while (CanExecuteStep())
            {
                int currentInstructionIndex = runner_.GetCurrentInstructionIndex();
                HighlightInstruction(currentInstructionIndex);

                runner_.ExecuteNextStep(game_);
                yield return RefreshViewAnimated();
                RefreshResultView();

                if (pauseRequested_)
                {
                    break;
                }

                if (!CanExecuteStep())
                {
                    break;
                }

                float elapsed = 0f;
                while (elapsed < stepDelaySeconds_)
                {
                    if (pauseRequested_)
                    {
                        break;
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (pauseRequested_)
                {
                    break;
                }
            }

            runCoroutine_ = null;
            pauseRequested_ = false;
            programPanelView_?.EnableInstructions(true);
            RefreshExecutionControlsView();
        }

        private void JumpToInstructionState(int instructionIndex)
        {
            StopRunningImmediately();

            if (game_ == null || runner_ == null || currentProgram_ == null)
            {
                return;
            }

            int instructionCount = currentProgram_.GetInstructionCount();
            int clampedInstructionIndex = Mathf.Clamp(instructionIndex, -1, instructionCount - 1);

            game_.ResetGame();
            runner_.ResetExecution();
            programDirty_ = false;

            if (clampedInstructionIndex >= 0)
            {
                while (!runner_.IsFinished() && !game_.HasWon() && !game_.HasFailed())
                {
                    int currentInstructionIndex = runner_.GetCurrentInstructionIndex();

                    if (currentInstructionIndex < 0 || currentInstructionIndex > clampedInstructionIndex)
                    {
                        break;
                    }

                    runner_.ExecuteNextStep(game_);
                    ApplyExecutedStepImmediate();

                    if (currentInstructionIndex == clampedInstructionIndex)
                    {
                        break;
                    }
                }
            }

            RefreshViewImmediate();

            if (clampedInstructionIndex >= 0)
            {
                HighlightInstruction(clampedInstructionIndex);
            }
            else
            {
                ClearInstructionHighlight();
            }

            RefreshResultView();
            RefreshExecutionControlsView();
        }

        private void StopRunningImmediately()
        {
            pauseRequested_ = false;
            programPanelView_?.EnableInstructions(true);

            if (runCoroutine_ != null)
            {
                StopCoroutine(runCoroutine_);
                runCoroutine_ = null;
            }

            if (stepCoroutine_ != null)
            {
                StopCoroutine(stepCoroutine_);
                stepCoroutine_ = null;
            }

            RefreshExecutionControlsView();
        }

        private void RefreshExecutionControlsView()
        {
            if (executionControlsView_ == null)
            {
                return;
            }

            bool isRunning = runCoroutine_ != null;
            executionControlsView_.SetIsRunning(isRunning);
        }
    }
}