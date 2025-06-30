using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MatchedPlayerMemory : UdonSharpBehaviour
    {
        [SerializeField] int MaxMemoryCount = 20;
        // [new, ..., old]
        [UdonSynced] uint[] MatchedPlayerHashes = new uint[0];

        public void _AddMatchedPlayer(uint matchedPlayerHash)
        {
            var index = Array.IndexOf(MatchedPlayerHashes, matchedPlayerHash);
            if (index < 0)
            {
                // add
                uint[] matchedPlayerHashes;
                if (MatchedPlayerHashes.Length >= MaxMemoryCount)
                {
                    // if the memory is full, remove the oldest(first, second, ...) matched player hash
                    matchedPlayerHashes = new uint[MaxMemoryCount];
                    Array.Copy(MatchedPlayerHashes, 0, matchedPlayerHashes, 1, MaxMemoryCount - 1);
                }
                else
                {
                    matchedPlayerHashes = new uint[MatchedPlayerHashes.Length + 1];
                    Array.Copy(MatchedPlayerHashes, 0, matchedPlayerHashes, 1, MatchedPlayerHashes.Length);
                }
                matchedPlayerHashes[0] = matchedPlayerHash;
                MatchedPlayerHashes = matchedPlayerHashes;
            }
            else
            {
                // reorder the matched player hash to the end of the array
                var matchedPlayerHashes = new uint[MatchedPlayerHashes.Length];
                Array.Copy(MatchedPlayerHashes, 0, matchedPlayerHashes, 1, index);
                Array.Copy(MatchedPlayerHashes, index + 1, matchedPlayerHashes, index + 1, MatchedPlayerHashes.Length - index - 1);
                matchedPlayerHashes[0] = matchedPlayerHash;
                MatchedPlayerHashes = matchedPlayerHashes;
            }
            RequestSerialization();
        }

        public int _AlreadyMatchedRate(uint playerHash)
        {
            if (MatchedPlayerHashes.Length == 0) return 0;
            var index = Array.LastIndexOf(MatchedPlayerHashes, playerHash);
            if (index < 0) return 0;
            return MaxMemoryCount - index;
        }
    }
}
