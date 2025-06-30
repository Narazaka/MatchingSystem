#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
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
        public int sessionCount = 32;
        public bool displayMatchedLogsByStep = false;
        public bool useDisplayNames = false;
        public StartData[] initialPlayers;
        public void RunTest()
        {
            var teleporter = MakeTeleporter();
            var manager = MakeManager();
            var matchingRooms = initialPlayers.Select(s => MakeMatchingRoom()).ToArray();
            manager.Rooms = matchingRooms;
            var players = initialPlayers.Select((s, i) => MakePlayer(useDisplayNames ? s.displayName : $"{i}", i + 1, manager, teleporter)).ToArray();
            manager.matchingPlayerRoomsForTest = players;
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
                    Debug.Log($"hashs {j} (exlonely={players[j].ExperiencedLoneliness})" + " [" + string.Join(", ", players[j].MatchingPlayer.MatchedPlayerHashes.Select(h => hashes[h])) + "]");
                }
                if (sessionIndex == 0)
                {
                    for (var j = 0; j < players.Length; j++)
                    {
                        manager._Join(players[j].Owner);
                    }
                }
                else
                {
                    manager._InitializeSession();
                }
                foreach (var pair in players.GroupBy(p => p.RoomId).Select(p => p.ToArray()).ToArray())
                {
                    if (pair.Length == 1)
                    {
                        matchedlog[pair[0].Owner.playerId - 1].Add(-1); // No match
                    }
                    else
                    {
                        matchedlog[pair[0].Owner.playerId - 1].Add(pair[1].Owner.playerId - 1);
                        matchedlog[pair[1].Owner.playerId - 1].Add(pair[0].Owner.playerId - 1);
                    }
                }
                if (displayMatchedLogsByStep)
                {
                    DisplayMatchedLogs(players, matchedlog);
                }
            }
            if (!displayMatchedLogsByStep)
            {
                DisplayMatchedLogs(players, matchedlog);
            }
            foreach (var player in players)
            {
                DestroyImmediate(player.gameObject);
            }
            foreach (var room in matchingRooms)
            {
                DestroyImmediate(room.gameObject);
            }
            DestroyImmediate(manager.gameObject);
            DestroyImmediate(teleporter.gameObject);
        }

        MatchingPlayerRoom MakePlayer(string displayName, int playerId, MatchingManager manager, Teleporter teleporter)
        {
            var go = new GameObject("Player");
            var matchingPlayerRoom = go.AddComponent<MatchingPlayerRoom>();
            var matchingPlayer = go.AddComponent<MatchingPlayer>();
            var memory = go.AddComponent<MatchedPlayerMemory>();
            var player = new VRC.SDKBase.VRCPlayerApi { displayName = displayName, isLocal = true };
            typeof(VRC.SDKBase.VRCPlayerApi).GetField("mPlayerId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(player, playerId);
            matchingPlayerRoom.Owner = player;
            matchingPlayerRoom.MatchingPlayer = matchingPlayer;
            matchingPlayerRoom.Teleporter = teleporter;
            matchingPlayerRoom.Manager = manager;
            typeof(MatchingPlayer).GetField("MatchedPlayerMemory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(matchingPlayer, memory);
            return matchingPlayerRoom;
        }

        MatchingRoom MakeMatchingRoom()
        {
            var go = new GameObject("MatchingRoom");
            var matchingRoom = go.AddComponent<MatchingRoom>();
            matchingRoom.SpawnPoints = new Transform[] {matchingRoom.transform, matchingRoom.transform };
            return matchingRoom;
        }

        MatchingManager MakeManager()
        {
            var go = new GameObject("MatchingManager");
            var matchingManager = go.AddComponent<MatchingManager>();
            return matchingManager;
        }

        Teleporter MakeTeleporter()
        {
            var go = new GameObject("Teleporter");
            var teleporter = go.AddComponent<Teleporter>();
            var fadeTeleporter = go.AddComponent<Narazaka.VRChat.FadeTeleport.FadeTeleporter>();
            typeof(Teleporter).GetField("FadeTeleporter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(teleporter, fadeTeleporter);
            return teleporter;
        }

        void DisplayMatchedLogs(MatchingPlayerRoom[] players, List<int>[] matchedlog)
        {
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
                Debug.Log("matchedlog " + j + " [" + string.Join(", ", matchelogWithCounts.Select(a => a.Item1).Select(i => i == -1 ? "_" : $"{i}")) + "]");
                Debug.Log("matchedcnt " + j + " [" + string.Join(", ", matchelogWithCounts.Select(a => a.Item2).Select(i => i == -1 ? "_" : $"{i}")) + "]");
            }
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
#endif
