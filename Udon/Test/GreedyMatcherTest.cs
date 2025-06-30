using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Narazaka.VRChat.MatchingSystem
{
    public class GreedyMatcherTest : MonoBehaviour
    {
        [System.Serializable]
        public class StartData
        {
            public string displayName;
            public int[] matchedIndexes = new int[0];
        }
        public StartData[] initialPlayers;
        public bool useDisplayNames = false;
        public int sessionCount = 32;
        MatchingPlayerRoom[] matchingPlayerRooms;
        public void RunTest()
        {
            var players = initialPlayers.Select((s, i) => MakePlayer(useDisplayNames ? s.displayName : $"{i}")).ToArray();
            for (var i = 0; i < initialPlayers.Length; i++)
            {
                var initialPlayer = initialPlayers[i];
                if (initialPlayer.matchedIndexes.Length > 0)
                {
                    var matchedIndexes = initialPlayer.matchedIndexes;
                    for (var j = 0; j < matchedIndexes.Length; j++)
                    {
                        if (matchedIndexes[j] < 0 || matchedIndexes[j] >= players.Length)
                        {
                            Debug.LogError($"Invalid matched index {matchedIndexes[j]} for player {i}");
                            continue;
                        }
                        players[i].MatchingPlayer._AddMatchedPlayerHash(players[matchedIndexes[j]].SelfPlayerHash);
                    }
                }
            }
            var hashes = players.Select((p, i) => (p, i)).ToDictionary(a => a.p.SelfPlayerHash, a => a.i);
            var matchedlog = players.Select(p => new List<int>()).ToArray();
            for (var sessionIndex = 0; sessionIndex < sessionCount; sessionIndex++)
            {
                for (var j = 0; j < players.Length; j++)
                {
                    Debug.Log("hashm " + j + " [" + string.Join(", ", players[j].MatchingPlayer.MatchedPlayerHashes.Select(h => hashes[h])) + "]");
                }
                var result = GreedyMatcher.MakeMatching(players);
                var nomatch = players.Select((p, i) => i).ToHashSet();
                for (var j = 0; j < result.Length; j += 2)
                {
                    players[result[j]].MatchingPlayer._AddMatchedPlayerHash(players[result[j + 1]].SelfPlayerHash);
                    matchedlog[result[j]].Add(result[j + 1]);
                    players[result[j + 1]].MatchingPlayer._AddMatchedPlayerHash(players[result[j]].SelfPlayerHash);
                    matchedlog[result[j + 1]].Add(result[j]);
                    nomatch.Remove(result[j]);
                    nomatch.Remove(result[j + 1]);
                }
                foreach (var index in nomatch)
                {
                    matchedlog[index].Add(-1);
                }
                Debug.Log("Matching Result: " + string.Join(", ", result));
            }
            for (var j = 0; j < players.Length; j++)
            {
                var matchedCounts = new Dictionary<int, int>();
                var matchelogWithCounts = matchedlog[j].Select(pid =>
                {
                    if (pid == -1) return (-1, -1); // No match
                    if (matchedCounts.TryGetValue(pid, out var count))
                    {
                        matchedCounts[pid] = count + 1;
                        return (pid, count + 1);
                    }
                    else
                    {
                        matchedCounts[pid] = 1;
                        return (pid, 1);
                    }
                }).ToList();
                Debug.Log("matchedlog " + j + " [" + string.Join(", ", matchelogWithCounts.Select(a => a.Item1)) + "]");
                Debug.Log("matchedlog " + j + " [" + string.Join(", ", matchelogWithCounts.Select(a => a.Item2)) + "]");
            }
            foreach (var player in players)
            {
                DestroyImmediate(player.gameObject);
            }
        }

        MatchingPlayerRoom MakePlayer(string displayName)
        {
            var go = new GameObject("Player");
            var matchingPlayerRoom = go.AddComponent<MatchingPlayerRoom>();
            var matchingPlayer = go.AddComponent<MatchingPlayer>();
            var memory = go.AddComponent<MatchedPlayerMemory>();
            matchingPlayerRoom.Owner = new VRC.SDKBase.VRCPlayerApi { displayName = displayName };
            matchingPlayerRoom.MatchingPlayer = matchingPlayer;
            typeof(MatchingPlayer).GetField("MatchedPlayerMemory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(matchingPlayer, memory);
            return matchingPlayerRoom;
        }

#if UNITY_EDITOR
            [UnityEditor.CustomEditor(typeof(GreedyMatcherTest))]
        public class GreedyMatcherTestEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                if (GUILayout.Button("Run Test"))
                {
                    var matcher = target as GreedyMatcherTest;
                    matcher.RunTest();
                }
            }
        }
#endif
    }
}
