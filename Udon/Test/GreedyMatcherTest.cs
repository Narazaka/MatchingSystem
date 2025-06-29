using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Narazaka.VRChat.MatchingSystem
{
    public class GreedyMatcherTest : MonoBehaviour
    {
        public int playerCount = 16;
        public int sessionCount = 32;
        MatchingPlayerRoom[] matchingPlayerRooms;
        public void RunTest()
        {
            var players = Enumerable.Range(0, playerCount)
                .Select(i => MakePlayer(i))
                .ToArray();
            var hashes = players.Select((p, i) => (p, i)).ToDictionary(a => a.p.SelfPlayerHash, a => a.i);
            var matchedlog = players.Select(p => new List<int>()).ToArray();
            for (var i = 0; i < sessionCount; i++)
            {
                for (var j = 0; j < players.Length; j++)
                {
                    Debug.Log("hashm " + j + " [" + string.Join(", ", players[j].MatchingPlayer.MatchedPlayerHashes.Select(h => hashes[h])) + "]");
                }
                var result = GreedyMatcher.MakeMatching(players);
                for (var j = 0; j < result.Length; j += 2)
                {
                    players[result[j]].MatchingPlayer._AddMatchedPlayerHash(players[result[j + 1]].SelfPlayerHash);
                    matchedlog[result[j]].Add(result[j + 1]);
                    players[result[j + 1]].MatchingPlayer._AddMatchedPlayerHash(players[result[j]].SelfPlayerHash);
                    matchedlog[result[j + 1]].Add(result[j]);
                }
                for (var j = 0; j < players.Length; j++)
                {
                    var matchedCounts = new Dictionary<int, int>();
                    var matchelogWithCounts = matchedlog[j].Select(pid =>
                    {
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
                Debug.Log("Matching Result: " + string.Join(", ", result));
            }
            foreach (var player in players)
            {
                DestroyImmediate(player.gameObject);
            }
        }

        MatchingPlayerRoom MakePlayer(int i)
        {
            var go = new GameObject("Player");
            var matchingPlayerRoom = go.AddComponent<MatchingPlayerRoom>();
            var matchingPlayer = go.AddComponent<MatchingPlayer>();
            var memory = go.AddComponent<MatchedPlayerMemory>();
            matchingPlayerRoom.Owner = new VRC.SDKBase.VRCPlayerApi { displayName = $"{i}" };
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
