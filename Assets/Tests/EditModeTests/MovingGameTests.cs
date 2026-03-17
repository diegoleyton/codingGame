using System;
using NUnit.Framework;
using CodingGame.Runtime.Games.Moving;

/// <summary>
/// Contains unit tests for MovingGame.
/// </summary>
public sealed class MovingGameTests
{
    [Test]
    public void Constructor_WithInvalidWidth_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new MovingGame(
                width: 0,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(1, 1)));
    }

    [Test]
    public void Constructor_WithInvalidHeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new MovingGame(
                width: 5,
                height: 0,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(1, 1)));
    }

    [Test]
    public void Constructor_WithStartPositionOutsideBounds_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(5, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(1, 1)));
    }

    [Test]
    public void Constructor_WithFoodPositionOutsideBounds_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(-1, 1)));
    }

    [Test]
    public void Constructor_InitializesGameCorrectly()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 4,
            startCharacterPosition: new GridPosition(1, 2),
            startCharacterDirection: Direction.Left,
            foodPosition: new GridPosition(3, 1));

        Assert.AreEqual(5, game.GetWidth());
        Assert.AreEqual(4, game.GetHeight());
        Assert.AreEqual(new GridPosition(1, 2), game.GetCharacterPosition());
        Assert.AreEqual(Direction.Left, game.GetCharacterDirection());
        Assert.AreEqual(new GridPosition(3, 1), game.GetFoodPosition());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void Constructor_WhenStartPositionEqualsFoodPosition_StartsWon()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Up,
            foodPosition: new GridPosition(2, 2));

        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void ResetGame_RestoresInitialState()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(3, 0));

        game.MoveForward(2);
        game.RotateLeft();
        game.ResetGame();

        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void MoveForward_WithInvalidSteps_Throws()
    {
        MovingGame game = CreateDefaultGame();

        Assert.Throws<ArgumentOutOfRangeException>(() => game.MoveForward(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => game.MoveForward(-1));
    }

    [Test]
    public void MoveForward_MovesCharacterForward()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(2);

        Assert.AreEqual(new GridPosition(2, 0), game.GetCharacterPosition());
        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void MoveForward_ReachingFood_SetsWon()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(3);

        Assert.AreEqual(new GridPosition(3, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void MoveForward_StopsWhenFoodIsReached()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(10);

        Assert.AreEqual(new GridPosition(3, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void MoveForward_LeavingBounds_SetsFailed()
    {
        MovingGame game = new MovingGame(
            width: 3,
            height: 3,
            startCharacterPosition: new GridPosition(2, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(0, 0));

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(2, 0), game.GetCharacterPosition());
        Assert.IsFalse(game.HasWon());
        Assert.IsTrue(game.HasFailed());
    }

    [Test]
    public void RotateLeft_ChangesDirectionCorrectly()
    {
        MovingGame game = CreateDefaultGame();

        game.RotateLeft();
        Assert.AreEqual(Direction.Up, game.GetCharacterDirection());

        game.RotateLeft();
        Assert.AreEqual(Direction.Left, game.GetCharacterDirection());

        game.RotateLeft();
        Assert.AreEqual(Direction.Down, game.GetCharacterDirection());

        game.RotateLeft();
        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
    }

    [Test]
    public void RotateRight_ChangesDirectionCorrectly()
    {
        MovingGame game = CreateDefaultGame();

        game.RotateRight();
        Assert.AreEqual(Direction.Down, game.GetCharacterDirection());

        game.RotateRight();
        Assert.AreEqual(Direction.Left, game.GetCharacterDirection());

        game.RotateRight();
        Assert.AreEqual(Direction.Up, game.GetCharacterDirection());

        game.RotateRight();
        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
    }

    [Test]
    public void RotateLeft_AfterWin_DoesNothing()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(3);
        game.RotateLeft();

        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsTrue(game.HasWon());
    }

    [Test]
    public void RotateRight_AfterWin_DoesNothing()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(3);
        game.RotateRight();

        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsTrue(game.HasWon());
    }

    [Test]
    public void MoveForward_AfterWin_DoesNothing()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(3);
        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(3, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void RotateLeft_AfterFail_DoesNothing()
    {
        MovingGame game = new MovingGame(
            width: 2,
            height: 2,
            startCharacterPosition: new GridPosition(1, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(0, 0));

        game.MoveForward(1);
        game.RotateLeft();

        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsTrue(game.HasFailed());
    }

    [Test]
    public void RotateRight_AfterFail_DoesNothing()
    {
        MovingGame game = new MovingGame(
            width: 2,
            height: 2,
            startCharacterPosition: new GridPosition(1, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(0, 0));

        game.MoveForward(1);
        game.RotateRight();

        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsTrue(game.HasFailed());
    }

    [Test]
    public void MoveForward_AfterFail_DoesNothing()
    {
        MovingGame game = new MovingGame(
            width: 2,
            height: 2,
            startCharacterPosition: new GridPosition(1, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(0, 0));

        game.MoveForward(1);
        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(1, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasFailed());
        Assert.IsFalse(game.HasWon());
    }

    [Test]
    public void IsInsideBounds_ReturnsTrueForValidPositions()
    {
        MovingGame game = CreateDefaultGame();

        Assert.IsTrue(game.IsInsideBounds(new GridPosition(0, 0)));
        Assert.IsTrue(game.IsInsideBounds(new GridPosition(4, 4)));
        Assert.IsTrue(game.IsInsideBounds(new GridPosition(2, 3)));
    }

    [Test]
    public void IsInsideBounds_ReturnsFalseForInvalidPositions()
    {
        MovingGame game = CreateDefaultGame();

        Assert.IsFalse(game.IsInsideBounds(new GridPosition(-1, 0)));
        Assert.IsFalse(game.IsInsideBounds(new GridPosition(0, -1)));
        Assert.IsFalse(game.IsInsideBounds(new GridPosition(5, 0)));
        Assert.IsFalse(game.IsInsideBounds(new GridPosition(0, 5)));
    }

    [Test]
    public void MoveForward_UsesCurrentDirection_Up()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Up,
            foodPosition: new GridPosition(2, 4));

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(2, 3), game.GetCharacterPosition());
    }

    [Test]
    public void MoveForward_UsesCurrentDirection_Down()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Down,
            foodPosition: new GridPosition(2, 0));

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(2, 1), game.GetCharacterPosition());
    }

    [Test]
    public void MoveForward_UsesCurrentDirection_Left()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Left,
            foodPosition: new GridPosition(0, 2));

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(1, 2), game.GetCharacterPosition());
    }

    [Test]
    public void MoveForward_UsesCurrentDirection_Right()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 2));

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(3, 2), game.GetCharacterPosition());
    }

    [Test]
    public void Constructor_WithNullBlockedPositions_TreatedAsEmpty()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(3, 0),
            blockedPositions: null);

        Assert.IsNotNull(game.GetBlockedPositions());
        Assert.AreEqual(0, game.GetBlockedPositions().Count);
    }

    [Test]
    public void Constructor_WithBlockedPositionOutsideBounds_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(3, 0),
                blockedPositions: new[]
                {
                        new GridPosition(10, 10)
                }));
    }

    [Test]
    public void Constructor_WithBlockedPositionOnStart_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(3, 0),
                blockedPositions: new[]
                {
                        new GridPosition(0, 0)
                }));
    }

    [Test]
    public void Constructor_WithBlockedPositionOnFood_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(3, 0),
                blockedPositions: new[]
                {
                        new GridPosition(3, 0)
                }));
    }

    [Test]
    public void Constructor_WithBlockedPositions_StoresThemCorrectly()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: new[]
            {
                    new GridPosition(1, 1),
                    new GridPosition(2, 2)
            });

        Assert.AreEqual(2, game.GetBlockedPositions().Count);
        Assert.IsTrue(game.IsBlocked(new GridPosition(1, 1)));
        Assert.IsTrue(game.IsBlocked(new GridPosition(2, 2)));
        Assert.IsFalse(game.IsBlocked(new GridPosition(0, 0)));
    }

    [Test]
    public void IsBlocked_ReturnsFalse_WhenNoBlockedPositionsExist()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: Array.Empty<GridPosition>());

        Assert.IsFalse(game.IsBlocked(new GridPosition(1, 0)));
        Assert.IsFalse(game.IsBlocked(new GridPosition(2, 2)));
    }

    [Test]
    public void MoveForward_IntoBlockedCell_SetsFailedAndDoesNotMove()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: new[]
            {
                    new GridPosition(1, 0)
            });

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.IsFalse(game.HasWon());
        Assert.IsTrue(game.HasFailed());
    }

    [Test]
    public void MoveForward_StopsBeforeBlockedCell_WhenMovingMultipleSteps()
    {
        MovingGame game = new MovingGame(
            width: 6,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(5, 0),
            blockedPositions: new[]
            {
                    new GridPosition(2, 0)
            });

        game.MoveForward(5);

        Assert.AreEqual(new GridPosition(1, 0), game.GetCharacterPosition());
        Assert.IsFalse(game.HasWon());
        Assert.IsTrue(game.HasFailed());
    }

    [Test]
    public void MoveForward_AroundBlockedCell_CanStillReachFood()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Up,
            foodPosition: new GridPosition(1, 1),
            blockedPositions: new[]
            {
                    new GridPosition(1, 0)
            });

        game.MoveForward(1);   // (0,1)
        game.RotateRight();    // Right
        game.MoveForward(1);   // (1,1)

        Assert.AreEqual(new GridPosition(1, 1), game.GetCharacterPosition());
        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void RotateLeft_WithBlockedCellAhead_DoesNotFail()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: new[]
            {
                    new GridPosition(1, 0)
            });

        game.RotateLeft();

        Assert.AreEqual(Direction.Up, game.GetCharacterDirection());
        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void RotateRight_WithBlockedCellAhead_DoesNotFail()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: new[]
            {
                    new GridPosition(1, 0)
            });

        game.RotateRight();

        Assert.AreEqual(Direction.Down, game.GetCharacterDirection());
        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void ResetGame_AfterBlockedCollision_ClearsFailedStateAndRestoresStart()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: new[]
            {
                    new GridPosition(1, 0)
            });

        game.MoveForward(1);

        Assert.IsTrue(game.HasFailed());

        game.ResetGame();

        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void GetBlockedPositions_ReturnsReadOnlyCollectionWithExpectedCount()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: new[]
            {
                    new GridPosition(1, 1),
                    new GridPosition(2, 1),
                    new GridPosition(3, 1)
            });

        Assert.AreEqual(3, game.GetBlockedPositions().Count);
    }

    private static MovingGame CreateDefaultGame()
    {
        return new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(3, 0));
    }
}