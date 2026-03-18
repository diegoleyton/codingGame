using System;
using NUnit.Framework;
using CodingGame.Runtime.Games.Moving;

/// <summary>
/// Contains unit tests for MovingGame.
/// </summary>
public sealed partial class MovingGameTests
{
    [Test]
    public void Constructor_WithBreakableObstacleOutsideBounds_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(4, 4),
                blockedPositions: Array.Empty<GridPosition>(),
                breakableBlockedPositions: new[]
                {
                    new GridPosition(10, 10)
                }));
    }

    [Test]
    public void Constructor_WithBreakableObstacleOnStart_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(4, 4),
                blockedPositions: Array.Empty<GridPosition>(),
                breakableBlockedPositions: new[]
                {
                    new GridPosition(0, 0)
                }));
    }

    [Test]
    public void Constructor_WithBreakableObstacleOnFood_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(4, 4),
                blockedPositions: Array.Empty<GridPosition>(),
                breakableBlockedPositions: new[]
                {
                    new GridPosition(4, 4)
                }));
    }

    [Test]
    public void Constructor_WithSameSolidAndBreakableObstacle_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPosition: new GridPosition(4, 4),
                blockedPositions: new[]
                {
                    new GridPosition(1, 0)
                },
                breakableBlockedPositions: new[]
                {
                    new GridPosition(1, 0)
                }));
    }

    [Test]
    public void Constructor_WithBreakableObstacles_StoresThemCorrectly()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(1, 1),
                new GridPosition(2, 2)
            });

        Assert.AreEqual(2, game.GetBreakableBlockedPositions().Count);
        Assert.IsTrue(game.IsBreakableBlocked(new GridPosition(1, 1)));
        Assert.IsTrue(game.IsBreakableBlocked(new GridPosition(2, 2)));
        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(0, 0)));
    }

    [Test]
    public void IsBlocked_ReturnsTrue_ForBreakableObstacle()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(1, 0)
            });

        Assert.IsTrue(game.IsBlocked(new GridPosition(1, 0)));
    }

    [Test]
    public void MoveForward_IntoBreakableObstacle_SetsFailedAndDoesNotMove()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(1, 0)
            });

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasFailed());
        Assert.IsFalse(game.HasWon());
    }

    [Test]
    public void BreakForward_WithBreakableObstacle_RemovesIt()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(1, 0)
            });

        game.BreakForward();

        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(1, 0)));
        Assert.IsFalse(game.IsBlocked(new GridPosition(1, 0)));
        Assert.AreEqual(0, game.GetBreakableBlockedPositions().Count);
    }

    [Test]
    public void BreakForward_WithNoObstacleAhead_DoesNothing()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: Array.Empty<GridPosition>());

        game.BreakForward();

        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.AreEqual(0, game.GetBreakableBlockedPositions().Count);
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void BreakForward_WithSolidObstacleAhead_DoesNothing()
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
            },
            breakableBlockedPositions: Array.Empty<GridPosition>());

        game.BreakForward();

        Assert.IsTrue(game.IsBlocked(new GridPosition(1, 0)));
        Assert.AreEqual(1, game.GetBlockedPositions().Count);
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void BreakForward_ThenMoveForward_CanMoveIntoFreedCell()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(2, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(1, 0)
            });

        game.BreakForward();
        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(1, 0), game.GetCharacterPosition());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void BreakForward_ThenMoveForwardToFood_CanStillWin()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(2, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
            new GridPosition(1, 0)
            });

        game.BreakForward();
        game.MoveForward(2);

        Assert.AreEqual(new GridPosition(2, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void BreakForward_WhenFacingUp_RemovesCorrectCell()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Up,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(2, 3)
            });

        game.BreakForward();

        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(2, 3)));
    }

    [Test]
    public void BreakForward_WhenFacingDown_RemovesCorrectCell()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Down,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(2, 1)
            });

        game.BreakForward();

        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(2, 1)));
    }

    [Test]
    public void BreakForward_WhenFacingLeft_RemovesCorrectCell()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Left,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(1, 2)
            });

        game.BreakForward();

        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(1, 2)));
    }

    [Test]
    public void BreakForward_WhenFacingRight_RemovesCorrectCell()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(3, 2)
            });

        game.BreakForward();

        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(3, 2)));
    }

    [Test]
    public void BreakForward_OutsideBounds_DoesNothing()
    {
        MovingGame game = new MovingGame(
            width: 2,
            height: 2,
            startCharacterPosition: new GridPosition(1, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(0, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: Array.Empty<GridPosition>());

        game.BreakForward();

        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
        Assert.AreEqual(0, game.GetBreakableBlockedPositions().Count);
    }

    [Test]
    public void BreakForward_AfterWin_DoesNothing()
    {
        MovingGame game = new MovingGame(
            width: 3,
            height: 3,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(1, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(2, 0)
            });

        game.MoveForward(1);
        game.BreakForward();

        Assert.IsTrue(game.HasWon());
        Assert.IsTrue(game.IsBreakableBlocked(new GridPosition(2, 0)));
    }

    [Test]
    public void BreakForward_AfterFail_DoesNothing()
    {
        MovingGame game = new MovingGame(
            width: 2,
            height: 2,
            startCharacterPosition: new GridPosition(1, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(0, 0),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(0, 1)
            });

        game.MoveForward(1);
        game.BreakForward();

        Assert.IsTrue(game.HasFailed());
        Assert.IsTrue(game.IsBreakableBlocked(new GridPosition(0, 1)));
    }

    [Test]
    public void ResetGame_DoesNotRestoreBrokenObstacle()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPosition: new GridPosition(4, 4),
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
                new GridPosition(1, 0)
            });

        game.BreakForward();
        game.ResetGame();

        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(1, 0)));
        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }
}