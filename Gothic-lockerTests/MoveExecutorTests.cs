using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GothicLockpickerTests;

[TestFixture]
public class MoveExecutorTests {
    private MoveExecutor _executor = null!;

    [SetUp]
    public void SetUp() {
        _executor = new MoveExecutor();
    }

    [Test]
    public void TryApply_Should_Move_Single_Lock() {
        // Arrange
        var state = new PuzzleState([3, 3, 3]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Right)
        };

        // Act
        var result = _executor.TryApply(
            state,
            movements,
            out var nextState);

        // Assert
        Assert.That(result, Is.True);

        Assert.That(
            nextState.Positions,
            Is.EqualTo(new[] { 4, 3, 3 }));
    }

    [Test]
    public void TryApply_Should_Move_Multiple_Locks() {
        // Arrange
        var state = new PuzzleState([3, 3, 3]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Right),
            new PlayerMove(1, Direction.Left)
        };

        // Act
        var result = _executor.TryApply(
            state,
            movements,
            out var nextState);

        // Assert
        Assert.That(result, Is.True);

        Assert.That(
            nextState.Positions,
            Is.EqualTo(new[] { 4, 2, 3 }));
    }

    [Test]
    public void TryApply_Should_Move_All_Locks_Atomically() {
        // Arrange
        var state = new PuzzleState([5, 0]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Right),
            new PlayerMove(1, Direction.Left)
        };

        // Act
        var result = _executor.TryApply(
            state,
            movements,
            out _);

        // Assert
        Assert.That(result, Is.False);

        Assert.That(
            state.Positions,
            Is.EqualTo(new[] { 5, 0 }));
    }

    [Test]
    public void TryApply_Should_ReturnFalse_When_Move_Goes_Beyond_Left_Boundary() {
        // Arrange
        var state = new PuzzleState([0, 3, 3]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Left)
        };

        // Act
        var result = _executor.TryApply(
            state,
            movements,
            out _);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TryApply_Should_ReturnFalse_When_Move_Goes_Beyond_Right_Boundary() {
        // Arrange
        var state = new PuzzleState([6, 3, 3]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Right)
        };

        // Act
        var result = _executor.TryApply(
            state,
            movements,
            out _);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TryApply_Should_Not_Modify_Original_State() {
        // Arrange
        var state = new PuzzleState([3, 3, 3]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Right)
        };

        // Act
        _executor.TryApply(
            state,
            movements,
            out _);

        // Assert
        Assert.That(
            state.Positions,
            Is.EqualTo(new[] { 3, 3, 3 }));
    }

    [Test]
    public void TryApply_Should_Allow_Multiple_Locks_To_End_In_Same_Position() {
        // Arrange
        var state = new PuzzleState([2, 4]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Right),
            new PlayerMove(1, Direction.Left)
        };

        // Act
        var result = _executor.TryApply(
            state,
            movements,
            out var nextState);

        // Assert
        Assert.That(result, Is.True);

        Assert.That(
            nextState.Positions,
            Is.EqualTo(new[] { 3, 3 }));
    }

    [Test]
    public void TryApply_Should_Move_All_Locks_Simultaneously() {
        // Arrange
        var state = new PuzzleState([3, 3, 3, 3]);

        var movements = new[]
        {
            new PlayerMove(0, Direction.Right),
            new PlayerMove(1, Direction.Left),
            new PlayerMove(2, Direction.Right),
            new PlayerMove(3, Direction.Left)
        };

        // Act
        var result = _executor.TryApply(
            state,
            movements,
            out var nextState);

        // Assert
        Assert.That(result, Is.True);

        Assert.That(
            nextState.Positions,
            Is.EqualTo(new[] { 4, 2, 4, 2 }));
    }

    [Test]
    public void IsSolved_Should_ReturnTrue_When_All_Locks_Are_In_Center() {
        // Arrange
        var state = new PuzzleState([3, 3, 3, 3]);

        // Assert
        Assert.That(state.IsSolved, Is.True);
    }

    [Test]
    public void IsSolved_Should_ReturnFalse_When_Any_Lock_Is_Not_In_Center() {
        // Arrange
        var state = new PuzzleState([3, 3, 2, 3]);

        // Assert
        Assert.That(state.IsSolved, Is.False);
    }
}