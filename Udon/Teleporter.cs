using Narazaka.VRChat.FadeTeleport;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Teleporter : UdonSharpBehaviour
    {
        [SerializeField] FadeTeleporter FadeTeleporter;
        [SerializeField] Transform Control;
        [SerializeField] Transform Information;

        internal void Respawn()
        {
            FadeTeleporter.ReserveRespawn();
        }

        internal void TeleportTo(MatchingRoom room, int spawnPointIndex)
        {
            var spawnPoint = room.SpawnPoints[spawnPointIndex];
            FadeTeleporter.ReserveTeleportTo(spawnPoint.position, spawnPoint.rotation);
            Control.position = room.ControlPosition.position;
            Control.rotation = room.ControlPosition.rotation;
            if (Information == null) return;
            Information.position = room.InformationPosition.position;
            Information.rotation = room.InformationPosition.rotation;
        }
    }
}
