public sealed class PuzzleSolver : IPuzzleSolver {
    private readonly IMoveExecutor _moveExecutor;

    public PuzzleSolver(
        IMoveExecutor moveExecutor) {
        _moveExecutor = moveExecutor;
    }

    public IReadOnlyList<SolutionStep>? Solve(
        PuzzleDefinition definition,
        PuzzleState initialState) {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(initialState);

        if (initialState.IsSolved) {
            return Array.Empty<SolutionStep>();
        }

        var lockLookup =
            definition.Locks.ToDictionary(
                x => x.LockIndex);

        var queue =
            new Queue<SearchNode>();

        var visited =
            new HashSet<string>
            {
                CreateStateKey(initialState)
            };

        queue.Enqueue(
            new SearchNode(
                initialState,
                null,
                null));

        while (queue.Count > 0) {
            var current =
                queue.Dequeue();

            var solutionNode =
                ExploreMoves(
                    definition,
                    lockLookup,
                    current,
                    queue,
                    visited);

            if (solutionNode is not null) {
                return BuildSolution(solutionNode);
            }
        }

        return null;
    }

    private SearchNode? ExploreMoves(
        PuzzleDefinition definition,
        IReadOnlyDictionary<int, LockDefinition> lockLookup,
        SearchNode current,
        Queue<SearchNode> queue,
        HashSet<string> visited) {
        for (var lockIndex = 0;
             lockIndex < definition.LockCount;
             lockIndex++) {
            var leftResult =
                TryMove(
                    lockLookup,
                    current,
                    lockIndex,
                    Direction.Left,
                    queue,
                    visited);

            if (leftResult is not null) {
                return leftResult;
            }

            var rightResult =
                TryMove(
                    lockLookup,
                    current,
                    lockIndex,
                    Direction.Right,
                    queue,
                    visited);

            if (rightResult is not null) {
                return rightResult;
            }
        }

        return null;
    }

    private SearchNode? TryMove(
        IReadOnlyDictionary<int, LockDefinition> lockLookup,
        SearchNode current,
        int lockIndex,
        Direction direction,
        Queue<SearchNode> queue,
        HashSet<string> visited) {
        var movements =
            BuildMovements(
                lockLookup,
                lockIndex,
                direction);

        if (!_moveExecutor.TryApply(
                current.State,
                movements,
                out var nextState)) {
            return null;
        }

        var stateKey =
            CreateStateKey(nextState);

        if (!visited.Add(stateKey)) {
            return null;
        }

        var nextNode =
            new SearchNode(
                nextState,
                current,
                new SolutionStep(
                    lockIndex,
                    direction));

        if (nextState.IsSolved) {
            return nextNode;
        }

        queue.Enqueue(nextNode);

        return null;
    }

    private static IReadOnlyCollection<PlayerMove> BuildMovements(
        IReadOnlyDictionary<int, LockDefinition> lockLookup,
        int lockIndex,
        Direction direction) {
        var result =
            new List<PlayerMove>
            {
                new(lockIndex, direction)
            };

        var lockDefinition =
            lockLookup[lockIndex];

        foreach (var move in lockDefinition.Moves) {
            var movementDirection =
                move.Direction == RelativeDirection.Same
                    ? direction
                    : Reverse(direction);

            result.Add(
                new PlayerMove(
                    move.LockIndex,
                    movementDirection));
        }

        return result;
    }

    private static Direction Reverse(
        Direction direction) {
        return direction == Direction.Left
            ? Direction.Right
            : Direction.Left;
    }

    private static string CreateStateKey(
        PuzzleState state) {
        return string.Join(
            ',',
            state.Positions);
    }

    private static IReadOnlyList<SolutionStep> BuildSolution(
        SearchNode node) {
        var result =
            new List<SolutionStep>();

        while (node.Parent is not null) {
            result.Add(node.Step!);

            node = node.Parent;
        }

        result.Reverse();

        return result;
    }
}
