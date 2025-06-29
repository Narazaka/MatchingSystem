using Cyan.PlayerObjectPool;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MatchingManager : UdonSharpBehaviour
    {
        [SerializeField] public CyanPlayerObjectAssigner Assigner;
        [SerializeField] public MatchingRoom[] Rooms;

        [UdonSynced] long SessionStartTimeTick;
        public DateTime SessionStartTime
        {
            get => new DateTime(SessionStartTimeTick);
            set
            {
                SessionStartTimeTick = value.Ticks;
            }
        }
        [UdonSynced, FieldChangeCallback(nameof(SessionId))] short _sessionId = -1; // 68 days (by 3min)
        public short SessionId
        {
            get => _sessionId;
            set
            {
                if (_sessionId == value) return;
                _sessionId = value;
                CallResetRooms();
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!IsOwner) return;
            _Leave(player);
        }

        /// <summary>
        /// by ui udon (owner)
        /// </summary>
        public void _Join(VRCPlayerApi player)
        {
            Logger.Log(nameof(MatchingManager), nameof(_Join), player);
            if (SessionId == -1)
            {
                Logger.Log(nameof(MatchingManager), nameof(_Join), player, "START FIRST SESSION");
                InitializeSession();
            }
            var subjectPlayerRoom = GetMatchingPlayerRoom(player);

            var playerRoomComponents = GetMatchingPlayerRooms();
            var len = playerRoomComponents.Length;
            var roomLen = Rooms.Length;

            var playerCountsByRoom = new int[roomLen];
            var indexByRoom = new int[roomLen * 2];
            for (var i = 0; i < len; i++)
            {
                var playerRoom = (MatchingPlayerRoom)playerRoomComponents[i];
                if (playerRoom.Joined)
                {
                    playerCountsByRoom[playerRoom.RoomId]++;
                    indexByRoom[playerRoom.RoomAndSpawnPoint] = i + 1;
                }
            }
            var oneRoomIndex = Array.IndexOf(playerCountsByRoom, 1);
            if (oneRoomIndex >= 0)
            {
                var spawnIndex = indexByRoom[oneRoomIndex * 2] == 0 ? 0 : 1;
                var otherIndex = indexByRoom[oneRoomIndex * 2 + (spawnIndex == 0 ? 1 : 0)];
                var otherPlayerRoom = (MatchingPlayerRoom)playerRoomComponents[otherIndex - 1];
                subjectPlayerRoom._SetRoom((sbyte)(oneRoomIndex * 2 + spawnIndex));
                subjectPlayerRoom._SetMatch(otherPlayerRoom.SelfPlayerHash, true);
                subjectPlayerRoom._SetJoiningSessionId(SessionId);
                otherPlayerRoom._SetMatch(subjectPlayerRoom.SelfPlayerHash, true);
                Logger.Log(nameof(MatchingManager), nameof(_Join), player, $"room=({oneRoomIndex}){(spawnIndex == 0 ? "a" : "b")} (TWO) with [{Logger.Player(otherPlayerRoom.Owner)}]");
            }
            else
            {
                var zeroRoomIndex = Array.IndexOf(playerCountsByRoom, 0);
                if (zeroRoomIndex >= 0)
                {
                    subjectPlayerRoom._SetRoom((sbyte)(zeroRoomIndex * 2));
                    subjectPlayerRoom._SetJoiningSessionId(SessionId);
                    Logger.Log(nameof(MatchingManager), nameof(_Join), player, $"room=({zeroRoomIndex})a (ONLYONE)");
                }
                else
                {
                    // no room available!!!!
                    subjectPlayerRoom._SetRoom(-1);
                    subjectPlayerRoom._SetJoiningSessionId(-1);
                    Logger.Log(nameof(MatchingManager), nameof(_Join), player, "FAILED!");
                }
            }
        }

        /// <summary>
        /// by OnPlayerLeft
        /// by spawn area collider trigger udon (owner)
        /// </summary>
        /// <param name="player"></param>
        public void _Leave(VRCPlayerApi player)
        {
            Logger.Log(nameof(MatchingManager), nameof(_Leave), player);
            var subjectPlayerRoom = GetMatchingPlayerRoom(player);
            if (subjectPlayerRoom == null)
            {
                Logger.Log(nameof(MatchingManager), nameof(_Leave), player, "not found MatchingPlayerRoom");
                return;
            }
            subjectPlayerRoom._SetRoom(-1);
            subjectPlayerRoom._SetJoiningSessionId(-1);
        }

        /// <summary>
        /// by owner (timing)
        /// </summary>
        public void _InitializeSession()
        {
            InitializeSession();
        }

        void InitializeSession()
        {
            Logger.Log(nameof(MatchingManager), nameof(InitializeSession), "(Start)==============================");
            var nextSessionId = (short)(SessionId + 1);

            // shuffle
            var playerRoomComponents = GetMatchingPlayerRooms();
            var len = playerRoomComponents.Length;
            var roomLen = Rooms.Length;
            var playerRooms = new MatchingPlayerRoom[len];

            var leaves = new bool[len];
            var remainCountsByRoom = new int[roomLen];
            var remainIndexByRoom = new int[roomLen * 2];
            for (var i = 0; i < len; i++)
            {
                var playerRoom = playerRooms[i] = (MatchingPlayerRoom)playerRoomComponents[i];
                if (playerRoom.MatchingPlayer.ReserveRemain)
                {
                    remainCountsByRoom[playerRoom.RoomId]++;
                    remainIndexByRoom[playerRoom.RoomAndSpawnPoint] = i;
                }
                else if (playerRoom.MatchingPlayer.ReserveLeave)
                {
                    leaves[i] = true;
                }
            }
            var remains = new bool[len];
            for (var i = 0; i < roomLen; ++i)
            {
                var remainCount = remainCountsByRoom[i];
                if (remainCount == 2)
                {
                    remains[remainIndexByRoom[i * 2]] = true;
                    remains[remainIndexByRoom[i * 2 + 1]] = true;
                }
            }

            var shufflePlayerRooms = new MatchingPlayerRoom[len];
            var shuffleLen = 0;
            for (var i = 0; i < len; i++)
            {
                var playerRoom = playerRooms[i];
                if (leaves[i])
                {
                    playerRoom._SetRoom(-1);
                    playerRoom._SetJoiningSessionId(-1);
                    Logger.Log(nameof(MatchingManager), nameof(InitializeSession) + "(Prepare)", playerRoom.Owner, "leave");
                }
                else if (remains[i])
                {
                    playerRoom._SetRemaining();
                    playerRoom._SetJoiningSessionId(nextSessionId);
                    Logger.Log(nameof(MatchingManager), nameof(InitializeSession) + "(Prepare)", playerRoom.Owner, $"remain room=({playerRoom.RoomId})");
                }
                else if (playerRoom.Joined)
                {
                    shufflePlayerRooms[shuffleLen] = playerRoom;
                    shuffleLen++;
                    Logger.Log(nameof(MatchingManager), nameof(InitializeSession) + "(Prepare)", playerRoom.Owner, "shuffle");
                }
                else
                {
                    // same value: not joined
                    Logger.Log(nameof(MatchingManager), nameof(InitializeSession) + "(Prepare)", playerRoom.Owner, "none");
                }
            }
            var shufflePlayerRoomsResized = new MatchingPlayerRoom[shuffleLen];
            Array.Copy(shufflePlayerRooms, shufflePlayerRoomsResized, shuffleLen);
            var matchedPlayerRoomIndexes = GreedyMatcher.MakeMatching(shufflePlayerRoomsResized);
            var matched = new bool[shuffleLen];
            var roomIndex = 0;
            for (var i = 0; i < matchedPlayerRoomIndexes.Length; i += 2)
            {
                var matchedIndex1 = matchedPlayerRoomIndexes[i];
                var matchedIndex2 = matchedPlayerRoomIndexes[i + 1];
                var playerRoom1 = shufflePlayerRoomsResized[matchedIndex1];
                var playerRoom2 = shufflePlayerRoomsResized[matchedIndex2];
                playerRoom1._SetRoom((sbyte)(roomIndex * 2));
                playerRoom1._SetMatch(playerRoom2.SelfPlayerHash, true);
                playerRoom1._SetJoiningSessionId(nextSessionId);
                playerRoom2._SetRoom((sbyte)(roomIndex * 2 + 1));
                playerRoom2._SetMatch(playerRoom1.SelfPlayerHash, true);
                playerRoom2._SetJoiningSessionId(nextSessionId);
                Logger.Log(nameof(MatchingManager), nameof(InitializeSession) + "(Result)", playerRoom1.Owner, $"room=({roomIndex})a");
                Logger.Log(nameof(MatchingManager), nameof(InitializeSession) + "(Result)", playerRoom2.Owner, $"room=({roomIndex})b");
                matched[matchedIndex1] = true;
                matched[matchedIndex2] = true;
                roomIndex++;
            }
            var unmatchedIndex = Array.IndexOf(matched, false);
            if (unmatchedIndex >= 0)
            {
                var unmatchedPlayerRoom = shufflePlayerRoomsResized[unmatchedIndex];
                unmatchedPlayerRoom._SetRoom((sbyte)(roomIndex * 2));
                unmatchedPlayerRoom._SetJoiningSessionId(nextSessionId);
                Logger.Log(nameof(MatchingManager), nameof(InitializeSession) + "(Result)", unmatchedPlayerRoom.Owner, $"room=({roomIndex})a (unmatched)");
            }

            SessionId = nextSessionId;
            SessionStartTime = Networking.GetNetworkDateTime();
            Logger.Log(nameof(MatchingManager), nameof(InitializeSession), $"(End) SessionId={SessionId} shuffleLen={shuffleLen}");
            RequestSerialization();
        }

        void CallResetRooms()
        {
            SendCustomEventDelayedFrames(nameof(ResetRooms), 1);
        }

        void ResetRooms()
        {
            var playerRoomComponents = GetMatchingPlayerRooms();
            var len = playerRoomComponents.Length;
            var roomLen = Rooms.Length;
            var remainRooms = new bool[roomLen];
            for (var i = 0; i < len; i++)
            {
                var playerRoom = (MatchingPlayerRoom)playerRoomComponents[i];
                if (playerRoom.Remaining)
                {
                    remainRooms[playerRoom.RoomId] = true;
                }
            }
            for (var i = 0; i < roomLen; i++)
            {
                if (!remainRooms[i])
                {
                    Logger.Log(nameof(MatchingManager), nameof(ResetRooms), $"({i})");
                    Rooms[i]._ResetRoom(Networking.GetOwner(gameObject));
                }
            }
        }

        bool IsOwner { get => Networking.IsOwner(gameObject); }

        Component[] GetMatchingPlayerRooms() => Assigner._GetActivePoolObjects();

        MatchingPlayerRoom GetMatchingPlayerRoom(VRCPlayerApi player) => (MatchingPlayerRoom)Assigner._GetPlayerPooledUdon(player);
    }
}
