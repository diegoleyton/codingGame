using System;
using NUnit.Framework;
using Flowbit.MovingGame.Core;

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
                foodPositions: new[]
                {
                    new GridPosition(1, 1)
                }));
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
                foodPositions: new[]
                {
                    new GridPosition(1, 1)
                }));
    }

    [Test]
    public void Constructor_WithNullFoodPositions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPositions: null));
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
                foodPositions: new[]
                {
                    new GridPosition(1, 1)
                }));
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
                foodPositions: new[]
                {
                    new GridPosition(-1, 1)
                }));
    }

    [Test]
    public void Constructor_WithFoodPositionOnStart_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPositions: new[]
                {
                    new GridPosition(0, 0)
                }));
    }

    [Test]
    public void Constructor_InitializesGameCorrectly()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 4,
            startCharacterPosition: new GridPosition(1, 2),
            startCharacterDirection: Direction.Left,
            foodPositions: new[]
            {
                new GridPosition(3, 1),
                new GridPosition(4, 2)
            });

        Assert.AreEqual(5, game.GetWidth());
        Assert.AreEqual(4, game.GetHeight());
        Assert.AreEqual(new GridPosition(1, 2), game.GetCharacterPosition());
        Assert.AreEqual(Direction.Left, game.GetCharacterDirection());
        Assert.AreEqual(2, game.GetFoodPositions().Count);
        Assert.IsTrue(game.HasFoodAt(new GridPosition(3, 1)));
        Assert.IsTrue(game.HasFoodAt(new GridPosition(4, 2)));
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void Constructor_WithSingleFoodOnDifferentCell_StartsNotWon()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Up,
            foodPositions: new[]
            {
                new GridPosition(2, 3)
            });

        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void Constructor_WithNoFood_StartsWon()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(2, 2),
            startCharacterDirection: Direction.Up,
            foodPositions: Array.Empty<GridPosition>());

        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void ResetGame_RestoresInitialStateAndFood()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPositions: new[]
            {
                new GridPosition(1, 0),
                new GridPosition(3, 0)
            });

        game.MoveForward(1);
        game.RotateLeft();
        game.ResetGame();

        Assert.AreEqual(new GridPosition(0, 0), game.GetCharacterPosition());
        Assert.AreEqual(Direction.Right, game.GetCharacterDirection());
        Assert.AreEqual(2, game.GetFoodPositions().Count);
        Assert.IsTrue(game.HasFoodAt(new GridPosition(1, 0)));
        Assert.IsTrue(game.HasFoodAt(new GridPosition(3, 0)));
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
    public void MoveForward_ConsumesFoodOnVisitedCell()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPositions: new[]
            {
                new GridPosition(1, 0),
                new GridPosition(3, 0)
            });

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(1, 0), game.GetCharacterPosition());
        Assert.IsFalse(game.HasFoodAt(new GridPosition(1, 0)));
        Assert.IsTrue(game.HasFoodAt(new GridPosition(3, 0)));
        Assert.AreEqual(1, game.GetFoodPositions().Count);
        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
    }

    [Test]
    public void MoveForward_ReachingLastFood_SetsWon()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(3);

        Assert.AreEqual(new GridPosition(3, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
        Assert.AreEqual(0, game.GetFoodPositions().Count);
    }

    [Test]
    public void MoveForward_StopsWhenAllFoodIsEaten()
    {
        MovingGame game = CreateDefaultGame();

        game.MoveForward(10);

        Assert.AreEqual(new GridPosition(3, 0), game.GetCharacterPosition());
        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
        Assert.AreEqual(0, game.GetFoodPositions().Count);
    }

    [Test]
    public void MoveForward_WithMultipleFood_WinsOnlyAfterEatingAll()
    {
        MovingGame game = new MovingGame(
            width: 6,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPositions: new[]
            {
                new GridPosition(1, 0),
                new GridPosition(3, 0)
            });

        game.MoveForward(1);

        Assert.IsFalse(game.HasWon());
        Assert.IsFalse(game.HasFailed());
        Assert.AreEqual(1, game.GetFoodPositions().Count);

        game.MoveForward(2);

        Assert.IsTrue(game.HasWon());
        Assert.IsFalse(game.HasFailed());
        Assert.AreEqual(0, game.GetFoodPositions().Count);
    }

    [Test]
    public void MoveForward_LeavingBounds_SetsFailed()
    {
        MovingGame game = new MovingGame(
            width: 3,
            height: 3,
            startCharacterPosition: new GridPosition(2, 0),
            startCharacterDirection: Direction.Right,
            foodPositions: new[]
            {
                new GridPosition(0, 0)
            });

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
            foodPositions: new[]
            {
                new GridPosition(0, 0)
            });

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
            foodPositions: new[]
            {
                new GridPosition(0, 0)
            });

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
            foodPositions: new[]
            {
                new GridPosition(0, 0)
            });

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
            foodPositions: new[]
            {
                new GridPosition(2, 4)
            });

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
            foodPositions: new[]
            {
                new GridPosition(2, 0)
            });

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
            foodPositions: new[]
            {
                new GridPosition(0, 2)
            });

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
            foodPositions: new[]
            {
                new GridPosition(4, 2)
            });

        game.MoveForward(1);

        Assert.AreEqual(new GridPosition(3, 2), game.GetCharacterPosition());
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
                foodPositions: new[]
                {
                    new GridPosition(3, 0)
                },
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
                foodPositions: new[]
                {
                    new GridPosition(3, 0)
                },
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
                foodPositions: new[]
                {
                    new GridPosition(3, 0)
                },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(5, 0)
            },
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
    public void MoveForward_AroundBlockedCell_CanStillReachAllFood()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Up,
            foodPositions: new[]
            {
                new GridPosition(1, 1)
            },
            blockedPositions: new[]
            {
                new GridPosition(1, 0)
            });

        game.MoveForward(1);
        game.RotateRight();
        game.MoveForward(1);

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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
    public void Constructor_WithBreakableObstacleOutsideBounds_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MovingGame(
                width: 5,
                height: 5,
                startCharacterPosition: new GridPosition(0, 0),
                startCharacterDirection: Direction.Right,
                foodPositions: new[]
                {
                    new GridPosition(4, 4)
                },
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
                foodPositions: new[]
                {
                    new GridPosition(4, 4)
                },
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
                foodPositions: new[]
                {
                    new GridPosition(4, 4)
                },
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
                foodPositions: new[]
                {
                    new GridPosition(4, 4)
                },
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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(2, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(2, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(4, 4)
            },
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
            foodPositions: new[]
            {
                new GridPosition(0, 0)
            },
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
            width: 4,
            height: 3,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPositions: new[]
            {
                new GridPosition(1, 0)
            },
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
            foodPositions: new[]
            {
                new GridPosition(0, 0)
            },
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
    public void ResetGame_RestoresBrokenObstacle()
    {
        MovingGame game = new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPositions: new[]
            {
            new GridPosition(4, 4)
            },
            blockedPositions: Array.Empty<GridPosition>(),
            breakableBlockedPositions: new[]
            {
            new GridPosition(1, 0)
            });

        game.BreakForward();
        Assert.IsFalse(game.IsBreakableBlocked(new GridPosition(1, 0)));

        game.ResetGame();

        Assert.IsTrue(game.IsBreakableBlocked(new GridPosition(1, 0)));
    }

    private static MovingGame CreateDefaultGame()
    {
        return new MovingGame(
            width: 5,
            height: 5,
            startCharacterPosition: new GridPosition(0, 0),
            startCharacterDirection: Direction.Right,
            foodPositions: new[]
            {
                new GridPosition(3, 0)
            });
    }
}