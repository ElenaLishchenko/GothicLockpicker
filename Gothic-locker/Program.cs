using System.Text;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var configuration =
    new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile(
            "appsettings.json",
            optional: false,
            reloadOnChange: true)
        .Build();

var botToken =
    configuration["BotToken"]
    ?? throw new InvalidOperationException(
        "BotToken is not configured.");

var bot =
    new TelegramBotClient(
        botToken);

using var cts =
    new CancellationTokenSource();

var receiverOptions =
    new ReceiverOptions {
        AllowedUpdates = []
    };

bot.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cts.Token);

var me =
    await bot.GetMe();

Console.WriteLine(
    $"Bot started: @{me.Username}");

Console.ReadLine();

cts.Cancel();

static async Task HandleUpdateAsync(
    ITelegramBotClient bot,
    Update update,
    CancellationToken cancellationToken) {
    if (update.Type != UpdateType.Message) {
        return;
    }

    if (update.Message?.Text is null) {
        return;
    }

    var chatId =
        update.Message.Chat.Id;

    var session =
        SessionStorage.Sessions.GetOrAdd(
            chatId,
            _ => new UserSession());

    if (update.Message.Text == "/start") {
        session.Step =
            SessionStep.WaitingForLockCount;

        session.Locks.Clear();
        session.CurrentMoves.Clear();
        session.CurrentLockIndex = 0;

        await bot.SendMessage(
            chatId,
            "Enter lock count:");

        return;
    }

    if (session.Step ==
    SessionStep.WaitingForLockCount) {
        session.LockCount =
            int.Parse(update.Message.Text);

        session.Step =
            SessionStep.WaitingForInfluences;

        await bot.SendMessage(
            chatId,
            """
        Lock 0 influences:

        Examples:
        1O 2S 5O
        
        Use:
        -
        
        if there are no influences.
        """);

        return;
    }

    if (session.Step ==
        SessionStep.WaitingForInfluences) {
        var text =
            update.Message.Text.Trim();

        var moves =
            ParseMoves(text);

        session.Locks.Add(
            new LockDefinition {
                LockIndex =
                    session.CurrentLockIndex,

                Moves = moves
            });

        session.CurrentLockIndex++;

        if (session.CurrentLockIndex ==
            session.LockCount) {
            session.Step =
                SessionStep.WaitingForPositions;

            await bot.SendMessage(
                chatId,
                $"Enter {session.LockCount} positions:");

            return;
        }

        await bot.SendMessage(
            chatId,
            $"""
        Lock {session.CurrentLockIndex} influences

        Examples:
        1O 2S 5O

        Use:
        -

        if there are no influences.
        """);

        return;
    }

    if (session.Step ==
    SessionStep.WaitingForPositions) {
        var positions =
            update.Message.Text
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

        var definition =
            new PuzzleDefinition {
                LockCount =
                    session.LockCount,

                Locks =
                    session.Locks
            };

        var state =
            new PuzzleState(
                positions);

        var solver =
            new PuzzleSolver(
                new MoveExecutor());

        var result =
            solver.Solve(
                definition,
                state);

        if (result is null) {
            await bot.SendMessage(
                chatId,
                "No solution found.");

            return;
        }

        var sb =
            new StringBuilder();

        sb.AppendLine(
            $"Solution found ({result.Count} steps)");

        sb.AppendLine();

        sb.AppendLine(
            $"Start: [{string.Join(", ", positions)}]");

        sb.AppendLine();

        foreach (var step in result) {
            var arrow =
                step.Direction == Direction.Right
                    ? "←"
                    : "→";

            sb.AppendLine(
                $"Lock {step.LockIndex} {step.Direction} {arrow}");
        }

        await bot.SendMessage(
            chatId,
            sb.ToString());

        SessionStorage.Sessions.TryRemove(
            chatId,
            out _);

        return;
    }

    await bot.SendMessage(
        update.Message.Chat.Id,
        $"You wrote: {update.Message.Text}",
        cancellationToken: cancellationToken);
}

static Task HandleErrorAsync(
    ITelegramBotClient bot,
    Exception exception,
    CancellationToken cancellationToken) {
    Console.WriteLine(
        exception);

    return Task.CompletedTask;
}
static IReadOnlyCollection<LockMove> ParseMoves(
    string text) {
    if (text == "-" || text == "-") {
        return [];
    }

    var result =
        new List<LockMove>();

    var tokens =
        text.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

    foreach (var token in tokens) {
        if (token.Length < 2) {
            throw new InvalidOperationException(
                $"Invalid move '{token}'.");
        }

        var lockIndex =
            int.Parse(
                token[..^1]);

        var direction =
            token[^1];

        result.Add(
            new LockMove(
                lockIndex,
                direction switch {
                    'S' or 's'
                        => RelativeDirection.Same,

                    'O' or 'o'
                        => RelativeDirection.Opposite,

                    _ => throw new InvalidOperationException(
                        $"Invalid direction '{direction}'.")
                }));
    }

    return result;
}
