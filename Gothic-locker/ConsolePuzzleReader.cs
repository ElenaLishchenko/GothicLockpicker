public static class ConsolePuzzleReader {
    public static PuzzleDefinition ReadDefinition() {
        Console.Write("Locks count: ");

        var lockCount =
            int.Parse(Console.ReadLine()!);

        var locks =
            new List<LockDefinition>();

        for (var lockIndex = 0;
             lockIndex < lockCount;
             lockIndex++) {
            Console.WriteLine();
            Console.WriteLine(
                $"Lock {lockIndex} influences:");

            Console.WriteLine(
                "Format: <lockIndex> <S|O>");

            Console.WriteLine(
                "Empty line to finish.");

            var moves =
                new List<LockMove>();

            while (true) {
                var line =
                    Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line)) {
                    break;
                }

                var parts =
                    line.Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2) {
                    throw new InvalidOperationException(
                        $"Invalid input '{line}'.");
                }

                var affectedLockIndex =
                    int.Parse(parts[0]);

                var direction =
                    ParseRelativeDirection(
                        parts[1]);

                moves.Add(
                    new LockMove(
                        affectedLockIndex,
                        direction));
            }

            locks.Add(
                new LockDefinition {
                    LockIndex = lockIndex,
                    Moves = moves
                });
        }

        return new PuzzleDefinition {
            LockCount = lockCount,
            Locks = locks
        };
    }

    public static PuzzleState ReadInitialState(
        int lockCount) {
        Console.WriteLine();
        Console.WriteLine(
            $"Enter {lockCount} positions:");

        var positions =
            Console.ReadLine()!
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

        if (positions.Length != lockCount) {
            throw new InvalidOperationException(
                $"Expected {lockCount} positions.");
        }

        foreach (var position in positions) {
            if (position < 0 ||
                position >= PuzzleDefinition.HoleCount) {
                throw new InvalidOperationException(
                    $"Invalid position '{position}'.");
            }
        }

        return new PuzzleState(
            positions);
    }

    private static RelativeDirection ParseRelativeDirection(
        string value) {
        return value
            .Trim()
            .ToUpperInvariant() switch {
                "S" => RelativeDirection.Same,
                "O" => RelativeDirection.Opposite,

                _ => throw new InvalidOperationException(
                    $"Unknown direction '{value}'.")
            };
    }
}