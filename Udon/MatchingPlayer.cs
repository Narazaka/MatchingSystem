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
        internal bool ReserveLeave => State == PlayerState.ReserveLeave;
        internal bool ReserveRemain => State == PlayerState.ReserveRemain;

        internal uint[] MatchedPlayerHashes => MatchedPlayerMemory.MatchedPlayerHashes;

        internal void _AddMatchedPlayerHash(uint matchedPlayerHash) => MatchedPlayerMemory._AddMatchedPlayer(matchedPlayerHash);

        internal void _OnChangePair()
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
        internal void _ToggleReserveLeave()
        {
            State = State == PlayerState.ReserveLeave ? PlayerState.Idle : PlayerState.ReserveLeave;
            RequestSerialization();
        }

        /// <summary>
        /// by ui
        /// </summary>
        internal void _ToggleReserveRemain()
        {
            State = State == PlayerState.ReserveRemain ? PlayerState.Idle : PlayerState.ReserveRemain;
            RequestSerialization();
        }
    }
}
