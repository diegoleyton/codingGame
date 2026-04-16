using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flowbit.Engine;
using Flowbit.Engine.Instructions;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Coordinates the game runtime, scene view, and execution controls.
    /// </summary>
    public abstract class GameControllerBase<TGame, TInstruction> : MonoBehaviour
        where TGame : class, IGame
    {
        [Header("UI")]
        [SerializeField]
        private ProgramViewBase<TInstruction> programPanelView_;

        [SerializeField]
        private GameStatusViewBase gameStatusView_;

        [SerializeField]
        private InstructionSelectionPanelViewBase<TInstruction> instructionSelectionPanelView_;

        [SerializeField]
        private ExecutionControlsViewBase executionControlsView_;

        [Header("Execution")]
        [SerializeField]
        private float stepDelaySeconds_ = 0.5f;

        private TGame game_;
        private ProgramRunner<TInstruction> runner_;
        private ProgramDefinition<TInstruction> currentProgram_;
        private IReadOnlyList<TInstruction> availableInstructions_;

        private Coroutine runCoroutine_;
        private Coroutine stepCoroutine_;
        private bool programDirty_;
        private bool pauseRequested_;
        private bool instructionEditingAvailable_;
        private bool? lastInstructionEditingAvailability_;
        private IGameRankingTracker rankingTracker_;
        private GameRankingResult currentRankingResult_;

        // Represents the instruction whose resulting state is currently shown in the game view.
        // -1 means "no selected instruction state".
        private int selectedInstructionIndex_ = -1;

        // True when execution has reached the end of the program and the UI should remain
        // in a stopped-at-end state: no highlight, no instruction panel, but stop/reset visible.
        private bool isStoppedAtProgramEnd_;

        /// <summary>
        /// Gets the resolver used to determine which instructions are available for a level.
        /// </summary>
        protected abstract IAvailableInstructionsResolver<TInstruction> AvailableInstructionsResolver { get; }

        /// <summary>
        /// Gets the resolver used to determine which instructions are available for a level.
        /// </summary>
        protected abstract IInstructionFactory<TGame, TInstruction> InstructionFactory { get; }

        /// <summary>
        /// Gets the instructions available for the currently loaded level.
        /// </summary>
        public IReadOnlyList<TInstruction> AvailableInstructions => availableInstructions_;

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Start()
        {
            runner_ = new ProgramRunner<TInstruction>();
            programPanelView_?.SetInstructionSelectedCallback(JumpToInstructionState);

            if (ShouldCreateGameOnStart())
            {
                LoadGame(CreateGame());
                return;
            }

            CreateNewProgram();
            SetInitialState();
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
            EnsureRunnerReady();

            runCoroutine_ = StartCoroutine(RunRoutine());
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
            RestartRankingTracker();

            RefreshViewImmediate();
            SetInitialState();
            RefreshResultView();
            RefreshExecutionControlsView();
            OnGameReset();
        }

        /// <summary>
        /// Stops the current execution and returns to the editable initial state
        /// without restarting the ranking tracker.
        /// </summary>
        public void StopExecutionAndReturnToEdit()
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
            SetInitialState();
            RefreshResultView();
            RefreshExecutionControlsView();
            OnGameReset();
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

            OnAllInstructionsDeleted();

            game_.ResetGame();
            CreateNewProgram();
            RestartRankingTracker();

            RefreshViewImmediate();
            SetInitialState();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Removes the last instruction from the current program.
        /// </summary>
        public void RemoveLastInstruction()
        {
            if (currentProgram_ == null || currentProgram_.GetInstructionCount() == 0)
            {
                return;
            }

            RemoveInstructionAt(currentProgram_.GetInstructionCount() - 1);
        }

        /// <summary>
        /// Removes the instruction at the given index and preserves the nearest valid shown state.
        /// </summary>
        public void RemoveInstructionAt(int removedInstructionIndex)
        {
            if (currentProgram_ == null)
            {
                return;
            }

            int instructionCount = currentProgram_.GetInstructionCount();

            if (removedInstructionIndex < 0 || removedInstructionIndex >= instructionCount)
            {
                return;
            }

            OnInstructionDeleted();

            int previousSelectedInstructionIndex = selectedInstructionIndex_;
            bool wasStoppedAtProgramEnd = isStoppedAtProgramEnd_;

            StopRunningImmediately();

            currentProgram_.RemoveInstructionAt(removedInstructionIndex);
            programDirty_ = true;
            RebuildProgramView();

            if (wasStoppedAtProgramEnd)
            {
                int lastInstructionIndex = currentProgram_.GetInstructionCount() - 1;
                JumpToInstructionState(lastInstructionIndex);
                return;
            }

            int targetInstructionIndex = GetInstructionIndexAfterRemoval(
                previousSelectedInstructionIndex,
                removedInstructionIndex);

            JumpToInstructionState(targetInstructionIndex);
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
        protected ProgramDefinition<TInstruction> GetCurrentProgram()
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
        /// Creates the ranking tracker for the currently loaded game.
        /// Return null if the game does not expose a ranking.
        /// </summary>
        protected virtual IGameRankingTracker CreateRankingTracker(TGame game)
        {
            return null;
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
                runner_ = new ProgramRunner<TInstruction>();
            }

            game_ = game;
            InitializeRankingTracker(game_);
            CreateNewProgram();
            programPanelView_?.SetInstructionSelectedCallback(JumpToInstructionState);

            SetAvailableInstructions();
            InitializeView();
            RefreshViewImmediate();
            SetInitialState();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Adds an instruction of the given instruction type to the current program.
        /// </summary>
        protected void AddInstructionToCurrentProgram(TInstruction instructionType)
        {
            if (currentProgram_ == null)
            {
                throw new InvalidOperationException(
                    "A program must be created before adding instructions.");
            }
            var instructionDefinition = InstructionFactory.CreateInstruction(instructionType);

            InstructionInstance<TInstruction> instructionInstance =
                new InstructionInstance<TInstruction>(instructionDefinition);

            currentProgram_.AddInstruction(instructionInstance);
            OnProgramChanged();
        }

        /// <summary>
        /// Called when the current program changes.
        /// </summary>
        protected virtual void OnProgramChanged()
        {
            programDirty_ = true;
            RebuildProgramView();
            SetInitialState();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        /// <summary>
        /// Highlights the instruction at the given index.
        /// </summary>
        protected virtual void HighlightInstruction(int instructionIndex)
        {
            SetSelectedInstructionIndex(instructionIndex);
        }

        /// <summary>
        /// Clears the current instruction highlight.
        /// </summary>
        protected virtual void ClearInstructionHighlight()
        {
            SetSelectedInstructionIndex(-1);
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

        /// <summary>
        /// Resolves and caches the instructions available for the provided level.
        /// </summary>
        protected void ResolveAvailableInstructions(int levelId)
        {
            if (AvailableInstructionsResolver == null)
            {
                throw new InvalidOperationException(
                    $"{GetType().Name} must provide an {nameof(IAvailableInstructionsResolver<TInstruction>)}.");
            }

            availableInstructions_ = AvailableInstructionsResolver.Resolve(levelId) ??
                                     Array.Empty<TInstruction>();
        }

        /// <summary>
        /// Returns whether the provided instruction is available for the current level.
        /// </summary>
        protected bool IsInstructionAvailable(TInstruction instruction)
        {
            if (availableInstructions_ == null)
            {
                return false;
            }

            var comparer = EqualityComparer<TInstruction>.Default;

            for (int i = 0; i < availableInstructions_.Count; i++)
            {
                if (comparer.Equals(availableInstructions_[i], instruction))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the available instructions for the current level.
        /// </summary>
        protected IReadOnlyList<TInstruction> GetAvailableInstructions()
        {
            return availableInstructions_ ?? Array.Empty<TInstruction>();
        }

        protected virtual void OnInstructionDeleted() { }
        protected virtual void OnAllInstructionsDeleted() { }

        protected virtual void OnGameReset() { }

        protected virtual void OnInstructionEditingAvailabilityChanged(bool isAvailable) { }

        /// <summary>
        /// Called whenever the current ranking result changes.
        /// </summary>
        protected virtual void OnRankingUpdated(GameRankingResult result) { }

        /// <summary>
        /// Called every frame so derived controllers can update view-specific state.
        /// </summary>
        protected virtual void UpdateController(float deltaTime) { }

        private void CreateNewProgram()
        {
            currentProgram_ = new ProgramDefinition<TInstruction>();
            runner_.LoadProgram(currentProgram_);
            programDirty_ = false;

            RebuildProgramView();
        }

        private void RebuildProgramView()
        {
            if (programPanelView_ == null)
            {
                return;
            }

            programPanelView_.Rebuild(
                currentProgram_.GenerateReadOnlyInstructions());

            programPanelView_.SetInstructionSelectedCallback(JumpToInstructionState);
        }

        private void EnsureRunnerReady()
        {
            if (!programDirty_)
            {
                return;
            }

            if (IsStoppedAtProgramEnd())
            {
                JumpToProgramEndState();
                return;
            }

            JumpToInstructionState(selectedInstructionIndex_);
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

            if (runner_.IsStopped())
            {
                return false;
            }

            return true;
        }

        private bool HasCurrentInstruction()
        {
            return selectedInstructionIndex_ >= 0;
        }

        private bool IsStoppedAtProgramEnd()
        {
            return isStoppedAtProgramEnd_;
        }

        private bool IsRunning()
        {
            return runCoroutine_ != null || stepCoroutine_ != null;
        }

        private void Update()
        {
            UpdateController(Time.unscaledDeltaTime);
        }

        private bool ShouldEnableInstructions()
        {
            return !IsRunning() && !HasCurrentInstruction() && !IsStoppedAtProgramEnd();
        }

        private bool ShouldShowStopButton()
        {
            return IsRunning() || HasCurrentInstruction() || IsStoppedAtProgramEnd();
        }

        private IEnumerator StepRoutine()
        {
            EnsureRunnerReady();

            int executingInstructionIndex = runner_.GetCurrentInstructionIndex();
            SetSelectedInstructionIndex(executingInstructionIndex);

            runner_.ExecuteNextStep(game_);
            yield return RefreshViewAnimated();

            if (runner_.IsStopped())
            {
                SetStoppedAtProgramEndState();
            }
            else
            {
                SetSelectedInstructionIndex(executingInstructionIndex);
            }

            RefreshResultView();

            stepCoroutine_ = null;
            RefreshExecutionControlsView();
        }

        private IEnumerator RunRoutine()
        {
            while (CanExecuteStep())
            {
                int executingInstructionIndex = runner_.GetCurrentInstructionIndex();
                SetSelectedInstructionIndex(executingInstructionIndex);

                runner_.ExecuteNextStep(game_);
                yield return RefreshViewAnimated();

                if (runner_.IsStopped())
                {
                    SetStoppedAtProgramEndState();
                }
                else
                {
                    SetSelectedInstructionIndex(executingInstructionIndex);
                }

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
            isStoppedAtProgramEnd_ = false;

            if (clampedInstructionIndex >= 0)
            {
                while (!runner_.IsStopped() && !game_.HasWon() && !game_.HasFailed())
                {
                    int executingInstructionIndex = runner_.GetCurrentInstructionIndex();

                    if (executingInstructionIndex < 0 || executingInstructionIndex > clampedInstructionIndex)
                    {
                        break;
                    }

                    runner_.ExecuteNextStep(game_);
                    ApplyExecutedStepImmediate();
                    SetSelectedInstructionIndex(executingInstructionIndex);

                    if (executingInstructionIndex == clampedInstructionIndex)
                    {
                        break;
                    }
                }
            }
            else
            {
                SetInitialState();
            }

            RefreshViewImmediate();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        private void JumpToProgramEndState()
        {
            StopRunningImmediately();

            if (game_ == null || runner_ == null || currentProgram_ == null)
            {
                return;
            }

            game_.ResetGame();
            runner_.ResetExecution();
            programDirty_ = false;

            while (!runner_.IsStopped() && !game_.HasWon() && !game_.HasFailed())
            {
                runner_.ExecuteNextStep(game_);
                ApplyExecutedStepImmediate();
            }

            SetStoppedAtProgramEndState();
            RefreshViewImmediate();
            RefreshResultView();
            RefreshExecutionControlsView();
        }

        private void StopRunningImmediately()
        {
            pauseRequested_ = false;

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
            bool shouldEnableInstructions = ShouldEnableInstructions();
            instructionEditingAvailable_ = shouldEnableInstructions;

            if (programPanelView_ != null)
            {
                programPanelView_.EnableInstructions(shouldEnableInstructions);
            }

            NotifyInstructionEditingAvailabilityChangedIfNeeded(shouldEnableInstructions);

            if (executionControlsView_ == null)
            {
                return;
            }

            executionControlsView_.SetIsRunning(runCoroutine_ != null);
            executionControlsView_.SetStopButtonVisible(ShouldShowStopButton());
        }

        private void SetInitialState()
        {
            selectedInstructionIndex_ = -1;
            isStoppedAtProgramEnd_ = false;

            if (programPanelView_ != null)
            {
                programPanelView_.ClearHighlight();
            }

            RefreshExecutionControlsView();
        }

        private void SetSelectedInstructionIndex(int instructionIndex)
        {
            selectedInstructionIndex_ = instructionIndex;
            isStoppedAtProgramEnd_ = false;

            if (programPanelView_ != null)
            {
                if (instructionIndex < 0)
                {
                    programPanelView_.ClearHighlight();
                }
                else
                {
                    programPanelView_.HighlightIndex(instructionIndex);
                }
            }

            RefreshExecutionControlsView();
        }

        private void SetStoppedAtProgramEndState()
        {
            isStoppedAtProgramEnd_ = true;
            RefreshExecutionControlsView();
        }

        private int GetInstructionIndexAfterRemoval(
            int previousSelectedInstructionIndex,
            int removedInstructionIndex)
        {
            if (previousSelectedInstructionIndex < 0)
            {
                return -1;
            }

            if (previousSelectedInstructionIndex < removedInstructionIndex)
            {
                return previousSelectedInstructionIndex;
            }

            return previousSelectedInstructionIndex - 1;
        }

        private void SetAvailableInstructions()
        {
            instructionSelectionPanelView_?.SetAvailableInstructions(
                GetAvailableInstructions(),
                OnAvailableInstructionClicked);
        }

        private void NotifyInstructionEditingAvailabilityChangedIfNeeded(bool isAvailable)
        {
            if (lastInstructionEditingAvailability_ == isAvailable)
            {
                return;
            }

            lastInstructionEditingAvailability_ = isAvailable;
            OnInstructionEditingAvailabilityChanged(isAvailable);
        }

        private void OnAvailableInstructionClicked(TInstruction instructionType)
        {
            AddInstructionToCurrentProgram(instructionType);
        }

        private void InitializeRankingTracker(TGame game)
        {
            rankingTracker_ = CreateRankingTracker(game);
            RefreshRankingResult();
        }

        private void RestartRankingTracker()
        {
            rankingTracker_?.Restart();
            RefreshRankingResult();
        }

        private void RefreshRankingResult()
        {
            if (rankingTracker_ == null)
            {
                currentRankingResult_ = default;
                return;
            }

            currentRankingResult_ = rankingTracker_.GetCurrentResult();
            OnRankingUpdated(currentRankingResult_);
        }

        public bool TryGetCurrentRankingResult(out GameRankingResult result)
        {
            if (rankingTracker_ == null)
            {
                result = default;
                return false;
            }

            RefreshRankingResult();
            result = currentRankingResult_;
            return currentRankingResult_.IsValid;
        }

        protected bool IsInstructionEditingAvailable()
        {
            return instructionEditingAvailable_;
        }
    }
}
