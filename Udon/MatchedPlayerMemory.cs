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
        [UdonSynced] public uint[] MatchedPlayerHashes = new uint[0];

        internal void AddMatchedPlayer(uint matchedPlayerHash)
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
                    Array.Copy(MatchedPlayerHashes, 1 + MatchedPlayerHashes.Length - MaxMemoryCount, matchedPlayerHashes, 0, MaxMemoryCount - 1);
                }
                else
                {
                    matchedPlayerHashes = new uint[MatchedPlayerHashes.Length + 1];
                    Array.Copy(MatchedPlayerHashes, matchedPlayerHashes, MatchedPlayerHashes.Length);
                }
                matchedPlayerHashes[matchedPlayerHashes.Length - 1] = matchedPlayerHash;
                MatchedPlayerHashes = matchedPlayerHashes;
            }
            else
            {
                // reorder the matched player hash to the end of the array
                var matchedPlayerHashes = new uint[MatchedPlayerHashes.Length];
                Array.Copy(MatchedPlayerHashes, matchedPlayerHashes, index);
                Array.Copy(MatchedPlayerHashes, index + 1, matchedPlayerHashes, index, MatchedPlayerHashes.Length - index - 1);
                matchedPlayerHashes[matchedPlayerHashes.Length - 1] = matchedPlayerHash;
                MatchedPlayerHashes = matchedPlayerHashes;
            }
            RequestSerialization();
        }
    }
}
