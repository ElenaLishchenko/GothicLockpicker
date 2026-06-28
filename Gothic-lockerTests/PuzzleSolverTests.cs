using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GothicLockpickerTests;

[TestFixture]
public class PuzzleSolverTests {
    private PuzzleSolver _solver = null!;

    [SetUp]
    public void SetUp() {
        _solver = new PuzzleSolver(
            new MoveExecutor());
    }

    [Test]
    public void Solve_Should_Return_Empty_List_When_Puzzle_Is_Already_Solved() {
        // Arrange
        var definition = CreateDefinition(
            1,
            CreateLock(0));

        var initialState =
            new PuzzleState([3]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Solve_Should_Find_Single_Move_Solution() {
        // Arrange
        var definition = CreateDefinition(
            1,
            CreateLock(0));

        var initialState =
            new PuzzleState([2]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result,
            Has.Count.EqualTo(1));

        Assert.That(
            result![0],
            Is.EqualTo(
                new SolutionStep(
                    0,
                    Direction.Right)));
    }

    [Test]
    public void Solve_Should_Find_Multi_Step_Solution() {
        // Arrange
        var definition = CreateDefinition(
            1,
            CreateLock(0));

        var initialState =
            new PuzzleState([1]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result,
            Has.Count.EqualTo(2));

        Assert.That(
            result![0].Direction,
            Is.EqualTo(Direction.Right));

        Assert.That(
            result[1].Direction,
            Is.EqualTo(Direction.Right));
    }

    [Test]
    public void Solve_Should_Return_Null_When_Solution_Does_Not_Exist() {
        // Arrange
        var definition = CreateDefinition(
            1,
            CreateLock(
                0,
                new LockMove(
                    0,
                    RelativeDirection.Same)));

        var initialState =
            new PuzzleState([0]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Solve_Should_Not_Loop_When_State_Cycle_Exists() {
        // Arrange
        var definition = CreateDefinition(
            1,
            CreateLock(0));

        var initialState =
            new PuzzleState([0]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result!.Count,
            Is.EqualTo(3));
    }

    [Test]
    public void Solve_Should_Find_Shortest_Path() {
        // Arrange
        var definition = CreateDefinition(
            2,
            CreateLock(0),
            CreateLock(1));

        var initialState =
            new PuzzleState([2, 3]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result,
            Has.Count.EqualTo(1));

        Assert.That(
            result![0],
            Is.EqualTo(
                new SolutionStep(
                    0,
                    Direction.Right)));
    }

    [Test]
    public void Solve_Should_Use_Side_Effects() {
        // Arrange
        var definition = CreateDefinition(
            2,

            CreateLock(
                0,
                new LockMove(
                    1,
                    RelativeDirection.Opposite)),

            CreateLock(1));

        var initialState =
            new PuzzleState([2, 4]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result,
            Has.Count.EqualTo(1));

        Assert.That(
            result![0],
            Is.EqualTo(
                new SolutionStep(
                    0,
                    Direction.Right)));
    }

    [Test]
    public void Solve_Should_Find_Solution_For_Multi_Lock_Puzzle() {
        // Arrange
        var definition = CreateDefinition(
            3,

            CreateLock(
                0,
                new LockMove(
                    1,
                    RelativeDirection.Opposite)),

            CreateLock(
                1,
                new LockMove(
                    2,
                    RelativeDirection.Opposite)),

            CreateLock(2));

        var initialState =
            new PuzzleState([2, 4, 4]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result!.Count,
            Is.GreaterThan(0));
    }

    [Test]
    public void Solve_Should_Handle_Cyclic_Influence_Graph() {
        // Arrange
        var definition = CreateDefinition(
            2,

            CreateLock(
                0,
                new LockMove(
                    1,
                    RelativeDirection.Same)),

            CreateLock(
                1,
                new LockMove(
                    0,
                    RelativeDirection.Same)));

        var initialState =
            new PuzzleState([2, 2]);

        // Act
        var result =
            _solver.Solve(
                definition,
                initialState);

        // Assert
        Assert.That(result, Is.Not.Null);

        Assert.That(
            result,
            Has.Count.EqualTo(1));

        Assert.That(
            result![0],
            Is.EqualTo(
                new SolutionStep(
                    0,
                    Direction.Right)));
    }

    private static PuzzleDefinition CreateDefinition(
        int lockCount,
        params LockDefinition[] locks) {
        return new PuzzleDefinition {
            LockCount = lockCount,
            Locks = locks
        };
    }

    private static LockDefinition CreateLock(
        int lockIndex,
        params LockMove[] moves) {
        return new LockDefinition {
            LockIndex = lockIndex,
            Moves = moves
        };
    }
}