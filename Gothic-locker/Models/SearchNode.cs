internal sealed record SearchNode(
    PuzzleState State,
    SearchNode? Parent,
    SolutionStep? Step);
