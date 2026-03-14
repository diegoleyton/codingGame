using UnityEngine;
using CodingGame.Runtime.Core;
using CodingGame.Runtime.Games.Moving;

namespace CodingGame.Presentation.Games.Moving
{

    public sealed class MovingGameController : MonoBehaviour
    {
        [SerializeField] private Transform character_;
        [SerializeField] private Transform food_;

        [SerializeField] private float cellSize_ = 1.0f;

        private MovingGame game_;
        private ProgramRunner runner_;
        private ProgramDefinition program_;

        private void Start()
        {
            CreateGame();
            CreateProgram();
            UpdateView();
        }

        private void CreateGame()
        {
            game_ = new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(3, 0)
            );

            runner_ = new ProgramRunner();
        }

        private void CreateProgram()
        {
            program_ = new ProgramDefinition();

            program_.AddInstruction(
                new InstructionInstance(new MoveForwardInstructionDefinition()));

            program_.AddInstruction(
                new InstructionInstance(new MoveForwardInstructionDefinition()));

            program_.AddInstruction(
                new InstructionInstance(new RotateLeftInstructionDefinition()));

            program_.AddInstruction(
                new InstructionInstance(new MoveForwardInstructionDefinition()));

            runner_.LoadProgram(program_);
        }

        public void Step()
        {
            runner_.ExecuteNextStep(game_);
            UpdateView();
        }

        public void ResetGame()
        {
            game_.ResetGame();
            runner_.ResetExecution();
            UpdateView();
        }

        private void UpdateView()
        {
            GridPosition pos = game_.GetCharacterPosition();

            character_.position = new Vector3(
                pos.GetX() * cellSize_,
                0,
                pos.GetY() * cellSize_);

            GridPosition foodPos = game_.GetFoodPosition();

            food_.position = new Vector3(
                foodPos.GetX() * cellSize_,
                0,
                foodPos.GetY() * cellSize_);
        }
    }
}