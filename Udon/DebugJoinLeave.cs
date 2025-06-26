using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    public class DebugJoinLeave : UdonSharpBehaviour
    {
        [SerializeField] MatchingManager MatchingManager;
        [SerializeField] CyanPlayerObjectAssigner Assigner;
        public int playerId;
        public int[] playerIds = new int[0];

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            var newPlayerIds = new int[playerIds.Length + 1];
            for (var i = 0; i < playerIds.Length; i++)
            {
                newPlayerIds[i] = playerIds[i];
            }
            newPlayerIds[newPlayerIds.Length - 1] = player.playerId;
            playerIds = newPlayerIds;
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            var newPlayerIds = new int[playerIds.Length - 1];
            var index = 0;
            for (var i = 0; i < playerIds.Length; i++)
            {
                if (playerIds[i] == player.playerId) continue;
                newPlayerIds[index++] = playerIds[i];
            }
            playerIds = newPlayerIds;
        }

        public void Join()
        {
            var player = Player();
            if (player != null) MatchingManager._Join(player);
        }

        public void Leave()
        {
            var player = Player();
            if (player != null) MatchingManager._Leave(player);
        }

        public void ToggleReserveRemain()
        {
            var player = Player();
            if (player != null)
            {
                var pr = (MatchingPlayerRoom)Assigner._GetPlayerPooledUdonById(playerId);
                if (pr != null) pr.MatchingPlayer._ToggleReserveRemain();
            }
        }

        public void ToggleReserveLeave()
        {
            var player = Player();
            if (player != null)
            {
                var pr = (MatchingPlayerRoom)Assigner._GetPlayerPooledUdonById(playerId);
                if (pr != null) pr.MatchingPlayer._ToggleReserveLeave();
            }
        }

        VRCPlayerApi Player()
        {
            var player = VRCPlayerApi.GetPlayerById(playerId);
            if (player == null || !player.IsValid())
            {
                return null;
            }
            return player;
        }
    }
}
