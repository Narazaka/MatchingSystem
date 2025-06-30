using Cyan.PlayerObjectPool;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MatchingPlayerRoom : CyanPlayerObjectPoolObject
    {
        [SerializeField] public MatchingManager Manager;
        [SerializeField] public Teleporter Teleporter;
        [SerializeField] public MatchingPlayer TemplateMatchingPlayer;

        public MatchingPlayer MatchingPlayer;

        public override void _OnOwnerSet()
        {
            MatchingPlayer = (MatchingPlayer)Networking.FindComponentInPlayerObjects(Owner, TemplateMatchingPlayer);
            ResetValues();
        }

        public override void _OnCleanup()
        {
            MatchingPlayer = null;
            ResetValues();
        }

        void ResetValues()
        {
            if (Networking.IsOwner(gameObject))
            {
                JoiningSessionId = -1;
                Matched = false;
                MatchedPlayerHash = 0;
                Remaining = false;
                RoomAndSpawnPoint = -1;
                RequestSerialization();
            }
        }

        [UdonSynced, FieldChangeCallback(nameof(JoiningSessionId))] short _joiningSessionId = -1;
        public short JoiningSessionId
        {
            get => _joiningSessionId;
            set
            {
                if (_joiningSessionId == value) return;
                _joiningSessionId = value;
                if (Owner != null && Owner.isLocal) OnStartSession();
            }
        }

        [UdonSynced] bool _matched;
        public bool Matched
        {
            get => _matched;
            private set => _matched = value;
        }
        bool prevMatched;
        [UdonSynced] uint MatchedPlayerHash;
        uint prevMatchedPlayerHash;
        [UdonSynced] bool _experiencedLoneliness;
        public bool ExperiencedLoneliness
        {
            get => _experiencedLoneliness;
            private set => _experiencedLoneliness = value;
        }

        [UdonSynced, NonSerialized] public bool Remaining;
        [UdonSynced] sbyte _roomAndSpawnPoint = -1;
        public sbyte RoomAndSpawnPoint
        {
            get => _roomAndSpawnPoint;
            set
            {
                _roomAndSpawnPoint = value;
            }
        }
        public int RoomId { get => RoomAndSpawnPoint >> 1; }
        int SpawnPointIndex { get => RoomAndSpawnPoint & 1; }
        public bool Joined { get => RoomAndSpawnPoint != -1; }

        public uint SelfPlayerHash { get => SimpleHash.FNV1a32String.ComputeHash(Owner.displayName); }

        /// <summary>
        /// by manager (owner)
        /// </summary>
        public void _SetRoom(sbyte roomAndSpawnPoint)
        {
            RoomAndSpawnPoint = roomAndSpawnPoint;
            Remaining = false;
            RequestSerialization();
        }

        /// <summary>
        /// by manager (owner)
        /// </summary>
        public void _SetRemaining()
        {
            Remaining = true;
            RequestSerialization();
        }

        /// <summary>
        /// by manager (owner)
        /// </summary>
        public void _SetMatch(uint matchedPlayerHash, bool matched)
        {
            Matched = matched;
            MatchedPlayerHash = matchedPlayerHash;
            ExperiencedLoneliness = ExperiencedLoneliness || !matched;
            TryAddMatchedPlayerHash();
            RequestSerialization();
        }

        /// <summary>
        /// by manager (owner)
        /// 
        /// call last
        /// </summary>
        public void _SetJoiningSessionId(short sessionId)
        {
            JoiningSessionId = sessionId;
            RequestSerialization();
        }

        /// <summary>
        /// by manager (owner)
        /// </summary>
        public void ResetLoneliness()
        {
            ExperiencedLoneliness = false;
            RequestSerialization();
        }

        void OnStartSession()
        {
            Logger.Log(nameof(MatchingPlayerRoom), nameof(OnStartSession), Owner, $"room=({RoomId})[{SpawnPointIndex}] {(Remaining ? "(Remaining)" : "")} ------------------------------");
            if (!Remaining)
            {
                MatchingPlayer._OnChangePair();
            }
            Teleport();
        }

        void Teleport()
        {
            if (RoomAndSpawnPoint < 0)
            {
                Teleporter.Respawn();
                return;
            }
            if (Remaining) return;
            Teleporter.TeleportTo(Manager.Rooms[RoomId], SpawnPointIndex);
        }

        void TryAddMatchedPlayerHash()
        {
            if (prevMatched != Matched || prevMatchedPlayerHash != MatchedPlayerHash)
            {
                prevMatched = Matched;
                prevMatchedPlayerHash = MatchedPlayerHash;
                if (Matched && Owner != null && Owner.isLocal) MatchingPlayer._AddMatchedPlayerHash(MatchedPlayerHash);
            }
        }

        public override void OnDeserialization()
        {
            TryAddMatchedPlayerHash();
        }
    }
}
