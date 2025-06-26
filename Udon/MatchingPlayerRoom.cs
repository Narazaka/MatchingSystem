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

        internal MatchingPlayer MatchingPlayer;

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
        internal short JoiningSessionId
        {
            get => _joiningSessionId;
            set
            {
                if (_joiningSessionId == value) return;
                _joiningSessionId = value;
                if (Owner != null && Owner.isLocal) OnStartSession();
            }
        }

        [UdonSynced] bool Matched;
        bool prevMatched;
        [UdonSynced] uint MatchedPlayerHash;
        uint prevMatchedPlayerHash;

        [UdonSynced, NonSerialized] internal bool Remaining;
        [UdonSynced] sbyte _roomAndSpawnPoint = -1;
        internal sbyte RoomAndSpawnPoint
        {
            get => _roomAndSpawnPoint;
            set
            {
                _roomAndSpawnPoint = value;
            }
        }
        internal int RoomId { get => RoomAndSpawnPoint >> 1; }
        int SpawnPointIndex { get => RoomAndSpawnPoint & 1; }
        internal bool Joined { get => RoomAndSpawnPoint != -1; }

        internal uint SelfPlayerHash { get => SimpleHash.FNV1a32String.ComputeHash(Owner.displayName); }

        /// <summary>
        /// by manager (owner)
        /// </summary>
        internal void _SetRoom(sbyte roomAndSpawnPoint)
        {
            RoomAndSpawnPoint = roomAndSpawnPoint;
            Remaining = false;
            RequestSerialization();
        }

        /// <summary>
        /// by manager (owner)
        /// </summary>
        internal void _SetRemaining()
        {
            Remaining = true;
            RequestSerialization();
        }

        /// <summary>
        /// by manager (owner)
        /// </summary>
        internal void _SetMatch(uint matchedPlayerHash, bool matched)
        {
            Matched = matched;
            MatchedPlayerHash = matchedPlayerHash;
            TryAddMatchedPlayerHash();
            RequestSerialization();
        }

        /// <summary>
        /// by manager (owner)
        /// 
        /// call last
        /// </summary>
        internal void _SetJoiningSessionId(short sessionId)
        {
            JoiningSessionId = sessionId;
            RequestSerialization();
        }

        void OnStartSession()
        {
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
