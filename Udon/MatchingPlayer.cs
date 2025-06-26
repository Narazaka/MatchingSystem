using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MatchingPlayer : UdonSharpBehaviour
    {
        [SerializeField] MatchedPlayerMemory MatchedPlayerMemory;

        [UdonSynced] sbyte State;
        public bool ReserveLeave => State == PlayerState.ReserveLeave;
        public bool ReserveRemain => State == PlayerState.ReserveRemain;

        public uint[] MatchedPlayerHashes => MatchedPlayerMemory.MatchedPlayerHashes;

        public void _AddMatchedPlayerHash(uint matchedPlayerHash) => MatchedPlayerMemory._AddMatchedPlayer(matchedPlayerHash);

        public void _OnChangePair()
        {
            ResetState();
        }

        void ResetState()
        {
            State = PlayerState.Idle;
            RequestSerialization();
        }

        /// <summary>
        /// by ui
        /// </summary>
        public void _ToggleReserveLeave()
        {
            State = State == PlayerState.ReserveLeave ? PlayerState.Idle : PlayerState.ReserveLeave;
            RequestSerialization();
        }

        /// <summary>
        /// by ui
        /// </summary>
        public void _ToggleReserveRemain()
        {
            State = State == PlayerState.ReserveRemain ? PlayerState.Idle : PlayerState.ReserveRemain;
            RequestSerialization();
        }
    }
}
