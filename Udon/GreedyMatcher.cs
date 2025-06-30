// #define MATCHINGSYSTEM_DEBUG
using System;

namespace Narazaka.VRChat.MatchingSystem
{
    public class GreedyMatcher
    {
        /// <summary>
        /// Executes the greedy matching algorithm.
        /// </summary>
        /// <returns>2 * N array of player indices, where N is the number of pairs.</returns>
        public static int[] MakeMatching(MatchingPlayerRoom[] players)
        {
            Logger.Log(nameof(GreedyMatcher), nameof(MakeMatching), $"(Start) {players.Length} players");
            var playerCount = players.Length;

            if (playerCount < 2)
            {
                Logger.Log(nameof(GreedyMatcher), nameof(MakeMatching), $"(End) {players.Length} players (no match)");
                return new int[0];
            }

            var playerHashes = new uint[playerCount];
            sbyte maxConsecutiveMatchedCount = -1;
            for (int i = 0; i < playerCount; i++)
            {
                var player = players[i];
                playerHashes[i] = player.SelfPlayerHash;
                if (player.ConsecutiveMatchedCount > maxConsecutiveMatchedCount)
                {
                    maxConsecutiveMatchedCount = player.ConsecutiveMatchedCount;
                }
            }

            // 2. Create all possible pairs and store them in parallel arrays
            int pairCount = playerCount * (playerCount - 1) / 2;
            var pairWeights = new int[pairCount];
            var pairPlayerIndices1 = new int[pairCount];
            var pairPlayerIndices2 = new int[pairCount];
            int currentPairIndex = 0;

            var log = "";
            for (int i = 0; i < playerCount; i++)
            {
                for (int j = i + 1; j < playerCount; j++)
                {
                    // Invert weight to sort descending with an ascending sort algorithm
                    pairWeights[currentPairIndex] = -CalculateWeight(players[i], playerHashes[i], players[j], playerHashes[j], maxConsecutiveMatchedCount);
                    pairPlayerIndices1[currentPairIndex] = i;
                    pairPlayerIndices2[currentPairIndex] = j;
#if MATCHINGSYSTEM_DEBUG
                    log += $"({i} {j})<{pairWeights[currentPairIndex]}> ";
#endif

                    currentPairIndex++;
                }
            }
#if MATCHINGSYSTEM_DEBUG
            Logger.Log(nameof(GreedyMatcher), nameof(MakeMatching), "LIST    " + log);
#endif

            // 3. Sort pairs by inverted weight (ascending).
            // Create an array of indices [0, 1, 2, ...] to be sorted according to the weights.
            var pairOrder = new int[pairCount];
            for (int i = 0; i < pairCount; i++)
            {
                pairOrder[i] = i;
            }

            // Sort the pairOrder array based on the keys in pairWeights.
            Array.Sort((Array)pairWeights, (Array)pairOrder);

            // 4. Greedily select pairs using the sorted order
            var matchedPlayerIndices = new bool[playerCount];
            var finalMatchedPairs_Player1 = new int[playerCount / 2];
            var finalMatchedPairs_Player2 = new int[playerCount / 2];
            int finalPairCount = 0;

            log = "";
#if MATCHINGSYSTEM_DEBUG
            var log2 = "";
#endif
            for (int i = 0; i < pairCount; i++)
            {
                int originalPairIndex = pairOrder[i];
                int pIndex1 = pairPlayerIndices1[originalPairIndex];
                int pIndex2 = pairPlayerIndices2[originalPairIndex];
#if MATCHINGSYSTEM_DEBUG
                log2 += $"({pIndex1} {pIndex2})<{pairWeights[i]}> ";
#endif

                if (!matchedPlayerIndices[pIndex1] && !matchedPlayerIndices[pIndex2])
                {
                    log += $"({pIndex1} {pIndex2})<{pairWeights[i]}> ";
                    finalMatchedPairs_Player1[finalPairCount] = pIndex1;
                    finalMatchedPairs_Player2[finalPairCount] = pIndex2;
                    finalPairCount++;

                    matchedPlayerIndices[pIndex1] = true;
                    matchedPlayerIndices[pIndex2] = true;
                }
            }
#if MATCHINGSYSTEM_DEBUG
            Logger.Log(nameof(GreedyMatcher), nameof(MakeMatching), "SORTED  " + log2);
#endif
            Logger.Log(nameof(GreedyMatcher), nameof(MakeMatching), "MATCHED " + log);

            // 5. Store the results in a flattened array
            var resultMatchedPlayerIndexes = new int[finalPairCount * 2];
            for (int i = 0; i < finalPairCount; i++)
            {
                resultMatchedPlayerIndexes[i * 2] = finalMatchedPairs_Player1[i];
                resultMatchedPlayerIndexes[i * 2 + 1] = finalMatchedPairs_Player2[i];
            }
            Logger.Log(nameof(GreedyMatcher), nameof(MakeMatching), $"(End) {players.Length} players");
            return resultMatchedPlayerIndexes;
        }

        /// <summary>
        /// Calculates the weight between two players.
        /// 
        /// higher weight means better match.
        /// </summary>
        static int CalculateWeight(MatchingPlayerRoom player1, uint player1hash, MatchingPlayerRoom player2, uint player2hash, sbyte maxConsecutiveMatchedCount)
        {
            var score = 0;
            score += player1.ConsecutiveMatchedCount == maxConsecutiveMatchedCount ? -100000 : 0;
            score += player2.ConsecutiveMatchedCount == maxConsecutiveMatchedCount ? -100000 : 0;
            score -= Array.IndexOf(player1.MatchingPlayer.MatchedPlayerHashes, player2hash) * 100;
            score -= Array.IndexOf(player2.MatchingPlayer.MatchedPlayerHashes, player1hash) * 100;
            return score;
        }
    }
}
