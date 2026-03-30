using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day22 : AbstractDaySolver<(IReadOnlyList<int> player1Deck, IReadOnlyList<int> player2Deck)>
{
    protected override (IReadOnlyList<int> player1Deck, IReadOnlyList<int> player2Deck) ParseInput(ILogger logger, string fileContents)
    {
        var lines = fileContents.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        var player1Deck = lines
            .First()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(int.Parse)
            .ToList();
        var player2Deck = lines
            .Last()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(int.Parse)
            .ToList();

        return (player1Deck, player2Deck);
    }

    protected override string SolvePart1(ILogger logger, (IReadOnlyList<int> player1Deck, IReadOnlyList<int> player2Deck) players)
    {
        var player1 = new Player(PlayerId.Player1, players.player1Deck);
        var player2 = new Player(PlayerId.Player2, players.player2Deck);

        while (true)
        {
            if (player1.DeckCount == 0)
            {
                logger.LogTrace("Player 1 has run out of cards! The game is over");
                return player2.CalculateScore().ToString();
            }

            if (player2.DeckCount == 0)
            {
                logger.LogTrace("Player 2 has run out of cards! The game is over");
                return player1.CalculateScore().ToString();
            }

            logger.LogTrace("{Player1} {Player2}", player1, player2);
            var player1Card = player1.Draw();
            var player2Card = player2.Draw();
            if (player1Card > player2Card)
            {
                logger.LogTrace("Player 1 wins the round! {Card1} vs {Card2}", player1Card, player2Card);
                player1.Add(player1Card);
                player1.Add(player2Card);
            }
            else if (player2Card > player1Card)
            {
                logger.LogTrace("Player 2 wins the round! {Card1} vs {Card2}", player2Card, player1Card);
                player2.Add(player2Card);
                player2.Add(player1Card);
            }
        }
    }

    protected override string SolvePart2(ILogger logger, (IReadOnlyList<int> player1Deck, IReadOnlyList<int> player2Deck) players)
    {
        var player1 = new Player(PlayerId.Player1, players.player1Deck);
        var player2 = new Player(PlayerId.Player2, players.player2Deck);
        var winner = PlayGameRec(logger, player1, player2);
        switch (winner)
        {
            case PlayerId.Player1:
                return player1.CalculateScore().ToString();
            case PlayerId.Player2:
                return player2.CalculateScore().ToString();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static PlayerId PlayGameRec(ILogger logger, Player player1, Player player2)
    {
        var gameStates = new HashSet<string>();
        while (true)
        {
            // Game ends if we enter a game state we've seen before
            var gameState = $"{string.Join("|", player1.Deck)}-{string.Join("|", player2.Deck)}";
            if (!gameStates.Add(gameState))
            {
                return PlayerId.Player1;
            }

            var player1Card = player1.Draw();
            var player2Card = player2.Draw();

            if (player1.DeckCount >= player1Card && player2.DeckCount >= player2Card)
            {
                // Play a recursive game
                var player1SubPlayer = new Player(PlayerId.Player1, player1.Deck.ToList()[..player1Card]);
                var player2SubPlayer = new Player(PlayerId.Player2, player2.Deck.ToList()[..player2Card]);
                var winner = PlayGameRec(logger, player1SubPlayer, player2SubPlayer);
                switch (winner)
                {
                    case PlayerId.Player1:
                        player1.Add(player1Card);
                        player1.Add(player2Card);
                        break;
                    case PlayerId.Player2:
                        player2.Add(player2Card);
                        player2.Add(player1Card);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // Play normal rules
                if (player1Card > player2Card)
                {
                    player1.Add(player1Card);
                    player1.Add(player2Card);
                }
                else if (player2Card > player1Card)
                {
                    player2.Add(player2Card);
                    player2.Add(player1Card);
                }
            }

            if (player1.DeckCount == 0)
            {
                return PlayerId.Player2;
            }

            if (player2.DeckCount == 0)
            {
                return PlayerId.Player1;
            }
        }
    }
}

internal enum PlayerId
{
    Player1 = 1,
    Player2 = 2
}

internal sealed class Player(PlayerId id, IReadOnlyList<int> deck)
{
    private PlayerId Id { get; } = id;
    public Queue<int> Deck { get; } = new(deck);

    public int DeckCount => Deck.Count;

    public int Draw()
    {
        return Deck.Dequeue();
    }

    public void Add(int card)
    {
        Deck.Enqueue(card);
    }

    public long CalculateScore()
    {
        var total = 0;
        while (Deck.TryDequeue(out var card))
        {
            total += card * (Deck.Count + 1);
        }

        return total;
    }

    public override string ToString()
    {
        return $"Player {Id}: [{string.Join(", ", Deck)}]";
    }
}